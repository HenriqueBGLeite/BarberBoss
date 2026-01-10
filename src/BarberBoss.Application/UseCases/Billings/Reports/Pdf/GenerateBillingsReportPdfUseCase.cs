using BarberBoss.Application.UseCases.Billings.Reports.Pdf.Colors;
using BarberBoss.Application.UseCases.Billings.Reports.Pdf.Fonts;
using BarberBoss.Domain.Extensions;
using BarberBoss.Domain.Reports;
using BarberBoss.Domain.Repositories.Billings;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using PdfSharp.Fonts;
using System.Reflection;

namespace BarberBoss.Application.UseCases.Billings.Reports.Pdf;

public class GenerateBillingsReportPdfUseCase : IGenerateBillingsReportPdfUseCase
{
    private const string CURRENCY_SYMBOL = "R$";
    private const int HEIGHT_ROW_EXPENSE_TABLE = 25;
    private readonly IBillingsReadOnlyRepository _repository;

    public GenerateBillingsReportPdfUseCase(IBillingsReadOnlyRepository repository)
    {
        _repository = repository;

        GlobalFontSettings.FontResolver = new BillingReportFontResolver();
    }

    public async Task<byte[]> Execute(DateOnly month)
    {
        var billings = await _repository.FilterByMonth(month);
        if (billings.Count == 0)
            return [];

        var barbers = billings.Select(b => b.BarberName).Distinct().ToList();

        var document = CreateDocument(month);

        foreach (var barberName in barbers)
        {
            var filteredBillings = billings.Where(billing => billing.BarberName == barberName).ToList();
            var page = CreatePage(document);

            CreateHeaderWithProfilePhotoAndBarberName(page, barberName);

            var totalBillings = filteredBillings.Sum(billing => billing.Amount);

            CreateTotalSpentSection(page, month, totalBillings);

            foreach (var billing in filteredBillings)
            {
                var table = CreateExpenseTable(page);
                
                var row = table.AddRow();
                row.Height = HEIGHT_ROW_EXPENSE_TABLE;

                AddBillingService(row.Cells[0], billing.ServiceName);
                AddHeaderForAmount(row.Cells[3]);

                row = table.AddRow();
                row.Height = HEIGHT_ROW_EXPENSE_TABLE;

                row.Cells[0].AddParagraph(billing.Date.ToLongDateNoDayOfWeek());
                SetStyleBaseForExpenseInformation(row.Cells[0]);
                row.Cells[0].Format.LeftIndent = 9;

                row.Cells[1].AddParagraph(billing.Date.ToString("t"));
                SetStyleBaseForExpenseInformation(row.Cells[1]);

                row.Cells[2].AddParagraph(billing.PaymentMethod.PaymentMethodToString());
                SetStyleBaseForExpenseInformation(row.Cells[2]);

                AddAmountForBilling(row.Cells[3], billing.Amount);

                if (string.IsNullOrWhiteSpace(billing.Notes) == false)
                {
                    var noteRow = table.AddRow();
                    noteRow.Height = HEIGHT_ROW_EXPENSE_TABLE;

                    noteRow.Cells[0].AddParagraph(billing.Notes);
                    noteRow.Cells[0].Format.Font = new Font { Name = FontHelper.ROBOTO_REGULAR, Size = 9, Color = ColorsHelper.GRAY_MEDIUM };
                    noteRow.Cells[0].Shading.Color = ColorsHelper.GRAY_LIGHT;
                    noteRow.Cells[0].VerticalAlignment = VerticalAlignment.Center;
                    noteRow.Cells[0].MergeRight = 2;
                    noteRow.Cells[0].Format.LeftIndent = 7;

                    row.Cells[3].MergeDown = 1;
                }

                AddWhiteSpace(table);
            }
        }

        return RenderDocument(document);
    }

    private Document CreateDocument(DateOnly month)
    {
        var document = new Document();

        document.Info.Title = $"{ResourceReportGenerationMessages.BILLING_AT_MONTH} {month:Y}";
        document.Info.Author = "Henrique Batista";

        var style = document.Styles["Normal"];
        style!.Font.Name = FontHelper.BEBASNEUE_REGULAR;

        return document;
    }

    private Section CreatePage(Document document)
    {
        var section = document.AddSection();
        section.PageSetup = document.DefaultPageSetup.Clone();

        section.PageSetup.PageFormat = PageFormat.A4;

        section.PageSetup.LeftMargin = 35;
        section.PageSetup.RightMargin = 35;
        section.PageSetup.TopMargin = 53;
        section.PageSetup.BottomMargin = 53;

        return section;
    }

    private void CreateHeaderWithProfilePhotoAndBarberName(Section page, string barberName)
    {
        var table = page.AddTable();
        table.AddColumn();
        table.AddColumn("400");

        var row = table.AddRow();

        var assembly = Assembly.GetExecutingAssembly();
        var directoryName = Path.GetDirectoryName(assembly.Location);
        var pathFile = Path.Combine(directoryName!, "Logo", "profile.png");

        row.Cells[0].AddImage(pathFile);
        row.Cells[0].Format.LeftIndent = 5;

        row.Cells[1].AddParagraph(barberName);
        row.Cells[1].Format.Font = new Font { Name = FontHelper.BEBASNEUE_REGULAR, Size = 25 };
        row.Cells[1].VerticalAlignment = VerticalAlignment.Center;
        row.Cells[1].Format.LeftIndent = 18;
    }

    private void CreateTotalSpentSection(Section page, DateOnly month, decimal totalBillings)
    {
        var paragraph = page.AddParagraph();
        paragraph.Format.SpaceBefore = "38";
        paragraph.Format.SpaceAfter = "64";

        paragraph.AddFormattedText(ResourceReportGenerationMessages.WEEKLY_BILLING, new Font { Name = FontHelper.ROBOTO_MEDIUM, Size = 15 });
        
        paragraph.AddLineBreak();

        paragraph.AddFormattedText($"{CURRENCY_SYMBOL} {totalBillings}", new Font { Name = FontHelper.BEBASNEUE_REGULAR, Size = 50 });
    }

    private Table CreateExpenseTable(Section page)
    {
        var table = page.AddTable();

        table.AddColumn("180").Format.Alignment = ParagraphAlignment.Left;
        table.AddColumn("80").Format.Alignment = ParagraphAlignment.Center;
        table.AddColumn("150").Format.Alignment = ParagraphAlignment.Center;
        table.AddColumn("100").Format.Alignment = ParagraphAlignment.Right;

        return table;
    }

    private void AddBillingService(Cell cell, string serviceName)
    {
        cell.AddParagraph(serviceName);
        cell.Format.Font = new Font { Name = FontHelper.BEBASNEUE_REGULAR, Size = 15, Color = ColorsHelper.WHITE };
        cell.Shading.Color = ColorsHelper.GREEN_DARK;
        cell.VerticalAlignment = VerticalAlignment.Center;
        cell.MergeRight = 2;
        cell.Format.LeftIndent = 7;
    }

    private void AddHeaderForAmount(Cell cell)
    {
        cell.AddParagraph(ResourceReportGenerationMessages.AMOUNT);
        cell.Format.Font = new Font { Name = FontHelper.BEBASNEUE_REGULAR, Size = 15, Color = ColorsHelper.WHITE };
        cell.Shading.Color = ColorsHelper.GREEN_LIGHT;
        cell.VerticalAlignment = VerticalAlignment.Center;
    }

    private void AddAmountForBilling(Cell cell, decimal amount)
    {
        cell.AddParagraph($"-{CURRENCY_SYMBOL} {amount}");
        cell.Format.Font = new Font { Name = FontHelper.ROBOTO_REGULAR, Size = 10 };
        cell.Shading.Color = ColorsHelper.WHITE;
        cell.VerticalAlignment = VerticalAlignment.Center;
    }

    private void SetStyleBaseForExpenseInformation(Cell cell)
    {
        cell.Format.Font = new Font { Name = FontHelper.ROBOTO_REGULAR, Size = 10, Color = ColorsHelper.BLACK };
        cell.Shading.Color = ColorsHelper.GRAY;
        cell.VerticalAlignment = VerticalAlignment.Center;
    }

    private void AddWhiteSpace(Table table)
    {
        var row = table.AddRow();
        row.Height = 16;
        row.Borders.Visible = false;
    }

    private byte[] RenderDocument(Document document)
    {
        var renderer = new PdfDocumentRenderer
        {
            Document = document
        };

        renderer.RenderDocument();

        using var file = new MemoryStream();
        renderer.PdfDocument.Save(file);

        return file.ToArray();
    }
}

namespace BarberBoss.Application.UseCases.Billings.Reports.Excel;

public interface IGenerateBillingReportExcelUseCase
{
    Task<byte[]> Execute(DateOnly date);
}

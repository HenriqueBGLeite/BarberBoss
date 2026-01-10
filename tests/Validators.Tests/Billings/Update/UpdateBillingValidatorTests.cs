using BarberBoss.Application.UseCases.Billings.Update;
using BarberBoss.Communication.Enums;
using BarberBoss.Exception;
using CommonTestsUtilities.Requests;
using Shouldly;

namespace Validators.Tests.Billings.Update;

public class UpdateBillingValidatorTests
{
    [Fact]
    public void Success()
    {
        var validator = new BillingUpdateValidator();
        var request = RequestUpdateExpenseJsonBuilder.Build();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Error_Date_Empty()
    {
        var validator = new BillingUpdateValidator();
        var request = RequestUpdateExpenseJsonBuilder.Build();
        request.Date = default;

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(error => error.ErrorMessage.Equals(ResourceErrorMessages.BILLING_REQUIRED));
        });
    }

    [Theory]
    [InlineData("B")]
    [InlineData("Este nome de barbeiro é propositalmente muito longo para passar dos oitenta caracteres permitidos pela regra")]
    public void Error_Barber_Name_Invalid(string barberName)
    {
        var validator = new BillingUpdateValidator();
        var request = RequestUpdateExpenseJsonBuilder.Build();
        request.BarberName = barberName;

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(error => error.ErrorMessage.Equals(ResourceErrorMessages.BARBER_NAME_LENGTH_BETWEEN_2_AND_80));
        });
    }

    [Theory]
    [InlineData("B")]
    [InlineData("Este nome do serviço é propositalmente muito longo para passar dos oitenta caracteres permitidos pela regra que o valida.")]
    public void Error_Service_Name_Invalid(string serviceName)
    {
        var validator = new BillingUpdateValidator();
        var request = RequestUpdateExpenseJsonBuilder.Build();
        request.ServiceName = serviceName;

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(error => error.ErrorMessage.Equals(ResourceErrorMessages.SERVICE_NAME_LENGTH_BETWEEN_2_AND_120));
        });
    }

    [Theory]
    [InlineData("B")]
    [InlineData("Este nome do cliente é propositalmente muito longo para passar dos oitenta caracteres permitidos pela regra que o valida.")]
    public void Error_Client_Name_Invalid(string clientName)
    {
        var validator = new BillingUpdateValidator();
        var request = RequestUpdateExpenseJsonBuilder.Build();
        request.ClientName = clientName;

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(error => error.ErrorMessage.Equals(ResourceErrorMessages.CLIENT_NAME_LENGTH_BETWEEN_2_AND_120));
        });
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Error_Amount_Invalid(decimal amount)
    {
        var validator = new BillingUpdateValidator();
        var request = RequestUpdateExpenseJsonBuilder.Build();
        request.Amount = amount;

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(error => error.ErrorMessage.Equals(ResourceErrorMessages.AMOUNT_MUST_BE_GREATER_THAN_ZERO));
        });
    }

    [Fact]
    public void Error_Payment_Method_Invalid()
    {
        var validator = new BillingUpdateValidator();
        var request = RequestUpdateExpenseJsonBuilder.Build();
        request.PaymentMethod = (PaymentMethod)700;

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(error => error.ErrorMessage.Equals(ResourceErrorMessages.PAYMENT_METHOD_INVALID));
        });
    }

    [Fact]
    public void Error_Payment_Status_Invalid()
    {
        var validator = new BillingUpdateValidator();
        var request = RequestUpdateExpenseJsonBuilder.Build();
        request.Status = (PaymentStatus)700;

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(error => error.ErrorMessage.Equals(ResourceErrorMessages.PAYMENT_STATUS_INVALID));
        });
    }

    [Fact]
    public void Error_Notes_Invalid()
    {
        var notes = new string('a', 501);
        var validator = new BillingUpdateValidator();
        var request = RequestUpdateExpenseJsonBuilder.Build();
        request.Notes = notes;

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(error => error.ErrorMessage.Equals(ResourceErrorMessages.NOTES_MUST_BE_LESS_THAN_500_CHARACTERS));
        });
    }
}

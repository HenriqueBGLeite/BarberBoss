using BarberBoss.Communication.Requests;
using BarberBoss.Exception;
using FluentValidation;

namespace BarberBoss.Application.UseCases.Billings.Register;

public class BillingRegisterValidator : AbstractValidator<RequestBillingJson>
{
    public BillingRegisterValidator()
    {
        RuleFor(billing => billing.Date).NotEmpty().WithMessage(ResourceErrorMessages.BILLING_REQUIRED);
        RuleFor(billing => billing.BarberName).Length(2, 80).WithMessage(ResourceErrorMessages.BARBER_NAME_LENGTH_BETWEEN_2_AND_80);
        RuleFor(billing => billing.ClientName).Length(2, 120).WithMessage(ResourceErrorMessages.CLIENT_NAME_LENGTH_BETWEEN_2_AND_120);
        RuleFor(billing => billing.ServiceName).Length(2, 120).WithMessage(ResourceErrorMessages.SERVICE_NAME_LENGTH_BETWEEN_2_AND_120);
        RuleFor(billing => billing.Amount).GreaterThan(0).WithMessage(ResourceErrorMessages.AMOUNT_MUST_BE_GREATER_THAN_ZERO);
        RuleFor(billing => billing.PaymentMethod).IsInEnum().WithMessage(ResourceErrorMessages.PAYMENT_METHOD_INVALID);
        RuleFor(billing => billing.Notes).MaximumLength(500).WithMessage(ResourceErrorMessages.NOTES_MUST_BE_LESS_THAN_500_CHARACTERS);
    }
}

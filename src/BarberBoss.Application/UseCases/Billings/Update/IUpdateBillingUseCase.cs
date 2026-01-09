using BarberBoss.Communication.Requests;

namespace BarberBoss.Application.UseCases.Billings.Update;

public interface IUpdateBillingUseCase
{
    Task Execute(Guid id, RequestUpdateExpenseJson request);
}

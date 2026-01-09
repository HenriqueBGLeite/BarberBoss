using BarberBoss.Communication.Responses;
using BarberBoss.Domain.Repositories.Billings;
using BarberBoss.Exception;
using BarberBoss.Exception.ExceptionsBase;
using Mapster;

namespace BarberBoss.Application.UseCases.Billings.GetById;

public class GetBillingByIdUseCase : IGetBillingByIdUseCase
{
    private readonly IBillingsReadOnlyRepository _repository;

    public GetBillingByIdUseCase(IBillingsReadOnlyRepository repository)
    {
        _repository = repository;
    }

    public async Task<ResponseBillingJson?> Execute(Guid id)
    {
        var result = await _repository.GetById(id);
        if (result is null)
            throw new NotFoundException(ResourceErrorMessages.BILLING_NOT_FOUND);

        return result.Adapt<ResponseBillingJson>();
    }
}

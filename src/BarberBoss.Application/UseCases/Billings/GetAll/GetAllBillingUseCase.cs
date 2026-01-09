using BarberBoss.Communication.Responses;
using BarberBoss.Domain.Repositories.Billings;
using Mapster;

namespace BarberBoss.Application.UseCases.Billings.GetAll;

public class GetAllBillingUseCase : IGetAllBillingUseCase
{
    private readonly IBillingsReadOnlyRepository _repository;

    public GetAllBillingUseCase(IBillingsReadOnlyRepository repository)
    {
        _repository = repository;
    }

    public async Task<ResponseBillingsJson> Execute()
    {
        var result = await _repository.GetAll();

        return new ResponseBillingsJson
        {
            Billings = result.Adapt<List<ResponseShortBillingJson>>()
        };
    }
}

using BarberBoss.Communication.Requests;
using BarberBoss.Communication.Responses;
using BarberBoss.Domain.Entities;
using BarberBoss.Domain.Repositories;
using BarberBoss.Domain.Repositories.Billings;
using BarberBoss.Exception.ExceptionsBase;
using Mapster;

namespace BarberBoss.Application.UseCases.Billings.Register;

public class RegisterBillingUseCase : IRegisterBillingUseCase
{
    private readonly IBillingsWriteOnlyRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterBillingUseCase(IBillingsWriteOnlyRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseRegisteredBillingJson> Execute(RequestBillingJson request)
    {
        Validate(request);

        var entity = request.Adapt<Billing>();

        await _repository.Add(entity);
        await _unitOfWork.Commit();

        return entity.Adapt<ResponseRegisteredBillingJson>();
    }

    private void Validate(RequestBillingJson request)
    {
        var validator = new BillingValidator();

        var result = validator.Validate(request);

        if (result.IsValid == false)
        {
            var errorMessages = result.Errors.Select(f => f.ErrorMessage).ToList();

            throw new ErrorOnValidationException(errorMessages);
        }
    }
}

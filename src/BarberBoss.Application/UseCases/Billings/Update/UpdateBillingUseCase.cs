using BarberBoss.Communication.Requests;
using BarberBoss.Domain.Repositories;
using BarberBoss.Domain.Repositories.Billings;
using BarberBoss.Exception;
using BarberBoss.Exception.ExceptionsBase;
using Mapster;

namespace BarberBoss.Application.UseCases.Billings.Update;

public class UpdateBillingUseCase : IUpdateBillingUseCase
{
    private readonly IBillingsUpdateOnlyRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateBillingUseCase(IBillingsUpdateOnlyRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Execute(Guid id, RequestUpdateExpenseJson request)
    {
        Validate(request);

        var billing = await _repository.GetById(id);
        if (billing is null)
            throw new NotFoundException(ResourceErrorMessages.BILLING_NOT_FOUND);

        request.Adapt(billing);

        _repository.Update(billing);
        await _unitOfWork.Commit();
    }

    private void Validate(RequestUpdateExpenseJson request)
    {
        var validator = new BillingUpdateValidator();

        var result = validator.Validate(request);

        if (result.IsValid == false)
        {
            var errorMessages = result.Errors.Select(f => f.ErrorMessage).ToList();

            throw new ErrorOnValidationException(errorMessages);
        }
    }
}

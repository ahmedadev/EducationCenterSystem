using EducationCenterSystem.Domain.Entities;
using EducationCenterSystem.Domain.Repositories;
using EducationCenterSystem.Domain.ValueObjects;
using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Parents.Register;

internal sealed class RegisterCommandHandler : IRequestHandler<RegisterCommand, ErrorOr<Guid>>
{
    private readonly IParentRepository _parentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterCommandHandler(IParentRepository parentRepository, IUnitOfWork unitOfWork)
    {
        _parentRepository = parentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Guid>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        Email? email = null;
        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var emailResult = Email.Create(request.Email);
            if (emailResult.IsError)
                return emailResult.Errors;
            email = emailResult.Value;
        }

        var phoneNumberResult = PhoneNumber.Create(request.PhoneNumber);
        if (phoneNumberResult.IsError)
            return phoneNumberResult.Errors;

        var parentResult = Parent.Register(
            request.FirstName,
            request.SecondName,
            request.ThirdName,
            request.LastName,
            email,
            phoneNumberResult.Value,
            request.NationalId,
            request.Job,
            request.Address,
            request.Notes);

        if (parentResult.IsError)
            return parentResult.Errors;

        var parent = parentResult.Value;

        _parentRepository.Add(parent);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return parent.Id;
    }
}

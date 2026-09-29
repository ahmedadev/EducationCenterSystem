using EducationCenterSystem.Application.Common.Interfaces;
using EducationCenterSystem.Domain.Repositories;
using EducationCenterSystem.Domain.ValueObjects;
using ErrorOr;
using MediatR;

namespace EducationCenterSystem.Application.Parents.UpdateProfile;

internal sealed class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, ErrorOr<Success>>
{
    private readonly IParentRepository _parentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProfileCommandHandler(IParentRepository parentRepository, IUnitOfWork unitOfWork)
    {
        _parentRepository = parentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Success>> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var parent = await _parentRepository.GetByIdAsync(request.Id, cancellationToken);
        if (parent is null)
            return Error.NotFound("Parent.NotFound", "Parent not found.");

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

        var updateResult = parent.UpdateProfile(
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

        if (updateResult.IsError)
            return updateResult.Errors;

        _parentRepository.Update(parent);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}

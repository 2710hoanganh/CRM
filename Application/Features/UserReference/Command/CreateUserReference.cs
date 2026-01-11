using MediatR;
using Domain.Models.Common;
using Application.Repositories.Base;
using Application.Repositories;
using Domain.Constants.AppEnum;
using Domain.Models.DTO.UserReference;

namespace Application.Features.UserReference.Command
{
    public class CreateUserReferenceCommand : BaseFields, IRequest<Response<bool>>
    {
        public required List<CreateUserReferenceRequest> Requests { get; set; }

        public class CreateUserReferenceCommandHandler : IRequestHandler<CreateUserReferenceCommand, Response<bool>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IUserReferenceRepository _userReferenceRepository;
            public CreateUserReferenceCommandHandler(IUnitOfWork unitOfWork, IUserReferenceRepository userReferenceRepository)
            {
                _unitOfWork = unitOfWork;
                _userReferenceRepository = userReferenceRepository;

            }
            
            public async Task<Response<bool>> Handle(CreateUserReferenceCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    List<Domain.Entities.UserReference> userReferences = new List<Domain.Entities.UserReference>();
                    foreach (var item in request.Requests)
                    {
                        userReferences.Add(new Domain.Entities.UserReference
                        {
                            FullName = item.FullName,
                            PhoneNumber = item.PhoneNumber,
                            Relationship = item.Relationship,
                            UserId = request.Id,
                        });
                    }
                    await _userReferenceRepository.AddRange(userReferences, cancellationToken);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    return new Response<bool>(ResponseResult.SUCCESS, "User references created successfully", true, null);
                }
                catch (Exception ex)
                {
                    return new Response<bool>(ResponseResult.ERROR, ex.Message, false, null);
                }
            }
        }
    }
}
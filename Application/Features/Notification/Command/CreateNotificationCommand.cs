using MediatR;
using Domain.Models.Common;
using Application.Repositories;
namespace Application.Features.Notification.Command
{
    public class CreateNotificationCommand : IRequest<Response<bool>>
    {
        public int UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public int Type { get; set; }

        // public class CreateNotificationCommandHandler : IRequestHandler<CreateNotificationCommand, Response<bool>>
        // {
        //     private readonly INotificationRepository _notificationRepository;
        //     public CreateNotificationCommandHandler(INotificationRepository notificationRepository)
        //     {
        //         _notificationRepository = notificationRepository;
        //     }

        //     public async Task<Response<bool>> Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
        //     {
        //         try
        //         {
        //             var notification = new Domain.Entities.Notification
        //             {
        //                 UserId = request.UserId,
        //                 Title = request.Title,
        //                 Content = request.Content,
        //                 Type = request.Type,
        //             };
        //             await _notificationRepository.Add(notification, cancellationToken);
        //             await _unitOfWork.SaveChangesAsync(cancellationToken);
        //             return new Response<bool>(true);
        //         }
        //         catch (Exception ex)
        //         {
        //             return new Response<bool>(ResponseResult.ERROR, ex.Message, null, null);
        //         }
        //     }
        // }
    }
}
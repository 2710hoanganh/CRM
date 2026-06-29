using MediatR;
using Domain.Models.Common;
using Domain.Models.DTO.Notification;
using Application.Repositories;
using Application.Repositories.Base;
using Domain.Constants.AppEnum;

namespace Application.Features.Notification.Query
{
    public class ListNoitifiactionQuery : BasePaginationQuery, IRequest<Response<List<NotficationList>>>
    {
        public class ListNoitifiactionQueryHandler : IRequestHandler<ListNoitifiactionQuery, Response<List<NotficationList>>>
        {
            private readonly INotificationRepository _notificationRepository;
            private readonly IAutoMapper _autoMapper;
            public ListNoitifiactionQueryHandler(INotificationRepository notificationRepository, IAutoMapper autoMapper)
            {
                _notificationRepository = notificationRepository;
                _autoMapper = autoMapper;
            }
            public async Task<Response<List<NotficationList>>> Handle(ListNoitifiactionQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    var paged = await _notificationRepository.GetPagination(
                        filter: x => x.UserId == request.Id,
                        orderBy: null,
                        includes: null,
                        selector: x => _autoMapper.Map<NotficationList>(x),
                        pageNumber: request.PageNumber,
                        pageSize: request.PageSize,
                        cancellationToken: cancellationToken);
                    var notifications = paged.Data.ToList();
                    return new Response<List<NotficationList>>(ResponseResult.SUCCESS, "Notifications fetched successfully", notifications, null);
                }
                catch (Exception ex)
                {
                    return new Response<List<NotficationList>>(ResponseResult.ERROR, ex.Message, null, null);
                }
            }
        }
    }
}
using Chat.Application.Features;
using Chat.Application.Responces.ChatRequest;
using Chat.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chat.Application.Handlers
{
    public class GetChatsQueryHandler
           : IRequestHandler<GetChatsQuery, List<ChatRequestResponse>>
    {
        private readonly CoreDbContext _context;

        public GetChatsQueryHandler(CoreDbContext context)
        {
            _context = context;
        }

        public async Task<List<ChatRequestResponse>> Handle(
            GetChatsQuery request,
            CancellationToken cancellationToken)
        {
            return await _context.ChatRequests
                .Where(x => (x.FromDepartmentId == request.DepartmentId
                          || x.ToDepartmentId == request.DepartmentId) // ← Исправил
                          && !x.IsDeleted)
                .OrderByDescending(x => x.CreatedDate)
                .Select(x => new ChatRequestResponse
                {
                    Id = x.Id,
                    Title = x.Title,
                    Status = x.Status.ToString(),
                    CreatedDate = x.CreatedDate,
                    CreatedByUserId = x.CreatedByUserId,
                    FromDepartmentId = x.FromDepartmentId,
                    ToDepartmentId = x.ToDepartmentId,
                    AssignedToUserId = x.AssignedToUserId
                })
                .ToListAsync(cancellationToken);
        }
    }
}
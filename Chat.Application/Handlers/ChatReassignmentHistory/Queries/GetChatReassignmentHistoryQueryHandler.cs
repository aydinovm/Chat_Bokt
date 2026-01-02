using Chat.Application.Features;
using Chat.Application.Responces;
using Chat.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chat.Application.Handlers
{
    public class GetChatReassignmentHistoryQueryHandler
        : IRequestHandler<GetChatReassignmentHistoryQuery, List<ChatReassignmentHistoryResponse>>
    {
        private readonly CoreDbContext _context;

        public GetChatReassignmentHistoryQueryHandler(CoreDbContext context)
        {
            _context = context;
        }

        public async Task<List<ChatReassignmentHistoryResponse>> Handle(
            GetChatReassignmentHistoryQuery request,
            CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == request.UserId
                    && !x.IsDeleted,
                    cancellationToken);

            if (user == null)
                return new List<ChatReassignmentHistoryResponse>();

            var chat = await _context.ChatRequests
                .FirstOrDefaultAsync(x => x.Id == request.ChatRequestId
                    && !x.IsDeleted,
                    cancellationToken);

            if (chat == null)
                return new List<ChatReassignmentHistoryResponse>();

            if (chat.ToDepartmentId != user.DepartmentId)
                return new List<ChatReassignmentHistoryResponse>();

            return await _context.ChatReassignmentHistory
                .Where(x => x.ChatRequestId == request.ChatRequestId
                    && !x.IsDeleted)
                .OrderByDescending(x => x.ReassignedAt)
                .Select(x => new ChatReassignmentHistoryResponse
                {
                    Id = x.Id,
                    ChatRequestId = x.ChatRequestId,
                    ReassignedByUserId = x.ReassignedByUserId,
                    ReassignedByUserName = _context.Users
                        .Where(u => u.Id == x.ReassignedByUserId)
                        .Select(u => u.FullName)
                        .FirstOrDefault(),
                    OldAssignedUserId = x.OldAssignedUserId,
                    OldAssignedUserName = x.OldAssignedUserId.HasValue
                        ? _context.Users
                            .Where(u => u.Id == x.OldAssignedUserId)
                            .Select(u => u.FullName)
                            .FirstOrDefault()
                        : null,
                    NewAssignedUserId = x.NewAssignedUserId,
                    NewAssignedUserName = _context.Users
                        .Where(u => u.Id == x.NewAssignedUserId)
                        .Select(u => u.FullName)
                        .FirstOrDefault(),
                    Reason = x.Reason,
                    ReassignedAt = x.ReassignedAt
                })
                .ToListAsync(cancellationToken);
        }
    }
}
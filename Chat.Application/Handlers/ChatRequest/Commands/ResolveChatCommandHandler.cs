using Chat.Application.Features;
using Chat.Application.Tags;
using Chat.Common.Helpers;
using Chat.Domain.Enums;
using Chat.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chat.Application.Handlers
{
    public class ResolveChatCommandHandler
        : IRequestHandler<ResolveChatCommand, Result<Unit>>
    {
        private readonly CoreDbContext _context;
        private readonly IRealtimeNotifier _rt;

        public ResolveChatCommandHandler(CoreDbContext context, IRealtimeNotifier rt)
        {
            _context = context;
            _rt = rt;
        }

        public async Task<Result<Unit>> Handle(ResolveChatCommand request, CancellationToken cancellationToken)
        {
            var chat = await _context.ChatRequests
                .FirstOrDefaultAsync(x => x.Id == request.ChatRequestId && !x.IsDeleted, cancellationToken);

            if (chat == null)
                return Result<Unit>.Failure("Chat request not found");

            if (chat.Status == ChatRequestStatusEnum.Closed)
                return Result<Unit>.Failure("Chat is closed");

            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == request.ResolvedByUserId && !x.IsDeleted, cancellationToken);

            if (user == null)
                return Result<Unit>.Failure("User not found");

            bool canResolve =
                chat.AssignedToUserId == request.ResolvedByUserId
                || (user.IsDepartmentAdmin && user.DepartmentId == chat.ToDepartmentId);

            if (!canResolve)
                return Result<Unit>.Failure("You don't have permission to resolve this chat");

            chat.Status = ChatRequestStatusEnum.Resolved;
            chat.ResolvedAt = DateTime.UtcNow;
            chat.ResolvedByUserId = request.ResolvedByUserId;
            chat.ModifiedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            // ✅ REALTIME
            await _rt.ChatUpdated(chat.Id, new
            {
                chatId = chat.Id,
                status = chat.Status.ToString(),
                resolvedAt = chat.ResolvedAt,
                resolvedByUserId = chat.ResolvedByUserId,
                modifiedDate = chat.ModifiedDate
            }, cancellationToken);

            return Result<Unit>.Success(Unit.Value);
        }
    }
}

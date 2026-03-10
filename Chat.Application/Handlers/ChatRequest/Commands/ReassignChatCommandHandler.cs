using Chat.Application.Features;
using Chat.Application.Tags;
using Chat.Common.Helpers;
using Chat.Domain.Persistence;
using Chat.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chat.Application.Handlers
{
    public class ReassignChatCommandHandler
        : IRequestHandler<ReassignChatCommand, Result<Unit>>
    {
        private readonly CoreDbContext _context;
        private readonly IRealtimeNotifier _rt;

        public ReassignChatCommandHandler(CoreDbContext context, IRealtimeNotifier rt)
        {
            _context = context;
            _rt = rt;
        }

        public async Task<Result<Unit>> Handle(ReassignChatCommand request, CancellationToken cancellationToken)
        {
            var actor = await _context.Users
                .Include(u => u.Department)
                .FirstOrDefaultAsync(x =>
                    x.Id == request.ReassignedByUserId &&
                    x.IsDepartmentAdmin &&
                    !x.IsDeleted,
                    cancellationToken);

            if (actor == null)
                return Result<Unit>.Failure("Only department admin can reassign chats");

            bool isSuperAdmin = actor.IsSuperAdmin();

            var chat = await _context.ChatRequests
                .FirstOrDefaultAsync(x => x.Id == request.ChatRequestId && !x.IsDeleted, cancellationToken);

            if (chat == null)
                return Result<Unit>.Failure("Chat request not found");

            if (!isSuperAdmin && chat.ToDepartmentId != actor.DepartmentId)
                return Result<Unit>.Failure("You can only reassign chats in your department");

            var newAssignee = await _context.Users
                .Include(u => u.Department)
                .FirstOrDefaultAsync(x =>
                    x.Id == request.NewAssignedUserId &&
                    !x.IsDeleted,
                    cancellationToken);

            if (newAssignee == null)
                return Result<Unit>.Failure("New assignee not found");

            if (!isSuperAdmin && newAssignee.DepartmentId != actor.DepartmentId)
                return Result<Unit>.Failure("New assignee must be in your department");

            var history = new ChatReassignmentHistoryModel
            {
                Id = Guid.NewGuid(),
                ChatRequestId = chat.Id,
                ReassignedByUserId = actor.Id,
                OldAssignedUserId = chat.AssignedToUserId,
                NewAssignedUserId = newAssignee.Id,
                OldToDepartmentId = chat.ToDepartmentId,
                NewToDepartmentId = newAssignee.DepartmentId,
                Reason = request.Reason,
                ReassignedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await _context.ChatReassignmentHistory.AddAsync(history, cancellationToken);

            chat.AssignedToUserId = newAssignee.Id;

            if (isSuperAdmin && chat.ToDepartmentId != newAssignee.DepartmentId)
                chat.ToDepartmentId = newAssignee.DepartmentId;

            chat.ModifiedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            // ✅ REALTIME: всем в чате + новому исполнителю персонально
            await _rt.ChatReassigned(chat.Id, new
            {
                chatId = chat.Id,
                reassignedByUserId = actor.Id,
                oldAssignedUserId = history.OldAssignedUserId,
                newAssignedUserId = history.NewAssignedUserId,
                reason = history.Reason,
                reassignedAt = history.ReassignedAt,
                toDepartmentId = chat.ToDepartmentId
            }, cancellationToken);

            await _rt.NotifyUser(newAssignee.Id, "ChatAssignedToYou", new
            {
                chatId = chat.Id
            }, cancellationToken);

            await _rt.ChatUpdated(chat.Id, new
            {
                chatId = chat.Id,
                assignedToUserId = chat.AssignedToUserId,
                toDepartmentId = chat.ToDepartmentId,
                modifiedDate = chat.ModifiedDate
            }, cancellationToken);

            return Result<Unit>.Success(Unit.Value);
        }
    }
}

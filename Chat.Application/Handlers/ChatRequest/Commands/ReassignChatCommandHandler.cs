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

            bool assignToUser = request.NewAssignedUserId.HasValue && request.NewAssignedUserId != Guid.Empty;
            bool assignToDept = request.NewToDepartmentId.HasValue && request.NewToDepartmentId != Guid.Empty;

            if (!assignToUser && !assignToDept)
                return Result<Unit>.Failure("Specify user or department to reassign");

            // ✅ Сохраняем старые значения ДО изменения chat
            Guid? oldAssignedUserId = chat.AssignedToUserId;
            Guid? oldToDepartmentId = chat.ToDepartmentId;

            Guid? newUserId = null;
            Guid? newDeptId = null;
            Guid? notifyUserId = null;

            if (assignToUser)
            {
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

                newUserId = newAssignee.Id;
                newDeptId = newAssignee.DepartmentId;
                notifyUserId = newAssignee.Id;

                chat.AssignedToUserId = newAssignee.Id;

                if (isSuperAdmin && chat.ToDepartmentId != newAssignee.DepartmentId)
                    chat.ToDepartmentId = newAssignee.DepartmentId;
            }
            else // assignToDept
            {
                var dept = await _context.Departments
                    .FirstOrDefaultAsync(x =>
                        x.Id == request.NewToDepartmentId &&
                        !x.IsDeleted,
                        cancellationToken);

                if (dept == null)
                    return Result<Unit>.Failure("Department not found");

                if (!isSuperAdmin && dept.Id != actor.DepartmentId)
                    return Result<Unit>.Failure("You can only reassign to your department");

                newDeptId = dept.Id;

                chat.AssignedToUserId = null;
                chat.ToDepartmentId = dept.Id;
            }

            var history = new ChatReassignmentHistoryModel
            {
                Id = Guid.NewGuid(),
                ChatRequestId = chat.Id,
                ReassignedByUserId = actor.Id,
                OldAssignedUserId = oldAssignedUserId,  // ✅ старое значение
                NewAssignedUserId = newUserId,           // ✅ null или реальный Id
                OldToDepartmentId = oldToDepartmentId,  // ✅ старое значение
                NewToDepartmentId = newDeptId,           // ✅ null или реальный Id
                Reason = request.Reason,
                ReassignedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await _context.ChatReassignmentHistory.AddAsync(history, cancellationToken);

            chat.ModifiedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

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

            if (notifyUserId.HasValue)
            {
                await _rt.NotifyUser(notifyUserId.Value, "ChatAssignedToYou", new
                {
                    chatId = chat.Id
                }, cancellationToken);
            }

            await _rt.ChatUpdated(chat.Id, new
            {
                chatId = chat.Id,
                assignedToUserId = chat.AssignedToUserId,
                toDepartmentId = chat.ToDepartmentId,
                modifiedDate = chat.ModifiedDate
            }, cancellationToken);

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
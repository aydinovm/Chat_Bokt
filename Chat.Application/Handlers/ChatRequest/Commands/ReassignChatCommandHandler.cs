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

        public ReassignChatCommandHandler(CoreDbContext context)
        {
            _context = context;
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

            // DepartmentAdmin: только свои входящие
            if (!isSuperAdmin && chat.ToDepartmentId != actor.DepartmentId)
                return Result<Unit>.Failure("You can only reassign chats in your department");

            // Новый исполнитель
            var newAssignee = await _context.Users
                .Include(u => u.Department)
                .FirstOrDefaultAsync(x =>
                    x.Id == request.NewAssignedUserId &&
                    !x.IsDeleted,
                    cancellationToken);

            if (newAssignee == null)
                return Result<Unit>.Failure("New assignee not found");

            // DepartmentAdmin: только внутри департамента
            if (!isSuperAdmin && newAssignee.DepartmentId != actor.DepartmentId)
                return Result<Unit>.Failure("New assignee must be in your department");

            // История
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

            // Обновляем чат
            chat.AssignedToUserId = newAssignee.Id;

            // ✅ Если SuperAdmin назначил в другой департамент — переносим чат
            if (isSuperAdmin && chat.ToDepartmentId != newAssignee.DepartmentId)
                chat.ToDepartmentId = newAssignee.DepartmentId;

            chat.ModifiedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return Result<Unit>.Success(Unit.Value);
        }
    }
}

using Chat.Application.Features;
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

        public async Task<Result<Unit>> Handle(
            ReassignChatCommand request,
            CancellationToken cancellationToken)
        {
            // Проверяем что пользователь - IT админ
            var admin = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == request.ReassignedByUserId
                    && x.IsDepartmentAdmin
                    && !x.IsDeleted,
                    cancellationToken);

            if (admin == null)
                return Result<Unit>.Failure("Only IT department admin can reassign chats");

            var chat = await _context.ChatRequests
                .FirstOrDefaultAsync(x => x.Id == request.ChatRequestId
                    && !x.IsDeleted,
                    cancellationToken);

            if (chat == null)
                return Result<Unit>.Failure("Chat request not found");

            // Проверяем что чат принадлежит IT департаменту
            if (chat.ToDepartmentId != admin.DepartmentId)
                return Result<Unit>.Failure("You can only reassign chats in your department");

            // Проверяем что новый исполнитель существует и из IT департамента
            var newAssignee = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == request.NewAssignedUserId
                    && x.DepartmentId == admin.DepartmentId
                    && !x.IsDeleted,
                    cancellationToken);

            if (newAssignee == null)
                return Result<Unit>.Failure("New assignee not found or not in IT department");

            // Сохраняем историю переназначения
            var history = new ChatReassignmentHistoryModel
            {
                Id = Guid.NewGuid(),
                ChatRequestId = request.ChatRequestId,
                ReassignedByUserId = request.ReassignedByUserId,
                OldAssignedUserId = chat.AssignedToUserId,
                NewAssignedUserId = request.NewAssignedUserId,
                Reason = request.Reason,
                ReassignedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await _context.ChatReassignmentHistory.AddAsync(history, cancellationToken);

            // Обновляем чат
            chat.AssignedToUserId = request.NewAssignedUserId;
            chat.ModifiedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return Result<Unit>.Success(Unit.Value);
        }
    }
}
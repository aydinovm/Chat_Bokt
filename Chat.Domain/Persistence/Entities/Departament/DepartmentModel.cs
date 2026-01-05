using Chat.Domain.Enums;

namespace Chat.Domain.Persistence
{
    public class DepartmentModel : BaseEntity, IEntity
    {
        public string Name { get; set; } = null!;
        public DepartmentTypeEnum Type { get; set; }

        public bool IsActive { get; set; }

        public ICollection<UserModel> Users { get; set; }
            = new HashSet<UserModel>();

        public bool IsDeleted { get; set; }
    }
}

using Chat.Domain.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Domain.Persistence
{
    public class DepartmentModel : BaseEntity, IEntity
    {
        public string Name { get; set; }
        public string Type { get; set; } // "HR", "Finance", "Marketing", "IT"

        public bool IsActive { get; set; }

        public ICollection<UserModel> Users { get; set; } // ← Вернул

        public bool IsDeleted { get; set; }
    }
}

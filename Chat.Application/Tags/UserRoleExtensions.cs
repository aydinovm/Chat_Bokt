using Chat.Domain.Enums;
using Chat.Domain.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Application.Tags
{
    public static class UserRoleExtensions
    {
        public static bool IsSuperAdmin(this UserModel user)
            => user.IsDepartmentAdmin && user.Department.Type == DepartmentTypeEnum.Control;
    }
}

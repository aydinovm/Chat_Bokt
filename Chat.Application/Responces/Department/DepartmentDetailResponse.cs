using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Application.Responces.Department
{
    public class DepartmentDetailResponse : BaseResponce
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public bool IsActive { get; set; }
        public int UsersCount { get; set; }
        public int ActiveChatsCount { get; set; } // Активные чаты
        public List<DepartmentUserResponse> Users { get; set; }
    }
    public class DepartmentUserResponse
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Username { get; set; }
        public bool IsDepartmentAdmin { get; set; }
    }
}

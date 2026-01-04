using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Application.Responces
{
    public class LoginResponse
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; }
        public string Username { get; set; }
        public Guid DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentType { get; set; }
        public bool IsDepartmentAdmin { get; set; }

        // JWT поля
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}

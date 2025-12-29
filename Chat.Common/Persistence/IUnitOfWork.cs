using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Common.Persistence
{
    public interface IUnitOfWork
    {
        Task AsUnitAsync(Func<Task> func);
        Task<T> AsUnitAsync<T>(Func<Task<T>> func);
    }
}

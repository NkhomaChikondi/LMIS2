using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMIS.DataStore.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        Task<int> SaveToStoreAsync();
    }
}

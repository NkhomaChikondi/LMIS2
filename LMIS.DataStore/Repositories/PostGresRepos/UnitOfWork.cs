using LMIS.DataStore.Data;
using LMIS.DataStore.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMIS.DataStore.Repositories.PostGresRepos
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public UnitOfWork(ApplicationDbContext context)
        {
            this._context = context;
        }
        public async Task<int> SaveToStoreAsync()
        {
            //Save changes to database

            var databaseResult = await this._context.SaveChangesAsync();

            return databaseResult;
        }
    }
}

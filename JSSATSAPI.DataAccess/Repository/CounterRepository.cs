using JSSATSAPI.DataAccess.IRepository;
using JSSATSAPI.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.DataAccess.Repository
{
    public class CounterRepository : GenericRepository<Counter>, ICounterRepository
    {
        public CounterRepository(JSS_DBContext context) : base(context)
        {
        }
    }
}

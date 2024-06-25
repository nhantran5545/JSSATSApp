using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.RequestModels.CounterRequest
{
    public class CreateCounteRequest
    {
        public string CounterName { get; set; } = null!;
        public int AccountId { get; set; }
    }
}

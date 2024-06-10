using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.ResponseModels.CounterResponse
{
    public class CounterResponse
    {
        public int CounterId { get; set; }
        public string CounterName { get; set; } = null!;
        public int AccountId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}

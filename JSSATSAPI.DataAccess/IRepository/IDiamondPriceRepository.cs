using JSSATSAPI.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.DataAccess.IRepository
{
    public interface IDiamondPriceRepository : IGenericRepository<DiamondPrice>
    {
        Task<DiamondPrice> GetLatestDiamondPriceAsync(string origin, decimal caratWeightFrom, decimal caratWeightTo, string color, string clarity, string cut);
        Task<DiamondPrice> GetBuyPriceDiamondPriceAsync(string origin, decimal caratWeight, string color, string clarity, string cut);
    }
}

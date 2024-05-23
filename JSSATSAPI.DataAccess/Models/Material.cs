using System;
using System.Collections.Generic;

namespace JSSATSAPI.DataAccess.Models
{
    public partial class Material
    {
        public Material()
        {
            MaterialPrices = new HashSet<MaterialPrice>();
        }

        public int MaterialId { get; set; }
        public string MaterialName { get; set; } = null!;

        public virtual ICollection<MaterialPrice> MaterialPrices { get; set; }
    }
}

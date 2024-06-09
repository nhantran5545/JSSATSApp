using System;
using System.Collections.Generic;

namespace JSSATSAPI.DataAccess.Models
{
    public partial class MaterialType
    {
        public MaterialType()
        {
            Materials = new HashSet<Material>();
        }

        public int MaterialTypeId { get; set; }
        public string? MaterialTypeName { get; set; }

        public virtual ICollection<Material> Materials { get; set; }
    }
}

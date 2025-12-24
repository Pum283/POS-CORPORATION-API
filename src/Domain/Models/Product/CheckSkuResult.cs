using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Product
{
    public class CheckSkuResult
    {
        public bool IsValid { get; set; }
        public string? SuggestSku { get; set; } // cho  SKU
        public string? Reason { get; set; }
    }
}

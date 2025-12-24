
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Ingredient
{
    public class CheckCodeResult
    {
        public bool IsValid { get; set; }
        public string? SuggestCode { get; set; }
        public string? Reason { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.PeriodLock
{
    public class FilterPeriodLockModel
    {
        public Guid? UserId { get; set; }
        public Guid? StoreId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }

    public class FilterPeriodLockDateModel
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }

}

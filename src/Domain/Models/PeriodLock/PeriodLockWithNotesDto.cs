using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.PeriodLock
{
    public class PeriodLockWithNotesDto
    {
        public Guid Id { get; set; }
        public Guid StoreId { get; set; }
        [JsonIgnore]
        public Guid UserId { get; set; }
        public string? Email { get; set; }
        public ShiftType Shift { get; set; }
        public ShiftPeriodStatusType Status { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public decimal OpeningBalance { get; set; }
        public decimal ClosingBalance { get; set; }
        public decimal TotalSales { get; set; }

        public List<PeriodLockNoteDto> Notes { get; set; } = new();
    }

    public class PeriodLockNoteDto
    {
        public Guid Id { get; set; }
        public decimal SpendingAmount { get; set; }
        public string Note { get; set; } = string.Empty;
    }

}

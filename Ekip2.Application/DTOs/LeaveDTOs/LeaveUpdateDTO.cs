using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ekip2.Application.DTOs.LeaveDTOs
{
    public class LeaveUpdateDTO
    {
        public Guid Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid LeaveTypeId { get; set; }
        public Guid ManagerId { get; set; }
        public LeaveStatus LeaveStatus { get; set; } = LeaveStatus.Pending;

    }
}

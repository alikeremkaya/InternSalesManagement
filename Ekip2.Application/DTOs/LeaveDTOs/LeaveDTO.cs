using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ekip2.Application.DTOs.LeaveDTOs
{
    public class LeaveDTO
    {
        public Guid Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid LeaveTypeId { get; set; }
        public Guid ManagerId { get; set; }
        public string LeaveType { get; set; }
        public string ManagerFirstName { get; set; }
        public string ManagerLastName { get; set; }
        public LeaveStatus LeaveStatus { get; set; }

    }
}

using Ekip2.Domain.Entities;
using Ekip2.Domain.Enums;


namespace Ekip2.Presentation.Areas.Manager.Models.LeaveVMs
{
    public class LeaveUpdateVM
    {
        public Guid Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid LeaveTypeId { get; set; }
        public SelectList? LeaveTypes { get; set; }
        public Guid ManagerId { get; set; }
        public SelectList? Managers { get; set; }
        public LeaveStatus LeaveStatus { get; set; }


    }
}

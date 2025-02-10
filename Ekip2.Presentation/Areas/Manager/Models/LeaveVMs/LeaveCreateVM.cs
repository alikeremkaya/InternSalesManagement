using System.ComponentModel.DataAnnotations;

namespace Ekip2.Presentation.Areas.Manager.Models.LeaveVMs
{
    public class LeaveCreateVM
    {
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }
       
        public Guid LeaveTypeId { get; set; }
        public SelectList? LeaveTypes { get; set; }
        public Guid ManagerId { get; set; }
        public SelectList Managers { get; set; }


    }
}

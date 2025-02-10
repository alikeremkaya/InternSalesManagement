namespace Ekip2.Presentation.Areas.Admin.Models.CompanyVMs
{
    public class CompanyIndexVM
    {
       public ICollection<CompanyListVM> CompanyListVMs { get; set; }
        public CompanyCreateVM CompanyCreateVM { get; set; } = new();
    }
}

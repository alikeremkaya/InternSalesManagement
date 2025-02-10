

using Castle.Components.DictionaryAdapter.Xml;

namespace Ekip2.Presentation.Areas.Admin.Controllers
{
    public class CompanyController : AdminBaseController
    {
        
        private readonly ICompanyService _companyService;
        private readonly IManagerService _managerService;
        public CompanyController(ICompanyService companyService, IManagerService managerService)
        {
            _companyService = companyService;
            _managerService = managerService;
        }

        public async Task<IActionResult> Index()
        {
            var companyIndexVM = new CompanyIndexVM();
            var result = await _companyService.GetAllAsync();
            var companyVMs = result.Data.Adapt<List<CompanyListVM>>();
           
            await Console.Out.WriteLineAsync(result.Messages);
            companyIndexVM.CompanyListVMs = companyVMs;
            return View(companyIndexVM);
        }
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CompanyIndexVM model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    await Console.Out.WriteLineAsync(error.ErrorMessage);
                }
                return View(model);
            }

            try
            {
                var companyDTO = model.CompanyCreateVM.Adapt<CompanyCreateDTO>();
                var result = await _companyService.CreateAsync(companyDTO);
                if (!result.IsSuccess)
                {
                    await Console.Out.WriteLineAsync(result.Messages);
                    return View(model);
                }
                await Console.Out.WriteLineAsync(result.Messages);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {

                await Console.Out.WriteLineAsync($"Unexpected error: {ex.Message}");
                return View(model);
            } 
        }
        public async Task<IActionResult> Delete(Guid id)
        {
            var deletinId = await _companyService.DeleteAsync(id);
            
            if (!deletinId.IsSuccess)
            {
                await Console.Out.WriteLineAsync(deletinId.Messages);
                return RedirectToAction("Index");
            }
            await Console.Out.WriteLineAsync(deletinId.Messages);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var result = await _companyService.GetByIdAsync(id);
            var companyVM = result.Data.Adapt<CompanyUpdateVM>();
            return View(companyVM);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(CompanyUpdateVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await _companyService.UpdateAsync(model.Adapt<CompanyUpdateDTO>());
            if (!result.IsSuccess)
            {
                await Console.Out.WriteLineAsync(result.Messages);
                return View(model);
            }
            await Console.Out.WriteLineAsync(result.Messages);
            return RedirectToAction("Index");

        }

    }
}

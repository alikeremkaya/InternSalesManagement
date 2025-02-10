using Ekip2.Domain.Enums;
using Ekip2.Presentation.Areas.Manager.Models.EmployeeVMs;
using Ekip2.Presentation.Extentions;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ekip2.Presentation.Areas.Manager.Controllers
{
    public class EmployeeController : ManagerBaseController
    {
        private readonly IManagerService _managerService;
        private readonly IMailService _mailService;
        private readonly ICompanyService _companyService;
        public EmployeeController(IManagerService managerService, IMailService mailService, ICompanyService companyService)
        {
            _managerService = managerService;
            _mailService = mailService;
            _companyService = companyService;
        }
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var managerResult = await _managerService.GetByIdentityUserIdAsync(userId);
            var result = await _managerService.GetAllAsync();

            if (!result.IsSuccess)
            {
                await Console.Out.WriteLineAsync(result.Messages);
                return View(new List<EmployeeListVM>());
            }
            var employeeVMs = result.Data
                               .Where(m => m.Roles == Roles.Employee).Where(x => x.CompanyId == managerResult.Data.CompanyId)
                               .Adapt<List<EmployeeListVM>>();
            await Console.Out.WriteLineAsync(result.Messages);
            return View(employeeVMs);
        }
        public async Task<IActionResult> Create()
        {
            var managerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var managerResult = await _managerService.GetByIdentityUserIdAsync(managerId);
            
            if (!managerResult.IsSuccess)
            {
                await Console.Out.WriteLineAsync(managerResult.Messages);
                return RedirectToAction("Index");
            }

            var manager = managerResult.Data;

            var employeeVM = new EmployeeCreateVM()
            {
                CompanyName = manager.CompanyName,
                RoleList = GetRoles(),
                CompanyId = manager.CompanyId
            };

            return View(employeeVM);
        }
        [HttpPost]
        public async Task<IActionResult> Create(EmployeeCreateVM model)
        {
            if (!ModelState.IsValid)
            {
                model.RoleList = GetRoles();
                return View(model);
            }

            var managerUserDTO = model.Adapt<ManagerCreateDTO>();
            if (model.NewImage != null && model.NewImage.Length>0)
            {
                managerUserDTO.Image = await model.NewImage.StringToByteArrayAsync();
            }
            var result = await _managerService.CreateEmployeeAsync(managerUserDTO);

            if (!result.IsSuccess)
            {
                model.RoleList = GetRoles();
                model.CompanyId = managerUserDTO.CompanyId;
                await Console.Out.WriteLineAsync(result.Messages);
                return View(model);
            }

            await Console.Out.WriteLineAsync(result.Messages);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _managerService.DeleteAsync(id);

            if (!result.IsSuccess)
            {
                await Console.Out.WriteLineAsync(result.Messages);
                return RedirectToAction("Index");
            }

            await Console.Out.WriteLineAsync(result.Messages);
            return RedirectToAction("Index");

        }

        public async Task<IActionResult> Update(Guid id)
        {
            var result = await _managerService.GetByIdAsync(id);
            if (!result.IsSuccess)
            {
                await Console.Out.WriteLineAsync(result.Messages);
                return RedirectToAction("Index");
            }

            var employeeUpdateVM = result.Data.Adapt<EmployeeUpdateVM>();
            employeeUpdateVM.CompanyId = result.Data.CompanyId; // Şirket listesini ekleyin
            employeeUpdateVM.RoleList = GetRoles(employeeUpdateVM.Roles);
            employeeUpdateVM.ExistingImage = result.Data.Image;

            return View(employeeUpdateVM);
        }

        [HttpPost]
        public async Task<IActionResult> Update(EmployeeUpdateVM model)

        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var updateManagerDTO = model.Adapt<ManagerUpdateDTO>();
            if (model.NewImage != null && model.NewImage.Length>0)
            {
                updateManagerDTO.Image = await model.NewImage.StringToByteArrayAsync();
            }
            else
            {
                updateManagerDTO.Image = (await _managerService.GetByIdAsync(model.Id)).Data.Image;
            }
            var result = await _managerService.UpdateAsync(updateManagerDTO);
            if (!result.IsSuccess)
            {
                await Console.Out.WriteLineAsync(result.Messages);
                model.CompanyId = updateManagerDTO.CompanyId;
                model.RoleList = GetRoles(model.Roles);
                return View(model);
            }
            await Console.Out.WriteLineAsync(result.Messages);
            return RedirectToAction("Index");

        }
        private SelectList GetRoles(Roles? selectedRole = null)
        {
            var roles = Enum.GetValues(typeof(Roles))
                            .Cast<Roles>()
                            .Select(role => new SelectListItem
                            {
                                Value = role.ToString(),
                                Text = role.ToString(),
                                Selected = role == selectedRole
                            }).ToList();

            return new SelectList(roles, "Value", "Text");
        }
    }
}

using Ekip2.Application.DTOs.LeaveDTOs;
using Ekip2.Application.Services.LeaveService;
using Ekip2.Domain.Enums;
using Ekip2.Presentation.Areas.Manager.Models.LeaveVMs;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using System.Security.Claims;

namespace Ekip2.Presentation.Areas.Manager.Controllers
{
    public class LeaveController : ManagerBaseController
    {
        private readonly ILeaveTypeService _leaveTypeService;
        private readonly ILeaveService _leaveService;
        private readonly IManagerService _managerService;

        public LeaveController(ILeaveTypeService leaveTypeService, ILeaveService leaveService, IManagerService managerService)
        {
            _leaveTypeService = leaveTypeService;
            _leaveService = leaveService;
            _managerService = managerService;
        }

        public async Task<IActionResult> Index()
        {

            var result = await _leaveService.GetAllAsync();
            if (!result.IsSuccess)
            {
                await Console.Out.WriteLineAsync(result.Messages);
                return View(new List<LeaveListVM>());
            }

            // DTO'dan VM'ye dönüştürme
            var leaveVMs = result.Data.Adapt<List<LeaveListVM>>();

            await Console.Out.WriteLineAsync(result.Messages);

            // VM'leri View'a gönderme
            return View(leaveVMs);
        }

        public async Task<IActionResult> Create()
        {
            var leaveVM = new LeaveCreateVM()
            {
                LeaveTypes = await GetLeaveTypes(),

                Managers = await GetManagers()
            };
            return View(leaveVM);
        }

        [HttpPost]
        public async Task<IActionResult> Create(LeaveCreateVM model)
        {


            try
            {
                var result = await _leaveService.CreateAsync(model.Adapt<LeaveCreateDTO>());

                if (!result.IsSuccess)
                {
                    model.Managers = await GetManagers(model.ManagerId);
                    await Console.Out.WriteLineAsync(result.Messages);
                    return View(model);
                }

                await Console.Out.WriteLineAsync(result.Messages);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Global exception handling or logging can be done here
                await Console.Out.WriteLineAsync($"Unexpected error: {ex.Message}");
                return View(model); // or another error view
            }
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _leaveService.DeleteAsync(id);

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
            var result = await _leaveService.GetByIdAsync(id);
            if (!result.IsSuccess)
            {
                await Console.Out.WriteLineAsync(result.Messages);
                return RedirectToAction("Index");
            }

            var leaveUpdateVM = result.Data.Adapt<LeaveUpdateVM>();
            leaveUpdateVM.LeaveTypes = await GetLeaveTypes(leaveUpdateVM.LeaveTypeId);
            leaveUpdateVM.Managers = await GetManagers(leaveUpdateVM.ManagerId);

            return View(leaveUpdateVM);
        }

        [HttpPost]
        public async Task<IActionResult> Update(LeaveUpdateVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _leaveService.UpdateAsync(model.Adapt<LeaveUpdateDTO>());
            if (!result.IsSuccess)
            {
               
                await Console.Out.WriteLineAsync(result.Messages);
                return View("Index");
            }
            await Console.Out.WriteLineAsync(result.Messages);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> ApproveLeave(Guid id)
        {
            

            var result = await _leaveService.ApproveLeaveAsync(id);

            if (!result.IsSuccess)
            {
                await Console.Out.WriteLineAsync(result.Messages);
                return RedirectToAction("Index");
            }

            await Console.Out.WriteLineAsync(result.Messages);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> RejectLeave(Guid id)
        {
           

            var result = await _leaveService.RejectLeaveAsync(id);

            if (!result.IsSuccess)
            {
                await Console.Out.WriteLineAsync(result.Messages);
                return RedirectToAction("Index");
            }

            await Console.Out.WriteLineAsync(result.Messages);
            return RedirectToAction("Index");
        }

        private async Task<SelectList> GetLeaveTypes(Guid? leaveTypeId = null)
        {
            var leaveTypes = (await _leaveTypeService.GetAllAsync()).Data;
            return new SelectList(leaveTypes.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Type,
                Selected = x.Id == (leaveTypeId != null ? leaveTypeId.Value : leaveTypeId)
            }).OrderBy(x => x.Text), "Value", "Text");
        }

        private async Task<SelectList> GetManagers(Guid? ManagerId = null)
        {
            var allEmployees = (await _managerService.GetAllAsync()).Data;

            // Role'ü Employee olanları filtrele
            var employees = allEmployees.Where(x => x.Roles == Roles.Employee);

            // SelectListItem oluşturma
            var selectListItems = employees.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.FirstName + " " + x.LastName,
                Selected = x.Id == ManagerId
            }).OrderBy(x => x.Text).ToList();

            // SelectList oluşturma
            var selectList = new SelectList(selectListItems, "Value", "Text");

            return selectList;
        }
    }
}

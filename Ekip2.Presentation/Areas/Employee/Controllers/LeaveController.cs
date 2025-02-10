using Ekip2.Application.DTOs.LeaveDTOs;
using Ekip2.Application.Services.LeaveService;
using Ekip2.Domain.Enums;
using Ekip2.Presentation.Areas.Manager.Models.LeaveVMs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Ekip2.Presentation.Areas.Employee.Controllers
{
    public class LeaveController : EmployeeBaseController
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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var managerResult = await _managerService.GetByIdentityUserIdAsync(userId);
            var result = await _leaveService.GetAllAsync();

            if (!result.IsSuccess)
            {
                await Console.Out.WriteLineAsync(result.Messages);
                return View(new List<LeaveListVM>());
            }

            var leaveVMs = result.Data
                                 .Where(x => x.ManagerId == managerResult.Data.Id)
                                 .Adapt<List<LeaveListVM>>();

            await Console.Out.WriteLineAsync(result.Messages);
            return View(leaveVMs);
        }

        public async Task<IActionResult> Create()
        {
            var leaveVM = new LeaveCreateVM
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
                    model.LeaveTypes = await GetLeaveTypes(model.LeaveTypeId);
                    model.Managers = await GetManagers(model.ManagerId);
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
                model.Managers = await GetManagers(); // ModelState geçerli değilse tekrar doldurun
                model.LeaveTypes = await GetLeaveTypes();
                return View(model);
            }
                
            var leaveUpdateDTO = model.Adapt<LeaveUpdateDTO>();

            leaveUpdateDTO.LeaveStatus = LeaveStatus.Pending; // Durumu Pending olarak ayarlayın

            var result = await _leaveService.UpdateAsync(leaveUpdateDTO);


            if (!result.IsSuccess)
            {
                await Console.Out.WriteLineAsync(result.Messages);
                return View(model);
            }

            await Console.Out.WriteLineAsync(result.Messages);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> Approve(Guid id)
        {
            var result = await _leaveService.ApproveLeaveAsync(id);
            await Console.Out.WriteLineAsync(result.Messages);
            return RedirectToAction("Index");
        }
        [HttpPost]
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
                Selected = x.Id == leaveTypeId
            }).OrderBy(x => x.Text), "Value", "Text");
        }

        private async Task<SelectList> GetManagers(Guid? managerId = null)
        {
            var allManagers = (await _managerService.GetAllAsync()).Data;
            var selectListItems = allManagers.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.FirstName + " " + x.LastName,
                Selected = x.Id == managerId
            }).OrderBy(x => x.Text).ToList();

            return new SelectList(selectListItems, "Value", "Text");
        }
    }
}

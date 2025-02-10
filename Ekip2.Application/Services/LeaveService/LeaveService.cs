using Ekip2.Application.DTOs.AdvanceDTOs;
using Ekip2.Application.DTOs.LeaveDTOs;
using Ekip2.Application.Services.AdvanceServices;
using Ekip2.Domain.Entities;
using Ekip2.Infrastructure.Repositories.LeaveRepositories;
using Mapster;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ekip2.Application.Services.LeaveService
{
    public class LeaveService : ILeaveService
    {
        private readonly ILeaveRepository _leaveRepository;
        private readonly ILogger<LeaveService> _logger;
        public LeaveService(ILeaveRepository leaveRepository, ILogger<LeaveService> logger)
        {
            _leaveRepository = leaveRepository;
            _logger = logger;
        }

        public async Task<IDataResult<List<LeaveListDTO>>> GetAllAsync()
        {
            var leaves = await _leaveRepository.GetAllAsync();
            if (leaves == null)
            {
                return new ErrorDataResult<List<LeaveListDTO>>("Listeleme başarısız");
            }
            var leaveListDTOs= new List<LeaveListDTO>();

            foreach (var leave in leaves)
            {
               var leaveListDTO=leave.Adapt<LeaveListDTO>();
                
                leaveListDTO.CompanyId = leave.Manager.CompanyId;
                leaveListDTO.LeaveTypeType=leave.LeaveType.Type;
                leaveListDTO.Roles = leave.Manager.Roles;
                leaveListDTO.ManagerFirstName = leave.Manager.FirstName;
                leaveListDTO.ManagerLastName = leave.Manager.LastName;

                leaveListDTOs.Add(leaveListDTO);
            }
            

            return new SuccessDataResult<List<LeaveListDTO>>(leaves.Adapt<List<LeaveListDTO>>(), "Listeleme başarılı");
        }

        public async Task<IDataResult<LeaveDTO>> CreateAsync(LeaveCreateDTO leaveCreateDTO)
        {
            var newLeave = leaveCreateDTO.Adapt<Leave>();
            newLeave.ManagerId = leaveCreateDTO.ManagerId;
            newLeave.LeaveTypeId = leaveCreateDTO.LeaveTypeId;
            newLeave.LeaveStatus = LeaveStatus.Pending;

            await _leaveRepository.AddAsync(newLeave);
            await _leaveRepository.SaveChangesAsync();
            return new SuccessDataResult<LeaveDTO>(newLeave.Adapt<LeaveDTO>(), "İzin başarıyla eklendi");
        }

        public async Task<IResult> DeleteAsync(Guid id)
        {
            var deletingLeave = await _leaveRepository.GetByIdAsync(id);
            if (deletingLeave == null)
            {
                return new ErrorResult("Silinecek izin bulunamadı.");
            }

            await _leaveRepository.DeleteAsync(deletingLeave);
            await _leaveRepository.SaveChangesAsync();
            return new SuccessResult("İzin başarıyla silindi.");
        }

        public async Task<IDataResult<LeaveDTO>> GetByIdAsync(Guid id)
        {
            var leave = await _leaveRepository.GetByIdAsync(id);
            if (leave == null)
            {
                return new ErrorDataResult<LeaveDTO>("Veri bulunamadı.");
            }

            // Mapster ile LeaveDTO'ya mapleme
            var leaveDTO = leave.Adapt<LeaveDTO>();

            return new SuccessDataResult<LeaveDTO>(leaveDTO, "Veri başarıyla bulundu");
        }

        public async Task<IDataResult<LeaveDTO>> UpdateAsync(LeaveUpdateDTO leaveUpdateDTO)
        {
            var leave = await _leaveRepository.GetByIdAsync(leaveUpdateDTO.Id);
            if (leave == null)
            {
                return new ErrorDataResult<LeaveDTO>("Güncellenecek izin bulunamadı.");
            }

            leave.StartDate = leaveUpdateDTO.StartDate;
            leave.EndDate = leaveUpdateDTO.EndDate;
            leave.LeaveTypeId = leaveUpdateDTO.LeaveTypeId;
            leave.ManagerId = leaveUpdateDTO.ManagerId;
            leave.LeaveStatus = leaveUpdateDTO.LeaveStatus;

            try
            {
                await _leaveRepository.UpdateAsync(leave);
                await _leaveRepository.SaveChangesAsync();
                return new SuccessDataResult<LeaveDTO>(leave.Adapt<LeaveDTO>(), "İzin güncelleme başarılı.");
            }
            catch (Exception ex)
            {
               
                return new ErrorDataResult<LeaveDTO>("İzin güncelleme sırasında bir hata oluştu.");
            }
        }

        public async Task<IResult> ApproveLeaveAsync(Guid id)
        {
            var leave = await _leaveRepository.GetByIdAsync(id);
            if (leave == null)
            {
                return new ErrorResult("Onaylanacak izin bulunamadı");
            }

            leave.LeaveStatus = LeaveStatus.Approved;
            try
            {
                await _leaveRepository.UpdateAsync(leave);
                await _leaveRepository.SaveChangesAsync();

                return new SuccessDataResult<LeaveDTO>("İzin Onaylama Başarılı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Avans onaylanırken bir hata oluştu.");
                return new ErrorResult("Avans onaylama sırasında bir hata oluştu.");
            }

           
        }

        public async Task<IResult> RejectLeaveAsync(Guid id)
        {
            var leave = await _leaveRepository.GetByIdAsync(id);
            if (leave == null)
            {
                return new ErrorResult("İzin bulunamadı.");
            }

            leave.LeaveStatus = LeaveStatus.Rejected;

            try
            {
                await _leaveRepository.UpdateAsync(leave);
                await _leaveRepository.SaveChangesAsync();

                return new SuccessDataResult<LeaveDTO>("İzin reddetme");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İzin onaylanırken bir hata oluştu.");
                return new ErrorResult("İzin onaylama sırasında bir hata oluştu.");
            }

        }
    }
}

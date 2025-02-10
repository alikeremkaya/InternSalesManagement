using Ekip2.Application.DTOs.AdvanceDTOs;
using Ekip2.Domain.Entities;
using Ekip2.Domain.Enums;
using Ekip2.Infrastructure.Repositories.AdvanceRepositories;
using Mapster;
using Microsoft.Extensions.Logging;

namespace Ekip2.Application.Services.AdvanceServices
{
    public class AdvanceService : IAdvanceService
    {
        private readonly IAdvanceRepository _advanceRepository;
        private readonly ILogger<AdvanceService> _logger;

        public AdvanceService(IAdvanceRepository advanceRepository, ILogger<AdvanceService> logger)
        {
            _advanceRepository = advanceRepository;
            _logger = logger;
        }

        public async Task<IDataResult<AdvanceDTO>> CreateAsync(AdvanceCreateDTO advanceCreateDTO)
        {
            var newAdvance = advanceCreateDTO.Adapt<Advance>();
            newAdvance.AdvanceStatus = AdvanceStatus.Pending;

            try
            {
                await _advanceRepository.AddAsync(newAdvance);
                await _advanceRepository.SaveChangesAsync();
                return new SuccessDataResult<AdvanceDTO>(newAdvance.Adapt<AdvanceDTO>(), "Avans başarıyla eklendi.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Avans eklenirken bir hata oluştu.");
                return new ErrorDataResult<AdvanceDTO>(newAdvance.Adapt<AdvanceDTO>(), "Avans ekleme başarısız: " + ex.Message);
            }
        }

        public async Task<IResult> DeleteAsync(Guid id)
        {
            var deletingAdvance = await _advanceRepository.GetByIdAsync(id);

            if (deletingAdvance is null)
            {
                return new ErrorResult("Silinecek avans bulunamadı.");
            }
            await _advanceRepository.DeleteAsync(deletingAdvance);
            await _advanceRepository.SaveChangesAsync();
            return new SuccessResult("Avans başarıyla silindi.");
        }

        public async Task<IDataResult<List<AdvanceListDTO>>> GetAllAsync()
        {
            var advances = await _advanceRepository.GetAllAsync(x => x.CreatedDate, true);

            if (advances.Count() <= 0)
            {
                return new ErrorDataResult<List<AdvanceListDTO>>(advances.Adapt<List<AdvanceListDTO>>(), "Listelenecek avans bulunamadı.");
            }

            var advanceListDTOs = new List<AdvanceListDTO>();

            foreach (var advance in advances)
            {
                var advanceListDTO = advance.Adapt<AdvanceListDTO>();

                // Manager rol bilgisi ve diğer gerekli bilgileri DTO'ya ekleyin
                advanceListDTO.ManagerFirstName = advance.Manager.FirstName;
                advanceListDTO.ManagerLastName = advance.Manager.LastName;
                advanceListDTO.Roles = advance.Manager.Roles; // assuming `Manager` entity has a `Role` property
                advanceListDTO.CompanyId = advance.Manager.CompanyId;
                advanceListDTOs.Add(advanceListDTO);
            }

            return new SuccessDataResult<List<AdvanceListDTO>>(advanceListDTOs, "Avans listeleme başarılı.");
        }

        public async Task<IDataResult<AdvanceDTO>> GetByIdAsync(Guid id)
        {
            var advance = await _advanceRepository.GetByIdAsync(id);

            if (advance is null)
            {
                return new ErrorDataResult<AdvanceDTO>("Gösterilecek avans bulunamadı.");
            }
            var advanceDto = advance.Adapt<AdvanceDTO>();

            return new SuccessDataResult<AdvanceDTO>(advanceDto, "Avans gösterme işlemi başarılı.");
        }

        public async Task<IDataResult<AdvanceDTO>> UpdateAsync(AdvanceUpdateDTO advanceUpdateDTO)
        {
            var advance = await _advanceRepository.GetByIdAsync(advanceUpdateDTO.Id);
            if (advance == null)
            {
                return new ErrorDataResult<AdvanceDTO>("Güncellenecek avans bulunamadı.");
            }

            advance.Amount = advanceUpdateDTO.Amount;
            advance.AdvanceDate = advanceUpdateDTO.AdvanceDate;
            advance.ManagerId = advanceUpdateDTO.ManagerId;

            if (advanceUpdateDTO.Image != null && advanceUpdateDTO.Image.Length > 0)
            {
                advance.Image = advanceUpdateDTO.Image;
            }

            advance.AdvanceStatus = advanceUpdateDTO.AdvanceStatus;

            try
            {
                await _advanceRepository.UpdateAsync(advance);
                await _advanceRepository.SaveChangesAsync();
                return new SuccessDataResult<AdvanceDTO>(advance.Adapt<AdvanceDTO>(), "Avans güncelleme başarılı.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Avans güncellenirken bir hata oluştu.");
                return new ErrorDataResult<AdvanceDTO>("Avans güncelleme sırasında bir hata oluştu.");
            }
        }

        public async Task<IResult> ApproveAsync(Guid id)
        {
            var advance = await _advanceRepository.GetByIdAsync(id);
            if (advance == null)
            {
                return new ErrorResult("Onaylanacak avans bulunamadı.");
            }

            advance.AdvanceStatus = AdvanceStatus.Approved;

            try
            {
                await _advanceRepository.UpdateAsync(advance);
                await _advanceRepository.SaveChangesAsync();
                return new SuccessResult("Avans onaylama başarılı.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Avans onaylanırken bir hata oluştu.");
                return new ErrorResult("Avans onaylama sırasında bir hata oluştu.");
            }
        }
        public async Task<IDataResult<List<AdvanceListDTO>>> GetAllByManagerIdAsync(Guid managerId)
        {
            var advances = await _advanceRepository.GetAllAsync(x => x.ManagerId == managerId);
            if (advances == null || !advances.Any())
            {
                return new ErrorDataResult<List<AdvanceListDTO>>(advances.Adapt<List<AdvanceListDTO>>(), "Bu yönetici için avans bulunamadı.");
            }
            return new SuccessDataResult<List<AdvanceListDTO>>(advances.Adapt<List<AdvanceListDTO>>(), "Avans listeleme başarılı.");
        }

        public async Task<IResult> RejectAsync(Guid id)
        {
            var advance = await _advanceRepository.GetByIdAsync(id);
            if (advance == null)
            {
                return new ErrorResult("Reddedilecek avans bulunamadı.");
            }

            advance.AdvanceStatus = AdvanceStatus.Rejected;

            try
            {
                await _advanceRepository.UpdateAsync(advance);
                await _advanceRepository.SaveChangesAsync();
                return new SuccessResult("Avans reddetme başarılı.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Avans reddedilirken bir hata oluştu.");
                return new ErrorResult("Avans reddetme sırasında bir hata oluştu.");
            }
        }
    }
}



namespace Ekip2.Application.Services.CompanyServices
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _companyRepository;

        public CompanyService(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }

        public async Task<IDataResult<CompanyDTO>> CreateAsync(CompanyCreateDTO companyCreateDTO)
        {
            DataResult<CompanyDTO> result = new ErrorDataResult<CompanyDTO>();

            var strategy = await _companyRepository.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                var transactionScope = await _companyRepository.BeginTransection().ConfigureAwait(false);
                try
                {
                    var newCompany = companyCreateDTO.Adapt<Company>();
                    await _companyRepository.AddAsync(newCompany);
                    await _companyRepository.SaveChangesAsync();

                    result = new SuccessDataResult<CompanyDTO>(newCompany.Adapt<CompanyDTO>(), "Şirket başarıyla eklendi");
                    transactionScope.Commit();
                }
                catch (Exception ex)
                {
                    result = new ErrorDataResult<CompanyDTO>("Şirket ekleme başarısız: " + ex.Message);
                    transactionScope.Rollback();
                }
                finally
                {
                    transactionScope.Dispose();
                }
            });

            return result;
        }

        public async Task<IResult> DeleteAsync(Guid id)
        {
            Result result = new ErrorResult();

            var strategy = await _companyRepository.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                var transactionScope = await _companyRepository.BeginTransection().ConfigureAwait(false);
                try
                {
                    var deletingId = await _companyRepository.GetByIdAsync(id);
                    if (deletingId == null)
                    {
                        result = new ErrorResult("Veri bulunamadı");
                        transactionScope.Rollback();
                        return;
                    }

                    await _companyRepository.DeleteAsync(deletingId);
                    await _companyRepository.SaveChangesAsync();

                    result = new SuccessResult("Silme işlemi başarılı");
                    transactionScope.Commit();
                }
                catch (Exception ex)
                {
                    result = new ErrorResult("Silme işlemi başarısız: " + ex.Message);
                    transactionScope.Rollback();
                }
                finally
                {
                    transactionScope.Dispose();
                }
            });

            return result;
        }

        public async Task<IDataResult<List<CompanyListDTO>>> GetAllAsync()
        {
            var list = await _companyRepository.GetAllAsync();
            if (list is null)
            {
                return new ErrorDataResult<List<CompanyListDTO>>("Listeleme başarısız");
            }
            return new SuccessDataResult<List<CompanyListDTO>>(list.Adapt<List<CompanyListDTO>>(), "Listeleme başarılı");
        }

        public async Task<IDataResult<CompanyDTO>> GetByIdAsync(Guid id)
        {
            var companyId = await _companyRepository.GetByIdAsync(id);
            if (companyId is not null)
            {
                return new SuccessDataResult<CompanyDTO>(companyId.Adapt<CompanyDTO>(), "Company bulundu");
            }
            return new ErrorDataResult<CompanyDTO>();
        }

        public async Task<IDataResult<CompanyDTO>> UpdateAsync(CompanyUpdateDTO companyUpdateDTO)
        {
            var updatingCompany = await _companyRepository.GetByIdAsync(companyUpdateDTO.Id);
            if (updatingCompany is null)
            {
                return new ErrorDataResult<CompanyDTO>("Güncellenecek veri bulunamadı");
            }
            var updatedCompany = companyUpdateDTO.Adapt(updatingCompany);
            await _companyRepository.UpdateAsync(updatedCompany);
            await _companyRepository.SaveChangesAsync();
            return new SuccessDataResult<CompanyDTO>(updatedCompany.Adapt<CompanyDTO>(), "Güncelleme başarılı");
        }
    }
}

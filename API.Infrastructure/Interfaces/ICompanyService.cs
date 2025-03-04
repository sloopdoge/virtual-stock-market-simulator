using API.Domain.Entities;

namespace API.Infrastructure.Interfaces;

public interface ICompanyService
{
    Task<Company?> GetById(Guid companyId);
    Task<List<Company>> GetAll();
    Task<Company?> Create(Company company);
    Task<Company?> Update(Company company);
    Task<bool> Delete(Guid companyId);
    Task<bool> IsExist(Guid companyId);
}
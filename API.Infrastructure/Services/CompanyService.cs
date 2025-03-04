using API.Domain.Entities;
using API.Infrastructure.Interfaces;
using API.Repository;
using Microsoft.Extensions.Logging;

namespace API.Infrastructure.Services;

public class CompanyService(
    ILogger<CompanyService> logger,
    CompanyDbRepository companyDbRepository) 
    : ICompanyService
{
    public async Task<Company?> GetById(Guid companyId)
    {
        try
        {
            return await companyDbRepository.GetById(companyId);
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            return null;
        }
    }

    public async Task<List<Company>> GetAll()
    {
        try
        {
            return await companyDbRepository.GetAll();
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            return [];
        }
    }

    public async Task<Company?> Create(Company company)
    {
        try
        {
            var newGuid = Guid.NewGuid();
            company.Id = newGuid;
            var res = await companyDbRepository.Create(company);
            
            return res
                ? await GetById(newGuid)
                : null;
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            return null;
        }
    }

    public async Task<Company?> Update(Company company)
    {
        try
        {
            throw new NotImplementedException();
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            return null;
        }
    }

    public async Task<bool> Delete(Guid companyId)
    {
        try
        {
            throw new NotImplementedException();
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            return false;
        }
    }

    public async Task<bool> IsExist(Guid companyId)
    {
        try
        {
            throw new NotImplementedException();
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            return false;
        }
    }
}
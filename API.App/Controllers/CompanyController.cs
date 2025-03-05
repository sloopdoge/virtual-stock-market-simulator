using API.Domain.Entities;
using API.Domain.Models;
using API.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.App.Controllers;

[ApiController]
[Route($"api/[controller]")]
public class CompanyController(
    ILogger<CompanyController> logger,
    ICompanyService companyService) 
    : ControllerBase
{
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Company))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpGet("GetById/{companyId}")]
    [Authorize]
    public async Task<IActionResult> GetCompanyById(Guid companyId)
    {
        try
        {
            if (companyId == Guid.Empty)
                return BadRequest("Company Id cannot be empty");
            
            var company = await companyService.GetById(companyId);
            
            return company != null 
                ? Ok(company) 
                : NotFound();
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
    
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Company))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status418ImATeapot)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPost]
    [Authorize(Policy = "AdminPermissions")]
    public async Task<IActionResult> CreateCompany([FromBody] CreateCompanyModel? companyModel)
    {
        try
        {
            if (companyModel is null || !companyModel.IsValid())
                return BadRequest($"{nameof(CreateCompanyModel)} is invalid");
            
            var createRes = await companyService.Create(new Company(companyModel));
            
            return createRes != null 
                ? Ok(createRes) 
                : StatusCode(418);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
    
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Company))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status418ImATeapot)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPut]
    [Authorize(Policy = "AdminPermissions")]
    public async Task<IActionResult> UpdateCompany([FromBody] Company? company)
    {
        try
        {
            if (company is null || company.IsValid() || company.Id == Guid.Empty)
                return BadRequest("Company model is invalid");
            
            var updateRes = await companyService.Update(company);
            
            return updateRes != null 
                ? Ok(updateRes) 
                : StatusCode(418);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
    
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status418ImATeapot)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpDelete("{companyId}")]
    [Authorize(Policy = "AdminPermissions")]
    public async Task<IActionResult> DeleteCompanyById(Guid companyId)
    {
        try
        {
            if (companyId == Guid.Empty)
                return BadRequest("Company Id cannot be empty");
            
            var deleteRes = await companyService.Delete(companyId);
            
            return deleteRes 
                ? NoContent() 
                : StatusCode(418);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
    
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Company>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpGet("GetAll")]
    [Authorize]
    public async Task<IActionResult> GetAllCompanies()
    {
        try
        {
            var companies = await companyService.GetAll();
            
            return companies.Any() 
                ? Ok(companies) 
                : NotFound();
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
}
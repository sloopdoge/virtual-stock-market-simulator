using API.Domain.Entities;
using API.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.App.Controllers;

[ApiController]
[Route($"api/[controller]")]
public class StockController(
    ILogger<StockController> logger, 
    IStockService stockService) 
    : ControllerBase
{
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Stock))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpGet("GetById/{stockId}")]
    [Authorize]
    public async Task<IActionResult> GetStockById(long? stockId)
    {
        try
        {
            if (stockId is null)
                return BadRequest("Company Id cannot be empty");
            
            var stock = await stockService.GetById(stockId.Value);
            
            return stock != null 
                ? Ok(stock) 
                : NotFound();
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
    
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Stock))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status418ImATeapot)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPost]
    [Authorize(Policy = "AdminPermissions")]
    public async Task<IActionResult> CreateStock([FromBody] Stock? stock)
    {
        try
        {
            if (stock is null || stock.IsValid())
                return BadRequest("Stock model is invalid");
            
            var createRes = await stockService.Create(stock);
            
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
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Stock))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status418ImATeapot)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPut]
    [Authorize(Policy = "AdminPermissions")]
    public async Task<IActionResult> UpdateStock([FromBody] Stock? stock)
    {
        try
        {
            if (stock is null || !stock.IsValid() || stock.Id == 0)
                return BadRequest("Stock model is invalid");
            
            var updateRes = await stockService.Update(stock);
            
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
    public async Task<IActionResult> DeleteStockById(long stockId)
    {
        try
        {
            if (stockId == 0)
                return BadRequest("Stock Id cannot be empty");
            
            var deleteRes = await stockService.Delete(stockId);
            
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
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Stock>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpGet("GetAll")]
    [Authorize]
    public async Task<IActionResult> GetAllStocks()
    {
        try
        {
            var companies = await stockService.GetAll();
            
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
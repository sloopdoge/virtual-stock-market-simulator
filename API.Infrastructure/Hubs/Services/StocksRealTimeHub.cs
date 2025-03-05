using API.Infrastructure.Hubs.Interfaces;
using Microsoft.Extensions.Logging;

namespace API.Infrastructure.Hubs.Services;

public class StocksRealTimeHub(
    ILogger<StocksRealTimeHub> logger, 
    StocksHub hub) 
    : IStocksRealTimeHub
{
    
}
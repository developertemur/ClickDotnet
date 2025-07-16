using Microsoft.AspNetCore.Mvc;

namespace ClickDotnet.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class PaymeController : ControllerBase
{

    private readonly ILogger<PaymeController> _logger;

    public PaymeController(ILogger<PaymeController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Index()
    {
        _logger.LogInformation("Payme Index action called.");
        
        return Ok("Payme Index action completed successfully.");
    }
   
}

using Microsoft.AspNetCore.Mvc;

namespace ClickDotnet.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class HomeController : ControllerBase
{

    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Prepare()
    {
        _logger.LogWarning("Preparation...");
        return await Task.FromResult(Ok("Preparation!"));
    }
    [HttpPost]
    public async Task<IActionResult> Complete()
    {
        _logger.LogWarning("Complete");
        return await Task.FromResult(Ok("Complete!"));
    }
}

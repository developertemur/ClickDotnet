using Microsoft.AspNetCore.Mvc;

namespace ClickDotnet.Controllers;

[ApiController]
[Route("[controller]")]
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
        Console.WriteLine("Preparation...");
        return await Task.FromResult(Ok("Preparation!"));
    }
    public async Task<IActionResult> Complete()
    {
        Console.WriteLine("Complete");
        return await Task.FromResult(Ok("Complete!"));
    }
}

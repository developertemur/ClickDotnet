using System.Text.Json;
using System.Web;
using ClickDotnet.Models;
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
        _logger.LogWarning("Preparation..." + DateTime.UtcNow.ToLongTimeString());
        var sr = new StreamReader(this.HttpContext.Request.Body);
        var body = await sr.ReadToEndAsync();
        var dict = HttpUtility.ParseQueryString(body);
        var jsonString = JsonSerializer.Serialize(
                    dict.AllKeys.ToDictionary(k => k, k => dict[k])
           );
        _logger.LogWarning(jsonString);
        PrepareReply json = new PrepareReply()
        {
            click_trans_id = long.Parse(dict["click_trans_id"]),
            error = 0,
            error_note = "Success",
            merchant_prepare_id = 777,//example id of prepare
            merchant_trans_id = dict["merchant_trans_id"]
        };
        if (json == null)
        {
            _logger.LogError("Failed to deserialize request body.");
            return BadRequest(new ErrorReply()
            {
                error = 1,
                error_note = "Invalid request format."
            });
        }
        return Ok(new PrepareReply()
        {
            click_trans_id = json.click_trans_id,
            error = 0,
            error_note = "Success",
            merchant_prepare_id = json.merchant_prepare_id,
            merchant_trans_id = json.merchant_trans_id
        });
    }
    [HttpPost]
    public async Task<IActionResult> Complete()
    {
        _logger.LogWarning("Complete..."+DateTime.UtcNow.ToLongTimeString());
        var sr=new StreamReader(this.HttpContext.Request.Body);
        var body = await sr.ReadToEndAsync();
        var dict = HttpUtility.ParseQueryString(body);
        var jsonString = JsonSerializer.Serialize(
                    dict.AllKeys.ToDictionary(k => k, k => dict[k])
           );
        _logger.LogWarning(jsonString);
        CompleteReply json = new CompleteReply()
        {
            click_trans_id = long.Parse(dict["click_trans_id"]),
            error = 0,
            error_note = "Success",
            merchant_confirm_id = 888,//example id of confirm
            merchant_trans_id = dict["merchant_trans_id"]
        };
        if (json == null)
        {
            _logger.LogError("Failed to deserialize request body.");
            return BadRequest(new ErrorReply()
            {
                error = 1,
                error_note = "Invalid request format."
            });
        }
        return Ok(new CompleteReply()
        {
            click_trans_id = json.click_trans_id,
            error = 0,
            error_note = "Success",
            merchant_confirm_id = json.merchant_confirm_id,
            merchant_trans_id = json.merchant_trans_id
        });
    }
}

using ClickDotnet.Models;
using Microsoft.AspNetCore.Mvc;
using ClickDotnet.PaymeModels;
using System.Transactions;
using System.Text.Json;

namespace ClickDotnet.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class PaymeController : ControllerBase
{

    private static Dictionary<string,PaymeOrder> orders = new Dictionary<string, PaymeOrder>(){
        {"1234", new PaymeOrder()
                    {
                        chat_id = "1234",
                        transaction_id = "tx123456789",
                        transaction = "transaction123",
                        amount = 10000,
                        state = 0, // new
                        create_time = 0,
                        perform_time = 0,
                        cancel_time = 0,
                        reason = 0 // no reason
                    }
        }
    };


    private static string[] methods = new string[]
    {
        "CheckPerformTransaction",
        "PerformTransaction",
        "CheckTransaction",
        "CreateTransaction",
        "CancelTransaction",
        "GetStatement",
    };
    private readonly ILogger<PaymeController> _logger;

    public PaymeController(ILogger<PaymeController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Index()
    {
        _logger.LogInformation("Payme Index action called.");
        var base64 = "UGF5Y29tOjNpbXZqY3RaZEVrdVVHR2NONURYekVJdk5RWTZRckQycGVxdg==";
        var auth = Request.Headers["Authorization"];
        var body = await new StreamReader(Request.Body).ReadToEndAsync();
        var element = await JsonSerializer.DeserializeAsync<JsonElement>(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(body)));
        var id = element.GetProperty("id").GetInt32();
        var method = element.GetProperty("method").GetString();
        var params_ = element.GetProperty("params");

        if (auth != $"Basic {base64}")
        {
            return Ok(new PaymeErrorReply
            {
                error = new PaymeError
                {
                    code = -32504,
                    message = new Message
                    {
                        ru = "Неверный токен авторизации",
                        uz = "Noto'g'ri avtorizatsiya tokeni",
                        en = "Invalid authorization token"
                    },
                    data = "chat_id"
                },
                id = id
            });
        }
        if (!methods.Contains(method))
        {
            return Ok(new PaymeErrorReply
            {
                error = new PaymeError
                {
                    code = -32601,
                    message = new Message
                    {
                        ru = "Метод не найден",
                        uz = "Metod topilmadi",
                        en = "Method not found"
                    },
                    data = "chat_id"
                },
                id = id
            });
        }

        if (method == "CheckPerformTransaction" || method == "CreateTransaction")
        {
            var amount = params_.GetProperty("amount").GetInt32();
            var chat_id = params_.GetProperty("account").GetProperty("chat_id").GetString();
            if (chat_id != orders[chat_id].chat_id)
            {
                return Ok(new PaymeErrorReply
                {
                    error = new PaymeError
                    {
                        code = -31050,
                        message = new Message
                        {
                            ru = "Неверный параметр chat_id",
                            uz = "Noto'g'ri chat_id parametri",
                            en = "Invalid parameter chat_id"
                        },
                        data = "chat_id"
                    },
                    id = id
                });
            }

            if (amount != orders[chat_id].amount)
            {
                return Ok(new PaymeErrorReply
                {
                    error = new PaymeError
                    {
                        code = -31001,
                        message = new Message
                        {
                            ru = "Неверный параметр amount",
                            uz = "Noto'g'ri amount parametri",
                            en = "Invalid parameter amount"
                        },
                        data = "null"
                    },
                    id = 0
                });
            }

            if (method == "CreateTransaction")
            {
                return CreateTransaction(params_, id);
            }
            else
            {
                return CheckPerformTransaction(params_, id);
            }
        }
        if (method == "CheckTransaction") return CheckTransaction(params_, id);
        if (method == "PerformTransaction") return PerformTransaction(params_, id);
        if (method == "CancelTransaction") return CancelTransaction(params_, id);
        if (method == "GetStatement") return GetStatement(params_, id);
        return Ok("ok");
    }

    private IActionResult GetStatement(JsonElement params_, int id)
    {
        var from = params_.GetProperty("from").GetInt64();
        var to = params_.GetProperty("to").GetInt64();
        var ordersInRange = orders.Values
            .Where(o => o.create_time >= from && o.create_time <= to)
            .Select(o => new
            {
                id = o.transaction_id,
                amount = o.amount,
                account = new
                {
                    chat_id = o.chat_id
                },
                create_time = o.create_time,
                perform_time = o.perform_time,
                cancel_time = o.cancel_time,
                transaction = o.transaction,
                o.state,
                reason = o.reason,
                receivers = new[]
                {
                    new
                    {
                        chat_id = o.chat_id,
                        amount = o.amount
                    }
                }
            })
            .ToList();
        return Ok(new{
            result = new
            {
                transactions = ordersInRange
            }
        });

    }

    private IActionResult CancelTransaction(JsonElement params_, int id)
    {
        var transaction = params_.GetProperty("id").GetString();
        if (!orders.Values.Any(o => o.transaction_id == transaction))
        {
            return Ok(new PaymeErrorReply
            {
                error = new PaymeError
                {
                    code = -31003,
                    message = new Message
                    {
                        ru = "Транзакция не найдена",
                        uz = "Tranzaksiya topilmadi",
                        en = "Transaction not found"
                    },
                    data = "transaction_id"
                },
                id = id
            });
        }
        var order = orders.First(o => o.Value.transaction_id == transaction).Value;
        if (order.state == 2)
        {
            order.state = -2; // transaction rollback
            order.cancel_time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            order.reason=5; // 5 = user cancelled
        }
        return Ok(new
        {
            jsonrpc = "2.0",
            result = new
            {
                transaction = order.transaction_id,
                cancel_time = order.cancel_time,
                state = order.state
            },
            id = id
        });


    }

    private IActionResult PerformTransaction(JsonElement params_, int id)
    {
        var transaction = params_.GetProperty("id").GetString();
        if (!orders.Values.Any(o => o.transaction_id == transaction))
        {
            return Ok(new PaymeErrorReply
            {
                error = new PaymeError
                {
                    code = -31003,
                    message = new Message
                    {
                        ru = "Транзакция не найдена",
                        uz = "Tranzaksiya topilmadi",
                        en = "Transaction not found"
                    },
                    data = "transaction_id"
                },
                id = id
            });
        }
        var order = orders.First(o => o.Value.transaction_id == transaction).Value;
        if (order.state != 2)
        {
            order.state = 2; // finished
            order.perform_time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
        return Ok(new
        {
            jsonrpc = "2.0",
            result = new
            {
                transaction = order.transaction_id,
                perform_time = order.perform_time,
                state = order.state
            },
            id = id
        });

    }

    private IActionResult CheckTransaction(JsonElement params_, int id)
    {
        var transaction = params_.GetProperty("id").GetString();
        if (!orders.Values.Any(o => o.transaction_id == transaction))
        {
            return Ok(new PaymeErrorReply
            {
                error = new PaymeError
                {
                    code = -31003,
                    message = new Message
                    {
                        ru = "Транзакция не найдена",
                        uz = "Tranzaksiya topilmadi",
                        en = "Transaction not found"
                    },
                    data = "transaction_id"
                },
                id = id
            });
        }
        var order = orders.First(o => o.Value.transaction_id == transaction).Value;
        return Ok(new
        {
            jsonrpc = "2.0",
            result = new
            {
                create_time = order.create_time,
                perform_time = order.perform_time,
                cancel_time = order.cancel_time,
                transaction = order.transaction_id,
                state = order.state,
                reason = order.reason
            },
            id = id
        });
    }

    private IActionResult CreateTransaction(JsonElement params_, int id)
    {
        var transaction = params_.GetProperty("id").GetString();
        var time = params_.GetProperty("time").GetInt64();
        var chat_id = params_.GetProperty("account").GetProperty("chat_id").GetString();
        if (!orders.Values.Any(o=>o.transaction_id == transaction))
        {
            var newOrder = new PaymeOrder
            {
                chat_id = chat_id,
                transaction_id = transaction,
                transaction = Random.Shared.Next(1000, 9999).ToString(), // Simulating a new transaction ID
                amount = params_.GetProperty("amount").GetInt32(),
                state = 1, // completed
                create_time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                perform_time = 0,
                cancel_time = 0,
                reason = null // no reason
            };
            orders.Add(newOrder.transaction, newOrder);
        }
        return Ok(new
            {
                jsonrpc = "2.0",
                result = new
                {
                    create_time = orders.First(o=>o.Value.transaction_id==transaction).Value.create_time,
                    state = 1, // completed
                    transaction = orders.First(o=>o.Value.transaction_id==transaction).Value.transaction_id,//new order id
                },
                id = id
            });
    }

    private IActionResult CheckPerformTransaction(JsonElement params_,int id)
    {
        return Ok(new
        {
            jsonrpc = "2.0",
            result = new
            {
                allow = true
            },
            id= id
        });
    }
}

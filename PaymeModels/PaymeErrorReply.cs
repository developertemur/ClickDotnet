namespace ClickDotnet.PaymeModels;

public class PaymeError
{
    public int code { get; set; }
    public Message message { get; set; }
    public string data { get; set; }
}

public class Message
{
    public string ru { get; set; }
    public string uz { get; set; }
    public string en { get; set; }
}

public class PaymeErrorReply
{
    public string jsonrpc { get; set; }= "2.0";
    public PaymeError error { get; set; }
    public int id { get; set; }
}

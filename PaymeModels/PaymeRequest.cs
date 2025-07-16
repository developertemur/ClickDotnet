namespace ClickDotnet.PaymeModels;
public class Account
{
    public string chat_id { get; set; }
}

public class Params
{
    public string id { get; set; }
    public long time { get; set; }
    public int amount { get; set; }
    public Account account { get; set; }
}

public class PaymeRequest
{
    public string method { get; set; }
    public Params @params { get; set; }
    public int id { get; set; }
}

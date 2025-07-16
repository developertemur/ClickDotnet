namespace ClickDotnet.PaymeModels;
public class Receiver
{
    public string id { get; set; }
    public int amount { get; set; }
}

public class Result
{
    public string id { get; set; }
    public long time { get; set; }
    public List<Receiver> receivers { get; set; }
}

public class PaymeResponse
{
    public Result result { get; set; }
    public int id { get; set; }
}

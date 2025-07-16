namespace ClickDotnet.PaymeModels;

public class PaymeOrder
{
    public string chat_id { get; set; }
    public string transaction_id { get; set; } // Nullable, can be None
    public string transaction { get; set; }
    public int amount { get; set; }
    public int state { get; set; } // 0 = new, 1 = completed, -1 = cancelled, 2 = finished
    public long create_time { get; set; }
    public long perform_time { get; set; }
    public long cancel_time { get; set; }
    public int? reason { get; set; } // 1, 2, 3, 4, 5, 10
}
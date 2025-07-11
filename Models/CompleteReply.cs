namespace ClickDotnet.Models;

public class CompleteReply
{
    public long click_trans_id { get; set; }
    public string merchant_trans_id { get; set; }
    public int merchant_confirm_id { get; set; }
    public int error { get; set; }
    public string error_note { get; set; }
    
}
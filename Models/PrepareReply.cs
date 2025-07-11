namespace ClickDotnet.Models;

public class PrepareReply
{
    public string click_trans_id { get; set; }
    public string merchant_trans_id { get; set; }
    public int merchant_prepare_id { get; set; }
    public int error { get; set; }
    public string error_note { get; set; }
    
}
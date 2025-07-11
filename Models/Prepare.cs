namespace ClickDotnet.Models;

public class Prepare
{
    public string click_trans_id { get; set; }
    public int service_id { get; set; }
    public string click_paydoc_id { get; set; }
    public string merchant_trans_id { get; set; }
    public float amount { get; set; }
    public int action { get; set; } //0 for prepare
    public int error { get; set; } //0 for success
    public string error_note { get; set; }
    public string sign_time { get; set; }
    public string sign_string { get; set; }
    
}
namespace ClickDotnet.Models;

public static class Consts
{
    public static Dictionary<int, Error> Errors = new Dictionary<int, Error>()
    {
        { 0, new Error { error_note = "Success", description = "Successful request" } },
        { -1, new Error { error_note = "SIGN CHECK FAILED!", description = "Signature verification error" } },
        { -2, new Error { error_note = "Incorrect parameter amount", description = "Invalid payment amount" } },
        { -3, new Error { error_note = "Action not found", description = "The requested action is not found" } },
        { -4, new Error { error_note = "Already paid", description = "The transaction was previously confirmed (when trying to confirm or cancel the previously confirmed transaction)" } },
        { -5, new Error { error_note = "User does not exist", description = "Do not find a user / order (check parameter merchant_trans_id)" } },
        { -6, new Error { error_note = "Transaction does not exist", description = "The transaction is not found (check parameter merchant_prepare_id)" } },
        { -7, new Error { error_note = "Failed to update user", description = "An error occurred while changing user data (changing account balance, etc.)" } },
        { -8, new Error { error_note = "Error in request from click", description = "The error in the request from CLICK (not all transmitted parameters, etc.)" } },
        { -9, new Error { error_note = "Transaction cancelled", description = "The transaction was previously canceled (When you attempt to confirm or cancel the previously canceled transaction)" } }
    };
}
public class Error
{
    public string error_note { get; set; }
    public string description { get; set; }
}
using System.Security.Cryptography;
using System.Text;
using ClickDotnet.Models;

namespace ClickDotnet.Services;

public class PrepareService
{
    private IConfiguration _config;

    public PrepareService(IConfiguration config)
    {
        _config = config;
    }
    public static string CreateMD5(string input)
    {
        // Use input string to calculate MD5 hash
        using (MD5 md5 = MD5.Create())
        {
            byte[] hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Convert the byte array to hexadecimal string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("x2")); // Use "x2" for lowercase
            }

            return sb.ToString();
        }
    }
    public ErrorReply RequestCheck(Prepare prepare) {

        // prepare sign string as md5 digest
        var sign_string = CreateMD5(
            prepare.click_trans_id +
            prepare.service_id.ToString() +
            _config["secret_key"] +
            prepare.merchant_trans_id +
            "" +
            prepare.amount.ToString() +
            prepare.action.ToString() +
            prepare.sign_time.ToString()
        );

        // check sign string to possible
        if (sign_string != prepare.sign_string) {
            // return response array-like
            return new ErrorReply
            {
                error = -1,
                error_note = "SIGN CHECK FAILED!"
            };
        }

        // check to action not found error
        if (!(prepare.action == 0 || prepare.action == 1)) {
            // return response array-like
            return new ErrorReply
            {
                error = -3,
                error_note = "Action not found"
            };
        }

        // get payment data by merchant_trans_id
        // $payment = $this->model->find_by_merchant_trans_id($request['merchant_trans_id']);

        // if (!$payment){
        //     // return response array-like
        //     return [
        //         'error' => -5,
        //         'error_note' => 'User does not exist'
        //     ];
        // }

        // get payment data by merchant_prepare_id
        // if ( $request['action'] == 1 ) {
        //     $payment = $this->model->find_by_id($request['merchant_prepare_id']);
        //     if (!$payment){
        //         // return response array-like
        //         return [
        //             'error' => -6,
        //             'error_note' => 'Transaction does not exist	'
        //         ];
        //     }
        // }


        // check to already paid
        // if ($payment['status'] == PaymentsStatus::CONFIRMED){
        //     // return response array-like
        //     return [
        //         'error' => -4,
        //         'error_note' => 'Already paid'
        //     ];
        // }

        // check to correct amount
        // if (abs((float)$payment['total'] - (float)$request['amount']) > 0.01) {
        //     // return response array-like
        //     return [
        //         'error' => -2,
        //         'error_note' => 'Incorrect parameter amount'
        //     ];
        // }

        // check status to transaction cancelled
        // if ($payment['status'] == PaymentsStatus::REJECTED){
        //     // return response array-like
        //     return [
        //         'error' => -9,
        //         'error_note' => 'Transaction cancelled'
        //     ];
        // }

        // return response array-like as success
        return new ErrorReply{
            error = 0,
            error_note = "Success"
        };

    }
}
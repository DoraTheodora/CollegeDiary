using System;
using Firebase.Auth;
using CD.Helper;
using System.Threading.Tasks;
using Xamarin.Forms;
using CD.iOS;

[assembly: Dependency(typeof(FirebaseForgotPassword))]
namespace CD.iOS
{
    class FirebaseForgotPassword: IFirebaseForgotPassword
    {
        public async Task<String> ForgotPassword(string email)
        {
            try
            {
                await Auth.DefaultInstance.SendPasswordResetAsync(email);
                return "emailSent";
            }
            catch (Exception ex)
            {
                if (ex.Message.ToString().Contains("17011"))
                {
                    return "emailNotFound";
                }
                else
                {
                    return "Error";
                }
            }
        }
    }
}
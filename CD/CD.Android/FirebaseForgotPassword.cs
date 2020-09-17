using System;
using Firebase.Auth;
using CD.Helper;
using System.Threading.Tasks;
using Xamarin.Forms;
using CD.Droid;

[assembly: Dependency(typeof(FirebaseForgotPassword))]
namespace CD.Droid
{
    class FirebaseForgotPassword: IFirebaseForgotPassword
    {
        public async Task<String> ForgotPassword(string email)
        {
            try
            {
                await FirebaseAuth.Instance.SendPasswordResetEmailAsync(email);
                return "emailSent";
            }
            catch (Exception ex)
            {
                if (ex is FirebaseAuthInvalidUserException)
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
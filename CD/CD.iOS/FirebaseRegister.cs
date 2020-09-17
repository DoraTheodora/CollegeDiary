using System;
using Firebase.Auth;
using CD.Helper;
using System.Threading.Tasks;
using Xamarin.Forms;
using CD.iOS;
using Foundation;

[assembly: Dependency(typeof(FirebaseRegister))]
namespace CD.iOS
{
    public class FirebaseRegister: IFirebaseRegister
    {
        public string UserID;
        public async Task<string> RegisterWithEmailAndPassword(string email, string password)
        {
            try
            {
                var user = await Auth.DefaultInstance.CreateUserAsync(email, password);
                var token = await user.User.GetIdTokenAsync(false);
                await user.User.SendEmailVerificationAsync();
                App.UserUID = user.User.Uid.ToString();
                return token;
            }
            catch (Exception ex) 
            {
                if (ex.Message.ToString().Contains("17007"))
                {
                    return "existing";
                }
                else
                {
                    return "";
                }
            }
        }
        public string UserUID()
        {
            return UserID;
        }
    }
}
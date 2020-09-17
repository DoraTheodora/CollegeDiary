using System;
using Firebase.Auth;
using CD.Helper;
using System.Threading.Tasks;
using Xamarin.Forms;
using CD.Droid;

[assembly: Dependency(typeof(FirebaseRegister))]
namespace CD.Droid
{
    public class FirebaseRegister : IFirebaseRegister
    {
        public string UserID;
        public async Task<string> RegisterWithEmailAndPassword(string email, string password)
        {
            try
            {
                var user = await FirebaseAuth.Instance.CreateUserWithEmailAndPasswordAsync(email, password);
                var token = await user.User.GetIdTokenAsync(false);
                user.User.SendEmailVerification();
                App.UserUID = user.User.Uid;
                return token.Token;
            }
            catch (FirebaseAuthUserCollisionException)
            {
                return "existing";
            }
            catch (Exception)
            {
                return "";
            }
        }

        public string UserUID()
        {
            return UserID;
        }
    }

}
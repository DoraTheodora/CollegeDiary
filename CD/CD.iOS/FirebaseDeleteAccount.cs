using System;
using Firebase.Auth;
using CD.Helper;
using System.Threading.Tasks;
using Xamarin.Forms;
using CD.iOS;

[assembly: Dependency(typeof(FirebaseDeleteAccount))]
namespace CD.iOS
{
    public class FirebaseDeleteAccount : IFirebaseDeleteAccount
    {
        public async Task DeleteAccount()
        {
            try
            {
                await Auth.DefaultInstance.CurrentUser.DeleteAsync();
            }
            catch (Exception)
            {

            }
        }
    }
}
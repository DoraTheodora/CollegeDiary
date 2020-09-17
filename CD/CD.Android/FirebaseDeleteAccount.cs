using System;
using Firebase.Auth;
using CD.Helper;
using System.Threading.Tasks;
using Xamarin.Forms;
using CD.Droid;

[assembly: Dependency(typeof(FirebaseDeleteAccount))]
namespace CD.Droid
{
    public class FirebaseDeleteAccount: IFirebaseDeleteAccount
    {
        public async Task DeleteAccount()
        {
            try 
            {
                await FirebaseAuth.Instance.CurrentUser.DeleteAsync();
            }
            catch(Exception ex)
            {
                Console.WriteLine("+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++" + ex.ToString());
            }
        }
    }
}
using System;
using Firebase.Auth;
using CD.Helper;
using System.Threading.Tasks;
using Xamarin.Forms;
using CD.Droid;

[assembly: Dependency(typeof(FirebaseSignOut))]
namespace CD.Droid
{
    public class FirebaseSignOut: IFirebaseSignOut
    {
        public async Task SignOut()
        {
            try
            {
                 FirebaseAuth.Instance.SignOut();
                 App.Current.Properties.Remove("App.UserUID");
                 await App.Current.SavePropertiesAsync();
            }
            catch (Exception)
            { 
            }
        }
    }
}
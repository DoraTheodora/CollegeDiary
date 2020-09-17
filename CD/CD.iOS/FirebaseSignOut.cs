using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Firebase.Auth;
using CD.Helper;
using Foundation;
using UIKit;
using System.Threading.Tasks;
using Xamarin.Forms;
using CD.iOS;

[assembly: Dependency(typeof(FirebaseSignOut))]
namespace CD.iOS
{
    public class FirebaseSignOut: IFirebaseSignOut
    {
        public async Task SignOut()
        {
            try
            {
                Auth.DefaultInstance.SignOut(out NSError error);
            }
            catch (Exception)
            { 
            }
        }
    }
}
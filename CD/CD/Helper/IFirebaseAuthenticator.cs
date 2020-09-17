using System.Threading.Tasks;

namespace CD.Helper
{
    public interface IFirebaseAuthenticator
    {
        Task<string> LoginWithEmailPassword(string email, string password);
        bool SignOut();
        bool IsSignedIn();
    }
}

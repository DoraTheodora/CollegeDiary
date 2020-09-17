using System.Threading.Tasks;

namespace CD.Helper
{
    public interface IFirebaseRegister
    {
        Task<string> RegisterWithEmailAndPassword(string email, string password);
        string UserUID();
    }
}

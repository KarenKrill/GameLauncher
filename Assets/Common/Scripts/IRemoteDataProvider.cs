using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Common.Scripts
{
    internal interface IRemoteDataProvider : IDataProvider
    {
        bool IsSignInNeeded { get; }
        bool IsSignedIn { get; }
        Task SignUpAsync(string login, string password);
        Task SignInAsync(string login, string password);
        Task SignInAnonymouslyAsync();
        Task SignOutAsync();
    }
}

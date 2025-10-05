using System.Threading.Tasks;
using UnityEngine;

namespace Sui.ZKLogin.Utils
{
    public interface IJwtFetcher
    {
        public Task<string> FetchJwt(params string[] parameters);
        public void Dispose();
    }
}


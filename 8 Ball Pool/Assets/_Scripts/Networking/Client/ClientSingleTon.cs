using System.Threading.Tasks;
using _Scripts.Utils;


namespace _Scripts.Networking.Client
{
    public class ClientSingleTon : SingletonPersistent<ClientSingleTon>
    {
        public ClientGameManager GameManager { get; private set; }


        public async Task<bool> CreateClient()
        {
            GameManager = new ClientGameManager();

            return await GameManager.InItAsync();
        }

        private void OnDestroy()
        {
            GameManager?.Dispose();
        }
    }
}
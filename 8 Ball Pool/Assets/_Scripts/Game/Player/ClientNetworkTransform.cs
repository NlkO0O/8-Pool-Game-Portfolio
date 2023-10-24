using Unity.Netcode.Components;

namespace _Scripts.Game.Player
{
    public class ClientNetworkTransform : NetworkTransform
    {
        protected override void Update()
        {
            CanCommitToTransform = IsOwner;
            base.Update();

            if (NetworkManager != null)
            {
                if (NetworkManager.IsConnectedClient || NetworkManager.IsListening)
                {
                    if (CanCommitToTransform)
                    {
                        TryCommitTransformToServer(transform, NetworkManager.LocalTime.Time);
                    }
                }
            }
        }

        protected override bool OnIsServerAuthoritative()
        {
            return false;
        }
    }
}
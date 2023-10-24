using System;

namespace _Scripts.Networking.Shared
{
    [Serializable]
    public class UserData
    {
        public string userName;
        public string userAuthId;
        public GameInfo userGamePreferences = new GameInfo();
    }

    [Serializable]
    public class GameInfo
    {
        public Map Map;
        public GameMode GameMode;
        public GameQueue GameQueue;

        public string ToMultiplayQueue()
        {
            return GameQueue switch
            {
                GameQueue.Solo => "solo-queue",
                GameQueue.Party => "party-queue",
                _ => "solo-queue",
            };
        }
    }

    public enum Map
    {
        Default
    }

    public enum GameMode
    {
        Default
    }

    public enum GameQueue
    {
        Solo,
        Party
    }
}
using System.Collections.Generic;

namespace _Scripts.UI.Matchmaker
{
    public class DisplayObject
    {
        private MessageType messageType;
        private string playerName;

        private static Dictionary<MessageType, string> messages = new Dictionary<MessageType, string>()
        {
            { MessageType.PottedCueBall, " potted the cue ball" },
            { MessageType.PlayersTurn, " turn" },
            { MessageType.IllegalShot, " made illegal shot" },
            { MessageType.Breaking, " breaking" },
            { MessageType.TimedOut, " ran out of time" }
        };

        public DisplayObject(MessageType msg, string pName)
        {
            messageType = msg;
            playerName = pName;
        }

        public string DisplayText()
        {
            string firstPart;

            if (playerName == string.Empty)
            {
                firstPart = messageType switch
                {
                    MessageType.Breaking => "You are",
                    MessageType.PlayersTurn => "Your",
                    _ => "You"
                };
            }
            else
            {
                firstPart = messageType switch
                {
                    MessageType.Breaking => $"{playerName} is",
                    MessageType.PlayersTurn => $"{playerName}'s",
                    _ => $"{playerName}"
                };
            }

            return firstPart + messages[messageType];
        }
    }
}
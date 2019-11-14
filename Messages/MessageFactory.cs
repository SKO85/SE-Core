using IngameScript.Serialization;

namespace IngameScript.Messages
{
    public class MessageFactory
    {
        public static Message Get(string name, string data)
        {
            var type = name.ToLower().Trim();

            if (type == "broadcast")
                return Serializer.DeSerialize<BroadcastMessage>(data);
            else if (type == "" || type == "message")
                return Serializer.DeSerialize<Message>(data);

            return null;
        }

        public static T Get<T>(string data) where T : Message, new()
        {
            return Serializer.DeSerialize<T>(data);
        }
    }
}
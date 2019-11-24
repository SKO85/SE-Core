using SKO85Core.Serialization;
using System.Collections.Generic;

namespace SKO85Core.Messages
{
    public class BroadcastMessage : Message
    {
        public string Data { get; set; } = "";

        public override void Load(Dictionary<string, Field> fields)
        {
            this.Data = fields["data"].GetString();
        }

        public override void Save()
        {
            fields["data"] = new Field(Data);
        }
    }
}

using IngameScript.Serialization;
using System.Collections.Generic;

namespace IngameScript.Messages
{
    public class Message : Serializable
    {
        public string Action { get; set; }
        public long Source { get; set; }
        public Dictionary<string, Field> Arguments { get; set; }

        public Message()
        {
            Action = "Unknown";
            Source = long.Parse("0");
            Arguments = new Dictionary<string, Field>();
        }

        public override void LoadFields(Dictionary<string, Field> fields)
        {
            this.Action = fields["action"].GetString();
            this.Source = fields["source"].GetLong();
            this.Arguments = fields["arguments"].children;

            this.Load(fields);
        }

        public override void SaveToFields()
        {
            fields["action"] = new Field(this.Action);
            fields["source"] = new Field(this.Source);
            fields["arguments"] = new Field(this.Arguments);
            this.Save();
        }

        public virtual void Load(Dictionary<string, Field> fields)
        {
        }

        public virtual void Save()
        {
        }
    }
}

//using IngameScript.Tasks;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using VRageMath;

//namespace IngameScript.Messages
//{
//    public class StatusMessage<T> : Message where T : Task, new()
//    {
//        public T CurrentTask { get; set; } = new T();
//        public Vector3 Position { get; set; }
//        public Vector3 Direction { get; set; }
//        public Vector3 Up { get; set; }
//        public Vector3 Forward { get; set; }
//        public Vector3 Right { get; set; }
//        public Vector3 Left { get; set; }

//        public StatusMessage() : base()
//        {
//            Name = "Status";
//        }

//        public override void Load(Dictionary<string, Field> fields)
//        {
//            this.Name = fields["name"].GetString();

//            this.CurrentTask = fields["currentTask"].GetObject<T>();
//            this.Position = fields["position"].GetVector3();
//            this.Direction = fields["direction"].GetVector3();
//            this.Up = fields["up"].GetVector3();
//            this.Forward = fields["forward"].GetVector3();
//            this.Right = fields["right"].GetVector3();
//            this.Left = fields["left"].GetVector3();
//        }

//        public override void Save()
//        {
//            fields["name"] = new Field(this.Name);
//            fields["currentTask"] = new Field(this.CurrentTask);
//        }
//    }
//}
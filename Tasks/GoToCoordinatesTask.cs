using IngameScript.Messages;
using IngameScript.Ship;
using Sandbox.ModAPI.Ingame;
using VRageMath;

namespace IngameScript.Tasks
{
    public class GoToCoordinatesTask : Task
    {
        private IMyRemoteControl RC;
        private Message Data;
        ShipControl sc;

        public GoToCoordinatesTask(MyGridProgram program, BroadcastMessage msg) : base(program, msg)
        {
            Data = MessageFactory.Get<Message>(msg.Data);
        }

        protected override void Init()
        {
            // Init resources we need.
            RC = GetBlock<IMyRemoteControl>();

            sc = new ShipControl(this.Program);
        }

        public override void Execute()
        {
            Program.Echo("Executed...");

            var xc = GetBlock<IMyShipConnector>("XC");

            Vector3D pos = Data.Arguments["coords"].GetVector3();
            Vector3D dir = Data.Arguments["dir"].GetVector3();

            Program.Echo(pos.ToString());
            Program.Echo(dir.ToString());

            //if (sc.AlignBlockTo(xc, pos, ) == false)
            //{
            //    Program.Runtime.UpdateFrequency = UpdateFrequency.Update10;
            //}
            //else
            //{
            //    Program.Runtime.UpdateFrequency = UpdateFrequency.Once;
            //    IsFinished = true;
            //}

            IsFinished = true;

            // IsFinished = true;

            //var distance = Vector3D.Distance(xc.GetPosition(), pos);

            //if (distance <= 2)
            //{
            //    RC.SetAutoPilotEnabled(false);
            //    RC.SetCollisionAvoidance(false);

            //    RC.ClearWaypoints();

            //}
            //else
            //{
            //    // Clear any waypoints.
            //    RC.ClearWaypoints();

            //    // Set coords.
            //    RC.AddWaypoint(goToPos, "TaskWaypoint");

            //    // Go one way.
            //    RC.FlightMode = FlightMode.OneWay;

            //    // Start moving...
            //    RC.SetCollisionAvoidance(true);
            //    RC.SetAutoPilotEnabled(true);
            //}

            if (!IsFinished)
            {
                IsRunning = true;
            }
            else
            {
                IsRunning = false;
            }
        }
    }
}
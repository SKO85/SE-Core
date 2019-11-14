//using Sandbox.ModAPI.Ingame;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace IngameScript
//{
//    class TaskSystem : BaseScript
//    {
//        #region TASK SYSTEM

//        public List<Task> Tasks { get; set; }
//        public List<string> Commands { get; set; }

//        public IMyProgrammableBlock PBAntenna;

//        public void Init()
//        {
//            Tasks = new List<Task>();
//            Commands = new List<string>();
//            PBAntenna = GetBlock<IMyProgrammableBlock>("PB-Antenna");

//            TaskInit();
//        }

//        public void Main(string argument, UpdateType updateSource)
//        {
//            if (!IsInit)
//                return;

//            if (!String.IsNullOrEmpty(argument))
//            {
//                // Parse the task.
//                Task t = Task.Parse(argument);
//                if (t != null)
//                {
//                    Tasks.Add(t);
//                }

//                InternalHandleTask(t);
//            }
//            else
//            {
//                // Get the frist task as it is always the current one.
//                var current = Tasks.FirstOrDefault();
//                if (current != null)
//                {
//                    InternalHandleTask(current);
//                }
//            }

//            ExecuteCommands();

//            Echo(SB.ToString());
//        }

//        public void RemoveCurrentTask()
//        {
//            Tasks.RemoveAt(0);
//        }

//        public void SendStatus()
//        {
//            var currentTask = Tasks.FirstOrDefault();

//            // Set idle task.
//            Task t = new Task();
//            t.name = "status";
//            t.source = Id;

//            // t.arguments["arguments"] = currentTask != null ? currentTask. : "idle";
//            t.arguments["type"] = Data.ContainsKey("type") ? Data["type"] : "unknown";

//            // Set position.
//            var pos = Me.CubeGrid.GetPosition();
//            t.arguments["posX"] = pos.X.ToString();
//            t.arguments["posY"] = pos.Y.ToString();
//            t.arguments["posZ"] = pos.Z.ToString();

//            // Set direction.
//            // ...

//            t.arguments["running"] = currentTask != null ? currentTask.name : "idle";
//            if (currentTask != null)
//            {
//                if (currentTask.arguments.Count > 0)
//                {
//                    foreach (var arr in currentTask.arguments)
//                    {
//                        t.arguments["arr-" + arr.Key] = arr.Value;
//                    }
//                }

//                Unicast(currentTask.source, t);
//            }
//            else
//            {
//                // Broadcast.
//                Broadcast(t);
//            }
//        }

//        public void Broadcast(Task t)
//        {
//            if (PBAntenna != null)
//            {
//                var cmd = string.Format("{0}${1}", "broadcast", t.ToString());
//                Commands.Add(cmd);
//            }
//        }

//        public void Unicast(long address, Task t)
//        {
//            if (PBAntenna != null)
//            {
//                var cmd = string.Format("{0}${1}${2}", "unicast", address.ToString(), t.ToString());
//                Commands.Add(cmd);
//            }
//        }

//        public void ExecuteCommands()
//        {
//            if (PBAntenna != null && Commands.Count > 0)
//            {
//                var indexToRemove = new List<int>();

//                for (int i = 0; i < Commands.Count; i++)
//                {
//                    if (PBAntenna.TryRun(Commands[i]))
//                    {
//                        SB.AppendLine("EXECUTE");
//                        SB.AppendLine(Commands[i]);
//                        indexToRemove.Add(i);
//                    }
//                }

//                foreach (var i in indexToRemove)
//                {
//                    Commands.RemoveAt(i);
//                }
//            }
//        }

//        public void InternalHandleTask(Task t)
//        {
//            SB.AppendLine("Received task");
//            SB.AppendLine(t.ToString());

//            if (!HandleTask(t))
//            {
//                switch (t.name.ToLower().Trim())
//                {
//                    case "sendstatus":
//                        SendStatus();
//                        RemoveCurrentTask();
//                        break;

//                    default:
//                        SB.AppendLine("Unknown task.");
//                        // Unknown task.
//                        RemoveCurrentTask();
//                        break;
//                }
//            }
//        }

//        #endregion TASK SYSTEM

//        public void TaskInit()
//        {
//            IsInit = true;
//        }

//        public bool HandleTask(Task t)
//        {
//            return false;
//        }
//    }
//}
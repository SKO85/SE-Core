using IngameScript.Work;
using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IngameScript.Abstract
{
    public abstract class Script : Grid
    {
        public enum UpdateFrequency
        {
            None = 0,
            Update1ms = 1,
            Update10ms = 10,
            Update50ms = 50,
            Update100ms = 100,
            Update1s = 1000,
            Update2s = 2000,
            Update5s = 5000,
            Update10s = 10000,
            Update15s = 15000,
            Update30s = 30000,
            Update1m = 60000,
            Update2m = 120000,
            Update3m = 180000,
            Update5m = 300000
        }

        #region Properties

        public UpdateFrequency Update = UpdateFrequency.None;
        private double ScriptRunTime { get; set; }
        protected IMyTextSurface PBScreen { get; set; }
        protected StringBuilder Output { get; set; }

        #endregion Properties

        #region Initialize

        public Script(MyGridProgram program) : base(program)
        {
            this.InternalInit();
        }

        private void InternalInit()
        {
            // Create a list for the Wait Callbacks.
            WaitList = new List<Wait>();

            // Set the PB UpdateFrequency to always be Update10.
            // The execution frequency of our own code is handled in our Main function.
            // This has a positive affect on the Avarage Execution Time of the script.
            this.Program.Runtime.UpdateFrequency = Sandbox.ModAPI.Ingame.UpdateFrequency.Update10;

            // The initial execution frequency is 10ms.
            this.Update = UpdateFrequency.Update10ms;

            // Set current PB Screen to text.
            PBScreen = this.Program.Me.GetSurface(0);
            PBScreen.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
            PBScreen.WriteText("");

            // String Builder.
            Output = new StringBuilder();

            // Init communications.
            InitCommunications();

            // Call the Init.
            this.Init();

            //// Timer for saving all configs.
            //Wait(10000, () =>
            //{
            //    // Get block ids that have configurations.
            //    foreach (var key in Configurations.Keys)
            //    {
            //        SaveConfig(Blocks.Where(b => b.EntityId == key).FirstOrDefault());
            //    }
            //    return false;
            //});

            Wait(1000, () =>
            {
                Output.AppendLine(string.Format("Avg. Execution: {0}", Math.Round(ExecutionTimeAverage, 4)));

                PBScreen.WriteText(Output.ToString());
                Output.Clear();
                return false;
            });
        }

        #endregion Initialize

        #region Avarage Execution Calculation

        private const double TickSignificance = 0.005;
        private double ExecutionTimePrevious = 0;
        private double ExecutionTimeAverage = 0;
        private int RuntimeCtr = 0;

        private void CalculateAverageExecutionTime()
        {
            if (++RuntimeCtr < 21)
                return;
            ExecutionTimeAverage = (1 - TickSignificance) * ExecutionTimePrevious + TickSignificance * Program.Runtime.LastRunTimeMs;
            ExecutionTimePrevious = ExecutionTimeAverage;
        }

        public double GetAvarageExecutionTime()
        {
            return ExecutionTimeAverage;
        }

        #endregion Avarage Execution Calculation

        #region Abstract Functions

        abstract protected void Init();
        abstract protected void Run(Queue<string> arguments);

        #endregion Abstract Functions

        #region Wait

        private List<Wait> WaitList;

        public void Wait(int ms, Func<bool> callback)
        {
            this.WaitList.Add(new Wait()
            {
                WaitTime = ms,
                Callback = callback
            });
        }

        private void HandleWait()
        {
            if (this.WaitList == null || this.WaitList.Count == 0)
            {
                return;
            }

            var waitListToCall = WaitList.Where(w => w.IsDone == false).ToList();
            if (waitListToCall.Count > 0)
            {
                foreach (var wait in waitListToCall)
                {
                    wait.LastWaitRuntime += Program.Runtime.TimeSinceLastRun.TotalMilliseconds;
                    if (wait.LastWaitRuntime >= wait.WaitTime)
                    {
                        if (wait.Callback())
                            wait.IsDone = true;

                        wait.LastWaitRuntime = 0;
                    }
                }
            }

            // Remove stuff from the list.
            if (WaitList.Count > waitListToCall.Count)
            {
                for (int i = WaitList.Count - 1; i > 0; i--)
                {
                    if (WaitList[i].IsDone)
                    {
                        WaitList.RemoveAtFast(i);
                    }
                }
            }
        }

        #endregion Wait

        #region Communication

        private string CommTagPrefix = "MyUniqueTagPrefix";
        private List<IMyBroadcastListener> BroadcastListeners { get; set; }
        private IMyUnicastListener UnicastListener { get; set; }
        private Dictionary<string, Action<MyIGCMessage>> MessageHandlers { get; set; }

        private void InitCommunications()
        {
            // Set Communication handlers.
            this.MessageHandlers = new Dictionary<string, Action<MyIGCMessage>>();
            this.BroadcastListeners = new List<IMyBroadcastListener>();
            this.UnicastListener = Program.IGC.UnicastListener;

            // Try to get tag-prefix from the config.
            CommTagPrefix = GetConfigValue("general", "tagprefix").ToString(CommTagPrefix);
        }

        protected void OnMessageAction(string action, Action<MyIGCMessage> handler)
        {
            // Register IGC Tag for this message, using unique prefix.
            // TODO: Set prefix in a configuration.
            Program.IGC.RegisterBroadcastListener(string.Format("{0}.{1}", CommTagPrefix, action));

            // Update Broadcasting listeners.
            // Do we need to update the list? Might by reference and set on Init.
            // TODO: Check this.
            Program.IGC.GetBroadcastListeners(this.BroadcastListeners);

            // Register the handler for this action.
            MessageHandlers[action] = handler;
        }

        private void HandleMessages()
        {
            // Check if there are direct messages for this PB.
            while (UnicastListener.HasPendingMessage)
            {
                // Check and handle the message.
                HandleMessage(UnicastListener.AcceptMessage());
            }

            // Check if there are any broadcast messages for the registered listeners.
            foreach (var listener in BroadcastListeners)
            {
                // Process all pending messages of this listener.
                while (listener.HasPendingMessage)
                {
                    // Check and handle the message.
                    HandleMessage(listener.AcceptMessage());
                }
            }
        }

        private void HandleMessage(MyIGCMessage msg)
        {
            if (msg.Tag.Contains("."))
            {
                var msgSplit = msg.Tag.Split('.');
                var prefix = msgSplit[0];
                var action = msgSplit[1];

                // Check if we have registered this action.
                if (MessageHandlers.ContainsKey(action))
                {
                    // Process action.
                    MessageHandlers[action](msg);
                }
            }
        }

        protected bool SendMessage(string action, string data, long targetId = 0)
        {
            return this.SendMessage<string>(action, data, 0);
        }

        protected bool SendMessage<T>(string action, T data, long targetId = 0)
        {
            // TODO: Add prefix to the tag.
            var tag = string.Format("{0}.{1}", CommTagPrefix, action);

            // If we have a targetId, then Unicast the message to the specific target.
            if (targetId > 0)
            {
                // Unicast message.
                if (!Program.IGC.SendUnicastMessage(targetId, tag, data))
                {
                    // TODO: Should we do something if data is not send?
                    return false;
                }
            }
            else
            {
                // Broadcast message.
                Program.IGC.SendBroadcastMessage(tag, data, TransmissionDistance.TransmissionDistanceMax);
            }

            return true;
        }

        #endregion Communication

        #region Main

        private Queue<string> Arguments = new Queue<string>();

        public void Main(string argument)
        {
            // Handle argument.
            if (!string.IsNullOrEmpty(argument))
            {
                Arguments.Enqueue(argument);
            }

            // If the update frequency is set to None, then simply return and do not do anything.
            if (this.Update == UpdateFrequency.None)
                return;

            // Handle messages.
            HandleMessages();

            // Get the runtime and add it to the Script Runtime.
            ScriptRunTime += Program.Runtime.TimeSinceLastRun.TotalMilliseconds;

            // If set frequency is not reached, simply return and do not do anything.
            if (ScriptRunTime < (int)Update)
                return;
            else
            {
                // Reset runtime.
                ScriptRunTime = 0;

                // Call the Script Run code.
                Run(Arguments);
            }

            // If any Timeout are present, handle them first.
            HandleWait();

            // Calculate avarage Execution Time.
            CalculateAverageExecutionTime();
        }

        #endregion Main
    }
}

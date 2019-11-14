using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IngameScript.Abstract
{
    class Script : MyGridProgram
    {
        #region BASE

        public bool IsInit { get; set; } = false;
        public string GridName { get; set; }
        public long Id { get; set; }
        public string Role { get; set; }

        private List<IMyTerminalBlock> Blocks;
        public IMyTimerBlock Timer { get; set; }

        public Dictionary<string, string> Data { get; set; }

        public StringBuilder SB { get; set; }

        public void InternalInit()
        {
            // Set update frequency to None or Once.
            // Runtime.UpdateFrequency = UpdateFrequency.None;

            // Get the name of this grid.
            GridName = Me.CubeGrid.CustomName;

            // Get the ID of the current Programming Block.
            Id = Me.EntityId;

            // Get blocks of this grid.
            Blocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType(Blocks, b => b.CubeGrid == Me.CubeGrid);

            // String Builder.
            SB = new StringBuilder();

            // Get Custom Data.
            Data = ParseCustomData(Me.CustomData);

            // Set the role of this grid.
            Role = Data.ContainsKey("role") ? Data["role"] : "unknown";

            // Call the Init.
            this.Init();
        }

        public void Wait(int seconds)
        {
            if (Timer != null)
            {
                Timer.GetActionWithName("Stop").Apply(Timer);
                Timer.TriggerDelay = seconds;
                Timer.GetActionWithName("Start").Apply(Timer);
            }
        }

        public T GetBlock<T>(string name)
        {
            var block = Blocks.Where(c => c.CustomName.ToLower().Trim() == name.ToLower().Trim()).FirstOrDefault();
            if (block != null)
            {
                return (T)block;
            }
            return default(T);
        }

        public Dictionary<string, string> ParseCustomData(string data)
        {
            var result = new Dictionary<string, string>();
            var lines = data.Split('\n');
            foreach (var line in lines)
            {
                var item = line.Split(':');
                if (item.Length == 2)
                {
                    var key = item[0];
                    var val = item[1];

                    result[key] = val;
                }
            }
            return result;
        }

        #endregion BASE

        public void Init()
        {
        }

        public void Main(string argument, UpdateType updateSource)
        {
        }
    }
}
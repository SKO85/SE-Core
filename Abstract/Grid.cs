using Sandbox.ModAPI.Ingame;
using System.Collections.Generic;
using System.Linq;
using VRage.Game.ModAPI.Ingame.Utilities;

namespace SKO85Core.Abstract
{
    public abstract class Grid
    {
        #region Properties

        public MyGridProgram Program;
        protected List<IMyTerminalBlock> Blocks;
        protected string GridName;
        protected long Id;

        #endregion Properties

        public Grid(MyGridProgram program)
        {
            // Set the program used in this script.
            this.Program = program;

            // Set abstract properties.
            this.GridName = Program.Me.CubeGrid.CustomName;
            this.Id = Program.Me.EntityId;

            // Initialize the Blocks.
            Blocks = new List<IMyTerminalBlock>();
            Program.GridTerminalSystem.GetBlocksOfType(Blocks, b => b.CubeGrid == Program.Me.CubeGrid);

            // Init Configurations Dictionary.
            Configurations = new Dictionary<long, MyIni>();
        }

        public T GetBlock<T>(string name) where T : IMyTerminalBlock
        {
            var block = Blocks.Where(c => c.CustomName.ToLower().Trim() == name.ToLower().Trim()).FirstOrDefault();
            if (block != null)
                return (T)block;
            return default(T);
        }

        public T GetBlock<T>() where T : IMyTerminalBlock
        {
            var block = Blocks.Where(c => c is T).FirstOrDefault();
            if (block != null)
                return (T)block;
            return default(T);
        }

        public List<T> GetBlocks<T>() where T : class, IMyTerminalBlock
        {
            var blocks = Blocks.Where(c => c is T).ToList();
            var toReturn = new List<T>();
            foreach (var block in blocks)
                toReturn.Add((T)block);
            return toReturn;
        }

        void GetBlocks<T>(List<T> blocks) where T : class, IMyTerminalBlock
        {
            foreach (var block in Blocks)
            {
                if (block.GetType() == typeof(T))
                    blocks.Add((T)block);
            }
        }

        public Dictionary<string, string> ParseCustomData(string customData)
        {
            var result = new Dictionary<string, string>();
            var lines = customData.Split('\n');
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

        #region INI Configuration

        public Dictionary<long, MyIni> Configurations { get; set; }

        public MyIniValue GetConfigValue(string section, string key, IMyTerminalBlock block = null)
        {
            if (block == null)
                block = this.Program.Me;
            try
            {
                CreateConfig(block);
                return Configurations[block.EntityId].Get(section, key);
            }
            catch (System.Exception)
            {
                return MyIniValue.EMPTY;
            }
        }

        private void CreateConfig(IMyTerminalBlock block)
        {
            if (!Configurations.ContainsKey(block.EntityId))
            {
                Configurations[block.EntityId] = new MyIni();
                Configurations[block.EntityId].TryParse(block.CustomData);
            }
        }

        public void SaveConfig(IMyTerminalBlock block = null)
        {
            if (block == null)
                block = this.Program.Me;

            CreateConfig(block);
            block.CustomData = Configurations[block.EntityId].ToString();
        }

        public void SetConfigValue(string s, string k, int v, IMyTerminalBlock block)
        {
            if (block == null) { block = this.Program.Me; }
            CreateConfig(block);
            Configurations[block.EntityId].Set(s, k, v);
        }

        public void SetConfigValue(string s, string k, double v, IMyTerminalBlock block)
        {
            if (block == null) { block = this.Program.Me; }
            CreateConfig(block);
            Configurations[block.EntityId].Set(s, k, v);
        }

        public void SetConfigValue(string s, string k, string v, IMyTerminalBlock block)
        {
            if (block == null) { block = this.Program.Me; }
            CreateConfig(block);
            Configurations[block.EntityId].Set(s, k, v);
        }

        public void SetConfigValue(string s, string k, float v, IMyTerminalBlock block)
        {
            if (block == null) { block = this.Program.Me; }
            CreateConfig(block);
            Configurations[block.EntityId].Set(s, k, v);
        }

        public void SetConfigValue(string s, string k, bool v, IMyTerminalBlock block)
        {
            if (block == null) { block = this.Program.Me; }
            CreateConfig(block);
            Configurations[block.EntityId].Set(s, k, v);
        }

        #endregion INI Configuration
    }
}

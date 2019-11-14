using Sandbox.ModAPI.Ingame;
using System.Collections.Generic;
using System.Linq;

namespace IngameScript
{
    public abstract class GridControl
    {
        protected MyGridProgram Program;
        protected List<IMyTerminalBlock> Blocks;

        public GridControl(MyGridProgram program)
        {
            this.Program = program;

            Blocks = new List<IMyTerminalBlock>();
            Program.GridTerminalSystem.GetBlocksOfType(Blocks, b => b.CubeGrid == Program.Me.CubeGrid);
        }

        public T GetBlock<T>(string name) where T : IMyTerminalBlock
        {
            var block = Blocks.Where(c => c.CustomName.ToLower().Trim() == name.ToLower().Trim()).FirstOrDefault();
            if (block != null)
            {
                return (T)block;
            }
            return default(T);
        }

        public T GetBlock<T>() where T : IMyTerminalBlock
        {
            var block = Blocks.Where(c => c is T).FirstOrDefault();
            if (block != null)
            {
                return (T)block;
            }
            return default(T);
        }

        public List<T> GetBlocks<T>() where T : IMyTerminalBlock
        {
            var blocks = Blocks.Where(c => c is T).ToList();
            var toReturn = new List<T>();
            foreach (var block in blocks)
            {
                toReturn.Add((T)block);
            }
            return toReturn;
        }
    }
}
using Sandbox.ModAPI.Ingame;
using System.Collections.Generic;
using System.Linq;

namespace IngameScript.Abstract
{
    public abstract class GridController
    {
        protected MyGridProgram Program;
        protected List<IMyTerminalBlock> Blocks;

        public GridController(MyGridProgram program)
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

        public List<T> GetBlocks<T>() where T : class, IMyTerminalBlock
        {
            var blocks = Blocks.Where(c => c is T).ToList();
            var toReturn = new List<T>();
            foreach (var block in blocks)
            {
                toReturn.Add((T)block);
            }
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
    }
}
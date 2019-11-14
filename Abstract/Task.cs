using IngameScript.Abstract;
using IngameScript.Messages;
using Sandbox.ModAPI.Ingame;

namespace IngameScript.Tasks
{
    public abstract class Task : GridController
    {
        protected BroadcastMessage Message;
        public bool IsRunning = false;
        public bool IsFinished = false;

        public Task(MyGridProgram program, BroadcastMessage msg) : base(program)
        {
            Message = msg;
            Init();
        }

        protected abstract void Init();
        public abstract void Execute();
    }
}
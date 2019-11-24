using System;

namespace IngameScript.Work
{
    public class Wait
    {
        public bool IsDone;
        public int WaitTime;
        public double LastWaitRuntime = 0;
        public Func<bool> Callback;
    }
}

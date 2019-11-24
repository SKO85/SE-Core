using System;
using System.Collections.Generic;

namespace SKO85Core.Work
{
    public static class Arguments
    {
        public static Dictionary<string, Action> ActionHandlers = new Dictionary<string, Action>();

        public static void On(string actionName, Action actionCallback)
        {
            ActionHandlers[actionName] = actionCallback;
        }

        public static void Run(string actionName)
        {
            if (ActionHandlers.ContainsKey(actionName))
            {
                ActionHandlers[actionName]();
            }
        }
    }
}

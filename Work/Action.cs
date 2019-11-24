using System;
using System.Collections.Generic;

namespace IngameScript.Work
{
    public static class Actions
    {
        public static Dictionary<string, Action> ActionHandlers = new System.Collections.Generic.Dictionary<string, Action>();

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

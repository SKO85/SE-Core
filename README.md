# SE-Core
SKO85's Space Engineers In-Game Scripting Core Library

# About
This library contains several classes and helpers that I use with the SE-MDK to create my scripts.

# Features
More will be added soon, but the following is already in the library:
- Script class: make script using this abstract class and use the available functions to manage your grids easily.
- Wait: Script has  a Wait(...) function.
- Integrated Communication System with IGC: Works between PB's on same block or between grids. Uses a shared tag-prefix that you can configure.
- Actions: Define actions with callbacks. Handy for execution of argument provided in the Run of the PB.
- Task Flow System: define tasks in a flow to execute.
- Ship class: handy functions for a ship, like navigating, aligning, etc.
- Menu class: simple menu with nested options and callbacks.
- Object Serialization: Serialize Object classes to strings and from strings back to Objects.
- Ini configuration supported: You can use SetConfigurationValue and GetConfigurationValue to retreive and store config values.


# Examples
I don't have much time to write down all examples, but will try to update this section soon. Let me know if you have questions via de Issues section.

## Defining a Script
```cs
using Sandbox.ModAPI.Ingame;
using SKO85Core.Abstract;
using System.Collections.Generic;

namespace IngameScript
{
    public class MyTestScript : Script
    {
        public MyTestScript(MyGridProgram program) : base(program)
        {
        }

        protected override void Init()
        {
            // Your init code goes here...
            // ...
        }

        protected override void Run()
        {
            // Your main code goes here.
            // ...
        }
    }
}
```

Your Program.cs looks like this then.

```cs
using Sandbox.ModAPI.Ingame;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        private MyTestScript script;

        public Program()
        {
            script = new MyTestScript(this);
        }

        public void Main(string argument, UpdateType updateSource)
        {
            script.Main(argument);
        }
    }
}
```

## Argument Actions
To define argument actions with callbacks, use this in your init function of your script.

```cs
Arguments.On("MyArgumentAction", () =>
{
    // Do whatever you need to do when MyArgumentAction is provided to the Run.
    // ...
});
```

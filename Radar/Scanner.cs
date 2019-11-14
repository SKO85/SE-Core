using System;
using System.Collections.Generic;
using System.Text;

namespace IngameScript.Radar
{
    class Scanner
    {
        //IMyCameraBlock Camera;
        //IMyTextSurface LCD;
        //IMyCockpit Cockpit;
        //IMyMotorStator pitchRotor, yawRotor;
        //public Program()
        //{
        //    Runtime.UpdateFrequency = UpdateFrequency.Update1;
        //    List<IMyCameraBlock> cameras = new List<IMyCameraBlock>();
        //    GridTerminalSystem.GetBlocksOfType(cameras);
        //    Camera = cameras[0];
        //    LCD = GridTerminalSystem.GetBlockWithName("LCD") as IMyTextSurface;
        //    pitchRotor = GridTerminalSystem.GetBlockWithName("pitchRotor") as IMyMotorStator;
        //    yawRotor = GridTerminalSystem.GetBlockWithName("yawRotor") as IMyMotorStator;
        //    Cockpit = GridTerminalSystem.GetBlockWithName("Cockpit") as IMyCockpit;
        //}

        //public void Save()
        //{
        //}
        //double maxRange;
        //string strOut = "";
        //MyDetectedEntityInfo hitInfo;
        //public void Main(string argument, UpdateType updateSource)
        //{
        //    pitchRotor.TargetVelocityRPM = Cockpit.RotationIndicator.X / 2;
        //    yawRotor.TargetVelocityRPM = Cockpit.RotationIndicator.Y / 2;

        //    strOut = "Raycast enabled: " + Camera.EnableRaycast;
        //    strOut += "\nMax Range : " + Camera.AvailableScanRange + " m";
        //    strOut += "\n\n";
        //    if (argument.Equals("switch"))
        //    {
        //        Camera.EnableRaycast = !Camera.EnableRaycast;

        //    }
        //    if (argument.Equals("cast"))
        //    {
        //        hitInfo = Camera.Raycast(Camera.AvailableScanRange);
        //    }
        //    if (!hitInfo.IsEmpty())
        //    {
        //        strOut += ("Hit Pos: " + hitInfo.HitPosition + "\nTarget Pos: " + hitInfo.Position + "\nName:" + hitInfo.Name + "\nType: " + hitInfo.Type + "\nVelocity: " + hitInfo.Velocity + "\n\nDistance: " + (hitInfo.Position - Camera.GetPosition()).Length());
        //    }
        //    else
        //    {
        //        strOut += ("No Target");
        //    }
        //    LCD.WriteText(strOut);
        //}
    }
}
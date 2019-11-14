using System;
using System.Collections.Generic;
using System.Text;

namespace IngameScript.Gravity
{
    class GravityGenerator
    {
        //private int power = 10; // 10 is max
        //private IMyTextSurface disp;
        ////private string text = "Test";
        //private IMyShipController seat;
        //private bool first = false;
        ////private bool inertia;
        //List<IMyGravityGenerator> GravFor = new List<IMyGravityGenerator>();
        //List<IMyGravityGenerator> GravBack = new List<IMyGravityGenerator>();
        //List<IMyGravityGenerator> GravUp = new List<IMyGravityGenerator>();
        //List<IMyGravityGenerator> GravDown = new List<IMyGravityGenerator>();
        //List<IMyGravityGenerator> GravLeft = new List<IMyGravityGenerator>();
        //List<IMyGravityGenerator> GravRight = new List<IMyGravityGenerator>();
        //public Program()
        //{
        //    Runtime.UpdateFrequency = UpdateFrequency.Update1;
        //}
        //public int reset(int a, int b)
        //{
        //    var Grav = new List<IMyTerminalBlock>();  //List of blocks
        //    GridTerminalSystem.GetBlocksOfType<IMyGravityGenerator>(Grav); // Replace it with only one type of blocks
        //    seat = GridTerminalSystem.GetBlockWithName("Cockpit") as IMyShipController;
        //    for (int i = 0; i < Grav.Count; i++)
        //    {
        //        if (Grav[i].IsSameConstructAs(Me)) // same grid as PB is
        //        {
        //            IMyGravityGenerator current = (IMyGravityGenerator)Grav[i];
        //            current.GravityAcceleration = 0;

        //            var seatorientation = seat.Orientation;
        //            var reverseBack = Base6Directions.GetOppositeDirection(seatorientation.Forward);
        //            var reverseDown = Base6Directions.GetOppositeDirection(seatorientation.Up);
        //            var reverseRight = Base6Directions.GetOppositeDirection(seatorientation.Left);
        //            if (Grav[i].Orientation.Up == seatorientation.Up)
        //            { Grav[i].CustomName = "ggg up"; GravUp.Add(Grav[i] as IMyGravityGenerator); }
        //            if (Grav[i].Orientation.Up == seatorientation.Forward)
        //            { Grav[i].CustomName = "ggg Forward"; GravFor.Add(Grav[i] as IMyGravityGenerator); }
        //            if (Grav[i].Orientation.Up == seatorientation.Left)
        //            { Grav[i].CustomName = "ggg Left"; GravLeft.Add(Grav[i] as IMyGravityGenerator); }
        //            if (Grav[i].Orientation.Up == reverseBack)
        //            { Grav[i].CustomName = "ggg Back"; GravBack.Add(Grav[i] as IMyGravityGenerator); }
        //            if (Grav[i].Orientation.Up == reverseRight)
        //            { Grav[i].CustomName = "ggg Right"; GravRight.Add(Grav[i] as IMyGravityGenerator); }
        //            if (Grav[i].Orientation.Up == reverseDown)
        //            { Grav[i].CustomName = "ggg Down"; GravDown.Add(Grav[i] as IMyGravityGenerator); }
        //        }
        //    }
        //    return a + b;
        //}

        //public void Main()
        //{
        //    if (first == false)
        //    { reset(1, 1); first = true; }

        //    Vector3D CurrentVelocityWorldAxes = seat.GetShipVelocities().LinearVelocity;
        //    MatrixD ShipVelocityMatrix = seat.WorldMatrix;
        //    ShipVelocityMatrix.M41 = ShipVelocityMatrix.M42 = ShipVelocityMatrix.M43 = 0.0; ShipVelocityMatrix.M44 = 1.0;
        //    Vector3D CurrentVelocityShipAxes = VRageMath.Vector3D.Transform(CurrentVelocityWorldAxes, VRageMath.MatrixD.Invert(ShipVelocityMatrix));

        //    /////////////////////////////TEXT SURFACE - PROGRAMABLE BLOCK DISPLAY////////////////////////////////////////////////
        //    disp = Me.GetSurface(0);
        //    disp.ContentType = ContentType.TEXT_AND_IMAGE;
        //    disp.FontSize = 1f;
        //    disp.FontColor = Color.Red;
        //    disp.Alignment = TextAlignment.CENTER;
        //    disp.Font = "Monospace";
        //    disp.WriteText(CurrentVelocityShipAxes.ToString("0.0"));
        //    //////////////////////////////////////////////////////////

        //    var pos = seat.MoveIndicator;
        //    foreach (var gen in GravFor)
        //    {
        //        gen.GravityAcceleration = powerpos.Z;
        //    }

        //    foreach (var gen in GravBack)
        //    {
        //        gen.GravityAcceleration = -1powerpos.Z;
        //    }

        //    foreach (var gen in GravLeft)
        //    {
        //        gen.GravityAcceleration = powerpos.X;
        //    }
        //    foreach (var gen in GravRight)
        //    {
        //        gen.GravityAcceleration = -1powerpos.X;
        //    }
        //    foreach (var gen in GravDown)
        //    {
        //        gen.GravityAcceleration = powerpos.Y;
        //    }
        //    foreach (var gen in GravUp)
        //    {
        //        gen.GravityAcceleration = -1power* pos.Y;
        //    }
        //    if (seat.DampenersOverride == true && pos.Z == 0)
        //    {
        //        foreach (var gen in GravFor)
        //        {
        //            gen.GravityAcceleration = (float)-0.5power(float)CurrentVelocityShipAxes.Z;
        //        }

        //        foreach (var gen in GravBack)
        //        {
        //            gen.GravityAcceleration = (float)0.5power(float)CurrentVelocityShipAxes.Z;
        //        }
        //    }
        //    if (seat.DampenersOverride == true && pos.X == 0)
        //    {
        //        foreach (var gen in GravLeft)
        //        {
        //            gen.GravityAcceleration = (float)-1power(float)CurrentVelocityShipAxes.X;
        //        }
        //        foreach (var gen in GravRight)
        //        {
        //            gen.GravityAcceleration = power(float)CurrentVelocityShipAxes.X;
        //        }
        //    }
        //    if (seat.DampenersOverride == true && pos.Y == 0)
        //    {
        //        foreach (var gen in GravDown)
        //        {
        //            gen.GravityAcceleration = (float)-1power(float)CurrentVelocityShipAxes.Y;
        //        }
        //        foreach (var gen in GravUp)
        //        {
        //            gen.GravityAcceleration = power(float)CurrentVelocityShipAxes.Y;
        //        }
        //    }

        //    //for(int i=0; i<GravUp.Count;i++)
        //    //{
        //    //   GravUp[i].ApplyAction("OnOff_Off");//
        //    //}

        //}
    }
}
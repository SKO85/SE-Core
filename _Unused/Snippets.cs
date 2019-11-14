using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using System.Text;
using VRageMath;

namespace IngameScript._Unused
{
    class Snippets
    {
        double GetAccelerationByAxis(List<IMyThrust> thrusters, Base6Directions.Direction directionOfMovement, float shipMass)
        {
            double accel = 0;

            foreach (var thruster in thrusters)
            {
                if (thruster.IsWorking && Base6Directions.GetOppositeDirection(thruster.Orientation.Forward) == directionOfMovement)
                    accel += thruster.MaxEffectiveThrust;
            }

            accel /= shipMass;

            if (double.IsInfinity(accel) || double.IsNaN(accel))
                return 0;

            return accel;
        }
    }
}
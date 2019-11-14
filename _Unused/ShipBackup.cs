//using IngameScript.Vector;
//using Sandbox.ModAPI.Ingame;
//using Sandbox.ModAPI.Interfaces;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using VRageMath;

//namespace IngameScript.Ship
//{
//    public class ShipControl : GridControl
//    {
//        #region Properties

//        IMyRemoteControl Remote;
//        List<IMyGyro> Gyros;

//        List<IMyThrust> ThrustersAll;
//        List<IMyThrust> ThrustersForward = new List<IMyThrust>();
//        List<IMyThrust> ThrustersBackward = new List<IMyThrust>();
//        List<IMyThrust> ThrustersRight = new List<IMyThrust>();
//        List<IMyThrust> ThrustersLeft = new List<IMyThrust>();
//        List<IMyThrust> ThrustersUp = new List<IMyThrust>();
//        List<IMyThrust> ThrustersDown = new List<IMyThrust>();

//        float MaxForwardThrust;
//        float MaxBackwardThrust;
//        float MaxLeftThrust;
//        float MaxRightThrust;
//        float MaxUpThrust;
//        float MaxDownThrust;

//        private DateTime LastChecked;
//        private Vector3D CurrentWaypoint;
//        public Vector3D VelocityVector = new Vector3D();
//        private Vector3D AccelerationVector = new Vector3D();
//        private Vector3D Pos;

//        float minAngleRad = 0.1f;
//        double CTRL_COEFF = 0.2;
//        double distanceAccuracy = 0.5;

//        #endregion Properties

//        IMyTextPanel lcd;

//        public ShipControl(MyGridProgram program) : base(program)
//        {
//            // Get Remote Control.
//            this.Remote = GetBlock<IMyRemoteControl>();
//            if (Remote == null)
//                throw new Exception("No RC");

//            // Get Gyros.
//            this.Gyros = GetBlocks<IMyGyro>();
//            if (this.Gyros == null || this.Gyros.Count == 0)
//                throw new Exception("No Gyros");

//            lcd = GetBlock<IMyTextPanel>("LCD1");

//            // Initialize the thrusters.
//            InitThrusters();
//        }

//        private void InitThrusters()
//        {
//            // Get Thrusters.
//            ThrustersAll = GetBlocks<IMyThrust>();

//            if (this.ThrustersAll == null || this.ThrustersAll.Count == 0)
//                throw new Exception("No Thrusters");

//            // Set thrusters to each directino list according to its relative direction.
//            foreach (var t in ThrustersAll)
//            {
//                t.Enabled = true;

//                // If not wokring, then skip this thruster.
//                if (!t.IsWorking)
//                    continue;

//                // Set override to disabled.
//                t.ThrustOverride = 0f;

//                // Get directino of the thruster.
//                Base6Directions.Direction tDir = Remote.WorldMatrix.GetClosestDirection(t.WorldMatrix.Backward);
//                switch (tDir)
//                {
//                    case Base6Directions.Direction.Forward:
//                        ThrustersForward.Add(t);
//                        MaxForwardThrust += t.MaxThrust;
//                        break;

//                    case Base6Directions.Direction.Backward:
//                        ThrustersBackward.Add(t);
//                        MaxBackwardThrust += t.MaxThrust;
//                        break;

//                    case Base6Directions.Direction.Left:
//                        ThrustersLeft.Add(t);
//                        MaxLeftThrust += t.MaxThrust;
//                        break;

//                    case Base6Directions.Direction.Right:
//                        ThrustersRight.Add(t);
//                        MaxRightThrust += t.MaxThrust;
//                        break;

//                    case Base6Directions.Direction.Up:
//                        ThrustersUp.Add(t);
//                        MaxUpThrust += t.MaxThrust;
//                        break;

//                    case Base6Directions.Direction.Down:
//                        ThrustersDown.Add(t);
//                        MaxDownThrust += t.MaxThrust;
//                        break;
//                }
//            }
//        }

//        private bool SetRotation(IMyGyro gyro, Vector3D localDir, Vector3D targetDir, string direction)
//        {
//            Matrix m;
//            gyro.Orientation.GetMatrix(out m);

//            var localUp = Vector3D.Transform(localDir, MatrixD.Transpose(m));
//            var lTargetUp = Vector3D.Transform(targetDir, MatrixD.Transpose(gyro.WorldMatrix.GetOrientation()));

//            var rot = Vector3D.Cross(localUp, lTargetUp);
//            double ang = rot.Length();
//            ang = Math.Atan2(ang, Math.Sqrt(Math.Max(0.0, 1.0 - ang * ang)));

//            if (ang < 0.01)
//            {
//                gyro.GyroOverride = false;
//                return true;
//            }

//            // Control speed to be proportional to distance (angle) we have left
//            double ctrl_vel = gyro.GetMaximum<float>(direction) * (ang / Math.PI) * CTRL_COEFF;
//            ctrl_vel = Math.Min(gyro.GetMaximum<float>(direction), ctrl_vel);
//            ctrl_vel = Math.Max(0.01, ctrl_vel); //Gyros don't work well at very low speeds

//            rot.Normalize();
//            rot *= ctrl_vel;

//            gyro.GyroPower = 1f;
//            gyro.GyroOverride = true;

//            float pitch = (float)rot.GetDim(0);
//            float yaw = -(float)rot.GetDim(1);
//            float roll = -(float)rot.GetDim(2);

//            gyro.Pitch = pitch;
//            gyro.Yaw = yaw;
//            gyro.Roll = roll;

//            return false;
//        }

//        public bool AlignBlockAs(IMyTerminalBlock block, Vector3D vPos, Vector3D vUp, Vector3D vRight, bool reverse = false)
//        {
//            Matrix m;
//            block.Orientation.GetMatrix(out m);

//            Vector3D forward = reverse ? m.Backward : m.Forward;
//            Vector3D right = reverse ? m.Left : m.Right;

//            foreach (var gyro in Gyros)
//            {
//                if (SetRotation(gyro, forward, (vUp - vPos), "Yaw"))
//                {
//                    SetRotation(gyro, right, (vRight - vPos), "Pitch");
//                }
//            }

//            return Gyros.Where(c => c.GyroOverride).Count() == 0;
//        }

//        public bool AlignBlockTo(IMyTerminalBlock block, Vector3D vPos)
//        {
//            Matrix m;
//            block.Orientation.GetMatrix(out m);

//            Vector3D forward = m.Forward;
//            Vector3D vDirection = (vPos - block.GetPosition());
//            vDirection.Normalize();

//            foreach (var gyro in Gyros)
//            {
//                gyro.Roll = 0;
//                gyro.Pitch = 0;
//                gyro.Yaw = 0;

//                gyro.Orientation.GetMatrix(out m);

//                var localCurrent = Vector3D.Transform(forward, MatrixD.Transpose(m));
//                var localTarget = Vector3D.Transform(vDirection, MatrixD.Transpose(gyro.WorldMatrix.GetOrientation()));
//                var rot = Vector3D.Cross(localCurrent, localTarget);

//                double dot2 = Vector3D.Dot(localCurrent, localTarget);
//                double ang = rot.Length();
//                ang = Math.Atan2(ang, Math.Sqrt(Math.Max(0.0, 1.0 - ang * ang)));
//                if (dot2 < 0) ang = Math.PI - ang; // compensate for >+/-90
//                if (ang < minAngleRad)
//                {
//                    gyro.GyroOverride = false;
//                    continue;
//                }

//                float yawMax = (float)(2 * Math.PI);
//                double ctrl_vel = yawMax * (ang / Math.PI) * CTRL_COEFF;

//                ctrl_vel = Math.Min(yawMax, ctrl_vel);
//                ctrl_vel = Math.Max(0.01, ctrl_vel);
//                rot.Normalize();
//                rot *= ctrl_vel;

//                float pitch = -(float)rot.X;
//                if (Math.Abs(gyro.Pitch - pitch) > 0.01)
//                    gyro.Pitch = pitch;

//                float yaw = -(float)rot.Y;
//                if (Math.Abs(gyro.Yaw - yaw) > 0.01)
//                    gyro.Yaw = yaw;

//                float roll = -(float)rot.Z;
//                if (Math.Abs(gyro.Roll - roll) > 0.01)
//                    gyro.Roll = roll;

//                gyro.GyroOverride = true;
//            }

//            return Gyros.Where(c => c.GyroOverride).Count() == 0;
//        }

//        public double DistanceTo(Vector3D vPos, IMyTerminalBlock block = null)
//        {
//            if (block == null)
//            {
//                block = Remote;
//            }
//            return VectorHelper.DistanceBetween(vPos, block);
//        }

//        public bool GoToPosition(Vector3D vPos, IMyTerminalBlock block = null)
//        {
//            if (block == null)
//            {
//                block = Remote;
//            }

//            if (block != Remote)
//            {
//                // Apply offset.
//                Vector3D offset = (Remote.GetPosition() - block.GetPosition());
//                vPos = (vPos + offset);
//            }

//            double distance = DistanceTo(vPos, Remote);

//            if (distance <= distanceAccuracy && VelocityVector.Length() <= 0.1)
//            {
//                //                Program.Runtime.UpdateFrequency = UpdateFrequency.None;
//                CurrentWaypoint = new Vector3D(-1, -1, -1);
//                DisableAutoPilot();
//                DisableThrustersOverride();
//                DisableGyroOverride();

//                lcd.WriteText("DONE.");

//                return true;
//            }

//            if (distance <= 40)
//            {
//                Program.Runtime.UpdateFrequency = UpdateFrequency.Update10;
//            }
//            else
//            {
//                //Program.Runtime.UpdateFrequency = UpdateFrequency.None;
//            }

//            bool jumpDrivesAvailable = false;       // For now.
//            int minJumpDriveDistance = 50000;       // From this distance, try to use Jump Drives.
//            int minRemoteControlDistance = 2000;    // Fro this distance, use the Remote Control Auto Pilot.

//            if (distance >= minJumpDriveDistance && jumpDrivesAvailable)
//            {
//                // Use Jump Drive if available.
//                // ...
//            }
//            else if (distance >= minRemoteControlDistance)
//            {
//                Remote.ClearWaypoints();
//                Remote.AddWaypoint(vPos, "GoToPosition");

//                // Use RC for flying.
//                EnableAutoPilot();
//            }
//            else
//            {
//                // Disable the auto pilot if enabled.
//                DisableAutoPilot();

//                if (SetThrusters(vPos, block))
//                {
//                    DisableGyroOverride();
//                    DisableThrustersOverride();
//                    return true;
//                };
//            }

//            return false;
//        }

//        public void EnableAutoPilot()
//        {
//            if (Remote != null)
//            {
//                DisableThrustersOverride();
//                Remote.SetAutoPilotEnabled(true);
//                Remote.SetCollisionAvoidance(true);
//                Remote.SetDockingMode(true);
//            }
//        }

//        public void DisableAutoPilot()
//        {
//            if (Remote != null)
//            {
//                Remote.ClearWaypoints();
//                Remote.SetAutoPilotEnabled(false);
//                Remote.SetCollisionAvoidance(false);
//            }
//        }

//        public void EnableThrusters(bool enable)
//        {
//            foreach (var t in ThrustersAll)
//                t.Enabled = enable;
//        }

//        public void DisableGyroOverride()
//        {
//            foreach (var gyro in Gyros)
//            {
//                gyro.Yaw = 0;
//                gyro.Pitch = 0;
//                gyro.Roll = 0;
//                gyro.GyroOverride = false;
//            }
//        }

//        public void DisableThrustersOverride()
//        {
//            foreach (var thrust in ThrustersAll)
//                thrust.ThrustOverride = 0;
//        }

//        public void GetVelocity()
//        {
//            var stamp = DateTime.Now;
//            var currentPos = Remote.GetPosition();
//            var lastVelocity = VelocityVector;

//            if (LastChecked != DateTime.MinValue && (stamp - LastChecked).TotalMilliseconds > 100)
//            {
//                var elapsedTime = (stamp - LastChecked).TotalMilliseconds;
//                VelocityVector.X = (float)(currentPos.X - Pos.X) / (float)elapsedTime;
//                VelocityVector.Y = (float)(currentPos.Y - Pos.Y) / (float)elapsedTime;
//                VelocityVector.Z = (float)(currentPos.Z - Pos.Z) / (float)elapsedTime;
//                //VelocityVector = VelocityVector * 1000;

//                AccelerationVector.X = (VelocityVector.X - lastVelocity.X) / elapsedTime;
//                AccelerationVector.Y = (VelocityVector.Y - lastVelocity.Y) / elapsedTime;
//                AccelerationVector.Z = (VelocityVector.Z - lastVelocity.Z) / elapsedTime;

//                //Velocity = (currentPos - Pos).Length() / elapsedTime;
//                //Acceleration = (Velocity - LastVelocity) / elapsedTime;

//                //Velocity = Velocity * 1000;
//                //Acceleration = Acceleration * 1000;
//            }
//            LastChecked = stamp;
//            Pos = currentPos;
//            //LastVelocity = Velocity;
//        }

//        public bool SetThrusters(Vector3D vPos, IMyTerminalBlock block = null)
//        {
//            // Set block to RC if nothing else is specfied.
//            if (block == null)
//            {
//                block = Remote;
//            }

//            var sb = new StringBuilder();

//            // Target vector.
//            Vector3D vTarget = (vPos - block.GetPosition());

//            // Get the velocity in XYZ directions.
//            GetVelocity();
//            var velocityNorm = Vector3D.Normalize(VelocityVector);

//            // Get Distance for X, Y, Z to position.
//            Vector3D xyzDist = VectorHelper.GetXYZDistance(block, vPos);
//            double distX = Math.Round(Math.Abs(xyzDist.X), 2);
//            double distY = Math.Round(Math.Abs(xyzDist.Y), 2);
//            double distZ = Math.Round(Math.Abs(xyzDist.Z), 2);

//            // Get target distance.
//            var targetDistance = VectorHelper.DistanceBetween(vPos, block);

//            // Ship mass
//            var mass = Remote.CalculateShipMass().PhysicalMass;

//            // Calc stop distance.
//            // var stopDistance = GetStoppingDistance();

//            // Time to stop?
//            // var _timeToStop = stopDistance >= targetDistance; // vTarget.LengthSquared() - (stopDistance * stopDistance) < 3;

//            // Adjust thrust percentage according to relative distance.
//            // distance = 0, then thrust should also be 0

//            //// Override percentage.
//            //float thrustOverridePercent = _timeToStop ? 0 : 0.1f;

//            //// Override.
//            //foreach (var thruster in ThrustersAll)
//            //{
//            //    if (thruster.WorldMatrix.Forward.Dot(vTarget) < 0)
//            //    {
//            //        if (!_timeToStop)
//            //        {
//            //            thruster.ThrustOverridePercentage = 1;
//            //            sb.AppendLine(string.Format("{0}", thruster.CustomName.ToString()));
//            //            continue;
//            //        }
//            //    }

//            //    thruster.ThrustOverride = 0;
//            //}

//            // Up
//            if (xyzDist.Z > distanceAccuracy)
//            {
//                SetThrustDirect(Base6Directions.Direction.Up, ThrustersDown, mass, distZ);
//            }

//            // Down
//            else if (xyzDist.Z < distanceAccuracy * -1)
//            {
//                SetThrustDirect(Base6Directions.Direction.Down, ThrustersUp, mass, distZ);
//            }

//            // Left
//            if (xyzDist.X > distanceAccuracy)
//            {
//                SetThrustDirect(Base6Directions.Direction.Left, ThrustersRight, mass, distX);
//            }

//            // Right
//            else if (xyzDist.X < distanceAccuracy * -1)
//            {
//                SetThrustDirect(Base6Directions.Direction.Right, ThrustersLeft, mass, distX);
//            }

//            // Forward
//            if (xyzDist.Y > distanceAccuracy)
//            {
//                SetThrustDirect(Base6Directions.Direction.Forward, ThrustersBackward, mass, distY);
//            }

//            // Backward
//            else if (xyzDist.Y < distanceAccuracy * -1)
//            {
//                SetThrustDirect(Base6Directions.Direction.Backward, ThrustersForward, mass, distY);
//            }

//            lcd.WriteText(sb.ToString());
//            return false;

//            //// Up
//            //if (xyzDist.Z > distanceAccuracy)
//            //{
//            //    SetThrustDirect(Base6Directions.Direction.Up, Base6Directions.Direction.Down, ThrustersDown, MaxDownThrust, velT, mass, distZ);
//            //}

//            //// Down
//            //else if (xyzDist.Z < distanceAccuracy * -1)
//            //{
//            //    SetThrustDirect(Base6Directions.Direction.Down, Base6Directions.Direction.Up, ThrustersUp, MaxUpThrust, velT, mass, distZ);
//            //}

//            //// Left
//            //if (xyzDist.X > distanceAccuracy)
//            //{
//            //    SetThrustDirect(Base6Directions.Direction.Left, Base6Directions.Direction.Right, ThrustersRight, MaxRightThrust, velT, mass, distX);
//            //}

//            //// Right
//            //else if (xyzDist.X < distanceAccuracy * -1)
//            //{
//            //    SetThrustDirect(Base6Directions.Direction.Right, Base6Directions.Direction.Left, ThrustersLeft, MaxLeftThrust, velT, mass, distX);
//            //}

//            //// Forward
//            //if (xyzDist.Y > distanceAccuracy)
//            //{
//            //    SetThrustDirect(Base6Directions.Direction.Forward, Base6Directions.Direction.Backward, ThrustersBackward, MaxBackwardThrust, velT, mass, distY);
//            //}

//            //// Backward
//            //else if (xyzDist.Y < distanceAccuracy * -1)
//            //{
//            //    SetThrustDirect(Base6Directions.Direction.Backward, Base6Directions.Direction.Forward, ThrustersForward, MaxForwardThrust, velT, mass, distY);
//            //}

//            return false;
//        }

//        public void SetThrustDirect(
//            Base6Directions.Direction to,
//            List<IMyThrust> thrusters,
//            //float maxThtust,
//            //double velocity,
//            float mass,
//            double dist)
//        {
//            // Use the oposite thrusters to calculate the acc needed for breaking...
//            // var acc = GetAccelerationByAxis(thrusters, to, mass);

//            double acc = CalculateAcceleration(thrusters, mass);
//            Base6Directions.Direction oposite = Base6Directions.GetOppositeDirection(to);
//            double stopDist = VelocityVector.LengthSquared() / (2 * acc);

//            double powerPercent = 100;

//            if (dist <= 10)
//                powerPercent = 10;
//            else if (dist <= 100)
//                powerPercent = 40;
//            else if (dist <= 400)
//                powerPercent = 80;

//            var sb = new StringBuilder();
//            //var _timeToStop = (dist * dist) - (stopDist * stopDist) < 0.2;

//            var _timeToStop = stopDist <= dist;

//            if (_timeToStop)
//            {
//                // Break;
//                SetThrust(oposite, 0);
//                SetThrust(to, 0);
//            }
//            else
//            {
//                SetThrust(oposite, 0);
//                SetThrust(to, powerPercent);
//            }
//        }

//        public void SetThrust(Base6Directions.Direction dir, double percent)
//        {
//            List<IMyThrust> thrustersToUse = new List<IMyThrust>();
//            switch (dir)
//            {
//                case Base6Directions.Direction.Forward:
//                    thrustersToUse = ThrustersForward;
//                    break;

//                case Base6Directions.Direction.Backward:
//                    thrustersToUse = ThrustersBackward;
//                    break;

//                case Base6Directions.Direction.Left:
//                    thrustersToUse = ThrustersLeft;
//                    break;

//                case Base6Directions.Direction.Right:
//                    thrustersToUse = ThrustersRight;
//                    break;

//                case Base6Directions.Direction.Up:
//                    thrustersToUse = ThrustersUp;
//                    break;

//                case Base6Directions.Direction.Down:
//                    thrustersToUse = ThrustersDown;
//                    break;
//            }

//            foreach (var thruster in thrustersToUse)
//            {
//                float val = ((float)percent / 100) * thruster.MaxThrust;
//                thruster.ThrustOverride = val;
//            }
//        }

//        private double GetAccelerationByAxis(List<IMyThrust> thrusters, Base6Directions.Direction directionOfMovement, float shipMass)
//        {
//            double accel = 0;

//            var velocityNorm = Vector3D.Normalize(VelocityVector);

//            foreach (var thruster in thrusters)
//            {
//                if (thruster.IsWorking && thruster.WorldMatrix.Forward.Dot(velocityNorm) > 0)
//                {
//                    Vector3D proj = VectorHelper.ProjectVector(thruster.WorldMatrix.Forward, velocityNorm);
//                    double percentage = proj.Length();

//                    accel += (float)(thruster.MaxEffectiveThrust * percentage);
//                }
//            }

//            accel /= shipMass;

//            if (double.IsInfinity(accel) || double.IsNaN(accel))
//                return 0;

//            return accel;
//        }

//        #region Private Functions

//        //double GetStoppingDistance(Vector3D? transformedGrav = null)
//        //{
//        //    // vf² = vi² + 2ad ==>  d = (vf² - vi²) / (2a)
//        //    // Let vf = 0      ==>  d = -vi² / (2a)

//        //    // Get initial velocity
//        //    Vector3D velocity = Remote.GetShipVelocities().LinearVelocity;

//        //    if (Vector3D.IsZero(velocity, 0.0002))
//        //        return 0;

//        //    // Calculate acceleration
//        //    float shipMass = Remote.CalculateShipMass().PhysicalMass;
//        //    var velocityDir = Base6Directions.GetClosestDirection(velocity);
//        //    double accel = GetAccelerationByAxis(ThrustersAll, velocityDir, shipMass);

//        //    // Account for gravity pulling us down
//        //    if (transformedGrav.HasValue)
//        //        accel += transformedGrav.Value.Z;

//        //    return velocity.LengthSquared() / (2 * accel);
//        //}

//        private double CalculateAcceleration(List<IMyThrust> thrusters, float shipMass)
//        {
//            float maxThrust = 0;
//            Vector3D velocityNorm = Vector3D.Normalize(VelocityVector);
//            foreach (var t in thrusters)
//            {
//                if (!t.IsWorking)
//                    continue;

//                Vector3D proj = VectorHelper.ProjectVector(t.WorldMatrix.Forward, velocityNorm);
//                double percentage = proj.Length();
//                maxThrust += (float)(t.MaxThrust * percentage);
//            }

//            double myAcceleration = (maxThrust / shipMass);
//            if (double.IsNaN(myAcceleration))
//            {
//                myAcceleration = 0;
//            }

//            return myAcceleration;
//        }

//        //private double GetStoppingDistance(List<IMyThrust> thrusters, Base6Directions.Direction opositeDirection, double accel, Vector3D? transformedGrav = null)
//        //{
//        //    // Get initial velocity
//        //    Vector3D velocity = Remote.GetShipVelocities().LinearVelocity;

//        //    if (Vector3D.IsZero(velocity, 0.0002))
//        //        return 0;

//        //    // Calculate acceleration
//        //    float shipMass = Remote.CalculateShipMass().TotalMass;
//        //    var velocityDir = Base6Directions.GetClosestDirection(velocity);

//        //    // Account for gravity pulling us down
//        //    if (transformedGrav.HasValue)
//        //        accel += transformedGrav.Value.Z;

//        //    return velocity.LengthSquared() / (2 * accel);
//        //}

//        #endregion Private Functions
//    }
//}

// Get accelerations.
// double accX = CalculateAcceleration(ThrustersLeft, mass);

//// Force Calculation.
//double forceX = accX * (double)mass;
//double forceY = accY * (double)mass;
//double forceZ = accZ * (double)mass;

//sb.AppendLine(string.Format("ACC: {0}", acc));

// Stop Distance Calculation.
//double stopDistT = Math.Round(Math.Abs((velX * velX) / (2 * acc)), 4);

//double stopDistX = Math.Round(Math.Abs((velX * velX) / (2 * accX)), 4);
//double stopDistY = Math.Round(Math.Abs((velY * velY) / (2 * accY)), 4);
//double stopDistZ = Math.Round(Math.Abs((velZ * velZ) / (2 * accZ)), 4);

//if (double.IsNaN(stopDistX)) stopDistX = 0;
//if (double.IsNaN(stopDistY)) stopDistY = 0;
//if (double.IsNaN(stopDistZ)) stopDistZ = 0;

//sb.AppendLine(string.Format("{0}", ""));
//sb.AppendLine(string.Format("dX: {0}", stopDistX));
//sb.AppendLine(string.Format("dY: {0}", stopDistY));
//sb.AppendLine(string.Format("dZ: {0}", stopDistZ));
//sb.AppendLine(string.Format("dT: {0}", stopDistT));

//Vector3D velocityNorm = Vector3D.Normalize(VelocityVector);

//double distDev = 0.5;

//double maxSpeed = 15;
//float thrustLevel = 345600 / 4;
//double currentSpeed = VelocityVector.Length() * 1000;

//if (targetDistance <= 10)
//{
//    //Program.Runtime.UpdateFrequency = UpdateFrequency.Update10;
//    maxSpeed = 2;
//    thrustLevel = 345600 / 16;
//}
//else if (targetDistance <= 25)
//{
//    //Program.Runtime.UpdateFrequency = UpdateFrequency.None;
//    maxSpeed = 5;
//}
//else if (targetDistance <= 50)
//{
//    //Program.Runtime.UpdateFrequency = UpdateFrequency.None;
//    maxSpeed = 10;
//}

//if (targetDistance <= stopDostance)
//{
//    // DisableThrustersOverride();
//}
//else if (currentSpeed > maxSpeed)
//{
//    DisableThrustersOverride();
//}
//else
//{
//    foreach (var thrust in ThrustersAll)
//    {
//        if (currentSpeed >= maxSpeed)
//        {
//            thrust.SetValueFloat("Override", 0f);
//        }
//        Base6Directions.Direction dir = block.WorldMatrix.GetClosestDirection(thrust.WorldMatrix.Backward);

//        bool overideThrust = false;
//        double stopDist = 0;
//        double targetDist = 0;
//        double velocity = 0;

//        if (xyzDist.Y > distDev && dir == Base6Directions.Direction.Forward)
//        {
//            stopDist = stopDistY;
//            targetDist = xyzDist.Y;
//            velocity = xyzVelocity.Y;
//            overideThrust = true;
//        }
//        else if (xyzDist.Y < distDev * -1 && dir == Base6Directions.Direction.Backward)
//        {
//            stopDist = stopDistY;
//            targetDist = xyzDist.Y;
//            velocity = xyzVelocity.Y;
//            overideThrust = true;
//        }
//        else if (xyzDist.X > distDev && dir == Base6Directions.Direction.Left)
//        {
//            stopDist = stopDistX;
//            targetDist = xyzDist.X;
//            velocity = xyzVelocity.X;
//            overideThrust = true;
//        }
//        else if (xyzDist.X < distDev * -1 && dir == Base6Directions.Direction.Right)
//        {
//            stopDist = stopDistX;
//            targetDist = xyzDist.X;
//            velocity = xyzVelocity.X;
//            overideThrust = true;
//        }
//        else if (xyzDist.Z > distDev && dir == Base6Directions.Direction.Up)
//        {
//            stopDist = stopDistZ;
//            targetDist = xyzDist.Z;
//            velocity = xyzVelocity.Z;
//            overideThrust = true;
//        }
//        else if (xyzDist.Z < distDev * -1 && dir == Base6Directions.Direction.Down)
//        {
//            stopDist = stopDistZ;
//            targetDist = xyzDist.Z;
//            velocity = xyzVelocity.Z;
//            overideThrust = true;
//        }

//        if (velocity * 1000 > maxSpeed)
//        {
//            continue;
//        }

//        if (overideThrust)
//        {
//            targetDist = Math.Abs(targetDist);
//            stopDist = Math.Abs(stopDist);

//            if (targetDist > stopDist)
//            {
//                // thrust.SetValueFloat("Override", thrustLevel);

//                //sb.AppendLine(string.Format("{0}: {1}", thrust.CustomName, dir.ToString()));
//                //sb.AppendLine(string.Format("Target: {0}", targetDist));
//                //sb.AppendLine(string.Format("Stop: {0}", stopDist));
//                //sb.AppendLine(string.Format("{0}", ""));
//            }
//        }
//    }
//}

//if (targetDistance <= stopDostance)
//{
//    sb.AppendLine("Stop ALL");
//    sb.AppendLine("");
//    DisableThrustersOverride();
//}
//else
//{
//}

//

//// Debug.
//sb.AppendLine("");
//sb.AppendLine(string.Format("veloX: {0}", xyzVelocity.X));
//sb.AppendLine(string.Format("distX: {0}", xyzDist.X));
//sb.AppendLine(string.Format("stopX: {0}", stopDistX));
//sb.AppendLine(string.Format("forceX: {0}", forceX));
//sb.AppendLine(string.Format("tSpeedX: {0}", targetXSpeed));
//sb.AppendLine(string.Format("thrustX: {0}", thrustPowerX));
//sb.AppendLine("");
//sb.AppendLine(string.Format("veloY: {0}", xyzVelocity.Y));
//sb.AppendLine(string.Format("distY: {0}", xyzDist.Y));
//sb.AppendLine(string.Format("stopY: {0}", stopDistY));
//sb.AppendLine(string.Format("forceY: {0}", forceY));
//sb.AppendLine(string.Format("tSpeedY: {0}", targetYSpeed));
//sb.AppendLine(string.Format("thrustY: {0}", thrustPowerY));
//sb.AppendLine("");
//sb.AppendLine(string.Format("veloZ: {0}", xyzVelocity.Z));
//sb.AppendLine(string.Format("distZ: {0}", xyzDist.Z));
//sb.AppendLine(string.Format("stopZ: {0}", stopDistZ));
//sb.AppendLine(string.Format("forceZ: {0}", forceZ));
//sb.AppendLine(string.Format("tSpeedZ: {0}", targetZSpeed));
//sb.AppendLine(string.Format("thrustZ: {0}", thrustPowerZ));

// lcd.WriteText(sb.ToString());
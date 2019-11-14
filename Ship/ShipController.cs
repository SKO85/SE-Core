using IngameScript.Abstract;
using IngameScript.Helpers;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VRageMath;

namespace IngameScript.Ship
{
    public class ShipController : GridController
    {
        #region Properties

        IMyRemoteControl Remote;
        List<IMyGyro> Gyros;
        List<IMyThrust> ThrustersAll;
        Dictionary<Base6Directions.Direction, List<IMyThrust>> Thrusters;
        Dictionary<Base6Directions.Direction, float> MaxThrusters;

        private DateTime LastChecked;
        private Vector3D CurrentWaypoint;
        public Vector3D VelocityVector = new Vector3D();
        private Vector3D AccelerationVector = new Vector3D();
        private Vector3D Pos;

        float minAngleRad = 0.1f;
        double CTRL_COEFF = 0.2;
        double distanceAccuracy = 0.6;

        #endregion Properties

        public ShipController(MyGridProgram program) : base(program)
        {
            // Get Remote Control.
            this.Remote = GetBlock<IMyRemoteControl>();

            // Check if a Remote Control or Cockpit is found.
            if (Remote == null)
                throw new Exception("No Remote Controller found.");

            // Get Gyros.
            this.Gyros = GetBlocks<IMyGyro>();
            if (this.Gyros == null || this.Gyros.Count == 0)
                throw new Exception("No Gyros found.");

            // Initialize the thrusters.
            InitThrusters();
        }

        #region Private Functions

        private void InitThrusters()
        {
            // Get Thrusters.
            ThrustersAll = GetBlocks<IMyThrust>();

            if (this.ThrustersAll == null || this.ThrustersAll.Count == 0)
                throw new Exception("No Thrusters");

            // Init Dictionaries.
            this.Thrusters = new Dictionary<Base6Directions.Direction, List<IMyThrust>>();
            this.MaxThrusters = new Dictionary<Base6Directions.Direction, float>();

            // Set Dictionaries.
            Thrusters[Base6Directions.Direction.Forward] = new List<IMyThrust>();
            Thrusters[Base6Directions.Direction.Backward] = new List<IMyThrust>();
            Thrusters[Base6Directions.Direction.Up] = new List<IMyThrust>();
            Thrusters[Base6Directions.Direction.Down] = new List<IMyThrust>();
            Thrusters[Base6Directions.Direction.Left] = new List<IMyThrust>();
            Thrusters[Base6Directions.Direction.Right] = new List<IMyThrust>();

            MaxThrusters[Base6Directions.Direction.Forward] = 0;
            MaxThrusters[Base6Directions.Direction.Backward] = 0;
            MaxThrusters[Base6Directions.Direction.Up] = 0;
            MaxThrusters[Base6Directions.Direction.Down] = 0;
            MaxThrusters[Base6Directions.Direction.Left] = 0;
            MaxThrusters[Base6Directions.Direction.Right] = 0;

            // Set thrusters to each directino list according to its relative direction.
            foreach (var t in ThrustersAll)
            {
                t.Enabled = true;

                // If not wokring, then skip this thruster.
                if (!t.IsWorking)
                    continue;

                // Set override to disabled.
                t.ThrustOverride = 0f;

                // Get directino of the thruster.
                Base6Directions.Direction tDir = Remote.WorldMatrix.GetClosestDirection(t.WorldMatrix.Backward);
                Thrusters[tDir].Add(t);
                MaxThrusters[tDir] += t.MaxThrust;
            }
        }

        private bool SetRotation(IMyGyro gyro, Vector3D localDir, Vector3D targetDir, string direction)
        {
            Matrix m;
            gyro.Orientation.GetMatrix(out m);

            var localUp = Vector3D.Transform(localDir, MatrixD.Transpose(m));
            var lTargetUp = Vector3D.Transform(targetDir, MatrixD.Transpose(gyro.WorldMatrix.GetOrientation()));

            var rot = Vector3D.Cross(localUp, lTargetUp);
            double ang = rot.Length();
            ang = Math.Atan2(ang, Math.Sqrt(Math.Max(0.0, 1.0 - ang * ang)));

            if (ang < 0.01)
            {
                gyro.SetValueBool("Override", false);
                return true;
            }

            //Control speed to be proportional to distance (angle) we have left
            double ctrl_vel = gyro.GetMaximum<float>(direction) * (ang / Math.PI) * CTRL_COEFF;
            ctrl_vel = Math.Min(gyro.GetMaximum<float>(direction), ctrl_vel);
            ctrl_vel = Math.Max(0.01, ctrl_vel); //Gyros don't work well at very low speeds

            rot.Normalize();
            rot *= ctrl_vel;

            gyro.SetValueFloat("Power", 1.0f);
            gyro.SetValueBool("Override", true);

            float pitch = (float)rot.GetDim(0);
            float yaw = -(float)rot.GetDim(1);
            float roll = -(float)rot.GetDim(2);

            gyro.SetValueFloat("Pitch", pitch);
            gyro.SetValueFloat("Yaw", yaw);
            gyro.SetValueFloat("Roll", roll);

            return false;
        }

        #endregion Private Functions

        public bool AlignBlockAs(IMyTerminalBlock block, Vector3D vPos, Vector3D vUp, Vector3D vRight, bool reverse = false)
        {
            Matrix m;
            block.Orientation.GetMatrix(out m);

            Vector3D forward = reverse ? m.Backward : m.Forward;
            Vector3D right = reverse ? m.Left : m.Right;

            foreach (var gyro in Gyros)
            {
                SetRotation(gyro, forward, (vUp - vPos), "Yaw");
                SetRotation(gyro, right, (vRight - vPos), "Pitch");
            }

            return Gyros.Where(c => c.GyroOverride).Count() == 0;
        }

        public bool AlignBlockTo(IMyTerminalBlock block, Vector3D vPos)
        {
            Matrix m;
            block.Orientation.GetMatrix(out m);

            Vector3D forward = m.Forward;
            Vector3D vDirection = (vPos - block.GetPosition());
            vDirection.Normalize();

            foreach (var gyro in Gyros)
            {
                gyro.Roll = 0;
                gyro.Pitch = 0;
                gyro.Yaw = 0;

                gyro.Orientation.GetMatrix(out m);

                var localCurrent = Vector3D.Transform(forward, MatrixD.Transpose(m));
                var localTarget = Vector3D.Transform(vDirection, MatrixD.Transpose(gyro.WorldMatrix.GetOrientation()));
                var rot = Vector3D.Cross(localCurrent, localTarget);

                double dot2 = Vector3D.Dot(localCurrent, localTarget);
                double ang = rot.Length();
                ang = Math.Atan2(ang, Math.Sqrt(Math.Max(0.0, 1.0 - ang * ang)));
                if (dot2 < 0) ang = Math.PI - ang; // compensate for >+/-90
                if (ang < minAngleRad)
                {
                    gyro.GyroOverride = false;
                    continue;
                }

                float yawMax = (float)(2 * Math.PI);
                double ctrl_vel = yawMax * (ang / Math.PI) * CTRL_COEFF;

                ctrl_vel = Math.Min(yawMax, ctrl_vel);
                ctrl_vel = Math.Max(0.01, ctrl_vel);
                rot.Normalize();
                rot *= ctrl_vel;

                float pitch = -(float)rot.X;
                if (Math.Abs(gyro.Pitch - pitch) > 0.01)
                    gyro.Pitch = pitch;

                float yaw = -(float)rot.Y;
                if (Math.Abs(gyro.Yaw - yaw) > 0.01)
                    gyro.Yaw = yaw;

                float roll = -(float)rot.Z;
                if (Math.Abs(gyro.Roll - roll) > 0.01)
                    gyro.Roll = roll;

                gyro.GyroOverride = true;
            }

            return Gyros.Where(c => c.GyroOverride).Count() == 0;
        }

        public double DistanceTo(Vector3D vPos, IMyTerminalBlock block = null)
        {
            if (block == null)
                block = Remote;
            return VectorHelper.DistanceBetween(vPos, block);
        }

        public bool GoToPosition(Vector3D vPos, IMyTerminalBlock block = null)
        {
            if (block == null)
                block = Remote;

            if (block != Remote)
            {
                // Apply offset.
                Vector3D offset = (Remote.GetPosition() - block.GetPosition());
                vPos = (vPos + offset);
            }

            double distance = DistanceTo(vPos, Remote);
            if (distance <= distanceAccuracy && VelocityVector.Length() <= 0.1)
            {
                // Program.Runtime.UpdateFrequency = UpdateFrequency.None;
                CurrentWaypoint = new Vector3D(-1, -1, -1);
                DisableAutoPilot();
                DisableThrustersOverride();
                DisableGyroOverride();

                return true;
            }

            if (distance <= 40)
            {
                Program.Runtime.UpdateFrequency = UpdateFrequency.Update10;
            }
            else
            {
                //Program.Runtime.UpdateFrequency = UpdateFrequency.None;
            }

            bool jumpDrivesAvailable = false;       // For now.
            int minJumpDriveDistance = 50000;       // From this distance, try to use Jump Drives.
            int minRemoteControlDistance = 2000;    // Fro this distance, use the Remote Control Auto Pilot.

            if (distance >= minJumpDriveDistance && jumpDrivesAvailable)
            {
                // Use Jump Drive if available.
                // ...
            }
            else if (distance >= minRemoteControlDistance)
            {
                if (Remote.IsAutoPilotEnabled)
                    Remote.ClearWaypoints();
                Remote.AddWaypoint(vPos, "GoToPosition");
                CurrentWaypoint = vPos;

                // Use RC for flying.
                EnableAutoPilot();
            }
            else
            {
                // Disable the auto pilot if enabled.
                DisableAutoPilot();

                if (SetThrusters(vPos, block))
                {
                    DisableGyroOverride();
                    DisableThrustersOverride();
                    return true;
                };
            }

            return false;
        }

        public void EnableAutoPilot()
        {
            if (Remote != null)
            {
                DisableThrustersOverride();
                Remote.SetAutoPilotEnabled(true);
                Remote.SetCollisionAvoidance(true);
                Remote.SetDockingMode(true);
            }
        }

        public void DisableAutoPilot()
        {
            if (Remote != null)
            {
                Remote.ClearWaypoints();
                Remote.SetAutoPilotEnabled(false);
                Remote.SetCollisionAvoidance(false);
            }
        }

        public void EnableThrusters(bool enable)
        {
            foreach (var t in ThrustersAll)
                t.Enabled = enable;
        }

        public void DisableGyroOverride()
        {
            foreach (var gyro in Gyros)
            {
                gyro.Yaw = 0;
                gyro.Pitch = 0;
                gyro.Roll = 0;
                gyro.GyroOverride = false;
            }
        }

        public void DisableThrustersOverride()
        {
            foreach (var thrust in ThrustersAll)
                thrust.SetValueFloat("Override", 0f);
        }

        public void GetVelocity()
        {
            var stamp = DateTime.Now;
            var currentPos = Remote.GetPosition();
            var lastVelocity = VelocityVector;

            if (LastChecked != DateTime.MinValue && (stamp - LastChecked).TotalMilliseconds > 100)
            {
                var elapsedTime = (stamp - LastChecked).TotalMilliseconds;
                VelocityVector.X = (float)(currentPos.X - Pos.X) / (float)elapsedTime;
                VelocityVector.Y = (float)(currentPos.Y - Pos.Y) / (float)elapsedTime;
                VelocityVector.Z = (float)(currentPos.Z - Pos.Z) / (float)elapsedTime;
                //VelocityVector = VelocityVector * 1000;

                AccelerationVector.X = (VelocityVector.X - lastVelocity.X) / elapsedTime;
                AccelerationVector.Y = (VelocityVector.Y - lastVelocity.Y) / elapsedTime;
                AccelerationVector.Z = (VelocityVector.Z - lastVelocity.Z) / elapsedTime;

                //Velocity = (currentPos - Pos).Length() / elapsedTime;
                //Acceleration = (Velocity - LastVelocity) / elapsedTime;

                //Velocity = Velocity * 1000;
                //Acceleration = Acceleration * 1000;
            }
            LastChecked = stamp;
            Pos = currentPos;
            //LastVelocity = Velocity;
        }

        public bool SetThrusters(Vector3D vPos, IMyTerminalBlock block = null)
        {
            // Set block to RC if nothing else is specfied.
            if (block == null)
                block = Remote;

            // Get tharget distance.
            var targetDistance = VectorHelper.DistanceBetween(vPos, block);

            // Get the velocity in XYZ directions.
            GetVelocity();
            Vector3D velocityNorm = Vector3D.Normalize(VelocityVector);

            double velT = VelocityVector.Length() * 1000;
            double velX = VelocityVector.X * 1000;
            double velY = VelocityVector.Y * 1000;
            double velZ = VelocityVector.Z * 1000;

            // Distance of block in XYZ directions to target position.
            var xyzDist = VectorHelper.GetXYZDistance(Remote, vPos);
            var distX = Math.Abs(xyzDist.X);
            var distY = Math.Abs(xyzDist.Y);
            var distZ = Math.Abs(xyzDist.Z);

            // Ship mass
            var mass = Remote.CalculateShipMass().TotalMass;

            // Up
            if (xyzDist.Z > distanceAccuracy)
                SetThrustDirect(Base6Directions.Direction.Up, velT, mass, distZ, velocityNorm);

            // Down
            else if (xyzDist.Z < distanceAccuracy * -1)
                SetThrustDirect(Base6Directions.Direction.Down, velT, mass, distZ, velocityNorm);

            // Left
            if (xyzDist.X > distanceAccuracy)
                SetThrustDirect(Base6Directions.Direction.Left, velT, mass, distX, velocityNorm);

            // Right
            else if (xyzDist.X < distanceAccuracy * -1)
                SetThrustDirect(Base6Directions.Direction.Right, velT, mass, distX, velocityNorm);

            // Forward
            if (xyzDist.Y > distanceAccuracy)
                SetThrustDirect(Base6Directions.Direction.Forward, velT, mass, distY, velocityNorm);

            // Backward
            else if (xyzDist.Y < distanceAccuracy * -1)
                SetThrustDirect(Base6Directions.Direction.Backward, velT, mass, distY, velocityNorm);

            return false;
        }

        private void SetThrustDirect(
            Base6Directions.Direction headingDir,
            double velocity,
            float mass,
            double dist,
            Vector3D velocityNorm)
        {
            // Get oposite direction.
            Base6Directions.Direction opositeDir = Base6Directions.GetOppositeDirection(headingDir);

            // Use the oposite thrusters to calculate the acc needed for breaking...
            double acc = CalculateAcceleration(Thrusters[opositeDir], mass, velocityNorm);
            double stopDist = CalculateStopDistance(velocity, acc);

            if (double.IsNaN(stopDist))
                stopDist = 0;

            // Calculate the power percentage to use based on distance.
            double powerPercent = MathHelper.Clamp(dist / 100, 0.1, 1) * 100;

            // Do we need to break?
            if (stopDist * 3 >= dist)
                SetThrust(headingDir, 0);
            else
                SetThrust(headingDir, powerPercent);
        }

        private double CalculateAcceleration(List<IMyThrust> thrusters, float shipMass, Vector3D velocityNorm)
        {
            float maxThrust = 0;
            foreach (var t in thrusters)
            {
                if (!t.IsWorking)
                    continue;

                Vector3D proj = VectorHelper.VectorProject(t.WorldMatrix.Forward, velocityNorm);
                double percentage = proj.Length();
                maxThrust += (float)(t.MaxThrust * percentage);
            }

            double myAcceleration = (maxThrust / shipMass);
            if (double.IsNaN(myAcceleration))
            {
                myAcceleration = 0;
            }
            return myAcceleration;
        }
        private double CalculateStopDistance(double velocity, double acceleration)
        {
            return Math.Abs(velocity * velocity / (2 * acceleration));
        }

        public void SetThrust(Base6Directions.Direction dir, double percent)
        {
            foreach (var thruster in Thrusters[dir])
                thruster.ThrustOverride = ((float)percent / 100) * thruster.MaxThrust;

            foreach (var thruster in Thrusters[Base6Directions.GetOppositeDirection(dir)])
                thruster.ThrustOverride = 0;
        }
    }
}
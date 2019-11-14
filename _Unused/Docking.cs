//using Sandbox.ModAPI.Ingame;
//using SpaceEngineers.Game.ModAPI.Ingame;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using VRageMath;

//namespace IngameScript
//{
//    public class Docking
//    {
//        #region CONFIG

//        #endregion

//        double maxSpeed = 8;                                //Limits the speed while moving to the connector to roughly this value.
//        double acceleration = 4;                             // Acceleration of the approach to the connector. (Meters per second)
//        double decceleration = 6;                           // Opposite to above, increase this if overshooting. (overshooting is caused by too much mass for the thruster power) (Meters per second)
//        double verticalAcceleration = 1;                  // Acceleration downwards towards the docking port
//        double verticalDecceleration = 1.2;             // Deceleration when moving towards the docking port
//        double verticalApproachSpeed = 1;             // The relative speed at which the ship will attempt to approach at (no particular unit)
//        double connectorClearance = 5;                 // The height the ship will rise to above the connector (Eg if there are trees of height 10, it could rise above them) (Minimum of 1m)
//        double hangarHeight = 9999;                     // MUST BE at least 2m larger than connectorClearance. This is the height the ship will make sure it's below.
//        double hangarCorrectionSpeed = 6;           // Speed at which it rises or lowers to get into the hangar.

//        bool spinsWhenConnected = true;             // If true, the ship will rotate to match the connector's direction once it has docked.
//        bool spinsBeforeConnecting = true;           // If true, the ship will rotate to match the connector's direction when it's moving down to dock in true Star Wars fashion.
//        bool largeShipsSpinToo = false;                // If false, large ships won't spin no matter what the settings are above.
//        bool onlySpinsClockwise = false;             // If true, while docked, the ship will only rotate clockwise instead of taking the shortest rotation adding extra coolness.
//        double spinSpeedOnConnector = 2;          // Maximum speed the ship will rotate at on the connector.
//        double maximumSuccessAngle = 0.025;  // The angle in radians at which the program determines it's successfully docked.
//        double spinStartDistance = 4;                   // Distance away the ship will be when it begins to spin around.
//        double runwaySuccessDistance = 4;       // The distance away the ship will be from the runway marker (excluding vertical distance) when it then goes to the next one.
//        double maxRunwaySpeed = 7;                 // Rough value, might be slightly faster than this.

//        string connectorTag = "[Dock]";                 // The tag on the name of the connector - must be unique for that ship. - can't be the same as the tag on the optional script.
//        string antennaTag = "[Dock]";                    // (Optional script) The tag on the name of the antenna - only relevent for using with the optional script. - can't be the same as the tag on the optional script.
//        string timerblockTag = "[Dock]";                // Completely optional. A timer with this tag gets activated once docking is complete.
//        string cockpitblockTag = "[Dock]";                // Completely optional. A cockpit/remote/flight seat with this tag gets prioritised as your main one if you're using a mod.
//        string runwayArgumentTag = "Runway";  // Tag required in the argument for the ship to use the runway

//        //////TECHNICAL Changeable variables:

//        double spinAngle = 0.1;                              //Don't change unless you don't want your ship to spin round. Can cause issues.
//        double maxConnectorDistanceX = 2;        // Maximum distance from the home connector the ship will be before it docks. (Measured by how lined up it is, eg 0.01 is almost perfectly in line)
//        double gravityMultiplier = 1.05;                    // Gravity is artificially increased by this multiplier to make atmospheric landings more energetic and less like I'm hauling the ship up by it's toes.
//        double retryDistance = 3;                          // If the ship is further from this distance it'll fly above connectorClearence and have another go.

//        bool gridCheck = true;                               //Set to false if you want this to operate with multiple grids.
//        float gyroSpeed = 10;

//        bool multiplayerFix = false;                // If your ship is bobbing above the connector and not lowering, set this to true.
//        double multiplayerFixPower = 0.1;              // How strong the fix is applied. (The maximum angle at which it deems it's at a correct angle)

//        bool overrideAtmosphericSpinning = false;     //The script automatically turns off spinning before connector if the script detects it could be dangerous for the ship to spin. This overrides that.

//        ////////////////////////////////////////////////////
//        ////////////////////////////////////////////////////
//        //
//        //  DO NOT CHANGE BELOW v v v

//        Vector3D homePos = new Vector3D(0, 0, 0);
//        Vector3D homeDirection = new Vector3D(0, 0, 0);
//        Vector3D homeUp = new Vector3D(0, 0, 0);

//        List<Vector3D> homePositions = new List<Vector3D>();
//        List<Vector3D> homeDirections = new List<Vector3D>();
//        List<Vector3D> homeUps = new List<Vector3D>();
//        List<string> homeNames = new List<string>();
//        List<string> connectorNames = new List<string>();

//        List<Vector3D> runwayPositions = new List<Vector3D>();
//        List<Vector3D> runwayDirections = new List<Vector3D>();
//        List<Vector3D> runwayForwards = new List<Vector3D>();
//        string runwayHome = "";
//        int currentRunwayMarker = 0;

//        string homeConnectorName = "";
//        string currentArg = "";

//        IMyShipConnector myConnector;
//        List<IMyShipConnector> myConnectors = new List<IMyShipConnector>();

//        List<IMyThrust> thrusters = new List<IMyThrust>();
//        List<IMyGyro> gyros = new List<IMyGyro>();

//        Vector3 velocityVector = new Vector3();
//        Vector3 lastPosition = new Vector3();
//        Vector3 lastHomePosition = new Vector3(0, 0, 0);
//        Vector3 additionalVelocityVector = new Vector3(0, 0, 0);

//        DateTime lastCheck = DateTime.MinValue;
//        DateTime lastCheckHome = DateTime.MinValue;
//        IMyShipController cockpit = null;
//        IMyRadioAntenna antenna = null;
//        IMyTimerBlock timer = null;
//        bool isLargeShip = false;

//        double maxRotation = 0;

//        bool docking = false;

//        DateTime lastAntennaArg = DateTime.MinValue;

//        public double dock(Vector3 targetPos, Vector3 targetDi, bool isFinalDock)
//        {
//            Vector3 myDir = myConnector.GetPosition();
//            Vector3 myPos = myConnector.CubeGrid.GridIntegerToWorld(myConnector.Position - Base6Directions.GetIntVector(myConnector.Orientation.Forward));
//            Vector3 myUp = myConnector.CubeGrid.GridIntegerToWorld(myConnector.Position - Base6Directions.GetIntVector(myConnector.Orientation.Forward) + Base6Directions.GetIntVector(myConnector.Orientation.Up));
//            Vector3 myRight = myConnector.CubeGrid.GridIntegerToWorld(myConnector.Position - Base6Directions.GetIntVector(myConnector.Orientation.Forward) + (Base6Directions.GetIntVector(myConnector.Orientation.Left) * -1));

//            Vector3 adjustmentPoint = new Vector3();

//            double dist = Vector3.Distance(myPos, targetPos);
//            Vector3 directionToMe = (myPos - targetPos);

//            Vector3 currentDirection = normalize(myDir - myPos);
//            Vector3 targetDirection = normalize(targetDi);
//            double rotationAngle = angleBetween(currentDirection, (targetDirection * -1));

//            double angle = angleBetween(directionToMe, targetDi);
//            double distToAdjustmentPoint = dist * Math.Cos(angle);
//            adjustmentPoint = targetPos + ((Vector3D)targetDi * distToAdjustmentPoint);

//            Vector3 ToAdj = adjustmentPoint - myPos;
//            LocalCoords myLocal = new LocalCoords(myPos, myUp, myDir);
//            Vector3 localVector = myLocal.GetLocalVector(adjustmentPoint);

//            var stamp = DateTime.Now;
//            if (lastCheck != DateTime.MinValue && (stamp - lastCheck).TotalMilliseconds > 100)
//            {
//                var elapsedTime = (stamp - lastCheck).TotalMilliseconds;
//                velocityVector.X = (float)(myPos.X - lastPosition.X) / (float)elapsedTime;
//                velocityVector.Y = (float)(myPos.Y - lastPosition.Y) / (float)elapsedTime;
//                velocityVector.Z = (float)(myPos.Z - lastPosition.Z) / (float)elapsedTime;
//                velocityVector = velocityVector * 1000;
//            }
//            lastPosition = myPos;
//            lastCheck = stamp;
//            Vector3 localVelocity = myLocal.GetLocalVector(myPos + velocityVector);
//            Vector3 localTarget = myLocal.GetLocalVector(targetPos);
//            double distanceToAdj = Vector3.Distance(myPos, adjustmentPoint);

//            float mass = 850000;
//            if (cockpit != null)
//            {
//                var Masses = cockpit.CalculateShipMass();
//                //mass = Masses.TotalMass;
//                //mass = Masses.BaseMass;
//                mass = Masses.PhysicalMass;
//                //Echo(Masses.ToString());
//                //Echo("tot: " + mass.ToString());
//            }
//            //mass = mass - 25000;//(float)manualMassIncrease;

//            double force = acceleration * (double)mass;
//            double massConstant = (double)mass * decceleration;

//            //if (localVector.X > maxSpeed)
//            //{
//            //    localVector.X = (float)maxSpeed;
//            //}
//            //else if (localVector.X < -maxSpeed)
//            //{
//            //    localVector.X = -(float)maxSpeed;
//            //}
//            //if (localVector.Y > maxSpeed)
//            //{
//            //    localVector.Y = (float)maxSpeed;
//            //}
//            //else if (localVector.Y < -maxSpeed)
//            //{
//            //    localVector.Y = -(float)maxSpeed;
//            //}
//            double len = localVector.Length();

//            if (len > (float)maxSpeed * 2)
//            {
//                localVector = normalize(localVector) * (float)maxSpeed * 2;
//            }
//            //Echo(localVector.Length().ToString());

//            if (isFinalDock == false)
//            {
//                localVector = normalize(localVector) * (float)maxRunwaySpeed * 2;
//            }
//            double finalThrustX = (localVector.X * force) - (localVelocity.X * massConstant); //+ (additionalVelocityVector.X * mass * 15);
//            double finalThrustY = (localVector.Y * force) - (localVelocity.Y * massConstant);// + (additionalVelocityVector.Y * mass * 15);

//            //if (isFinalDock == false)
//            //{
//            //    finalThrustX = (localVector.X * force) - (localVelocity.X * massConstant * 0.2);
//            //    finalThrustY = (localVector.Y * force) - (localVelocity.Y * massConstant * 0.2);
//            //}

//            foreach (var thruster in thrusters)
//            {
//                thruster.SetValueFloat("Override", 0f);
//            }

//            bool countION = true;

//            Vector3 resultantGravity = new Vector3(0, 0, 0);
//            if (cockpit != null)
//            {
//                Vector3 gravity = cockpit.GetNaturalGravity() * gravityMultiplier;
//                float gravStrength = gravity.Length();
//                if (gravStrength > 0.1)
//                {
//                    Vector3 localGravity = myLocal.GetLocalVector(myPos + gravity);
//                    //Vector3 localGravity = myLocal.GetLocalVector(gravity);
//                    resultantGravity = localGravity;
//                }
//                if (gravStrength > 3)
//                {
//                    countION = false;
//                }
//            }

//            if ((rotationAngle < spinAngle) && (localTarget.Z > connectorClearance || distanceToAdj < 3) && localTarget.Z < hangarHeight)
//            {
//                setThrust(new Vector3(0, 0, 1), (float)finalThrustX + (resultantGravity.X * mass * 1), countION);
//                setThrust(new Vector3(0, -1, 0), (float)finalThrustY + (resultantGravity.Y * mass * -1), countION);
//            }

//            double downMaxSpeed = verticalApproachSpeed * 3;

//            double moveDistance = 10;
//            if (localTarget.Z < 11)
//            {
//                moveDistance = 10;
//            }
//            if (localTarget.Z < 7)
//            {
//                moveDistance = 1;
//                downMaxSpeed = verticalApproachSpeed * 2;
//            }
//            if (localTarget.Z < 5)
//            {
//                moveDistance = 0.5;
//                downMaxSpeed = verticalApproachSpeed * 1;
//            }
//            if (localTarget.Z < 3)
//            {
//                moveDistance = 0.3;
//            }

//            if (localTarget.Z > 17)
//            {
//                moveDistance = 35;
//                //downMaxSpeed = verticalApproachSpeed * 3;
//            }
//            if (localTarget.Z < 10 && isFinalDock == false)
//            {
//                moveDistance = 0;
//                downMaxSpeed = 1;
//            }
//            if (downMaxSpeed > maxSpeed)
//            {
//                downMaxSpeed = maxSpeed;
//            }

//            bool tooLow = false;
//            bool tooHigh = false;
//            if (localTarget.Z < connectorClearance && distanceToAdj > retryDistance)
//            {
//                tooLow = true;
//            }
//            if (localTarget.Z > hangarHeight)
//            {
//                tooHigh = true;
//            }
//            if (hangarHeight < connectorClearance)
//            {
//                Echo("WARNING:\nhangarHeight < connectorClearance");
//                tooHigh = false;
//            }

//            // If Close to the finishing connector
//            if (distanceToAdj < moveDistance && rotationAngle < spinAngle && tooLow == false && tooHigh == false)
//            {
//                double targetDownSpeed = downMaxSpeed - ((distanceToAdj / moveDistance) * downMaxSpeed);
//                double currentDownSpeed = localVelocity.Z;
//                if (currentDownSpeed > targetDownSpeed)
//                {
//                    setThrust(new Vector3(1, 0, 0), ((float)(currentDownSpeed - targetDownSpeed) * (float)targetDownSpeed * mass * -(float)verticalDecceleration) + (resultantGravity.Z * mass * -1), countION);
//                }
//                else
//                {
//                    setThrust(new Vector3(1, 0, 0), ((float)(targetDownSpeed - currentDownSpeed) * (float)targetDownSpeed * mass * (float)verticalAcceleration) + (resultantGravity.Z * mass * -1), countION);
//                }
//            }
//            else if (rotationAngle < spinAngle && tooLow == false && tooHigh == false) // If away from the finishing connector
//            {
//                if (localVelocity.Z > 0)
//                {
//                    setThrust(new Vector3(1, 0, 0), (5 * mass * -1) + (resultantGravity.Z * mass * -1), countION);
//                }
//            }
//            double hangarBuffer = 0.1;

//            if ((tooLow && distanceToAdj > retryDistance) || localTarget.Z > hangarHeight - hangarBuffer) // If below connector clearance
//            {
//                double maxSpeed = hangarCorrectionSpeed;
//                double currentDownSpeed = -localVelocity.Z;
//                if (localTarget.Z > hangarHeight - hangarBuffer)
//                {
//                    maxSpeed = -maxSpeed;
//                }
//                if (tooLow && distanceToAdj > retryDistance)
//                {
//                    if (currentDownSpeed > maxSpeed)
//                    {
//                        setThrust(new Vector3(1, 0, 0), ((float)(currentDownSpeed - maxSpeed) * mass * (float)verticalDecceleration) + (resultantGravity.Z * mass * -1), countION);
//                    }
//                    else
//                    {
//                        setThrust(new Vector3(1, 0, 0), ((float)(maxSpeed - currentDownSpeed) * -(float)verticalAcceleration * mass) + (resultantGravity.Z * mass * -1), countION);
//                    }
//                }
//                else
//                {
//                    if (currentDownSpeed > maxSpeed)
//                    {
//                        setThrust(new Vector3(1, 0, 0), ((float)(currentDownSpeed - maxSpeed) * mass * (float)verticalDecceleration) + (resultantGravity.Z * mass * -1), countION);
//                    }
//                    else
//                    {
//                        setThrust(new Vector3(1, 0, 0), ((float)(maxSpeed - currentDownSpeed) * mass * -(float)verticalAcceleration) + (resultantGravity.Z * mass * -1), countION);
//                    }
//                }

//            }
//            return distanceToAdj;

//        }

//        public static void align(Vector3 targetPos, Vector3 targetDi)
//        {
//            Vector3 myDir = myConnector.GetPosition();
//            Vector3 myPos = myConnector.CubeGrid.GridIntegerToWorld(myConnector.Position - Base6Directions.GetIntVector(myConnector.Orientation.Forward));
//            Vector3 myUp = myConnector.CubeGrid.GridIntegerToWorld(myConnector.Position - Base6Directions.GetIntVector(myConnector.Orientation.Forward) + Base6Directions.GetIntVector(myConnector.Orientation.Up));
//            Vector3 myRight = myConnector.CubeGrid.GridIntegerToWorld(myConnector.Position - Base6Directions.GetIntVector(myConnector.Orientation.Forward) + (Base6Directions.GetIntVector(myConnector.Orientation.Left) * -1));

//            LocalCoords myLocal = new LocalCoords(myPos, myUp, myDir);

//            Vector3 currentDirection = VectorHelper.normalize(myDir - myPos);
//            Vector3 targetDirection = VectorHelper.normalize(targetDi);

//            Vector3 crossCurrentTarget = VectorHelper.crossProduct(currentDirection, targetDirection);
//            crossCurrentTarget = VectorHelper.normalize(crossCurrentTarget);

//            Vector3 upDirection = VectorHelper.normalize(myUp - myPos);
//            double rollAngle = VectorHelper.angleBetween(upDirection, crossCurrentTarget);
//            int rollDirection = VectorHelper.AngleDir(upDirection, crossCurrentTarget, currentDirection);

//            double mainAngle = VectorHelper.angleBetween((currentDirection * -1), targetDirection);
//            float directionOfRoll = VectorHelper.AngleDir((currentDirection * -1), targetDirection, VectorHelper.normalize(myRight - myPos)) * -1;

//            Vector3 rightDirection = VectorHelper.normalize(myRight - myPos);
//            Vector3 cross2 = VectorHelper.crossProduct(VectorHelper.normalize(myUp - myPos), targetDirection);
//            double yawAngle = VectorHelper.angleBetween(rightDirection, cross2);
//            float directionOfYaw = VectorHelper.AngleDir(rightDirection, cross2, upDirection);

//            mainAngle = (mainAngle - yawAngle) * directionOfRoll; //

//            float speed = gyroSpeed;

//            if (multiplayerFix == false || (multiplayerFix == true && mainAngle < multiplayerFixPower))
//            {
//                foreach (var gyro in gyros)
//                {
//                    gyro.Yaw = 0;
//                    gyro.Pitch = 0;
//                    gyro.Roll = 0;
//                    gyro.GyroOverride = true;
//                }

//                if (Math.Abs(yawAngle) > 0.01)
//                {
//                    setYaw(directionOfYaw * (float)yawAngle * speed, myConnector, gyros);
//                }

//                setPitch((float)mainAngle * speed, myConnector, gyros);
//            }
//            else
//            {
//                foreach (var gyro in gyros)
//                {
//                    gyro.Yaw = 0;
//                    gyro.Pitch = 0;
//                    gyro.Roll = 0;
//                    gyro.GyroOverride = true;
//                }
//            }
//        }

//        public static void setPitch(float pitch)
//        {
//            Vector3 rightVector = Base6Directions.GetIntVector(myConnector.Orientation.Left) * -1;

//            foreach (var gyro in gyros)
//            {
//                gyro.GyroOverride = true;
//                Vector3 gyroForward = Base6Directions.GetIntVector(gyro.Orientation.Forward);
//                Vector3 gyroUp = Base6Directions.GetIntVector(gyro.Orientation.Up);
//                Vector3 gyroRight = Base6Directions.GetIntVector(gyro.Orientation.Left) * -1;

//                if (gyroRight == (rightVector * -1))
//                {
//                    gyro.Pitch = pitch;
//                }
//                else if (gyroRight == rightVector)
//                {
//                    gyro.Pitch = pitch * -1;
//                }
//                else if (gyroUp == rightVector)
//                {
//                    gyro.Yaw = pitch;
//                }
//                else if (gyroUp == (rightVector * -1))
//                {
//                    gyro.Yaw = pitch * -1;
//                }
//                else if (gyroForward == (rightVector * -1))
//                {
//                    gyro.Roll = pitch;
//                }
//                else if (gyroForward == rightVector)
//                {
//                    gyro.Roll = pitch * -1;
//                }
//            }
//        }

//        public static void setYaw(float yaw)
//        {
//            Vector3 upVector = Base6Directions.GetIntVector(myConnector.Orientation.Up);

//            foreach (var gyro in gyros)
//            {
//                gyro.GyroOverride = true;
//                Vector3 gyroForward = Base6Directions.GetIntVector(gyro.Orientation.Forward);
//                Vector3 gyroUp = Base6Directions.GetIntVector(gyro.Orientation.Up);
//                Vector3 gyroRight = Base6Directions.GetIntVector(gyro.Orientation.Left) * -1;

//                if (gyroUp == (upVector * -1))
//                {
//                    gyro.Yaw = yaw;
//                }
//                else if (gyroUp == upVector)
//                {
//                    gyro.Yaw = yaw * -1;
//                }
//                else if (gyroRight == upVector)
//                {
//                    gyro.Pitch = yaw;
//                }
//                else if (gyroRight == (upVector * -1))
//                {
//                    gyro.Pitch = yaw * -1;
//                }
//                else if (gyroForward == upVector)
//                {
//                    gyro.Roll = yaw;
//                }
//                else if (gyroForward == (upVector * -1))
//                {
//                    gyro.Roll = yaw * -1;
//                }
//            }
//        }

//        public static void setRoll(float roll)
//        {
//            Vector3 forwardVector = Base6Directions.GetIntVector(myConnector.Orientation.Forward);
//            foreach (var gyro in gyros)
//            {
//                gyro.GyroOverride = true;
//                Vector3 gyroForward = Base6Directions.GetIntVector(gyro.Orientation.Forward);
//                Vector3 gyroUpward = Base6Directions.GetIntVector(gyro.Orientation.Up);
//                Vector3 gyroRight = Base6Directions.GetIntVector(gyro.Orientation.Left) * -1;

//                if (gyroForward == forwardVector)
//                {
//                    gyro.Roll = roll;
//                }
//                else if (gyroForward == (forwardVector * -1))
//                {
//                    gyro.Roll = roll * -1;
//                }
//                else if (gyroUpward == (forwardVector * -1))
//                {
//                    gyro.Yaw = roll;
//                }
//                else if (gyroUpward == forwardVector)
//                {
//                    gyro.Yaw = roll * -1;
//                }
//                else if (gyroRight == forwardVector)
//                {
//                    gyro.Pitch = roll;
//                }
//                else if (gyroRight == (forwardVector * -1))
//                {
//                    gyro.Pitch = roll * -1;
//                }
//            }
//        }

//        public static void disableGyroOverride()
//        {
//            foreach (var gyro in gyros)
//            {
//                gyro.SetValueFloat("Yaw", 0);
//                gyro.SetValueFloat("Pitch", 0);
//                gyro.SetValueFloat("Roll", 0);
//                gyro.GyroOverride = false;
//            }
//        }
//    }
//}
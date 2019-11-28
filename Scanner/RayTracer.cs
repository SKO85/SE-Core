using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using System.Text;
using VRageMath;

namespace SKO85Core.Scanner
{
    public class RayTracer
    {
        public static Vector3D DetectedVector { get; set; }
        public static MyDetectedEntityInfo DetectedEntityInfo { get; set; }

        public static void EnableRaycast(List<IMyCameraBlock> cameras, bool enable)
        {
            foreach (var cam in cameras)
            {
                // Enable raycast for this camera.
                cam.EnableRaycast = enable;
            }
        }

        public static bool Trace(Vector3D target, List<IMyCameraBlock> cameras, double distance, bool ignorePlanet = true)
        {
            if (cameras != null && cameras.Count > 0)
            {
                foreach (var cam in cameras)
                {
                    // Check if we can scan the target.
                    if (!cam.CanScan(distance))
                        continue;

                    // Scan target.
                    DetectedEntityInfo = cam.Raycast(2000);

                    // If nothing detected, check next camera.
                    if (DetectedEntityInfo.IsEmpty())
                        continue;

                    // Detected somehintg. Check what.
                    DetectedVector = DetectedEntityInfo.HitPosition.Value;

                    // Stop loop and return true that we have detected something.
                    return true;
                }
            }
            return false;
        }
    }
}

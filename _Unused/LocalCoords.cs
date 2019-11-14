//using System;
//using VRageMath;

//namespace IngameScript.Vector
//{
//    public class LocalCoords
//    {
//        private Vector3 Radix;
//        private Vector3 XAxis;
//        private Vector3 YAxis;
//        private Vector3 ZAxis;
//        private TransformMatrix3D TransformToGlobal;
//        public LocalCoords(Vector3 radix, Vector3 y, Vector3 z)
//        {
//            Radix = radix;
//            YAxis = Normalize(GetVectorRelative(y, Radix));
//            ZAxis = Normalize(GetVectorRelative(z, Radix));
//            XAxis = CrossProduct(YAxis, ZAxis);
//            TransformToGlobal = new TransformMatrix3D();
//            TransformToGlobal.Translate(Radix.GetDim(0), Radix.GetDim(1), Radix.GetDim(2));
//            TransformToGlobal.Orientate(XAxis, YAxis, ZAxis);
//        }
//        public Vector3 GetLocalVector(Vector3 point)
//        {
//            return TransformToGlobal.WorldToModel(point);
//        }
//        public Vector3 GetGlobalVector(Vector3 point)
//        {
//            return TransformToGlobal.ModelToWorld(point);
//        }
//        private Vector3 GetVectorRelative(Vector3 vec, Vector3 rel)
//        {
//            double X = vec.GetDim(0) - rel.GetDim(0);
//            double Y = vec.GetDim(1) - rel.GetDim(1);
//            double Z = vec.GetDim(2) - rel.GetDim(2);
//            return new Vector3(X, Y, Z);
//        }
//        private Vector3 CrossProduct(Vector3 a, Vector3 b)
//        {
//            return Vector3.Cross(a, b);
//        }
//        private Vector3 Normalize(Vector3 v)
//        {
//            double n = Math.Sqrt(v.GetDim(0) * v.GetDim(0) + v.GetDim(1) * v.GetDim(1) + v.GetDim(2) * v.GetDim(2));
//            double x = v.GetDim(0) / n;
//            double y = v.GetDim(1) / n;
//            double z = v.GetDim(2) / n;
//            return new Vector3(x, y, z);
//        }
//        private Vector3 Reverse(Vector3 v)
//        {
//            double x = -v.GetDim(0);
//            double y = -v.GetDim(1);
//            double z = -v.GetDim(2);
//            return new Vector3(x, y, z);
//        }
//    }
//}
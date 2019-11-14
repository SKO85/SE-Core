//using System;
//using VRageMath;

//namespace IngameScript.Vector
//{
//    public class TransformMatrix3D
//    {
//        private double RotX;
//        private double RotY;
//        private double RotZ;
//        private double TranslateX;
//        private double TranslateY;
//        private double TranslateZ;
//        private Matrix RotXMatrix;
//        private Matrix RotYMatrix;
//        private Matrix RotZMatrix;
//        private Matrix RotMatrix;
//        private Matrix TranslateMatrix;
//        private Matrix TransformMatrix;
//        private Matrix InverseTransform;
//        private bool UpdateTransform;
//        private bool UpdateRotation;
//        public TransformMatrix3D()
//        {
//            RotXMatrix = new Matrix(3);
//            RotYMatrix = new Matrix(3);
//            RotZMatrix = new Matrix(3);
//            RotMatrix = new Matrix(3);
//            TranslateMatrix = new Matrix(3);
//            TransformMatrix = new Matrix(3);
//            InverseTransform = new Matrix(3);
//            UpdateTransform = false;
//            UpdateRotation = false;
//        }
//        public void RotateX(double Rads)
//        {
//            RotX = Rads;
//            RotXMatrix.SetValue(1, 1, Math.Cos(Rads));
//            RotXMatrix.SetValue(2, 1, -Math.Sin(Rads));
//            RotXMatrix.SetValue(1, 2, Math.Sin(Rads));
//            RotXMatrix.SetValue(2, 2, Math.Cos(Rads));
//            UpdateTransform = true;
//            UpdateRotation = true;
//        }
//        public void RotateY(double Rads)
//        {
//            RotY = Rads;
//            RotYMatrix.SetValue(0, 0, Math.Cos(Rads));
//            RotYMatrix.SetValue(2, 1, Math.Sin(Rads));
//            RotYMatrix.SetValue(0, 2, -Math.Sin(Rads));
//            RotYMatrix.SetValue(2, 2, Math.Cos(Rads));
//            UpdateTransform = true;
//            UpdateRotation = true;
//        }
//        public void RotateZ(double Rads)
//        {
//            RotZ = Rads;
//            RotZMatrix.SetValue(0, 0, Math.Cos(Rads));
//            RotZMatrix.SetValue(1, 0, -Math.Sin(Rads));
//            RotZMatrix.SetValue(0, 1, Math.Sin(Rads));
//            RotZMatrix.SetValue(1, 1, Math.Cos(Rads));
//            UpdateTransform = true;
//            UpdateRotation = true;
//        }
//        public void Orientate(Vector3 XAxis, Vector3 YAxis, Vector3 ZAxis)
//        {
//            RotMatrix.SetValue(0, 0, XAxis.GetDim(0));
//            RotMatrix.SetValue(0, 1, XAxis.GetDim(1));
//            RotMatrix.SetValue(0, 2, XAxis.GetDim(2));
//            RotMatrix.SetValue(1, 0, YAxis.GetDim(0));
//            RotMatrix.SetValue(1, 1, YAxis.GetDim(1));
//            RotMatrix.SetValue(1, 2, YAxis.GetDim(2));
//            RotMatrix.SetValue(2, 0, ZAxis.GetDim(0));
//            RotMatrix.SetValue(2, 1, ZAxis.GetDim(1));
//            RotMatrix.SetValue(2, 2, ZAxis.GetDim(2));
//            UpdateRotation = false;
//        }
//        private Matrix InverseRotate()
//        {
//            Matrix inv = new Matrix(3);
//            Matrix rot = GetRotationMatrix();
//            for (uint x = 0; x < rot.Cols; x++)
//            {
//                for (uint y = 0; y < rot.Rows; y++)
//                {
//                    inv.SetValue(y, x, rot.GetValue(x, y));
//                }
//            }
//            return inv;
//        }
//        public void Translate(double X, double Y, double Z)
//        {
//            TranslateX = X;
//            TranslateY = Y;
//            TranslateZ = Z;
//            TranslateMatrix.SetValue(3, 0, X);
//            TranslateMatrix.SetValue(3, 1, Y);
//            TranslateMatrix.SetValue(3, 2, Z);
//            UpdateTransform = true;
//        }
//        private Matrix InverseTranslate()
//        {
//            Matrix inv = new Matrix(3);
//            inv.SetValue(3, 0, -TranslateMatrix.GetValue(3, 0));
//            inv.SetValue(3, 1, -TranslateMatrix.GetValue(3, 1));
//            inv.SetValue(3, 2, -TranslateMatrix.GetValue(3, 2));
//            return inv;
//        }
//        public Matrix GetRotationMatrix()
//        {
//            if (UpdateRotation)
//            {
//                RotMatrix.SetToIdenity();
//                RotMatrix = RotXMatrix.Multiply(RotMatrix);
//                RotMatrix = RotYMatrix.Multiply(RotMatrix);
//                RotMatrix = RotZMatrix.Multiply(RotMatrix);
//                UpdateRotation = false;
//            }
//            return RotMatrix;
//        }
//        public Matrix GetTransformMatrix(String ver)
//        {
//            if (UpdateTransform)
//            {
//                UpdateMatrix();
//            }
//            switch (ver)
//            {
//                case "Inverse":
//                    return InverseTransform;

//                default:
//                    return TransformMatrix;
//            }
//        }
//        public Matrix GetTransformMatrix()
//        {
//            return GetTransformMatrix("");
//        }
//        private void UpdateMatrix()
//        {
//            //Model to World
//            TransformMatrix.SetToIdenity();
//            TransformMatrix = GetRotationMatrix().Multiply(TransformMatrix);
//            TransformMatrix = TranslateMatrix.Multiply(TransformMatrix);
//            //World to Model
//            InverseTransform.SetToIdenity();
//            InverseTransform = InverseTranslate().Multiply(InverseTransform);
//            InverseTransform = InverseRotate().Multiply(InverseTransform);
//            UpdateTransform = false;
//        }
//        public Matrix Transform(Matrix ToTransform)
//        {
//            return GetTransformMatrix().Multiply(ToTransform);
//        }
//        public Vector3 Transform(Vector3 ToTransform)
//        {
//            Matrix result = GetTransformMatrix().Multiply(VecToMatrix(ToTransform));
//            return MatrixToVec(result);
//        }
//        public Matrix TransformInverse(Matrix ToTransform)
//        {
//            return GetTransformMatrix("Inverse").Multiply(ToTransform);
//        }
//        public Vector3 TransformInverse(Vector3 ToTransform)
//        {
//            Matrix result = GetTransformMatrix("Inverse").Multiply(VecToMatrix(ToTransform));
//            return MatrixToVec(result);
//        }
//        public Vector3 ModelToWorld(Vector3 ToTransform)
//        {
//            Matrix result = GetTransformMatrix().Multiply(VecToMatrix(ToTransform));
//            return MatrixToVec(result);
//        }
//        public Vector3 WorldToModel(Vector3 ToTransform)
//        {
//            Matrix result = GetTransformMatrix("Inverse").Multiply(VecToMatrix(ToTransform));
//            return MatrixToVec(result);
//        }
//        private Matrix VecToMatrix(Vector3 vec)
//        {
//            Matrix result = new Matrix(1, 4);
//            result.SetValue(0, 0, vec.GetDim(0));
//            result.SetValue(0, 1, vec.GetDim(1));
//            result.SetValue(0, 2, vec.GetDim(2));
//            return result;
//        }
//        private Vector3 MatrixToVec(Matrix m)
//        {
//            return new Vector3(m.GetValue(0, 0), m.GetValue(0, 1), m.GetValue(0, 2));
//        }
//    }
//}
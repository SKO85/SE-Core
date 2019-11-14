//namespace IngameScript.Vector
//{
//    public class Matrix
//    {
//        private uint MyRows;
//        private uint MyCols;
//        private double[][] MyData;
//        public uint Rows
//        {
//            get
//            {
//                return MyRows;
//            }
//        }
//        public uint Cols
//        {
//            get
//            {
//                return MyCols;
//            }
//        }
//        public Matrix(uint dimensions)
//        {
//            //Start with idenity matrix
//            dimensions++;
//            MyRows = dimensions;
//            MyCols = dimensions;
//            MyData = new double[MyCols][];
//            for (var i = 0; i < MyCols; i++)
//            {
//                MyData[i] = new double[MyRows];
//            }
//            SetToIdenity();
//        }
//        public Matrix(uint Cols, uint Rows)
//        {
//            // Start with identity matrix;
//            MyRows = Rows;
//            MyCols = Cols;
//            MyData = new double[MyCols][];
//            for (var i = 0; i < MyCols; i++)
//            {
//                MyData[i] = new double[MyRows];
//            }
//            SetToIdenity();
//        }
//        public Matrix(double[][] data)
//        {
//            MyData = data;
//        }
//        public void SetToIdenity()
//        {
//            for (int x = 0; x < MyCols; x++)
//            {
//                for (int y = 0; y < MyRows; y++)
//                {
//                    if (y == x + (MyRows - MyCols))
//                    {
//                        MyData[x][y] = 1;
//                    }
//                    else
//                    {
//                        MyData[x][y] = 0;
//                    }
//                }
//            }
//        }
//        public double GetValue(uint x, uint y)
//        {
//            return MyData[x][y];
//        }
//        public void SetValue(uint x, uint y, double val)
//        {
//            MyData[x][y] = val;
//        }
//        public Matrix Multiply(Matrix ToMultiply)
//        {
//            Matrix Result = new Matrix(ToMultiply.Cols, MyRows);
//            for (uint x = 0; x < Result.Cols; x++)
//            {
//                for (uint y = 0; y < Result.Rows; y++)
//                {
//                    double val = 0;
//                    for (uint c = 0; c < MyCols; c++)
//                    {
//                        val += MyData[c][y] * ToMultiply.GetValue(x, c);
//                    }
//                    Result.SetValue(x, y, val);
//                }
//            }
//            return Result;
//        }
//    }
//}
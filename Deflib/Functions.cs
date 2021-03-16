using System;

namespace Deflib
{

    //All functions used in ML written in C# for when testing data
    public class Functions
    {        public static void ZeroMatrix(double[, ,] M) 
        {
            int row = M.GetLength(0); // finds dimension
            int col = M.GetLength(1);
            int dim = M.GetLength(2);
            for  (int i = 0; i < row; i++) // loops over row and col 
            {
                for  (int j = 0; j < col; j++)
                {
                    for (int k = 0; k < dim; k++)
                    {
                        M[i,j,k] = 0; // setting all values to 0
                    }

                }
            }
        }

          public static void ZeroMatrix(double[,] M) 
        {
            int row = M.GetLength(0); // finds dimension
            int col = M.GetLength(1);

            for  (int i = 0; i < row; i++) // loops over row and col 
            {
                for  (int j = 0; j < col; j++){
                        M[i,j] = 0; // setting all values to 0
                }
            }
        }
    
        public static T[] Flatten<T>(T[,,] arr)
        {
            int rows0 = arr.GetLength(0);
            int rows1 = arr.GetLength(1);
            int rows2 = arr.GetLength(2);
            T[] arrFlattened = new T[rows0 * rows1* rows2];

            int i, j, k;
            for (k = 0; k < rows0; k++)
            {
                for (j = 0; j < rows1; j++)
                {
                    for (i = 0; i < rows2; i++)
                    {
                        var test = arr[k, j, i];
                        int index = i + j * rows2 + k * rows1 * rows2;
                        arrFlattened[index] = test;
                    }
                }
            }
            return arrFlattened;
        }

        public static T[] Flatten<T>(T[,] arr){
            int rows0 = arr.GetLength(0);
            int rows1 = arr.GetLength(1);
            T[] arrFlattened = new T[rows0 * rows1];

            int i, j;

                for (j = 0; j < rows0; j++)
                {
                    for (i = 0; i < rows1; i++)
                    {
                        var test = arr[j, i];
                        int index = i + j * rows1;
                        arrFlattened[index] = test;
                    }
                }
            return arrFlattened;
        }


        public static double[,] reshape(double[,,] X, int r0, int r1) {
            var Xnew = new double[r0,r1];
            Buffer.BlockCopy(X, 0, Xnew, 0, r0*r1*sizeof(double));
            return Xnew;
        }

        public static double[,] reshape(double[] X, int r0, int r1) {
            var Xnew = new double[r0,r1];
            Buffer.BlockCopy(X, 0, Xnew, 0, r0*r1*sizeof(double));
            return Xnew;
        }
        public static double[,,] reshape(double[] X, int r0, int r1, int r2) {
            var Xnew = new double[r0,r1,r2];
            Buffer.BlockCopy(X, 0, Xnew, 0, r0*r1*r2*sizeof(double));
            return Xnew;
        }
        public static double[,,] reshape(double[,] X, int r0, int r1, int r2) {
            var Xnew = new double[r0,r1,r2];
            Buffer.BlockCopy(X, 0, Xnew, 0, r0*r1*r2*sizeof(double));
            return Xnew;
        }

         public static double[,] reshape(double[,] X, int r0, int r1) {
            var Xnew = new double[r0,r1];
            Buffer.BlockCopy(X, 0, Xnew, 0, r0*r1*sizeof(double));
            return Xnew;
        }



        public static double[,] matmul(double[,] X, double[,] Y) 
        {   

            int x_row = X.GetLength(0); // find dimensions
            int x_col = X.GetLength(1);
            int y_row = Y.GetLength(0);
            int y_col = Y.GetLength(1);
            double[,] result = new double[x_row, y_col];

            for  (int i = 0; i < x_row; i++) // loops over row and col 
            {
               for  (int j = 0; j < y_col ; j++)
               {
                   for (int k = 0; k < x_col; k++)
                   {

                       result[i,j] += X[i,k] * Y[k,j]; 
                        
                   }

                   
               }
            }
            return result;
        }

        public static double[,] transpose(double[,] matrix)
        {
            int w = matrix.GetLength(0);
            int h = matrix.GetLength(1);

            double[,] result = new double[h, w];

            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    result[j, i] = matrix[i, j];
                }
            }

            return result;
        }

        public static double[,] multiply(double[,] mat1, double[,] mat2){

            int a = mat1.GetLength(0);
            int b = mat1.GetLength(1);
            double[,] matresult = new double[a,b];
            
            ZeroMatrix(matresult);
            for  (int i = 0; i < a; i++) {
                for  (int j = 0; j < b; j++) {
                    
                        matresult[i,j] += (mat1[i,j] * mat2[i,j]);
                    
                }
            }       
            return matresult;
        }

        public static double[,,] multiply(double[,,] mat1, double[,,] mat2){

            int a = mat1.GetLength(0);
            int b = mat1.GetLength(1);
            int c = mat1.GetLength(2);
            double[,,] matresult = new double[a,b,c];
            
            ZeroMatrix(matresult);
            for  (int i = 0; i < a; i++) {
                for  (int j = 0; j < b; j++) {
                    for  (int k = 0; k < c; k++) {
                    
                        matresult[i,j,k] += (mat1[i,j,k] * mat2[i,j,k]);
                    }
                    
                }
            }       
            return matresult;
        }

        public static double[] sigmoid(double[,] X)
        {
            //double[] X = Flatten(X);
            int row = X.GetLength(0); // finds dimension
            //int col = X.GetLength(1);

            double[] sig = new double[row];
    
            for (int i = 0; i < row; i++) // loops over row and col 
            {
                    sig[i] = 1 /(1 + Math.Exp(-i));
            }

            return sig;
        }
        public static double[,] sigmoid2d(double[,] X)
        {
            int row = X.GetLength(0); // finds dimension
            int col = X.GetLength(1);

            double[,] sig = new double[row,col];
    
            for (int i = 0; i < row; i++) // loops over row and col 
            {                                            
               for (int j = 0; j < col; j++)
               {
                    sig[i,j] = 1 /(1 + Math.Exp(-X[i,j]));
               }
            }

            return sig;
        }

        public static double[,] softplus(double[,] X)
        {
            int row = X.GetLength(0); // finds dimension
            int col = X.GetLength(1);
            int threshold = 20;
            
            double[,] soft = new double[row,col];

            for (int i = 0; i < row; i++) // loops over row and col 
            {
               for (int j = 0; j < col; j++)
               {
                    if (X[i,j] > threshold){
                        soft[i,j] = X[i,j];
                    } 
                    else{
                        soft[i,j] = Math.Log(1 + Math.Exp(X[i,j]));

                    }
               }
            }

            return soft;
        }
     
        public static double[,] clamp(double[,] matrix, double  min_val, double max_val)
        {
            int row = matrix.GetLength(0); // finds dimension
            int col = matrix.GetLength(1);

            double[,] a = new double[row,col];
            Array.Copy(matrix, a, row * col);
            
             for (int i = 0; i < row; i++) 
             {
                 for (int j = 0; j < col; j++) 
                 {
                    if (matrix[i,j] < min_val)
                    {
                        a[i,j] = min_val;
                    }
                    else if (matrix[i,j] > max_val)
                    {
                        a[i,j] = max_val;
                    }
                 }

             }
        return a;
        }


        public class RandomProportional : Random
        {
            // The Sample method generates a distribution proportional to the value 
            // of the random numbers, in the range [0.0, 1.0].
            protected override double Sample( )
            {
                return Math.Sqrt( base.Sample( ) );
            }
        
            public override int Next()
            {
            return (int) (Sample() * int.MaxValue);
            }   
        }

        public static double[] uniform(int x)
        {
            double[] M = new double[x];
            RandomProportional randObj = new RandomProportional( );
            
              // Generate and display [rows * cols] random doubles.
            for( int i = 0; i < x; i++ ) {
                    M[i] = randObj.NextDouble();
            }
            return M;
        }    

          public static double[,] uniform(int x, int y)
        {
            double[,] M = new double[x,y];
            RandomProportional randObj = new RandomProportional( );
            
              // Generate and display [rows * cols] random doubles.
            for( int i = 0; i < x; i++ )
            {
                for( int j = 0; j < y; j++ )
                {
                        M[i,j] = randObj.NextDouble();
                    
                }
                    
            }
            return M;
        }    

        public static double[,,] uniform(int x, int y, int z)
        {

            double[,,] M = new double[x,y,z];
            RandomProportional randObj = new RandomProportional( );
            
              // Generate and display [rows * cols] random doubles.
            for( int i = 0; i < x; i++ )
            {
                for( int j = 0; j < y; j++ )
                {
                    for( int k = 0; k < z; k++ )
                    {
                        M[i,j,k] = randObj.NextDouble();
                    }
                    
                }
                    
            }
            return M;
        }



          public static double mean(double[,] lst)
        {   
            int x = lst.GetLength(0);
            int y = lst.GetLength(1);
            Console.WriteLine($"y{y}");
            double value = 0;
            
             for(int i = 0; i < x; i++ )
            {  
                for(int j = 0; j < y ; j++)
                {
                    value += lst[i,j];
                    
                } 
                  
            }
            //Console.WriteLine($"value{value}");  
            value /= x*y;
            
            return value; 
        }


        public static void PrintArray(double[] M)
        {
            int n = M.GetLength(0);
            for  (int i = 0; i < n; i++)
            {
                   Console.Write($"{M[i]} ");
            }   
        }
    

        public static void PrintMatrix(double[,] M)
        {
            int row = M.GetLength(0);
            int col = M.GetLength(1);
            for  (int i = 0; i < row; i++)
            {
               for  (int j = 0; j < col; j++) 
               {
                   Console.Write($"{M[i,j]} ");
               }
               Console.WriteLine("");
            }   
        }

        public static void PrintTensor(double[,,] M){
            int row = M.GetLength(0);
            int col = M.GetLength(1);
            int dim = M.GetLength(2);
            for  (int i = 0; i < row; i++)
            {
               for  (int j = 0; j < col; j++) 
               {
                    for  (int k = 0; k < dim; k++) 
                    {
                      Console.Write($"{M[i,j,k]} ");
                    }  
                    Console.WriteLine(" ");  
                }

            }  
 
        }


    }
}

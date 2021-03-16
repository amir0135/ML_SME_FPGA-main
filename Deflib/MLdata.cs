using System;
using System.Linq;
using System.IO;
using System.Globalization;


namespace Deflib
{

    //All functions used in ML written in C# for when testing data
    public class Generate_data{

        public static double[] csvfile(string file){

            var A = File.ReadAllLines(file)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => double.Parse(x.Trim(), CultureInfo.InvariantCulture))
            .ToArray();

            return A;
        }

        public static double[,] csvfile(string file, int x, int y){

            var A = File.ReadAllLines(file)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => double.Parse(x.Trim(), CultureInfo.InvariantCulture))
            .ToArray();

            var Reshape = Functions.reshape(A, x, y);

            return Reshape;
        }
        public static double[,,] csvfile(string file, int x, int y, int z){

            var A = File.ReadAllLines(file)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => double.Parse(x.Trim(), CultureInfo.InvariantCulture))
            .ToArray();

            var Reshape = Functions.reshape(A, x, y, z);

            return Reshape;

        }


        public static double[,] matmul_mat(double[,] x, double[,,] W0){
             

            var hh = Functions.matmul(x, 
                Functions.transpose(
                    Functions.reshape(W0, (int)Parameters.num_networks * (int)Parameters.hidden_size, (int)Parameters.input_size)
                )
            );

            return hh;
        }


         public static Tuple<double[,,], double[,,]> generate_hz(double[,] hh, double[,] x)//int[,] x)
        {   var batchsize = x.GetLength(0);
        
            var h = Functions.reshape(hh, batchsize, (int)Parameters.num_networks, (int)Parameters.hidden_size);
            
            int a = h.GetLength(0);
            int b = h.GetLength(1);
            int c = h.GetLength(2);
            
            double[,,] hz = new double[a,b,c];
            for  (int i = 0; i < a; i++) {
                for  (int j = 0; j < b; j++) {
                    for  (int k = 0; k < c; k++) {
                        if (h[i,j,k] < 0) {
                            
                            hz[i,j,k] = h[i,j,k]* dataMatrix.prelu_z_slopes[j];

                        }
                        else {
                            hz[i,j,k] = h[i,j,k];
                        }
                    }
                }
            }

            
            double[,,] hr = new double[a,b,c];
            for  (int i = 0; i < a; i++) {
                for  (int j = 0; j < b; j++) {
                    for  (int k = 0; k < c; k++) {
                        if (h[i,j,k] < 0) {
                            
                            hr[i,j,k] = h[i,j,k]* dataMatrix.prelu_r_slopes[j];
                        }
                        else {
                            hr[i,j,k] = h[i,j,k];
                        }
                    }
                }
            }

            
            return new Tuple<double[,,], double[,,]>(hz, hr);
        }

        public static Tuple<double[,,], double[,,]> z_r(double[,,] hz, double[,,] hr ){
            
            int a = hz.GetLength(0);
            int b = hz.GetLength(1);
            int c = hz.GetLength(2);

            double[,,] wz = new double[a,b,c];
            for (int i = 0;  i < a; i++){
                for (int j = 0;  j < b; j++){
                    for (int k = 0;  k < c; k++){
                    wz[i,j,k] = dataMatrix.Wz[j,k];
                    }
                }
            }

            double[,,] z = new double[a,b,c];
            Functions.ZeroMatrix(z);
            for  (int i = 0; i < a; i++) {
                for  (int j = 0; j < b; j++) {
                    for  (int k = 0; k < c; k++) {
                        z[i,j,k] += (hz[i,j,k] * wz[i,j,k]);
                    }
                    
                }

            } 

            int e = hr.GetLength(0);
            int f = hr.GetLength(1);
            int g = hr.GetLength(2);

            double[,,] wr = new double[e,f,g];
            for (int i = 0;  i < e; i++){
                for (int j = 0;  j < f; j++){
                    for (int k = 0;  k < g; k++){
                    wr[i,j,k] = dataMatrix.Wr[j,k];
                    }
                }
            }

            double[,,] r = new double[e,f,g];
            Functions.ZeroMatrix(r);
            for  (int i = 0; i < e; i++) {
                for  (int j = 0; j < f; j++) {
                    for  (int k = 0; k < g; k++) {
                        r[i,j,k] += (hr[i,j,k] * wr[i,j,k]);
                    }
                    
                }

            } 

            return new Tuple<double[,,], double[,,]>(z, r);  

        }



        public static Tuple<double[,], double[,]> gen_SLA(double[,,] z, double[,,] r){

            
            int a = z.GetLength(0);
            int b = z.GetLength(1);
            int c = z.GetLength(2);
            double[,] sum_z_lastaxis = new double[a,b];
            Functions.ZeroMatrix(sum_z_lastaxis);
            for  (int i = 0; i < a; i++) {
                for  (int j = 0; j < b; j++) {
                    for  (int k = 0; k < c; k++) {
                        sum_z_lastaxis[i,j] += z[i,j,k] ;
                    }
                    
                }

            } 
                  

            int e = r.GetLength(0);
            int f = r.GetLength(1);
            int g = r.GetLength(2);
            double[,] sum_r_lastaxis = new double[e,f];
            Functions.ZeroMatrix(sum_r_lastaxis);
            for  (int i = 0; i < e; i++) {
                for  (int j = 0; j < f; j++) {
                    for  (int k = 0; k < g; k++) {
                        sum_r_lastaxis[i,j] += r[i,j,k];
                    

                    }
                }
            }                      
        
            return new Tuple<double[,], double[,]>(sum_z_lastaxis, sum_r_lastaxis);  
        }

        public static double[,] zz (double[,] z){
            int a = z.GetLength(0);
            int b = z.GetLength(1);

            double[,] zz_scale = new double[a,b];
            for (int i = 0;  i < a; i++){
                for (int j = 0;  j < b; j++){
                    zz_scale[i,j] = dataMatrix.z_scale[j];
                }
            }

            var zz = Functions.multiply(zz_scale,z);
        
            return zz;

        }

        public static double[,] sig (double[,] zz){
            
            var zz_sig = Functions.sigmoid2d(zz);

            return zz_sig;
        }

        public static double[,] mulmin (double[,] zz_sig){

            int a = zz_sig.GetLength(0);
            int b = zz_sig.GetLength(1);

            for (int i = 0;  i < a; i++){
                for (int j = 0;  j < b; j++){
                    zz_sig[i,j] *= 2;
                    zz_sig[i,j] -= 1;
                }
            }

            return zz_sig;
        }

        public static double[,] soft(double[,] r){

            var soft = Functions.softplus(r);

            return soft;
        }


        public static double[,] rz(double[,] soft,double[,] mulmin)
        {
            
            var rz = Functions.multiply(soft,mulmin);

            return rz;
        }

        public static double[,] clamp(double[,] rz){
            var clamp_fun = Functions.clamp(rz, - (int)Parameters.max_predict, (int)Parameters.max_predict);

            return clamp_fun;
        }

        public static double[] ensemble_predictions(double[,] clamp){

            
            var y_mean = Functions.mean(clamp);
            double[] mean = new double[1] {y_mean};
            return mean;
        }





        





    }




    
}
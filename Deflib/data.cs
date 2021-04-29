using System;
using Deflib;

namespace Deflib
{

    public class data
    {
        public static float[] W0 = Generate_data.csvfile("../Data/W0.csv");
        public static float[] prelu_z_slopes = Generate_data.csvfile("../Data/prelu_z_slopes.csv");
        public static float[] prelu_r_slopes = Generate_data.csvfile("../Data/prelu_r_slopes.csv");
        public static float[] Wz = Generate_data.csvfile("../Data/Wz.csv");
        public static float[] Wr = Generate_data.csvfile("../Data/Wr.csv");
        public static float[] z_scale = Generate_data.csvfile("../Data/z_scale.csv");
        public static float[] x = Generate_data.csvfile("../Data/x.csv");
    }

    public class dataMatrix
    {
        public static float[,,] W0 = Generate_data.csvfile("../Data/W0.csv", (int)Deflib.Parameters.num_networks, (int)Deflib.Parameters.hidden_size, (int)Deflib.Parameters.input_size);
        public static float[] prelu_z_slopes = Generate_data.csvfile("../Data/prelu_z_slopes.csv");
        public static float[] prelu_r_slopes = Generate_data.csvfile("../Data/prelu_r_slopes.csv");
        public static float[,] Wz = Generate_data.csvfile("../Data/Wz.csv", (int)Deflib.Parameters.num_networks, (int)Deflib.Parameters.hidden_size);
        public static float[,] Wr = Generate_data.csvfile("../Data/Wr.csv", (int)Deflib.Parameters.num_networks, (int)Deflib.Parameters.hidden_size);
        public static float[] z_scale = Generate_data.csvfile("../Data/z_scale.csv");
        public static float[,] x = Generate_data.csvfile("../Data/x.csv", 1, (int)Deflib.Parameters.input_size);
    }

}
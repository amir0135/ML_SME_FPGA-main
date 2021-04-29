using System;
using Deflib;

namespace Matmul
{

    public class Generate_transpose
    {
        public static float[] transpose()
        {
            var reshape_tra = Deflib.Functions.reshape(Deflib.dataMatrix.W0, (int)Deflib.Parameters.num_networks * (int)Deflib.Parameters.hidden_size, (int)Deflib.Parameters.input_size);
            var transpose = Deflib.Functions.transpose(reshape_tra);

            return Deflib.Functions.Flatten(transpose);
        }
    }

}
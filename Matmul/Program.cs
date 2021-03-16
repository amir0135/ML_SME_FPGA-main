using System;
using SME;
using System.IO;
using System.Linq;
using System.Globalization;
using SME.Components;
using Deflib;

namespace Matmul
{       

    class MainClass{
        public static void Main(string[] args)
        {
            using (var sim = new Simulation())
            {

                ////// Test data///////////////
                var control_x = Scope.CreateBus<IndexControl>();
                var control_W0 = Scope.CreateBus<IndexControl>();

                var x_data =  Deflib.Generate_data.csvfile("../Data/x.csv");
                var tra_W0_data = Generate_transpose.transpose();
                

                var matmul_expected = Deflib.Functions.Flatten(Deflib.Generate_data.matmul_mat(Deflib.dataMatrix.x, Deflib.dataMatrix.W0));

                var array_x = new SME.Components.SimpleDualPortMemory<double>((int)Deflib.Parameters.Batchsize*(int)Deflib.Parameters.input_size,x_data);  
                var array_W0 = new SME.Components.SimpleDualPortMemory<double>((int)Deflib.Parameters.num_networks* (int)Deflib.Parameters.hidden_size*(int)Deflib.Parameters.input_size, tra_W0_data); 

                var matmul_x_ready = new Deflib.TestIndexSim(control_x, (int)Deflib.Parameters.Batchsize,(int)Deflib.Parameters.input_size);
                var prelu_W0_ready = new Deflib.TestIndexSim(control_W0, (int)Deflib.Parameters.input_size, (int)Deflib.Parameters.num_networks* (int)Deflib.Parameters.hidden_size);
                var matmul = new MatmulStage(control_x, control_W0, array_x, array_W0);
                
                var outsimtra = new OutputSim(matmul.control_out, matmul.ram_out_1, matmul_expected);
            


                sim

                    .BuildCSVFile()
                    .BuildGraph()
                    .Run();
            }        
        }
    


    }
    

    public class MatmulStage{

    public SME.Components.SimpleDualPortMemory<double> ram_out_1;
    public IndexControl control_out;
        public MatmulStage(IndexControl controlA_in, IndexControl controlB_in, SME.Components.SimpleDualPortMemory<double> ramA_in, SME.Components.SimpleDualPortMemory<double> ramB_in ){

            var matmulindex_A = Scope.CreateBus<IndexValue>();
            var matmulindex_B = Scope.CreateBus<IndexValue>();
            var matmulindex_C = Scope.CreateBus<IndexValue>();
            var matmulindex_AB = Scope.CreateBus<IndexValue>();
            var converter_toram = Scope.CreateBus<ValueTransfer>();
            control_out = Scope.CreateBus<IndexControl>();

            var pipeoutmat = Scope.CreateBus<IndexValue>();
            var pipe1outmat = Scope.CreateBus<IndexValue>();

            var forwarded = Scope.CreateBus<SME.Components.SimpleDualPortMemory<double>.IReadResult>();
            var matmulresult = Scope.CreateBus<ValueTransfer>();
            

            var ram_out = new SME.Components.SimpleDualPortMemory<double>((int)Deflib.Parameters.num_networks* (int)Deflib.Parameters.hidden_size);
            var ram_out_AB = new SME.Components.SimpleDualPortMemory<double>((int)Deflib.Parameters.num_networks* (int)Deflib.Parameters.hidden_size);
            ram_out_1 = new SME.Components.SimpleDualPortMemory<double>((int)Deflib.Parameters.num_networks* (int)Deflib.Parameters.hidden_size);

            var generatemat_x = new Generate(matmulindex_A, ramA_in.ReadControl);
            var generatemat_W0 = new Generate(matmulindex_B, ramB_in.ReadControl);
            var generate_AB = new Generate(matmulindex_AB, ram_out_AB.ReadControl)
            var generatematmul = new Generate(matmulindex_C, ram_out.ReadControl);
            
            var matmulind = new MatMulIndex(controlA_in, controlB_in, matmulindex_A, matmulindex_B, matmulindex_C, control_out);
            var pipematmul = new Pipe(matmulindex_C, pipeoutmat);
            var forward = new Forward(pipeoutmat,pipe1outmat, matmulresult, ram_out.ReadResult, forwarded);
            var matmul = new MatMul(pipeoutmat, ramA_in.ReadResult, ramB_in.ReadResult, forwarded , matmulresult)
            // skal jeg lave en ny matmulresult, pipeoutmat og forward bus???
            var matmul_add = new MatMul_add(pipeoutmat, ramAB_in.ReadResult, forwarded , matmulresult);
            var pipe1 = new Pipe(pipeoutmat, pipe1outmat);

            var toram = new ToRam(matmulresult, pipe1outmat, ram_out.WriteControl);
            var toram_1 = new ToRam(matmulresult, pipe1outmat, ram_out_1.WriteControl);



        }
    }

}
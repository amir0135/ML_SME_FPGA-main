using System;
using SME;
using Deflib;

namespace RZ
{

    class MainClass
    {
        public static void Main(string[] args)
        {
            using (var sim = new Simulation())
            {
                ////// Test data r///////////////
                var control_sig = Scope.CreateBus<IndexControl>();
                var control_soft = Scope.CreateBus<IndexControl>();

                var control_rz = Scope.CreateBus<IndexControl>();

                var soft_data = Generate_rz.gen_soft_mulmin().Item1;
                var soft_flat = Deflib.Functions.Flatten(soft_data);

                var sig_data = Generate_rz.gen_soft_mulmin().Item2;
                var mulmin_flat = Deflib.Functions.Flatten(sig_data);

                var RZ_expected = Deflib.Functions.Flatten(Deflib.Generate_data.rz(soft_data, sig_data));

                var array_soft = new SME.Components.SimpleDualPortMemory<double>((int)Deflib.Parameters.num_networks, soft_flat);

                var array_sig = new SME.Components.SimpleDualPortMemory<double>((int)Deflib.Parameters.num_networks, mulmin_flat);

                var index_rz = new TestIndexSim(control_rz, 1, (int)Deflib.Parameters.num_networks);
                var sig_ready = new TestIndexSim(control_sig, 1, (int)Deflib.Parameters.num_networks);
                var soft_ready = new TestIndexSim(control_soft, 1, (int)Deflib.Parameters.num_networks);
                var rz = new RZStage(control_rz, control_sig, control_soft, array_sig, array_soft);

                var outsimtra = new OutputSim(rz.control_out, rz.ram_out, RZ_expected);

                sim
                    .BuildCSVFile()
                    .BuildGraph()
                    .Run();
            }
        }
    }

    public class RZStage
    {
        public SME.Components.SimpleDualPortMemory<double> ram_out;
        public IndexControl control_out;

        public RZStage(IndexControl testcontrol, IndexControl controlA_in, IndexControl controlB_in, SME.Components.SimpleDualPortMemory<double> ramA_in, SME.Components.SimpleDualPortMemory<double> ramB_in)
        {
            var rz_index_A = Scope.CreateBus<IndexValue>();
            var rz_index_B = Scope.CreateBus<IndexValue>();

            var pipeout_rz = Scope.CreateBus<IndexValue>();
            var pipe1out_rz = Scope.CreateBus<IndexValue>();
            var rz_result = Scope.CreateBus<ValueTransfer>();

            control_out = Scope.CreateBus<IndexControl>();

            ram_out = new SME.Components.SimpleDualPortMemory<double>((int)Deflib.Parameters.num_networks);

            var generate_rz_A = new Generate(rz_index_A, ramA_in.ReadControl);
            var generate_rz_B = new Generate(rz_index_B, ramB_in.ReadControl);

            var rz_ind = new MulIndex(controlA_in, controlB_in, rz_index_A, rz_index_B, control_out, testcontrol);
            var pipe_rz = new Pipe(rz_index_B, pipeout_rz);
            var rz = new Mul(pipeout_rz, ramA_in.ReadResult, ramB_in.ReadResult, rz_result);
            var pipe_rz_1 = new Pipe(pipeout_rz, pipe1out_rz);

            var toram_rz = new ToRam(rz_result, pipe1out_rz, ram_out.WriteControl);
        }
    }

}
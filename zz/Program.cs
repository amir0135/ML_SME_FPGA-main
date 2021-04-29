using System;
using SME;
using Deflib;

namespace zz
{

    class MainClass
    {
        public static void Main(string[] args)
        {
            using (var sim = new Simulation())
            {
                var control_z_scale = Scope.CreateBus<IndexControl>();
                var control_SLA = Scope.CreateBus<IndexControl>();

                var control_zz = Scope.CreateBus<IndexControl>();

                var SLA_data = Generate_sla.gen_sla();
                var SLA_flat = Deflib.Functions.Flatten(SLA_data.Item1);

                var zz_expected = Deflib.Functions.Flatten(Deflib.Generate_data.zz(SLA_data.Item1));

                var array_zscale = new SME.Components.SimpleDualPortMemory<float>((int)Deflib.Parameters.num_networks, Deflib.data.z_scale);
                var array_SLA = new SME.Components.SimpleDualPortMemory<float>((int)Deflib.Parameters.num_networks, SLA_flat);

                //size of output array
                var index_zz = new TestIndexSim(control_zz, 1, (int)Deflib.Parameters.num_networks);

                //size of input array
                var zscale_ready = new TestIndexSim(control_z_scale, 1, (int)Deflib.Parameters.num_networks);

                var SLA_ready = new TestIndexSim(control_SLA, 1, (int)Deflib.Parameters.num_networks);
                var zz = new zz_Stage(control_zz, control_z_scale, control_SLA, array_zscale, array_SLA);

                var outsimtra = new OutputSim(zz.control_out, zz.ram_out, zz_expected);

                sim
                    .BuildCSVFile()
                    .BuildGraph()
                    .Run();
            }
        }
    }

    public class zz_Stage
    {
        public SME.Components.SimpleDualPortMemory<float> ram_out;
        public IndexControl control_out;

        public zz_Stage(IndexControl testcontrol, IndexControl controlA_in, IndexControl controlB_in, SME.Components.SimpleDualPortMemory<float> ramA_in, SME.Components.SimpleDualPortMemory<float> ramB_in)
        {
            var zz_index_A = Scope.CreateBus<IndexValue>();
            var zz_index_B = Scope.CreateBus<IndexValue>();

            var pipeout_zz = Scope.CreateBus<IndexValue>();
            var pipe1out_zz = Scope.CreateBus<IndexValue>();
            var zz_result = Scope.CreateBus<ValueTransfer>();

            control_out = Scope.CreateBus<IndexControl>();

            ram_out = new SME.Components.SimpleDualPortMemory<float>((int)Deflib.Parameters.num_networks);

            var generate_zz_A = new Generate(zz_index_A, ramA_in.ReadControl);
            var generate_zz_B = new Generate(zz_index_B, ramB_in.ReadControl);

            var zz_ind = new MulIndex(controlA_in, controlB_in, zz_index_A, zz_index_B, control_out, testcontrol);
            var pipe_zz = new Pipe(zz_index_A, pipeout_zz);
            var zz = new Mul(pipeout_zz, ramA_in.ReadResult, ramB_in.ReadResult, zz_result);
            var pipe_zz_1 = new Pipe(pipeout_zz, pipe1out_zz);

            var toram_zz = new ToRam(zz_result, pipe1out_zz, ram_out.WriteControl);
        }
    }

}
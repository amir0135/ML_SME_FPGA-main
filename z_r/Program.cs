using System;
using System.Linq;
using SME;
using Deflib;

namespace z_r
{

    class MainClass
    {
        public static void Main(string[] args)
        {
            using (var sim = new Simulation())
            {
                ////// Test data z///////////////
                var control_hz = Scope.CreateBus<IndexControl>();
                var control_wz = Scope.CreateBus<IndexControl>();

                var control_z = Scope.CreateBus<IndexControl>();

                var hz_data = Generate_hz.generate_hz();
                var hz_flat = Deflib.Functions.Flatten(hz_data.Item1);

                var z_expected = Deflib.Functions.Flatten(Deflib.Generate_data.z_r(hz_data.Item1, hz_data.Item2).Item1);

                var array_hz = new SME.Components.SimpleDualPortMemory<float>((int)Deflib.Parameters.num_networks * (int)Deflib.Parameters.hidden_size, hz_flat);
                var array_wz = new SME.Components.SimpleDualPortMemory<float>((int)Deflib.Parameters.num_networks * (int)Deflib.Parameters.hidden_size, Deflib.data.Wz);

                var index_z = new TestIndexSim(control_z, (int)Deflib.Parameters.num_networks, (int)Deflib.Parameters.hidden_size);
                var hz_ready = new TestIndexSim(control_hz, (int)Deflib.Parameters.num_networks, (int)Deflib.Parameters.hidden_size, 1);
                var wz_ready = new TestIndexSim(control_wz, (int)Deflib.Parameters.num_networks, (int)Deflib.Parameters.hidden_size);
                var z = new zStage(control_z, control_hz, control_wz, array_hz, array_wz);

                var outsim_z = new OutputSim(z.control_out, z.ram_out, z_expected);

                ////// Test data r///////////////
                var control_hr = Scope.CreateBus<IndexControl>();
                var control_wr = Scope.CreateBus<IndexControl>();

                var control_r = Scope.CreateBus<IndexControl>();

                var hr_data = Generate_hz.generate_hz();
                var hr_flat = Deflib.Functions.Flatten(hr_data.Item2);

                var r_expected = Deflib.Functions.Flatten(Deflib.Generate_data.z_r(hr_data.Item1, hr_data.Item2).Item2);

                var array_hr = new SME.Components.SimpleDualPortMemory<float>((int)Deflib.Parameters.num_networks * (int)Deflib.Parameters.hidden_size, hr_flat);
                var array_wr = new SME.Components.SimpleDualPortMemory<float>((int)Deflib.Parameters.num_networks * (int)Deflib.Parameters.hidden_size, Deflib.data.Wr);

                var index_r = new TestIndexSim(control_r, (int)Deflib.Parameters.num_networks, (int)Deflib.Parameters.hidden_size);
                var hr_ready = new TestIndexSim(control_hr, (int)Deflib.Parameters.num_networks, (int)Deflib.Parameters.hidden_size, 1);
                var wr_ready = new TestIndexSim(control_wr, (int)Deflib.Parameters.num_networks, (int)Deflib.Parameters.hidden_size);
                var r = new zStage(control_r, control_hr, control_wr, array_hr, array_wr);

                var outsim_r = new OutputSim(r.control_out, r.ram_out, r_expected);

                sim
                    .AddTopLevelInputs(control_hz, control_wz, control_z, control_hr, control_wr, control_r, array_hz.WriteControl, array_wz.WriteControl, array_hr.WriteControl, array_wr.WriteControl, z.ram_out.ReadControl, r.ram_out.ReadControl)
                    .AddTopLevelOutputs(z.control_out, r.control_out, z.ram_out.ReadResult, r.ram_out.ReadResult)
                    .BuildCSVFile()
                    .BuildGraph()
                    .BuildVHDL()
                    .Run(exitMethod:
                        () =>
                        {
                            OutputSim[] procs = { outsim_z, outsim_r };
                            return procs.All(x => ((IProcess)x).Finished().IsCompleted);
                        }
                    );
            }
        }
    }

    public class zStage
    {
        public SME.Components.SimpleDualPortMemory<float> ram_out;
        public IndexControl control_out;

        public zStage(IndexControl testcontrol, IndexControl controlA_in, IndexControl controlB_in, SME.Components.SimpleDualPortMemory<float> ramA_in, SME.Components.SimpleDualPortMemory<float> ramB_in)
        {
            var z_index_A = Scope.CreateBus<IndexValue>();
            var z_index_B = Scope.CreateBus<IndexValue>();

            var pipeout_z = Scope.CreateBus<IndexValue>();
            var pipe1out_z = Scope.CreateBus<IndexValue>();
            var z_result = Scope.CreateBus<ValueTransfer>();
            control_out = Scope.CreateBus<IndexControl>();

            ram_out = new SME.Components.SimpleDualPortMemory<float>((int)Deflib.Parameters.num_networks * (int)Deflib.Parameters.hidden_size);

            var generate_z_A = new Generate(z_index_A, ramA_in.ReadControl);
            var generate_z_B = new Generate(z_index_B, ramB_in.ReadControl);

            var z_ind = new ZIndex(controlA_in, controlB_in, z_index_A, z_index_B, control_out, testcontrol);
            var pipe_z = new Pipe(z_index_A, pipeout_z);
            var z = new Mul(pipeout_z, ramA_in.ReadResult, ramB_in.ReadResult, z_result);
            var pipe_z_1 = new Pipe(pipeout_z, pipe1out_z);

            var toram_z = new ToRam(z_result, pipe1out_z, ram_out.WriteControl);
        }
    }

}
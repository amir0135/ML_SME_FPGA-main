using System;
using SME;
using System.IO;
using System.Linq;
using System.Globalization;
using Deflib;

namespace sum_lastaxis
{       

    class MainClass{
        public static void Main(string[] args)
        {
            using (var sim = new Simulation())
            {
                //// Test data SLA_z///////////////
                var control_z = Scope.CreateBus<IndexControl>();
                
                var control_SLA_z = Scope.CreateBus<IndexControl>();

                var z_data = Generate_z_r.gen_z_r();
                var z_flat = Deflib.Functions.Flatten(z_data.Item1);
                // var r_flat = Deflib.Functions.Flatten(z_r.Item2);

                var SLA_z_expected = Deflib.Functions.Flatten(Deflib.Generate_data.gen_SLA(z_data.Item1,z_data.Item2).Item1);

                var array_z = new SME.Components.SimpleDualPortMemory<double>((int)Deflib.Parameters.num_networks * (int)Deflib.Parameters.hidden_size, z_flat); 

                var index_SLA_z = new TestIndexSim(control_SLA_z, (int)Deflib.Parameters.num_networks,  (int)Deflib.Parameters.hidden_size);
                var z_ready = new TestIndexSim(control_z, (int)Deflib.Parameters.num_networks, (int)Deflib.Parameters.hidden_size);

                var SLA_z = new SLA_Stage(control_SLA_z, control_z, array_z);
                
                var outsimtra = new OutputSim(SLA_z.control_out, SLA_z.ram_out, SLA_z_expected);




                //// Test data sla_r///////////////
                var control_r = Scope.CreateBus<IndexControl>();
                
                var control_SLA_r = Scope.CreateBus<IndexControl>();

                var r_data = Generate_z_r.gen_z_r();
                var r_flat = Deflib.Functions.Flatten(r_data.Item1);
                // var r_flat = Deflib.Functions.Flatten(r_r.Item2);

                var SLA_r_expected = Deflib.Functions.Flatten(Deflib.Generate_data.gen_SLA(r_data.Item1,r_data.Item2).Item1);

                var array_r = new SME.Components.SimpleDualPortMemory<double>((int)Deflib.Parameters.num_networks * (int)Deflib.Parameters.hidden_size, r_flat); 

                var index_SLA_r = new TestIndexSim(control_SLA_r, (int)Deflib.Parameters.num_networks, (int)Deflib.Parameters.hidden_size);
                var Wr_ready = new TestIndexSim(control_r,(int)Deflib.Parameters.num_networks, (int)Deflib.Parameters.hidden_size);
                var SLA_r = new SLA_Stage(control_SLA_r, control_r, array_r);
                
               // var outsimtra = new OutputSim(SLA_r.control_out, SLA_r.ram_out, SLA_r_expected);

                
                sim
                    .BuildCSVFile()
                    .BuildGraph()
                    .Run();
            }        
        }
    


        }
    public class SLA_Stage{

        public SME.Components.SimpleDualPortMemory<double> ram_out;
        public IndexControl control_out;
        public SLA_Stage(IndexControl testcontrol, IndexControl control_in,  SME.Components.SimpleDualPortMemory<double> ram_in ){

            var SLA_z_index_A = Scope.CreateBus<IndexValue>();
            var SLA_z_index_B = Scope.CreateBus<IndexValue>();

            var pipeout_z_SLA = Scope.CreateBus<IndexValue>();
            var pipeout1_z_SLA = Scope.CreateBus<IndexValue>();
            var SLA_z_result = Scope.CreateBus<ValueTransfer>();
            control_out = Scope.CreateBus<IndexControl>();


            ram_out = new SME.Components.SimpleDualPortMemory<double>((int)Deflib.Parameters.num_networks);

            var generate_z_SLA = new Generate(SLA_z_index_A, ram_in.ReadControl); 

            var SLA_z_ind = new SLAIndex(control_in, SLA_z_index_A, SLA_z_index_B, control_out,  testcontrol);
            var pipe_z_SLA = new Pipe(SLA_z_index_B, pipeout_z_SLA);
            var SLA_z = new SumLastAxis(ram_in.ReadResult, pipeout_z_SLA, SLA_z_result);
            var pipe1_z_SLA = new Pipe(pipeout_z_SLA, pipeout1_z_SLA );

            var toram_z_SLA = new ToRam(SLA_z_result, pipeout1_z_SLA, ram_out.WriteControl);





        }
    }
}
using System;
using SME;
using Deflib;

namespace Transpose
{

    class MainClass
    {
        public static void Main(string[] args)
        {
            using (var sim = new Simulation())
            {
                ////// Test data///////////////
                var test_control = Scope.CreateBus<IndexControl>();

                var transpose_expected = Deflib.Functions.Flatten(Generate_data.transpose());

                var array_test = new SME.Components.SimpleDualPortMemory<float>((int)Deflib.Parameters.num_networks * (int)Deflib.Parameters.hidden_size * (int)Deflib.Parameters.input_size, Deflib.data.W0);

                var testind = new TestIndexSim(test_control, (int)Deflib.Parameters.input_size, (int)Deflib.Parameters.num_networks * (int)Deflib.Parameters.hidden_size);
                var transpose = new TransposeStage(test_control, array_test);

                var outsimtra = new OutputSim(transpose.control_out, transpose.ram_out, transpose_expected);

                sim
                    .AddTopLevelInputs(test_control, array_test.WriteControl, transpose.ram_out.ReadControl)
                    .AddTopLevelOutputs(transpose.control_out, transpose.ram_out.ReadResult)
                    .BuildCSVFile()
                    .BuildGraph()
                    .BuildVHDL()
                    .Run();
            }
        }
    }

    public class TransposeStage
    {
        public SME.Components.SimpleDualPortMemory<float> ram_out;
        public IndexControl control_out;

        public TransposeStage(IndexControl control_in, SME.Components.SimpleDualPortMemory<float> ram_in)
        {
            var transposeindex_W0_A = Scope.CreateBus<IndexValue>();
            var transposeindex_W0_B = Scope.CreateBus<IndexValue>();
            control_out = Scope.CreateBus<IndexControl>();
            var pipeouttra_W0 = Scope.CreateBus<IndexValue>();
            var converter_toram = Scope.CreateBus<ValueTransfer>();

            var generate_tra_W0 = new Generate(transposeindex_W0_A, ram_in.ReadControl);

            ram_out = new SME.Components.SimpleDualPortMemory<float>((int)Deflib.Parameters.num_networks * (int)Deflib.Parameters.hidden_size * (int)Deflib.Parameters.input_size);

            var transposeind_tra_W0 = new Transpose(control_in, transposeindex_W0_A, transposeindex_W0_B, control_out);
            var pipe_tra_W0 = new Pipe(transposeindex_W0_B, pipeouttra_W0);
            var toram_convert = new Value_Converter(pipeouttra_W0, ram_in.ReadResult, converter_toram);
            var toram_tra_W0 = new ToRam(converter_toram, pipeouttra_W0, ram_out.WriteControl);
        }
    }

}
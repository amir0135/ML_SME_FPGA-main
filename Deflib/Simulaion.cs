using System;
using SME;

namespace Deflib
{

    public class OutputSim : SimulationProcess
    {
        [InputBus]
        public IndexControl index;
        [InputBus]
        private SME.Components.SimpleDualPortMemory<float>.IReadResult ignore;

        private SME.Components.SimpleDualPortMemory<float> ram;

        private float[] expected;

        public OutputSim(IndexControl index, SME.Components.SimpleDualPortMemory<float> ram, float[] expected)
        {
            this.index = index;
            this.ram = ram;
            ignore = ram.ReadResult;
            this.expected = expected;
        }

        public override async System.Threading.Tasks.Task Run()
        {
            await ClockAsync();
            while (!index.Ready)
                await ClockAsync();

            await ClockAsync();
            var match = true;
            for (int i = 0; i < expected.Length; i++)
            {
                match = match && Math.Abs(ram.m_memory[i] - expected[i]) < 0.0000001;
                if (!(Math.Abs(ram.m_memory[i] - expected[i]) < 0.0000001))
                    Console.WriteLine($"{i} {ram.m_memory[i]} != {expected[i]}");
            }

            System.Diagnostics.Debug.Assert(match, "expected did not match result");
        }
    }

}
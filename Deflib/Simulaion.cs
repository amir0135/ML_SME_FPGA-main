   
using System;
using SME;
using System.Linq;
using System.IO;
using System.Globalization;

namespace Deflib
{       

        public class OutputSim_T : SimulationProcess{
        [InputBus]
        public IndexControl index;

        private SME.Components.SimpleDualPortMemory<double> ram;

        public OutputSim_T( IndexControl index, SME.Components.SimpleDualPortMemory<double> ram){
            this.index = index;
            this.ram = ram;
        }

        public override async System.Threading.Tasks.Task Run(){
            await ClockAsync();
            while (!index.Ready)
                await ClockAsync();
        
            await ClockAsync();

            Simulation.Current.RequestStop(); 

        }
    }

   
    public class OutputSim : SimulationProcess{
        [InputBus]
        public IndexControl index;

        private SME.Components.SimpleDualPortMemory<double> ram;

        private double[] expected;
        public OutputSim( IndexControl index, SME.Components.SimpleDualPortMemory<double> ram, double[] expected){
            this.index = index;
            this.ram = ram;
            this.expected = expected;
        }

        public override async System.Threading.Tasks.Task Run(){
            await ClockAsync();
            while (!index.Ready)
                await ClockAsync();
        
            await ClockAsync();
            var match = true;
            for (int i =0; i<expected.Length; i++){
                match = match && Math.Abs(ram.m_memory[i] - expected[i]) < 0.0000001;
            }
            System.Diagnostics.Debug.Assert(match,"expected did not match result");
            Simulation.Current.RequestStop(); 

        }
    }




}
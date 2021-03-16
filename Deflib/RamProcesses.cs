    
using System.Threading.Tasks;
using SME;
using Deflib;

namespace Deflib{


    //Takes the data in and outputs as a flat array
    //input: index of the values
    //output: reads the data 
    public class Generate : SimpleProcess{
        [OutputBus]
        private SME.Components.SimpleDualPortMemory<double>.IReadControl output;

        [InputBus]
        private IndexValue index;

        public Generate(IndexValue value, SME.Components.SimpleDualPortMemory<double>.IReadControl output){

            index = value;
            this.output = output;

        }

        protected override void OnTick()
        {
            output.Enabled = index.Ready;
            if (index.Ready)
            {
                output.Address = index.Addr ;
            }
        }
    }


    [ClockedProcess]
    public class Pipe : SimpleProcess{
        
        [InputBus]
        private IndexValue v_input;

        [OutputBus]
        private IndexValue v_output;

          
        
        public Pipe (IndexValue v_input, IndexValue v_output){
            this.v_input = v_input;
            this.v_output =v_output;
        }

        protected override void OnTick()
        {
           v_output.Ready = v_input.Ready;
           if (v_input.Ready){
                v_output.Addr = v_input.Addr;    
           }
           
        }
                
    }

        public class ToRam : SimpleProcess{
        [InputBus]
        private ValueTransfer v_input;

        [InputBus]
        private IndexValue Address;
        
        [OutputBus]
        private SME.Components.SimpleDualPortMemory<double>.IWriteControl output;

        public ToRam(ValueTransfer v_input,  IndexValue Address, SME.Components.SimpleDualPortMemory<double>.IWriteControl output){
            this.v_input = v_input;
            this.Address = Address;
            this.output = output;
        }
        protected override void OnTick(){

            
            output.Enabled = Address.Ready;

            if (Address.Ready){
                output.Address = Address.Addr;
                output.Data = v_input.value; 
            }
        }


    }

     public class Value_Converter : SimpleProcess{
        [InputBus]
        private SME.Components.SimpleDualPortMemory<double>.IReadResult input;

        [InputBus]
        private IndexValue guard;

        [OutputBus]
        private ValueTransfer output;

        public Value_Converter(IndexValue guard, SME.Components.SimpleDualPortMemory<double>.IReadResult input, ValueTransfer output){
            this.guard = guard;
            this.input = input;
            this.output = output;
        }

        protected override void OnTick(){
            
            if (guard.Ready){
                output.value = input.Data;
            }
            
 
        }

    }







}



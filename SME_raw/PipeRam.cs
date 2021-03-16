// using System.Globalization;
// using System.IO;
// using System.Diagnostics.Contracts;
// using System.Linq;
// using System;
// using System.Threading.Tasks;
// using SME.Components;
// using SME;

// namespace GettingStarted
// {

//     public class Generate : SimpleProcess{
//         [OutputBus]
//         private SME.Components.SimpleDualPortMemory<double>.IReadControl output;

//         [InputBus]
//         private IndexValue index;

//         public Generate(IndexValue value, SME.Components.SimpleDualPortMemory<double>.IReadControl output){

//             index = value;
//             this.output = output;

//         }

//         protected override void OnTick()
//         {
//             output.Enabled = index.Ready;
//             if (index.Ready)
//             {
//                 output.Address = index.Addr ;
//             }
//         }
//     }




//     [ClockedProcess]
//     public class Pipe : SimpleProcess{
        
//         [InputBus]
//         private IndexValue v_input;

//         [OutputBus]
//         private IndexValue v_output;

          
        
//         public Pipe (IndexValue v_input, IndexValue v_output){
//             this.v_input = v_input;
//             this.v_output =v_output;
//         }

//         protected override void OnTick()
//         {
//            v_output.Ready = v_input.Ready;
//            if (v_input.Ready){
//                 v_output.Addr = v_input.Addr;    
//            }
           
//         }
                
//     }


//     public class Value_Converter : SimpleProcess{
//         [InputBus]
//         private SME.Components.SimpleDualPortMemory<double>.IReadResult input;

//         [InputBus]
//         private IndexValue guard;

//         [OutputBus]
//         private ValueTransfer output;

//         public Value_Converter(IndexValue guard, SME.Components.SimpleDualPortMemory<double>.IReadResult input, ValueTransfer output){
//             this.guard = guard;
//             this.input = input;
//             this.output = output;
//         }

//         protected override void OnTick(){
            
//             if (guard.Ready){
//                 output.value = input.Data;
//             }
            
 
//         }

//     }
//     public class ToRam : SimpleProcess{
//         [InputBus]
//         private ValueTransfer v_input;

//         [InputBus]
//         private IndexValue index;
        
//         [OutputBus]
//         private SME.Components.SimpleDualPortMemory<double>.IWriteControl output;

//         public ToRam(ValueTransfer v_input,  IndexValue index, SME.Components.SimpleDualPortMemory<double>.IWriteControl output){
//             this.v_input = v_input;
//             this.index = index;
//             this.output = output;
//         }
//         protected override void OnTick(){

            
//             output.Enabled = index.Ready;

//             if (index.Ready){
//                 output.Address = index.Addr;
//                 output.Data = v_input.value; 
//             }
//         }


//     }
 


//     public class Forward : SimpleProcess{

//         [InputBus]
        
//         private IndexValue old_input;

//         [InputBus]
//         private IndexValue new_input;
        
//         [InputBus]
//         private ValueTransfer v_inputNew;

//          [InputBus]
//         private SME.Components.SimpleDualPortMemory<double>.IReadResult v_inputOld;

//         [OutputBus]
//         private SME.Components.SimpleDualPortMemory<double>.IReadResult v_output;


//         public Forward(IndexValue old_input, IndexValue new_input, ValueTransfer v_inputNew, SME.Components.SimpleDualPortMemory<double>.IReadResult v_inputOld, SME.Components.SimpleDualPortMemory<double>.IReadResult v_output)
//         {
//             this.old_input = old_input;
//             this.new_input = new_input;
//             this.v_inputNew = v_inputNew;
//             this.v_inputOld = v_inputOld;
//             this.v_output = v_output;

//         }
//         protected override void OnTick(){
//             if(new_input.Ready && new_input.Addr == old_input.Addr){
//                 v_output.Data = v_inputNew.value;

//             }
//             else if(old_input.Ready){
//                 v_output.Data = v_inputOld.Data;
//             }
//             else{
//                 v_output.Data = 0;
//             }
            

//         }


//     }





// }
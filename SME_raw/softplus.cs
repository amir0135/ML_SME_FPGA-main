
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

   
//     [ClockedProcess]
//     public class Softplus: SimpleProcess {
//         [InputBus]
//         private SME.Components.SimpleDualPortMemory<double>.IReadResult m_input;

//         [InputBus]
//         private IndexValue index;
  
//         [OutputBus]
//         private ValueTransfer m_output;

//         private int threshold = 20;
//         public Softplus(SME.Components.SimpleDualPortMemory<double>.IReadResult input, IndexValue index,  ValueTransfer output)
//         {
//             m_input = input ?? throw new ArgumentNullException(nameof(input));
//             this.index = index ?? throw new ArgumentNullException(nameof(index));
//             m_output = output ?? throw new ArgumentNullException(nameof(output));
//         }

//         protected override void OnTick(){
    

//                 //if (input_pipe.Ready == true){
//             if (index.Ready == true){
//                 if (m_input.Data > threshold){
//                     m_output.value = m_input.Data;

//                 }
//                 else{
//                     m_output.value = Math.Log((1 + Math.Exp(m_input.Data)));

//                 }        
                    
                    
//             }
//         }  
//     }
// }
   



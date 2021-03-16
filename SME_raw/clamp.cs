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
//     public class Clamp: SimpleProcess {
//         [InputBus]
//         private SME.Components.SimpleDualPortMemory<double>.IReadResult m_input;

//         [InputBus]
//         private IndexValue index;

  
//         [OutputBus]
//         private ValueTransfer m_output;

//         double min_val, max_val;


//         public Clamp(SME.Components.SimpleDualPortMemory<double>.IReadResult input, IndexValue index,  ValueTransfer output, double min_val, double max_val)
//         {
//             m_input = input ?? throw new ArgumentNullException(nameof(input));
//             this.index = index ?? throw new ArgumentNullException(nameof(index));
//             m_output = output ?? throw new ArgumentNullException(nameof(output));
//             this.min_val = min_val;
//             this.max_val = max_val ;
//         }

//         protected override void OnTick(){

//                         if (index.Ready == true){
//                 if (m_input.Data < min_val){
//                         m_output.value = min_val;
//                     }
//                 else if (m_input.Data > max_val){
//                         m_output.value = max_val;
//                 }
//                 else{
//                     m_output.value = m_input.Data;
//                 }    
//             }
//         }  
//     }
// }    
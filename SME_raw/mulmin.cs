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
//     public class MulMin: SimpleProcess {
//         [InputBus]
//         private SME.Components.SimpleDualPortMemory<double>.IReadResult m_input;

//         [InputBus]
//         private IndexValue index;
  
//         [OutputBus]
//         private ValueTransfer m_output;


//         public MulMin(SME.Components.SimpleDualPortMemory<double>.IReadResult input, IndexValue index,  ValueTransfer output)
//         {
//             m_input = input ?? throw new ArgumentNullException(nameof(input));
//             this.index = index ?? throw new ArgumentNullException(nameof(index));
//             m_output = output ?? throw new ArgumentNullException(nameof(output));
//         }

//         protected override void OnTick(){

//             if (index.Ready){
//                 m_output.value = 2* m_input.Data - 1;

//             }
//         }  
//     }
// }    
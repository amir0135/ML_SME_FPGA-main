
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
//     public class Mean: SimpleProcess {
//         [InputBus]
//         private SME.Components.SimpleDualPortMemory<double>.IReadResult m_input;

//         [InputBus]
//         private IndexValue index;
  
//         [OutputBus]
//         private ValueTransfer m_output;

//         private double accumulated;
//         private int count;
//         private int ind = -1;


//         public Mean(SME.Components.SimpleDualPortMemory<double>.IReadResult input, IndexValue index,  ValueTransfer output)
//         {
//             m_input = input ?? throw new ArgumentNullException(nameof(input));
//             this.index = index ?? throw new ArgumentNullException(nameof(index));
//             m_output = output ?? throw new ArgumentNullException(nameof(output));
//         }

//         protected override void OnTick(){
    

//             if (index.Ready == true){
//                 if (index.Addr == ind){
//                     accumulated += m_input.Data;
//                     count += 1;
//                     m_output.value = accumulated /count;

//                 }
//                 else{

//                     m_output.value = accumulated /count;
//                     accumulated = m_input.Data;
//                     count = 1;
//                     ind = index.Addr;

//                 }


                    

                    
//             }
//         }  
//     }
// }

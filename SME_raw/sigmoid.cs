
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
//     public class Sigmoid: SimpleProcess {
//         [InputBus]
//         private SME.Components.SimpleDualPortMemory<double>.IReadResult m_input;

//         [InputBus]
//         private IndexValue index;
  
//         [OutputBus]
//         private ValueTransfer m_output;


//         public Sigmoid(SME.Components.SimpleDualPortMemory<double>.IReadResult input, IndexValue index,  ValueTransfer output)
//         {
//             m_input = input ?? throw new ArgumentNullException(nameof(input));
//             this.index = index ?? throw new ArgumentNullException(nameof(index));
//             m_output = output ?? throw new ArgumentNullException(nameof(output));
//         }

//         protected override void OnTick(){
    

//             if (index.Ready == true){
//                 m_output.value = 1 /(1 + Math.Exp(-(m_input.Data)));
                    
//             }
//         }  
//     }
   


//     public class SigIndex : SimpleProcess
//     {
  
//         [InputBus]
//         private IndexControl controlA;

//         [OutputBus]
//         private IndexValue output;
//         [OutputBus]

//         private IndexControl controlout;
//         [InputBus]
//         private IndexControl control;

//         private bool running = false;
//         private int i = 0;
//         private int Addr;
//         private int width, height;
//         private bool Aready = false;
//         private bool started = false;


//         public SigIndex(IndexControl controlA, IndexValue output, IndexControl controlout, IndexControl control)
//         {   
//             this.controlA = controlA;
//             this.controlout = controlout;
//             this.output = output;
//             this.control = control;
//         }

//         protected override void OnTick() 
//         {
//         if (running == true) 
//             {   
//                 started = true;
//                 output.Ready = true;
                
//                 output.Addr =  i ;


//                 i++;

//                 if (i >= width)
//                 {
//                     running = false;
//                 }
//             } 
        
//             else 
//             {
//                 Aready |= controlA.Ready;

//                 if (Aready)
//                 {
//                     Aready = false;

//                     running = true;
//                     width = control.Width;
//                     height = control.Height;

//                     i = 0;
//                     output.Ready = true;
//                     output.Addr = controlA.OffsetA;
//                     started = true;
//                 }
                

//                else {
//                     if (started == true){
                        
//                         controlout.Ready = true;
//                         controlout.Height = control.Height;
//                         controlout.Width = control.Width;
//                         controlout.OffsetA = controlA.OffsetA;
//                         controlout.OffsetB = controlA.OffsetB;
//                         started = false;
//                     }
//                     else{
//                         controlout.Ready = false;
//                     }
            
//                     output.Ready = false;
//                 }
//             }
//         }         
//     }
// }    


// using System.Globalization;
// using System.IO;
// using System.Diagnostics.Contracts;
// using System.Linq;
// using System;
// using System.Threading.Tasks;
// using SME;

// namespace GettingStarted
// {

//  [ClockedProcess]
//     public class Mul : SimpleProcess
//     {
//         [InputBus]
//         private SME.Components.SimpleDualPortMemory<double>.IReadResult m_inputA;


//         [InputBus]
//         private SME.Components.SimpleDualPortMemory<double>.IReadResult m_inputB;


        
//         [InputBus]
//         private IndexValue input_pipe;

//         [OutputBus]
//         private ValueTransfer v_output;

     
//         public Mul(IndexValue inputpipe, SME.Components.SimpleDualPortMemory<double>.IReadResult inputA, SME.Components.SimpleDualPortMemory<double>.IReadResult inputB,  ValueTransfer output)
//         {

//             input_pipe = inputpipe ?? throw new ArgumentNullException(nameof(inputpipe));
//             m_inputA = inputA ?? throw new ArgumentNullException(nameof(inputA));
//             m_inputB = inputB ?? throw new ArgumentNullException(nameof(inputB));
//             v_output = output ?? throw new ArgumentNullException(nameof(output));

//         }

//         protected override void OnTick(){
        
//             if (input_pipe.Ready == true){

//                     v_output.value =  m_inputA.Data * m_inputB.Data;

//             }
//             else{
//                 v_output.value = 0;
//             }

//         }

//     }
//     public class MulIndex : SimpleProcess
//     {
  
//         [InputBus]
//         private IndexControl controlA;

//         [InputBus]
//         private IndexControl controlB;

//         [OutputBus]
//         private IndexValue outputA;
        
//         [OutputBus]
//         private IndexValue outputB;
        
//         [OutputBus]

//         private IndexControl controlout;
//         [InputBus]
//         private IndexControl control;

//         private bool running = false;
//         private int i, j, k = 0;
//         private int Addr;
//         private int width, height;
//         private bool Aready = false, Bready =  false;
//         private bool started = false;


//         public MulIndex(IndexControl controlA, IndexControl controlB, IndexValue outputA, IndexValue outputB, IndexControl controlout, IndexControl control)
//         {   
//             this.controlA = controlA;
//             this.controlB = controlB;
//             this.controlout = controlout;
//             this.outputA = outputA;
//             this.outputB = outputB;
//             this.control = control;
//         }

//         protected override void OnTick() 
//         {
//             if (running == true) 
//             {   
//                 outputA.Ready = true;
//                 outputB.Ready = true;
//                 started = true;

                
//                 outputA.Addr = i*width*height + j;
//                 outputB.Addr  = i*width*height + j;

//                 j++;

//                 if (j >= width)
//                 {
//                     j = 0;
//                     i ++;
//                 }

//                 if (i >= height)
//                 {
//                     running = false;
//                 }
//             } 
//             else 
//             {
//                 Aready |= controlA.Ready;
//                 Bready |= controlB.Ready;

//                 if (Aready && Bready)
//                 {
//                     Aready = false;
//                     Bready = false;

//                     running = true;
//                     width = control.Width;
//                     height = control.Height;

//                     i = j = 0;
//                     outputA.Ready = true;
//                     outputA.Addr = controlA.OffsetA;
//                     outputB.Ready = true;
//                     outputB.Addr = controlB.OffsetB;

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
            
//                     outputA.Ready = false;
//                     outputB.Ready = false;
//                 }
//             }
//         }         
//     }
// }    
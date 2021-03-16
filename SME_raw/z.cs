// using System.Globalization;
// using System.IO;
// using System.Diagnostics.Contracts;
// using System.Linq;
// using System;
// using System.Threading.Tasks;
// using SME;

// namespace GettingStarted
// {

//     public class ZIndex : SimpleProcess
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
//         private int width, height, dim;
//         private bool Aready = false, Bready =  false;
//         private bool started = false;


//         public ZIndex(IndexControl controlA, IndexControl controlB, IndexValue outputA, IndexValue outputB, IndexControl controlout, IndexControl control)
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

                
//                 outputA.Addr =  i *width*height + j*width + k ;
//                 outputB.Addr =  i *width*height + j*width + k ;
                

//                 k++;

//                 if (k >= width)
//                 {
//                     k = 0;
//                     j++;
//                 }

//                 if (j >= height)
//                 {
//                     j = 0;
//                     i ++;
//                 }

//                 if (i >= dim)
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
//                     dim = control.Dim;

//                     i = j = k = 0;

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
//                         controlout.Dim = control.Dim;
//                         controlout.OffsetA = controlA.OffsetA;
//                         controlout.OffsetB = controlA.OffsetB;
//                         controlout.OffsetC = controlA.OffsetC;   
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

// // using System.Globalization;
// // using System.IO;
// // using System.Diagnostics.Contracts;
// // using System.Linq;
// // using System;
// // using System.Threading.Tasks;
// // using SME;

// // namespace GettingStarted
// // {

// //  [ClockedProcess]
// //     public class Z : SimpleProcess
// //     {
// //         [InputBus]
// //         private SME.Components.SimpleDualPortMemory<double>.IReadResult m_inputA;


// //         [InputBus]
// //         private SME.Components.SimpleDualPortMemory<double>.IReadResult m_inputB;
        
// //         [InputBus]
// //         private SME.Components.SimpleDualPortMemory<double>.IReadResult m_inputC;

        
// //         [InputBus]
// //         private IndexValue input_pipe;

// //         [OutputBus]
// //         private ValueTransfer v_output;

     
// //         public Z(IndexValue inputpipe, SME.Components.SimpleDualPortMemory<double>.IReadResult inputA, SME.Components.SimpleDualPortMemory<double>.IReadResult inputB, SME.Components.SimpleDualPortMemory<double>.IReadResult inputC,   ValueTransfer output)
// //         {

// //             input_pipe = inputpipe ?? throw new ArgumentNullException(nameof(inputpipe));
// //             m_inputA = inputA ?? throw new ArgumentNullException(nameof(inputA));
// //             m_inputB = inputB ?? throw new ArgumentNullException(nameof(inputB));
// //             m_inputC = inputC ?? throw new ArgumentNullException(nameof(inputC));
// //             v_output = output ?? throw new ArgumentNullException(nameof(output));

// //         }

// //         protected override void OnTick(){
        
// //         if (input_pipe.Ready == true){

// //                //Console.WriteLine($"C{m_inputC.Data} += A{m_inputA.Data} * B{m_inputB.Data} ");
                
// //                 v_output.value = m_inputC.Data + (m_inputA.Data * m_inputB.Data);
                

// //             }
// //             else{
// //                 v_output.value = 0;
// //             }

// //         }

// //     }
// //     public class ZIndex : SimpleProcess
// //     {
  
// //         [InputBus]
// //         private IndexControl controlA;

// //         [InputBus]
// //         private IndexControl controlB;

// //         [OutputBus]
// //         private IndexValue output;
// //         [OutputBus]

// //         private IndexControl controlout;

// //         [InputBus]
// //         private IndexControl control;

// //         private bool running = false;
// //         private int i, j, k = 0;
// //         private int AddrA, AddrB, AddrC;
// //         private int width, height, dim;
// //         private bool Aready = false, Bready =  false, controlready = false;
// //         private bool started = false;


// //         public 
        
        
        
// //         ZIndex(IndexControl controlA, IndexControl controlB, IndexValue output, IndexControl controlout, IndexControl control)
       
// //         {   
// //             this.controlA = controlA;
// //             this.controlB = controlB;
// //             this.controlout = controlout;
// //             this.output = output;
// //             this.control = control;
// //         }

// //         protected override void OnTick() 
// //         {
// //             if (running == true) 
// //             {   
// //                 started = true;

                
// //                 output.AddrA =  i *width*height + j*width + k ;
// //                 output.AddrB = i *width + j ; // ???????
// //                 output.AddrC = i *width + j ;

// //                 k++;

// //                 if (k >= width)
// //                 {
// //                     k = 0;
// //                     j++;
// //                 }

// //                 if (j >= height)
// //                 {
// //                     j = 0;
// //                     i ++;
// //                 }

// //                 if (i >= dim)
// //                 {
// //                     running = false;
// //                 }
// //             } 
// //             else 
// //             {
// //                 Aready |= controlA.Ready;
// //                 Bready |= controlB.Ready;
// //                 controlready |=control.Ready;

// //                 if (Aready && Bready && controlready)
// //                 {
// //                     Aready = false;
// //                     Bready = false;
// //                     controlready = false;

// //                     running = true;
// //                     width = control.Width;
// //                     height = control.Height;
// //                     dim = control.Dim;

// //                     i = j = k = 0;
// //                     AddrA = controlA.OffsetA;
// //                     AddrB = controlB.OffsetB;
// //                     AddrC = controlA.OffsetC; 

// //                     output.Ready = true;
// //                     output.AddrA = AddrA;
// //                     output.AddrB = AddrB;
// //                     output.AddrC = AddrC;
// //                     started = true;
// //                 }
                

// //                else {
// //                     if (started == true){
                        
// //                         controlout.Ready = true;
// //                         controlout.Height = control.Height;
// //                         controlout.Width = control.Height;
// //                         controlout.Dim = control.Dim;
// //                         controlout.OffsetA = controlA.OffsetA;
// //                         controlout.OffsetB = controlA.OffsetB;
// //                         controlout.OffsetC = controlA.OffsetC;   
// //                         started = false;
// //                     }
// //                     else{
// //                         controlout.Ready = false;
// //                     }
            
// //                     output.Ready = false;
// //                 }
// //             }
// //         }         
// //     }
 
// // }    
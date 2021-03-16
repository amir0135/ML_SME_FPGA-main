
// using System.Globalization;
// using System.IO;
// using System.Diagnostics.Contracts;
// using System.Linq;
// using System;
// using System.Threading.Tasks;
// using SME;

// namespace GettingStarted
// {


//     [ClockedProcess]
//     public class MatMul : SimpleProcess
//     {
//         [InputBus]
//         private SME.Components.SimpleDualPortMemory<double>.IReadResult m_inputA;


//         [InputBus]
//         private SME.Components.SimpleDualPortMemory<double>.IReadResult m_inputB;
        
//         [InputBus]
//         private SME.Components.SimpleDualPortMemory<double>.IReadResult m_inputC;

        
//         [InputBus]
//         private IndexValue input_pipe;

//         [OutputBus]
//         private ValueTransfer v_output;

     
//         public MatMul(IndexValue inputpipe, SME.Components.SimpleDualPortMemory<double>.IReadResult inputA, SME.Components.SimpleDualPortMemory<double>.IReadResult inputB, SME.Components.SimpleDualPortMemory<double>.IReadResult inputC,   ValueTransfer output)
//         {

//             input_pipe = inputpipe ?? throw new ArgumentNullException(nameof(inputpipe));
//             m_inputA = inputA ?? throw new ArgumentNullException(nameof(inputA));
//             m_inputB = inputB ?? throw new ArgumentNullException(nameof(inputB));
//             m_inputC = inputC ?? throw new ArgumentNullException(nameof(inputC));
//             v_output = output ?? throw new ArgumentNullException(nameof(output));

//         }

//         protected override void OnTick(){
        
//             if (input_pipe.Ready == true){

//                //Console.WriteLine($"C{m_inputC.Data} += A{m_inputA.Data} * B{m_inputB.Data} ");

//                 v_output.value = m_inputC.Data + (m_inputA.Data * m_inputB.Data);
                

//             }
//             else{
//                 v_output.value = 0;
//             }

//         }

//     }

//     [ClockedProcess]
// public class MatMulIndex : SimpleProcess
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
//         private IndexValue outputC;

//         [OutputBus]
//         private IndexControl controlout;

//         private bool running = false;
//         private int i, j, k = 0;
//         private int Addr;
//         private int widthA, heightA, widthB, heightB;
//         private bool Aready = false, Bready =  false;
//         private bool started = false;

//         public MatMulIndex(IndexControl controlA,IndexControl controlB, IndexValue outputA,IndexValue outputB,IndexValue outputC , IndexControl controlout)
//         {
//             this.controlA = controlA;
//             this.controlB = controlB;
//             this.controlout = controlout;
//             this.outputA = outputA;
//             this.outputB = outputB;
//             this.outputC = outputC;
//         }

//         protected override void OnTick() 
//         {
//             if (running == true) 
//             {   
//                 outputA.Ready = true;
//                 outputB.Ready = true;
//                 outputC.Ready = true;
//                 started = true;
                

//                // Console.WriteLine($"i{i} j{j} k{k}");
//                 outputA.Addr = i * widthA + k;
//                 outputB.Addr = k * widthB + j;
//                 outputC.Addr = i * widthA + j;

//                 k++;

//                 if (k >= widthA)
//                 {
//                     k = 0;
//                     j++;
//                 }

//                 if (j >= widthB)
//                 {
//                     j = 0;
//                     i ++;
//                 }

//                 if (i >= heightA)
//                 {
//                     running = false;
//                 }
//             } 
//             else 
//             {
//                 Aready |= controlA.Ready;
//                 Bready |= controlB.Ready;

//                 if (Aready && Bready){
//                     Aready = false;
//                     Bready = false;

//                     running = true;
//                     widthA = controlA.Width;
//                     heightA = controlA.Height;
//                     widthB = controlB.Width;
//                     heightB = controlB.Height;

//                     i = j = k = 0;
                    
//                     outputA.Addr = controlA.OffsetA;
//                     outputB.Addr = controlB.OffsetB;
//                     outputC.Addr = controlA.OffsetC; 
//                     started = true;

//                 }
//                 else {
//                     if (started == true){
//                         controlout.Ready = true;
//                         controlout.Height = controlA.Height;
//                         controlout.Width = controlB.Width;
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
//                     outputC.Ready = false;
//                 }
//             }
//         }         
//     }
 
// }    
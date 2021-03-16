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



//     public class Index : SimpleProcess{
//         [InputBus]
//         private IndexControl control;
//         [OutputBus]
//         private IndexValue output;
//         [OutputBus]
//         private IndexControl controlout;

//         private bool running = false;
//         private bool started = false;
//         private int i = 0;
//         private int Addr;
//         private int width;
//         private int height;
//         private int dim;

//         public Index(IndexControl control, IndexValue output, IndexControl controlout)
//         {
//             this.control = control;
//             this.output = output;
//             this.controlout = controlout;
//         }

//         protected override void OnTick() 
//         {
//             if (running == true) 
//             {   
//                 started = true;

//                 //Console.WriteLine($"i{i}");
//                 i++;
//                 output.Addr = i ;
                

//                 if (i >= width*height*dim)
//                 {
//                     running = false;
//                     output.Ready = false;
//                 }
//             } 
//             else 
//             {
//                 if (control.Ready == true)
//                 {
//                     started = true;
//                     running = true;
//                     width = control.Width;
//                     height = control.Height;
//                     dim = control.Dim;
//                     i = 0;
//                     Addr = control.OffsetA;

//                     output.Ready = true;
//                     output.Addr = Addr;

//                 }
//                 else 
//                 {
//                     if (started == true){
//                         controlout.Ready = true;
//                         controlout.Height = control.Height;
//                         controlout.Width = control.Width;
//                         controlout.Dim = control.Dim;
//                         controlout.OffsetA = control.OffsetA;
//                         controlout.OffsetB = control.OffsetB;
//                         controlout.OffsetC = control.OffsetC;   
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



//      public class TestIndexSim : SimulationProcess{
//         [OutputBus]
//         private IndexControl control;

//         int Row, Col, Dim;

//         public SME.Components.SimpleDualPortMemory<double> ram;

//         public TestIndexSim(IndexControl control, int Row)
//         {
//             this.control = control;
//             this.Row = Row;
//         }


//         public TestIndexSim(IndexControl control, int Row, int Col)
//         {
//             this.control = control;
//              this.Row = Row;
//             this.Col = Col;
//             this.Dim = 1;
//         }

//         public TestIndexSim(IndexControl control, int Row, int Col, int Dim)
//         {
//             this.control = control;
//             this.Row = Row;
//             this.Col = Col;
//             this.Dim = Dim;
//         }


//         public override async Task Run()
//         {
//             control.OffsetA = 0;
//             control.OffsetB = 0;
//             control.OffsetC = 0;

//             await ClockAsync();

//             control.Ready = true;
            
//             control.Height = Row;
//             control.Width = Col;
//             control.Dim = Dim ;

//             await ClockAsync();

//             control.Ready = false;

            

             

//         }
//     }



//     public class OutputSim : SimulationProcess{
//         [InputBus]
//         private IndexControl index;

//         private SME.Components.SimpleDualPortMemory<double> ram;
//         public OutputSim( IndexControl index, SME.Components.SimpleDualPortMemory<double> ram){
//             this.index = index;
//             this.ram = ram;
//         }

//         public override async System.Threading.Tasks.Task Run(){
//             await ClockAsync();
//             while (!index.Ready)
//                 await ClockAsync();
        
//             await ClockAsync();
//             Simulation.Current.RequestStop(); 
            

            
             
            

//         }
        

//     }


  
// }
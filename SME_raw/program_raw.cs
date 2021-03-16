// using System;
// using SME;
// using System.Linq;

// namespace GettingStarted
// {
//         public static class Constants {

//             public static int input_size =3;
//             public static int hidden_size =3;
//             public static int num_networks =3;
//             public static int max_predict =1;

//             public static int Batchsize = 1;
            
//         } 

//         public class LoadStage{

//             public SME.Components.SimpleDualPortMemory<double> output;
//             public IndexControl control;
            

//             public LoadStage(int size, string CSVfile){


//                 var load_control = Scope.CreateBus<IndexControl>();
//                 var load_index = Scope.CreateBus<IndexValue>();
//                 control = Scope.CreateBus<IndexControl>();

                
//                 output = new SME.Components.SimpleDualPortMemory<double>(size);

//                 var load_sim = new TestIndexSim(load_control, Constants.num_networks, Constants.hidden_size);
//                 var generate_load = new Dataload(size, CSVfile, load_index, output.WriteControl);
//                 var load_ind = new Index(load_control, load_index, control);


//             }


//         }
//     class MainClass{
//             public static void Main(string[] args)
//         {
//             using (var sim = new Simulation())
//             {

//                 var Wr_data = new LoadStage(Constants.num_networks*Constants.hidden_size, "Wr.csv");

// // ////// Wr data //////////
//             //     var Wr_control = Scope.CreateBus<IndexControl>();
//             //     var Wr_index = Scope.CreateBus<IndexValue>();
//             //     var control_Wr = Scope.CreateBus<IndexControl>();

                
//             //     var array_Wr = new SME.Components.SimpleDualPortMemory<double>( Constants.num_networks*Constants.hidden_size);

//             //     var Wr_sim = new TestIndexSim(Wr_control, Constants.num_networks, Constants.hidden_size);
//             //     var generate_Wr = new Data_Wr(Wr_index, array_Wr.WriteControl);
//             //     var Wr_ind = new Index(Wr_control, Wr_index, control_Wr);
                
//             //     Wr_sim.ram = array_Wr;  
//             //    // var outsimtra = new OutputSim(control_Wr, array_Wr );

// // ////// Wz data //////////       
//                 var Wz_control = Scope.CreateBus<IndexControl>();
//                 var Wz_index = Scope.CreateBus<IndexValue>();
//                 var control_Wz = Scope.CreateBus<IndexControl>();

                
//                 var array_Wz = new SME.Components.SimpleDualPortMemory<double>( Constants.num_networks*Constants.hidden_size);

//                 var Wz_sim = new TestIndexSim(Wz_control, Constants.num_networks, Constants.hidden_size, 1);
//                 var generate_Wz = new Data_Wz(Wz_index, array_Wz.WriteControl);
//                 var Wz_ind = new Index(Wz_control, Wz_index, control_Wz);


//                 Wz_sim.ram = array_Wz;  
//                 //var outsimtra = new OutputSim(control_Wz, array_Wz );

// // ////// prelu z data //////////  
//                 var prelu_z_control = Scope.CreateBus<IndexControl>();
//                 var prelu_z_index = Scope.CreateBus<IndexValue>();
//                 var control_prelu_z = Scope.CreateBus<IndexControl>();

                
//                 var array_prelu_z = new SME.Components.SimpleDualPortMemory<double>(Constants.num_networks);

//                 var prelu_z_sim = new TestIndexSim(prelu_z_control, Constants.num_networks);
//                 var generate_prelu_z = new Data_Prelu_z(prelu_z_index, array_prelu_z.WriteControl);
//                 var prelu_z_ind = new Index(prelu_z_control, prelu_z_index, control_prelu_z);
                
//                 prelu_z_sim.ram = array_prelu_z;  
//                // var outsimtra = new OutputSim(control_prelu_z, array_prelu_z );


// // //////  prelu r data //////////  

//                 var prelu_r_control = Scope.CreateBus<IndexControl>();
//                 var prelu_r_index = Scope.CreateBus<IndexValue>();
//                 var control_prelu_r = Scope.CreateBus<IndexControl>();

                
//                 var array_prelu_r = new SME.Components.SimpleDualPortMemory<double>( Constants.num_networks);

//                 var prelu_r_sim = new TestIndexSim(prelu_r_control, Constants.num_networks);
//                 var generate_prelu_r = new Data_Prelu_r(prelu_r_index, array_prelu_r.WriteControl);
//                 var prelu_r_ind = new Index(prelu_r_control, prelu_r_index, control_prelu_r);
                
//                 prelu_r_sim.ram = array_prelu_r;  
//                // var outsimtra = new OutputSim(control_prelu_r, array_prelu_r );

// // //////  z_scale data //////////  

//                 var zscale_control = Scope.CreateBus<IndexControl>();
//                 var zscale_index = Scope.CreateBus<IndexValue>();
//                 var control_zscale = Scope.CreateBus<IndexControl>();

                
//                 var array_zscale = new SME.Components.SimpleDualPortMemory<double>( Constants.num_networks);

//                 var zscale_sim = new TestIndexSim(zscale_control, 1,Constants.num_networks );
//                 var generate_zscale = new Data_z_scale(zscale_index, array_zscale.WriteControl);
//                 var zscale_ind = new Index(zscale_control, zscale_index, control_zscale);
                
//                 zscale_sim.ram = array_zscale;  
               
//                 //var outsimtra = new OutputSim(control_zscale, array_zscale );



// // ////// x data //////////       
//                 var x_control = Scope.CreateBus<IndexControl>();
//                 var x_index = Scope.CreateBus<IndexValue>();
//                 var control_x = Scope.CreateBus<IndexControl>();

                
//                 var array_x = new SME.Components.SimpleDualPortMemory<double>( Constants.Batchsize*Constants.input_size);

//                 var x_sim = new TestIndexSim(x_control, Constants.Batchsize, Constants.input_size);
//                 var generate_x = new Data_x(x_index, array_x.WriteControl);
//                 var x_ind = new Index(x_control, x_index, control_x);
                
//                 x_sim.ram = array_x;  
//                 //var outsimtra = new OutputSim(control_x, array_x );


// ////// Test data W0 ///////////////
//                 var W0_control = Scope.CreateBus<IndexControl>();
//                 var W0_index = Scope.CreateBus<IndexValue>();
//                 var control_W0 = Scope.CreateBus<IndexControl>();

                
//                 var array_W0 = new SME.Components.SimpleDualPortMemory<double>(Constants.num_networks* Constants.hidden_size* Constants.input_size);

//                 var W0_sim = new TestIndexSim(W0_control, Constants.input_size, Constants.num_networks* Constants.hidden_size);
//                 var generate_W0 = new Data_W0(W0_index, array_W0.WriteControl);
//                 var W0_ind = new Index(W0_control, W0_index, control_W0);
                
//                 W0_sim.ram = array_W0;  

//                //var outsimtra = new OutputSim(control_W0, array_W0  );

// // ////// Transpose W0 //////////        

//                 var transposeindex_W0 = Scope.CreateBus<IndexValue>();
//                 var pipeouttra_W0 = Scope.CreateBus<IndexValue>();
//                 var control_tra_W0 = Scope.CreateBus<IndexControl>();

//                 var arrayTra_W0 = new SME.Components.SimpleDualPortMemory<double>(Constants.num_networks* Constants.hidden_size* Constants.input_size);
                
//                 var generate_tra_W0 = new GenerateA(transposeindex_W0, array_W0.ReadControl);
//                 var transposeind_tra_W0 = new Transpose(control_W0, transposeindex_W0, control_tra_W0);
//                 var pipe_tra_W0 = new Pipe(transposeindex_W0, pipeouttra_W0);
//                 var toram_tra_W0 = new ToRamB(array_W0.ReadResult, pipeouttra_W0, arrayTra_W0.WriteControl);
                
//                 //var outsimtra = new OutputSim(control_tra_W0, arrayTra_W0 );
                

// /////////// ////// matmul x and tra W0  ///////////////
                
//                 var matmulindex = Scope.CreateBus<IndexValue>();
//                 var pipeoutmat = Scope.CreateBus<IndexValue>();
//                 var pipe1outmat = Scope.CreateBus<IndexValue>();
//                 var forwarded = Scope.CreateBus<SME.Components.SimpleDualPortMemory<double>.IReadResult>();
//                 var matmulresult = Scope.CreateBus<ValueTransfer>();
//                 var matmulresultcontrol = Scope.CreateBus<IndexControl>();
//                 //pipecontrolin = scope
//                 //pipecontrolout = scope


//                 var arraymatmul = new SME.Components.SimpleDualPortMemory<double>(Constants.num_networks* Constants.hidden_size);
//                 var arraymatmulresult = new SME.Components.SimpleDualPortMemory<double>(Constants.num_networks* Constants.hidden_size);

//                 var generatemat_x = new GenerateA(matmulindex, array_x.ReadControl);
//                 var generatemat_W0 = new GenerateB(matmulindex, arrayTra_W0.ReadControl);
//                 var generatematmul = new GenerateC(matmulindex, arraymatmul.ReadControl);
                
//                 var matmulind = new MatMulIndex(control_x, control_tra_W0, matmulindex, matmulresultcontrol);
//                 var pipematmul = new Pipe(matmulindex, pipeoutmat);
//                 var forward = new Forward(pipeoutmat,pipe1outmat, matmulresult, arraymatmul.ReadResult, forwarded);
//                 var matmul = new MatMul(pipeoutmat, array_x.ReadResult, arrayTra_W0.ReadResult, forwarded , matmulresult);
//                 var pipe1 = new Pipe(pipeoutmat, pipe1outmat);
//                 //var pipecontrol = new pipe(hz_result, pipecontrolin)
//                 //pipecontrol1 = new pipe(pipecontrolin, pipecontrolout)
//                 var toram = new ToRamC(matmulresult, pipe1outmat, arraymatmul.WriteControl);
//                 var toramresult = new ToRamC(matmulresult, pipe1outmat, arraymatmulresult.WriteControl);

               
//                 //var outsimtra = new OutputSim(matmulresultcontrol, arraymatmul );
//                 //var outsimtra = new OutputSim(pipecontrolout, arraymatmul);



// // ///////////// hz /////////////////////////                
//                 var hz_index = Scope.CreateBus<IndexValue>();
//                 var pipeout_hz = Scope.CreateBus<IndexValue>();
//                 var pipe1out_hz = Scope.CreateBus<IndexValue>();
//                 var hz_result = Scope.CreateBus<ValueTransfer>();
//                 var hz_resultcontrol = Scope.CreateBus<IndexControl>();
//                 var control_3D_hz = Scope.CreateBus<IndexControl>();


//                 var array_hz = new SME.Components.SimpleDualPortMemory<double>(Constants.input_size*Constants.hidden_size);
//                 var array_hz_result = new SME.Components.SimpleDualPortMemory<double>(Constants.input_size*Constants.hidden_size);

//                 var generateHzA = new GenerateA(hz_index, arraymatmulresult.ReadControl);
//                 var generateHzB = new GenerateB(hz_index, array_prelu_z.ReadControl);


//                 var dim_control_hz = new TestIndexSim(control_3D_hz,3,3, 1);
//                 var hz_ind = new HzIndex(matmulresultcontrol, control_prelu_z , hz_index, hz_resultcontrol, control_3D_hz);
//                 var pipe_hz = new Pipe(hz_index, pipeout_hz);
//                 var hz = new Hz(pipeout_hz, arraymatmulresult.ReadResult, array_prelu_z.ReadResult, hz_result);
//                 var pipe_hz_1 = new Pipe(pipeout_hz, pipe1out_hz);

//                 var toram_hz = new ToRam_hz(hz_result, pipe1out_hz, array_hz.WriteControl);
//                 var toram_hz_result = new ToRam_hz(hz_result, pipe1out_hz, array_hz_result.WriteControl);
                
//               //  var outsimtra = new OutputSim(hz_resultcontrol, array_hz );
         

// // // ///////////// hr /////////////////////////      
//                 var hr_index = Scope.CreateBus<IndexValue>();
//                 var pipeout_hr = Scope.CreateBus<IndexValue>();
//                 var pipe1out_hr = Scope.CreateBus<IndexValue>();
//                 var hr_result = Scope.CreateBus<ValueTransfer>();
//                 var hr_resultcontrol = Scope.CreateBus<IndexControl>();
//                 var control_3D_hr = Scope.CreateBus<IndexControl>();


//                 var array_hr = new SME.Components.SimpleDualPortMemory<double>(Constants.input_size*Constants.hidden_size);
//                 var array_hr_result = new SME.Components.SimpleDualPortMemory<double>(Constants.input_size*Constants.hidden_size);

//                 var generate_hr_A = new GenerateA(hr_index, arraymatmulresult.ReadControl);
//                 var generate_hr_B = new GenerateB(hr_index, array_prelu_r.ReadControl);
                

//                 var dim_control_hr = new TestIndexSim(control_3D_hr,3,3,1);
//                 var hr_ind = new HzIndex(matmulresultcontrol, control_prelu_r , hr_index, hr_resultcontrol, control_3D_hr);
//                 var pipe_hr = new Pipe(hr_index, pipeout_hr);
//                 var hr = new Hz(pipeout_hr, arraymatmulresult.ReadResult, array_prelu_r.ReadResult, hr_result);
//                 var pipe_hr_1 = new Pipe(pipeout_hr, pipe1out_hr);

//                 var toram_hr = new ToRam_hr(hr_result, pipe1out_hr, array_hr.WriteControl);
//                 var toram_hr_result = new ToRam_hr(hr_result, pipe1out_hr, array_hr_result.WriteControl);
                
//                 //var outsimtra = new OutputSim(hr_resultcontrol, array_hr );


// // // ///////////// hz * wz /////////////////////////     

//                 var z_index = Scope.CreateBus<IndexValue>();
//                 var pipeout_z = Scope.CreateBus<IndexValue>();
//                 var pipe1out_z = Scope.CreateBus<IndexValue>();
//                 var z_result = Scope.CreateBus<ValueTransfer>();
//                 var z_resultcontrol = Scope.CreateBus<IndexControl>();
//                 var control_z = Scope.CreateBus<IndexControl>();


//                 var array_z = new SME.Components.SimpleDualPortMemory<double>(Constants.num_networks *Constants.hidden_size);

//                 var generate_z_A = new GenerateA(z_index, array_hz_result.ReadControl);
//                 var generate_z_B = new GenerateB(z_index, array_Wz.ReadControl);
                

//                 var dim_control_z = new TestIndexSim(control_z,3,3);
//                 var z_ind = new ZIndex(hz_resultcontrol, control_Wz, z_index, z_resultcontrol, control_z);
//                 var pipe_z = new Pipe(z_index, pipeout_z);
//                 var z = new Mul(pipeout_z, array_hz_result.ReadResult, array_Wz.ReadResult, z_result);
//                 var pipe_z_1 = new Pipe(pipeout_z, pipe1out_z);

//                 var toram_z = new ToRam_z(z_result, pipe1out_z, array_z.WriteControl);

//                 //var outsimtra = new OutputSim(z_resultcontrol, array_z  );


                


// // // // ////////* Sum over last axis for z*////////////

//                 var SLA_z_index = Scope.CreateBus<IndexValue>();
//                 var pipeout_z_SLA = Scope.CreateBus<IndexValue>();
//                 var pipeout1_z_SLA = Scope.CreateBus<IndexValue>();
//                 var SLA_z_result = Scope.CreateBus<ValueTransfer>();
//                 var SLA_z_resultcontrol = Scope.CreateBus<IndexControl>();
//                 var controlind_z_SLA = Scope.CreateBus<IndexControl>();


//                 var array_z_SLA = new SME.Components.SimpleDualPortMemory<double>(Constants.num_networks);

//                 var generate_z_SLA = new GenerateA(SLA_z_index, array_z.ReadControl); 

//                 var dim_control_z_SLA = new TestIndexSim(controlind_z_SLA,3,3); //FORSTÅR IKKE HVORFOR 3,3 ????? burde være 1,3 what
//                 var SLA_z_ind = new SLAIndex(z_resultcontrol, SLA_z_index ,SLA_z_resultcontrol,  controlind_z_SLA);
//                 var pipe_z_SLA = new Pipe(SLA_z_index, pipeout_z_SLA);
//                 var SLA_z = new SumLastAxis(array_z.ReadResult, pipeout_z_SLA, SLA_z_result);
//                 var pipe1_z_SLA = new Pipe(pipeout_z_SLA, pipeout1_z_SLA );

//                 var toram_z_SLA = new ToRam_SLA_z(SLA_z_result, pipeout1_z_SLA, array_z_SLA.WriteControl);

//                 //var outsimtra = new OutputSim(SLA_z_resultcontrol, array_z_SLA );





// // // ///////////// r /////////////////////////     


//                 var r_index = Scope.CreateBus<IndexValue>();
//                 var pipeout_r = Scope.CreateBus<IndexValue>();
//                 var pipe1out_r = Scope.CreateBus<IndexValue>();
//                 var r_result = Scope.CreateBus<ValueTransfer>();
//                 var r_resultcontrol = Scope.CreateBus<IndexControl>();
//                 var control_r = Scope.CreateBus<IndexControl>();


//                 var array_r = new SME.Components.SimpleDualPortMemory<double>(Constants.num_networks *Constants.hidden_size);

//                 var generate_r_A = new GenerateA(r_index, array_hr_result.ReadControl);
//                 var generate_r_B = new GenerateB(r_index, Wr_data.output.ReadControl);
                

//                 var dim_control_r = new TestIndexSim(control_r,3,3);
//                 var r_ind = new ZIndex(hr_resultcontrol, Wr_data.control, r_index, r_resultcontrol, control_r);
//                 var pipe_r = new Pipe(r_index, pipeout_r);
//                 var rr = new Mul(pipeout_r, array_hr_result.ReadResult, Wr_data.output.ReadResult, r_result);
//                 var pipe_r_1 = new Pipe(pipeout_r, pipe1out_r);

//                 var toram_r = new ToRam_r(r_result, pipe1out_r, array_r.WriteControl);

//                 var outsimtra = new OutputSim(r_resultcontrol, array_r  );





// // // // ////////* Sum over last axis for r *////////////
//                 var SLA_r_index = Scope.CreateBus<IndexValue>();
//                 var pipeout_r_SLA = Scope.CreateBus<IndexValue>();
//                 var pipeout1_r_SLA = Scope.CreateBus<IndexValue>();
//                 var SLA_r_result = Scope.CreateBus<ValueTransfer>();
//                 var SLA_r_resultcontrol = Scope.CreateBus<IndexControl>();
//                 var controlind_r_SLA = Scope.CreateBus<IndexControl>();


//                 var array_r_SLA = new SME.Components.SimpleDualPortMemory<double>(Constants.num_networks);

//                 var generate_r_SLA = new GenerateA(SLA_r_index, array_r.ReadControl); 

//                 var dim_control_r_SLA = new TestIndexSim(controlind_r_SLA,3,3); //FORSTÅR IKKE HVORFOR 3,3 ????? burde være 1,3 what
//                 var SLA_r_ind = new SLAIndex(r_resultcontrol, SLA_r_index ,SLA_r_resultcontrol,  controlind_r_SLA);
//                 var pipe_r_SLA = new Pipe(SLA_r_index, pipeout_r_SLA);
//                 var SLA_r = new SumLastAxis(array_r.ReadResult, pipeout_r_SLA, SLA_r_result);
//                 var pipe1_r_SLA = new Pipe(pipeout_r_SLA, pipeout1_r_SLA );

//                 var toram_r_SLA = new ToRam_SLA_r(SLA_r_result, pipeout1_r_SLA, array_r_SLA.WriteControl);

//                 //var outsimtra = new OutputSim(SLA_r_resultcontrol, array_r_SLA );


// // ///////////////// multiply zz_scale and z ///////////////////////// 
//                 var zz_index = Scope.CreateBus<IndexValue>();
//                 var pipeout_zz = Scope.CreateBus<IndexValue>();
//                 var pipe1out_zz = Scope.CreateBus<IndexValue>();
//                 var zz_result = Scope.CreateBus<ValueTransfer>();
//                 var zz_resultcontrol = Scope.CreateBus<IndexControl>();
//                 var control_zz = Scope.CreateBus<IndexControl>();


//                 var array_zz = new SME.Components.SimpleDualPortMemory<double>(Constants.input_size);

//                 var generate_zz_A = new GenerateA(zz_index, array_zscale.ReadControl);
//                 var generate_zz_B = new GenerateB(zz_index, array_z_SLA.ReadControl);
                

//                 var dim_control_zz = new TestIndexSim(control_zz,1,3);
//                 var zz_ind = new MulIndex(control_zscale, SLA_z_resultcontrol , zz_index, zz_resultcontrol, control_zz);
//                 var pipe_zz = new Pipe(zz_index, pipeout_zz);
//                 var zz = new Mul(pipeout_zz, array_zscale.ReadResult, array_z_SLA.ReadResult, zz_result);
//                 var pipe_zz_1 = new Pipe(pipeout_zz, pipe1out_zz);

//                 var toram_zz = new ToRam_zz(zz_result, pipe1out_zz, array_zz.WriteControl);

//                //var outsimtra = new OutputSim(zz_resultcontrol, array_zz  );


// // // ///////////////// sigmoid of zz /////////////////////////
//                 var sig_index = Scope.CreateBus<IndexValue>();
//                 var pipeout_sig = Scope.CreateBus<IndexValue>();
//                 var pipeout1_sig = Scope.CreateBus<IndexValue>();
//                 var sig_result = Scope.CreateBus<ValueTransfer>();
//                 var sig_resultcontrol = Scope.CreateBus<IndexControl>();
//                 var controlind_sig = Scope.CreateBus<IndexControl>();


//                 var array_sig = new SME.Components.SimpleDualPortMemory<double>(Constants.input_size);

//                 var generate_Sig = new GenerateA(sig_index, array_zz.ReadControl);

//                 var dim_control_sig = new TestIndexSim(controlind_sig, 1, 3);
//                 var sig_ind = new SLAIndex(zz_resultcontrol, sig_index ,sig_resultcontrol,  controlind_sig);
//                 var pipe_sig = new Pipe(sig_index, pipeout_sig);
//                 var sigmoid = new Sigmoid(array_zz.ReadResult,pipeout_sig, sig_result);
//                 var pipe1_sig = new Pipe(pipeout_sig, pipeout1_sig );

//                 var toram_sig = new ToRam_Sig(sig_result, pipeout1_sig, array_sig.WriteControl);

//                 //var outsimtra = new OutputSim(sig_resultcontrol, array_sig );


// // // ///////////////// 2* sig - 1 ///////////////////////// 

//                 var minmul_index = Scope.CreateBus<IndexValue>();
//                 var pipeout_minmul = Scope.CreateBus<IndexValue>();
//                 var pipeout1_minmul = Scope.CreateBus<IndexValue>();
//                 var minmul_result = Scope.CreateBus<ValueTransfer>();
//                 var minmul_resultcontrol = Scope.CreateBus<IndexControl>();
//                 var controlind_minmul = Scope.CreateBus<IndexControl>();


//                 var array_minmul = new SME.Components.SimpleDualPortMemory<double>(Constants.input_size);

//                 var generate_minmul = new GenerateA(minmul_index, array_sig.ReadControl);

//                 var dim_control_minmul = new TestIndexSim(controlind_minmul, 1, 3);
//                 var minmul_ind = new SigIndex(sig_resultcontrol, minmul_index ,minmul_resultcontrol, controlind_minmul);
//                 var pipe_minmul = new Pipe(minmul_index, pipeout_minmul);
//                 var minmul = new MulMin(array_sig.ReadResult,pipeout_minmul, minmul_result);
//                 var pipe1_minmul = new Pipe(pipeout_minmul, pipeout1_minmul );

//                 var toram_minmul = new ToRam_minmul(minmul_result, pipeout1_minmul, array_minmul.WriteControl);

//                 //var outsimtra = new OutputSim(minmul_resultcontrol, array_minmul );



// // // ///////////////// Softplus of r /////////////////////////
//                 var soft_index = Scope.CreateBus<IndexValue>();
//                 var pipeout_soft = Scope.CreateBus<IndexValue>();
//                 var pipeout1_soft = Scope.CreateBus<IndexValue>();
//                 var soft_result = Scope.CreateBus<ValueTransfer>();
//                 var soft_resultcontrol = Scope.CreateBus<IndexControl>();
//                 var controlind_soft = Scope.CreateBus<IndexControl>();


//                 var array_soft = new SME.Components.SimpleDualPortMemory<double>(Constants.input_size);

//                 var generate_soft = new GenerateA(soft_index, array_r_SLA.ReadControl);

//                 var dim_control_soft = new TestIndexSim(controlind_soft,1, 3);
//                 var soft_ind = new SigIndex(SLA_r_resultcontrol, soft_index ,soft_resultcontrol,  controlind_soft);
//                 var pipe_soft = new Pipe(soft_index, pipeout_soft);
//                 var softplus = new Softplus(array_r_SLA.ReadResult,pipeout_soft, soft_result);
//                 var pipe1_soft = new Pipe(pipeout_soft, pipeout1_soft );

//                 var toram_soft = new ToRam_Soft(soft_result, pipeout1_soft, array_soft.WriteControl);

// //                var outsimtra = new OutputSim(soft_resultcontrol, array_soft );

// // ///////////////// softplus * minmul ///////////////////////// 
//                 var rz_index = Scope.CreateBus<IndexValue>();
//                 var pipeout_rz = Scope.CreateBus<IndexValue>();
//                 var pipe1out_rz = Scope.CreateBus<IndexValue>();
//                 var rz_result = Scope.CreateBus<ValueTransfer>();
//                 var rz_resultcontrol = Scope.CreateBus<IndexControl>();
//                 var control_rz = Scope.CreateBus<IndexControl>();


//                 var array_rz = new SME.Components.SimpleDualPortMemory<double>(Constants.input_size);

//                 var generate_rz_A = new GenerateA(rz_index, array_soft.ReadControl);
//                 var generate_rz_B = new GenerateB(rz_index, array_minmul.ReadControl);
                

//                 var dim_control_rz = new TestIndexSim(control_rz,1,3);
//                 var rz_ind = new MulIndex(soft_resultcontrol, minmul_resultcontrol , rz_index, rz_resultcontrol, control_rz);
//                 var pipe_rz = new Pipe(rz_index, pipeout_rz);
//                 var rz = new Mul(pipeout_rz, array_soft.ReadResult, array_minmul.ReadResult, rz_result);
//                 var pipe_rz_1 = new Pipe(pipeout_rz, pipe1out_rz);

//                 var toram_rz = new ToRam_rz(rz_result, pipe1out_rz, array_rz.WriteControl);

//                //var outsimtra = new OutputSim(rz_resultcontrol, array_rz  );




// // // // ////////* Clamp *////////////
//                 var clamp_index = Scope.CreateBus<IndexValue>();
//                 var pipeout_clamp = Scope.CreateBus<IndexValue>();
//                 var pipeout1_clamp = Scope.CreateBus<IndexValue>();
//                 var clamp_result = Scope.CreateBus<ValueTransfer>();
//                 var clamp_resultcontrol = Scope.CreateBus<IndexControl>();
//                 var controlind_clamp = Scope.CreateBus<IndexControl>();
 
//                 var array_clamp = new SME.Components.SimpleDualPortMemory<double>(Constants.input_size);

//                 var generate_clamp = new GenerateA(clamp_index, array_rz.ReadControl);
                
//                 var dim_control_clamp = new TestIndexSim(controlind_clamp,1,3);
//                 var clampind = new SigIndex(rz_resultcontrol, clamp_index, clamp_resultcontrol, controlind_clamp);
//                 var pipe_clamp = new Pipe(clamp_index, pipeout_clamp);
//                 //NOGET GALT MED MIN VÆRDI FOR CLAMP
//                 var clamp = new Clamp(array_rz.ReadResult, pipeout_clamp, clamp_result, -Constants.max_predict, Constants.max_predict);
//                 var pipe1_clamp  = new Pipe(pipeout_clamp, pipeout1_clamp);
                
//                 var toram_clamp = new ToRam_Clamp(clamp_result, pipeout1_clamp, array_clamp.WriteControl);
               

//                 //var outsimtra = new OutputSim(clamp_resultcontrol, array_clamp );


// // // // ////////* mean *////////////

//                 // var mean_index = Scope.CreateBus<IndexValue>();
//                 // var pipeout_mean = Scope.CreateBus<IndexValue>();
//                 // var pipeout1_mean = Scope.CreateBus<IndexValue>();
//                 // var mean_result = Scope.CreateBus<ValueTransfer>();
//                 // var mean_resultcontrol = Scope.CreateBus<IndexControl>();
//                 // var controlind_mean = Scope.CreateBus<IndexControl>();


//                 // var array_mean = new SME.Components.SimpleDualPortMemory<double>(Constants.max_predict);

//                 // //var generate_mean = new GenerateA(mean_index, array_C.ReadControl);
//                 // var generate_mean = new GenerateA(mean_index, array_clamp.ReadControl); 

//                 // var dim_control_mean = new TestIndexSim(controlind_mean, 1,3);
//                 // var mean_ind = new meanIndex(clamp_resultcontrol, mean_index ,mean_resultcontrol,  controlind_mean);
//                 // var pipe_mean = new Pipe(mean_index, pipeout_mean);
//                 // var mean = new Mean(array_clamp.ReadResult,pipeout_mean, mean_result);
//                 // var pipe1_mean = new Pipe(pipeout_mean, pipeout1_mean );

//                 // var toram_mean = new ToRam_mean(mean_result, pipeout1_mean, array_mean.WriteControl);

//                 // var outsimtra = new OutputSim(mean_resultcontrol, array_mean );
                


// // //////// Test data C ///////////////
//             var C_control = Scope.CreateBus<IndexControl>();
//             var C_index = Scope.CreateBus<IndexValue>();
//             var control_C = Scope.CreateBus<IndexControl>();

//             var array_C = new SME.Components.SimpleDualPortMemory<double>(Constants.input_size*Constants.input_size);

//             var C_sim = new TestIndexSim(C_control,3,3);
//             var generate_C = new Data_Wr(C_index, array_C.WriteControl);

//             var C_ind = new Index(C_control, C_index, control_C);
            
//             C_sim.ram = array_C;  

//             //var outsimtra = new OutputSim(control_C, array_C  );



// // // // ////////* mean *////////////

//                 var mean_index = Scope.CreateBus<IndexValue>();
//                 var pipeout_mean = Scope.CreateBus<IndexValue>();
//                 var pipeout1_mean = Scope.CreateBus<IndexValue>();
//                 var mean_result = Scope.CreateBus<ValueTransfer>();
//                 var mean_resultcontrol = Scope.CreateBus<IndexControl>();
//                 var controlind_mean = Scope.CreateBus<IndexControl>();


//                 var array_mean = new SME.Components.SimpleDualPortMemory<double>(Constants.max_predict);

//                 //var generate_mean = new GenerateA(mean_index, array_C.ReadControl);
//                 var generate_mean = new GenerateA(mean_index, array_C.ReadControl); 

//                 var dim_control_mean = new TestIndexSim(controlind_mean, 1,3);
//                 var mean_ind = new meanIndex(control_C, mean_index ,mean_resultcontrol,  controlind_mean);
//                 var pipe_mean = new Pipe(mean_index, pipeout_mean);
//                 var mean = new Mean(array_C.ReadResult,pipeout_mean, mean_result);
//                 var pipe1_mean = new Pipe(pipeout_mean, pipeout1_mean );

//                 var toram_mean = new ToRam_mean(mean_result, pipeout1_mean, array_mean.WriteControl);

//                 //var outsimtra = new OutputSim(mean_resultcontrol, array_mean );
                
                
//                 sim
//                     //.AddTopLevelInputs(simulationdata)
//                     //.AddTopLevelOutputs(sigmoidresult)
//                     //.AddPostClockRunner(s => Console.WriteLine("Current clock is: {0}", s.Tick))
//                     .BuildCSVFile()
//                     .BuildGraph()
//                     .Run();
//             }        
//         }
//     }
// }
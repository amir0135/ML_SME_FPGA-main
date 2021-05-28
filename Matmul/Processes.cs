using System;
using SME;
using SME.VHDL;
using Deflib;

namespace Matmul
{
    [ClockedProcess]
    public class MatMul_add : SimpleProcess
    {
        [InputBus]
        private ValueTransfer m_inputAB;
        [InputBus]
        private SME.Components.SimpleDualPortMemory<float>.IReadResult m_inputC;
        [InputBus]
        private IndexValue input_pipe;

        [OutputBus]
        private ValueTransfer v_output;

        public class Renderer : ICustomRenderer
        {
            public string IncludeRegion(RenderStateProcess renderer, int indentation)
            {
                return string.Empty;
            }

            public string BodyRegion(RenderStateProcess renderer, int indentation)
            {
                return @"
    signal add_res : T_SYSTEM_FLOAT := (others => '0');
    signal ignore : std_logic;

    -- create_ip -name floating_point -vendor xilinx.com -library ip -version 7.1 -module_name matmul_fl_add
    -- set_property -dict [list CONFIG.Component_Name {matmul_fl_add} CONFIG.Add_Sub_Value {Add} CONFIG.Flow_Control {NonBlocking} CONFIG.Maximum_Latency {false} CONFIG.C_Latency {0} CONFIG.Has_RESULT_TREADY {false}] [get_ips matmul_fl_add]
    COMPONENT matmul_fl_add
    PORT (
        s_axis_a_tvalid : IN STD_LOGIC;
        s_axis_a_tdata : IN STD_LOGIC_VECTOR(31 DOWNTO 0);
        s_axis_b_tvalid : IN STD_LOGIC;
        s_axis_b_tdata : IN STD_LOGIC_VECTOR(31 DOWNTO 0);
        m_axis_result_tvalid : OUT STD_LOGIC;
        m_axis_result_tdata : OUT STD_LOGIC_VECTOR(31 DOWNTO 0)
    );
    END COMPONENT;
begin
    process (
        CLK,
        RST
    )
    begin
        if RST = '1' then
            FIN <= '0';
        elsif rising_edge(CLK) then
            if input_pipe_Ready = '1' then
                v_output_Value <= add_res;
            else
                v_output_Value <= (others => '0');
            end if;
            FIN <= not RDY;
        end if;
    end process;

    adder : matmul_fl_add
    PORT MAP (
        s_axis_a_tvalid => input_pipe_Ready,
        s_axis_a_tdata => m_inputC_Data,
        s_axis_b_tvalid => input_pipe_Ready,
        s_axis_b_tdata => m_inputAB_Value,
        m_axis_result_tvalid => ignore,
        m_axis_result_tdata => add_res
    );
                ";
            }
        }
        [Ignore]
        private Renderer renderer = new Renderer();
        public override object CustomRenderer { get { return renderer; } }

        public MatMul_add(IndexValue inputpipe, ValueTransfer inputAB, SME.Components.SimpleDualPortMemory<float>.IReadResult inputC, ValueTransfer output)
        {
            input_pipe = inputpipe ?? throw new ArgumentNullException(nameof(inputpipe));
            m_inputAB = inputAB ?? throw new ArgumentNullException(nameof(inputAB));
            m_inputC = inputC ?? throw new ArgumentNullException(nameof(inputC));
            v_output = output ?? throw new ArgumentNullException(nameof(output));
        }

        protected override void OnTick()
        {
            if (input_pipe.Ready == true)
            {
                v_output.value = m_inputC.Data + m_inputAB.value;
            }
            else
            {
                v_output.value = 0;
            }
        }
    }

    [ClockedProcess]
    public class MatMul : SimpleProcess
    {
        [InputBus]
        private SME.Components.SimpleDualPortMemory<float>.IReadResult m_inputA;
        [InputBus]
        private SME.Components.SimpleDualPortMemory<float>.IReadResult m_inputB;
        [InputBus]
        private IndexValue input_pipe;

        [OutputBus]
        private ValueTransfer v_output;

        public class Renderer : ICustomRenderer
        {
            public string IncludeRegion(RenderStateProcess renderer, int indentation)
            {
                return string.Empty;
            }

            public string BodyRegion(RenderStateProcess renderer, int indentation)
            {
                return @"
    signal mul_res : T_SYSTEM_FLOAT := (others => '0');
    signal ignore : std_logic;

    -- create_ip -name floating_point -vendor xilinx.com -library ip -version 7.1 -module_name matmul_fl_mul
    -- set_property -dict [list CONFIG.Component_Name {matmul_fl_mul} CONFIG.Operation_Type {Multiply} CONFIG.Flow_Control {NonBlocking} CONFIG.Maximum_Latency {false} CONFIG.C_Latency {0} CONFIG.A_Precision_Type {Single} CONFIG.C_A_Exponent_Width {8} CONFIG.C_A_Fraction_Width {24} CONFIG.Result_Precision_Type {Single} CONFIG.C_Result_Exponent_Width {8} CONFIG.C_Result_Fraction_Width {24} CONFIG.C_Mult_Usage {Full_Usage} CONFIG.Has_RESULT_TREADY {false} CONFIG.C_Rate {1}] [get_ips matmul_fl_mul]
    COMPONENT matmul_fl_mul
    PORT (
        s_axis_a_tvalid : IN STD_LOGIC;
        s_axis_a_tdata : IN STD_LOGIC_VECTOR(31 DOWNTO 0);
        s_axis_b_tvalid : IN STD_LOGIC;
        s_axis_b_tdata : IN STD_LOGIC_VECTOR(31 DOWNTO 0);
        m_axis_result_tvalid : OUT STD_LOGIC;
        m_axis_result_tdata : OUT STD_LOGIC_VECTOR(31 DOWNTO 0)
    );
    END COMPONENT;
begin
    process (
        CLK,
        RST
    )
    begin
        if RST = '1' then
            FIN <= '0';
        elsif rising_edge(CLK) then
            if input_pipe_Ready = '1' then
                v_output_Value <= mul_res;
            else
                v_output_Value <= (others => '0');
            end if;
            FIN <= not RDY;
        end if;
    end process;

    muller : matmul_fl_mul
    PORT MAP (
        s_axis_a_tvalid => input_pipe_Ready,
        s_axis_a_tdata => m_inputA_Data,
        s_axis_b_tvalid => input_pipe_Ready,
        s_axis_b_tdata => m_inputB_Data,
        m_axis_result_tvalid => ignore,
        m_axis_result_tdata => mul_res
    );
                ";
            }
        }
        [Ignore]
        private Renderer renderer = new Renderer();
        public override object CustomRenderer { get { return renderer; } }

        public MatMul(IndexValue inputpipe, SME.Components.SimpleDualPortMemory<float>.IReadResult inputA, SME.Components.SimpleDualPortMemory<float>.IReadResult inputB, ValueTransfer output)
        {
            input_pipe = inputpipe ?? throw new ArgumentNullException(nameof(inputpipe));
            m_inputA = inputA ?? throw new ArgumentNullException(nameof(inputA));
            m_inputB = inputB ?? throw new ArgumentNullException(nameof(inputB));
            v_output = output ?? throw new ArgumentNullException(nameof(output));
        }

        protected override void OnTick()
        {
            if (input_pipe.Ready == true)
            {

                v_output.value = m_inputA.Data * m_inputB.Data;
            }
            else
            {
                v_output.value = 0;
            }
        }
    }

    [ClockedProcess]
    public class MatMulIndex : SimpleProcess
    {
        [InputBus]
        private IndexControl controlA;
        [InputBus]
        private IndexControl controlB;

        [OutputBus]
        private IndexValue outputA;
        [OutputBus]
        private IndexValue outputB;
        [OutputBus]
        private IndexValue outputC;
        [OutputBus]
        private IndexControl controlout;

        private bool running = false;
        private int i = 0, j = 0, k = 0;
        private int widthA, heightA, widthB, heightB;
        private bool Aready = false, Bready = false;
        private bool started = false;

        public MatMulIndex(IndexControl controlA, IndexControl controlB, IndexValue outputA, IndexValue outputB, IndexValue outputC, IndexControl controlout)
        {
            this.controlA = controlA;
            this.controlB = controlB;
            this.controlout = controlout;
            this.outputA = outputA;
            this.outputB = outputB;
            this.outputC = outputC;
        }

        protected override void OnTick()
        {
            if (running == true)
            {
                outputA.Ready = true;
                outputB.Ready = true;
                outputC.Ready = true;
                started = true;

                outputA.Addr = i * widthA + k;
                outputB.Addr = k * widthB + j;
                outputC.Addr = i * widthA + j;

                k++;

                if (k >= widthA)
                {
                    k = 0;
                    j++;
                }

                if (j >= widthB)
                {
                    j = 0;
                    i++;
                }

                if (i >= heightA)
                {
                    running = false;
                }
            }
            else
            {
                Aready |= controlA.Ready;
                Bready |= controlB.Ready;

                if (Aready && Bready)
                {
                    Aready = false;
                    Bready = false;

                    running = true;
                    widthA = controlA.Width;
                    heightA = controlA.Height;
                    widthB = controlB.Width;
                    heightB = controlB.Height;

                    i = j = k = 0;

                    outputA.Addr = controlA.OffsetA;
                    outputB.Addr = controlB.OffsetB;
                    outputC.Addr = controlA.OffsetC;
                    started = true;
                }
                else
                {
                    if (started == true)
                    {
                        controlout.Ready = true;
                        controlout.Height = controlA.Height;
                        controlout.Width = controlB.Width;
                        controlout.OffsetA = controlA.OffsetA;
                        controlout.OffsetB = controlA.OffsetB;
                        controlout.OffsetC = controlA.OffsetC;
                        controlout.Dim = controlA.Dim;
                        started = false;
                    }
                    else
                    {
                        controlout.Ready = false;
                    }

                    outputA.Ready = false;
                    outputB.Ready = false;
                    outputC.Ready = false;
                }
            }
        }
    }

    public class Forward : SimpleProcess
    {
        [InputBus]
        private IndexValue old_input;
        [InputBus]
        private IndexValue new_input;
        [InputBus]
        private ValueTransfer v_inputNew;
        [InputBus]
        private SME.Components.SimpleDualPortMemory<float>.IReadResult v_inputOld;

        [OutputBus]
        private SME.Components.SimpleDualPortMemory<float>.IReadResult v_output;

        public Forward(IndexValue old_input, IndexValue new_input, ValueTransfer v_inputNew, SME.Components.SimpleDualPortMemory<float>.IReadResult v_inputOld, SME.Components.SimpleDualPortMemory<float>.IReadResult v_output)
        {
            this.old_input = old_input;
            this.new_input = new_input;
            this.v_inputNew = v_inputNew;
            this.v_inputOld = v_inputOld;
            this.v_output = v_output;
        }

        protected override void OnTick()
        {
            if (new_input.Ready && new_input.Addr == old_input.Addr)
            {
                v_output.Data = v_inputNew.value;
            }
            else if (old_input.Ready)
            {
                v_output.Data = v_inputOld.Data;
            }
            else
            {
                v_output.Data = 0;
            }
        }
    }

}
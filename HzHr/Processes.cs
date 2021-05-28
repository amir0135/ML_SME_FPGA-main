using System;
using SME;
using SME.VHDL;
using Deflib;

namespace HzHr
{

    [ClockedProcess]
    public class Hz : SimpleProcess
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
    signal less_than_zero : std_logic_vector(7 downto 0) := (others => '0');
    signal A_mul_B : T_SYSTEM_FLOAT := (others => '0');
    signal ignore : std_logic;

    -- create_ip -name floating_point -vendor xilinx.com -library ip -version 7.1 -module_name hzhr_less
    -- set_property -dict [list CONFIG.Component_Name {hzhr_less} CONFIG.Operation_Type {Compare} CONFIG.C_Compare_Operation {Less_Than} CONFIG.Flow_Control {NonBlocking} CONFIG.Maximum_Latency {false} CONFIG.C_Latency {0} CONFIG.A_Precision_Type {Single} CONFIG.C_A_Exponent_Width {8} CONFIG.C_A_Fraction_Width {24} CONFIG.Result_Precision_Type {Custom} CONFIG.C_Result_Exponent_Width {1} CONFIG.C_Result_Fraction_Width {0} CONFIG.C_Mult_Usage {No_Usage} CONFIG.Has_RESULT_TREADY {false} CONFIG.C_Rate {1}] [get_ips hzhr_less]
    COMPONENT hzhr_less
    PORT (
        s_axis_a_tvalid : IN STD_LOGIC;
        s_axis_a_tdata : IN STD_LOGIC_VECTOR(31 DOWNTO 0);
        s_axis_b_tvalid : IN STD_LOGIC;
        s_axis_b_tdata : IN STD_LOGIC_VECTOR(31 DOWNTO 0);
        m_axis_result_tvalid : OUT STD_LOGIC;
        m_axis_result_tdata : OUT STD_LOGIC_VECTOR(7 DOWNTO 0)
    );
    END COMPONENT;

    -- create_ip -name floating_point -vendor xilinx.com -library ip -version 7.1 -module_name hzhr_mul
    -- set_property -dict [list CONFIG.Component_Name {hzhr_mul} CONFIG.Operation_Type {Multiply} CONFIG.Flow_Control {NonBlocking} CONFIG.Maximum_Latency {false} CONFIG.C_Latency {0} CONFIG.A_Precision_Type {Single} CONFIG.C_A_Exponent_Width {8} CONFIG.C_A_Fraction_Width {24} CONFIG.Result_Precision_Type {Single} CONFIG.C_Result_Exponent_Width {8} CONFIG.C_Result_Fraction_Width {24} CONFIG.C_Mult_Usage {Full_Usage} CONFIG.Has_RESULT_TREADY {false} CONFIG.C_Rate {1}] [get_ips hzhr_mul]
    COMPONENT hzhr_mul
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
            if input_Pipe_Ready = '1' then
                if less_than_zero(0) = '1' then
                    v_output_Value <= A_mul_B;
                else
                    v_output_Value <= m_inputA_Data;
                end if;
            else
                v_output_Value <= (others => '0');
            end if;
            FIN <= not RDY;
        end if;
    end process;

    mul: hzhr_mul
    port map (
        s_axis_a_tvalid => input_Pipe_Ready,
        s_axis_a_tdata => m_inputA_Data,
        s_axis_b_tvalid => input_Pipe_Ready,
        s_axis_b_tdata => m_inputB_Data,
        m_axis_result_tvalid => ignore,
        m_axis_result_tdata => A_mul_B
    );

    lt: hzhr_less
    port map (
        s_axis_a_tvalid => input_Pipe_Ready,
        s_axis_a_tdata => m_inputA_Data,
        s_axis_b_tvalid => input_Pipe_Ready,
        s_axis_b_tdata => (others => '0'),
        m_axis_result_tvalid => ignore,
        m_axis_result_tdata => less_than_zero
    );
                ";
            }
        }
        [Ignore]
        private Renderer renderer = new Renderer();
        public override object CustomRenderer { get { return renderer; } }

        public Hz(IndexValue inputpipe, SME.Components.SimpleDualPortMemory<float>.IReadResult inputA, SME.Components.SimpleDualPortMemory<float>.IReadResult inputB, ValueTransfer output)
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
                if (m_inputA.Data < 0)
                {
                    v_output.value = m_inputA.Data * m_inputB.Data;
                }
                else
                {
                    v_output.value = m_inputA.Data;
                }
            }
            else
            {
                v_output.value = 0;
            }
        }
    }

    [ClockedProcess]
    public class HzIndex : SimpleProcess
    {
        [InputBus]
        private IndexControl control;
        [InputBus]
        private IndexControl controlA;
        [InputBus]
        private IndexControl controlB;

        [OutputBus]
        private IndexValue outputA;
        [OutputBus]
        private IndexValue outputB;
        [OutputBus]
        private IndexControl controlout;

        private bool running = false;
        private int i = 0, j = 0, k = 0;
        private int width, height, dim;
        private bool Aready = false, Bready = false;
        private bool started = false;

        public HzIndex(IndexControl controlA, IndexControl controlB, IndexValue outputA, IndexValue outputB, IndexControl controlout, IndexControl control)
        {
            this.controlA = controlA;
            this.controlB = controlB;
            this.controlout = controlout;
            this.outputA = outputA;
            this.outputB = outputB;
            this.control = control;
        }

        protected override void OnTick()
        {
            if (running == true)
            {
                outputA.Ready = true;
                outputB.Ready = true;
                started = true;

                outputA.Addr = i*width*height + j*width + k;
                outputB.Addr = j;

                k++;

                if (k >= width)
                {
                    k = 0;
                    j++;
                }

                if (j >= height)
                {
                    j = 0;
                    i++;
                }

                if (i >= dim)
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
                    width = control.Width;
                    height = control.Height;
                    dim = control.Dim;

                    i = j = k = 0;

                    outputA.Ready = true;
                    outputA.Addr = controlA.OffsetA;
                    outputB.Ready = true;
                    outputB.Addr = controlB.OffsetB;
                    started = true;
                }
                else
                {
                    if (started == true)
                    {

                        controlout.Ready = true;
                        controlout.Height = control.Height;
                        controlout.Width = control.Width;
                        controlout.Dim = control.Dim;
                        controlout.OffsetA = controlA.OffsetA;
                        controlout.OffsetB = controlA.OffsetB;
                        controlout.OffsetC = controlA.OffsetC;
                        started = false;
                    }
                    else
                    {
                        controlout.Ready = false;
                    }

                    outputA.Ready = false;
                    outputB.Ready = false;
                }
            }
        }
    }

}


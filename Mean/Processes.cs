using SME;
using SME.VHDL;
using Deflib;
using System;

namespace Mean
{

    [ClockedProcess]
    public class Mean_count : SimpleProcess
    {
        [InputBus]
        private IndexValue index;
        [InputBus]
        private Flag flush;

        [OutputBus]
        private ValueTransfer m_output;

        private int count = 0;

        public Mean_count(IndexValue index, ValueTransfer output, Flag flush)
        {
            this.index = index ?? throw new ArgumentNullException(nameof(index));
            this.m_output = output ?? throw new ArgumentNullException(nameof(output));
            this.flush = flush ?? throw new ArgumentNullException(nameof(flush));
        }

        protected override void OnTick()
        {
            if (index.Ready)
            {
                if (flush.flg)
                {
                    m_output.value = count + 1;
                    count = 0;
                }
                else
                {
                    count += 1;
                }
            }
        }
    }

    [ClockedProcess]
    public class Mean_add : SimpleProcess
    {
        [InputBus]
        private SME.Components.SimpleDualPortMemory<float>.IReadResult m_input;
        [InputBus]
        private IndexValue index;
        [InputBus]
        private Flag flush;

        [OutputBus]
        private ValueTransfer m_output;

        public class Renderer : ICustomRenderer
        {
            public string IncludeRegion(RenderStateProcess renderer, int indentation)
            {
                return string.Empty;
            }

            public string BodyRegion(RenderStateProcess renderer, int indentation)
            {
                return @"
    signal accumulated : T_SYSTEM_FLOAT := (others => '0');
    signal add_res : T_SYSTEM_FLOAT := (others => '0');
    signal ignore : std_logic;

    -- create_ip -name floating_point -vendor xilinx.com -library ip -version 7.1 -module_name mean_fl_add
    -- set_property -dict [list CONFIG.Component_Name {mean_fl_add} CONFIG.Add_Sub_Value {Add} CONFIG.Flow_Control {NonBlocking} CONFIG.Maximum_Latency {false} CONFIG.C_Latency {0} CONFIG.Has_RESULT_TREADY {false}] [get_ips mean_fl_add]
    COMPONENT mean_fl_add
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
            if index_Ready = '1' then
                if flush_flg = '1' then
                    m_output_Value <= add_res;
                    accumulated <= (others => '0');
                else
                    accumulated <= add_res;
                end if;
            end if;
            FIN <= not RDY;
        end if;
    end process;

    adder : mean_fl_add
    PORT MAP (
        s_axis_a_tvalid => index_Ready,
        s_axis_a_tdata => accumulated,
        s_axis_b_tvalid => index_Ready,
        s_axis_b_tdata => m_input_Data,
        m_axis_result_tvalid => ignore,
        m_axis_result_tdata => add_res
    );
                ";
            }
        }
        [Ignore]
        private Renderer renderer = new Renderer();
        public override object CustomRenderer { get { return renderer; } }

        private float accumulated = 0;

        public Mean_add(SME.Components.SimpleDualPortMemory<float>.IReadResult input, IndexValue index, ValueTransfer output, Flag flush)
        {
            m_input = input ?? throw new ArgumentNullException(nameof(input));
            this.index = index ?? throw new ArgumentNullException(nameof(index));
            m_output = output ?? throw new ArgumentNullException(nameof(output));
            this.flush = flush ?? throw new ArgumentNullException(nameof(flush));
        }

        protected override void OnTick()
        {
            if (index.Ready)
            {
                if (flush.flg)
                {
                    m_output.value = accumulated + m_input.Data;
                    accumulated = 0;
                }
                else
                {
                    accumulated += m_input.Data;
                }
            }
        }
    }


    [ClockedProcess]
    public class Mean_div : SimpleProcess
    {
        [InputBus]
        private ValueTransfer m_input;
        [InputBus]
        private ValueTransfer m_count;
        [InputBus]
        private IndexValue index;
        [InputBus]
        private Flag flush;

        [OutputBus]
        private ValueTransfer m_output;

        public class Renderer : ICustomRenderer
        {
            public string IncludeRegion(RenderStateProcess renderer, int indentation)
            {
                return string.Empty;
            }

            public string BodyRegion(RenderStateProcess renderer, int indentation)
            {
                return @"
    signal div_res : T_SYSTEM_FLOAT := (others => '0');
    signal ignore : std_logic;

    -- create_ip -name floating_point -vendor xilinx.com -library ip -version 7.1 -module_name mean_fl_div
    -- set_property -dict [list CONFIG.Component_Name {mean_fl_div} CONFIG.Operation_Type {Divide} CONFIG.Flow_Control {NonBlocking} CONFIG.Maximum_Latency {false} CONFIG.C_Latency {0} CONFIG.A_Precision_Type {Single} CONFIG.C_A_Exponent_Width {8} CONFIG.C_A_Fraction_Width {24} CONFIG.Result_Precision_Type {Single} CONFIG.C_Result_Exponent_Width {8} CONFIG.C_Result_Fraction_Width {24} CONFIG.C_Mult_Usage {No_Usage} CONFIG.Has_RESULT_TREADY {false} CONFIG.C_Rate {1}] [get_ips mean_fl_div]
    COMPONENT mean_fl_div
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
            if index_Ready = '1' and flush_flg = '1' then
                m_output_Value <= div_res;
            end if;
            FIN <= not RDY;
        end if;
    end process;

    diver : mean_fl_div
    PORT MAP (
        s_axis_a_tvalid => index_Ready,
        s_axis_a_tdata => m_input_Value,
        s_axis_b_tvalid => index_Ready,
        s_axis_b_tdata => m_count_Value,
        m_axis_result_tvalid => ignore,
        m_axis_result_tdata => div_res
    );
                ";
            }
        }
        [Ignore]
        private Renderer renderer = new Renderer();
        public override object CustomRenderer { get { return renderer; } }

        public Mean_div(ValueTransfer input, ValueTransfer count, IndexValue index, Flag flush, ValueTransfer output)
        {
            m_input = input ?? throw new ArgumentNullException(nameof(input));
            m_count = count ?? throw new ArgumentNullException(nameof(count));
            this.index = index ?? throw new ArgumentNullException(nameof(index));
            m_output = output ?? throw new ArgumentNullException(nameof(output));
            this.flush = flush ?? throw new ArgumentNullException(nameof(flush));
        }

        protected override void OnTick()
        {
            if (index.Ready && flush.flg)
            {
                m_output.value = m_input.value / m_count.value;
            }
        }
    }

    [ClockedProcess]
    public class ShouldSave : SimpleProcess
    {
        [InputBus]
        private IndexValue input;
        [InputBus]
        private Flag flush;

        [OutputBus]
        private IndexValue output;

        public ShouldSave(IndexValue input, Flag flush, IndexValue output)
        {
            this.input = input;
            this.flush = flush;
            this.output = output;
        }

        protected override void OnTick()
        {
            output.Ready = input.Ready && flush.flg;
            if (input.Ready)
            {
                output.Addr = input.Addr;
            }
        }
    }

    [ClockedProcess]
    public class SLAIndex : SimpleProcess
    {
        [InputBus]
        private IndexControl controlA;
        [InputBus]
        private IndexControl control;

        [OutputBus]
        private IndexValue outputA;
        [OutputBus]
        private IndexValue outputB;
        [OutputBus]
        private IndexControl controlout;
        [OutputBus]
        private Flag flush;

        private bool running = false;
        private int i = 0, j = 0;
        private int width, height;
        private bool Aready = false;
        private bool started = false;

        public SLAIndex(IndexControl controlA, IndexValue outputA, IndexValue outputB, IndexControl controlout, IndexControl control, Flag flush)
        {
            this.controlA = controlA;
            this.controlout = controlout;
            this.outputA = outputA;
            this.outputB = outputB;
            this.control = control;
            this.flush = flush;
        }

        protected override void OnTick()
        {
            flush.flg = false;
            if (running)
            {
                outputA.Ready = true;
                outputB.Ready = true;
                started = true;

                outputA.Addr = i * width + j;
                outputB.Addr = i;

                j++;

                if (j >= width)
                {
                    j = 0;
                    i++;
                    flush.flg = true;
                }
                if (i >= height)
                {
                    running = false;
                }
            }
            else
            {
                Aready |= controlA.Ready;

                if (Aready)
                {
                    Aready = false;

                    running = true;
                    width = control.Width;
                    height = control.Height;

                    i = 0;
                    j = 0;
                    outputA.Ready = false;
                    outputA.Addr = controlA.OffsetA;
                    outputB.Ready = false;
                    outputB.Addr = controlA.OffsetB;
                    started = true;
                }
                else
                {
                    if (started == true)
                    {
                        controlout.Ready = true;
                        controlout.Height = control.Height;
                        controlout.Width = control.Width;
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
                }
            }
        }
    }
}
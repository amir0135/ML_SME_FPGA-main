using SME;
using SME.VHDL;
using Deflib;
using System;

namespace sum_lastaxis
{

    [ClockedProcess]
    public class SumLastAxis : SimpleProcess
    {
        [InputBus]
        private SME.Components.SimpleDualPortMemory<float>.IReadResult m_input;
        [InputBus]
        private IndexValue index;

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
    signal add_res : T_SYSTEM_FLOAT := (others => '0');
    signal accumulated : T_SYSTEM_FLOAT := (others => '0');
    signal ind : T_SYSTEM_INT32 := (others => '1');
    signal ignore : std_logic;

    -- create_ip -name floating_point -vendor xilinx.com -library ip -version 7.1 -module_name sumlast_fl_add
    -- set_property -dict [list CONFIG.Component_Name {sumlast_fl_add} CONFIG.Add_Sub_Value {Add} CONFIG.Flow_Control {NonBlocking} CONFIG.Maximum_Latency {false} CONFIG.C_Latency {0} CONFIG.Has_RESULT_TREADY {false}] [get_ips sumlast_fl_add]
    COMPONENT sumlast_fl_add
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
                if index_Addr = ind then
                    accumulated <= add_res;
                    m_output_Value <= accumulated;
                else
                    m_output_Value <= accumulated;
                    accumulated <= m_input_Data;
                    ind <= index_Addr;
                end if;
            end if;
            FIN <= not RDY;
        end if;
    end process;

    adder : sumlast_fl_add
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

        private float accumulated;
        private int ind = -1;

        public SumLastAxis(SME.Components.SimpleDualPortMemory<float>.IReadResult input, IndexValue index, ValueTransfer output)
        {
            m_input = input ?? throw new ArgumentNullException(nameof(input));
            this.index = index ?? throw new ArgumentNullException(nameof(index));
            m_output = output ?? throw new ArgumentNullException(nameof(output));
        }

        protected override void OnTick()
        {
            if (index.Ready == true)
            {
                if (index.Addr == ind)
                {
                    accumulated += m_input.Data;
                    m_output.value = accumulated;
                }
                else
                {
                    m_output.value = accumulated;
                    accumulated = m_input.Data;
                    ind = index.Addr;
                }
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

        private bool running = false;
        private int i = 0, j = 0;
        private int width, height;
        private bool Aready;
        private bool started = false;

        public SLAIndex(IndexControl controlA, IndexValue outputA, IndexValue outputB, IndexControl controlout, IndexControl control)
        {
            this.controlA = controlA;
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

                outputA.Addr = i*width + j;
                outputB.Addr = i;

                j++;

                if (j >= width)
                {
                    j = 0;
                    i++;
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
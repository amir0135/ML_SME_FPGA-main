using SME;
using SME.VHDL;
using Deflib;
using System;

namespace Softplus
{

    [ClockedProcess]
    public class Softplus_1 : SimpleProcess
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
    signal exp_res : T_SYSTEM_FLOAT := (others => '0');
    signal ignore : std_logic;

    -- create_ip -name floating_point -vendor xilinx.com -library ip -version 7.1 -module_name softplus_fl_exp
    -- set_property -dict [list CONFIG.Component_Name {softplus_fl_exp} CONFIG.Operation_Type {Exponential} CONFIG.Flow_Control {NonBlocking} CONFIG.Maximum_Latency {false} CONFIG.C_Latency {0} CONFIG.A_Precision_Type {Single} CONFIG.C_A_Exponent_Width {8} CONFIG.C_A_Fraction_Width {24} CONFIG.Result_Precision_Type {Single} CONFIG.C_Result_Exponent_Width {8} CONFIG.C_Result_Fraction_Width {24} CONFIG.C_Mult_Usage {Medium_Usage} CONFIG.Has_RESULT_TREADY {false} CONFIG.C_Rate {1}] [get_ips softplus_fl_exp]
    COMPONENT softplus_fl_exp
    PORT (
        s_axis_a_tvalid : IN STD_LOGIC;
        s_axis_a_tdata : IN STD_LOGIC_VECTOR(31 DOWNTO 0);
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
                m_output_Value <= exp_res;
            end if;
            FIN <= not RDY;
        end if;
    end process;

    exper : softplus_fl_exp
    PORT MAP (
        s_axis_a_tvalid => index_Ready,
        s_axis_a_tdata => m_input_Data,
        m_axis_result_tvalid => ignore,
        m_axis_result_tdata => exp_res
    );
                ";
            }
        }
        [Ignore]
        private Renderer renderer = new Renderer();
        public override object CustomRenderer { get { return renderer; } }

        //private int threshold = 20;

        public Softplus_1(SME.Components.SimpleDualPortMemory<float>.IReadResult input, IndexValue index, ValueTransfer output, Flag flush)
        {
            m_input = input ?? throw new ArgumentNullException(nameof(input));
            this.index = index ?? throw new ArgumentNullException(nameof(index));
            m_output = output ?? throw new ArgumentNullException(nameof(output));
            this.flush = flush ?? throw new ArgumentNullException(nameof(flush));
        }

        protected override void OnTick()
        {
            if (index.Ready == true)
            {
                SimulationOnly(() => m_output.value = (float)Math.Exp(m_input.Data));
            }
        }
    }

    [ClockedProcess]
    public class Softplus_2 : SimpleProcess
    {
        [InputBus]
        private ValueTransfer m_input;
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
    signal add_res : T_SYSTEM_FLOAT := (others => '0');
    signal ignore : std_logic;

    -- create_ip -name floating_point -vendor xilinx.com -library ip -version 7.1 -module_name softplus_fl_add
    -- set_property -dict [list CONFIG.Component_Name {softplus_fl_add} CONFIG.Add_Sub_Value {Add} CONFIG.Flow_Control {NonBlocking} CONFIG.Maximum_Latency {false} CONFIG.C_Latency {0} CONFIG.Has_RESULT_TREADY {false}] [get_ips softplus_fl_add]
    COMPONENT softplus_fl_add
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
                m_output_Value <= add_res;
            end if;
            FIN <= not RDY;
        end if;
    end process;

    adder : softplus_fl_add
    PORT MAP (
        s_axis_a_tvalid => index_Ready,
        s_axis_a_tdata => x""3f800000"",
        s_axis_b_tvalid => index_Ready,
        s_axis_b_tdata => m_input_Value,
        m_axis_result_tvalid => ignore,
        m_axis_result_tdata => add_res
    );
                ";
            }
        }
        [Ignore]
        private Renderer renderer = new Renderer();
        public override object CustomRenderer { get { return renderer; } }

        public Softplus_2(ValueTransfer input, IndexValue index, ValueTransfer output, Flag flush)
        {
            m_input = input ?? throw new ArgumentNullException(nameof(input));
            this.index = index ?? throw new ArgumentNullException(nameof(index));
            m_output = output ?? throw new ArgumentNullException(nameof(output));
            this.flush = flush ?? throw new ArgumentNullException(nameof(flush));
        }

        protected override void OnTick()
        {
            if (index.Ready == true)
            {
                SimulationOnly(() => m_output.value = (float)Math.Log(1 + m_input.value));
            }
        }
    }

}
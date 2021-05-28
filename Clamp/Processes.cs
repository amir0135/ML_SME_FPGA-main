using SME;
using SME.VHDL;
using Deflib;
using System;

namespace Clamp
{

    [ClockedProcess]
    public class Clamp : SimpleProcess
    {
        [InputBus]
        private SME.Components.SimpleDualPortMemory<float>.IReadResult m_input;

        [InputBus]
        private IndexValue index;

        [OutputBus]
        private ValueTransfer m_output;

        float min_val, max_val;

        // TODO husk at capture float ordentligt i trace!
        // TODO husk at capture float ordentligt i reset værdierne!
        public class Renderer : ICustomRenderer
        {
            public string IncludeRegion(RenderStateProcess renderer, int indentation)
            {
                return string.Empty;
            }

            public string BodyRegion(RenderStateProcess renderer, int indentation)
            {
                return @"
    signal min_val : T_SYSTEM_FLOAT := reset_min_val;
    signal max_val : T_SYSTEM_FLOAT := reset_min_val;
    signal was_less : std_logic_vector(7 downto 0);
    signal was_grea : std_logic_vector(7 downto 0);

    -- create_ip -name floating_point -vendor xilinx.com -library ip -version 7.1 -module_name fl_gt
    -- set_property -dict [list CONFIG.Component_Name {fl_gt} CONFIG.Operation_Type {Compare} CONFIG.C_Compare_Operation {Greater_Than} CONFIG.Flow_Control {NonBlocking} CONFIG.Maximum_Latency {false} CONFIG.C_Latency {0} CONFIG.A_Precision_Type {Single} CONFIG.C_A_Exponent_Width {8} CONFIG.C_A_Fraction_Width {24} CONFIG.Result_Precision_Type {Custom} CONFIG.C_Result_Exponent_Width {1} CONFIG.C_Result_Fraction_Width {0} CONFIG.C_Mult_Usage {No_Usage} CONFIG.Has_RESULT_TREADY {false} CONFIG.C_Rate {1}] [get_ips fl_gt]
    COMPONENT fl_gt
    PORT (
        s_axis_a_tvalid : IN STD_LOGIC;
        s_axis_a_tdata : IN STD_LOGIC_VECTOR(31 DOWNTO 0);
        s_axis_b_tvalid : IN STD_LOGIC;
        s_axis_b_tdata : IN STD_LOGIC_VECTOR(31 DOWNTO 0);
        m_axis_result_tvalid : OUT STD_LOGIC;
        m_axis_result_tdata : OUT STD_LOGIC_VECTOR(7 DOWNTO 0)
    );
    END COMPONENT;

    -- create_ip -name floating_point -vendor xilinx.com -library ip -version 7.1 -module_name fl_lt
    -- set_property -dict [list CONFIG.Component_Name {fl_lt} CONFIG.Operation_Type {Compare} CONFIG.C_Compare_Operation {Less_Than} CONFIG.Flow_Control {NonBlocking} CONFIG.Maximum_Latency {false} CONFIG.C_Latency {0} CONFIG.A_Precision_Type {Single} CONFIG.C_A_Exponent_Width {8} CONFIG.C_A_Fraction_Width {24} CONFIG.Result_Precision_Type {Custom} CONFIG.C_Result_Exponent_Width {1} CONFIG.C_Result_Fraction_Width {0} CONFIG.C_Mult_Usage {No_Usage} CONFIG.Has_RESULT_TREADY {false} CONFIG.C_Rate {1}] [get_ips fl_lt]
    COMPONENT fl_lt
    PORT (
        s_axis_a_tvalid : IN STD_LOGIC;
        s_axis_a_tdata : IN STD_LOGIC_VECTOR(31 DOWNTO 0);
        s_axis_b_tvalid : IN STD_LOGIC;
        s_axis_b_tdata : IN STD_LOGIC_VECTOR(31 DOWNTO 0);
        m_axis_result_tvalid : OUT STD_LOGIC;
        m_axis_result_tdata : OUT STD_LOGIC_VECTOR(7 DOWNTO 0)
    );
    END COMPONENT;
begin

    process (
        CLK,
        RST
    )
    begin
        if RST = '1' then
            m_output_value <= (others => '0');
            min_val <= reset_min_val;
            max_val <= reset_max_val;
            FIN <= '0';
        elsif rising_edge(CLK) then
            if index_Ready = '1' then
                if was_less(0) = '1' then
                    m_output_value <= min_val;
                elsif was_grea(0) = '1' then
                    m_output_value <= max_val;
                else
                    m_output_value <= m_input_Data;
                end if;
            end if;
            FIN <= not RDY;
        end if;
    end process;

    greater_than : fl_gt
    PORT MAP (
        s_axis_a_tvalid => index_Ready,
        s_axis_a_tdata => m_input_Data,
        s_axis_b_tvalid => index_Ready,
        s_axis_b_tdata => max_val,
        --m_axis_result_tvalid => m_axis_result_tvalid,
        m_axis_result_tdata => was_grea
    );

    less_than : fl_lt
    PORT MAP (
        s_axis_a_tvalid => index_Ready,
        s_axis_a_tdata => m_input_Data,
        s_axis_b_tvalid => index_Ready,
        s_axis_b_tdata => min_val,
        --m_axis_result_tvalid => m_axis_result_tvalid,
        m_axis_result_tdata => was_less
    );

";
            }
        }
        [Ignore]
        private Renderer renderer = new Renderer();
        public override object CustomRenderer { get { return renderer; } }

        public Clamp(SME.Components.SimpleDualPortMemory<float>.IReadResult input, IndexValue index,  ValueTransfer output, float min_val, float max_val)
        {
            m_input = input ?? throw new ArgumentNullException(nameof(input));
            this.index = index ?? throw new ArgumentNullException(nameof(index));
            m_output = output ?? throw new ArgumentNullException(nameof(output));
            this.min_val = min_val;
            this.max_val = max_val ;
        }

        protected override void OnTick()
        {
            if (index.Ready == true)
            {
                if (m_input.Data < min_val)
                    m_output.value = min_val;
                else if (m_input.Data > max_val)
                    m_output.value = max_val;
                else
                    m_output.value = m_input.Data;
            }
        }
    }

}

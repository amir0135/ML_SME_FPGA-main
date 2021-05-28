
using System.Threading.Tasks;
using SME;
using SME.VHDL;
using System.Globalization;
using System.IO;
using System.Linq;
using System;

namespace Deflib
{

    // Forms the data into 1,2 or 3D
    // input: takes ind the dimensions of data
    // output: indexcontrol that makes sure to hold the size
    public class TestIndexSim : SimulationProcess
    {
        [OutputBus]
        private IndexControl control;

        int Row, Col, Dim;

        public SME.Components.SimpleDualPortMemory<float> ram;

        public TestIndexSim(IndexControl control, int Row)
        {
            this.control = control;
            this.Row = Row;
            this.Col = 1;
            this.Dim = 1;
        }

        public TestIndexSim(IndexControl control, int Row, int Col)
        {
            this.control = control;
            this.Row = Row;
            this.Col = Col;
            this.Dim = 1;
        }

        public TestIndexSim(IndexControl control, int Row, int Col, int Dim)
        {
            this.control = control;
            this.Row = Row;
            this.Col = Col;
            this.Dim = Dim;
        }

        public override async Task Run()
        {
            control.OffsetA = 0;
            control.OffsetB = 0;
            control.OffsetC = 0;

            await ClockAsync();

            control.Ready = true;
            control.Height = Row;
            control.Width = Col;
            control.Dim = Dim;

            await ClockAsync();

            control.Ready = false;
        }
    }

    public class Dataload : SimulationProcess
    {
        [InputBus]
        private IndexValue index;

        [OutputBus]
        private SME.Components.SimpleDualPortMemory<float>.IWriteControl output;

        int size;
        string CSVfile;
        public float[] A;

        public Dataload(int size, string CSVfile, IndexValue value, SME.Components.SimpleDualPortMemory<float>.IWriteControl output)
        {
            this.size = size;
            this.CSVfile = CSVfile;
            index = value;
            this.output = output;

            A = File.ReadAllLines(CSVfile)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => float.Parse(x.Trim(), CultureInfo.InvariantCulture))
                .Take(size)
                .ToArray();
        }

        public override async System.Threading.Tasks.Task Run()
        {
            while (true)
            {
                await ClockAsync();

                output.Enabled = index.Ready;

                if (index.Ready)
                {
                    output.Address = index.Addr;
                    output.Data = A[index.Addr];
                }
            }
        }
    }

    [ClockedProcess]
    public class Mul : SimpleProcess
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

    -- create_ip -name floating_point -vendor xilinx.com -library ip -version 7.1 -module_name deflib_fl_mul
    -- set_property -dict [list CONFIG.Component_Name {deflib_fl_mul} CONFIG.Operation_Type {Multiply} CONFIG.Flow_Control {NonBlocking} CONFIG.Maximum_Latency {false} CONFIG.C_Latency {0} CONFIG.A_Precision_Type {Single} CONFIG.C_A_Exponent_Width {8} CONFIG.C_A_Fraction_Width {24} CONFIG.Result_Precision_Type {Single} CONFIG.C_Result_Exponent_Width {8} CONFIG.C_Result_Fraction_Width {24} CONFIG.C_Mult_Usage {Full_Usage} CONFIG.Has_RESULT_TREADY {false} CONFIG.C_Rate {1}] [get_ips deflib_fl_mul]
    COMPONENT deflib_fl_mul
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

    diver : deflib_fl_mul
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

        public Mul(IndexValue inputpipe, SME.Components.SimpleDualPortMemory<float>.IReadResult inputA, SME.Components.SimpleDualPortMemory<float>.IReadResult inputB, ValueTransfer output)
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
    public class MulIndex : SimpleProcess
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
        private int i, j;
        private int width, height;
        private bool Aready = false, Bready = false;
        private bool started = false;

        public MulIndex(IndexControl controlA, IndexControl controlB, IndexValue outputA, IndexValue outputB, IndexControl controlout, IndexControl control)
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

                outputA.Addr = i * width * height + j;
                outputB.Addr = i * width * height + j;

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
                Bready |= controlB.Ready;

                if (Aready && Bready)
                {
                    Aready = false;
                    Bready = false;

                    running = true;
                    width = control.Width;
                    height = control.Height;

                    i = j = 0;
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

    [ClockedProcess]
    public class SigIndex : SimpleProcess
    {
        [InputBus]
        private IndexControl control;
        [InputBus]
        private IndexControl controlA;

        [OutputBus]
        private IndexValue output;
        [OutputBus]
        private IndexControl controlout;

        private bool running = false;
        private int i = 0;
        private int width, height;
        private bool Aready = false;
        private bool started = false;

        public SigIndex(IndexControl controlA, IndexValue output, IndexControl controlout, IndexControl control)
        {
            this.controlA = controlA;
            this.controlout = controlout;
            this.output = output;
            this.control = control;
        }

        protected override void OnTick()
        {
            if (running == true)
            {
                started = true;
                output.Ready = true;

                output.Addr = i;

                i++;

                if (i >= width)
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
                    output.Ready = true;
                    output.Addr = controlA.OffsetA;
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
                    output.Ready = false;
                }
            }
        }
    }

    [ClockedProcess]
    public class SigIndex_flag : SimpleProcess
    {
        [InputBus]
        private IndexControl controlA;
        [InputBus]
        private IndexControl control;

        [OutputBus]
        private IndexValue output;
        [OutputBus]
        private IndexControl controlout;
        [OutputBus]
        private Flag flush;

        private bool running = false;
        private int i = 0;
        private int width, height;
        private bool Aready = false;
        private bool started = false;

        public SigIndex_flag(IndexControl controlA, IndexValue output, IndexControl controlout, IndexControl control, Flag flush)
        {
            this.controlA = controlA;
            this.controlout = controlout;
            this.output = output;
            this.control = control;
            this.flush = flush;
        }

        protected override void OnTick()
        {
            flush.flg = false;
            if (running == true)
            {
                started = true;
                output.Ready = true;

                output.Addr = i;
                i++;
                flush.flg = true;

                if (i >= width)
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
                    output.Ready = true;
                    output.Addr = controlA.OffsetA;
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
                    output.Ready = false;
                }
            }
        }
    }

}
using SME;
using Deflib;

namespace Load_data
{

    [ClockedProcess]
    public class Index : SimpleProcess
    {
        [InputBus]
        private IndexControl control;

        [OutputBus]
        private IndexValue output;
        [OutputBus]
        private IndexControl controlout;

        private bool running = false;
        private bool started = false;
        private int i = 0;
        private int Addr;
        private int width;
        private int height;
        private int dim;

        public Index(IndexControl control, IndexValue output, IndexControl controlout)
        {
            this.control = control;
            this.output = output;
            this.controlout = controlout;
        }

        protected override void OnTick()
        {
            if (running == true)
            {
                started = true;

                i++;
                output.Addr = i;

                if (i >= width*height*dim)
                {
                    running = false;
                    output.Ready = false;
                }
            }
            else
            {
                if (control.Ready == true)
                {
                    started = true;
                    running = true;
                    width = control.Width;
                    height = control.Height;
                    dim = control.Dim;
                    i = 0;
                    Addr = control.OffsetA;

                    output.Ready = true;
                    output.Addr = Addr;
                }
                else
                {
                    if (started == true)
                    {
                        controlout.Ready = true;
                        controlout.Height = control.Height;
                        controlout.Width = control.Width;
                        controlout.Dim = control.Dim;
                        controlout.OffsetA = control.OffsetA;
                        controlout.OffsetB = control.OffsetB;
                        controlout.OffsetC = control.OffsetC;
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
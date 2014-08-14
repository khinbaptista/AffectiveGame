using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AffectiveGame.Comparison
{
    class Manager
    {
        private static Recorder recorder;
        private bool working = true;
        private bool started;

        public Manager()
        {
            started = false;
        }

        public void startProcessing()
        {
            if (!started && working)
            {
                recorder = new Recorder();
                started = true;
            }
        }

        public void stopProcessing()
        {
            if (started && working)
            {
                recorder.stopComparison();
                started = false;

                recorder = null;
            }
        }

        public bool getActionValue()
        {
            if (started && working)
                return recorder.getAction();
            else
                return false;
        }

        public double getValue()
        {
            if(started && working)
                return recorder.Value;
            else
            {
                return 0;
            }
        }
    }
}

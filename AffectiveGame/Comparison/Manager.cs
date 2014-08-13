using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AffectiveGame.Comparison
{
    class Manager
    {
        private static Recorder recorder;
        private bool started;

        public Manager()
        {
            started = false;
        }

        public void startProcessing()
        {
            if (!started)
            {
                recorder = new Recorder();
                started = true;
            }
        }

        public void stopProcessing()
        {
            if (started)
            {
                recorder.stopComparison();
                started = false;

                recorder = null;
            }
        }

        public bool getActionValue()
        {
            if (started)
                return recorder.getAction();
            else
                return false;
        }

        public double getValue()
        {
            if(started)
                return recorder.Value;
            else
            {
                return 0;
            }
        }
    }
}

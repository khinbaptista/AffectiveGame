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
            }
        }

        public bool getActionValue()
        {
            return recorder.getAction();
        }

        public double getValue()
        {
            return recorder.Value;
        }
    }
}

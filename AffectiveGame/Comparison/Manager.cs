using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AffectiveGame.Comparison
{
    class Manager
    {
        private static Recorder recorder;
        private bool working = false;
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

        public soundState getActionValue()
        {
            if (started && working)
                return recorder.getAction();
            else
                return soundState.NONE;
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

        public double strokeValue()
        {
            if (started && working)
                return recorder.strokeValue;
            else
            {
                return 0;
            }
        }
    }
}

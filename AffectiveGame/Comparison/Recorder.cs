using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Timers;

namespace AffectiveGame.Comparison
{
    public class Recorder
    {
        public static double[] leftHowl;
        public static double[] rightAudio;
        public static double[] leftCompared;
        public static double[] rightCompared;
        public static double[] leftStroke;
        private static string howlFile;
        private static string strokeFile;
        static Program program;
        static Correlation crossCorr;
        public static int offset;
        private static RealTime recorder;
        private static Stopwatch stopwatchProcess;
        //private static Stopwatch stopwatchRecord = new Stopwatch();
        private static Stopwatch stopwatchTotal;
        private static soundState comparisonState = soundState.NONE;
        private static double howlResult;
        private static double strokeResult;
        private static Timer recordWindow;
        private static bool lastHowl = false;

        public double Value
        {
            get { return howlResult; }
        }

        public double strokeValue
        {
            get { return strokeResult; }
        }

        public Recorder()
        {
            recorder = new RealTime();
            program = new Program();
            stopwatchProcess = new Stopwatch();
            stopwatchTotal = new Stopwatch();
            recordWindow = new System.Timers.Timer(500);
            howlFile = @"howling\002.wav";
            strokeFile = @"stroking\012.wav";

            if (recorder.checkMic())
            {
                program.openWav(howlFile, null, out leftHowl, out rightAudio);
                program.openWav(strokeFile, null, out leftStroke, out rightAudio);
                //stopwatchTotal.Start();
                //Console.WriteLine("Recording...");
                //stopwatchRecord.Start();
                recorder.startRecording();

                recordWindow.Elapsed += OnTimedEvent;
                recordWindow.Enabled = true;

                //Console.ReadKey();
            }
        }

        public void stopComparison()
        {
            //stopwatchTotal.Stop();
            recorder.stopRecording();
            recordWindow.Enabled = false;
        }

        public soundState getAction()
        {
            return comparisonState;
        }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            recorder.stopRecording();
            //stopwatchRecord.Stop();

            //Console.WriteLine("Sound recorded. Processing...");

            //stopwatchProcess.Start();
            program.openWav(null, recorder.wavMem(), out leftCompared, out rightCompared);

            recorder.disposeStream();

            double[] result;

            if (leftCompared.Length != 0)
            {
                if (lastHowl)
                {
                    alglib.corrr1d(leftStroke, leftStroke.Length, leftCompared, leftCompared.Length, out result);
                    crossCorr = new Correlation(result, leftStroke, leftCompared, out offset, out strokeResult);
                    lastHowl = false;
                }
                else
                {
                    alglib.corrr1d(leftHowl, leftHowl.Length, leftCompared, leftCompared.Length, out result);
                    crossCorr = new Correlation(result, leftHowl, leftCompared, out offset, out howlResult);
                    lastHowl = true;
                }
            }
            else
                Console.WriteLine();

            /*stopwatchProcess.Start();
            program.openWav(null, recorder.wavMem(), out leftCompared, out rightCompared);

            recorder.disposeStream();

            double[] result;

            if (leftCompared.Length != 0)
            {
                    alglib.corrr1d(leftStroke, leftStroke.Length, leftCompared, leftCompared.Length, out result);
                    crossCorr = new Correlation(result, leftStroke, leftCompared, out offset, out strokeResult);

                    result = null;

                    alglib.corrr1d(leftHowl, leftHowl.Length, leftCompared, leftCompared.Length, out result);
                    crossCorr = new Correlation(result, leftHowl, leftCompared, out offset, out howlResult);
            }
            else
                Console.WriteLine();*/

            if ((strokeResult < 0.6) && (howlResult < 0.4))
            {
                comparisonState = soundState.NONE;
            }
            else if (strokeResult > howlResult)
            {
                comparisonState = soundState.STROKING;
            }
            else
            {
                comparisonState = soundState.HOWLING;
            }

            //stopwatchProcess.Stop();

            //Console.WriteLine("Value: " + value);

            //Console.WriteLine("Time elapsed recording: " + recorder.getStopWatchRecord() + " ms");
            //Console.WriteLine("Time elapsed processing: " + stopwatchProcess.ElapsedMilliseconds + " ms");
            //Console.WriteLine("Total time elapsed: " + stopwatchTotal.ElapsedMilliseconds + " ms\r\n");

            //recorder.resetSWRecord();
            //stopwatchProcess.Reset();

            //Console.WriteLine("Recording...");
            recorder.startRecording();
            //stopwatchRecord.Start();
        }
    }
}

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
        public static double[] leftAudio;
        public static double[] rightAudio;
        public static double[] leftCompared;
        public static double[] rightCompared;
        private static string soundName = @"stroking\snap.wav";
        static Program program = new Program();
        static Correlation crossCorr;
        public static int offset;
        private static RealTime recorder = new RealTime();
        private static Stopwatch stopwatchProcess = new Stopwatch();
        //private static Stopwatch stopwatchRecord = new Stopwatch();
        private static Stopwatch stopwatchTotal = new Stopwatch();
        private static bool actionValue = false;

        private static Timer recordWindow = new System.Timers.Timer(500);

        public Recorder()
        {
            if (recorder.checkMic())
            {
                program.openWav(soundName, null, out leftAudio, out rightAudio);
                stopwatchTotal.Start();
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
            stopwatchTotal.Stop();
            recordWindow.Enabled = false;
        }

        public bool getAction()
        {
            return actionValue;
        }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            recorder.stopRecording();
            //stopwatchRecord.Stop();

            //Console.WriteLine("Sound recorded. Processing...");

            stopwatchProcess.Start();
            program.openWav(null, recorder.wavMem(), out leftCompared, out rightCompared);

            recorder.disposeStream();

            double value;
            double[] result;
            alglib.corrr1d(leftAudio, leftAudio.Length, leftCompared, leftCompared.Length, out result);

            crossCorr = new Correlation(result, leftAudio, leftCompared, out offset, out value);

            stopwatchProcess.Stop();

            //Console.WriteLine("Value: " + value);

            if (value > 0.4)
            {
                //Console.WriteLine("Action: True");
                actionValue = true;
            }
            else
            {
                //Console.WriteLine("Action: False");
                actionValue = false;
            }

            //Console.WriteLine("Time elapsed recording: " + recorder.getStopWatchRecord() + " ms");
            //Console.WriteLine("Time elapsed processing: " + stopwatchProcess.ElapsedMilliseconds + " ms");
            //Console.WriteLine("Total time elapsed: " + stopwatchTotal.ElapsedMilliseconds + " ms\r\n");

            recorder.resetSWRecord();
            stopwatchProcess.Reset();

            //Console.WriteLine("Recording...");
            recorder.startRecording();
            //stopwatchRecord.Start();
        }
    }
}

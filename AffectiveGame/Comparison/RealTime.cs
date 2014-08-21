extern alias Mic;

using System;
using System.IO;
using System.Threading;
using NAudio.Wave;
using Mic::Microsoft.Xna.Framework.Audio;
using System.Timers;
using System.Diagnostics;

namespace AffectiveGame.Comparison
{
    public class RealTime
    {
        public WaveInEvent waveSource = null;
        public WaveFileWriter waveFile = null;
        public MemoryStream audioStream;
        Microphone mic = Microphone.Default;
        private bool hasStopped = false;
        private static Stopwatch stopwatchRecord = new Stopwatch();

        private static object syncLock = new object();
        private static object writeLock = new object();
        private static object soundLock = new object();

        public bool checkMic()
        {
            if (mic == null)
            {
                return false; // No microphone is attached to the device
            }
            return true;
        }

        public bool startRecording()
        {
            waveSource = new WaveInEvent();
            waveSource.WaveFormat = new WaveFormat(44100, 1);

            waveSource.DataAvailable += new EventHandler<WaveInEventArgs>(waveSource_DataAvailable);
            waveSource.RecordingStopped += new EventHandler<StoppedEventArgs>(waveSource_RecordingStopped);
            
            audioStream = new MemoryStream();
            waveFile = new WaveFileWriter(audioStream, waveSource.WaveFormat);

            stopwatchRecord.Start();
            //Monitor.Enter(syncLock);
            hasStopped = false;
            waveSource.StartRecording();
            return true;
        }

        public bool stopRecording()
        {
            Monitor.Enter(soundLock);
            if (waveSource != null)
            {
                waveSource.StopRecording();
                //Monitor.Exit(syncLock);
                stopwatchRecord.Stop();
            }
            Monitor.Exit(soundLock);
            Monitor.Enter(syncLock);
            
            return true;
        }

        public MemoryStream wavMem()
        {
            return audioStream;
        }

        void waveSource_DataAvailable(object sender, WaveInEventArgs e)
        {
            Monitor.Enter(writeLock);
            if (waveFile != null)
            {
                waveFile.Write(e.Buffer, 0, e.BytesRecorded);
            }
            Monitor.Exit(writeLock);
        }

        public void disposeStream()
        {
            if (waveFile != null)
            {
                waveFile.Dispose();
                waveFile = null;
            }
            if (waveSource != null)
            {
                waveSource.Dispose();
                waveSource = null;
            }
            Monitor.Exit(syncLock);
        }

        void waveSource_RecordingStopped(object sender, StoppedEventArgs e)
        {
            hasStopped = true;
        }

        public float getStopWatchRecord()
        {
            return stopwatchRecord.ElapsedMilliseconds;
        }

        public void resetSWRecord()
        {
            stopwatchRecord.Reset();
        }
    }
}

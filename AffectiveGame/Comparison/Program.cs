﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NAudio.Wave;

namespace AffectiveGame.Comparison
{
    class Program
    {
        // convert two bytes to one double in the range -1 to 1
        static double bytesToDouble(byte firstByte, byte secondByte)
        {
            // convert two bytes to one short (little endian)
            short s = (short)((secondByte << 8) | firstByte);
            // convert to range from -1 to (just below) 1
            return s / 32768.0;
        }

        // Returns left and right double arrays. 'right' will be null if sound is mono.
        public void openWav(string filename, MemoryStream auxArray, out double[] left, out double[] right)
        {
            byte[] wav;
            if (auxArray != null)
            {
                wav = auxArray.ToArray();
                //Console.WriteLine(wav.Length);
            }
            else
            {
                wav = File.ReadAllBytes(filename);
            }

            //Console.WriteLine(wav.Length);

            // Determine if mono or stereo
            int channels = wav[22];     // Forget byte 23 as 99.999% of WAVs are 1 or 2 channels

            // Get past all the other sub chunks to get to the data subchunk:
            int pos = 12;   // First Subchunk ID from 12 to 16

            // Keep iterating until we find the data chunk (i.e. 64 61 74 61 ...... (i.e. 100 97 116 97 in decimal))
            while (!(wav[pos] == 100 && wav[pos + 1] == 97 && wav[pos + 2] == 116 && wav[pos + 3] == 97))
            {
                pos += 4;
                int chunkSize = wav[pos] + wav[pos + 1] * 256 + wav[pos + 2] * 65536 + wav[pos + 3] * 16777216;
                pos += 4 + chunkSize;
            }
            pos += 8;

            // Pos is now positioned to start of actual sound data.
            int samples = (wav.Length - pos) / 2;     // 2 bytes per sample (16 bit sound mono)
            if (channels == 2)
            {
                samples /= 2;        // 4 bytes per sample (16 bit stereo)
            }

            // Allocate memory (right will be null if only mono sound)
            left = new double[samples];
            //left = new double[size];
            if (channels == 2)
            {
                right = new double[samples];
                //right = new double[size];
            }
            else
            {
                right = null;
            }

            // Write to double array/s:
            int i = 0;
            while (pos < wav.Length)
            //while (i < size)
            {
                left[i] = bytesToDouble(wav[pos], wav[pos + 1]);
                pos += 2;
                if (channels == 2)
                {
                    right[i] = bytesToDouble(wav[pos], wav[pos + 1]);
                    pos += 2;
                }
                i++;
            }
        }
    }
}

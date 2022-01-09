using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace ValueNoise
{
    static class ValueNoise
    {

        // Noise function for one position
        public static double Noise(double pos)
        {
            // Floored point at pos
            double low = Math.Floor(pos);
            // (High is only one above each)

            // Result of RNG around pos
            double y1 = RNG.GetDouble(BitConverter.GetBytes(low));
            double y2 = RNG.GetDouble(BitConverter.GetBytes(low + 1));

            // Return result of lerp
            return Lerp(y1, y2, Fade(pos - low));
        }

        // Noise function for a 2D position
        public static double Noise2D(double x1, double x2)
        {
            // Floored point at pos
            double low1 = Math.Floor(x1);
            double low2 = Math.Floor(x2);

            double y11 = RNG.GetDouble(RNG.CombineAll(new double[] { low1, low2 }));
            double y21 = RNG.GetDouble(RNG.CombineAll(new double[] { low1 + 1, low2 }));
            double y12 = RNG.GetDouble(RNG.CombineAll(new double[] { low1, low2 + 1 }));
            double y22 = RNG.GetDouble(RNG.CombineAll(new double[] { low1 + 1, low2 + 1 }));

            return Lerp2D(y11, y21, y12, y22, x1 - low1, x2 - low2);
        }

        // Noise function for a 3D position
        public static double Noise3D(double x1, double x2, double x3)
        {
            // Floored point at pos
            double low1 = Math.Floor(x1);
            double low2 = Math.Floor(x2);
            double low3 = Math.Floor(x3);

            // RNG at corners
            double[] y = new double[8];
            {
                y[0] = RNG.GetDouble(RNG.CombineAll(new double[] { low1, low2, low3 }));
                y[1] = RNG.GetDouble(RNG.CombineAll(new double[] { low1 + 1, low2, low3 }));
                y[2] = RNG.GetDouble(RNG.CombineAll(new double[] { low1, low2 + 1, low3 }));
                y[3] = RNG.GetDouble(RNG.CombineAll(new double[] { low1 + 1, low2 + 1, low3 }));
                y[4] = RNG.GetDouble(RNG.CombineAll(new double[] { low1, low2, low3 + 1 }));
                y[5] = RNG.GetDouble(RNG.CombineAll(new double[] { low1 + 1, low2, low3 + 1 }));
                y[6] = RNG.GetDouble(RNG.CombineAll(new double[] { low1, low2 + 1, low3 + 1 }));
                y[7] = RNG.GetDouble(RNG.CombineAll(new double[] { low1 + 1, low2 + 1, low3 + 1 }));
            }

            // Linear interpolate points
            return Lerp3D(y, new double[] { x1 - low1, x2 - low2, x3 - low3 });
        }

        // Linear interpolation of a single line
        static double Lerp(double y1, double y2, double x)
        {
            return (y1 * (1 - x)) + (y2 * x);
        }

        // Linear interpolate 2 lines, then linear interpolate result
        static double Lerp2D(double y11, double y21, double y12, double y22, double x1, double x2)
        {
            return Lerp(Lerp(y11, y12, x2), Lerp(y21, y22, x2), x1);
        }

        // Linear interpolate 2 lines twice, then interpolate result
        static double Lerp3D(double[] y, double[] x)
        {
            return Lerp(
                    Lerp2D(
                        y[0], y[1], y[2], y[3], 
                        x[0], x[1]), 
                    Lerp2D(
                        y[4], y[5], y[6], y[7], 
                        x[0], x[1]), 
                    x[2]);
        }

        // Shifting weight distribution
        static double Fade(double x)
        {
            return ((6 * Math.Pow(x, 5)) - (15 * Math.Pow(x, 4)) + (10 * Math.Pow(x, 3)));
        }
    }
	
	
    static class RNG
    {
        static SHA256 sha256 = SHA256.Create();

        // Seed for RNG
        public static long seed = 0;

        // Get double from input
        public static double GetDouble(byte[] buffer)
        {
            List<byte[]> bytes = new List<byte[]>();
            {

                // Add seed
                if (seed != 0)
                    bytes.Add(BitConverter.GetBytes(seed));
                // Add buffer
                bytes.Add(buffer);
            }

            // SHA256 byte array
            byte[] hash = sha256.ComputeHash(Combine(bytes));

            // Shorten to 4 bytes
            hash = Shorten(hash);

            // Get double between (0, 1)
            double value = Math.Abs(ToInt(hash) / (double)Int32.MaxValue);

            return value;
        }

        // Turns byte array to 8 byte array by XOR'ing overflow
        public static byte[] Shorten(byte[] bytes)
        {
            byte[] output = new byte[4];

            uint curr = 0;
            foreach (byte b in bytes)
            {
                output[curr] = (byte)(output[curr] ^ b);

                curr = (uint)((curr + 1) % 4);
            }

            return output;
        }

        // Turn 8 bytes to double
        public static int ToInt(byte[] bytes)
        {
            return BitConverter.ToInt32(bytes, 0);
        }

        public static byte[] Combine(List<byte[]> bytes)
        {
            int count = 0;
            foreach (byte[] b in bytes)
            {
                count += b.Length;
            }

            byte[] all = new byte[count];

            count = 0;
            foreach (byte[] bA in bytes)
            {

                foreach (byte b in bA)
                {
                    all[count] ^= b;

                    count++;
                }
            }

            return all;
        }

        public static byte[] CombineAll(double[] nums)
        {
            List<byte[]> bytes = new List<byte[]>();

            foreach (double d in nums)
            {
                bytes.Add(BitConverter.GetBytes(d));
            }

            return Combine(bytes);
        }
    }

}


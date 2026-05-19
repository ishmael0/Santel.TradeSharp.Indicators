namespace HashingApp
{
    public class Runner
    {
        static void Main()
        {
            var rpc = new BitcoinRPC(
                "http://127.0.0.1:8332",
                "user",
                "pass"
            );

            Console.WriteLine("Requesting block template...");
            var tmpl = rpc.Call("getblocktemplate", new object[]
            {
            new { rules = new [] { "segwit" } }
            });

            string prevHash = tmpl.GetProperty("previousblockhash").GetString();
            string bitsHex = tmpl.GetProperty("bits").GetString();
            int version = tmpl.GetProperty("version").GetInt32();
            uint time = (uint)tmpl.GetProperty("curtime").GetInt64();

            byte[] prevBlockLE = HexToLE(prevHash);
            byte[] bits = HexToBytes(bitsHex);

            byte[] merkle = new byte[32]; // واقعی: باید coinbase بسازید + مرکل‌روت
                                          // فعلاً صفر تا ساخت قطعه بعدی را بخواهید

            byte[] hdr = new byte[80];
            Buffer.BlockCopy(ToLE(version), 0, hdr, 0, 4);
            Buffer.BlockCopy(prevBlockLE, 0, hdr, 4, 32);
            Buffer.BlockCopy(merkle, 0, hdr, 36, 32);
            Buffer.BlockCopy(ToLE(time), 0, hdr, 68, 4);
            Buffer.BlockCopy(bits, 0, hdr, 72, 4);
            // nonce = 0..threads handles it

            byte[] target = TargetFromBits(bits);

            int THREADS = Environment.ProcessorCount;
            new Miner(hdr, target, THREADS).Start();
        }

        static byte[] TargetFromBits(byte[] bits)
        {
            int exponent = bits[0];
            uint mantissa = (uint)(bits[1] << 16 | bits[2] << 8 | bits[3]);

            byte[] target = new byte[32];
            int offset = exponent - 3;

            if (offset < 0) return target;

            target[31 - offset] = (byte)(mantissa);
            target[30 - offset] = (byte)(mantissa >> 8);
            target[29 - offset] = (byte)(mantissa >> 16);

            return target;
        }

        static byte[] HexToBytes(string hex)
        {
            hex = hex.Replace(" ", "").ToLower();
            byte[] r = new byte[hex.Length / 2];
            for (int i = 0; i < r.Length; i++)
                r[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            return r;
        }

        static byte[] HexToLE(string hex)
        {
            byte[] b = HexToBytes(hex);
            Array.Reverse(b);
            return b;
        }

        static byte[] ToLE(uint x)
        {
            return new byte[]
            {
            (byte)x,
            (byte)(x>>8),
            (byte)(x>>16),
            (byte)(x>>24)
            };
        }

        static byte[] ToLE(int x) => ToLE((uint)x);
    }
}

public class Miner
{
    private volatile bool found = false;
    private readonly object lockObj = new object();
    private readonly byte[] headerTemplate;
    private readonly byte[] target;
    private readonly int threadCount;

    public Miner(byte[] headerTemplate, byte[] target, int threads)
    {
        this.headerTemplate = headerTemplate;
        this.target = target;
        this.threadCount = threads;
    }

    public void Start()
    {
        for (int t = 0; t < threadCount; t++)
        {
            int id = t;
            new Thread(() => MineThread(id)).Start();
        }
    }

    private void MineThread(int id)
    {
        byte[] hdr = new byte[80];
        Buffer.BlockCopy(headerTemplate, 0, hdr, 0, 80);

        ulong start = (ulong)id * 200_000_000UL;
        ulong end = start + 200_000_000UL;

        for (ulong nonce = start; nonce < end && !found; nonce++)
        {
            hdr[76] = (byte)(nonce);
            hdr[77] = (byte)(nonce >> 8);
            hdr[78] = (byte)(nonce >> 16);
            hdr[79] = (byte)(nonce >> 24);

            byte[] hash = SHA256Custom.DoubleSHA256(hdr);

            if (Compare(hash, target) < 0)
            {
                lock (lockObj)
                {
                    if (!found)
                    {
                        found = true;
                        Console.WriteLine($"Thread {id} FOUND BLOCK");
                        Console.WriteLine($"NONCE = {nonce}");
                        Console.WriteLine($"HASH  = {Hex(hash)}");
                    }
                }
                return;
            }
        }
    }

    private int Compare(byte[] h, byte[] t)
    {
        for (int i = 31; i >= 0; i--)
        {
            if (h[i] < t[i]) return -1;
            if (h[i] > t[i]) return +1;
        }
        return 0;
    }

    private string Hex(byte[] b)
    {
        char[] c = new char[b.Length * 2];
        int p = 0;
        foreach (byte x in b)
        {
            int hi = (x >> 4) & 0xF;
            int lo = x & 0xF;
            c[p++] = (char)(hi < 10 ? hi + '0' : hi - 10 + 'a');
            c[p++] = (char)(lo < 10 ? lo + '0' : lo - 10 + 'a');
        }
        return new string(c);
    }
}

using System;

namespace GostCrypto
{
    public static class Gost
    {
        public static byte[] Encrypt(byte[] key, byte[] data) => Run(key, data, false);

        public static byte[] Decrypt(byte[] key, byte[] data) => Run(key, data, true);

        private static byte[] Run(byte[] key, byte[] data, bool isDecrypting)
        {
            var subkeys = GenerateKeys(key);
            var result = new byte[data.Length];
            var block = new byte[8];

            for (int i = 0; i < data.Length / 8; i++)
            {
                Array.Copy(data, 8 * i, block, 0, 8);

                Array.Copy(ProcessBlock(block, subkeys, isDecrypting), 0, result, 8 * i, 8);
            }

            return result;
        }

        private static byte[] ProcessBlock(byte[] block, uint[] keys, bool isDecrypting)
        {
            var N1 = BitConverter.ToUInt32(block, 0);
            var N2 = BitConverter.ToUInt32(block, 4);

            for (var i = 0; i < 32; i++)
            {
                var keyIndex = isDecrypting 
                    ? i < 8 ? (i % 8) : (7 - i % 8)
                    : i < 24 ? (i % 8) : (7 - i % 8);

                var s = (N1 + keys[keyIndex]) % uint.MaxValue;
                s = Substitution(s);
                s = (s << 11) | (s >> 21);
                s ^= N2;

                if (i < 31)
                {
                    N2 = N1;
                    N1 = s;
                }
                else
                {
                    N2 = s;
                }
            }

            var output = new byte[8];
            var N1buff = BitConverter.GetBytes(N1);
            var N2buff = BitConverter.GetBytes(N2);

            for (var i = 0; i < 4; i++)
            {
                output[i] = N1buff[i];
                output[4 + i] = N2buff[i];
            }

            return output;
        }

        private static uint[] GenerateKeys(byte[] key)
        {
            if (key.Length != 32)
            {
                throw new Exception("Key length must be 256 bits");
            }

            var subkeys = new uint[8];

            for (var i = 0; i < 8; i++)
            {
                subkeys[i] = BitConverter.ToUInt32(key, 4 * i);
            }

            return subkeys;
        }

        private static uint Substitution(uint value)
        {
            uint output = 0;

            for (int i = 0; i < 8; i++)
            {
                var temp = (byte)((value >> (4 * i)) & 0x0f);
                temp = Constants.SubstitutionBox[i][temp];
                output |= (uint)temp << (4 * i);
            }

            return output;
        }
    }
}

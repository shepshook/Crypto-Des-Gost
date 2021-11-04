namespace DesCrypto
{
    public static class TripleDes
    {
        public static byte[] Encrypt(byte[] key1, byte[] key2, byte[] message) =>
            Des.Encrypt(key1, Des.Decrypt(key2, Des.Encrypt(key1, message)));

        public static byte[] Decrypt(byte[] key1, byte[] key2, byte[] message) =>
            Des.Decrypt(key1, Des.Encrypt(key2, Des.Decrypt(key1, message)));
    }
}
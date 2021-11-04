namespace DesCrypto
{
    public static class DoubleDes
    {
        public static byte[] Encrypt(byte[] key1, byte[] key2, byte[] message) =>
            Des.Encrypt(key2, Des.Encrypt(key1, message));

        public static byte[] Decrypt(byte[] key1, byte[] key2, byte[] message) =>
            Des.Decrypt(key1, Des.Decrypt(key2, message));
    }
}
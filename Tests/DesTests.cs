using System.Diagnostics;
using System.IO;
using System.Text;
using DesCrypto;
using Xunit;
using FluentAssertions;
using System;

namespace Tests
{
    public class DesTests
    {
        private const string TestFilePath = "des-test-data.txt";

        public DesTests()
        {
            var text = Faker.Lorem.Paragraph();

            Debug.Print($"Generated text: \n{text}\n");

            File.WriteAllText(TestFilePath, text);
        }

        [Fact]
        public void TestDoubleDesSourceEqualsDecrypted()
        {
            var source = File.ReadAllBytes(TestFilePath);

            var key1 = Encoding.UTF8.GetBytes("key-one");
            var key2 = Encoding.UTF8.GetBytes("key-two");

            var encrypted = DoubleDes.Encrypt(key1, key2, source);
            
            var decrypted = DoubleDes.Decrypt(key1, key2, encrypted);

            source.Should().BeEquivalentTo(decrypted);
        }
        
        [Fact]
        public void TestTripleDesSourceEqualsDecrypted()
        {
            var source = File.ReadAllBytes(TestFilePath);

            var key1 = Encoding.UTF8.GetBytes("key-one");
            var key2 = Encoding.UTF8.GetBytes("key-two");

            var encrypted = TripleDes.Encrypt(key1, key2, source);
            
            var decrypted = TripleDes.Decrypt(key1, key2, encrypted);

            source.Should().BeEquivalentTo(decrypted);
        }

        [Theory]
        [InlineData("7-bytes", false)]
        [InlineData("small", true)]
        [InlineData("too-big-key-for-des-algo", true)]
        public void TestDesAlgorithmRequires7ByteKey(string key, bool shouldThrow)
        {
            var source = File.ReadAllBytes(TestFilePath);

            var keyBytes = Encoding.UTF8.GetBytes(key);

            Action doubleDesAct = () => DoubleDes.Encrypt(keyBytes, keyBytes, source);
            Action tripleDesAct = () => TripleDes.Encrypt(keyBytes, keyBytes, source);

            if (shouldThrow)
            {
                doubleDesAct.Should().Throw<ArgumentException>();
                tripleDesAct.Should().Throw<ArgumentException>();
            }
            else
            {
                doubleDesAct.Should().NotThrow();
                tripleDesAct.Should().NotThrow();
            }
        }
    }
}

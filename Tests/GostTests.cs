using System.Diagnostics;
using System.IO;
using System.Text;
using Xunit;
using FluentAssertions;
using System;
using GostCrypto;
using System.Linq;

namespace Tests
{
    public class GostTests
    {
        private const string TestFilePath = "gost-test-data.txt";

        public GostTests()
        {
            var text = Faker.Lorem.Paragraph();
            var textSizeLimit = text.Length / 8 * 8;
            text = text[..textSizeLimit];

            Debug.Print($"Generated text: \n{text}\n");

            File.WriteAllText(TestFilePath, text);
        }

        [Fact]
        public void TestGostSourceEqualsDecrypted()
        {
            var source = File.ReadAllBytes(TestFilePath);

            var randomWord = Faker.Lorem.GetFirstWord();
            var word32BytesLong = Enumerable
                .Repeat(randomWord, 32 / randomWord.Length + 1)
                .Aggregate((a, b) => string.Join("", a, b))[..32];

            var key = Encoding.UTF8.GetBytes(word32BytesLong);

            var encrypted = Gost.Encrypt(key, source);
            
            var decrypted = Gost.Decrypt(key, encrypted);

            source.Should().BeEquivalentTo(decrypted);
        }

        [Theory]
        [InlineData("exactly-32-bytes-string-gost-key", false)]
        [InlineData("too-small-key", true)]
        [InlineData("tremendously-big-key-for-the-beautiful-gost-algorithm", true)]
        public void TestGostAlgorithmRequires32ByteKey(string key, bool shouldThrow)
        {
            var source = File.ReadAllBytes(TestFilePath);

            var keyBytes = Encoding.UTF8.GetBytes(key);

            Action act = () => Gost.Encrypt(keyBytes, source);

            if (shouldThrow)
            {
                act.Should().Throw<Exception>();
            }
            else
            {
                act.Should().NotThrow();
            }
        }
    }
}

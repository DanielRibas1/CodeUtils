using Codecs.Models;
using System;
using System.Linq;
using System.Text;
using Xunit;

namespace Codecs.Test
{
    public class DeflateCodecTests
    {
        private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private const string TestDecodedString = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. In lobortis ac ante sed placerat. Curabitur consequat ipsum eu ex sagittis, in pellentesque ante lobortis. Aenean elit turpis, sollicitudin ut accumsan sit amet, tempor eu magna. Nam pulvinar enim in nisl elementum cursus. Interdum et malesuada fames ac ante ipsum primis in faucibus. Donec ut ligula est. In molestie erat vestibulum turpis efficitur fringilla. In pretium condimentum scelerisque. Etiam ullamcorper ullamcorper ultricies. Proin justo tortor, pulvinar at lectus ac, blandit tempus tortor. Suspendisse quis maximus purus, sed imperdiet nulla. Morbi porttitor nulla risus, ut molestie leo venenatis eu. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Aliquam imperdiet risus feugiat mauris aliquet, ultrices ultrices ipsum sodales.";        
        private const string TestEncodedString = "SgMAAB+LCAAAAAAAAAt9kk1qHEEMha+iAwx9gaxM4oXBCYFA9upq9aCg+hlVyfj4edVtz0AW2TXV0tP3nvRaXTJp65Fpq1adug7iLONCqZYuacgIJ960aU9ariSmY6GXQlbX6kM7cSIuQ6jLRs04iTMqvobzqrP5ELoFj49BEiTv1PmqA+0X0kJNzAQa/RZyin2qL/QkRbgccwly4LhQr2aaoL6hOUCcUuSOqgf+kNzgB8MyXwsv9IMztbA3LYznovBdqGg3SEvGdKCl8B6Y+QIE3ybrQLtJD96Ydig/7J5mmmtGBpDaOZKus/tbLZImluk1jEn6mViuUBoqNBOit/m9hkHktEWy74cpp90RtZrx0ddchk66Wjb9IO0J1K4zsIWeh8JcoD6n6k38n+/h0BWQ/fQK0j/RR6WBfKtfHpmAybDwmBYvtBpj2jhyxNNZvdCv6E3wo3ehWwA687tmFLTwmJvBEWjG2E2RXZkcC32vviphHVg4VM5nAv3sQE73YEwqcilSeB6WxEK/Hyn9N3WqnpQsDn5MPk1jXa32EBesdlVTPu5SvtCTKU4SN3BnPXBol7gqksgceCCeZfOc7nr3j5Ok123ex/IXshAPBUoDAAA=";

        private static Random _randomString = new Random();
        private DeflateCodec _deflateCodec;
                
        public DeflateCodecTests()
        {
            _deflateCodec = new DeflateCodec(Encoding.UTF8);
        }

        [Fact]
        public void CodecFixedStringTest()
        {
            var encoded = _deflateCodec.Encode(new CodecInput<string> { Content = TestDecodedString });
            Assert.Equal(TestEncodedString, encoded.Content);
            var decoded = _deflateCodec.Decode(new CodecInput<string> { Content = encoded.Content });
            Assert.Equal(TestDecodedString, decoded.Content);
        }

        [Fact]
        public void EncodeStringTest()
        {
            var result = _deflateCodec.Encode(new CodecInput<string> { Content = TestDecodedString });
            Assert.Equal(TestEncodedString, result.Content);
        }

        [Fact]
        public void DecodeStringTest()
        {
            var result = _deflateCodec.Decode(new CodecInput<string> { Content = TestEncodedString });
            Assert.Equal(TestDecodedString, result.Content);
        }

        [Theory]
        [InlineData(5, 100)]
        [InlineData(5, 200)]
        [InlineData(5, 300)]
        [InlineData(5, 400)]
        [InlineData(5, 500)]
        public void CodecRandomStringsTest(int iterations, int length)
        {
            for (int i = 0; i < iterations; i++)
            {
                var s = GetRandomString(length);
                var encoded = _deflateCodec.Encode(new CodecInput<string> { Content = s });
                var decoded = _deflateCodec.Decode(new CodecInput<string> { Content = encoded.Content });
                Assert.Equal(s, decoded.Content);
            }
        }

        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(-10)]
        [InlineData(-8)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(4)]
        [InlineData(8)]
        [InlineData(10)]
        [InlineData(15)]
        [InlineData(int.MaxValue)]
        public void CompressionLevels(int compressionLevel)
        {
            var codec = new DeflateCodec(null, compressionLevel);
            Assert.True(codec.CompressionLevel <= 9);
            Assert.True(codec.CompressionLevel >= 1);
        }

        private static string GetRandomString(int length)
        {
            return new string(Enumerable.Repeat(Chars, length)
              .Select(s => s[_randomString.Next(s.Length)]).ToArray());
        }
    }
}
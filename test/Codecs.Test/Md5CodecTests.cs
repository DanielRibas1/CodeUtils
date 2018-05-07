using Codecs.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Codecs.Test
{    
    public class Md5CodecTests
    {
        [Theory]
        [InlineData("utf-8", "Lorem ipsum dolor sit amet, consectetur adipiscing elit. In lobortis ac ante sed placerat.", "4f9ab52a66b8ccfd6a3734680b31ee7e")]
        [InlineData("utf-32", "An an valley indeed so no wonder future nature vanity. Debating all she mistaken indulged believed provided declared. He many kept on draw lain song as same", "1305b1d35bdb4e936fdaa7a04dd180e0")]
        [InlineData("ascii", "Do play they miss give so up.", "d481751b64aa3c531e7185827a6273d5")]
        public void EncodeText(string encodingName, string input, string output)
        {
            var encoding = Encoding.GetEncoding(encodingName);
            Assert.NotNull(encoding);
            var sup = new Md5Codec(encoding);
            var response = sup.Encode(new CodecInput<string> { Content = input });
            Assert.NotNull(response);
            Assert.Equal(output, response.Content);
        }

        [Fact]
        public void ExceptionOnDecode()
        {
            var sup = new Md5Codec(Encoding.UTF8);
            Assert.Throws<NotSupportedException>(() => sup.Decode(new CodecInput<string>()));
        }
    }
}

using Codecs.Abstractions;
using Codecs.Models;
using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Codecs
{
    public class Md5Codec : ICodec<CodecInput<string>, CodecOutput<string>>
    {
        private const string Separator = ",";
        private const string HashToStringFormat = "x2";
        private readonly Encoding _encodingFormat;
        public Md5Codec(Encoding encodingFormat)
        {
            _encodingFormat = encodingFormat;
        }

        public CodecOutput<string> Decode(CodecInput<string> input)
        {
            throw new NotSupportedException("Cannot decode MD5");
        }

        public CodecOutput<string> Encode(CodecInput<string> input)
        {            
            var builder = new StringBuilder();
            using (MD5 hash = MD5.Create())
            {                
                byte[] data = hash.ComputeHash(_encodingFormat.GetBytes(input.Content));
                for (int i = 0; i < data.Length; i++)                
                    builder.Append(data[i].ToString(HashToStringFormat, CultureInfo.InvariantCulture));                
            }
            return new CodecOutput<string> { Content = builder.ToString() };
        }
    }
}

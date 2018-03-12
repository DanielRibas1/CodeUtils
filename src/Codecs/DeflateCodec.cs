using Codecs.Abstractions;
using Codecs.Models;
using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Codecs
{    
    public class DeflateCodec : ICodec<CodecInput<string>, CodecOutput<string>>
    {
        private readonly int _compressionLevel;
        private readonly Encoding _encodingFormat;     

        public DeflateCodec(Encoding encodingFormat, int compressionLevel = 5)
        {            
            if (compressionLevel > 9) compressionLevel = 9;
            if (compressionLevel < 1) compressionLevel = 1;
            _compressionLevel = compressionLevel;
            _encodingFormat = (encodingFormat == null) ? Encoding.UTF8 : encodingFormat;
        }       

        public int CompressionLevel
        {
            get
            {
                return _compressionLevel;
            }
        }

        public CodecOutput<string> Decode(CodecInput<string> value)
        {
            var gzBuffer = Convert.FromBase64String(value.Content);            
            using (MemoryStream ms = new MemoryStream())
            {
                var conversionLength = BitConverter.ToInt32(gzBuffer, 0);
                ms.Write(gzBuffer, 4, gzBuffer.Length - 4);
                var buffer = new byte[conversionLength];
                ms.Position = 0;
                using (var zip = new GZipStream(ms, CompressionMode.Decompress))                
                    zip.Read(buffer, 0, buffer.Length);                
                return new CodecOutput<string> { Content = _encodingFormat.GetString(buffer, 0, buffer.Length) };
            }
        }         

        public CodecOutput<string> Encode(CodecInput<string> value)
        {            
            byte[] buffer = _encodingFormat.GetBytes(value.Content);
            using (var ms = new MemoryStream())
            {
                using (var zip = new GZipStream(ms, CompressionMode.Compress, true))
                    zip.Write(buffer, 0, buffer.Length);

                ms.Position = 0;
                var compressed = new byte[ms.Length];
                ms.Read(compressed, 0, compressed.Length);

                var gzBuffer = new byte[compressed.Length + 4];
                Buffer.BlockCopy(compressed, 0, gzBuffer, 4, compressed.Length);
                Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gzBuffer, 0, 4);
                return new CodecOutput<string> { Content = Convert.ToBase64String(gzBuffer) };
            }            
        }       
    }
}

// Reference:
//     How to add seek and position capabilities to CryptoStream
//         - https://stackoverflow.com/questions/5026409/how-to-add-seek-and-position-capabilities-to-cryptostream

namespace SeekableAesAssetBundle.Scripts
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    
    public class SeekableAesStream : Stream
    {
        readonly Stream _baseStream;
        readonly AesManaged _aes;
        readonly ICryptoTransform _encryptor;
        
        public bool AutoDisposeBaseStream { get; set; } = true;

        public SeekableAesStream(Stream baseStream, string password, byte[] salt)
        {
            _baseStream = baseStream;
            using (var key = new Rfc2898DeriveBytes(password, salt))
            {
                _aes = new AesManaged
                {
                    KeySize = 128,
                    Mode = CipherMode.ECB,
                    Padding = PaddingMode.None
                };
                _aes.Key = key.GetBytes(_aes.KeySize / 8);
                // zero buffer is adequate since we have to use new salt for each stream
                // ※Stream毎に新しいsaltを使用する必要があるためにゼロバッファが適切
                _aes.IV = new byte[16];
                _encryptor = _aes.CreateEncryptor(_aes.Key, _aes.IV);
            }
        }

        void Cipher(byte[] buffer, int offset, int count, long streamPos)
        {
            // find block number
            // ※ブロック番号の検索
            var blockSizeInByte = _aes.BlockSize / 8;
            var blockNumber = (streamPos / blockSizeInByte) + 1;
            var keyPos = streamPos % blockSizeInByte;
            
            // buffer
            var outBuffer = new byte[blockSizeInByte];
            var nonce = new byte[blockSizeInByte];
            var init = false;

            for (var i = offset; i < count; i++)
            {
                // encrypt the nonce to form next xro buffer(unique key)
                // ※nonceを暗号化して次のxorバッファを作成
                if (!init || (keyPos % blockSizeInByte) == 0)
                {
                    BitConverter.GetBytes(blockNumber).CopyTo(nonce, 0);
                    _encryptor.TransformBlock(nonce, 0, nonce.Length, outBuffer, 0);
                    if (init) keyPos = 0;
                    init = true;
                    blockNumber++;
                }
                buffer[i] ^= outBuffer[keyPos];
                keyPos++;
            }
        }

        public override bool CanRead => _baseStream.CanRead;
        public override bool CanSeek => _baseStream.CanSeek;
        public override bool CanWrite => _baseStream.CanWrite;
        public override long Length => _baseStream.Length;
        public override long Position
        {
            get => _baseStream.Position;
            set => _baseStream.Position = value;
        }
        public override void Flush() => _baseStream.Flush();
        public override void SetLength(long value) => _baseStream.SetLength(value);
        public override long Seek(long offset, SeekOrigin origin) => _baseStream.Seek(offset, origin);

        public override int Read(byte[] buffer, int offset, int count)
        {
            var streamPos = Position;
            var ret = _baseStream.Read(buffer, offset, count);
            Cipher(buffer, offset, count, streamPos);
            return ret;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            Cipher(buffer, offset, count, Position);
            _baseStream.Write(buffer, offset, count);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _encryptor?.Dispose();
                _aes?.Dispose();
                if (AutoDisposeBaseStream)
                {
                    _baseStream?.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}

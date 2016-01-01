using System;
using Core.Crypto;

namespace app.core.security.onetime
{
    public class OTP
    {
        public const int SecretLength = 20;
        private const string
        MsgSecretlength = "Secret must be at least 20 bytes",
        MsgCounterMinvalue = "Counter min value is 1";

        public OTP()
        {
        }

        private static readonly int[] Dd = new int[10] { 0, 2, 4, 6, 8, 1, 3, 5, 7, 9 };

        private byte[] _secretKey = new byte[SecretLength] 
        {
	        0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39,
	        0x3A, 0x3B, 0x3C, 0x3D, 0x3E, 0x3F, 0x40, 0x41, 0x42, 0x43
        };

        private ulong _counter = 0x0000000000000001;

        public static int Checksum(int codeDigits)
        {
            int d1 = (codeDigits / 1000000) % 10;
            int d2 = (codeDigits / 100000) % 10;
            int d3 = (codeDigits / 10000) % 10;
            int d4 = (codeDigits / 1000) % 10;
            int d5 = (codeDigits / 100) % 10;
            int d6 = (codeDigits / 10) % 10;
            int d7 = codeDigits % 10;
            return (10 - ((Dd[d1] + d2 + Dd[d3] + d4 + Dd[d5] + d6 + Dd[d7]) % 10)) % 10;
        }

        /// <summary>
        /// Formats the OTP. This is the OTP algorithm.
        /// </summary>
        /// <param name="hmac">HMAC value</param>
        /// <returns>8 digits OTP</returns>
        private static string FormatOTP(byte[] hmac)
        {
            int offset = hmac[19] & 0xf;
            int bin_code = (hmac[offset] & 0x7f) << 24
                | (hmac[offset + 1] & 0xff) << 16
                | (hmac[offset + 2] & 0xff) << 8
                | (hmac[offset + 3] & 0xff);
            int Code_Digits = bin_code % 10000000;
            int csum = Checksum(Code_Digits);
            int OTP = Code_Digits * 10 + csum;

            return string.Format("{0:d08}", OTP);
        }

        public byte[] CounterArray
        {
            get
            {
                return BitConverter.GetBytes(_counter);
            }

            set
            {
                _counter = BitConverter.ToUInt64(value, 0);
            }
        }

        /// <summary>
        /// Sets the OTP secret
        /// </summary>
        public byte[] Secret
        {
            set
            {
                if (value.Length < SecretLength)
                {
                    throw new Exception(MsgSecretlength);
                }

                _secretKey = value;
            }
        }

        /// <summary>
        /// Gets the current OTP value
        /// </summary>
        /// <returns>8 digits OTP</returns>
        public string GetCurrentOTP()
        {
            var hmacSha1 = new HmacSha1();

            hmacSha1.Init(_secretKey);
            hmacSha1.Update(CounterArray);
            var hmacResult = hmacSha1.Final();

            return FormatOTP(hmacResult);
        }

        /// <summary>
        /// Gets the next OTP value
        /// </summary>
        /// <returns>8 digits OTP</returns>
        public string GetNextOTP()
        {
            // increment the counter
            ++_counter;

            return GetCurrentOTP();
        }

        /// <summary>
        /// Gets/sets the counter value
        /// </summary>
        public ulong Counter
        {
            get
            {
                return _counter;
            }

            set
            {
                _counter = value;
            }
        }
    }
}

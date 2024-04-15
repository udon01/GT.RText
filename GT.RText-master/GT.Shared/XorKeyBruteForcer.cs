using System;

namespace GT.Shared
{
    public class XorKeyBruteForcer
    {
        private readonly byte[] _obfuscated;
        private readonly byte[] _unobfuscated;

        public XorKeyBruteForcer(byte[] obfuscated, byte[] unobfuscated)
        {
            _obfuscated = obfuscated;
            _unobfuscated = unobfuscated;

            if (_obfuscated.Length != _unobfuscated.Length)
                throw new ArgumentOutOfRangeException("Array lengths must match.");
        }

        public void PrintXorKeys()
        {
            var xorKeys = GetXorKeys();
            for (int i = 0; i < xorKeys.Length; i++)
            {
                if (i % 0x10 == 0) Console.WriteLine();
                Console.Write($"0x{xorKeys[i]:X2}, ");
            }
        }

        public byte[] GetXorKeys()
        {
            var xorKeys = new byte[_obfuscated.Length];
            for (int i = 0; i < _obfuscated.Length; i++)
            {
                xorKeys[i] = GetXorByte(_obfuscated[i], _unobfuscated[i]);
            }

            return xorKeys;
        }

        private byte GetXorByte(byte obfuscatedByte, byte unobfuscatedByte)
        {
            for (int i = 0; i <= 0xFF; i++)
            {
                if ((obfuscatedByte ^ i) == unobfuscatedByte)
                    return (byte)i;
            }

            throw new ArgumentOutOfRangeException($"Couldn't brute force xor byte.");
        }
    }
}

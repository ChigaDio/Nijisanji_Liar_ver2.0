
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
namespace GameCore.Utils
{
    public sealed class FastEnumBitFlags<TEnum> where TEnum : struct, Enum
    {
        private readonly ulong[] _bits;
        private readonly int _bitCount;
        private readonly int _arrayLength;

        public FastEnumBitFlags()
        {
            var values = (TEnum[])Enum.GetValues(typeof(TEnum));
            int maxValue = values.Select(v => Convert.ToInt32(v)).Max();
            _bitCount = maxValue + 1;
            if (_bitCount <= 0)
                throw new ArgumentException("Enum must contain at least one non-negative value.");

            _arrayLength = (_bitCount + 63) / 64;
            _bits = new ulong[_arrayLength];
        }


        private FastEnumBitFlags(ulong[] bits, int bitCount, int arrayLength)
        {
            _bits = bits;
            _bitCount = bitCount;
            _arrayLength = arrayLength;
        }

        #region 基本操作（従来通り）

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsSet(TEnum flag)
        {
            int index = Convert.ToInt32(flag);
            return index > 0 && index < _bitCount && GetBit(index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set(TEnum flag)
        {
            int index = Convert.ToInt32(flag);
            if (index > 0 && index < _bitCount) SetBit(index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear(TEnum flag)
        {
            int index = Convert.ToInt32(flag);
            if (index > 0 && index < _bitCount) ClearBit(index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Toggle(TEnum flag)
        {
            int index = Convert.ToInt32(flag);
            if (index >= 0 && index < _bitCount)
                FlipBit(index);
        }

        #endregion

        #region 演算付きビット操作（XOR / AND / OR）

        /// <summary>
        /// XOR 演算でビット操作
        /// flag = true  → 反転
        /// flag = false → 何もしない
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void XORBit(TEnum flag, bool value)
        {
            if (!value) return;
            int index = Convert.ToInt32(flag);
            if (index > 0 && index < _bitCount)
                FlipBit(index);
        }

        /// <summary>
        /// AND 演算でビット操作
        /// flag = true  → 何もしない
        /// flag = false → クリア
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ANDBit(TEnum flag, bool value)
        {
            if (value) return;
            int index = Convert.ToInt32(flag);
            if (index > 0 && index < _bitCount)
                ClearBit(index);
        }

        /// <summary>
        /// OR 演算でビット操作
        /// flag = true  → セット
        /// flag = false → 何もしない
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ORBit(TEnum flag, bool value)
        {
            if (!value) return;
            int index = Convert.ToInt32(flag);
            if (index > 0 && index < _bitCount)
                SetBit(index);
        }

        #endregion

        #region 内部ヘルパー（インライン）

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool GetBit(int index)
        {
            int arrayIdx = index >> 6;
            int bitIdx = index & 63;
            return (_bits[arrayIdx] & (1UL << bitIdx)) != 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetBit(int index)
        {
            int arrayIdx = index >> 6;
            int bitIdx = index & 63;
            _bits[arrayIdx] |= 1UL << bitIdx;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ClearBit(int index)
        {
            int arrayIdx = index >> 6;
            int bitIdx = index & 63;
            _bits[arrayIdx] &= ~(1UL << bitIdx);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void FlipBit(int index)
        {
            int arrayIdx = index >> 6;
            int bitIdx = index & 63;
            _bits[arrayIdx] ^= 1UL << bitIdx;
        }

        #endregion

        #region ユーティリティ

        public void ClearAll() => Array.Clear(_bits, 0, _arrayLength);

        public void SetAll()
        {
            for (int i = 0; i < _arrayLength - 1; i++)
                _bits[i] = ulong.MaxValue;
            int rem = _bitCount & 63;
            _bits[_arrayLength - 1] = rem > 0 ? (1UL << rem) - 1 : ulong.MaxValue;
        }

        public FastEnumBitFlags<TEnum> Clone()
        {
            var clone = new ulong[_arrayLength];
            Buffer.BlockCopy(_bits, 0, clone, 0, _bits.Length * 8);
            return new FastEnumBitFlags<TEnum>(clone, _bitCount, _arrayLength);
        }

        public IEnumerable<TEnum> GetSetFlags()
        {
            for (int i = 1; i < _bitCount; i++)
            {
                if (GetBit(i) && Enum.IsDefined(typeof(TEnum), i))
                    yield return (TEnum)(object)i;
            }
        }

        #endregion
    }
}
        
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
namespace GameCore
{
    public readonly struct EnumIDIter<T> where T : unmanaged, Enum
    {
        private readonly int _value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EnumIDIter(T value)
        {
            _value = Unsafe.As<T, int>(ref value);
    #if UNITY_EDITOR
            if (!Enum.IsDefined(typeof(T), value))
                throw new ArgumentException($"Invalid enum value: {value}");
    #endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator EnumIDIter<T>(T value) => new(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator T(EnumIDIter<T> iter)
        {
            int val = iter._value;
            return Unsafe.As<int, T>(ref val);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EnumIDIter<T> operator ++(EnumIDIter<T> iter)
        {
            int next = iter._value + 1;
            T nextEnum = Unsafe.As<int, T>(ref next);
    #if UNITY_EDITOR
            if (!Enum.IsDefined(typeof(T), nextEnum))
                throw new InvalidOperationException($"Enum value out of range: {next}");
    #endif
            return new(nextEnum);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(EnumIDIter<T> a, EnumIDIter<T> b) => a._value < b._value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(EnumIDIter<T> a, EnumIDIter<T> b) => a._value > b._value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(EnumIDIter<T> a, EnumIDIter<T> b) => a._value == b._value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(EnumIDIter<T> a, EnumIDIter<T> b) => a._value != b._value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(EnumIDIter<T> a, EnumIDIter<T> b) => a._value <= b._value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(EnumIDIter<T> a, EnumIDIter<T> b) => a._value >= b._value;

        public override bool Equals(object obj) => obj is EnumIDIter<T> other && this == other;
        public override int GetHashCode() => _value.GetHashCode();
    }
}

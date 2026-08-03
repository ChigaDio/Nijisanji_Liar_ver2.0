
using System;
namespace GameCore.Behavior
{
    public class BaseBehaviorBlackboard<T, TEnum> 
    where T : BaseBehaviorBlackboard<T,TEnum>, new()
    where TEnum : struct, Enum

    {
        protected readonly Utils.FastEnumBitFlags<TEnum> Flags = new Utils.FastEnumBitFlags<TEnum>();
        public BaseBehaviorBlackboard()
        {
            Flags = new Utils.FastEnumBitFlags<TEnum>();
        }
        public void OnInit(Action<T> action = null)
        {
            action?.Invoke((T)this); // T にキャストして渡す
        }

        public void OnReset(Action<T> action = null)
        {
            action?.Invoke((T)this);
        }

        // ================ フラグ操作（委譲） ================
        public bool IsFlagSet(TEnum flag) => Flags.IsSet(flag);
        public void SetFlag(TEnum flag) => Flags.Set(flag);
        public void ClearFlag(TEnum flag) => Flags.Clear(flag);
        public void ToggleFlag(TEnum flag) => Flags.Toggle(flag);
        public void XORFlag(TEnum flag, bool value) => Flags.XORBit(flag, value);
        public void ANDFlag(TEnum flag, bool value) => Flags.ANDBit(flag, value);
        public void ORFlag(TEnum flag, bool value) => Flags.ORBit(flag, value);
        public void ClearAllFlags() => Flags.ClearAll();
        public void SetAllFlags() => Flags.SetAll();
    }


}


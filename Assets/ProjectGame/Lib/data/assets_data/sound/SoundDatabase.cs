

using System.Collections.Generic;
using GameCore.Enums;
namespace GameCore.Sound
{
    public class SoundDatabase
    {
        [System.Serializable]
        public class SoundData
        {
            private readonly string idName;
            private readonly string addressablePath;
            private readonly float baseVolume;
            private readonly SoundType type;
            private readonly SoundID soundID;
            private readonly int subGroupId;
            public SoundData(SoundID soundID, string idName, string addressablePath, float baseVolume, SoundType type, int subGroupId = 0)
            {
                this.idName = idName;
                this.addressablePath = addressablePath;
                this.baseVolume = baseVolume;
                this.type = type;
                this.soundID = soundID;
                this.subGroupId = subGroupId;
            }
            public string IdName => idName;
            public string AddressablePath => addressablePath;
            public float BaseVolume => baseVolume;
            public SoundID SoundID => soundID;
            public SoundType Type => type;
            // SubGroup ID（0 = SubGroupなし）。専用enum(例:Sound_EnemyID)にキャストして使う
            public int SubGroupId => subGroupId;
        }
        [System.Serializable]
        public class GroupedSounds
        {
            private readonly SoundGroup group;
            private readonly List<SoundData> sounds;
            public GroupedSounds(SoundGroup group, List<SoundData> sounds)
            {
                this.group = group;
                this.sounds = sounds ?? new List<SoundData>();
            }
            public SoundGroup Group => group;
            public List<SoundData> Sounds => sounds;
        }
        private readonly List<GroupedSounds> groupedSounds;
        public SoundDatabase()
        {
            groupedSounds = new List<GroupedSounds>();
        }
        public List<GroupedSounds> GroupedSoundsList => groupedSounds;
    }
}

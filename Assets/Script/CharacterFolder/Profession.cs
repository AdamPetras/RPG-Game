using System;
using Assets.Scripts.InventoryFolder;

namespace Assets.Script.CharacterFolder
{
    [Serializable]
    public class Profession
    {
        public uint Experience { get; set; }
        public EProfession EProfession { get; set; }
        public uint NextLevelExp { get; set; }
        public uint Level { get; set; }

        public Profession(EProfession eProfession)
        {
            EProfession = eProfession;
            Experience = 0;
            Level = 1;
            NextLevelExp = 300;
        }

        public void AddExperiences(uint exp)
        {
            Experience += exp;
            LevelUp();
        }

        public void LevelUp()
        {
            if (Experience >= NextLevelExp)
            {
                uint diff = Experience - NextLevelExp;
                Experience = 0;
                Experience += diff;
                Level++;
                NextLevelExp *= 2;
            }
        }
    }
}
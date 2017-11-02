using System;

namespace Assets.Script.StatisticsFolder
{
    /// <summary>
    /// This is the list of all skills
    /// </summary>
    public enum ESkill
    {
        Constitution,
        Intelect,
        Dexterity,
        Strenght,
        Agility,
        Attack,
        Defence,
        Magic,
        Ranged,
        Regenerate,
        Consumable,
        Usable,
        None
    }
    /// <summary>
    /// Skill.cs
    /// Adam Petráš
    /// 14.10.2016
    /// 
    /// This is the class for all skills
    /// </summary>
    [Serializable]
    public class Skill : Statistics
    {
        public new float CurrentValue { get { return ValuesTogether; } set { currentValue = value; } }
    }
}
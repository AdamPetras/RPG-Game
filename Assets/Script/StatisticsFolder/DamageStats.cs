using System;

namespace Assets.Script.StatisticsFolder
{
    public enum EDamageStats
    {
        SwordDamage,
        AttackSpeed,
        AxeDamage,
        MaceDamage,      
        MagicPower,
        RangedPower,
        CriticalChance,
        DamageBlock,
        HealthRegen
    }
    [Serializable]
    public class DamageStats : SkillSettings
    {
    }
}
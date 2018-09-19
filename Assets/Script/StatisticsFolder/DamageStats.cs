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
        HealthRegen,
        None
    }
    [Serializable]
    public class DamageStats : SkillSettings
    {
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Script.StatisticsFolder
{

    /// <summary>
    /// SkillSettings.cs
    /// Adam Petráš
    /// 14.10.2016
    /// 
    /// This is the class for modify the skills
    /// </summary>
    [Serializable]
    public class SkillSettings : Statistics
    {
        private List<ModifySkill> ModifySkills;
        private float value;
        /// <summary>
        /// Initialize the new instance of the <see cref="SkillSettings"/> class
        /// </summary>
        public SkillSettings()
        {
            ModifySkills = new List<ModifySkill>();
        }
        /// <summary>
        /// This method add the modify skill to the list
        /// </summary>
        /// <param name="modifySkill"></param>
        public void AddSkill(ModifySkill modifySkill)
        {
            ModifySkills.Add(modifySkill);
        }

        /// <summary>
        /// This method update values of modify skills
        /// If list isn't empty
        /// </summary>
        public void Update()
        {
            if (ModifySkills.Count > 0)
            {
                value = 0;
                foreach (ModifySkill modifySkill in ModifySkills)
                {
                    if (modifySkill.secondSkill == null && modifySkill.firstSkill.IfAdd)
                        value += ((modifySkill.firstSkill.ValuesTogether) * modifySkill.firstRatio);
                    else if (modifySkill.secondSkill == null && !modifySkill.firstSkill.IfAdd && modifySkill.defaultValue != 0)
                    {
                        value = modifySkill.defaultValue + (modifySkill.firstSkill.BonusValue * modifySkill.firstRatio);
                    }
                    else if (modifySkill.firstSkill != null && modifySkill.secondSkill != null)
                        value += ((modifySkill.firstSkill.ValuesTogether * modifySkill.firstRatio) +
                                  (modifySkill.secondSkill.ValuesTogether * modifySkill.secondRatio));
                    //pokud jsou například ubrané životy
                    float i = 0;
                    if (CurrentValue < MaxValue && CurrentValue > 0)
                    {
                        i = MaxValue - CurrentValue;
                    }
                    MaxValue = value;
                    CurrentValue = MaxValue - i;
                    // ošetření s buggem že by si nasazoval a sundával oblečení a měl pořád plno životu
                }
            }
        }
        /// <summary>
        /// Getter for return the ValuesTogether+value
        /// </summary>
        /// <value>
        /// ValuesTogether and value
        /// </value>
        public override float ValuesTogether
        {
            get { return MaxValue = value; }
        }
    }
    /// <summary>
    /// The struct hold skills and ratios that will be added as modified
    /// </summary>
    [Serializable]
    public struct ModifySkill
    {
        public Skill firstSkill;
        public Skill secondSkill;
        public float firstRatio;
        public float secondRatio;
        public float defaultValue;
        /// <summary>
        /// Initialize the new instance of the <see cref="ModifySkill"/>
        /// </summary>
        /// <param name="firstSkill"></param>
        /// <param name="firstRatio"></param>
        public ModifySkill(Skill firstSkill, float firstRatio)
        {
            this.firstSkill = firstSkill;
            this.firstRatio = firstRatio;
            secondSkill = null;
            secondRatio = 0;
            defaultValue = 0;
        }
        public ModifySkill(Skill firstSkill, float firstRatio, float defaultValue)
        {
            this.firstSkill = firstSkill;
            this.firstRatio = firstRatio;
            secondSkill = null;
            secondRatio = 0;
            this.defaultValue = defaultValue;
        }
        /// <summary>
        /// Initialize the new instance of the <see cref="ModifySkill"/>
        /// </summary>
        /// <param name="firstSkill"></param>
        /// <param name="secondSkill"></param>
        /// <param name="firstRatio"></param>
        /// <param name="secondRatio"></param>
        public ModifySkill(Skill firstSkill, Skill secondSkill, float firstRatio, float secondRatio)
        {
            this.firstSkill = firstSkill;
            this.secondSkill = secondSkill;
            this.firstRatio = firstRatio;
            this.secondRatio = secondRatio;
            defaultValue = 0;
        }
        public ModifySkill(Skill firstSkill, Skill secondSkill, float firstRatio, float secondRatio, float defaultValue)
        {
            this.firstSkill = firstSkill;
            this.secondSkill = secondSkill;
            this.firstRatio = firstRatio;
            this.secondRatio = secondRatio;
            this.defaultValue = defaultValue;
        }
    }
}
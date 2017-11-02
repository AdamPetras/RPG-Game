using System;

namespace Assets.Script.StatisticsFolder
{
    /// <summary>
    /// Statistics
    /// Adam Petras
    /// 14.10.2016
    /// 
    /// This is the base class of statistics
    /// </summary>
    [Serializable]
    public abstract class Statistics
    {
        protected float maxValue;
        protected float currentValue;

        /// <summary>
        /// This method gets or sets the BasicValue
        /// </summary>
        /// <value>
        /// The BasicValue
        /// </value>
        public float BasicValue { get; set; }
        /// <summary>
        /// This method gets or sets the BonusValue
        /// </summary>
        /// <value>
        /// The BonusValue
        /// </value>
        public float BonusValue { get; set; }
        public float CurrentValue
        {
            get { return currentValue; }
            set { currentValue = value; }
        }

        public float MaxValue
        {
            get { return maxValue; }
            set { maxValue = value; }
        }

        public bool IfAdd { get; set; }

        /// <summary>
        /// Initialize the new instance of the <see cref="Statistics"/> class
        /// </summary
        protected Statistics()
        {
            BasicValue = 0;
            BonusValue = 0;           
            MaxValue = 0;
            currentValue = MaxValue;
            IfAdd = true;
        }

        /// <summary>
        /// This method gets or sets the _valuesTogether and calculate
        /// </summary>
        /// <value>
        /// The _valuesTogether
        /// </value>
        public virtual float ValuesTogether
        {
            get { return (BasicValue + BonusValue); }
        }
    }
}


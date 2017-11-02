namespace Assets.Script.QuestFolder
{


    public interface IQuest : IBasicAtribute
    {
        int QuestID { get; set; }
        string Description { get; set; }
        int Experiences { get; set; }
        int QuestMasterAsign { get; set; }
        int QuestMasterSubmit { get; set; }
        uint MoneyReward { get; set; }
        EQuest EQuest { get; set; }
        EQuestState EQuestState { get; set; }
    }
}

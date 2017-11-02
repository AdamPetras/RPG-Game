namespace Assets.Script.QuestFolder
{
    public interface IQuestMaster
    {
        void OnAcceptQuest(ModifyQuest quest);
        void OnSubmitQuest(ModifyQuest quest);
    }
}

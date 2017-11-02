using System.Collections.Generic;

namespace Assets.Script.QuestFolder
{
    public interface IQuestTalk : IQuest
    {
        List<int> QuestMasterList { get; set; }
        List<int> QuestMasterTalkedList { get;set; }
    }
}

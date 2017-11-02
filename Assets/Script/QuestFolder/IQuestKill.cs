namespace Assets.Script.QuestFolder
{
    public interface IQuestKill : IQuest
    {
        int EnemyId { get; set; }
        byte CurrentKills { get; set; }
        byte KillsToComplete { get; set; }
        byte TotalKills { get; set; }
    }
}

[System.Serializable]
public class Quest
{
    public string questName;
    public string gameName; 
    public string description;
    public QuestStep[] steps = new QuestStep[0];
}
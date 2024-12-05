using UnityEngine;

public class allScripts : MonoBehaviour
{
    public ClicksHandler clicksHandler;
    public ColonyManager colonyManager;
    public DialogManager dialogManager;
    public BuildingSystem buildingSystem;
    public QuestSystem questSystem;

    public bool CheckOpenedWindows(bool mode, bool extraOpen = false) //  Общий метод для проверки "открыто ли какое-либо меню"
    {
        if (!mode) // Если окно закрывается, то скипать проверку
            return false;
        if ((dialogManager.dialogMenu.activeSelf || questSystem.questMenu.activeSelf || colonyManager.bearsListMenu.activeSelf || buildingSystem.buildingCreateMenu.activeSelf || buildingSystem.buildMenu.activeSelf) && !extraOpen)
            return true;
        else
            return false;
    }
}

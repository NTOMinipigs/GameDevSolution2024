using UnityEngine;

public class AllScripts : MonoBehaviour
{
    public APIClient APIClient_;
    public ClicksHandler clicksHandler;
    public CameraMove cameraMove;
    public ColonyManager colonyManager;
    public DialogManager dialogManager;
    public BuildingSystem buildingSystem;
    public QuestSystem questSystem;
    public GameMenuManager GMM;
    public TravelingManager TM;
    public Preference preference;
    public MusicManager musicManager;

    public bool CheckOpenedWindows(bool mode, bool extraOpen = false) //  Общий метод для проверки "открыто ли какое-либо меню"
    {
        if (!mode) // Если окно закрывается, то скипать проверку
            return false;
        if ((dialogManager.dialogMenu.activeSelf || questSystem.questMenu.activeSelf || colonyManager.bearsListMenu.activeSelf || buildingSystem.buildingCreateMenu.activeSelf || buildingSystem.buildMenu.activeSelf || TM.travelMenu.activeSelf || GMM.gameMenu.activeSelf) && !extraOpen)
            return true;
        else
            return false;
    }
}

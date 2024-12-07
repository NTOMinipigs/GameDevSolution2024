using UnityEngine;

public class SaveAndLoad : MonoBehaviour
{
    // Start is called before the first frame update

    void Start()
    {
        SystemSaver systemSaver = gameObject.GetComponent<SystemSaver>();
        bool loadResult = systemSaver.LoadGame();
        // Если файл сохранения не найден
        if (!loadResult)
        {
            CreateNewGame();
        }

        LoadGame();
    }

    void OnApplicationQuit()
    {
        SaveGame();
    }

    /// <summary>
    /// Вызывает методы загрузки игры, например спавн медведей из памяти
    /// Оставьте этот метод приватным, он должен вызываться только один раз в одном месте
    /// </summary>
    private void LoadGame()
    {
        SpawnBears();
    }

    /// <summary>
    /// Вызывает методы сохранения игры.
    /// Оставьте его публичным, чтобы игру можно было сохранять из любого класса
    /// </summary>
    public void SaveGame()
    {
        SaveBears();
        SystemSaver systemSaver = gameObject.GetComponent<SystemSaver>();
        systemSaver.SaveGame();
    }

    /// <summary>
    /// Вызывает методы создания игры, например создает медведей постройки и т.д.
    /// </summary>
    private void CreateNewGame()
    {
        CreateBears();
    }

    /// <summary>
    /// СОЗДАСТ новых медведей в игре
    /// </summary>
    private void CreateBears()
    {
        ColonyManager colonyManager = gameObject.GetComponent<ColonyManager>();
        colonyManager.GenerateChrom();
        colonyManager.GenerateNewBear(TraditionsManager.Traditions.Beekeepers);
        colonyManager.GenerateNewBear(TraditionsManager.Traditions.Programmers);
        colonyManager.GenerateNewBear(TraditionsManager.Traditions.Constructors);
        colonyManager.GenerateNewBear(TraditionsManager.Traditions.BioEngineers);
    }

    /// <summary>
    /// Загрузит медведей из json'а
    /// </summary>
    private void SpawnBears()
    {
        SystemSaver systemSaver = gameObject.GetComponent<SystemSaver>();
        ColonyManager colonyManager = gameObject.GetComponent<ColonyManager>();

        foreach (BearSave bearSave in systemSaver.gameSave.bearSaves)
        {
            colonyManager.BearSpawn(bearSave);
        }
    }

    /// <summary>
    /// Сохранить медведей, сведения о них, например позиции
    /// </summary>
    private void SaveBears()
    {
        ColonyManager colonyManager = gameObject.GetComponent<ColonyManager>();
        SystemSaver systemSaver = gameObject.GetComponent<SystemSaver>();
        // Рассчитывается на то, что каждый медведь из bearsInColony, соответствует индексу в json 

        for (int i = 0; i < colonyManager.bearsInColony.Count; i++)
        {
            // Получаем медведей медведя и сейв медведя из списков
            Bear bear = colonyManager.bearsInColony[i];
            BearSave bearSave = systemSaver.gameSave.bearSaves[i];

            if (bear.tradition != TraditionsManager.Traditions.Chrom)
            {
                // Сейвим координаты
                GameObject bearObj = GameObject.Find(bear.gameName);
                bearSave.x = bearObj.transform.position.x;
                bearSave.z = bearObj.transform.position.z;
            }

            // Настроение голод и активность
            bearSave.hungry = bear.hungry;
            bearSave.tired = bear.tired;
            bearSave.activity = ActivityManager.GetStrByActivity(bear.activity);
        }
    }

}

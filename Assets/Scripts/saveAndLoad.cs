using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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

    /// <summary>
    /// Вызывает методы загрузки игры, например спавн медведей из памяти
    /// </summary>
    private void LoadGame()
    {
        SpawnBears();
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
        Debug.Log("Creating new bears");
        ColonyManager colonyManager = gameObject.GetComponent<ColonyManager>();
        colonyManager.GenerateChrom();
        colonyManager.GenerateNewBear(TraditionsManager.Traditions.Beekeepers);
        colonyManager.GenerateNewBear(TraditionsManager.Traditions.Programmers);
        colonyManager.GenerateNewBear(TraditionsManager.Traditions.Beekeepers);
        colonyManager.GenerateNewBear(TraditionsManager.Traditions.Programmers);
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
            Debug.Log(bearSave);
            colonyManager.BearSpawn(bearSave);
        }
    }
}

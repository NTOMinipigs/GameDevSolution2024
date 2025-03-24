using System.Collections.Generic;
using Alerts;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;


/// <summary>
/// Объект медведя, хранит по медведю всю информацию
/// </summary>
[System.Serializable]
public class Bear
{
    [Header("MainInformation")]
    /// <summary>
    /// gameName - имя медведя которое видит разработчик (gameObject name)
    /// </summary>
    [JsonProperty("gameName")]
    public string gameName;

    /// <summary>
    ///  bearName - имя медведя которое видит игрок
    /// </summary>
    [JsonProperty("bearName")] public string bearName;

    /// <summary>
    /// Объект Tradition из enum. Определяет профессию медвдея
    /// Сереализуемый объект в json
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))] [JsonProperty("tradition")]
    public Traditions tradition;

    /// <summary>
    /// Объект Activity из enum. Определяет активность медведя
    /// Сереализуемый объект в json
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))] [JsonProperty("activity")]
    public Activities activity;

    /// <summary>
    /// 2д спрайт медведя в диалоге
    /// </summary>
    [JsonIgnore] public Sprite sprite;

    /// <summary>
    /// serializableBear - Название объекта из иерархии BearSprites
    /// </summary>
    [JsonProperty("serializableBear")] public string serializableBear;

    [Header("Modify")]

    #region Modificators

    /// <summary>
    /// Чертвы характера медведя
    /// </summary>
    public List<BearCharacter> bearCharacters = new List<BearCharacter>();

    /// <summary>
    /// Максимальная переносимая минусовая температура
    /// </summary>
    public float maxComfortableTemperature;

    /// <summary>
    /// Модификатор работы
    /// </summary>
    public float modifyWork = 1f;

    /// <summary>
    /// Модификатор голода
    /// </summary>
    public float modifyHungry = 1f;

    /// <summary>
    /// Модификатор усталости
    /// </summary>
    public float modifyTired = 1f;

    /// <summary>
    /// Модификатор скорости передвижения медведя
    /// </summary>
    public float modifySpeed = 1f;

    #endregion

    [Header("Needs")]
    /// <summary>
    /// Сытость  медведя
    /// </summary>
    [JsonProperty("hungry")]
    public float hungry;

    /// <summary>
    /// Усталость медведя
    /// </summary>
    [JsonProperty("tired")] public float tired;

    /// <summary>
    /// Может двигаться или нет
    /// </summary>
    public bool canMove = true;

    [Header("Other")] [JsonProperty("x")] public float x;
    [JsonProperty("y")] public float y;
    [JsonProperty("z")] public float z;

    /// <summary>
    /// Конструктор медведя
    /// </summary>
    /// <param name="gameName">имя медведя как gameObject</param>
    /// <param name="bearName">Имя медведя которое видит пользователь</param>
    /// <param name="tradition">Профессия медведя</param>
    /// <param name="sprite">Спрайт медведя</param>
    public Bear(string gameName, string bearName, Traditions tradition, Sprite sprite)
    {
        this.gameName = gameName;
        this.bearName = bearName;
        this.tradition = tradition;
        this.sprite = sprite;
    }

    /// <summary>
    /// Выдать рандомную черту характера
    /// </summary>
    /// <returns></returns>
    public string AddRandomCharacters()
    {
        BearCharacter newCharacter = ColonyManager.Singleton.allBearCharacters
            [Random.Range(0, ColonyManager.Singleton.allBearCharacters.Length)];
        if (bearCharacters.Contains(newCharacter))
            return AddRandomCharacters();
        else
            bearCharacters.Add(newCharacter);
        return newCharacter.gameName;
    }

    /// <summary>
    /// Получить все черты характера медведя
    /// </summary>
    /// <returns></returns>
    public string GetAllCharacters()
    {
        string characters = "";
        foreach (BearCharacter character in bearCharacters)
            characters += character.gameName + "\n";

        return characters;
    }
    
    public void UpdateModifiers()
    {
        foreach (BearCharacter character in bearCharacters)
        {
            foreach (CharacterModificators characterModificator in character.characterModificators)
            {
                switch (characterModificator.characterChanges)
                {
                    case CharacterModificators.CharacterChanges.ModifyWork:
                        modifyWork += characterModificator.modif;
                        break;
                    case CharacterModificators.CharacterChanges.ModifyHungry:
                        modifyHungry += characterModificator.modif;
                        break;
                    case CharacterModificators.CharacterChanges.ModifyTired:
                        modifyTired += characterModificator.modif;
                        break;
                }
            }
        }
    }
}
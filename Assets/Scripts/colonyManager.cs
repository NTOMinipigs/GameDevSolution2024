using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class colonyManager : MonoBehaviour
{
    [Header("Bears")]
    public List<bear> bearsInColony = new List<bear>();
    [SerializeField] private string[] firstnamesForBears, secondnamesForBears = new string[0];

    public void GenerateNewBear(string gameName, bear.traditions tradition)
    {
        string bearName = firstnamesForBears[Random.Range(0, firstnamesForBears.Length - 1)] + " " + secondnamesForBears[Random.Range(0, secondnamesForBears.Length - 1)];
        bear newBear = new bear("", bearName, tradition);
        bearsInColony.Add(newBear);
    }
}

[System.Serializable]
public class bear
{
    public string gameName;
    public string bearName;
    public Sprite sprite;
    public enum traditions {none, beekeepers, constructors, programmers, bioEngineers, special}
    public traditions tradition;
    public float lvl = 0f;

    public bear()
    {

    }

    public bear(string _gameName, string _bearName, traditions _tradition, Sprite _sprite = null)
    {
        gameName = _gameName;
        bearName = _bearName;
        tradition = _tradition;
        sprite = _sprite;
    }
}
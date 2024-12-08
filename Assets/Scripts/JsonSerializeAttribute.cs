using System;


/// <summary>
/// Этот аттрибут указывает на то, что текущее поле должно быть сохранено в json и успешно в нем сереализуется
/// В будущем весь модуль save нужно будет отрефакторить под этот аттрибут
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class JsonSerializeAttribute : Attribute
{
    public string inJsonName = "";

    public JsonSerializeAttribute(string inJsonNameArg)
    {
        inJsonName = inJsonNameArg;
    }
}


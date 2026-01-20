using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class SaveableObject : MonoBehaviour
{
    public bool SaveObjectState;

    [Space(10)]
    public bool SaveAtSpawn;
    public int PrefabID;

    const string SaveSeparator = "#UwU#";

    List<string> ValuesToSave = new List<string>();

    [Space(10)]
    public List<ComponentFields> ComponentList = new List<ComponentFields>();

    [TextArea(0, 0), Space(20), Tooltip("Make Preset")]
    public string ___________________________________Important___________________________________;

    public void OnEnable()
    {
        if (SaveAtSpawn)
            SaveManager.Instance.SaveableSpawns[PrefabID].SpawnsToSave.Add(this);
    }

    #if UNITY_EDITOR
    public void Update()
    {
        if (!Application.isPlaying)
        {
            foreach (ComponentFields CompFields in ComponentList)
            {
                CompFields.name = CompFields.component.GetType().ToString();
            }
        }
    }
    #endif

    public void SaveFields()
    {
        for (int i = 0; i < ComponentList.Count; i++)
        {
            foreach(string FieldName in ComponentList[i].FieldsToSave)
            {
                if (ComponentList[i].component.GetType().GetField(FieldName) != null)
                {
                    var Field = ComponentList[i].component.GetType().GetField(FieldName);
                    var value = Field.GetValue(ComponentList[i].component);

                    ValuesToSave.Add(Field.FieldType == typeof(GameObject) ? Array.FindIndex(SaveManager.Instance.SaveableReferences, obj => obj.gameObject == value).ToString() : Field.FieldType == typeof(Component) ? SaveManager.Instance.SaveableComponentReferences.FindIndex(obj => obj == value).ToString() : value.ToString());
                }

                else
                {
                    var Property = ComponentList[i].component.GetType().GetProperty(FieldName);
                    var value = Property.GetValue(ComponentList[i].component);

                    ValuesToSave.Add(Property.PropertyType == typeof(GameObject) ? Array.FindIndex(SaveManager.Instance.SaveableReferences, obj => obj.gameObject == value).ToString() : Property.PropertyType == typeof(Component) ? SaveManager.Instance.SaveableComponentReferences.FindIndex(obj => obj == value).ToString() : value.ToString());
                }
            }
        }

        if(SaveObjectState)
        {
            if (gameObject.activeSelf)
                ValuesToSave.Add("true");
            else
                ValuesToSave.Add("false");
        }

        string SaveValues = string.Join(SaveSeparator, ValuesToSave);

        if(SaveAtSpawn)
            SaveManager.Instance.SaveableSpawns[PrefabID].SpawnContentsToSave.Add(SaveValues);

        else
            SaveManager.Instance.ContentsToSave.Add(SaveValues);

        ValuesToSave.Clear();
    }

    string[] VectorValues;

    public void LoadFields(string Contents)
    {
        string[] LoadedValues = Contents.Split(new[] { SaveSeparator }, System.StringSplitOptions.None);

        int LoadCount = 0;

        for (int i = 0; i < ComponentList.Count; i++)
        {
            for (int j = 0; j < ComponentList[i].FieldsToSave.Count; j++)
            {
                var FieldName = ComponentList[i].FieldsToSave[j];

                if (ComponentList[i].component.GetType().GetField(FieldName) != null)
                {
                    var Field = ComponentList[i].component.GetType().GetField(FieldName);

                    if (Field.FieldType == typeof(bool))
                    {
                        if (LoadedValues[LoadCount] == "false" || LoadedValues[LoadCount] == "False")
                            Field.SetValue(ComponentList[i].component, false);
                        else
                            Field.SetValue(ComponentList[i].component, true);
                    }

                    else if (Field.FieldType == typeof(int))
                        Field.SetValue(ComponentList[i].component, int.Parse(LoadedValues[LoadCount]));

                    else if (Field.FieldType == typeof(float))
                        Field.SetValue(ComponentList[i].component, float.Parse(LoadedValues[LoadCount]));

                    else if (Field.FieldType == typeof(string))
                        Field.SetValue(ComponentList[i].component, LoadedValues[LoadCount]);

                    else if (Field.FieldType == typeof(Vector3))
                    {
                        VectorValues = LoadedValues[LoadCount].Split(new[] { ", ", "(", ")" }, StringSplitOptions.None);
                        Field.SetValue(ComponentList[i].component, new Vector3(float.Parse(VectorValues[1]), float.Parse(VectorValues[2]), float.Parse(VectorValues[3])));
                    }

                    else if (Field.FieldType == typeof(Quaternion))
                    {
                        VectorValues = LoadedValues[LoadCount].Split(new[] { ", ", "(", ")" }, StringSplitOptions.None);
                        Field.SetValue(ComponentList[i].component, new Quaternion(float.Parse(VectorValues[1]), float.Parse(VectorValues[2]), float.Parse(VectorValues[3]), float.Parse(VectorValues[4])));
                    }

                    else if (Field.FieldType == typeof(Color32))
                    {
                        VectorValues = LoadedValues[LoadCount].Split(new[] { ", ", "(", ")" }, StringSplitOptions.None);
                        Field.SetValue(ComponentList[i].component, new Color32(byte.Parse(VectorValues[1]), byte.Parse(VectorValues[2]), byte.Parse(VectorValues[3]), byte.Parse(VectorValues[4])));
                    }

                    else if (Field.FieldType == typeof(Color))
                    {
                        VectorValues = LoadedValues[LoadCount].Split(new[] { ", ", "(", ")" }, StringSplitOptions.None);
                        Field.SetValue(ComponentList[i].component, new Color(float.Parse(VectorValues[1]), float.Parse(VectorValues[2]), float.Parse(VectorValues[3]), float.Parse(VectorValues[4])));
                    }

                    else if (Field.FieldType == typeof(GameObject))
                    {
                        if (int.Parse(LoadedValues[LoadCount]) != -1)
                            Field.SetValue(ComponentList[i].component, SaveManager.Instance.SaveableReferences[int.Parse(LoadedValues[LoadCount])]);

                        else
                            Field.SetValue(ComponentList[i].component, null);
                    }

                    else if (Field.FieldType == typeof(Component))
                    {
                        if (int.Parse(LoadedValues[LoadCount]) != -1)
                            Field.SetValue(ComponentList[i].component, SaveManager.Instance.SaveableComponentReferences[int.Parse(LoadedValues[LoadCount])]);

                        else
                            Field.SetValue(ComponentList[i].component, null);
                    }
                }

                else
                {
                    var Property = ComponentList[i].component.GetType().GetProperty(FieldName);

                    if (Property.PropertyType == typeof(bool))
                    {
                        if (LoadedValues[LoadCount] == "false" || LoadedValues[LoadCount] == "False")
                            Property.SetValue(ComponentList[i].component, false);
                        else
                            Property.SetValue(ComponentList[i].component, true);
                    }

                    else if (Property.PropertyType == typeof(int))
                        Property.SetValue(ComponentList[i].component, int.Parse(LoadedValues[LoadCount]));

                    else if (Property.PropertyType == typeof(float))
                        Property.SetValue(ComponentList[i].component, float.Parse(LoadedValues[LoadCount]));

                    else if (Property.PropertyType == typeof(string))
                        Property.SetValue(ComponentList[i].component, LoadedValues[LoadCount]);

                    else if (Property.PropertyType == typeof(Vector3))
                    {
                        VectorValues = LoadedValues[LoadCount].Split(new[] { ", ", "(", ")" }, StringSplitOptions.None);
                        Property.SetValue(ComponentList[i].component, new Vector3(float.Parse(VectorValues[1]), float.Parse(VectorValues[2]), float.Parse(VectorValues[3])));
                    }

                    else if (Property.PropertyType == typeof(Quaternion))
                    {
                        VectorValues = LoadedValues[LoadCount].Split(new[] { ", ", "(", ")" }, StringSplitOptions.None);
                        Property.SetValue(ComponentList[i].component, new Quaternion(float.Parse(VectorValues[1]), float.Parse(VectorValues[2]), float.Parse(VectorValues[3]), float.Parse(VectorValues[4])));
                    }

                    else if (Property.PropertyType == typeof(Color32))
                    {
                        VectorValues = LoadedValues[LoadCount].Split(new[] { ", ", "(", ")" }, StringSplitOptions.None);
                        Property.SetValue(ComponentList[i].component, new Color32(byte.Parse(VectorValues[1]), byte.Parse(VectorValues[2]), byte.Parse(VectorValues[3]), byte.Parse(VectorValues[4])));
                    }

                    else if (Property.PropertyType == typeof(Color))
                    {
                        VectorValues = LoadedValues[LoadCount].Split(new[] { ", ", "(", ")" }, StringSplitOptions.None);
                        Property.SetValue(ComponentList[i].component, new Color(float.Parse(VectorValues[1]), float.Parse(VectorValues[2]), float.Parse(VectorValues[3]), float.Parse(VectorValues[4])));
                    }

                    else if (Property.PropertyType == typeof(GameObject))
                    {
                        if(int.Parse(LoadedValues[LoadCount]) != -1)
                            Property.SetValue(ComponentList[i].component, SaveManager.Instance.SaveableReferences[int.Parse(LoadedValues[LoadCount])]);

                        else
                            Property.SetValue(ComponentList[i].component, null);
                    }

                    else if(Property.PropertyType == typeof(Component))
                    {
                        if (int.Parse(LoadedValues[LoadCount]) != -1)
                            Property.SetValue(ComponentList[i].component, SaveManager.Instance.SaveableComponentReferences[int.Parse(LoadedValues[LoadCount])]);

                        else
                            Property.SetValue(ComponentList[i].component, null);
                    }
                }

                LoadCount++;
            }
        }

        if(SaveObjectState)
        {
            if (LoadedValues[LoadCount] == "true")
                gameObject.SetActive(true);
            else
                gameObject.SetActive(false);
        }
    }
}

[System.Serializable]
public class ComponentFields
{
    [HideInInspector]
    public string name;

    [Space(10)]
    public Component component;

    [Space(10)]
    public List<string> FieldsToSave;
}

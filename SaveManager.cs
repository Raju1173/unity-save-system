using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    public string LoadData;

    [Space(10)]
    public SaveableObject[] ObjectsToSave;

    [Space(10)]
    public SaveablePrefabs[] SaveableSpawns;

    [Space(10)]
    public GameObject[] SaveableReferences = new GameObject[100];

    [HideInInspector]
    public List<Component> SaveableComponentReferences = new List<Component>();

    [HideInInspector]
    public List<string> ContentsToSave = new List<string>();

    [HideInInspector]
    public List<string> SpawnSaveValues = new List<string>();

    [TextArea(0, 0), Space(20), Tooltip("Make Preset")]
    public string ___________________________________Important___________________________________;

    public void Awake()
    {
        Instance = this;

        foreach(GameObject obj in SaveableReferences)
        {
            SaveableComponentReferences.AddRange(obj.GetComponents(typeof(Component)));
        }
    }

    [ContextMenu("SAVE")]
    public void Save()
    {
        SaveablePrefabs Pfb = null;

        SaveHandler.Instance.TempData[SceneManager.GetActiveScene().buildIndex] = string.Empty;

        if (SaveableSpawns != null)
        {
            for (int j = 0; j < SaveableSpawns.Length; j++)
            {
                Pfb = SaveableSpawns[j];

                for (int k = 0; k < SaveableSpawns[j].SpawnsToSave.Count; k++)
                {
                    Pfb.SpawnsToSave[k].SaveFields();
                }

                Pfb.SpawnContentsToSave.Add(Pfb.SpawnsToSave.Count.ToString());

                SpawnSaveValues.Add(string.Join("#MEOW#", Pfb.SpawnContentsToSave));

                Pfb.SpawnContentsToSave.Clear();
            }
        }

        string SpawnedSaveValues = string.Join("#NYA#", SpawnSaveValues);

        SpawnSaveValues.Clear();

        if (ObjectsToSave != null)
        {
            foreach (SaveableObject Obj in ObjectsToSave)
            {
                Obj.SaveFields();
            }
        }

        string SaveValues = string.Join("#MEOW#", ContentsToSave);

        string CombinedValues = string.Join("#(=^w^=)#", SpawnedSaveValues, SaveValues);

        SaveHandler.Instance.TempData[SceneManager.GetActiveScene().buildIndex] = CombinedValues;

        ContentsToSave.Clear();
    }

    string[] IndividualValues;

    [ContextMenu("LOAD")]
    public async void Load()
    {
        int LoadCount = 0;

        if (LoadData == string.Empty || LoadData == null)
            return;

        IndividualValues = LoadData.Split(new[] { "#(=^w^=)#" }, StringSplitOptions.None);

        string[] SpawnValues = IndividualValues[0].Split(new[] { "#NYA#" }, StringSplitOptions.None);

        await InstantiateSpawns();

        if (SaveableSpawns != null)
        {
            for (int j = 0; j < SaveableSpawns.Length; j++)
            {
                SaveablePrefabs Pfb = SaveableSpawns[j];

                for (int k = 0; k < SaveableSpawns[j].SpawnsToSave.Count; k++)
                {
                    SaveableObject Obj = Pfb.SpawnsToSave[k];

                    string[] LoadedValues = SpawnValues[j].Split(new[] { "#MEOW#" }, StringSplitOptions.None);

                    if (k != LoadedValues.Length - 1)
                    {
                        Obj.LoadFields(LoadedValues[LoadCount]);
                    }

                    LoadCount++;
                }

                LoadCount = 0;
            }
        }

        LoadCount = 0;

        if (ObjectsToSave != null)
        {
            for (int j = 0; j < ObjectsToSave.Length; j++)
            {
                SaveableObject Obj = ObjectsToSave[j];

                IndividualValues = LoadData.Split(new[] { "#(=^w^=)#" }, StringSplitOptions.None);

                string[] LoadedValues = IndividualValues[1].Split(new[] { "#MEOW#" }, StringSplitOptions.None);

                Obj.LoadFields(LoadedValues[LoadCount]);

                LoadCount++;
            }
        }

        LoadCount = 0;
    }

    public async Task InstantiateSpawns()
    {
        string[] IndividualValues = LoadData.Split(new[] { "#(=^w^=)#" }, StringSplitOptions.None);

        string[] SpawnValues = IndividualValues[0].Split(new[] { "#NYA#" }, StringSplitOptions.None);

        if (SaveableSpawns != null)
        {
            for (int j = 0; j < SaveableSpawns.Length; j++)
            {
                SaveablePrefabs Pfb = SaveableSpawns[j];

                int count = int.Parse(SpawnValues[j].Split(new[] { "#MEOW#" }, StringSplitOptions.None).Last());

                for (int m = 0; m < count; m++)
                {
                    Instantiate(Pfb.Prefab);
                }
            }
        }

        await Task.Yield();
    }
}

[Serializable]
public class SaveablePrefabs
{
    public GameObject Prefab;

    public List<SaveableObject> SpawnsToSave;

    [HideInInspector]
    public List<string> SpawnContentsToSave = new List<string>();
}

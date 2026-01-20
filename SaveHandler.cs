using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class SaveHandler : MonoBehaviour
{
    public bool SaveScene;

    public string[] SaveSlots = new string[1];

    public static SaveHandler Instance;

    public string[] SceneData;

    public void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        SceneData = new string[SceneManager.sceneCountInBuildSettings];

        if (!Directory.Exists(Application.dataPath + "/SaveFiles"))
        {
            Directory.CreateDirectory(Application.dataPath + "/SaveFiles");
        }
    }

    public void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public async void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        await DistributeData();

        SaveManager.Instance.Load();
    }

    public void LoadScene(int Index)
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.Save();
        }

        SceneManager.LoadSceneAsync(Index);
    }

    public void Save(int SlotNum = 1)
    {
        if (SaveScene)
            PlayerPrefs.SetInt("SceneIndex", SceneManager.GetActiveScene().buildIndex);

        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.Save();
        }

        string DataToSave = string.Join("#CATS!!!#", SceneData);

        File.WriteAllText(Application.dataPath + "/SaveFiles/" + SaveSlots[SlotNum - 1] + ".txt", DataToSave);
    }

    public async void Load(int SlotNum = 1)
    {
        if (SaveScene && PlayerPrefs.HasKey("SceneIndex"))
            SceneManager.LoadSceneAsync(PlayerPrefs.GetInt("SceneIndex"));

        string Data = File.ReadAllText(Application.dataPath + "/SaveFiles/" + SaveSlots[SlotNum - 1] + ".txt");

        SceneData = Data.Split(new[] { "#CATS!!!#" }, StringSplitOptions.None);

        await DistributeData();

        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.Load();
        }
    }

    public async Task DistributeData()
    {
        if (SaveManager.Instance != null)
        {
            if (SceneData[SceneManager.GetActiveScene().buildIndex] != string.Empty)
            {
                SaveManager.Instance.LoadData = SceneData[SceneManager.GetActiveScene().buildIndex];
            }

            else
            {
                await Task.Yield();
            }
        }

        await Task.Yield();
    }

    public void DeleteSaveFile(bool DeleteAll = false, int SlotNum = 1)
    {
        if (!DeleteAll)
        {
            if (File.Exists(Application.dataPath + "/SaveFiles/" + SaveSlots[SlotNum - 1] + ".txt"))
            {
                File.Delete(Application.dataPath + "/SaveFiles/" + SaveSlots[SlotNum - 1] + ".txt");
            }
        }

        else
        {
            string[] files = Directory.GetFiles(Application.dataPath + "/SaveFiles/");

            foreach(string file in files)
            {
                File.Delete(file);
            }
        }
    }

    public void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
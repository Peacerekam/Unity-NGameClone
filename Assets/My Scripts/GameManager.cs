using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;

    public MapData currentMap;

    void Awake()
    {
        if (instance == null) {
            instance = this;

        } else if (instance != this) {
            Destroy(gameObject);
        }
        
        DontDestroyOnLoad(gameObject);
        
    }

    public void StartMapEditor()
    {
        SceneManager.LoadScene("Map Editor");
    }
    
    public void StartMap()
    {
        SceneManager.LoadScene("Map Play");
    }

    public void OpenMainMenu()
    {
        currentMap = null;
        SceneManager.LoadScene("Main Menu");
    }

    public MapData[] GetLocalMaps() {
        
        string filePath = Path.Combine(Application.streamingAssetsPath, "local_maps.json");

        if (File.Exists(filePath)) {
            MapData[] allMaps = JsonHelper.FromJson<MapData>(File.ReadAllText(filePath));

            //Debug.Log("Loaded " + allMaps.Length + " maps from the file");
            return allMaps;
            
        } else {
            Debug.LogError("Couldn't find local_maps.json file");
            return null;
        }

    }


    public void AddHighscoreToCurrentMap(string score)
    {
        List<MapData> loadedData = GetLocalMaps().ToList();

        if (loadedData != null) {

            int index = loadedData.FindIndex(e => e.name == currentMap.name);

            currentMap.NewHighscore(score);
            loadedData[index] = currentMap;
            
            string filePath = Path.Combine(Application.streamingAssetsPath, "local_maps.json");
            string dataAsJson = JsonHelper.ToJson(loadedData.ToArray(), true);

            File.WriteAllText(filePath, dataAsJson);

        }

    }

}

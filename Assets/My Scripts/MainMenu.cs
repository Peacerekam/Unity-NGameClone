using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    
    [SerializeField]
    private GameObject mapButtonPrefab;

    [SerializeField]
    private Transform mapInfoButtonsContainer;

    [SerializeField]
    private GameObject mainMenu, localMapSelection, onlineMapSelection;

    void Start()
    {
        mainMenu.SetActive(true);
        onlineMapSelection.SetActive(false);

        // probably wont be needed if i make mapselection work for both
        localMapSelection.SetActive(false);
        
    }

    void Update() {
        if (EventSystem.current.currentSelectedGameObject == null) {
            GameManager.instance.currentMap = null;
        }
    }

    public void OpenMainMenu() {

        mainMenu.SetActive(true);
        onlineMapSelection.SetActive(false);

        // probably wont be needed if i make mapselection work for both
        localMapSelection.SetActive(false);
        
    }

    public void OpenMapEditor()
    {
        GameManager.instance.currentMap = null;
        GameManager.instance.StartMapEditor();
    }

    public void OpenOnlineMaps()
    {
        // ...
    }

    public void OpenLocalMaps()
    {

        RemoveAllButtons();
        
        MapData[] loadedData = GameManager.instance.GetLocalMaps();
        
        for (int i = 0; i < loadedData.Length; i++) {
            AddButton(i, loadedData[i]);
        }

        mainMenu.SetActive(false);
        onlineMapSelection.SetActive(false);

        localMapSelection.SetActive(true);

    }


    private void RemoveAllButtons()
    {
        for (int i = 0; i < mapInfoButtonsContainer.childCount; i++) {
            Destroy(mapInfoButtonsContainer.GetChild(i).gameObject);
        }
    }
    
    private void AddButton(int i, MapData mapData)
    {

        GameObject spawnedButton = (GameObject)GameObject.Instantiate(mapButtonPrefab);
        
        spawnedButton.transform.SetParent(mapInfoButtonsContainer);

        MapSelectButton button = spawnedButton.GetComponent<MapSelectButton>();
        button.SetUpButton(i, mapData);

    }

}

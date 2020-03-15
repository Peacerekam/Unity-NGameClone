using UnityEngine;
using UnityEngine.UI;

public class MapSelectButton : MonoBehaviour {

    [SerializeField]
    private Button button;

    [SerializeField]
    private Text mapID, mapName, mapAuthor, mapHighscore;

    private MapData mapData;

    private void Start()
    {
        button.onClick.AddListener(ButtonClick);
    }

    public void SetUpButton (int id, MapData currentMapData) {

        mapData = currentMapData;

        mapID.text = (++id).ToString();
        mapName.text = mapData.name;
        mapAuthor.text = mapData.author;
        mapHighscore.text = mapData.highscore;

    }

    private void ButtonClick()
    {
        GameManager.instance.currentMap = mapData;
        //GameManager.instance.StartMapEditor(mapData);
    }
	
}

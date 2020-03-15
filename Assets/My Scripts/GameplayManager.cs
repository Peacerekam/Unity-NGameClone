using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    private int tilesLayer;
    private int propsLayer;

    private string loadedMapTilesData;
    private string loadedMapPropsData;
    
    private static Dictionary<Vector3, string> busyTiles = new Dictionary<Vector3, string>();
    private static Dictionary<Vector3, string> busyProps = new Dictionary<Vector3, string>();

    private TilesAndProps TAP;
    private Vector3 zeroedPosition;
    private MapData[] loadedData;

    [SerializeField]
    private GameObject spawnpoint;

    [SerializeField]
    private GameObject endpoint;



    private void Awake()
    {
        foreach (Transform child in spawnpoint.transform) {
            child.gameObject.SetActive(false);
        }
    }

    void Start()
    {
        TAP = GameObject.Find("Game Manager").GetComponent<TilesAndProps>();

        tilesLayer = LayerMask.GetMask("Tiles");
        propsLayer = LayerMask.GetMask("Props");
        

        if (GameManager.instance.currentMap != null) {
            loadedMapTilesData = GameManager.instance.currentMap.tilesData;
            loadedMapPropsData = GameManager.instance.currentMap.propsData;
            LoadLevel();
        } else {
            loadedMapTilesData = "0,0,0";
            loadedMapPropsData = "0,0,0";
        }
        
    }
    


    public void LoadLevel()
    {

        ClearLevel();

        int currentRow = 0;
        float tileX, tileY;

        string[] tempTilesArray = loadedMapTilesData.Split(',').ToArray();
        int[,] mapTiles = new int[704, 2];

        for (int i = 0; i < tempTilesArray.Length; i++) {

            string[] t = tempTilesArray[i].Split('+');

            if (t.Length == 1) {
                mapTiles[i, 0] = int.Parse(t[0]);
                mapTiles[i, 1] = 0;
            } else {
                mapTiles[i, 0] = int.Parse(t[0]);
                mapTiles[i, 1] = int.Parse(t[1]);
            }

        }

        for (int i = 0; i < mapTiles.Length; i++) {
            if (i > 703) break;

            if (i % 32 == 0 && i != 0) {
                currentRow++;
            }

            if (mapTiles[i, 0] != 0) {

                tileX = ((i - (32f * currentRow)) * 0.5f) - 8f;
                tileY = (currentRow * 0.5f) - 5.5f;

                zeroedPosition = new Vector3(tileX, tileY, 0f);

                //var newTile = (GameObject)
                Instantiate(
                    TAP.tileTypes[mapTiles[i, 0]],
                    zeroedPosition,
                    Quaternion.Euler(0, 0, mapTiles[i, 1]));

                busyTiles[zeroedPosition] = TAP.tileNames.FirstOrDefault(x => x.Value == mapTiles[i, 0]).Key;

            }
        }

        currentRow = 0;
        tileX = 0;
        tileY = 0;

        string[] tempPropsArray = loadedMapPropsData.Split(',').ToArray();
        int[,] mapProps = new int[704, 2];

        for (int i = 0; i < tempPropsArray.Length; i++) {

            string[] t = tempPropsArray[i].Split('+');

            if (t.Length == 1) {
                mapProps[i, 0] = int.Parse(t[0]);
                mapProps[i, 1] = 0;
            } else {
                mapProps[i, 0] = int.Parse(t[0]);
                mapProps[i, 1] = int.Parse(t[1]);
            }

        }

        for (int i = 0; i < mapProps.Length; i++) {
            if (i > 703) break;

            if (i % 32 == 0 && i != 0) {
                currentRow++;
            }

            if (mapProps[i, 0] != 0) {

                tileX = ((i - (32f * currentRow)) * 0.5f) - 8f;
                tileY = (currentRow * 0.5f) - 5.5f;

                zeroedPosition = new Vector3(tileX, tileY, 0f);

                if (mapProps[i, 0] == 1) {
                    spawnpoint.transform.position = zeroedPosition;
                    spawnpoint.GetComponent<Spawnpoint>().SpawnPlayer(zeroedPosition);
                } else if (mapProps[i, 0] == 3) {
                    endpoint.transform.position = zeroedPosition;
                    //spawnpoint.GetComponent<Spawnpoint>().SpawnPlayer(zeroedPosition);
                } else {

                    Instantiate(
                        TAP.propTypes[mapProps[i, 0]],
                        zeroedPosition,
                        Quaternion.Euler(0, 0, mapProps[i, 1]));

                }

                busyProps[zeroedPosition] = TAP.tileNames.FirstOrDefault(x => x.Value == mapProps[i, 0]).Key;

            }
        }


    }

    private void GetLevelData(out string tiles, out string props)
    {
        int currentRow = 0;
        float tileX, tileY;
        string tempName;
        int tileRotation;
        Collider[] hitColliders;

        string newTilesData = "";
        string newPropsData = "";

        for (int i = 0; i < 704; i++) {

            if (i % 32 == 0 && i != 0) {
                currentRow++;
            }
            tileX = ((i - (32 * currentRow)) * 0.5f) - 8f;
            tileY = (currentRow * 0.5f) - 5.5f;

            hitColliders = Physics.OverlapSphere(new Vector3(tileX, tileY, 0f), 0.1f, tilesLayer);
            if (hitColliders.Length > 0) {

                tempName = hitColliders[0].transform.parent.name.Replace("(Clone)", "");

                tileRotation = (int)hitColliders[0].transform.parent.gameObject.transform.eulerAngles.z;
                newTilesData += TAP.tileNames[tempName] + "+" + tileRotation + ",";

            } else {
                // 0 = empty
                newTilesData += "0,";
            }

            hitColliders = Physics.OverlapSphere(new Vector3(tileX, tileY, 0f), 0.1f, propsLayer);
            if (hitColliders.Length > 0) {

                tempName = hitColliders[0].name.Replace("(Clone)", "");

                tileRotation = (int)hitColliders[0].gameObject.transform.eulerAngles.z;
                newPropsData += TAP.propNames[tempName] + "+" + tileRotation + ",";

            } else {
                // 0 = empty
                newPropsData += "0,";
            }

        }

        tiles = newTilesData.Substring(0, newTilesData.Length - 1);
        props = newPropsData.Substring(0, newPropsData.Length - 1);
    }
    
    public void ClearLevel()
    {

        // pop up "are you sure?"

        GameObject[] currentMapTiles = GameObject.FindGameObjectsWithTag("Map Element");
        GameObject[] currentMapProps = GameObject.FindGameObjectsWithTag("Prop Element");

        spawnpoint.GetComponent<Spawnpoint>().SpawnPlayer(new Vector3(0, -100, 0));

        for (int i = 0; i < currentMapTiles.Length; i++) {
            Destroy(currentMapTiles[i]);
        };

        for (int i = 0; i < currentMapProps.Length; i++) {
            Destroy(currentMapProps[i]);
        }

        busyTiles = new Dictionary<Vector3, string>();
    }
    


    public void OpenMainMenu()
    {
        GameManager.instance.OpenMainMenu();
    }

}

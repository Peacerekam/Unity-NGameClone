using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;

public class MapLogic : MonoBehaviour {

    private int tilesLayer;
    private int propsLayer;

    private string loadedMapTilesData;
    private string loadedMapPropsData;

    private static Dictionary<Vector3, string> busyTiles;
    private static Dictionary<Vector3, string> busyProps;

    private TilesAndProps TAP;
    private Vector3 zeroedPosition;
    private MapData[] loadedData;

    [SerializeField]
    private GameObject spawnpoint;

    [SerializeField]
    private GameObject endpoint;
    
    [SerializeField]
    private GameObject targeter;

    private Camera myCamera;
    
    [SerializeField]
    private InputField mapName, mapAuthor;

    [SerializeField]
    private Button saveButton;



    private void Awake()
    {
        saveButton.interactable = false;
    }

    void Start()
    {
        busyTiles = new Dictionary<Vector3, string>();
        busyProps = new Dictionary<Vector3, string>();

        TAP = GameObject.Find("Game Manager").GetComponent<TilesAndProps>();

        tilesLayer = LayerMask.GetMask("Tiles");
        propsLayer = LayerMask.GetMask("Props");

        myCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        
        if (GameManager.instance.currentMap != null) {
            loadedMapTilesData = GameManager.instance.currentMap.tilesData;
            loadedMapPropsData = GameManager.instance.currentMap.propsData;
            LoadLevel();
        } else {
            loadedMapTilesData = "0,0,0";
            loadedMapPropsData = "0,0,0";
        }

        mapName.onValueChanged.AddListener( s =>  mapName.textComponent.color = Color.black );
        mapAuthor.onValueChanged.AddListener( s =>  mapAuthor.textComponent.color = Color.black );
    }

    void SaveButtonHighlight() {
        if (mapName.text.Length == 0 || mapAuthor.text.Length == 0) {
            saveButton.interactable = false;
        } else {
            saveButton.interactable = true;
        }
    }

	void Update () {

        SaveButtonHighlight();
        TargeterPosition();

        if (EventSystem.current.IsPointerOverGameObject() || EventSystem.current.currentSelectedGameObject != null ) {
            // don't place or delete tiles when interacting with ui
            return;
        }
        
        HandleTiles();
        HandleProps();
    }


    private void HandleTiles()
    {

        PlaceCube();
        PlaceTriangle();
        PlaceSphere();

        DeleteTile();

    }

    private void HandleProps()
    {

        PlaceSpawnpoint();
        PlaceEndpoint();

        PlaceMine();

    }


    private void TargeterPosition()
    {

        zeroedPosition = myCamera.ScreenToWorldPoint(Input.mousePosition);

        zeroedPosition.x = Mathf.Round(2 * zeroedPosition.x) / 2;
        zeroedPosition.y = Mathf.Round(2 * zeroedPosition.y) / 2;
        zeroedPosition.z = 0f;

        if (zeroedPosition.x > 7.5f) {
            zeroedPosition.x = 7.5f;
        } else if (zeroedPosition.x < -8f) {
            zeroedPosition.x = -8f;
        }

        if (zeroedPosition.y > 5f) {
            zeroedPosition.y = 5f;
        } else if (zeroedPosition.y < -5.5f) {
            zeroedPosition.y = -5.5f;
        }

        targeter.transform.position = zeroedPosition;

    }
    
    private void PlaceMine() {
        if (Input.GetKey(KeyCode.Alpha3)) {

            if (busyProps.ContainsKey(zeroedPosition)) return;
            
            Instantiate(TAP.propTypes[2], zeroedPosition, transform.rotation);

            busyProps[zeroedPosition] = "Mine";

        }
    }

    private void PlaceSpawnpoint() {

        if (Input.GetKey(KeyCode.Z)) {

            if (busyTiles.ContainsKey(zeroedPosition)) return;
            if (busyProps.ContainsKey(zeroedPosition)) return;

            
            busyProps.Remove(busyProps.FirstOrDefault(x => x.Value == "Spawnpoint").Key);
            
            busyProps[zeroedPosition] = "Spawnpoint";

            spawnpoint.GetComponent<Spawnpoint>().SpawnPlayer(zeroedPosition);


        }
    }
    
    private void PlaceEndpoint()
    {

        if (Input.GetKey(KeyCode.X)) {

            if (busyTiles.ContainsKey(zeroedPosition)) return;
            if (busyProps.ContainsKey(zeroedPosition)) return;


            busyProps.Remove(busyProps.FirstOrDefault(x => x.Value == "Endpoint").Key);

            endpoint.transform.position = zeroedPosition;

            busyProps[zeroedPosition] = "Endpoint";
            
            
        }
    }

    private void DeleteTile() {

        if (Input.GetKey(KeyCode.Backspace)) {
            if (busyProps.ContainsKey(zeroedPosition)) {

                Collider[] hitColliders = Physics.OverlapSphere(zeroedPosition, 0.1f, propsLayer);
                if (hitColliders.Length > 0) {
                    if (hitColliders[0].gameObject.name != "Spawnpoint" && hitColliders[0].gameObject.name != "Endpoint") {
                        Destroy(hitColliders[0].gameObject);
                    }
                }

                busyProps.Remove(zeroedPosition);

            } else if (busyTiles.ContainsKey(zeroedPosition)) {

                Collider[] hitColliders = Physics.OverlapSphere(zeroedPosition, 0.1f, tilesLayer);
                if (hitColliders.Length > 0) {
                    Destroy(hitColliders[0].gameObject.transform.parent.gameObject);
                }

                busyTiles.Remove(zeroedPosition);

            }
            

        }
        
    }

    private void PlaceCube() {

        if (Input.GetKey(KeyCode.Alpha1) || Input.GetMouseButton(0)) {

            if (busyTiles.ContainsKey(zeroedPosition)) return;

            //var newTile = (GameObject)
            Instantiate(TAP.tileTypes[1], zeroedPosition, transform.rotation);

            busyTiles[zeroedPosition] = "Cube Element";
            //busyTiles.Add(zeroedPosition);
        }

    }

    private void PlaceTriangle() {

        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            if (busyTiles.ContainsKey(zeroedPosition) && busyTiles[zeroedPosition] == "Triangle Element") {

                // rotate triangle element by 90 degrees
                //Debug.Log("triangle rotate?");

                Collider[] hitColliders = Physics.OverlapSphere(zeroedPosition, 0.1f);
                if (hitColliders.Length > 0) {

                    int nextAngle = (int)hitColliders[0].gameObject.transform.parent.gameObject.transform.rotation.eulerAngles.z + 90;
                    hitColliders[0].gameObject.transform.parent.gameObject.transform.rotation = Quaternion.Euler(0, 0, nextAngle);

                }
            }
        }

        if (Input.GetKey(KeyCode.Alpha2)) {

            if (busyTiles.ContainsKey(zeroedPosition)) return;

            //var newTile = (GameObject)
            Instantiate(TAP.tileTypes[2], zeroedPosition, transform.rotation);

            busyTiles[zeroedPosition] = "Triangle Element";
            //busyTiles.Add(zeroedPosition);
        }


    }

    private void PlaceSphere()
    {

        if (Input.GetKey(KeyCode.Alpha4) || Input.GetMouseButton(0)) {

            if (busyTiles.ContainsKey(zeroedPosition)) return;

            //var newTile = (GameObject)
            Instantiate(TAP.tileTypes[3], zeroedPosition, transform.rotation);

            busyTiles[zeroedPosition] = "Sphere Element";
            //busyTiles.Add(zeroedPosition);
        }

    }



    public void Savelevel() {

        List<MapData> loadedData = GameManager.instance.GetLocalMaps().ToList();

        if (loadedData != null) {

            string tempMapName = mapName.text;
            string tempMapAuthor = mapAuthor.text;

            GetLevelData(out loadedMapTilesData, out loadedMapPropsData);

            MapData newMap = new MapData(tempMapName, tempMapAuthor, loadedMapTilesData, loadedMapPropsData);

            loadedData.Add(newMap);

            string filePath = Path.Combine(Application.streamingAssetsPath, "local_maps.json");
            string dataAsJson = JsonHelper.ToJson(loadedData.ToArray(), true);

            File.WriteAllText(filePath, dataAsJson);

        }
        
    }
    
    public void LoadLevel () {

        if (GameManager.instance.currentMap == null) {
            string tempMapName = mapName.text;
            string tempMapAuthor = mapAuthor.text;

            GetLevelData(out loadedMapTilesData, out loadedMapPropsData);

            GameManager.instance.currentMap = new MapData(tempMapName, tempMapAuthor, loadedMapTilesData, loadedMapPropsData);
        }

        ClearLevel();
        
        int currentRow = 0;
        float tileX, tileY;
        
        string[] tempTilesArray = loadedMapTilesData.Split(',').ToArray();
        int[,] mapTiles = new int[704, 2];

        for (int i = 0; i < tempTilesArray.Length; i++) {

            string[] t = tempTilesArray[i].Split('+');

            if (t.Length == 1) {
                mapTiles[ i, 0 ] = int.Parse(t[0]);
                mapTiles[ i, 1 ] = 0;
            } else {
                mapTiles[ i, 0 ] = int.Parse(t[0]);
                mapTiles[ i, 1 ] = int.Parse(t[1]);
            }

        }
        
        for (int i = 0; i < mapTiles.Length; i++){
			if (i > 703) break;
			
			if (i % 32 == 0 && i != 0) {
				currentRow++;
			}
			
			if (mapTiles[i, 0] != 0){
				
				tileX = ((i - (32f * currentRow)) * 0.5f) - 8f;
				tileY = (currentRow * 0.5f) - 5.5f;
				
				zeroedPosition = new Vector3(tileX, tileY, 0f);

                //var newTile = (GameObject)
                Instantiate (
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

        GameManager.instance.currentMap = null;
    }

    private void GetLevelData(out string tiles, out string props) {
		int currentRow = 0;
        float tileX, tileY;
        string tempName;
        int tileRotation;
        Collider[] hitColliders;

        string newTilesData = "";
        string newPropsData = "";

        for (int i = 0; i<704; i++){
			
			if (i%32 == 0 && i != 0) {
				currentRow++;
			}
			tileX = ((i - (32*currentRow))*0.5f) - 8f;
			tileY = (currentRow*0.5f) - 5.5f;
			
			hitColliders = Physics.OverlapSphere(new Vector3(tileX, tileY, 0f), 0.1f, tilesLayer);
			if (hitColliders.Length > 0){

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
    
    public void ClearLevel() {

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

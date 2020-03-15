using System.Collections.Generic;
using UnityEngine;

public class TilesAndProps : MonoBehaviour
{

    public GameObject[] tileTypes;
    public GameObject[] propTypes;
    
    public readonly Dictionary<string, int> tileNames = new Dictionary<string, int>();
    public readonly Dictionary<string, int> propNames = new Dictionary<string, int>();
    
    void Start()
    {

        // Tiles
        tileNames["Cube Element"] = 1;
        tileNames["Triangle Element"] = 2;
        tileNames["Sphere Element"] = 3;

        // Props
        propNames["Spawnpoint"] = 1;
        propNames["Mine"] = 2;
        propNames["Endpoint"] = 3;

    }

}

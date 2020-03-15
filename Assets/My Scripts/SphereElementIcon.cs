using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereElementIcon : MonoBehaviour {
    
    [SerializeField]
    private GameObject elToHide1, elToHide2;

    private GameObject gameplayLogic;

    void Awake () {
        gameplayLogic = GameObject.Find("Gameplay Manager");

        if (gameplayLogic != null) {
            elToHide1.SetActive(false);
            elToHide2.SetActive(false);
        }
	}
}

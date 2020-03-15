using UnityEngine;
using UnityEngine.UI;

public class PressEdit : MonoBehaviour {

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.interactable = false;
    }

    void Start () {
        button.onClick.AddListener(ButtonClick);
    }

    void Update()
    {
        if (GameManager.instance.currentMap == null) {
            button.interactable = false;
        } else {
            button.interactable = true;
        }
    }

    private void ButtonClick()
    {
        if (GameManager.instance.currentMap != null) {
            GameManager.instance.StartMapEditor();
        }
    }

}

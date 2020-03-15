using UnityEngine;
using UnityEngine.UI;

public class PopUpLogic : MonoBehaviour {

    [SerializeField]
    private GameObject popup;

    [SerializeField]
    private Text mainText, subText;
    
    private void Awake()
    {
        popup.SetActive(false);
        mainText.text = "";
        subText.text = "";
    }
    
	public void ShowPopup (string main, string sub) {
        popup.SetActive(true);
        mainText.text = main;
        subText.text = sub;
	}

    public void HidePopup()
    {
        popup.SetActive(false);
        mainText.text = "";
        subText.text = "";
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ReloadButton : MonoBehaviour {

    private Button button;

    [SerializeField]
    private GameplayManager gameplay;

    [SerializeField]
    private GameplayLogic gameplayLogic;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.interactable = true;
    }

    void Start()
    {
        button.onClick.AddListener(ButtonClick);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && button.interactable) {
            ButtonClick();
        }
    }

    IEnumerator DelayInteraction()
    {
        button.interactable = false;
        yield return new WaitForSeconds(1f);
        button.interactable = true;
    }

    public void ButtonClick()
    {
        gameplayLogic.RestartTime();
        gameplay.LoadLevel();
        StartCoroutine("DelayInteraction");
    }

}

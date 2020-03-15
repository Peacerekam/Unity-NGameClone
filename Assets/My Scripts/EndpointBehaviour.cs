using UnityEngine;

public class EndpointBehaviour : MonoBehaviour
{

    private GameObject gameplayLogic;
    private GameplayLogic gameplayLogicScript;
    private int playerLayer = 9;

    private void Awake()
    {
        gameplayLogic = GameObject.Find("Gameplay Manager");

        if (gameplayLogic) {
            gameplayLogicScript = gameplayLogic.GetComponent<GameplayLogic>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // check if editor mode
        if (!gameplayLogic) return;
        

        if (other.gameObject.layer == playerLayer) {
            gameplayLogicScript.FinishLevel();
        }
    }
    
}

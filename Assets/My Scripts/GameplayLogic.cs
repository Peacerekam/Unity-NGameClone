using UnityEngine;
using UnityEngine.UI;

public class GameplayLogic : MonoBehaviour {

    private float timer;

    [SerializeField]
    private TextMesh currentScoreAbovePlayer;

    [SerializeField]
    private Text currentScore;

    [SerializeField]
    private Text bestScore;
    
    private float currentT, bestT;

    [HideInInspector]
    public bool stopMeasuring = false;

    [SerializeField]
    private GameObject endpoint;

    [SerializeField]
    private PopUpLogic popup;

    [SerializeField]
    private MyCharacterController player;

    void Start ()
    {
        bestScore.text = GameManager.instance.currentMap.highscore == "" ? "0.00" : GameManager.instance.currentMap.highscore;

        timer = 0f;
        currentScoreAbovePlayer.color = new Color(0f, 0f, 0f);
        currentScore.color = new Color(0f, 0f, 0f);
    }
    
    public void FinishLevel() {

        stopMeasuring = true;

        if (timer < float.Parse(bestScore.text) || bestScore.text == "0.00") {

            if (bestScore.text == timer.ToString("0.00")) return; // tied

            GameManager.instance.AddHighscoreToCurrentMap(timer.ToString("0.00"));
            //Debug.Log("new highscore!!!");

            popup.ShowPopup("Winner!", "You've got a new highscore!");

            bestScore.text = timer.ToString("0.00");

        } else {

            popup.ShowPopup("Winner!", "---");
            //Debug.Log("couldn't beat the highscore");

        }
        
    }

    public void GameOver() {

        currentScoreAbovePlayer.text = "";
        popup.ShowPopup("You died!", "Press <b>Enter</b> to resume...");

        stopMeasuring = true;

        // death?
        // prop to re-load?

    }

    public void RestartTime()
    {
        player.Respawn();
        popup.HidePopup();
        stopMeasuring = false;
        timer = 0f;
        currentScoreAbovePlayer.color = new Color(0f, 0f, 0f);
        currentScore.color = new Color(0f, 0f, 0f);
    }

	void Update ()
    {
        if (!stopMeasuring) {
            timer += Time.deltaTime;

            currentScore.text = timer.ToString("0.00");
            currentScoreAbovePlayer.text = currentScore.text;
            currentScoreAbovePlayer.fontSize = 25 + (int)timer;

            currentT = float.Parse(currentScore.text);
            bestT = float.Parse(bestScore.text);

            if (bestT != 0) {
                float diff = (bestT + 2.5f) - currentT;

                if (diff < 5f) {
                    currentScoreAbovePlayer.color = new Color(1 - (diff / 5), 0f, 0f);
                    currentScore.color = new Color(1 - (diff / 5), 0f, 0f);
                }
            }
        }

    }
}

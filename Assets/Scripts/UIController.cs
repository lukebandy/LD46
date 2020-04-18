using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour {

    [SerializeField]
    private GameObject main;
    [SerializeField]
    private TextMeshProUGUI mainHighscore;

    [SerializeField]
    private GameObject hud;
    [SerializeField]
    private TextMeshProUGUI hudBalance;
    [SerializeField]
    private TextMeshProUGUI hudSeason;
    [SerializeField]
    private TextMeshProUGUI hudWaterRemaining;

    [SerializeField]
    private GameObject pause;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        if (GameController.main.gameState == GameController.GameStates.MainMenu) {
            main.SetActive(true);
        }
        else
            main.SetActive(false);

        if (GameController.main.gameState == GameController.GameStates.Gameplay) {
            hud.SetActive(true);
            hudBalance.text = "£" + GameController.main.farmValue.ToString();
            hudSeason.text = GameController.main.season.ToString() + " - Year " + GameController.main.year.ToString();
            hudWaterRemaining.text = Mathf.Clamp(Mathf.RoundToInt(100 * (Player.main.hoseRemaining / Player.main.hoseCapacity)), 0, 100).ToString() + "%";
        }
        else
            hud.SetActive(false);

        if (GameController.main.gameState == GameController.GameStates.Paused) {
            pause.SetActive(true);
        }
        else
            pause.SetActive(false);
    }

    public void Play() {

    }

    public void Quit() {

    }
}

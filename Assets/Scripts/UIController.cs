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
    private TextMeshProUGUI hudDeaths;
    [SerializeField]
    private TextMeshProUGUI hudWaterRemaining;

    [SerializeField]
    private GameObject pause;

    [SerializeField]
    private GameObject gameover;

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
            hudDeaths.text = "Deaths: " + Plant.deaths.ToString() + "/10";
            hudWaterRemaining.text = Mathf.Clamp(Mathf.RoundToInt(100 * (Player.main.hoseRemaining / Player.main.hoseCapacity)), 0, 100).ToString() + "%";
        }
        else
            hud.SetActive(false);

        if (GameController.main.gameState == GameController.GameStates.Paused) {
            pause.SetActive(true);
        }
        else
            pause.SetActive(false);

        if (GameController.main.gameState == GameController.GameStates.Outro && Plant.deaths >= 10) {
            gameover.SetActive(true);
        }
        else {
            gameover.SetActive(false);
        }
    }

    public void Play() {
        GameController.main.gameState = GameController.GameStates.Intro;
    }

    public void Resume() {
        GameController.main.gameState = GameController.GameStates.Gameplay;
    }

    public void MainMenu() {
        GameController.main.camera.transform.position = Player.main.transform.GetChild(0).transform.position;
        GameController.main.camera.transform.rotation = Player.main.transform.GetChild(0).transform.rotation;
        GameController.main.camera.gameObject.SetActive(true);
        GameController.main.gameState = GameController.GameStates.Outro;
    }

    public void Quit() {
        Application.Quit();
    }
}

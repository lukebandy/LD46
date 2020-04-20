using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour {

    [SerializeField]
    private GameObject main;
    [SerializeField]
    private TextMeshProUGUI mainHighscore;
    [SerializeField]
    private GameObject mainQuit;

    [SerializeField]
    private GameObject tutorial;
    private float tutorialRemaining;

    [SerializeField]
    private GameObject hud;
    [SerializeField]
    private Image hudSeason;
    [SerializeField]
    private Sprite hudSeasonAutumn;
    [SerializeField]
    private Sprite hudSeasonSpring;
    [SerializeField]
    private Sprite hudSeasonSummer;
    [SerializeField]
    private Sprite hudSeasonWinter;
    [SerializeField]
    private TextMeshProUGUI hudSeasonYear;
    [SerializeField]
    private Transform hudLives;
    [SerializeField]
    private Sprite hudLivesPlant;
    [SerializeField]
    private Sprite hudLivesSkull;
    [SerializeField]
    private RectTransform hudWaterRemaining;
    private float hudWaterRemainingWidth;

    [SerializeField]
    private GameObject pause;
    [SerializeField]
    private GameObject pauseQuit;

    [SerializeField]
    private GameObject gameover;

    [SerializeField]
    private GameObject settings;
    private bool settingsOpen;

    // Start is called before the first frame update
    void Start() {
        settingsOpen = false;

        mainQuit.SetActive(Application.platform != RuntimePlatform.WebGLPlayer);
        pauseQuit.SetActive(Application.platform != RuntimePlatform.WebGLPlayer);

        hudWaterRemainingWidth = hudWaterRemaining.sizeDelta.x;
    }

    // Update is called once per frame
    void Update() {
        if (GameController.main.gameState == GameController.GameStates.MainMenu && !settingsOpen) {
            main.SetActive(true);
        }
        else
            main.SetActive(false);

        if (GameController.main.gameState == GameController.GameStates.Intro) {
            tutorial.SetActive(true);
        }
        else
            tutorial.SetActive(false);

        if (GameController.main.gameState == GameController.GameStates.Gameplay) {
            hud.SetActive(true);
            switch (GameController.main.season) {
                case GameController.Seasons.Spring:
                    hudSeason.sprite = hudSeasonSpring;
                    break;
                case GameController.Seasons.Autumn:
                    hudSeason.sprite = hudSeasonAutumn;
                    break;
                case GameController.Seasons.Summer:
                    hudSeason.sprite = hudSeasonSummer;
                    break;
                case GameController.Seasons.Winter:
                    hudSeason.sprite = hudSeasonWinter;
                    break;
            }
            hudSeasonYear.text = " Year " + GameController.main.year.ToString();
            for (int i = 0; i < 10; i++) {
                if (Plant.deaths <= i)
                    hudLives.GetChild(9 - i).GetComponent<Image>().sprite = hudLivesPlant;
                else
                    hudLives.GetChild(9 - i).GetComponent<Image>().sprite = hudLivesSkull;
            }
            hudWaterRemaining.sizeDelta = new Vector2(hudWaterRemainingWidth * (Player.main.hoseRemaining / Player.main.hoseCapacity), hudWaterRemaining.sizeDelta.y);
        }
        else
            hud.SetActive(false);

        if (GameController.main.gameState == GameController.GameStates.Paused && !settingsOpen) {
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

        if (settingsOpen) {
            settings.SetActive(true);
        }
        else {
            settings.SetActive(false);
        }
    }

    public void Play() {
        GameController.main.gameState = GameController.GameStates.Intro;
        Player.main.Setup();
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

    public void Settings() {
        settingsOpen = true;
    }

    public void SettingsExit() {
        settingsOpen = false;
    }
}

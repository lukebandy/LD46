using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    // Static variables
    public static GameController main;
    public enum Seasons { Spring, Autumn, Summer, Winter }

    // Prefabs
    [SerializeField]
    private GameObject prefabTile;
    [SerializeField]
    private GameObject prefabFences;
    [SerializeField]
    private GameObject prefabTap;
    [SerializeField]
    private GameObject prefabWorker;

    // Public variables
    [HideInInspector]
    public Seasons season;
    [HideInInspector]
    public int year;
    [HideInInspector]
    public int farmValue;

    // Private variables
    private float seasonProgress;
    [SerializeField]
    private float seasonLength;
    [SerializeField]
    private new Camera camera;
    private Vector3 cameraPosition;
    private Quaternion cameraRotation;
    public enum GameStates { MainMenu, Intro, Gameplay, Paused, Outro }
    public GameStates gameState;

    // Start is called before the first frame update
    void Start() {
        main = this;
        GenerateMap();

        Object[] objs = Resources.LoadAll("Plants", typeof(PlantData));
        Plant.plantDataAll = new PlantData[objs.Length];
        for (int i = 0; i < objs.Length; i++)
            Plant.plantDataAll[i] = (PlantData)objs[i];


        cameraPosition = camera.transform.position;
        cameraRotation = camera.transform.rotation;

        gameState = GameStates.MainMenu;
    }

    // Update is called once per frame
    void Update() {
        Cursor.lockState = CursorLockMode.None;

        switch(gameState) {
            case GameStates.MainMenu:
                if (Input.GetKeyDown("space")) {
                    gameState = GameStates.Intro;
                }
                break;

            case GameStates.Intro:
                // Move camera
                camera.transform.position = Vector3.MoveTowards(camera.transform.position, Player.main.transform.GetChild(0).position, Time.deltaTime * 10.0f);
                camera.transform.rotation = Quaternion.RotateTowards(camera.transform.rotation, Player.main.transform.GetChild(0).rotation, Time.deltaTime * 10.0f);

                if (camera.transform.position == Player.main.transform.GetChild(0).position && camera.transform.rotation == Player.main.transform.GetChild(0).rotation) {
                    // Setup game
                    season = Seasons.Spring;
                    year = 1;
                    seasonProgress = 0.0f;
                    // Spawn workers
                    for (int i = 0; i < 2; i++)
                        Instantiate(prefabWorker, new Vector3(Random.Range(0, Tile.tiles.GetLength(0) - 1.0f), 0, Random.Range(0, Tile.tiles.GetLength(1) - 1.0f)), Quaternion.identity, transform.Find("Workers"));
                    // Start game
                    gameState = GameStates.Gameplay;
                    camera.gameObject.SetActive(false);
                }
                break;

            case GameStates.Gameplay:
                if (Input.GetKeyDown("escape"))
                    gameState = GameStates.Paused;

                else {
                    Cursor.lockState = CursorLockMode.Locked;

                    if (Plant.deaths > 10) {
                        Debug.Log("10 deaths - game over");
                        camera.gameObject.SetActive(true);
                        gameState = GameStates.Outro;
                    }

                    seasonProgress += Time.deltaTime;
                    if (seasonProgress >= seasonLength) {
                        switch (season) {
                            case Seasons.Spring:
                                season = Seasons.Summer;
                                break;
                            case Seasons.Summer:
                                season = Seasons.Autumn;
                                break;
                            case Seasons.Autumn:
                                season = Seasons.Winter;
                                break;
                            case Seasons.Winter:
                                season = Seasons.Spring;
                                year++;
                                break;
                        }
                        Plant.deaths = 0;
                        seasonProgress -= seasonLength;
                    }
                }
                break;

            case GameStates.Paused:
                if (Input.GetKeyDown("escape"))
                    gameState = GameStates.Gameplay;
                break;

            case GameStates.Outro:
                // Move camera
                camera.transform.position = Vector3.MoveTowards(camera.transform.position, cameraPosition, Time.deltaTime * 10.0f);
                camera.transform.rotation = Quaternion.RotateTowards(camera.transform.rotation, cameraRotation, Time.deltaTime * 10.0f);
                break;
        }
    }

    public void GenerateMap(int sizeX = 20, int sizeY = 20, int taps = 3) {
        // Tiles
        int child = 0;
        Tile.tiles = new Tile[sizeX, sizeY];
        for (int x = 0; x < sizeX; x++) {
            for (int y = 0; y < sizeY; y++) {
                if (child + 1 > transform.Find("Tiles").childCount)
                    Instantiate(prefabTile, transform.Find("Tiles"));
                transform.Find("Tiles").GetChild(child).position = new Vector3(x, 0, y);
                transform.Find("Tiles").GetChild(child).gameObject.SetActive(true);
                Tile.tiles[x, y] = transform.Find("Tiles").GetChild(child).GetComponent<Tile>();
                child++;
            }
        }
        while (child + 1 < transform.Find("Tiles").childCount) {
            transform.Find("Tiles").GetChild(child).gameObject.SetActive(false);
            child++;
        }

        // Fences
        child = 0;
        for (int x = 0; x < sizeX; x++) {
            if (child + 1 > transform.Find("Fences").childCount)
                Instantiate(prefabFences, transform.Find("Fences"));
            transform.Find("Fences").GetChild(child).position = new Vector3(x, 0, -1);
            transform.Find("Fences").GetChild(child).rotation = Quaternion.identity;
            transform.Find("Fences").GetChild(child).gameObject.SetActive(true);
            child++;

            if (child + 1 > transform.Find("Fences").childCount)
                Instantiate(prefabFences, transform.Find("Fences"));
            transform.Find("Fences").GetChild(child).position = new Vector3(x, 0, sizeY);
            transform.Find("Fences").GetChild(child).rotation = Quaternion.Euler(0, 180, 0);
            transform.Find("Fences").GetChild(child).gameObject.SetActive(true);
            child++;
        }
        for (int y = 0; y < sizeY; y++) {
            if (child + 1 > transform.Find("Fences").childCount)
                Instantiate(prefabFences, transform.Find("Fences"));
            transform.Find("Fences").GetChild(child).position = new Vector3(-1, 0, y);
            transform.Find("Fences").GetChild(child).rotation = Quaternion.Euler(0, 90, 0);
            transform.Find("Fences").GetChild(child).gameObject.SetActive(true);
            child++;

            if (child + 1 > transform.Find("Fences").childCount)
                Instantiate(prefabFences, transform.Find("Fences"));
            transform.Find("Fences").GetChild(child).position = new Vector3(sizeX, 0, y);
            transform.Find("Fences").GetChild(child).rotation = Quaternion.Euler(0, 270, 0);
            transform.Find("Fences").GetChild(child).gameObject.SetActive(true);
            child++;
        }
        while (child + 1 < transform.Find("Fences").childCount) {
            transform.Find("Fences").GetChild(child).gameObject.SetActive(false);
            child++;
        }

        // Taps
        child = 0;
        int tapsPlaced = 0;
        while (tapsPlaced < taps) {
            Tile tile = Tile.tiles[Random.Range(0, Tile.tiles.GetLength(0)), Random.Range(0, Tile.tiles.GetLength(1))];
            if (!tile.tap) {
                tile.tap = true;
                if (child + 1 > transform.Find("Taps").childCount)
                    Instantiate(prefabTap, transform.Find("Taps"));
                transform.Find("Taps").GetChild(child).position = tile.transform.position;
                tapsPlaced++;
                child++;
            }
        }
        while (child + 1 < transform.Find("Taps").childCount) {
            transform.Find("Taps").GetChild(child).gameObject.SetActive(false);
            child++;
        }
    }
}

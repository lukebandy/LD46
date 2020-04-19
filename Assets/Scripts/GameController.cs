using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    // Static variables
    public static GameController main;
    public enum Seasons { Spring, Autumn, Summer, Winter }
    public enum GameStates { MainMenu, Intro, Gameplay, Paused, Outro }

    // Prefabs
    [SerializeField]
    private GameObject prefabTile;
    [SerializeField]
    private GameObject prefabFences;
    [SerializeField]
    private GameObject prefabTap;
    [SerializeField]
    private GameObject prefabWorker;
    [SerializeField]
    private GameObject prefabGrass;
    [SerializeField]
    private GameObject prefabTree;

    // Public variables
    [HideInInspector]
    public Seasons season;
    [HideInInspector]
    public int year;
    [HideInInspector]
    public int farmValue;
    [HideInInspector]
    public GameStates gameState;
    public new Camera camera;

    // Private variables
    private float seasonProgress;
    [SerializeField]
    private float seasonLength;
    private Vector3 cameraPosition;
    private Quaternion cameraRotation;


    // Start is called before the first frame update
    void Start() {
        main = this;
        GenerateMap();

        Object[] objs = Resources.LoadAll("Plants", typeof(PlantData));
        Plant.plantDataAll = new PlantData[objs.Length];
        for (int i = 0; i < objs.Length; i++)
            Plant.plantDataAll[i] = (PlantData)objs[i];

        camera.gameObject.SetActive(true);
        cameraPosition = camera.transform.position;
        cameraRotation = camera.transform.rotation;

        gameState = GameStates.MainMenu;

        // Spawn workers
        for (int i = 0; i < 3; i++)
            Instantiate(prefabWorker, new Vector3(Random.Range(0, Tile.tiles.GetLength(0) - 1.0f), 0, Random.Range(0, Tile.tiles.GetLength(1) - 1.0f)), Quaternion.identity, transform.Find("Workers")).GetComponent<Worker>().Setup();
    }

    // Update is called once per frame
    void Update() {
        Cursor.lockState = CursorLockMode.None;

        switch(gameState) {
            case GameStates.MainMenu:
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
                            break;
                    }
                    seasonProgress -= seasonLength;
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
                    farmValue = 0;
                    Plant.deaths = 0;
                    // Setup workers
                    for (int i = 1; i < transform.Find("Workers").childCount; i++)
                        transform.Find("Workers").GetChild(i).gameObject.SetActive(false);
                    transform.Find("Workers").GetChild(0).GetComponent<Worker>().Setup();
                    // Reset map
                    foreach (Transform plant in transform.Find("Plants"))
                        plant.gameObject.SetActive(false);
                    for (int x = 0; x < Tile.tiles.GetLength(0); x++) {
                        for (int y = 0; y < Tile.tiles.GetLength(1); y++) {
                            Tile.tiles[x, y].plant = null;
                            Tile.tiles[x, y].plantInProgress = false;
                        }
                    }
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

                    if (Plant.deaths >=  10) {
                        camera.gameObject.SetActive(true);
                        gameState = GameStates.Outro;
                    }

                    seasonProgress += Time.deltaTime;
                    if (seasonProgress >= seasonLength) {
                        // Change season
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
                                // Spawn another worker
                                bool spawned = false;
                                foreach (Transform worker in transform.Find("Workers")) {
                                    if (!worker.gameObject.activeSelf) {
                                        worker.gameObject.SetActive(true);
                                        worker.GetComponent<Worker>().Setup();
                                        spawned = true;
                                        break;
                                    }
                                }
                                if (!spawned) {
                                    Instantiate(prefabWorker, new Vector3(Random.Range(0, Tile.tiles.GetLength(0) - 1.0f), 0, Random.Range(0, Tile.tiles.GetLength(1) - 1.0f)), Quaternion.identity, transform.Find("Workers")).GetComponent<Worker>().Setup();
                                }
                                break;
                        }
                        //Plant.deaths = 0;
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

                if (camera.transform.position == cameraPosition && camera.transform.rotation == cameraRotation) {
                    gameState = GameStates.MainMenu;
                }
                break;
        }
    }

    public void GenerateMap(int sizeX = 20, int sizeY = 20, int taps = 3, int border = 5) {
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

        // Grass
        child = 0;
        for (int x = -border; x < sizeX + border; x++) {
            for (int y = -border; y < sizeY + border; y++) {
                if (x < 0 || x >= sizeX || y < 0 || y >= sizeY) {
                    if (child + 1 > transform.Find("Grass").childCount)
                        Instantiate(prefabGrass, transform.Find("Grass"));
                    transform.Find("Grass").GetChild(child).position = new Vector3(x, 0, y);
                    transform.Find("Grass").GetChild(child).gameObject.SetActive(true);
                    child++;
                }
            }
        }
        while (child + 1 < transform.Find("Grass").childCount) {
            transform.Find("Grass").GetChild(child).gameObject.SetActive(false);
            child++;
        }

        // Trees
        child = 0;
        for (int i = 0; i < Random.Range(5, 8); i++) {
            if (child + 1 > transform.Find("Trees").childCount)
                Instantiate(prefabTree, transform.Find("Trees"));
            transform.Find("Trees").GetChild(child).position = new Vector3(Random.Range(2 - border, sizeX + border - 2), 4.5f, Random.Range(-2, -border));
            transform.Find("Trees").GetChild(child).rotation = Quaternion.Euler(0, 180, 0);
            transform.Find("Trees").GetChild(child).gameObject.SetActive(true);
            child++;
        }

        for (int i = 0; i < Random.Range(5, 8); i++) {
            if (child + 1 > transform.Find("Trees").childCount)
                Instantiate(prefabTree, transform.Find("Trees"));
            transform.Find("Trees").GetChild(child).position = new Vector3(Random.Range(2 - border, sizeX + border - 2), 4.5f, Random.Range(sizeX, sizeX + border));
            transform.Find("Trees").GetChild(child).rotation = Quaternion.Euler(0, 0, 0);
            transform.Find("Trees").GetChild(child).gameObject.SetActive(true);
            child++;
        }

        for (int i = 0; i < Random.Range(5, 8); i++) {
            if (child + 1 > transform.Find("Trees").childCount)
                Instantiate(prefabTree, transform.Find("Trees"));
            transform.Find("Trees").GetChild(child).position = new Vector3(Random.Range(-border, -1), 4.5f, Random.Range(-border, sizeY + border));
            transform.Find("Trees").GetChild(child).rotation = Quaternion.Euler(0, 270, 0);
            transform.Find("Trees").GetChild(child).gameObject.SetActive(true);
            child++;
        }

        for (int i = 0; i < Random.Range(5, 8); i++) {
            if (child + 1 > transform.Find("Trees").childCount)
                Instantiate(prefabTree, transform.Find("Trees"));
            transform.Find("Trees").GetChild(child).position = new Vector3(Random.Range(sizeX, sizeX + border), 4.5f, Random.Range(-border + 2, sizeY + border - 2));
            transform.Find("Trees").GetChild(child).rotation = Quaternion.Euler(0, 90, 0);
            transform.Find("Trees").GetChild(child).gameObject.SetActive(true);
            child++;
        }

        while (child + 1 < transform.Find("Trees").childCount) {
            transform.Find("Trees").GetChild(child).gameObject.SetActive(false);
            child++;
        }
    }
}

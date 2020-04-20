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
    public GameStates gameState;
    public new Camera camera;
    public int highScore;

    // Private variables
    private float seasonProgress;
    [SerializeField]
    private float seasonLength;
    private Vector3 cameraPosition;
    private Quaternion cameraRotation;
    private float cameraElapsed;
    [SerializeField]
    private ParticleSystem rain;
    private float rainTimerWet;
    [SerializeField]
    private AudioSource audioRain;

    private Transform folderTiles;
    private Transform folderFences;
    private Transform folderWorkers;
    private Transform folderPlants;
    private Transform folderTaps;
    private Transform folderGrass;
    private Transform folderTrees;

    // Start is called before the first frame update
    void Start() {
        main = this;

        Object[] objs = Resources.LoadAll("Plants", typeof(PlantData));
        Plant.plantDataAll = new PlantData[objs.Length];
        for (int i = 0; i < objs.Length; i++)
            Plant.plantDataAll[i] = (PlantData)objs[i];

        folderTiles = transform.Find("Tiles");
        folderFences = transform.Find("Fences");
        folderWorkers = transform.Find("Workers");
        folderPlants = transform.Find("Plants");
        folderTaps = transform.Find("Taps");
        folderGrass = transform.Find("Grass");
        folderTrees = transform.Find("Trees");

        camera.gameObject.SetActive(true);
        cameraPosition = camera.transform.position;
        cameraRotation = camera.transform.rotation;

        gameState = GameStates.MainMenu;

        GenerateMap();

        // Spawn workers
        for (int i = 0; i < 3; i++)
            Instantiate(prefabWorker, new Vector3(Random.Range(0, Tile.tiles.GetLength(0) - 1.0f), 0, Random.Range(0, Tile.tiles.GetLength(1) - 1.0f)), Quaternion.identity, transform.Find("Workers")).GetComponent<Worker>().Setup();
    }

    // Update is called once per frame
    void Update() {
        // Cursor
        if (gameState == GameStates.Gameplay || gameState == GameStates.Intro || gameState == GameStates.Outro)
            Cursor.lockState = CursorLockMode.Locked;
        else
            Cursor.lockState = CursorLockMode.None;

        // Core stuff
        switch (gameState) {
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
                cameraElapsed += Time.deltaTime;
                camera.transform.position = Vector3.Slerp(cameraPosition, Player.main.transform.GetChild(0).position, cameraElapsed/7.0f);
                camera.transform.rotation = Quaternion.Slerp(cameraRotation, Player.main.transform.GetChild(0).rotation, cameraElapsed / 7.0f);

                if (camera.transform.position == Player.main.transform.GetChild(0).position && camera.transform.rotation == Player.main.transform.GetChild(0).rotation) {
                    // Setup game
                    season = Seasons.Spring;
                    year = 1;
                    seasonProgress = 0.0f;
                    Plant.deaths = 0;
                    cameraElapsed = 0.0f;
                    // Setup workers
                    for (int i = 1; i < folderWorkers.childCount; i++)
                        folderWorkers.GetChild(i).gameObject.SetActive(false);
                    folderWorkers.GetChild(0).GetComponent<Worker>().Setup();
                    // Reset map
                    foreach (Transform plant in folderPlants)
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
                    // End game
                    if (Plant.deaths >=  10) {
                        camera.gameObject.SetActive(true);
                        gameState = GameStates.Outro;
                        highScore = Mathf.Max(highScore, year - 1);
                    }

                    // Progress time
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
                                foreach (Transform worker in folderWorkers) {
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
                        seasonProgress -= seasonLength;
                    }

                    // Rain
                    if (season == Seasons.Autumn && !rain.emission.enabled) {
                        var emission = rain.emission;
                        emission.enabled = true;
                        audioRain.Play();
                    }
                    else if (season != Seasons.Autumn && rain.emission.enabled) {
                        var emission = rain.emission;
                        emission.enabled = false;
                        audioRain.Stop();
                    }
                    if (rain.emission.enabled) {
                        rainTimerWet -= Time.deltaTime;
                        if (rainTimerWet <= 0) {
                            rainTimerWet = Random.Range(0.2f, 0.4f);
                            Tile.tiles[Random.Range(0, Tile.tiles.GetLength(0)), Random.Range(0, Tile.tiles.GetLength(1))].Water();
                        }
                    }
                }
                break;

            case GameStates.Paused:
                if (Input.GetKeyDown("escape"))
                    gameState = GameStates.Gameplay;
                break;

            case GameStates.Outro:
                // Move camera
                cameraElapsed += Time.deltaTime;
                camera.transform.position = Vector3.Slerp(Player.main.transform.GetChild(0).position, cameraPosition, cameraElapsed / 7.0f);
                camera.transform.rotation = Quaternion.Slerp(Player.main.transform.GetChild(0).rotation, cameraRotation, cameraElapsed / 7.0f);

                if (cameraElapsed > 10.0f) {
                    camera.transform.position = cameraPosition;
                    camera.transform.rotation = cameraRotation;
                    Debug.LogWarning("Camera got stuck");
                }

                if (camera.transform.position == cameraPosition && camera.transform.rotation == cameraRotation) {
                    cameraElapsed = 0.0f;
                    gameState = GameStates.MainMenu;
                }
                break;
        }
    }

    public void GenerateMap(int sizeX = 20, int sizeY = 20, int taps = 3, int border = 10) {
        // Tiles
        int child = 0;
        Tile.tiles = new Tile[sizeX, sizeY];
        for (int x = 0; x < sizeX; x++) {
            for (int y = 0; y < sizeY; y++) {
                if (child + 1 > folderTiles.childCount)
                    Instantiate(prefabTile, folderTiles);
                folderTiles.GetChild(child).position = new Vector3(x, 0, y);
                folderTiles.GetChild(child).gameObject.SetActive(true);
                Tile.tiles[x, y] = folderTiles.GetChild(child).GetComponent<Tile>();
                child++;
            }
        }
        while (child + 1 < folderTiles.childCount) {
            folderTiles.GetChild(child).gameObject.SetActive(false);
            child++;
        }

        // Fences
        child = 0;
        for (int x = 0; x < sizeX; x++) {
            if (child + 1 > folderFences.childCount)
                Instantiate(prefabFences, folderFences);
            folderFences.GetChild(child).position = new Vector3(x, 0, -1);
            folderFences.GetChild(child).rotation = Quaternion.identity;
            folderFences.GetChild(child).gameObject.SetActive(true);
            child++;

            if (child + 1 > folderFences.childCount)
                Instantiate(prefabFences, folderFences);
            folderFences.GetChild(child).position = new Vector3(x, 0, sizeY);
            folderFences.GetChild(child).rotation = Quaternion.Euler(0, 180, 0);
            folderFences.GetChild(child).gameObject.SetActive(true);
            child++;
        }
        for (int y = 0; y < sizeY; y++) {
            if (child + 1 > folderFences.childCount)
                Instantiate(prefabFences, folderFences);
            folderFences.GetChild(child).position = new Vector3(-1, 0, y);
            folderFences.GetChild(child).rotation = Quaternion.Euler(0, 90, 0);
            folderFences.GetChild(child).gameObject.SetActive(true);
            child++;

            if (child + 1 > folderFences.childCount)
                Instantiate(prefabFences, folderFences);
            folderFences.GetChild(child).position = new Vector3(sizeX, 0, y);
            folderFences.GetChild(child).rotation = Quaternion.Euler(0, 270, 0);
            folderFences.GetChild(child).gameObject.SetActive(true);
            child++;
        }
        while (child + 1 < folderFences.childCount) {
            folderFences.GetChild(child).gameObject.SetActive(false);
            child++;
        }

        // Taps
        child = 0;
        int tapsPlaced = 0;
        Tap.taps = new Tap[taps];
        while (tapsPlaced < taps) {
            Tile tile = Tile.tiles[Random.Range(0, Tile.tiles.GetLength(0)), Random.Range(0, Tile.tiles.GetLength(1))];
            if (!tile.tap) {
                tile.tap = true;
                if (child + 1 > folderTaps.childCount)
                    Instantiate(prefabTap, folderTaps);
                folderTaps.GetChild(child).position = tile.transform.position;
                Tap.taps[child] = folderTaps.GetChild(child).GetComponent<Tap>();
                tapsPlaced++;
                child++;
            }
        }
        while (child + 1 < folderTaps.childCount) {
            folderTaps.GetChild(child).gameObject.SetActive(false);
            child++;
        }

        // Grass
        child = 0;
        for (int x = -border; x < sizeX + border; x++) {
            for (int y = -border; y < sizeY + border; y++) {
                if (x < 0 || x >= sizeX || y < 0 || y >= sizeY) {
                    if (child + 1 > folderGrass.childCount)
                        Instantiate(prefabGrass, folderGrass);
                    folderGrass.GetChild(child).position = new Vector3(x, 0, y);
                    folderGrass.GetChild(child).gameObject.SetActive(true);
                    child++;
                }
            }
        }
        while (child + 1 < folderGrass.childCount) {
            folderGrass.GetChild(child).gameObject.SetActive(false);
            child++;
        }

        // Trees
        child = 0;

        for (int i = 0; i < 100; i++) {
            Vector3 positon = new Vector3(Random.Range(1 - border, sizeX + border - 1), 4.5f, Random.Range(1 - border, sizeY + border - 1));
            if (!Physics.CheckSphere(positon, 0.6f) && !Physics.CheckSphere(new Vector3(positon.x, 0, positon.z), 0.6f)) {
                if (child + 1 > folderTrees.childCount)
                    Instantiate(prefabTree, folderTrees);
                folderTrees.GetChild(child).position = positon;
                folderTrees.GetChild(child).rotation = Quaternion.Euler(0, 180, 0);
                folderTrees.GetChild(child).gameObject.SetActive(true);
                child++;
            }
        }

        /*

        for (int y = -4; y >= -border + 1; y -= 2) {
            int x = 2 + Random.Range(0, 6);
            while (x < sizeX - 3) {
                if (child + 1 > folderTrees.childCount)
                    Instantiate(prefabTree, folderTrees);
                folderTrees.GetChild(child).position = new Vector3(x, 4.5f, y);
                folderTrees.GetChild(child).rotation = Quaternion.Euler(0, 180, 0);
                folderTrees.GetChild(child).gameObject.SetActive(true);
                child++;
                x += Random.Range(7, 11);
            }
        }

        for (int y = sizeY + 2; y < sizeY + border - 1; y += 2) {
            int x = 2 + Random.Range(0, 6);
            while (x < sizeX - 3) {
                if (child + 1 > folderTrees.childCount)
                    Instantiate(prefabTree, folderTrees);
                folderTrees.GetChild(child).position = new Vector3(x, 4.5f, y);
                folderTrees.GetChild(child).rotation = Quaternion.Euler(0, 0, 0);
                folderTrees.GetChild(child).gameObject.SetActive(true);
                child++;
                x += Random.Range(7, 11);
            }
        }

        for (int x = -3; x >= -border + 1; x -= 2) {
            int y = 2 + Random.Range(0, 6);
            while (y < sizeY - 3) {
                if (child + 1 > folderTrees.childCount)
                    Instantiate(prefabTree, folderTrees);
                folderTrees.GetChild(child).position = new Vector3(x, 4.5f, y);
                folderTrees.GetChild(child).rotation = Quaternion.Euler(0, 270, 0);
                folderTrees.GetChild(child).gameObject.SetActive(true);
                child++;
                y += Random.Range(7, 11);
            }
        }

        for (int x = sizeX + 2; x <= sizeX + border; x += 2) {
            int y = 2 + Random.Range(0, 6);
            while (y < sizeY - 3) {
                if (child + 1 > folderTrees.childCount)
                    Instantiate(prefabTree, folderTrees);
                folderTrees.GetChild(child).position = new Vector3(x, 4.5f, y);
                folderTrees.GetChild(child).rotation = Quaternion.Euler(0, 90, 0);
                folderTrees.GetChild(child).gameObject.SetActive(true);
                child++;
                y += Random.Range(7, 11);
            }
        }
        */

        while (child + 1 < folderTrees.childCount) {
            folderTrees.GetChild(child).gameObject.SetActive(false);
            child++;
        }
    }
}

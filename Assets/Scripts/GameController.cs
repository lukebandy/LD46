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
    private GameObject prefabPlant;
    [SerializeField]
    private PlantData dataPlantA;

    // Public variables
    public Seasons season;
    public int year;
    private float seasonProgress;

    // Private variables

    // Start is called before the first frame update
    void Start() {
        main = this;
        GenerateMap();
        Instantiate(prefabPlant, transform.Find("Plants")).GetComponent<Plant>().Setup(15, 13, dataPlantA);

        season = Seasons.Spring;
        year = 1;
        seasonProgress = 0.0f;
    }

    // Update is called once per frame
    void Update() {
        seasonProgress += Time.deltaTime;
        if (seasonProgress >= 20.0f) {
            switch(season) {
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
        }
    }

    public void GenerateMap(int sizeX = 20, int sizeY = 20, int hoses = 3) {
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
    }
}

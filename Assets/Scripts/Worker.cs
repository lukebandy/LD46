using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worker : MonoBehaviour {

    // Private variables
    [SerializeField]
    private GameObject prefabPlant;
    private float timeIdleProgress;
    private float timePlantProgress;

    private enum Action { Idle, Walking, Planting }
    private Action action;
    private Tile target;

    // Start is called before the first frame update
    void Start() {
        action = Action.Idle;
    }

    // Update is called once per frame
    void Update() {
        // Do stuff
        switch (action) {
            case Action.Idle:
                timeIdleProgress += Time.deltaTime;
                if (timeIdleProgress <= 10.0f) {
                    if (target == null || transform.position == target.transform.position)
                        target = Tile.tiles[Random.Range(0, Tile.tiles.GetLength(0)), Random.Range(0, Tile.tiles.GetLength(1))];
                    transform.position = Vector3.MoveTowards(transform.position, target.transform.position, Time.deltaTime);
                }
                else {
                    action = Action.Walking;
                    target = null;
                    while (target == null) {
                        Tile tile = Tile.tiles[Random.Range(0, Tile.tiles.GetLength(0)), Random.Range(0, Tile.tiles.GetLength(1))];
                        if (tile.plant == null && !tile.plantInProgress && !tile.tap)
                            target = tile;
                    }
                    target.plantInProgress = true;
                }
                break;

            case Action.Walking:
                transform.position = Vector3.MoveTowards(transform.position, target.transform.position, Time.deltaTime);
                if (transform.position == target.transform.position) {
                    action = Action.Planting;
                    timePlantProgress = 0.0f;
                }
                break;

            case Action.Planting:
                timePlantProgress += Time.deltaTime;
                if (timePlantProgress >= 10.0f) {
                    // Get plants suitable for this season
                    List<PlantData> plants = new List<PlantData>();
                    foreach (PlantData plant in Plant.plantDataAll) {
                        if (plant.season == GameController.main.season)
                            plants.Add(plant);
                    }

                    if (plants.Count > 0) {
                        // Plant
                        bool planted = false;
                        foreach (Transform plant in GameController.main.transform.Find("Plants")) {
                            if (!plant.gameObject.activeSelf) {
                                plant.gameObject.SetActive(true);
                                plant.GetComponent<Plant>().Setup(target, plants[Random.Range(0, plants.Count)]);
                                planted = true;
                                break;
                            }
                        }
                        if (!planted)
                            Instantiate(prefabPlant, GameController.main.transform.Find("Plants")).GetComponent<Plant>().Setup(target, plants[Random.Range(0, plants.Count)]);
                    }
                    else
                        Debug.LogWarning("No plants found for current season");

                    // Reset action state
                    target.plantInProgress = false;
                    action = Action.Idle;
                    timeIdleProgress = 0.0f;
                }
                break;
        }

        // Look towards player
        transform.GetChild(0).LookAt(Player.main.transform.position);
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y + 180, 0);

        // Update material to show angle and action
    }
}
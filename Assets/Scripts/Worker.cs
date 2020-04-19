﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worker : MonoBehaviour {

    // Private variables
    [SerializeField]
    private GameObject prefabPlant;
    private float timeIdleRemaning;
    private float timePlantRemaining;
    private enum Action { Idle, Walking, Planting }
    private Action action;
    private Tile target;
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip[] audioDone;
    [SerializeField]
    private AudioClip[] audioAnnoyed;
    private float audioAnnoyedTimout;
    private MeshRenderer meshRenderer;
    [SerializeField]
    private Material materialWalking;
    [SerializeField]
    private Material materialWorking;

    public void Setup() {
        action = Action.Idle;
        timeIdleRemaning = Random.Range(5.0f, 10.0f);
        if (meshRenderer == null)
            meshRenderer = GetComponentInChildren<MeshRenderer>();
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update() {
        if (GameController.main.gameState != GameController.GameStates.Paused) {
            if (audioAnnoyedTimout > 0)
                audioAnnoyedTimout -= Time.deltaTime;

            // Do stuff
            switch (action) {
                case Action.Idle:
                    timeIdleRemaning -= Time.deltaTime;
                    if (timeIdleRemaning <= 0) {
                        if (target == null || transform.position == target.transform.position)
                            target = Tile.tiles[Random.Range(0, Tile.tiles.GetLength(0)), Random.Range(0, Tile.tiles.GetLength(1))];
                        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, Time.deltaTime);
                    }
                    else {
                        action = Action.Walking;
                        target = null;

                        // Does a plant need picking
                        for (int x = 0; x < Tile.tiles.GetLength(0); x++) {
                            for (int y = 0; y < Tile.tiles.GetLength(1); y++) {
                                if (Tile.tiles[x, y].plant != null && Tile.tiles[x, y].plant.Pickable && !Tile.tiles[x, y].plantInProgress) {
                                    target = Tile.tiles[x, y];
                                    break;
                                }
                            }
                        }

                        // Otherwise go and plant one
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
                        timePlantRemaining = Random.Range(3.0f, 5.0f);
                    }
                    break;

                case Action.Planting:
                    timePlantRemaining -= Time.deltaTime;
                    if (timePlantRemaining <= 0) {
                        // Plant a plant
                        if (target.plant == null) {
                            // Get plants suitable for this season
                            List<PlantData> plants = new List<PlantData>();
                            foreach (PlantData plant in Plant.plantDataAll) {
                                //if (plant.season == GameController.main.season)
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
                        }
                        // Pick plant
                        else {
                            if (target.plant.Pickable)
                                GameController.main.farmValue += target.plant.Value;
                            target.plant.gameObject.SetActive(false);
                            target.plant = null;
                        }

                        // Reset action state
                        audioSource.clip = audioDone[Random.Range(0, audioDone.Length)];
                        audioSource.Play();
                        target.plantInProgress = false;
                        action = Action.Idle;
                        timeIdleRemaning = Random.Range(5.0f, 10.0f);
                    }
                    break;
            }
        }

        // Look towards camera
        if (GameController.main.gameState == GameController.GameStates.Gameplay) {
            transform.GetChild(0).LookAt(Player.main.transform.position);
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y + 180, 0);
        }
        else {
            transform.GetChild(0).LookAt(GameController.main.camera.transform.position);
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y + 180, 0);
        }

        // TODO: Update material to show angle and action
        if (action == Action.Planting)
            meshRenderer.material = materialWorking;
        else
            meshRenderer.material = materialWalking;
    }

    public void Annoy() {
        if (audioAnnoyedTimout <= 0) {
            audioAnnoyedTimout = 5.0f;
            audioSource.clip = audioAnnoyed[Random.Range(0, audioAnnoyed.Length)];
            audioSource.Play();
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour {

    // Static variables
    public static int deaths;
    public static PlantData[] plantDataAll;

    // Private variables
    private PlantData plantData;
    private MeshRenderer meshRenderer;
    private Tile tile;
    private float timeGrowProgress;
    private float timeDryProgress;
    private float timeDeadProgress;
    private bool died;

    public void Setup(Tile tile, PlantData plantData) {
        transform.position = tile.transform.position;
        this.plantData = plantData;
        tile.plant = this;
        this.tile = tile;
        timeGrowProgress = 0.0f;
        timeDryProgress = 0.0f;
        timeDeadProgress = 0.0f;
        died = false;

        if (meshRenderer == null)
            meshRenderer = GetComponentInChildren<MeshRenderer>();

        Update();
    }

    // Update is called once per frame
    void Update() {
        // Look towards camera
        if (GameController.main.gameState == GameController.GameStates.Gameplay) {
            transform.GetChild(0).LookAt(Player.main.transform.position);
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y + 180, 0);
        }
        else {
            transform.GetChild(0).LookAt(GameController.main.camera.transform.position);
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y + 180, 0);
        }

        // Gameplay loop
        if (GameController.main.gameState == GameController.GameStates.Gameplay) {
            // Current state
            if (!died) {
                //if (plantData.season == GameController.main.season) {
                if (tile.wet && timeDryProgress < plantData.dryTime)
                    timeDryProgress = 0.0f;
                else
                    timeDryProgress += Time.deltaTime;

                if (timeDryProgress < plantData.dryTime)
                    timeGrowProgress += Time.deltaTime;
                else {
                    if (!died) {
                        deaths++;
                        died = true;
                    }
                }
                //}
                //else {
                //    Debug.Log("Died - season");
                //    deaths++;
                //    died = true;
                //}
            }
            else {
                timeDeadProgress += Time.deltaTime;
                if (timeDeadProgress >= 5.0f) {
                    tile.plant = null;
                    gameObject.SetActive(false);
                }
            }

            // Update material to reflect current state
            if (!died) {
                if (timeGrowProgress < plantData.growTime * 0.5f)
                    meshRenderer.material = plantData.materialSproutAlive;
                else if (timeGrowProgress < plantData.growTime)
                    meshRenderer.material = plantData.materialGrowingAlive;
                else
                    meshRenderer.material = plantData.materialGrownAlive;
            }
            else {
                if (timeGrowProgress < plantData.growTime * 0.5f)
                    meshRenderer.material = plantData.materialSproutDead;
                else if (timeGrowProgress < plantData.growTime)
                    meshRenderer.material = plantData.materialGrowingDead;
                else
                    meshRenderer.material = plantData.materialGrownDead;
            }
        }
    }

    public bool Pickable {
        get {
            return (!died) && timeGrowProgress >= plantData.growTime;
        }
    }

    public int Value {
        get {
            return plantData.value;
        }
    }
}

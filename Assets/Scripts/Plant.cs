using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour {

    // Static variables
    public static int deaths;

    // Private variables
    private PlantData plantData;
    private MeshRenderer meshRenderer;

    // Start is called before the first frame update
    void Start() {
        meshRenderer = GetComponentInChildren<MeshRenderer>();
    }

    public void Setup(int x, int y, PlantData plantData) {
        transform.position = new Vector3(x, 0, y);
        this.plantData = plantData;
        Tile.tiles[x, y].plant = this;
    }

    // Update is called once per frame
    void Update() {
        // Look towards player
        transform.GetChild(0).LookAt(Player.main.transform.position);
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y + 180, 0);

        // Update material to reflect current state
    }
}

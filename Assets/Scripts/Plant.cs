using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour {

    // Static variables
    public static int deaths;

    // Private variables
    PlantData plantData;

    // Start is called before the first frame update
    void Start() {
        
    }

    public void Setup(int x, int y, PlantData plantData) {
        transform.position = new Vector3(x, 0, y);
        this.plantData = plantData;
    }

    // Update is called once per frame
    void Update() {
        // Look towards player
        transform.GetChild(0).LookAt(Player.main.transform.position);
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y + 180, 0);
    }
}

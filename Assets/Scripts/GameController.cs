using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    // Static variables
    public static GameController main;

    // Prefabs
    [SerializeField]
    private GameObject prefabTile;
    [SerializeField]
    private GameObject prefabFences;

    // Public variables

    // Private variables

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void GenerateMap(int sizeX = 20, int sizeY = 20, int hoses = 3) {
        // Tiles
        int child = 0;
        for (int x = 0; x < sizeX; x++) {
            for (int y = 0; y < sizeY; y++) {
                if (child + 1 > transform.Find("Tiles").childCount)
                    Instantiate(prefabTile, transform.Find("Tiles"));
                transform.Find("Tiles").GetChild(child).position = new Vector3(x, 0, y);
                transform.Find("Tiles").GetChild(child).gameObject.SetActive(true);
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
                Instantiate(prefabTile, transform.Find("Fences"));
            transform.Find("Fences").GetChild(child).position = new Vector3(x, 0, 0);
            transform.Find("Fences").GetChild(child).rotation = Quaternion.identity;
            transform.Find("Fences").GetChild(child).gameObject.SetActive(true);
            child++;

            if (child + 1 > transform.Find("Fences").childCount)
                Instantiate(prefabTile, transform.Find("Fences"));
            transform.Find("Fences").GetChild(child).position = new Vector3(x, 0, sizeY - 1);
            transform.Find("Fences").GetChild(child).rotation = Quaternion.identity;
            transform.Find("Fences").GetChild(child).gameObject.SetActive(true);
            child++;
        }
        for (int y = 0; y < sizeY; y++) {
            
        }
        while (child + 1 < transform.Find("Fences").childCount) {
            transform.Find("Fences").GetChild(child).gameObject.SetActive(false);
            child++;
        }
    }
}

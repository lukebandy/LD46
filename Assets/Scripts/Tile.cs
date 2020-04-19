using UnityEngine;

public class Tile : MonoBehaviour {

    // Static variables
    public static Tile[,] tiles;

    // Public variables
    public Plant plant;
    public bool wet;
    public bool plantInProgress;
    public bool tap;

    // Private variables
    private float wetTimer;
    private MeshRenderer meshRenderer;
    [SerializeField]
    private Material materialDry;
    [SerializeField]
    private Material materialWet;

    // Start is called before the first frame update
    void Start() {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update() {
        if (GameController.main.gameState != GameController.GameStates.Paused) {
            if (wetTimer > 0)
                wetTimer -= Time.deltaTime;
            if (wetTimer <= 0)
                wet = false;
        }

        if (wet)
            meshRenderer.material = materialWet;
        else
            meshRenderer.material = materialDry;
    }

    public void Water() {
        wet = true;
        wetTimer = 13 + Random.Range(0.0f, 5.0f);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

    public bool wet;

    [SerializeField]
    private Material materialDry;
    [SerializeField]
    private Material materialWet;

    private MeshRenderer meshRenderer;

    public float wetTimer;

    // Start is called before the first frame update
    void Start() {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update() {
        if (wetTimer > 0)
            wetTimer -= Time.deltaTime;
        if (wetTimer <= 0)
            wet = false;

        if (wet)
            meshRenderer.material = materialWet;
        else
            meshRenderer.material = materialDry;
    }

    public void Water() {
        wet = true;
        wetTimer = 10 + Random.Range(0.0f, 5.0f);
    }
}

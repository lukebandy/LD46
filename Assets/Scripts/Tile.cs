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

    // Start is called before the first frame update
    void Start() {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update() {
        if (wet)
            meshRenderer.material = materialWet;
        else
            meshRenderer.material = materialDry;
    }
}

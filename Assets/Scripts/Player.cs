using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    // Static variables
    public static Player main;

    // Private variables
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float lookSensitivity;
    [SerializeField]
    private float lookSmoothing;
    private Vector2 lookIncrement;
    private Vector2 lookSmooth;
    [SerializeField]
    private float hoseDistance;
    private new ParticleSystem particleSystem;
    [SerializeField]
    private AudioSource audioWater;
    [SerializeField]
    private AudioSource audioEmpty;
    [SerializeField]
    private AudioSource audioWalking;
    [SerializeField]
    private Rigidbody rb;

    // Public variables
    [HideInInspector]
    public float hoseRemaining;
    public float hoseCapacity;

    // Start is called before the first frame update
    void Start() {
        main = this;
        particleSystem = GetComponentInChildren<ParticleSystem>();
        rb = GetComponent<Rigidbody>();
        Setup();
    }

    public void Setup() {
        transform.position = new Vector3((Tile.tiles.GetLength(0) - 1) * 0.5f, 1.5f, (Tile.tiles.GetLength(1) - 1) * 0.5f);
        transform.rotation = Quaternion.Euler(0, 180, 0);
        lookIncrement.x = 180;
        hoseRemaining = hoseCapacity;
    }

    // Update is called once per frame
    void Update() {
        if (GameController.main.gameState == GameController.GameStates.Gameplay) {
            // Looking
            Vector2 lookInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
            lookInput = Vector2.Scale(lookInput, new Vector2(lookSensitivity * lookSmoothing, lookSensitivity * lookSmoothing));
            lookSmooth.x = Mathf.Lerp(lookSmooth.x, lookInput.x, 1f / lookSmoothing);
            lookSmooth.y = Mathf.Lerp(lookSmooth.y, lookInput.y, 1f / lookSmoothing);
            lookIncrement += lookSmooth;
            lookIncrement.y = Mathf.Clamp(lookIncrement.y, -80.0f, 80.0f);
            transform.GetChild(0).localRotation = Quaternion.AngleAxis(-lookIncrement.y, Vector3.right);

            // Hose
            if (Input.GetMouseButton(0) && hoseRemaining > 0) {
                var emission = particleSystem.emission;
                emission.enabled = true;
                // Reduce remaining water
                hoseRemaining -= Time.deltaTime;
                if (hoseRemaining <= 0)
                    audioEmpty.Play();
                // Wake tiles wet
                if (Physics.Raycast(transform.GetChild(0).position, transform.GetChild(0).forward, out RaycastHit hitTile, hoseDistance, 1 << 8)) {
                    if (hitTile.transform.CompareTag("Tile")) {
                        hitTile.transform.GetComponent<Tile>().Water();
                    }
                }
                // Annoy workers
                if (Physics.Raycast(transform.GetChild(0).position, transform.GetChild(0).forward, out RaycastHit hitWorker, hoseDistance, 1 << 10)) {
                    hitWorker.transform.GetComponent<Worker>().Annoy();
                }
            }
            else {
                var emission = particleSystem.emission;
                emission.enabled = false;
            }

            if (Input.GetMouseButtonDown(0) && hoseRemaining > 0)
                audioWater.Play();
            else if (Input.GetMouseButtonUp(0) || hoseRemaining <= 0)
                audioWater.Stop();

            // Water topup
            foreach (Tap tap in FindObjectsOfType<Tap>()) {
                var emission = tap.particleSystem.emission;
                emission.enabled = false;
                tap.inuse = false;
            }
            if (Physics.Raycast(transform.GetChild(0).position, transform.GetChild(0).forward, out RaycastHit hitTap, 4.0f)) {
                if (hitTap.transform.CompareTag("Tap")) {
                    hoseRemaining = Mathf.Clamp(hoseRemaining + (Time.deltaTime * 5.0f), 0, hoseCapacity);
                    var emission = hitTap.transform.GetComponent<Tap>().particleSystem.emission;
                    emission.enabled = true;
                    hitTap.transform.GetComponent<Tap>().inuse = true;
                }
            }
        }
        else {
            var emission = particleSystem.emission;
            emission.enabled = false;

            foreach (Tap tap in FindObjectsOfType<Tap>()) {
                var emissionTap = tap.particleSystem.emission;
                emissionTap.enabled = false;
            }
        }
    }

    private void FixedUpdate() {
        if (GameController.main.gameState == GameController.GameStates.Gameplay) {
            rb.isKinematic = true;
            rb.rotation = Quaternion.AngleAxis(lookIncrement.x, transform.up);
            float translation = Input.GetAxis("Vertical") * moveSpeed * Time.fixedDeltaTime;
            float straffe = Input.GetAxis("Horizontal") * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(transform.position + (translation * transform.forward) + (straffe * transform.right));
        }
        else
            rb.isKinematic = false;
    }
}

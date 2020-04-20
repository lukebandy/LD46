using UnityEngine;

public class Player : MonoBehaviour {

    // Static variables
    public static Player main;

    // Private variables
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float lookSensitivity;
    private Vector2 lookIncrement;
    [SerializeField]
    private float hoseDistance;
    private new ParticleSystem particleSystem;
    [SerializeField]
    private AudioSource audioWater;
    private float audioWaterTimer;
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
            // Vertical looking
            Vector3 rotation = transform.GetChild(0).localRotation.eulerAngles;
            rotation -= Vector3.right * Input.GetAxis("Mouse Y") * lookSensitivity * Time.deltaTime;
            while (rotation.x >= 180.0f)
                rotation.x -= 360.0f;
            while (rotation.x <= -180.0f)
                rotation.x += 360.0f;
            rotation.x = Mathf.Clamp(rotation.x, -75.0f, 75.0f);
            transform.GetChild(0).localRotation = Quaternion.Euler(rotation);

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

            // Hose audio

            if (Input.GetMouseButtonUp(0))
                audioWaterTimer = 0.2f;
            else if (audioWaterTimer > 0)
                audioWaterTimer -= Time.deltaTime;

            if (Input.GetMouseButton(0) && hoseRemaining > 0 && !audioWater.isPlaying) {
                audioWater.Play();
            }
            else if (((!Input.GetMouseButton(0) && audioWaterTimer <= 0) || hoseRemaining <= 0) && audioWater.isPlaying) {
                audioWater.Stop();
            }

            // Water topup
            foreach (Tap tap in Tap.taps) {
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

            foreach (Tap tap in Tap.taps) {
                var emissionTap = tap.particleSystem.emission;
                emissionTap.enabled = false;
                tap.inuse = false;
            }

            audioWater.Stop();
        }

        if ((Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) && GameController.main.gameState == GameController.GameStates.Gameplay) {
            if (!audioWalking.isPlaying)
                audioWalking.Play();
        }
        else{
            audioWalking.Stop();
        }
    }

    private void FixedUpdate() {
        if (GameController.main.gameState == GameController.GameStates.Gameplay) {
            rb.isKinematic = false;
            // Horizontal looking
            float angle = Input.GetAxis("Mouse X") * Time.deltaTime * lookSensitivity;
            rb.MoveRotation(Quaternion.Euler(rb.rotation.eulerAngles + (transform.up * angle)));
            // Movement
            float translation = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
            float straffe = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
            rb.MovePosition(transform.position + (translation * transform.forward) + (straffe * transform.right));
        }
        else
            rb.isKinematic = true;
    }
}

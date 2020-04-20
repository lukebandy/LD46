using UnityEngine;

public class Tap : MonoBehaviour {

    public static Tap[] taps;

    public new ParticleSystem particleSystem;
    [SerializeField]
    private AudioSource audioTap;
    [SerializeField]
    private AudioSource audioWater;
    [HideInInspector]
    public bool inuse;
    private bool inusePrevious;

    // Start is called before the first frame update
    void Start() {
        particleSystem = GetComponentInChildren<ParticleSystem>();
        inuse = false;
        inusePrevious = false;
    }

    // Update is called once per frame
    void Update() {
        // Look towards player
        if (GameController.main.gameState == GameController.GameStates.Gameplay || GameController.main.gameState == GameController.GameStates.Paused) {
            transform.GetChild(0).LookAt(Player.main.transform.position);
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y + 180, 0);
        }
        else {
            transform.GetChild(0).LookAt(GameController.main.camera.transform.position);
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y + 180, 0);
        }

        // On noise
        if (inuse != inusePrevious && inuse) {
            audioTap.Play();
            audioWater.Play();
        }
        if (inuse != inusePrevious && !inuse) {
            audioWater.Stop();
        }
        inusePrevious = inuse;
    }
}

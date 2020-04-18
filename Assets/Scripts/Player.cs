// Looking and movement adapted from https://github.com/jiankaiwang/FirstPersonController

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    // Static variables
    public static Player main;

    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float lookSensitivity;
    [SerializeField]
    private float lookSmoothing;
    private Vector2 lookIncrement;
    private Vector2 lookSmooth;

    public float hoseRemaining;

    // Start is called before the first frame update
    void Start() {
        main = this;
        hoseRemaining = 30.0f;

        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update() {
        // 
        if (Input.GetKeyDown("escape")) {
            if (Cursor.lockState == CursorLockMode.Locked)
                Cursor.lockState = CursorLockMode.None;
            else
                Cursor.lockState = CursorLockMode.Locked;
        }

        if (Cursor.lockState == CursorLockMode.Locked) {
            // Looking
            Vector2 lookInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
            lookInput = Vector2.Scale(lookInput, new Vector2(lookSensitivity * lookSmoothing, lookSensitivity * lookSmoothing));
            lookSmooth.x = Mathf.Lerp(lookSmooth.x, lookInput.x, 1f / lookSmoothing);
            lookSmooth.y = Mathf.Lerp(lookSmooth.y, lookInput.y, 1f / lookSmoothing);
            lookIncrement += lookSmooth;
            transform.GetChild(0).localRotation = Quaternion.AngleAxis(-lookIncrement.y, Vector3.right);
            transform.localRotation = Quaternion.AngleAxis(lookIncrement.x, transform.up);

            // Movement
            float translation = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
            float straffe = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
            transform.Translate(straffe, 0, translation);

            // Hose
            if (Input.GetMouseButton(0)) {
                if (hoseRemaining > 0) {
                    // Reduce remaining water
                    hoseRemaining -= Time.deltaTime;
                    // Wake tiles wet

                    RaycastHit hit;
                    if (Physics.Raycast(transform.GetChild(0).position, transform.GetChild(0).forward, out hit, 500, 1 << 8)) {
                        if (hit.transform.CompareTag("Tile")) {
                            hit.transform.GetComponent<Tile>().Water();
                        }
                    }
                }
            }
        }
    }
}

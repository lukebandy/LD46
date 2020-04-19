﻿// Looking and movement adapted from https://github.com/jiankaiwang/FirstPersonController

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
    private AudioSource audioWalking;

    // Public variables
    [HideInInspector]
    public float hoseRemaining;
    public float hoseCapacity;

    // Start is called before the first frame update
    void Start() {
        main = this;
        particleSystem = GetComponentInChildren<ParticleSystem>();
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
            lookIncrement.y = Mathf.Clamp(lookIncrement.y, -80.0f, 85.0f);
            transform.GetChild(0).localRotation = Quaternion.AngleAxis(-lookIncrement.y, Vector3.right);
            transform.localRotation = Quaternion.AngleAxis(lookIncrement.x, transform.up);

            // Movement
            float translation = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
            float straffe = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
            transform.Translate(straffe, 0, translation);

            // Hose
            if (Input.GetMouseButton(0) && hoseRemaining > 0) {
                var emission = particleSystem.emission;
                emission.enabled = true;
                // Reduce remaining water
                hoseRemaining -= Time.deltaTime;
                // Wake tiles wet
                if (Physics.Raycast(transform.GetChild(0).position, transform.GetChild(0).forward, out RaycastHit hitTile, hoseDistance, 1 << 8)) {
                    if (hitTile.transform.CompareTag("Tile")) {
                        hitTile.transform.GetComponent<Tile>().Water();
                    }
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
}

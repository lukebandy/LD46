using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour{
    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        // Look towards camera
        if (GameController.main.gameState == GameController.GameStates.Gameplay) {
            transform.LookAt(Player.main.transform.position);
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y + 180, 0);
        }
        else {
            transform.LookAt(GameController.main.camera.transform.position);
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y + 180, 0);
        }
    }
}

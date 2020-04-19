using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameController))]
public class GameControllerEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        GameController gameController = (GameController)target;

        if (GUILayout.Button("Generate Map"))
            gameController.GenerateMap();
    }
}

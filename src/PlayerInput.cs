using UnityEngine;

public class PlayerInput : MonoBehaviour {

    public AISpawner AISpawner;

    private void Update() {
        if (Input.GetKey(KeyCode.E)) {
            AISpawner.Spawn();
        }
        else if (Input.GetKey(KeyCode.Q)) {
            AISpawner.Remove();
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
        #if UNITY_STANDALONE
            Application.Quit();
        #endif

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
        }
    }
}

using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class AISpawner : MonoBehaviour {

    public GameObject AIPrefab;
    public int NumSpawn;
    public float SpawnRadius;
    public TextMeshProUGUI TextBox;

    private List<GameObject> spawned;
    private GameObject go;

    public void Spawn() {
        if (spawned.Count < 200) {
            go = Instantiate(AIPrefab, (Vector2)transform.position + Random.insideUnitCircle * SpawnRadius, Quaternion.identity, transform);
            spawned.Add(go);
            TextBox.text = (spawned.Count).ToString();
        }
    }

    public void Remove() {
        if (spawned.Count > 0) {
            Destroy(spawned[0]);
            spawned.RemoveAt(0);
            TextBox.text = (spawned.Count).ToString();
        }
    }

    private void Awake() {
        spawned = new List<GameObject>(NumSpawn);
        for (int i = 0; i < NumSpawn; i++) {
            go = Instantiate(AIPrefab, (Vector2)transform.position + Random.insideUnitCircle * SpawnRadius, Quaternion.identity, transform);
            spawned.Add(go);
        }
        TextBox.text = (spawned.Count).ToString();
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, SpawnRadius);
    }
}

using UnityEngine;

[CreateAssetMenu()]
public class BoidConfig : ScriptableObject {

    [Header("General")]
    public float MoveSpeed;
    public float TurnTime;

    [Header("Neighbors")]
    public LayerMask CheckLayer;
    public float CheckRadius;

    [Header("Keep Distance")]
    public float KeepDistance;
    public float Threshold;

    [Header("Weights")]
    public float PathWeight;
    public float CohesionWeight;
    public float AlignmentWeight;
    public float SeparationWeight;
}

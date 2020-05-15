using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Boid : UnityFSM {

    public BoidConfig BoidConfig;

    // STATES
    public BoidHaltState BoidHalt { get; protected set; }
    public BoidSeekState BoidSeek { get; protected set; }

    // these fields should not be visible in the inspector
    public GameObject MyTarget { get; private set; }
    public Vector2 DirToTarget { get; protected set; }
    public Transform BoidTransform { get; private set; }
    public Rigidbody2D BoidRB { get; private set; }
    public Collider2D BoidCollider { get; private set; }

    // smooth rotate
    public float ElapsedTime { get; private set; }
    private Quaternion startRot;
    private Quaternion endRot;
    public void UpdateRotateVars(Quaternion startRot, Quaternion endRot) {
        ElapsedTime = 0f;
        this.startRot = startRot;
        this.endRot = endRot;
    }

    public override void CreateStates() {
        BoidHalt = new BoidHaltState(this);
        BoidSeek = new BoidSeekState(this);
    }

    protected virtual void Awake() {
        BoidTransform = this.GetComponent<Transform>();
        BoidRB = this.GetComponent<Rigidbody2D>();
        BoidCollider = this.GetComponent<Collider2D>();

        CreateStates();
        StartingState(BoidSeek);
    }

    protected virtual void Update() {
        DirToTarget = Camera.main.ScreenToWorldPoint(Input.mousePosition) - BoidTransform.position;

        if (ElapsedTime <= BoidConfig.TurnTime) {
            ElapsedTime += Time.deltaTime;
            BoidTransform.rotation = Quaternion.Slerp(startRot, endRot, ElapsedTime / BoidConfig.TurnTime);
        }

        CurrentState.OnUpdate();
    }

    protected virtual void FixedUpdate() {
        CurrentState.OnFixedUpdate();
    }
}

public class BoidHaltState : UnityFSMState<Boid> {

    public BoidHaltState(Boid boid) : base(boid) { }

    public override void OnStateEntry() {
        myFSM.BoidRB.velocity *= 0f;
        myFSM.BoidRB.angularVelocity *= 0f;
        myFSM.BoidRB.isKinematic = true;
        myFSM.UpdateRotateVars(myFSM.BoidTransform.rotation, Utility.XLookRotation2D(myFSM.DirToTarget));
    }

    public override void OnStateExit() {
        myFSM.BoidRB.isKinematic = false;
        myFSM.GetComponent<SpriteRenderer>().color = Color.white;
    }

    public override void OnUpdate() {
        if (myFSM.ElapsedTime > myFSM.BoidConfig.TurnTime) {
            FacingTargetBehavior();
        }
    }

    public override void OnFixedUpdate() {
        if (myFSM.DirToTarget.sqrMagnitude <= myFSM.BoidConfig.KeepDistance) {
            if (myFSM.DirToTarget.sqrMagnitude < myFSM.BoidConfig.KeepDistance - myFSM.BoidConfig.Threshold) {
                RetreatBehavior();
            }
        }
        else {
            myFSM.GoToState(myFSM.BoidSeek);
        }
    }

    protected virtual void FacingTargetBehavior() {
        myFSM.GetComponent<SpriteRenderer>().color = Color.red;
    }

    protected virtual void RetreatBehavior() {
        myFSM.BoidRB.velocity = myFSM.DirToTarget * -1;
        myFSM.BoidRB.velocity = myFSM.BoidRB.velocity.normalized * myFSM.BoidConfig.MoveSpeed;
        myFSM.UpdateRotateVars(myFSM.BoidTransform.rotation, Utility.XLookRotation2D(myFSM.BoidRB.velocity));
        myFSM.GetComponent<SpriteRenderer>().color = Color.yellow;
    }
}

public class BoidSeekState : UnityFSMState<Boid> {

    private Collider2D[] nearbyColliders = new Collider2D[100];
    private Vector2 currentVelocity;
    private Vector2 cohesionVector;
    private Vector2 alignmentVector;
    private Vector2 separationVector;
    private Vector2 separationDir;
    private Vector2 orthogonalDir;

    public BoidSeekState(Boid boid) : base(boid) { }

    public override void OnFixedUpdate() {
        if (myFSM.DirToTarget.sqrMagnitude <= myFSM.BoidConfig.KeepDistance) {
            myFSM.GoToState(myFSM.BoidHalt);
        }
        else {
            myFSM.BoidRB.velocity = myFSM.DirToTarget;
            myFSM.BoidRB.velocity = ((myFSM.BoidRB.velocity) * myFSM.BoidConfig.PathWeight + CalcFlockingVector()).normalized * myFSM.BoidConfig.MoveSpeed;
            myFSM.UpdateRotateVars(myFSM.BoidTransform.rotation, Utility.XLookRotation2D(myFSM.BoidRB.velocity));
        }
    }

    private Vector2 CalcFlockingVector() {
        cohesionVector = alignmentVector = separationVector = Vector2.zero;
        nearbyColliders = Physics2D.OverlapCircleAll(myFSM.BoidTransform.position, myFSM.BoidConfig.CheckRadius, myFSM.BoidConfig.CheckLayer);

        for (int i = 0; i < nearbyColliders.Length; i++) {
            if (nearbyColliders[i] != myFSM.BoidCollider) {
                separationDir = myFSM.BoidTransform.position - nearbyColliders[i].transform.position;
                separationDir = separationDir.normalized / separationDir.sqrMagnitude; // weight by distance
                orthogonalDir.x = separationDir.y;
                orthogonalDir.y = -separationDir.x;
                separationVector += separationDir + orthogonalDir;

                cohesionVector.x += nearbyColliders[i].transform.position.x;
                cohesionVector.y += nearbyColliders[i].transform.position.y;
                alignmentVector += nearbyColliders[i].attachedRigidbody.velocity;
            }
        }

        if (nearbyColliders.Length > 0) {
            cohesionVector /= nearbyColliders.Length;
            alignmentVector /= nearbyColliders.Length;
        }

        cohesionVector.x -= myFSM.BoidTransform.position.x;
        cohesionVector.y -= myFSM.BoidTransform.position.y;
        cohesionVector = Vector2.SmoothDamp(myFSM.BoidTransform.right, cohesionVector, ref currentVelocity, 1f);

        return (cohesionVector * myFSM.BoidConfig.CohesionWeight) + (alignmentVector * myFSM.BoidConfig.AlignmentWeight) + (separationVector * myFSM.BoidConfig.SeparationWeight);
    }
}

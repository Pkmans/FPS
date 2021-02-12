using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKenemy : MonoBehaviour
{    
    public LayerMask whatIsGround;

    [Header("IK stuff")]
    //the legs of the robot
    //IMPORTANT: order is right1, left, right2, left2 etc
    public FastIKFabric[] legs; 

    //MAIN body for defining orientation (hips / torso etc.)
    public Transform root;
    
    //position to raycast down from to find targetposition
    private Transform[] legTargetHomes;
    //target positions leg can move to
    private Vector3[] targetPositions, currentPositions;

    //move leg to targetposition if distance greater than threshold
    public float thresholdDistance;
    private float startThreshold;
    public float offSetMult;

    //how far legs are from desired destination
    private float[] legProgress;

    private StabilizeEnemy stabilizeScript;

    void Start() {
        legTargetHomes = new Transform[legs.Length];
        targetPositions = new Vector3[legs.Length];
        currentPositions = new Vector3[legs.Length];
        legProgress = new float[legs.Length];
        startThreshold = thresholdDistance;
        stabilizeScript = GetComponent<StabilizeEnemy>();
        InitLegTargetHomes();

        UpdateLegTargetPositions();
        UpdateCurrentLegPosition(0);
        UpdateCurrentLegPosition(1);
    }

    // Update is called once per frame
    void Update() {
        UpdateLegTargetPositions();
        UpdateCurrentLegPositions();
        MoveLegs();
    }

    public void CollisionDetected() {
        thresholdDistance = 999f;     //enables 'ragdoll' mode
        stabilizeScript.enabled = false;

        Invoke("renable", 2f);
    }

    void renable() {
        thresholdDistance = startThreshold;
        stabilizeScript.enabled = true;

    }

    ///SUMMARY: Set legTargetHome to be chain root, to raycast down and find target point
    void InitLegTargetHomes() {
        for (int i = 0; i < legs.Length; i ++) {
            int chainLength = legs[i].ChainLength;
            Transform chainRoot = legs[i].transform;
            while (chainLength > 0) {
                chainRoot = chainRoot.parent;
                chainLength--;
            }

            legTargetHomes[i] = chainRoot;
        }
    }


    void UpdateLegTargetPositions() {
        for (int i = 0; i < legs.Length; i ++) {
            //position to raycast from
            Vector3 rayCastPosition = legTargetHomes[i].position;

            RaycastHit hit;
            if (Physics.Raycast(rayCastPosition, Vector3.down, out hit, 10f, whatIsGround))
                targetPositions[i] = hit.point;

        }
    }

    void UpdateCurrentLegPositions() {
        for (int i = 0; i < legs.Length; i ++) {
            if(!OppositeLegGrounded(i)) return;
            if (Vector3.Distance(currentPositions[i], targetPositions[i]) > thresholdDistance)
                UpdateCurrentLegPosition(i);
        }
    }
    
    bool OppositeLegGrounded(int leg) {
        int otherLeg = (leg + 1) % (legs.Length); //gets the opposite and diagonal leg
        return legProgress[otherLeg] < 0.01f;
    }

    void UpdateCurrentLegPosition (int legIndex) {
        // overshoot direction of leg moving
        Vector3 offSetDir = targetPositions[legIndex] - currentPositions[legIndex];
        Vector3 offSet = offSetDir.normalized * offSetMult;
        offSet = Vector3.ProjectOnPlane(offSet, Vector3.up);

        currentPositions[legIndex] = targetPositions[legIndex] + offSet;
        legProgress[legIndex] = 1f;

    }

    private float legSpeed = 15f;
    private float currentSpeed = 1f;
    
    void MoveLegs() {
        for (int i = 0; i < legs.Length; i++) {
            Transform legTarget = legs[i].Target;
            
            //update lerp progress
            legProgress[i] = Mathf.Lerp(legProgress[i], 0, Time.deltaTime * legSpeed);
            //distance from desination
            Vector3 upwardsOffset = Vector3.up * legProgress[i];
            //smoothly lerp leg to target position 
            legTarget.position = Vector3.Lerp(legTarget.position, currentPositions[i] + upwardsOffset, Time.deltaTime * legSpeed);
        }
    }

    // void OnDrawGizmos() {
    //     Gizmos.color = Color.red;

    //     //legTargetHomes
    //     Gizmos.DrawSphere(legTargetHomes[0].position, 0.15f);
    //     Gizmos.DrawSphere(legTargetHomes[1].position, 0.15f);

    //     //targetPositions
    //     Gizmos.color = Color.cyan;
    //     Gizmos.DrawSphere(targetPositions[0], 0.15f);
    //     Gizmos.DrawSphere(targetPositions[1], 0.15f);

    //     //currentPositiosn
    //     Gizmos.color = Color.yellow;
    //     Gizmos.DrawSphere(currentPositions[0], 0.15f);
    //     Gizmos.DrawSphere(currentPositions[1], 0.15f);
    // }

}

using UnityEngine;
using NTC.Global.Cache;

public class ShieldEnemyAi : MonoCache{
    [SerializeField] private Transform bodyTipPoint;

    [SerializeField] private float upForce, downForce;

    [SerializeField] private Rigidbody bodyBase;
    [SerializeField] private Rigidbody[] legTips;
    [SerializeField] private Rigidbody handPivot;

    [SerializeField] private float a = 10;

    private bool _inStun;

    protected override void FixedRun(){
        if (_inStun || !bodyTipPoint) return;

        DoForces();
        handPivot.transform.rotation = (Quaternion.RotateTowards(handPivot.transform.rotation, Quaternion.Euler(Vector3.back), a)); 
    }

    private void DoForces(){
        bodyBase.AddForceAtPosition(Vector3.up * upForce, bodyTipPoint.position);

        for (int i = 0; i < legTips.Length; i++){
            legTips[i].AddForce(Vector3.down * downForce);
        }
    }
}

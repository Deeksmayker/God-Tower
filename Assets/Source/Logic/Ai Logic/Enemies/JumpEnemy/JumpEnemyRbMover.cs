using UnityEngine;
using NTC.Global.Cache;

public class JumpEnemyRbMover : MonoCache{
    [SerializeField] private Transform bodyTipPoint;
    [SerializeField] private Transform checkContactPoint;
    [SerializeField] private float checkContactRadius;

    [SerializeField] private float jumpForce;
    [SerializeField] private float bodyUpForce;
    [SerializeField] private float maxFlySpeed;

    [SerializeField] private Rigidbody[] grabHands;
    [SerializeField] private Rigidbody bodyBase;

    private float _dontGrabTimer;

    private bool _inStun;
    private bool _isContacting;
    private bool _sticking;

    //private JumpEnemyHealthSystem _health;

    private void Awake(){
        //_health = GetComponent<JumpEnemyHealthSystem>();
        StickToClosestSurface();
    }

    protected override void FixedRun(){
        if (_inStun || !checkContactPoint) return;

        if (_dontGrabTimer > 0) _dontGrabTimer -= Time.fixedDeltaTime;
        
       var newContacting = Physics.CheckSphere(checkContactPoint.position, checkContactRadius, Layers.Environment);
       _isContacting = newContacting;

       if (IsContacting()){
           DoUpForce();
       }

       if (!IsSticking() && !IsGrounded() && _dontGrabTimer <= 0){
           TryGrab();
       }
    }

    public void JumpToDirection(Vector3 direction){
        Log("Jumping to direction - " + direction);
        StopGrab();
        SetVelocity(direction * jumpForce);
        _dontGrabTimer = 1f;
    }

    private void DoUpForce(){
        bodyBase.AddForceAtPosition(bodyUpForce * Time.fixedDeltaTime * Vector3.up, bodyTipPoint.position);
    }

    private void TryGrab(){
        for (var i = 0; i < grabHands.Length; i++){
            //var closestHit = PhysicsUtils.GetClosestSurfaceHit(grabHands[i].transform.position);
            var surfaceClose = Physics.CheckSphere(grabHands[i].transform.position, 1.5f * grabHands[i].transform.localScale.x, Layers.Environment);
            if (surfaceClose){
                grabHands[i].constraints = RigidbodyConstraints.FreezePosition;
                _sticking = true;
                SetVelocity(bodyBase.velocity * 0.1f);
            }
        }
    }

    private void StopGrab(){
        for (var i = 0; i < grabHands.Length; i++){
            grabHands[i].constraints = RigidbodyConstraints.None;
            _sticking = false;
        }
    }

    public void AccelerateTowardsPoint(Vector3 point, float power){
        if (_inStun || !checkContactPoint) return;
        var damping = 0.15f;
        _dontGrabTimer = 1f;

        for (int i = 0 ; i < grabHands.Length; i++){
            grabHands[i].velocity += power * Time.deltaTime * (point-grabHands[i].transform.position).normalized;
            grabHands[i].velocity *= Mathf.Clamp01(1f - damping*Time.deltaTime);
        }

        bodyBase.velocity += power * Time.deltaTime * (point-bodyBase.position).normalized;
        bodyBase.velocity *= Mathf.Clamp01(1f - damping*Time.deltaTime);

        if (bodyBase.velocity.sqrMagnitude > maxFlySpeed * maxFlySpeed){
            bodyBase.velocity = Vector3.ClampMagnitude(bodyBase.velocity, maxFlySpeed);
        }
    }

    public void StartStun()
    {
        //_rotator.SetTorque(UnityEngine.Random.insideUnitSphere.normalized * 1000);

        _inStun = true;
        StopGrab();
    }

    public void EndStun()
    {
        _inStun = false;
    }

    private void AddVelocity(Vector3 addedVelocity){
        bodyBase.velocity += addedVelocity;

        for (var i = 0; i < grabHands.Length; i++){
            grabHands[i].velocity += addedVelocity;
        }
    }

    private void SetVelocity(Vector3 newVelocity){
        bodyBase.velocity = newVelocity;

        for (var i = 0; i < grabHands.Length; i++){
            grabHands[i].velocity = newVelocity;
        }
    }

    public bool IsContacting(){
        return !_inStun && _isContacting;
    }

    public bool IsGrounded(){
        return Physics.Raycast(checkContactPoint.position, Vector3.down, 10 * transform.localScale.y, Layers.Environment);
    }

    public bool IsSticking() => _sticking;

    public Vector3 GetCurrentNormal(){
        return PhysicsUtils.GetClosestSurfaceHit(bodyBase.transform.position).normal;
    }

	private void StickToClosestSurface(){
		var closestHit = PhysicsUtils.GetClosestSurfaceHit(bodyBase.transform.position);

        SetVelocity((closestHit.point - bodyBase.transform.position).normalized * jumpForce);
	}

    private void OnDrawGizmosSelected(){
        if (!checkContactPoint) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(checkContactPoint.position, checkContactRadius);
    }
}

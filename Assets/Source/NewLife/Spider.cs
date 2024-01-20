using UnityEngine;

public class Spider : MonoBehaviour{
    [SerializeField] private Transform[] legs;
    [SerializeField] private float moveDelay = 5f;
    [SerializeField] private float walkTime = 1f;
    [SerializeField] private float speed;
    
    private float _delayTimer;
    private float _walkT;
    private float _startY;
    
    private bool _walking;
    private Vector3 _moveStartPosition;
    private Vector3 _foundPlayerPosition;
    
    private LegState[] _legStates;
    
    private SphereCollider _collider;
    
    private void Awake(){
        _collider = GetComponent<SphereCollider>();
        /*
        if (Physics.Raycast(transform.position, Vector3.down, out var hit, 100, Layers.Environment)){
            transform.position = new Vector3(hit.point.x, hit.point.y + _collider.radius * (transform.localScale.y * 1.3f), hit.point.z);
            _startY = transform.position.y;
        }
        */
        
        _legStates = new LegState[legs.Length];
        for (int i = 0; i < _legStates.Length; i++){
            _legStates[i] = new LegState();
            _legStates[i].legTransform = legs[i];
            _legStates[i].speed = Random.Range(10f, 30f);
        }
    }
    
    private void Update(){
        if (!Grid.Instance || !Grid.Instance.PlayerGrid) return;
        if (!_walking && _delayTimer <= 0){
            _walking = true;
            _moveStartPosition = transform.position;
            _delayTimer = moveDelay;
        } 
        
        if (_walking){
            _walkT += Time.deltaTime / walkTime; 
            _foundPlayerPosition = PlayerLocator.Instance.GetPlayerPos();
            var wishDirection = (_foundPlayerPosition - transform.position).normalized;
            wishDirection *= speed * Time.deltaTime;
            transform.position += Grid.Instance.GetMoveDirection(transform.position, wishDirection);
            if (_walkT >= 1){
                _walkT = 0;
                _walking = false;
            }
        } else{
            _delayTimer -= Time.deltaTime;
        }
        
        //Leg Animation
        var animationSpeedMultiplier = _walking ? 1f : 0.1f;
        for (int i = 0; i < _legStates.Length; i++){
            var startY = _legStates[i].legTransform.eulerAngles.y;
            _legStates[i].legTransform.rotation = Quaternion.AngleAxis(
            Mathf.Abs(Mathf.Sin(Time.time * _legStates[i].speed * animationSpeedMultiplier) * 40), Vector3.right);
            _legStates[i].legTransform.eulerAngles = new Vector3(_legStates[i].legTransform.eulerAngles.x,
                                                     startY, _legStates[i].legTransform.eulerAngles.z);
        }
    }   
}

public class LegState{
    public Transform legTransform;
    public float speed;
}

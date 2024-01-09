using UnityEngine;

public class RealBallShooter : MonoBehaviour{
    [SerializeField] private RealBall ball;
    
    private LineRenderer _lr;
    
    private void Awake(){
        _lr = GetComponent<LineRenderer>();
        _lr.positionCount = 0;
    }
    
    private void Update(){
        if (Input.GetMouseButton(1)){
            var imaginaryBall = Instantiate(ball);
            imaginaryBall.BallIsReal = false;
            var calculatedPositions = imaginaryBall.PredictPositions(Camera.main.transform.position + Camera.main.transform.forward * 2, Camera.main.transform.forward);
            
            _lr.positionCount = calculatedPositions.Length;
            _lr.SetPositions(calculatedPositions);
            Destroy(imaginaryBall.gameObject);
        } else if (Input.GetMouseButtonUp(1)){
            _lr.positionCount = 0;
        }
        
        if (Input.GetKeyDown(KeyCode.F)){
            var spawnedBall = Instantiate(ball, Camera.main.transform.position + Camera.main.transform.forward * 2, Quaternion.identity);
            //spawnedBall.HandleKick(Camera.main.transform.forward);
        }
    }
    
}

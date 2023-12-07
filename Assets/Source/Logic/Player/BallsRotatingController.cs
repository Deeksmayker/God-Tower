using UnityEngine;
using NTC.Global.Cache;
using System.Linq;

public class BallsRotatingController : MonoCache{
	[SerializeField] private float radius = 5f;
	[SerializeField] private float speed = 100f;
	[SerializeField] private float damping = 5;

	private Vector3 _axis;
	private Vector3 _orbit;

	private Ball[] _balls;

	private void Awake(){
		var rbs = FindObjectsOfType<PlayerBall>().Select(b => b.GetComponent<Rigidbody>()).ToArray();
		_balls = new Ball[rbs.Length];
		
		for (var i = 0; i < rbs.Length; i++){
			_balls[i].Rb = rbs[i];
			_balls[i].Orbit = Random.onUnitSphere * radius;
			_balls[i].Rb.MovePosition(transform.position + _balls[i].Orbit);
			_balls[i].Axis = Random.onUnitSphere;
			_balls[i].Axis += (Vector3.Dot(transform.position, _balls[i].Orbit) - Vector3.Dot(_balls[i].Axis, _balls[i].Orbit)) * Vector3.forward / _balls[i].Orbit.z - transform.position;
		}
	}

	private void FixedUpdate(){
		for (var i = 0; i < _balls.Length; i++){
			_balls[i].Rb.velocity = Vector3.zero;
			_balls[i].Orbit = Quaternion.AngleAxis(speed * Time.fixedDeltaTime, _balls[i].Axis) * _balls[i].Orbit;
			_balls[i].Rb.MovePosition(Vector3.Lerp(_balls[i].Rb.transform.position, transform.position + _balls[i].Orbit, damping * Time.fixedDeltaTime));
		}
	}
}

public struct Ball{
	public Rigidbody Rb;
	public Vector3 Axis;
	public Vector3 Orbit;
}

using UnityEngine;

public class JumpPad : MonoBehaviour{
	[SerializeField] private float speedToMover;
	[SerializeField] private float forceToRb;

	private void OnTriggerEnter(Collider col){
		if (col.TryGetComponent<IMover>(out var mover)){
			mover.SetVerticalVelocity(0);
			mover.AddVelocity(transform.up * speedToMover);
		}
	}
}

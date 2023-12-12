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

    private void OnTriggerStay(Collider col){
		if (col.TryGetComponent<IMover>(out var mover)){
            if (Vector3.Dot(mover.GetVelocity().normalized, transform.up) * speedToMover < speedToMover * 0.9f){
                mover.SetVerticalVelocity(0);
                mover.AddVelocity(transform.up * speedToMover);
            }
        }
    }
}

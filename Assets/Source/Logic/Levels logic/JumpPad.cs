using UnityEngine;

public class JumpPad : MonoBehaviour{
	[SerializeField] private float speedToMover;
	[SerializeField] private float forceToRb = 50;

	private void OnTriggerEnter(Collider col){
		if (col.TryGetComponent<IMover>(out var mover)){
			mover.SetVerticalVelocity(0);
			mover.AddVelocity(transform.up * speedToMover);
		}

        var rbUnit = col.GetComponentInParent<RbUnit>();
        if (rbUnit){
            rbUnit.SetVelocity(transform.up * forceToRb);
        }

        else if (col.TryGetComponent<Rigidbody>(out var rb)){
            var velocity = rb.velocity;
            velocity.y = 0;
            velocity += (transform.up * forceToRb);
            rb.velocity = velocity;
        }
	}

    private void OnTriggerStay(Collider col){
		if (col.TryGetComponent<IMover>(out var mover)){
            if (Vector3.Dot(mover.GetVelocity().normalized, transform.up) * speedToMover < speedToMover * 0.9f){
                mover.SetVerticalVelocity(0);
                mover.AddVelocity(transform.up * speedToMover);
            }
        }

        var rbUnit = col.GetComponentInParent<RbUnit>();
        if (rbUnit){
            rbUnit.SetVelocity(transform.up * forceToRb);
        }

        else if (col.TryGetComponent<Rigidbody>(out var rb)){
            if (Vector3.Dot(rb.velocity.normalized, transform.up) * forceToRb < forceToRb * 0.9f){
                var velocity = rb.velocity;
                velocity.y = 0;
                velocity += (transform.up * forceToRb);
                rb.velocity = velocity;
            }
        }
    }
}

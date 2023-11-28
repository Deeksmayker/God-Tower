using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidGpu : MonoBehaviour, IMover
{

    BoidSettings settings;

    // State
    [HideInInspector]
    public Vector3 position;
    [HideInInspector]
    public Vector3 forward;
    Vector3 velocity;

    // To update:
    Vector3 acceleration;
    [HideInInspector]
    public Vector3 avgFlockHeading;
    [HideInInspector]
    public Vector3 avgAvoidanceHeading;
    [HideInInspector]
    public Vector3 centreOfFlockmates;
    [HideInInspector]
    public int numPerceivedFlockmates;

    // Cached
    Material material;
    Transform cachedTransform;
    Transform target;

    private bool _moveMyself = true;

    public event Action OnLanding;
    public event Action<Vector3> OnBounce;
/*
    void Awake () {
        material = transform.GetComponentInChildren<MeshRenderer> ().material;
        cachedTransform = transform;
    }

    public void Initialize (BoidSettings settings, Transform target) {
        this.target = target;
        this.settings = settings;

        position = cachedTransform.position;
        forward = cachedTransform.forward;

        float startSpeed = (settings.minSpeed + settings.maxSpeed) / 2;
        velocity = transform.forward * startSpeed;
    }

    public void SetColour (Color col) {
        if (material != null) {
            material.color = col;
        }
    }

    private void OnDisable()
    {
        position = Vector3.positiveInfinity;
    }

    public void UpdateBoid () 
    {
        if (!this)
            return;
        if (!_moveMyself)
        {
            transform.position += velocity * Time.deltaTime;
            return;
        }

        Vector3 acceleration = Vector3.zero;

        if (target != null) {
            Vector3 offsetToTarget = (target.position - position);
            acceleration = SteerTowards (offsetToTarget) * settings.targetWeight;
        }

        if (numPerceivedFlockmates != 0) {
            centreOfFlockmates /= numPerceivedFlockmates;

            Vector3 offsetToFlockmatesCentre = (centreOfFlockmates - position);

            var alignmentForce = SteerTowards (avgFlockHeading) * settings.alignWeight;
            var cohesionForce = SteerTowards (offsetToFlockmatesCentre) * settings.cohesionWeight;
            var seperationForce = SteerTowards (avgAvoidanceHeading) * settings.seperateWeight;

            acceleration += alignmentForce;
            acceleration += cohesionForce;
            acceleration += seperationForce;
        }

        if (IsHeadingForCollision ()) {
            Vector3 collisionAvoidDir = ObstacleRays ();
            Vector3 collisionAvoidForce = SteerTowards (collisionAvoidDir) * settings.avoidCollisionWeight;
            acceleration += collisionAvoidForce;
        }

        velocity += acceleration * Time.deltaTime;
        float speed = velocity.magnitude;
        Vector3 dir = velocity / speed;
        speed = Mathf.Clamp (speed, settings.minSpeed, settings.maxSpeed);
        velocity = dir * speed;

        cachedTransform.position += velocity * Time.deltaTime;
        cachedTransform.forward = dir;
        position = cachedTransform.position;
        forward = dir;
    }

    bool IsHeadingForCollision () {
        RaycastHit hit;
        if (Physics.SphereCast (position, settings.boundsRadius, forward, out hit, settings.collisionAvoidDst, settings.obstacleMask)) {
            return true;
        } else { }
        return false;
    }

    Vector3 ObstacleRays () {
        Vector3[] rayDirections = BoidHelper.directions;

        for (int i = 0; i < rayDirections.Length; i++) {
            Vector3 dir = cachedTransform.TransformDirection (rayDirections[i]);
            Ray ray = new Ray (position, dir);
            if (!Physics.SphereCast (ray, settings.boundsRadius, settings.collisionAvoidDst, settings.obstacleMask)) {
                return dir;
            }
        }

        return forward;
    }

    Vector3 SteerTowards (Vector3 vector) {
        Vector3 v = vector.normalized * settings.maxSpeed - velocity;
        return Vector3.ClampMagnitude (v, settings.maxSteerForce);
    }
*/

    public void SetInput(Vector3 input)
    {
        Debug.LogError("NO INPUT FOR BOIDS");
    }

    public void SetVerticalVelocity(float velocity)
    {
        this.velocity.y = velocity;
    }

    public void AddVerticalVelocity(float addedVelocity)
    {
        this.velocity.y += addedVelocity;
    }

    public void SetVelocity(Vector3 newVelocity)
    {
        this.velocity = newVelocity; 
    }

    public void AddVelocity(Vector3 addedVelocityVector)
    {
        this.velocity += addedVelocityVector;
    }

    public void SetInputResponse(bool value)
    {
        _moveMyself = value; 
    }

    public Vector3 GetVelocity()
    {
        return velocity;
    }

    public float GetVelocityMagnitude()
    {
        return velocity.magnitude;
    }

    public bool IsGrounded()
    {
        throw new NotImplementedException();
    }

    public void AddForce(Vector3 force)
    {
        velocity += force;
    }
}
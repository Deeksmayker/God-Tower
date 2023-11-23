using NTC.Global.Cache;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

public class BoidsBehaviour : MonoCache
{
    [BurstCompile]
    public struct BoidMoveJob : IJobParallelForTransform
    {
        [WriteOnly]
        public NativeArray<Vector3> Positions;
        public NativeArray<Vector3> Velocities;
        public NativeArray<Vector3> Accelerations;

        public float MinSpeed, MaxSpeed;
        public float DeltaTime;

        public void Execute(int index, TransformAccess transform)
        {
            var velocity = Velocities[index] + Accelerations[index] * DeltaTime;
            var speed = velocity.magnitude;
            var dir = velocity / speed;
            speed = Mathf.Clamp(speed, MinSpeed, MaxSpeed);
            velocity = dir * speed;

            transform.position += velocity * DeltaTime;
            transform.rotation = Quaternion.LookRotation(dir);

            Positions[index] = transform.position;
            Velocities[index] = velocity;
            Accelerations[index] = Vector3.zero;
        }
    }

    [BurstCompile]
    public struct AccelerationJob : IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<Vector3> Positions; 
        [ReadOnly]
        public NativeArray<Vector3> Velocities; 
        public NativeArray<Vector3> Accelerations;

        [ReadOnly]
        public float PerceptionRadius;
        public float AvoidanceRadius;
        public float TargetWeight;
        public float AlignWeight;
        public float CohesionWeight;
        public float SeparateWeight;
        public float MaxSpeed;
        public float MaxSteerForce;
        [ReadOnly]
        public Vector3 TargetPos;

        private int Count => Positions.Length - 1;

        public void Execute(int index)
        {
            var numFlockmates = 0;
            var flockHeading = Vector3.zero;
            var flockCenter = Vector3.zero;
            var separationHeading = Vector3.zero;

            for (int i = 0; i < Count; i++)
            {
                if (i == index)
                    continue;

                var offset = Positions[i] - Positions[index];
                var sqrDst = offset.sqrMagnitude;

                if (sqrDst >= PerceptionRadius * PerceptionRadius)
                    continue;

                numFlockmates += 1;
                flockHeading += Velocities[i].normalized;
                flockCenter += Positions[i];

                if (sqrDst < AvoidanceRadius * AvoidanceRadius)
                {
                    separationHeading -= offset / sqrDst;
                }
            }

            if (!TargetPos.Equals(Vector3.zero))
            {
                var offetToTarget = TargetPos - Positions[index];
                Accelerations[index] = SteerTowards(offetToTarget, Velocities[index]) * TargetWeight;
            }

            if (numFlockmates != 0)
            {
                flockCenter /= numFlockmates;
                var offsetToFlockCenter = flockCenter - Positions[index];
                var alignmentForce = SteerTowards(flockHeading, Velocities[index]) * AlignWeight;
                var cohesionForce = SteerTowards(offsetToFlockCenter, Velocities[index]) * CohesionWeight;
                var separationForce = SteerTowards(separationHeading, Velocities[index]) * SeparateWeight;

                Accelerations[index] += alignmentForce;
                Accelerations[index] += cohesionForce;
                Accelerations[index] += separationForce;
            }

        }

        Vector3 SteerTowards (Vector3 vector, Vector3 velocity)
        {
            Vector3 v = vector.normalized * MaxSpeed - velocity;
            return Vector3.ClampMagnitude (v, MaxSteerForce);
        }
    }

    [SerializeField] private int numberOfBoids;

    [SerializeField] private GameObject entityPrefab;

    [SerializeField] private BoidSettings settings;

    private Transform _targetTransform;

    private NativeArray<Vector3> _positions;
    private NativeArray<Vector3> _velocities;
    private NativeArray<Vector3> _accelerations;

    private TransformAccessArray _transformAccessArray;

    private void Start()
    {
        _targetTransform = FindObjectOfType<PlayerTargetPoint>().transform;

        _positions = new NativeArray<Vector3>(numberOfBoids, Allocator.Persistent);
        _velocities = new NativeArray<Vector3>(numberOfBoids, Allocator.Persistent);
        _accelerations = new NativeArray<Vector3>(numberOfBoids, Allocator.Persistent);

        var transforms = new Transform[numberOfBoids];
        for (var i = 0; i < numberOfBoids; i++)
        {
            transforms[i] = Instantiate(entityPrefab, transform.position, Quaternion.identity).transform;
            _velocities[i] = Random.insideUnitSphere;
        }

        _transformAccessArray = new TransformAccessArray(transforms);
    }

    private JobHandle _accelerationJobHandle;
    private JobHandle _moveJobHandle;

    protected override void Run()
    {
        _moveJobHandle.Complete();

        var accelerationJob = new AccelerationJob()
        {
            Positions = _positions,
            Velocities = _velocities,
            Accelerations = _accelerations,
            PerceptionRadius = settings.perceptionRadius,
            AvoidanceRadius = settings.avoidanceRadius,
            TargetWeight = settings.targetWeight,
            AlignWeight = settings.alignWeight,
            CohesionWeight = settings.cohesionWeight,
            SeparateWeight = settings.seperateWeight,
            MaxSpeed = settings.maxSpeed,
            MaxSteerForce = settings.maxSteerForce,
            TargetPos = _targetTransform.position
        };

        _accelerationJobHandle = accelerationJob.Schedule(_positions.Length, 0);
    }

    private int _index;

    protected override void LateRun()
    {
        var moveJob = new BoidMoveJob()
        {
            Positions = _positions,
            Velocities = _velocities,
            Accelerations = _accelerations,
            MinSpeed = settings.minSpeed,
            MaxSpeed = settings.maxSpeed,
            DeltaTime = Time.deltaTime
        };
        _accelerationJobHandle.Complete();
        /*
        for (var i = _index; i < _index + 50 && i < numberOfBoids; i++)
        {
            if (IsHeadingForCollision(_positions[i], _velocities[i].normalized))
            {
                var collisionAvoidDir = ObstacleRays(_positions[i], _velocities[i].normalized);
                var collisionAvoidForce = SteerTowards(collisionAvoidDir, _velocities[i]) * settings.avoidCollisionWeight;
                _accelerations[i] += collisionAvoidForce;
            }
        }

        _index += 50;
        _index %= numberOfBoids;
        */
        _moveJobHandle = moveJob.Schedule(_transformAccessArray);
        
    }

    bool IsHeadingForCollision (Vector3 position, Vector3 forward) {
        RaycastHit hit;
        if (Physics.SphereCast (position, settings.boundsRadius, forward, out hit, settings.collisionAvoidDst, settings.obstacleMask)) {
            return true;
        } else { }
        return false;
    }

    Vector3 ObstacleRays (Vector3 position, Vector3 forward) {
        Vector3[] rayDirections = BoidHelper.directions;

        for (int i = 0; i < rayDirections.Length; i++) {
            if (!_transformAccessArray[i])
                continue;
            Vector3 dir = _transformAccessArray[i].TransformDirection(rayDirections[i]);
            Ray ray = new Ray (position, dir);
            if (!Physics.SphereCast (ray, settings.boundsRadius, settings.collisionAvoidDst, settings.obstacleMask)) {
                return dir;
            }
        }

        return forward;
    }
    Vector3 SteerTowards (Vector3 vector, Vector3 velocity)
    {
        Vector3 v = vector.normalized * settings.maxSpeed - velocity;
        return Vector3.ClampMagnitude (v, settings.maxSteerForce);
    }

    private void OnDestroy()
    {
        _positions.Dispose();
        _velocities.Dispose();
        _accelerations.Dispose();
        _transformAccessArray.Dispose();
    }
}
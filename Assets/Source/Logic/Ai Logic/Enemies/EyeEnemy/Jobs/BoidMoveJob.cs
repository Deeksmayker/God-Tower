using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;
/*
public class BoidMoveJob : IJobParallelForTransform
{
    public NativeArray<Vector3> Positions;
    [ReadOnly]
    public NativeArray<Vector3> Velocities;

    public float DeltaTime;

    public void Execute(int index, TransformAccess transform)
    {
        transform.position += Velocities[index] * DeltaTime;
        transform.rotation = Quaternion.LookRotation(Velocities[index]);

        Positions[index] = transform.position;
    }
}
*/
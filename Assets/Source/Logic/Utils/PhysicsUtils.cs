using UnityEngine;

public static class PhysicsUtils{
	public static RaycastHit GetClosestSurfaceHit(Vector3 pos){
        RaycastHit closestHit = new();
        var hits = new RaycastHit[6];

        Physics.Raycast(pos, Vector3.down, out hits[0], 100, Layers.Environment);
        Physics.Raycast(pos, -Vector3.forward, out hits[1], 100, Layers.Environment);
        Physics.Raycast(pos, Vector3.up, out hits[2], 100, Layers.Environment);
        Physics.Raycast(pos, Vector3.forward, out hits[3], 100, Layers.Environment);
        Physics.Raycast(pos, Vector3.right, out hits[4], 100, Layers.Environment);
        Physics.Raycast(pos, Vector3.left, out hits[5], 100, Layers.Environment);
        
        closestHit = hits[0];
        for (var i = 1; i < hits.Length; i++)
        {
            if (Vector3.Distance(closestHit.point, pos) > Vector3.Distance(hits[i].point, pos))
                closestHit = hits[i];
        }
		
		return closestHit;
	}
}

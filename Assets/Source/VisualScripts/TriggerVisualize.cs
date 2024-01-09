using UnityEngine;
using NTC.Global.Cache;

public class TriggerVisualize : MonoCache{
	[SerializeField] private Color baseColor = Color.green;

	private BoxCollider _col;

	private bool _inside;

	private void Awake(){
		_col = GetComponent<BoxCollider>();
	}

	private void OnValidate(){
		_col = GetComponent<BoxCollider>();
	}

	private void OnTriggerEnter(Collider col){
		_inside = true;
	}

	private void OnTriggerExit(Collider col){
		_inside = false;
	}

	private void OnDrawGizmos(){
		var color = _inside ? Color.red : baseColor;
		color.a = 0.1f;
		Gizmos.color = color;

		Gizmos.matrix = transform.localToWorldMatrix;

		Gizmos.DrawCube(Vector3.zero + _col.center, _col.size);
		color.a = 1;
		Gizmos.color = color;
		Gizmos.DrawWireCube(Vector3.zero + _col.center, _col.size);
	}
}

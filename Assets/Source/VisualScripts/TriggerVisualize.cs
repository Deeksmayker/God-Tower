using UnityEngine;
using NTC.Global.Cache;

public class TriggerVisualize : MonoCache{
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
		var color = _inside ? Color.red : Color.green;
		color.a = 0.1f;
		Gizmos.color = color;

		Gizmos.DrawCube(transform.position, _col.size);
		color.a = 1;
		Gizmos.color = color;
		Gizmos.DrawWireCube(transform.position, _col.size);
	}
}

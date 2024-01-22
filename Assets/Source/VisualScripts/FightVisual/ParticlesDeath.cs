using UnityEngine;
using NTC.Global.Cache;

public class ParticlesDeath : MonoCache{
    private float _time;
    private float _timer;

	private void Awake(){
		transform.parent = null;
		GetComponent<ParticleSystem>().emissionRate = 0;
		transform.localScale = Vector3.one;
	}

    protected override void Run(){
        _timer += Time.deltaTime;
        if (_timer >= _time)
            Destroy(gameObject);
    }

    public void SetDeathTime(float time){
        _time = time;
    }
}

using NTC.Global.Cache;
using System;
using System.Collections;
using UnityEngine;

public class PlayerHealthHandler : MonoCache, IHealthHandler
{
    [SerializeField] private float health;
	[SerializeField] private GameObject deadMan;
	[SerializeField] private float dmgImmortalTime = 0.3f;
	[SerializeField] private float pushForcePerDamage = 5;

	private bool _dead;

	private float _immortalTimer;

    private float _maxHealth;

	private IMover _mover;

    public event Action OnHit;
    public event Action<float> OnHealthChanged;
    public event Action OnHealthAdd;
    public event Action OnDied;

    private void Awake()
    {
        _maxHealth = health;

		_mover = GetComponent<IMover>();
    }

    protected override void OnEnabled()
    {
        var hitTaker = GetComponentInChildren<ITakeHit>();
        hitTaker.OnTakeHit += ChangeHealth;
		hitTaker.OnTakeHitWithDescription += Knockback;
    }

    protected override void OnDisabled()
    {
        var hitTaker = GetComponentInChildren<ITakeHit>();
        hitTaker.OnTakeHit -= ChangeHealth;
		hitTaker.OnTakeHitWithDescription -= Knockback;
    }

	protected override void Run(){
		if (_immortalTimer > 0)
			_immortalTimer -= Time.deltaTime;
	}

    public void Die(bool order = false)
    {
		if (_dead) return;
        Log("I'm a dead man");
        OnDied?.Invoke();

		if (deadMan) StartCoroutine(MakeDeadMan());

		//Instantiate(deadCamPrefab, transform.position, transform.rotation);
		//Destroy(gameObject);
    }

    public void ChangeHealth(float changeValue)
    {
		if (changeValue < 0 && _immortalTimer > 0) return;

		health = Mathf.Clamp(health + changeValue, 0, _maxHealth);

		Log($"Health changed by {changeValue} and my health now at {health}");

		if (changeValue < 0){
			OnHit?.Invoke();
			_immortalTimer = dmgImmortalTime;
		}
        OnHealthChanged?.Invoke(health);
        if (health <= 0)
        {
            Die();
        }
    }

	private void Knockback(float dmg, Vector3 attackerPos, string name){
		var dir = (transform.position - attackerPos).normalized * dmg * pushForcePerDamage;
		_mover.AddForce(dir);
	}

    public void SetHealth(float value)
    {
        health = value;
    }

    public float GetHealth01()
    {
        return health / _maxHealth;
    }

	private IEnumerator MakeDeadMan(){
		while (true){
			deadMan.SetActive(true);
			yield return new WaitForSeconds(0.05f);
			deadMan.SetActive(false);
			yield return new WaitForSeconds(0.05f);
		}
	}
}

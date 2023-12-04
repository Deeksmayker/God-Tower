using System;
using System.Threading.Tasks;
using NTC.Global.Cache;
using UnityEngine;
using UnityEngine.Assertions;

public class Wave : MonoCache
{
	public enum EnemyType{
		Jumping,
		Eyes,
		Centipede
	}

	[Serializable]
	public struct SpawnData{
		public EnemyType enemy;
		public Transform point;
		public float spawnDelay;
		[Header("ЕСЛИ ВРАГ УЖЕ ТУТ И ЕГО НУЖНО ПРОСТО ВКЛЮЧИТЬ")]
		public Enemy onSceneEnemy;
	}

    [SerializeField] private SpawnData[] _spawns;

    public event Action OnEnded;

    private int _aliveCount;
	private bool _spawnedAll;

	private Enemy _jumpingPrefab, _eyesSpawnerPrefab, _centipedePrefab;

	private void Awake(){
		_jumpingPrefab = (Resources.Load(ResPath.Enemies + "JumpEnemy/JumpingEnemy") as GameObject).GetComponent<Enemy>();
	}

    public async void StartWave(){
		for (var i = 0; i < _spawns.Length; i++){
			await Task.Delay((int)(_spawns[i].spawnDelay * 1000));

			if (_spawns[i].onSceneEnemy){
				_spawns[i].onSceneEnemy.gameObject.SetActive(true);
				_spawns[i].onSceneEnemy.OnDie += HandleEnemyKilled;
			}else{
				var enemy = Instantiate(GetEnemyPrefabByType(_spawns[i].enemy), _spawns[i].point.position, Quaternion.identity);
				enemy.OnDie += HandleEnemyKilled;
			}

			_aliveCount++;
		}

		_spawnedAll = true;
    }

	private void HandleEnemyKilled(){
		_aliveCount--;
		Log("Enemy Truly killed and now we at alive count: " + _aliveCount);
		Assert.IsTrue(_aliveCount >= 0);

		if (_aliveCount == 0 && _spawnedAll){
			HandleAllEnemiesKilled();
		}
	}

	private Enemy GetEnemyPrefabByType(EnemyType type){
		switch (type){
			case EnemyType.Jumping:
				return _jumpingPrefab;
				break;
			case EnemyType.Eyes:
				return _eyesSpawnerPrefab;
				break;
			case EnemyType.Centipede:
				return _centipedePrefab;
				break;
		}

		return null;
	}

	private void HandleAllEnemiesKilled(){
		Log("All enemies truly died");
		OnEnded?.Invoke();
	}
}

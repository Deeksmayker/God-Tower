using System;
using System.Threading.Tasks;
using NTC.Global.Cache;
using UnityEngine;
using UnityEngine.Assertions;

public class Wave : MonoCache
{
	public enum EnemyType{
	    Spider,
	    Shield,
	    Shooter,
		Jumping,
		Eyes,
		Centipede,
		CentipedeShort,
		CentipedeLong,
        OldShield
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

	private Enemy _jumpingPrefab, _eyesSpawnerPrefab, _centipedePrefab, _centipedeShortPrefab,
			_centipedeLongPrefab, _oldShieldPrefab;
	private Enemy _shield, _shooter, _spider;

	private void Awake(){
		_jumpingPrefab = (Resources.Load(ResPath.Enemies + "JumpEnemy/JumpingEnemy") as GameObject).GetComponent<Enemy>();
		_centipedePrefab = (Resources.Load(ResPath.Enemies + "CentipedeEnemy/CentipedeEnemy") as GameObject).GetComponent<Enemy>();
		_centipedeShortPrefab = (Resources.Load(ResPath.Enemies + "CentipedeEnemy/CentipedeEnemyShort") as GameObject).GetComponent<Enemy>();
		_centipedeLongPrefab = (Resources.Load(ResPath.Enemies + "CentipedeEnemy/CentipedeEnemyLong") as GameObject).GetComponent<Enemy>();
		_eyesSpawnerPrefab = (Resources.Load(ResPath.Enemies + "EyeEnemy/EyeEnemySpawner") as GameObject).GetComponent<Enemy>();
		_oldShieldPrefab = (Resources.Load(ResPath.Enemies + "ShieldEnemy/ShieldEnemy") as GameObject).GetComponent<Enemy>();
		_shield = (Resources.Load("Prefabs/NewLife/Shitovik") as GameObject).GetComponent<Enemy>();
		_shooter = (Resources.Load("Prefabs/NewLife/Shooter") as GameObject).GetComponent<Enemy>();
		_spider = (Resources.Load("Prefabs/NewLife/Spider") as GameObject).GetComponent<Enemy>();
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
			case EnemyType.Shield:
				return _shield;
				break;
			case EnemyType.Spider:
				return _spider;
				break;
			case EnemyType.Shooter:
				return _shooter;
				break;
			case EnemyType.Jumping:
				return _jumpingPrefab;
				break;
			case EnemyType.Eyes:
				return _eyesSpawnerPrefab;
				break;
			case EnemyType.Centipede:
				return _centipedePrefab;
				break;
			case EnemyType.CentipedeShort:
				return _centipedeShortPrefab;
				break;
			case EnemyType.CentipedeLong:
				return _centipedeLongPrefab;
				break;
			case EnemyType.OldShield:
				return _oldShieldPrefab;
				break;
		}

		return null;
	}

	private void HandleAllEnemiesKilled(){
		Log("All enemies truly died");
		OnEnded?.Invoke();
	}
}

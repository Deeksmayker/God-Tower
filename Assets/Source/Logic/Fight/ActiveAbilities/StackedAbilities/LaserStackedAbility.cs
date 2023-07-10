using NTC.Global.Pool;
using UnityEngine;

public class LaserStackedAbility : StackedAbility
{
    [SerializeField] private float shootCount;

    private LaserShooter _laser;

    private void Awake()
    {
        _laser = Get<LaserShooter>();
    }

    public override void PerformOnImpact(Vector3 position)
    {
        for (var i = 0; i < shootCount; i++)
        {
            if (!_laser)
                break;

            var direction = Random.insideUnitSphere;
            direction.y = Mathf.Clamp(direction.y, -0.3f, 0.3f);
            _laser.transform.position = position;
            _laser.ShootLaser(position, direction);
        }

        TimeController.Instance.AddTimeStopDuration(0.005f);

        base.PerformOnImpact(position);

        _performed = true;
    }

    public override AbilityTypes GetStackedAbilityType()
    {
        return AbilityTypes.Laser;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingAbility : DefaultActiveAbility
{
    [SerializeField] private BaseHomingObject HomingPrefab;
    [SerializeField] private float maxSpread = 30f;
    
    public override void PerformAbility(bool isDumping = false)
    {
        base.PerformAbility(isDumping);
        
        var homing = Instantiate(HomingPrefab,GetStartPoint(), directionTarget.rotation);
        
        if (isDumping)
        {
            var randomNumberX = Random.Range(-maxSpread/2, maxSpread/2);
            var randomNumberY = Random.Range(-maxSpread, maxSpread);
            var randomNumberZ = Random.Range(-maxSpread, maxSpread);

            homing.transform.Rotate(randomNumberX, randomNumberY, randomNumberZ);
        }
        
        homing.Get<Rigidbody>().velocity = (homing.transform.forward + 0.5f * homing.transform.up) * 10;
    }
}

 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    //where do the bullets come form.
    public Transform muzzle;
    
    //which bullets do we shoot.
    public Bullet Bullet;
    
    //how much time is there between shots.
    public float timeBetweenShotsInMs = 100;
    
    //how fast does each bullet move.
    public float muzzleVelocity = 35;

    //when will we shoot again.
    private float nextShotTime;

    public void Shoot()
    {
        //is it time to shoot?
        if (Time.time >= nextShotTime)
        {
            //setting the next time to shoot.
            nextShotTime = Time.time + timeBetweenShotsInMs / 1000;
            
            //craeting a new bullet.
            var newBullet = Instantiate(Bullet, muzzle.position, muzzle.rotation) as Bullet;
            
            //setting the bullet's speed.
            newBullet.setSpeed(muzzleVelocity);
        }
    }
}

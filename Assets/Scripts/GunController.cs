using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GunController : MonoBehaviour
{
    //where we hold the gun.
    public Transform WeaponHold;
    
    //what gun are we starting with.
    public Gun startingGun;
    
    
    // which gun do we have equiped right now.
    private Gun equipedGun;

    private void Start()
    {
        //were we given a starting gun? 
        if (startingGun != null)
        {
            //equip it!
            equipGun(startingGun);
        }
    }

    public void equipGun(Gun newGun)
    {
        //do we have something equiped?
        if (equipedGun != null)
        {
            //kill it!
            Destroy(equipedGun);
        }
        //create the new gun.
        equipedGun = Instantiate(newGun, WeaponHold.position,WeaponHold.rotation) as Gun;
        equipedGun.transform.parent = WeaponHold;

    }

    public void Shoot()
    {
        //do we have a gun?
        if (equipedGun != null)
        {
            //shoot!
            equipedGun.Shoot();
        }
    }
}

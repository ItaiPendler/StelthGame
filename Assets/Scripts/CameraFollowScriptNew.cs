using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowScriptNew : MonoBehaviour
{
    //what we follow.
    public Transform follow;
    
    void Update()
    {
        //follow the 'follow' object but keep your y level.
        this.transform.position = new Vector3(follow.position.x, this.transform.position.y, follow.transform.position.z);
    }
}

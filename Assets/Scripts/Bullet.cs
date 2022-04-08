using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    //what do we collide with. 
    public LayerMask collisionMask;
    
    //how long is this bullet live.
    public float lifeTime;
    
    //how fast we move.
    private float speed =10;

    private Stopwatch lifeTimer;

    //set the speed to a new value.
    public void setSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    private void Start()
    {
        lifeTimer = new Stopwatch();
        lifeTimer.Start();
    }

    void Update()
    {
        if (lifeTimer.Elapsed.Seconds >= lifeTime)
        {
            Destroy(gameObject);
        }
        //how much are we going to move?
        var moveDistance = Time.deltaTime * speed;
        
        //checking if we hit anything?
        CheckCollision(moveDistance);
        
        //if not, we move.
        transform.Translate(transform.forward * moveDistance, Space.World);
    }

    private void CheckCollision(float moveDistance)
    {
        //creating a ray to check if we will collide with something when we move.
        var ray = new Ray(transform.position, Vector3.forward);
        RaycastHit raycastHit;
        
        //did we hit something in the layers we are given?
        if (Physics.Raycast(ray, out raycastHit, moveDistance, collisionMask, QueryTriggerInteraction.Collide))
        {
            OnHitObject(raycastHit);
        }
    }

    private void OnHitObject(RaycastHit raycastHit)
    {
        //if we hit a guard, we destroy him.
        if (raycastHit.collider.CompareTag("Guard"))
        {
            raycastHit.collider.gameObject.SetActive(false);
        }
        
        //we destroy ourselves if we hit anything. 
        Destroy(gameObject);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GunController))]
public class Player : MonoBehaviour
{
    public static event Action OnReachedWinPlace;

    public float shotRange;
    public float speed;
    public float smoothMoveTime;
    public float turnSpeed;
    public Transform winPlace;
    public GunController GunController;

    private float angle;
    private float smoothInputMagnitude;
    private float smoothMoveVelocity;
    
    private Rigidbody rigidbody;
    private Vector3 velocity;
    
    /// <summary>
    /// Used to get information from firing the ray.
    /// </summary>
    private RaycastHit _raycastHit;
    
    private bool gameOver = false;
    
    private void Start()
    {
        GunController = GetComponent<GunController>();
        rigidbody = GetComponent<Rigidbody>();
        Player.OnReachedWinPlace += EndGame;
        Guard.OnGuardSpottedPlayer += EndGame;
    }

    void Update()
    {
        
        //if the game is over, we dont want to check anything else. 
        if (gameOver) return;

        // if the game is over because we killed everyone, we win the game.
        if (GameManager.gameOver) OnReachedWinPlace?.Invoke();
        
        //Fire System.
        if (Input.GetKey(KeyCode.Mouse0))
        {
            GunController.Shoot();
        }

        //if we made it to the end platform, we want to win.
        if (Vector3.Distance(transform.position,winPlace.position)<5)
        { 
            OnReachedWinPlace?.Invoke();
            return;
        }
        
        //getting the direction of input.
        var inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        var magnitude = inputDirection.magnitude;
        smoothInputMagnitude = Mathf.SmoothDamp(smoothInputMagnitude, magnitude, ref smoothMoveVelocity, smoothMoveTime);

        //calculating the angle of rotation. 
        var targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
        angle = Mathf.LerpAngle(angle, targetAngle, turnSpeed * Time.deltaTime * magnitude);
        velocity = transform.forward * speed * smoothInputMagnitude;
        
    }

    public void EndGame()
    {
        gameOver = true;
        speed = 0;
        velocity = Vector3.zero;
    }

    //Rigidbodies must be updated in fixed update!!! not in update.
    public void FixedUpdate()
    {
        rigidbody.MoveRotation(Quaternion.Euler(Vector3.up * angle));
        rigidbody.MovePosition(rigidbody.position + velocity * Time.deltaTime);

    }

    private void OnDestroy()
    {
        Guard.OnGuardSpottedPlayer -= EndGame;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, Vector3.forward * shotRange);
    }
}

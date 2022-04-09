using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class Guard : MonoBehaviour
{
    public static event Action OnGuardSpottedPlayer;

    public Transform PathHolder;
    public float speed;
    public float TimeToWait;
    public float turnSpeed;
    public bool drawGizmos;
    public Light spotlight;
    public float viewDistance;
    public LayerMask viewMask;
    public static int CurrentGuardCount = 0;
    public Color GuardNumber = Color.black;

    public float TimeToSpotPlayer = 0.5f;
    public float playerVisibleTimer;
    
    private float viewAngle;
    private Transform player;

    private bool disabled = false;
    private Vector3[] _waypoints;
    private Coroutine currentFollowPathCoroutine, currentFollowPlayerCoroutine;
    private bool seen;
    private int _currentWaypointIndex;
    private Vector3 _currentWaypoint;

    public void Start()
    {
        

        Guard.OnGuardSpottedPlayer += Disable;
        Player.OnReachedWinPlace += Disable;
        
        viewAngle = spotlight.spotAngle;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        
        _waypoints = new Vector3[PathHolder.childCount];
        for (int i = 0; i < _waypoints.Length; i++)
        {
            _waypoints[i] = PathHolder.GetChild(i).position;
            _waypoints[i] = new Vector3(_waypoints[i].x, transform.position.y, _waypoints[i].z);
        }

        CurrentGuardCount++;
        
        currentFollowPathCoroutine = StartCoroutine(FollowPath(_waypoints));
        
    }

   
    private void Update()
    {
        //if we can see the player, we update the timer.
        if (canSeePlayer())
        {
            playerVisibleTimer += Time.deltaTime;
            
            //if we haven't already stopped following the path, we stop.
            if (currentFollowPathCoroutine != null)
            {
                StopCoroutine(currentFollowPathCoroutine);
                currentFollowPathCoroutine = null;
            }

            //if we haven't started following the player, we start now.
            if (currentFollowPlayerCoroutine == null)
            {
                currentFollowPlayerCoroutine = StartCoroutine(GetToPlayer());
            }

            seen = true;
            
           // print("we see the player. \n seen is " + seen + ". currentFollowPlayer is " + currentFollowPlayerCoroutine + "\n. currentFollowPath is " + currentFollowPathCoroutine);
        }
        //we don't see the player.
        else
        {
            
            playerVisibleTimer -= Time.deltaTime;
            
            //if we haven't stopped following the player, we stop.
            if (currentFollowPlayerCoroutine != null)
            {
                StopCoroutine(currentFollowPlayerCoroutine);
                currentFollowPlayerCoroutine = null;
            }

            //if we saw the player last frame, and now we don't we enter. 
            if (seen)
            {
                //if we haven't started to follow the path again, we start.
                if (currentFollowPathCoroutine == null)
                {
                    currentFollowPathCoroutine = StartCoroutine(FollowPath(_waypoints));
                }

                //forgetting seeing the player.
                seen = false;
            }
            //print("we dont see the player. \n seen is " + seen + ". currentFollowPlayer is " + currentFollowPlayerCoroutine + "\n. currentFollowPath is " + currentFollowPathCoroutine);
        }

        //setting the color based on the time we saw the player.
        playerVisibleTimer = Mathf.Clamp(playerVisibleTimer, 0, TimeToSpotPlayer);
        
        //setting the color to the right color.
        spotlight.color = Color.Lerp(Color.yellow, Color.red, playerVisibleTimer/TimeToSpotPlayer);

    }

    bool canSeePlayer()
    {
        //are we in line of sight of the player?
        if (Vector3.Distance(transform.position, player.position)< viewDistance)
        {
            var dirToPlayer = (player.position - transform.position).normalized;
            var angleToPlayer = Vector3.Angle(transform.forward, dirToPlayer);
            
            //are we in sight of the player?
            if (angleToPlayer < viewAngle)
            {
                //is the path to the player clear of obstacles?
                if (!Physics.Linecast(transform.position, player.position, viewMask))
                {
                    return true;
                }
            }
        }

        return false;
    }
    
    IEnumerator GetToPlayer()
    {
//        yield return StartCoroutine(TurnToFace(player.transform.position));
        while (true)
        {
            var dirToLook = (player.position - transform.position).normalized;
            //print("we move to the player!");
            transform.LookAt(player);
            this.transform.position = Vector3.MoveTowards(transform.position, player.position, speed * 1.5f * Time.deltaTime);
            yield return null;
        }   
    }
    IEnumerator FollowPath(Vector3 [] waypoints )
    {
        _currentWaypointIndex = _currentWaypointIndex == 0 ? 0 : _currentWaypointIndex;
        _currentWaypoint = waypoints[_currentWaypointIndex];
        transform.LookAt(_currentWaypoint);
        while (true)
        {
            if (!disabled)
            {
                this.transform.position = Vector3.MoveTowards(transform.position, _currentWaypoint, speed * Time.deltaTime);
                if (transform.position == _currentWaypoint)
                {
                    _currentWaypointIndex = (_currentWaypointIndex + 1) % waypoints.Length;
                    _currentWaypoint = waypoints[_currentWaypointIndex];
                    yield return new WaitForSeconds(TimeToWait);
                    yield return StartCoroutine(TurnToFace(_currentWaypoint));
                }
            }
            yield return null; 
        }
    }

    IEnumerator TurnToFace(Vector3 lookTarget)
    {
        var dirToLook = (lookTarget - transform.position).normalized;
        var targetAngle = 90 - Mathf.Atan2(dirToLook.z, dirToLook.x) * Mathf.Rad2Deg;
        while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.05f)
        {
            var angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle,turnSpeed * Time.deltaTime);
            transform.eulerAngles = Vector3.up * angle;
            yield return null;
        }
    }

    void Disable()
    {
        
        if (currentFollowPathCoroutine != null)
        {
            StopCoroutine(currentFollowPathCoroutine);
        }

        if (currentFollowPlayerCoroutine != null)
        {
            StopCoroutine(currentFollowPlayerCoroutine);
        }

        OnGuardSpottedPlayer -= Disable;

        disabled = true;
    }
    
  

    private void OnDestroy()
    {
        CurrentGuardCount--;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            OnGuardSpottedPlayer?.Invoke();
        }
    }

    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            OnGuardSpottedPlayer?.Invoke();
        }
    }
    
    public void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            Color[] colors = {Color.blue, Color.magenta, Color.green, Color.cyan};
            Vector3 startPosition = PathHolder.GetChild(0).position;
            Vector3 previousPositon = startPosition;
//            var counter = 0;
            foreach (Transform waypoint in PathHolder)
            {
                Gizmos.color = this.GuardNumber;
                Gizmos.DrawLine(previousPositon, waypoint.position);
                //Gizmos.DrawSphere(waypoint.position, 0.3f);
                previousPositon = waypoint.position;
            }

            Gizmos.DrawLine(previousPositon, startPosition);

            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, transform.forward * viewDistance);
        }
    }
}

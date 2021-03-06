﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : MonoBehaviour
{
    public Transform pathHolder;
    public float speed = 6f;
    public float waitTime = .3f;
    public float turnSpeed = 90;

    public Light spotlight;
    public float viewDistance;
    public LayerMask viewMask;
    float viewAngle;
    Transform player;
    Color originalSpotlightColor;
    public float timeToSpotPlayer = .5f;
    float playerVisibleTimer;
    public static event System.Action OnGuardHasSpottedPlayer;


    void Start(){
        player= GameObject.FindGameObjectWithTag("Player").transform;
        viewAngle = spotlight.spotAngle;
        originalSpotlightColor = spotlight.color;
        Vector3[] waypoints = new Vector3[pathHolder.childCount];
        for(int i = 0;i<waypoints.Length;i++){
            waypoints[i] = pathHolder.GetChild(i).position;
            waypoints[i] = new Vector3(waypoints[i].x, transform.position.y, waypoints[i].z);
        }

        StartCoroutine(FollowPath(waypoints));
    }

    bool CanSeePlayer(){
        if(Vector3.Distance(transform.position, player.position)<viewDistance){
            Vector3 dirToPlayer = (player.position-transform.position).normalized;
            float angleBetweenGuardAndPlayer = Vector3.Angle(transform.forward, dirToPlayer);
            if(angleBetweenGuardAndPlayer < viewDistance/2f){
                if(!Physics.Linecast(transform.position,player.position,viewMask)){
                    return true;
                }
            }
        }
        return false;
    }

    void Update(){
        if(CanSeePlayer()){
            spotlight.color = Color.red;
            playerVisibleTimer +=Time.deltaTime;
        }
        else{
            spotlight.color = originalSpotlightColor;
            playerVisibleTimer -= Time.deltaTime;
        }
        playerVisibleTimer = Mathf.Clamp(playerVisibleTimer,0,timeToSpotPlayer);
        spotlight.color = Color.Lerp(originalSpotlightColor,Color.red, playerVisibleTimer/ timeToSpotPlayer);
        if(playerVisibleTimer>=timeToSpotPlayer){
            if(OnGuardHasSpottedPlayer!=null){
                OnGuardHasSpottedPlayer();
            }
        }
    }

    IEnumerator FollowPath(Vector3[] waypoints){
        
        transform.position = waypoints[0];
        int wayPointIndex = 1;
        Vector3 targetWaypoint = waypoints[wayPointIndex];
        transform.LookAt(targetWaypoint);

        while(true){
            transform.position = Vector3.MoveTowards(transform.position, targetWaypoint, speed * Time.deltaTime);
            if(transform.position == targetWaypoint){
                wayPointIndex = (wayPointIndex+1)%waypoints.Length;
                targetWaypoint = waypoints[wayPointIndex];
                yield return new WaitForSeconds(waitTime);
                yield return StartCoroutine(TurnToFace(targetWaypoint));
            }
            yield return null;
        }
    }

    IEnumerator TurnToFace(Vector3 lookTarget){
        Vector3 dirToLookTarget = (lookTarget-transform.position).normalized;
        float targetAngle = 90-Mathf.Atan2(dirToLookTarget.z,dirToLookTarget.x)*Mathf.Rad2Deg;;

        while(Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y,targetAngle))>0.05f){
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y,targetAngle, turnSpeed * Time.deltaTime);
            transform.eulerAngles = Vector3.up * angle;
            yield return null;
        }
    }

    void OnDrawGizmos() {
        Vector3 startPosition = pathHolder.GetChild(0).position;
        Vector3 previousPosition = startPosition;
        foreach(Transform waypoint in pathHolder){
            Gizmos.DrawSphere(waypoint.position,.3f);
            Gizmos.DrawLine(previousPosition,waypoint.position);
            previousPosition = waypoint.position;
        } 
        Gizmos.DrawLine(previousPosition,startPosition);   

        Gizmos.color = Color.red;
        Gizmos.DrawRay (transform.position, transform.forward*viewDistance);
    }
}

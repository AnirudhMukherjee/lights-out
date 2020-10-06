﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour
{
    public float moveSpeed = 7;
    public float smoothMoveTime = 0.1f;
    float smoothInputMagnitude;
    float smoothMoveVelocity;

    public float turnSpeed = 8;
    float angle;
    public event System.Action onEndLevel;
    Vector3 velocity;

    Rigidbody rigidbody;
    bool disabled;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        Guard.OnGuardHasSpottedPlayer += Disabled;
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 inputDirection = Vector3.zero;
        if(!disabled){
            inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"),0,Input.GetAxisRaw("Vertical")).normalized;
        }
        
        float inputMagnitude = inputDirection.magnitude;
        smoothInputMagnitude = Mathf.SmoothDamp(smoothInputMagnitude,inputMagnitude, ref smoothMoveVelocity, smoothMoveTime);
        
        float targetAngle = Mathf.Atan2(inputDirection.x,inputDirection.z)* Mathf.Rad2Deg;
        angle = Mathf.LerpAngle(angle, targetAngle, Time.deltaTime * turnSpeed * inputMagnitude);
        velocity = transform.forward * moveSpeed * smoothInputMagnitude;
    }

    void FixedUpdate() {
        rigidbody.MoveRotation(Quaternion.Euler(Vector3.up * angle));
        rigidbody.MovePosition(rigidbody.position + velocity * Time.deltaTime);
    }

    void Disabled(){
        disabled = true;
    }

    void OnDestroy(){
        Guard.OnGuardHasSpottedPlayer -=Disabled;
    }

    void OnTriggerEnter(Collider hitCollider){
        if(hitCollider.tag == "Finish"){
            Disabled();
            if(onEndLevel!=null){
                onEndLevel();
            }
        }
    }
}

﻿using UnityEngine;
using System.Collections;

public class Enemy : Character {

    private IEnemyState currentState;
    public GameObject Target { get; set; }

	// Use this for initialization
	public override void Start () {
        base.Start();
        
        ChangeState(new IdleState());
	}
	
	// Update is called once per frame
	void Update () {
        currentState.Execute();
        LookAtTarget();
	}

    public void ChangeState(IEnemyState newState) {
        if (currentState != null) {
            currentState.Exit();
        }

        currentState = newState;
        currentState.Enter(this);
    }

    public void Move() {
        mAnimator.SetFloat("speed", 1);

        transform.Translate(GetDirection() * movementSpeed * Time.deltaTime);
    }

    private void LookAtTarget() {
        if (Target != null) {
            float xDir = Target.transform.position.x - transform.position.x;
            if (xDir < 0 && facingRight || xDir > 0 && !facingRight) {
                ChangeDirection();
            }
        }
    }

    public Vector2 GetDirection() {
        return facingRight ? Vector2.right : Vector2.left;
    }

    void OnTriggerEnter2D(Collider2D other) {
        currentState.OnTriggerEnter(other);
    }
}

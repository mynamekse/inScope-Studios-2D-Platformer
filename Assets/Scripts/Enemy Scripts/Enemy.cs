﻿using UnityEngine;
using System.Collections;

public class Enemy : Character {

    private IEnemyState currentState;
    public GameObject Target { get; set; }

    [SerializeField]
    private float meleeRange;
    [SerializeField]
    private float throwRange;

    private bool dumb;

    public bool InMeleeRange {
        get {
            if (Target != null) {
                return Vector2.Distance(transform.position, Target.transform.position) <= meleeRange;
            }

            return false;
        }
    }

    public bool InThrowRange {
        get {
            if (Target != null) {
                return Vector2.Distance(transform.position, Target.transform.position) <= throwRange;
            }

            return false;
        }
    }

	// Use this for initialization
	public override void Start () {
        base.Start();
        ChangeDirection();
        Player.Instance.Dead += new DeadEventHandler(RemoveTarget);
        ChangeState(new IdleState());
	}
	
	// Update is called once per frame
	void Update () {
        if (!IsDead) {
            if (!TakingDamage) {
                currentState.Execute();
            }
            LookAtTarget();
        }

        //////////////////////////////
        // DUMBEST SHIT EVER UNITY PLS
        //////////////////////////////
        if (dumb) {
            transform.Translate(new Vector3(0.0001f, 0, 0));
        }
        else {
            transform.Translate(new Vector3(-0.0001f, 0, 0));
        }
        dumb = !dumb;
	}

    public void ChangeState(IEnemyState newState) {
        if (currentState != null) {
            currentState.Exit();
        }

        currentState = newState;
        currentState.Enter(this);
    }

    public void Move() {
        if (!Attack) {
            mAnimator.SetFloat("speed", 1);
            transform.Translate(GetDirection() * movementSpeed * Time.deltaTime);
        }

    }

    private void LookAtTarget() {
        if (Target != null) {
            float xDir = Target.transform.position.x - transform.position.x;
            if (xDir < 0 && facingRight || xDir > 0 && !facingRight) {
                ChangeDirection();
            }
        }
    }

    public void RemoveTarget() {
        Target = null;
        ChangeState(new PatrolState());
    }

    public Vector2 GetDirection() {
        return facingRight ? Vector2.right : Vector2.left;
    }

    public override void OnTriggerEnter2D(Collider2D other) {
        base.OnTriggerEnter2D(other);
        currentState.OnTriggerEnter(other);
    }


    public override IEnumerator TakeDamage() {
        health -= 10;
        if (!IsDead) {
            mAnimator.SetTrigger("damage");
        }
        else {
            mAnimator.SetTrigger("death");
        }
        yield return null;
    }

    public override bool IsDead {
        get {
            return health <= 0;
        }
    }
}

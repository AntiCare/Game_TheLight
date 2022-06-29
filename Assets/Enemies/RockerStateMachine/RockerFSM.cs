using System;
using System.Collections.Generic;
using Enums;
using Quests.Objectives.Events;
using UnityEngine;

//By YangCheng 07/10/2021
public enum RockerStateType
{
    Idle,
    Patrol,
    Chase,
    React,
    Attack,
    BeAttacked,
    Death
}

//some values of enemy
[Serializable]
public class RockerParameter
{
    public  int         health;
    public  float       moveSpeed;
    public  float       chaseSpeed;
    public  float       idleTime;
    public  Transform[] patrolPoints;
    public  Transform[] chasePoints;
    public  Transform   target;
    public  bool        beAttacked;
    public  Animator    animator;
    public  AudioSource _audioSource;
    public  AudioClip   hurtSoundClip;
    public  AudioClip   deadSoundClip;
    private DropLoot    _dropLoot;

    //Enemy Attack: Use the circular range of physics2D.OverlapCircle to detect the collision body of the Player layer,
    //and use OnDrawGizmos to draw the attack range
    public LayerMask targetLayer;

    public Transform attackPoint;

    //attack range
    public float attackArea;

    //drop loot
    public GameObject[] loots;

    //shoot Attack
    public Transform player;

    public GameObject tar;

    //shoot frequency
    public float timeBtwShots;

    public float startTimeBtwShoots;

    //shoot item
    public GameObject projectile;

    public DropLoot dropLoot
    {
        get => _dropLoot;
        set => _dropLoot = value;
    }
}

[Serializable]
public class RockerFSM : Entity
{
    private State currentState;

    //Register status using dictionary.
    private Dictionary<RockerStateType, State> states = new Dictionary<RockerStateType, State>();

    public RockerParameter parameter = new RockerParameter();
    // Start is called before the first frame update

    public Enemy enemy;

    void Start()
    {
        //Add key-value pairs
        states.Add(RockerStateType.Idle, new RockerIdleState(this));
        states.Add(RockerStateType.Patrol, new RockerPatrolState(this));
        states.Add(RockerStateType.React, new RockerReactState(this));
        states.Add(RockerStateType.Chase, new RockerChaseState(this));
        states.Add(RockerStateType.Attack, new RockerAttackState(this));
        states.Add(RockerStateType.BeAttacked, new RockerBeAttackedState(this));
        states.Add(RockerStateType.Death, new RockerDeathState(this));

        //Set the initial state = Idle
        TransitionState(RockerStateType.Idle);

        //Get the animator component 
        parameter.animator     = GetComponent<Animator>();
        parameter._audioSource = GetComponent<AudioSource>();
        parameter.dropLoot     = GetComponent<DropLoot>();

        foreach (Enemy dataEnemy in GameManager.Instance.GameData.enemies)
        {
            if (dataEnemy.id == this.enemy.id)
            {
                Destroy(gameObject);
            }
        }
        // GameManager.Instance.GameData.enemies.Add(enemy);
    }

    // Update is called once per frame
    void Update()
    {
        currentState.OnUpdate();

        if (GameEnding.plantFlower)
        {
            Destroy(gameObject);
        }
        //For test beAttacked state, Press "K" to let the enemy be attacked.
        //if (Input.GetKeyDown(KeyCode.K))
        //{
        //   parameter.beAttacked = true;
        // }
    }

    /**
     * This method is used to switch the state.
     */
    public void TransitionState(RockerStateType type)
    {
        //Execute the OnExit function of the previous state.
        if (currentState != null)
            currentState.OnExit();
        //Switch the current state to a given state
        currentState = states[type];
        //start OnEnter function of the new state
        currentState.OnEnter();
    }

    //Change the enemy's direction.
    public void FlipTo(Transform target)
    {
        if (target != null)
        {
            if (transform.position.x > target.position.x)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else if (transform.position.x < target.position.x)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.collider.CompareTag("Arrow"))
        {
            parameter.beAttacked = true;
            PlayHurtSound();
            //animation to destroy arrow
            other.gameObject.SetActive(false);
        }
    }

    void PlayHurtSound()
    {
        parameter._audioSource.clip = parameter.hurtSoundClip;
        parameter._audioSource.Play();
    }

    //When the player enters the trigger range, let the enemy find the player.
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            parameter.target = other.transform;
        }
    }

    //dead
    public void dead()
    {
        GameManager.Instance.GameData.enemies.Add(enemy);
        KillEnemyEvent.Kill(EnemyType.ROCKER);
        transform.GetComponent<BoxCollider2D>().enabled = false;
    }

    public void shoot()
    {
        if (parameter.timeBtwShots <= 0)
        {
            Instantiate(parameter.projectile, transform.position, Quaternion.identity);
            parameter.timeBtwShots = parameter.startTimeBtwShoots;
        }
        else
        {
            parameter.timeBtwShots -= Time.deltaTime;
        }
    }

    //When the player moves away from the trigger area, the enemy loses the player position.
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            parameter.target = null;
        }
    }

    //Draw the attack range.
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(parameter.attackPoint.position, parameter.attackArea);
    }
    
    public override Entity Interact()
    {
        throw new NotImplementedException();
    }

    public override Entity StartInteraction(bool enable)
    {
        throw new NotImplementedException();
    }

    public override ScriptableObject GetScriptableObject()
    {
        throw new NotImplementedException();
    }

    public override void SpecialInteraction(bool enable)
    {
        throw new NotImplementedException();
    }
}
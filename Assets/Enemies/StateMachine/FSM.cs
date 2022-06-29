using System;
using System.Collections.Generic;
using Enums;
using Quests.Objectives.Events;
using UnityEngine;

//By YangCheng 07/10/2021
public enum StateType
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
public class Parameter
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

    public DropLoot dropLoot
    {
        get => _dropLoot;
        set => _dropLoot = value;
    }

    //Enemy Attack: Use the circular range of physics2D.OverlapCircle to detect the collision body of the Player layer,
    //and use OnDrawGizmos to draw the attack range
    public LayerMask targetLayer;

    public Transform attackPoint;

    //attack range
    public float attackArea;

    //Attack
    public GameObject attack;
}

[Serializable]
public class FSM : Entity
{
    private State currentState;

    //Register status using dictionary.
    private Dictionary<StateType, State> states = new Dictionary<StateType, State>();

    public Parameter parameter = new Parameter();

    public Enemy enemy;
    
    // Start is called before the first frame update
    void Start()
    {
        //Add key-value pairs
        states.Add(StateType.Idle, new IdleState(this));
        states.Add(StateType.Patrol, new PatrolState(this));
        states.Add(StateType.React, new ReactState(this));
        states.Add(StateType.Chase, new ChaseState(this));
        states.Add(StateType.Attack, new AttackState(this));
        states.Add(StateType.BeAttacked, new BeAttackedState(this));
        states.Add(StateType.Death, new DeathState(this));

        //Set the initial state = Idle
        TransitionState(StateType.Idle);

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
    }

    // Update is called once per frame
    void Update()
    {
        //execute the current state OnUPdate function.
        currentState.OnUpdate();
        
        if (GameEnding.plantFlower)
        {
            Destroy(gameObject);
        }
    }

    /**
     * This method is used to switch the state.
     */
    public void TransitionState(StateType type)
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

    //When the player enters the trigger range, let the enemy find the player.
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            parameter.target = other.transform;
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.collider.CompareTag("Arrow"))
        {
            parameter.beAttacked = true;
            PlayHurtSound();
            //animation to destroy arrow
            other.collider.GetComponent<Arrow>().gameObject.SetActive(false);
        }
    }

    void PlayHurtSound()
    {
        parameter._audioSource.clip = parameter.hurtSoundClip;
        parameter._audioSource.Play();
    }

    public override Entity Interact()
    {
        // UIManager.Instance.ToggleDialogBox(true);
        return this;
    }

    public override Entity StartInteraction(bool enable)
    {
        return this;
    }

    public override ScriptableObject GetScriptableObject()
    {
        return null;
    }

    public override void SpecialInteraction(bool enable)
    {
        throw new NotImplementedException();
    }

    //When the player moves away from the trigger area, the enemy loses the player position.
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            parameter.target = null;
        }
    }

    public void dead()
    {
        GameManager.Instance.GameData.enemies.Add(enemy);
        KillEnemyEvent.Kill(EnemyType.ROCKY);
        transform.GetComponent<CircleCollider2D>().enabled = false;
    }

    //dead

    //Draw the attack range.
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(parameter.attackPoint.position, parameter.attackArea);
    }
}
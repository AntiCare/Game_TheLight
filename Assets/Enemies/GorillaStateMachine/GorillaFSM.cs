using System;
using System.Collections.Generic;
using UnityEngine;

//By YangCheng 14/10/2021
public enum GorillaStateType
{
    Idle,
    Patrol,
    Chase,
    React,
    TransitionState,
    ShootAttack,
    Dash,
    Attack,
    BeAttacked,
    Death
}

//some values of enemy
[Serializable]
public class GorillaParameter
{
    public int         health;
    public int         transitionStateHealth;
    public float       moveSpeed;
    public float       chaseSpeed;
    public float       idleTime;
    public Transform[] patrolPoints;
    public Transform[] chasePoints;
    public Transform   target;
    public bool        beAttacked;
    public Animator    animator;

    //Enemy Attack: Use the circular range of physics2D.OverlapCircle to detect the collision body of the Player layer,
    //and use OnDrawGizmos to draw the attack range
    public LayerMask targetLayer;
    public Transform attackPoint;

    public Transform ShootAttackPoint;

    //attack range
    public float attackArea;

    //shoot Attack
    //target
    public Transform player;

    public GameObject tar;

    //shoot frequency
    public float timeBtwShots;

    public float startTimeBtwShoots;

    //shoot item
    public GameObject projectile;

    //shoot range
    public float ShootArea;

    //drop loot
    public GameObject[] loots;

    //AttackState1
    public GameObject MeleeAttackS1;

    //AttackState2
    public GameObject MeleeAttackS2;

    //Dash
    public float speed;
    public int   dashCounter = 3;

    //dialog
    public bool fight;

    //collider 2D
    public BoxCollider2D collider;
}

[Serializable]
public class GorillaFSM : Entity
{
    private State currentState;

    //Register status using dictionary.
    private Dictionary<GorillaStateType, State> states = new Dictionary<GorillaStateType, State>();

    public GorillaParameter parameter = new GorillaParameter();

    //Dash target
    private Transform player;

    private Vector2 target;

    //Interact
    public GameObject interact;
    public GameObject fight;

    public Character character;
    public Character character2;
    public Enemy     enemy;

    public static bool startTerranisCG = false;

    public static bool getPlantTask = false;

    // Start is called before the first frame update
    void Start()
    {
        //Add key-value pairs
        states.Add(GorillaStateType.Idle, new GorillaIdleState(this));
        states.Add(GorillaStateType.Patrol, new GorillaPatrolState(this));
        states.Add(GorillaStateType.React, new GorillaReactState(this));
        states.Add(GorillaStateType.Chase, new GorillaChaseState(this));
        states.Add(GorillaStateType.ShootAttack, new GorillaShootAttackState(this));
        states.Add(GorillaStateType.Dash, new GorillaDashState(this));
        states.Add(GorillaStateType.Attack, new GorillaAttackState(this));
        states.Add(GorillaStateType.BeAttacked, new GorillaBeAttackedState(this));
        states.Add(GorillaStateType.TransitionState, new GorillaTransitionState(this));
        states.Add(GorillaStateType.Death, new GorillaDeathState(this));

        //Set the initial state = Idle
        TransitionState(GorillaStateType.Idle);

        //Get the animator component 
        parameter.animator = GetComponent<Animator>();

        //get collider
        parameter.collider = GetComponent<BoxCollider2D>();

        //Dash target
        player            = GameObject.FindGameObjectWithTag("Player").transform;
        target            = player.position;
        ChangeColor.color = false;
        foreach (Enemy dataEnemy in GameManager.Instance.GameData.enemies)
        {
            if (dataEnemy.id == this.enemy.id)
            {
                TransitionState(GorillaStateType.Death);
                GameEnding.gorillaDead = true;
                fight.SetActive(false);
                Destroy(gameObject);
            }
        }

        /*Terranis quest id is 10*/
        if (GameManager.Instance.GameData.activeQuests.Find(q => q.QuestId == 10))
        {
            getPlantTask           = true;
        }

        if (GameManager.Instance.GameData.activeQuests.Find(q => q.QuestId == 10) != null && GameManager.Instance.GameData.Interactable.Contains(GameManager.Instance.GameData.Interactable.Find(i => i.id == 1)))  
        {
            GameEnding.plantFlower = true;
        }

        if (GameEnding.TerranisActive && GameManager.Instance.GameData.completedQuests.Find(q => q.QuestId == 10) &&
            GameEnding.EarthEndingFinish)
        {
            // GameEnding.gorillaDead = true;
            fight.SetActive(false);
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //execute the current state OnUpdate function.
        currentState.OnUpdate();
        //For test beAttacked state, Press "K" to let the enemy be attacked.
        if (Input.GetKeyDown(KeyCode.K))
        {
            parameter.beAttacked = true;
        }

        if (ChangeColor.color)
        {
            Destroy(GameObject.FindWithTag("GorillaDamage"));
        }
    }

    void Fight()
    {
        parameter.fight = true;
    }

    /**
     * This method is used to switch the state.
     */
    public void TransitionState(GorillaStateType type)
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

    public void State1MeleeAttack(Transform target)
    {
        Vector3 pos = new Vector3();
        if (transform.position.x > target.position.x)
        {
            pos = new Vector3(transform.position.x - 20, transform.position.y, transform.position.z);
        }
        else if (transform.position.x < target.position.x)
        {
            pos = new Vector3(transform.position.x + 20, transform.position.y, transform.position.z);
        }

        Instantiate(parameter.MeleeAttackS1, pos, transform.rotation);
    }


    public void State2MeleeAttack(Transform target)
    {
        Vector3 pos = new Vector3();
        if (transform.position.x > target.position.x)
        {
            pos = new Vector3(transform.position.x - 20, transform.position.y, transform.position.z);
        }
        else if (transform.position.x < target.position.x)
        {
            pos = new Vector3(transform.position.x + 20, transform.position.y, transform.position.z);
        }

        Instantiate(parameter.MeleeAttackS2, pos, transform.rotation);
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
            //animation to destroy arrow
            other.collider.GetComponent<Arrow>().gameObject.SetActive(false);
        }
    }

    //When the player moves away from the trigger area, the enemy loses the player position.
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            parameter.target = null;
            interact.SetActive(false);
            UIManager.Instance.ToggleDialogBox(false, null);
        }
    }

    //Draw the attack range.
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(parameter.attackPoint.position, parameter.attackArea);
        Gizmos.DrawWireSphere(parameter.ShootAttackPoint.position, parameter.ShootArea);
    }

    //drop loot
    public void GenLoots()
    {
        GetComponent<DropLoot>().GenLoots();
    }

    //Dash
    public void Dash()
    {
        transform.position    = Vector2.MoveTowards(transform.position, target, parameter.speed * Time.deltaTime);
        parameter.dashCounter = 3;
    }

    public override Entity Interact()
    {
        if (currentState == states[GorillaStateType.Death])
        {
            return null;
        }

        if (GameManager.Instance.GameData.activeQuests.Find(q => q.QuestId == 10))
        {
            Debug.Log($"restricted dialog");

            UIManager.Instance.ToggleDialogBox(true, character.Dialog[10]);
            
            return this;
        }

        if (!fight.activeSelf)
        {
            Debug.Log($"open dialog");
            UIManager.Instance.ToggleDialogBox(true,
                GameEnding.plantFlower ? character2.Dialog[0] : character.Dialog[0]);
        }

        return this;
    }

    public override Entity StartInteraction(bool enable)
    {
        if (currentState == states[GorillaStateType.Death])
        {
            return null;
        }

        if (!fight.activeSelf)
        {
            interact.SetActive(enable);
        }

        return this;
    }

    public override ScriptableObject GetScriptableObject()
    {
        return character;
    }

    public override void SpecialInteraction(bool enable)
    {
        if (GameEnding.plantFlower)
            //good ending
        {
            startTerranisCG = true;
            Destroy(gameObject);
        }
        //plant flower
        else if (UIManager.Instance.DialogManager.currentDialogNUM == 11)
        {
            getPlantTask = true;
        }
        //fight-bad ending
        else
        {
            fight.SetActive(true);
            interact.SetActive(false);
        }
    }
}
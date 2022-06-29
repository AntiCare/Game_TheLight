using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//By YangCheng 07/10/2021

/**
 * Class for IdleState.
 */
public class RockerIdleState : State
{
    private RockerFSM       manager;
    private RockerParameter parameter;

    //patrols need to stay at each point for a period of time, create a timer to control the stay time.
    private float timer;

    public RockerIdleState(RockerFSM manager)
    {
        this.manager   = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter()
    {
        parameter.animator.Play("RockerIdle");
    }

    public void OnUpdate()
    {
        timer += Time.deltaTime;

        //If attacked, switch to the attacked state
        if (parameter.beAttacked)
        {
            manager.TransitionState(RockerStateType.BeAttacked);
        }

        //Check if the player is found & the player is in the chasing range.
        if (parameter.target            != null                                &&
            parameter.target.position.x >= parameter.chasePoints[0].position.x &&
            parameter.target.position.x <= parameter.chasePoints[1].position.x &&
            parameter.target.position.y <= parameter.chasePoints[2].position.y &&
            parameter.target.position.y >= parameter.chasePoints[3].position.y)
        {
            manager.TransitionState(RockerStateType.React);
        }

        //When staying at the patrol point for a while, switch to patrol state.
        if (timer >= parameter.idleTime)
        {
            manager.TransitionState(RockerStateType.Patrol);
        }
    }

    public void OnExit()
    {
        //reset timer
        timer = 0;
    }
}


/**
 * Class for PatrolState.
 */
public class RockerPatrolState : State
{
    private RockerFSM       manager;
    private RockerParameter parameter;

    //Used to find and switch patrol points
    private int patrolPosition;

    public RockerPatrolState(RockerFSM manager)
    {
        this.manager   = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter()
    {
        parameter.animator.Play("RockerRun");
    }

    public void OnUpdate()
    {
        manager.FlipTo(parameter.patrolPoints[patrolPosition]);

        manager.transform.position = Vector2.MoveTowards(manager.transform.position,
            parameter.patrolPoints[patrolPosition].position, parameter.moveSpeed * Time.deltaTime);

        if (parameter.beAttacked)
        {
            manager.TransitionState(RockerStateType.BeAttacked);
        }

        if (parameter.target            != null                                &&
            parameter.target.position.x >= parameter.chasePoints[0].position.x &&
            parameter.target.position.x <= parameter.chasePoints[1].position.x &&
            parameter.target.position.y <= parameter.chasePoints[2].position.y &&
            parameter.target.position.y >= parameter.chasePoints[3].position.y)
        {
            manager.TransitionState(RockerStateType.React);
        }

        //When reaching the patrol point position, switch to Idle state.
        if (Vector2.Distance(manager.transform.position, parameter.patrolPoints[patrolPosition].position) < .1f)
        {
            manager.TransitionState(RockerStateType.Idle);
        }
    }

    public void OnExit()
    {
        patrolPosition++;

        //If the patrol point exceeds the range of the array, reset to 0.
        if (patrolPosition >= parameter.patrolPoints.Length)
        {
            patrolPosition = 0;
        }
    }
}


/**
 * Class for ReactState.
 */
public class RockerReactState : State
{
    private RockerFSM       manager;
    private RockerParameter parameter;

    //Get the progress of attack animation.
    private AnimatorStateInfo animationInfo;

    public RockerReactState(RockerFSM manager)
    {
        this.manager   = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter()
    {
        parameter.animator.Play("RockerReact");
    }

    public void OnUpdate()
    {
        //Get animation state.
        animationInfo = parameter.animator.GetCurrentAnimatorStateInfo(0);

        if (parameter.beAttacked)
        {
            manager.TransitionState(RockerStateType.BeAttacked);
        }

        // When the animation progress = 1, means the attack animation already finish then switch to the chase state.
        if (animationInfo.normalizedTime >= .99f)
        {
            manager.TransitionState(RockerStateType.Chase);
        }
    }

    public void OnExit()
    {
    }
}

/**
 * Class for ChaseState.
 */
public class RockerChaseState : State
{
    private RockerFSM       manager;
    private RockerParameter parameter;

    public RockerChaseState(RockerFSM manager)
    {
        this.manager   = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter()
    {
        parameter.animator.Play("RockerRun");
    }

    public void OnUpdate()
    {
        //face to player.
        manager.FlipTo(parameter.target);
        if (parameter.target)
            manager.transform.position = Vector2.MoveTowards(manager.transform.position,
                parameter.target.position, parameter.chaseSpeed * Time.deltaTime);

        if (parameter.beAttacked)
        {
            manager.TransitionState(RockerStateType.BeAttacked);
        }

        //When the player leaves, switch to idle state.
        if (parameter.target             == null                               ||
            manager.transform.position.x < parameter.chasePoints[0].position.x ||
            manager.transform.position.x > parameter.chasePoints[1].position.x ||
            manager.transform.position.y > parameter.chasePoints[2].position.y ||
            manager.transform.position.y < parameter.chasePoints[3].position.y)
        {
            manager.TransitionState(RockerStateType.Idle);
        }

        //Detect the attack range, if there is a player, switch to the attack state.
        if (Physics2D.OverlapCircle(parameter.attackPoint.position, parameter.attackArea, parameter.targetLayer))
        {
            manager.TransitionState(RockerStateType.Attack);
        }
    }

    public void OnExit()
    {
    }
}

/**
 * Class for AttackState.
 */
public class RockerAttackState : State
{
    private RockerFSM manager;

    private RockerParameter parameter;

    //Get the progress of attack animation.
    private AnimatorStateInfo animationInfo;

    public RockerAttackState(RockerFSM manager)
    {
        this.manager   = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter()
    {
        parameter.animator.Play("RockerAttack");
    }

    public void OnUpdate()
    {
        //Get animation state.
        animationInfo = parameter.animator.GetCurrentAnimatorStateInfo(0);

        if (parameter.beAttacked)
        {
            manager.TransitionState(RockerStateType.BeAttacked);
        }

        manager.shoot();
        // When the animation progress = 1, means the attack animation already finish then switch to the chase state.
        if (animationInfo.normalizedTime >= .99f)
        {
            manager.TransitionState(RockerStateType.Chase);
        }
    }

    public void OnExit()
    {
    }
}

/**
 * Class for BeAttackedState.
 */
public class RockerBeAttackedState : State
{
    private AnimatorStateInfo animationInfo;
    private RockerFSM         manager;
    private RockerParameter   parameter;

    public RockerBeAttackedState(RockerFSM manager)
    {
        this.manager   = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter()
    {
        parameter.animator.Play("RockerBeAttacked");
        parameter.health--;
    }

    public void OnUpdate()
    {
        animationInfo = parameter.animator.GetCurrentAnimatorStateInfo(0);
        //When health == 0, switch to dead state.
        if (parameter.health <= 0)
        {
            manager.TransitionState(RockerStateType.Death);
        }

        if (animationInfo.normalizedTime >= .99f)
        {
            parameter.target = GameObject.FindWithTag("Player").transform;

            manager.TransitionState(RockerStateType.Chase);
        }
    }

    public void OnExit()
    {
        parameter.beAttacked = false;
    }
}

/**
 * Class for DeathState.
 */
public class RockerDeathState : State
{
    private RockerFSM       manager;
    private RockerParameter parameter;

    public RockerDeathState(RockerFSM manager)
    {
        this.manager   = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter()
    {
        EnemyCounter.rockerCounter                 += 1;
        GameManager.Instance.GameData.rockerCounter =  EnemyCounter.rockerCounter;

        manager.dead();
        GameManager.Instance.Player.Upgrade(0.2f);
        parameter.animator.Play("RockerDie");
    
        parameter.dropLoot.GenLoots();
    }

    public void OnUpdate()
    {
    }

    public void OnExit()
    {
    }
}
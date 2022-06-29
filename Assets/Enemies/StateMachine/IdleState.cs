using System.Collections;
using System.Collections.Generic;
using StateMachine.State;
using UnityEngine;

//By YangCheng 07/10/2021

/**
 * Class for IdleState.
 */
public class IdleState : State
{
    private FSM       manager;
    private Parameter parameter;

    //patrols need to stay at each point for a period of time, create a timer to control the stay time.
    private float timer;

    public IdleState(FSM manager)
    {
        this.manager   = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter()
    {
        parameter.animator.Play("SilmeIdle");
    }

    public void OnUpdate()
    {
        timer += Time.deltaTime;

        //If attacked, switch to the attacked state
        if (parameter.beAttacked)
        {
            manager.TransitionState(StateType.BeAttacked);
        }

        //Check if the player is found & the player is in the chasing range.
        if (parameter.target            != null                                &&
            parameter.target.position.x >= parameter.chasePoints[0].position.x &&
            parameter.target.position.x <= parameter.chasePoints[1].position.x &&
            parameter.target.position.y <= parameter.chasePoints[2].position.y &&
            parameter.target.position.y >= parameter.chasePoints[3].position.y)
        {
            manager.TransitionState(StateType.React);
        }

        //When staying at the patrol point for a while, switch to patrol state.
        if (timer >= parameter.idleTime)
        {
            manager.TransitionState(StateType.Patrol);
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
public class PatrolState : State
{
    private FSM       manager;
    private Parameter parameter;

    //Used to find and switch patrol points
    private int patrolPosition;

    public PatrolState(FSM manager)
    {
        this.manager   = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter()
    {
        parameter.animator.Play("SlimeRun");
    }

    public void OnUpdate()
    {
        manager.FlipTo(parameter.patrolPoints[patrolPosition]);

        manager.transform.position = Vector2.MoveTowards(manager.transform.position,
            parameter.patrolPoints[patrolPosition].position, parameter.moveSpeed * Time.deltaTime);

        if (parameter.beAttacked)
        {
            manager.TransitionState(StateType.BeAttacked);
        }

        if (parameter.target            != null                                &&
            parameter.target.position.x >= parameter.chasePoints[0].position.x &&
            parameter.target.position.x <= parameter.chasePoints[1].position.x &&
            parameter.target.position.y <= parameter.chasePoints[2].position.y &&
            parameter.target.position.y >= parameter.chasePoints[3].position.y)
        {
            manager.TransitionState(StateType.React);
        }

        //When reaching the patrol point position, switch to Idle state.
        if (Vector2.Distance(manager.transform.position, parameter.patrolPoints[patrolPosition].position) < .1f)
        {
            manager.TransitionState(StateType.Idle);
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
public class ReactState : State
{
    private FSM       manager;
    private Parameter parameter;

    //Get the progress of attack animation.
    private AnimatorStateInfo animationInfo;

    public ReactState(FSM manager)
    {
        this.manager   = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter()
    {
        parameter.animator.Play("SlimeReact");
    }

    public void OnUpdate()
    {
        //Get animation state.
        animationInfo = parameter.animator.GetCurrentAnimatorStateInfo(0);

        if (parameter.beAttacked)
        {
            manager.TransitionState(StateType.BeAttacked);
        }

        // When the animation progress = 1, means the attack animation already finish then switch to the chase state.
        if (animationInfo.normalizedTime >= .99f)
        {
            manager.TransitionState(StateType.Chase);
        }
    }

    public void OnExit()
    {
    }
}

/**
 * Class for ChaseState.
 */
public class ChaseState : State
{
    private FSM       manager;
    private Parameter parameter;

    public ChaseState(FSM manager)
    {
        this.manager   = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter()
    {
        parameter.animator.Play("SlimeRun");
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
            manager.TransitionState(StateType.BeAttacked);
        }

        //When the player leaves, switch to idle state.
        if (parameter.target             == null                               ||
            manager.transform.position.x < parameter.chasePoints[0].position.x ||
            manager.transform.position.x > parameter.chasePoints[1].position.x ||
            manager.transform.position.y > parameter.chasePoints[2].position.y ||
            manager.transform.position.y < parameter.chasePoints[3].position.y)
        {
            manager.TransitionState(StateType.Idle);
        }

        //Detect the attack range, if there is a player, switch to the attack state.
        if (Physics2D.OverlapCircle(parameter.attackPoint.position, parameter.attackArea, parameter.targetLayer))
        {
            manager.TransitionState(StateType.Attack);
        }
    }

    public void OnExit()
    {
    }
}

/**
 * Class for AttackState.
 */
public class AttackState : State
{
    private FSM manager;

    private Parameter parameter;

    //Get the progress of attack animation.
    private AnimatorStateInfo animationInfo;

    public AttackState(FSM manager)
    {
        this.manager   = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter()
    {
        parameter.animator.Play("SlimeRun");
    }

    public void OnUpdate()
    {
        //Get animation state.
        animationInfo = parameter.animator.GetCurrentAnimatorStateInfo(0);

        if (parameter.beAttacked)
        {
            manager.TransitionState(StateType.BeAttacked);
        }

        // When the animation progress = 1, means the attack animation already finish then switch to the chase state.
        if (animationInfo.normalizedTime >= .99f)
        {
            manager.TransitionState(StateType.Chase);
        }
    }

    public void OnExit()
    {
    }
}

/**
 * Class for BeAttackedState.
 */
public class BeAttackedState : State
{
    private AnimatorStateInfo animationInfo;
    private FSM               manager;
    private Parameter         parameter;

    public BeAttackedState(FSM manager)
    {
        this.manager   = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter()
    {
        parameter.animator.Play("SlimeBeAttacked");
        parameter.health--;
    }

    public void OnUpdate()
    {
        animationInfo = parameter.animator.GetCurrentAnimatorStateInfo(0);
        //When health == 0, switch to dead state.
        if (parameter.health <= 0)
        {
            manager.TransitionState(StateType.Death);
        }

        if (animationInfo.normalizedTime >= .99f)
        {
            parameter.target = GameObject.FindWithTag("Player").transform;

            manager.TransitionState(StateType.Chase);
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
public class DeathState : State
{
    private FSM       manager;
    private Parameter parameter;

    public DeathState(FSM manager)
    {
        this.manager   = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter()
    {
        EnemyCounter.slimeCounter                  += 1;
        GameManager.Instance.GameData.slimeCounter =  EnemyCounter.slimeCounter;
        manager.dead();
        GameManager.Instance.Player.Upgrade(0.1f);
        parameter.animator.Play("SlimeDeath");
        parameter.attack.SetActive(false);
        parameter.dropLoot.GenLoots();
    }

    public void OnUpdate()
    {
    }

    public void OnExit()
    {
    }
}
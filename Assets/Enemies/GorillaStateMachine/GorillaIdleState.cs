using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GorillaIdleState : State
{
    private GorillaFSM       manager;
    private GorillaParameter parameter;

    //patrols need to stay at each point for a period of time, create a timer to control the stay time.
    private float timer;

    public GorillaIdleState(GorillaFSM manager)
    {
        this.manager   = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter()
    {
        if (parameter.health > parameter.transitionStateHealth)
        {
            parameter.animator.Play("GorillaIdle");
        }
        else
        {
            parameter.animator.Play("GorillaIdle2");
        }
    }

    public void OnUpdate()
    {
        if (!manager.fight.activeSelf)
        {
        }
        else
        {
            parameter.collider.size =  new Vector2(250f, 250f);
            timer                   += Time.deltaTime;

            //If attacked, switch to the attacked state
            if (parameter.beAttacked)
            {
                manager.TransitionState(GorillaStateType.BeAttacked);
            }

            //Check if the player is found & the player is in the chasing range.
            if (parameter.target            != null                                &&
                parameter.target.position.x >= parameter.chasePoints[0].position.x &&
                parameter.target.position.x <= parameter.chasePoints[1].position.x &&
                parameter.target.position.y <= parameter.chasePoints[2].position.y &&
                parameter.target.position.y >= parameter.chasePoints[3].position.y)
            {
                manager.TransitionState(GorillaStateType.React);
            }

            //When staying at the patrol point for a while, switch to patrol state.
            if (timer >= parameter.idleTime)
            {
                manager.TransitionState(GorillaStateType.Patrol);
            }
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
public class GorillaPatrolState : State
{
    private GorillaFSM       manager;
    private GorillaParameter parameter;

    //Used to find and switch patrol points
    private int patrolPosition;

    public GorillaPatrolState(GorillaFSM manager)
    {
        this.manager   = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter()
    {
        if (parameter.health > parameter.transitionStateHealth)
        {
            parameter.animator.Play("GorillaMove");
        }
        else
        {
            parameter.animator.Play("GorillaMove2");
        }

        parameter.health = 20;
        // parameter.HealthBar.SetActive(false);
    }

    public void OnUpdate()
    {
        manager.FlipTo(parameter.patrolPoints[patrolPosition]);

        manager.transform.position = Vector2.MoveTowards(manager.transform.position,
            parameter.patrolPoints[patrolPosition].position, parameter.moveSpeed * Time.deltaTime);

        if (parameter.beAttacked)
        {
            manager.TransitionState(GorillaStateType.BeAttacked);
        }

        if (parameter.target            != null                                &&
            parameter.target.position.x >= parameter.chasePoints[0].position.x &&
            parameter.target.position.x <= parameter.chasePoints[1].position.x &&
            parameter.target.position.y <= parameter.chasePoints[2].position.y &&
            parameter.target.position.y >= parameter.chasePoints[3].position.y)
        {
            manager.TransitionState(GorillaStateType.React);
        }

        //When reaching the patrol point position, switch to Idle state.
        if (Vector2.Distance(manager.transform.position, parameter.patrolPoints[patrolPosition].position) < .1f)
        {
            manager.TransitionState(GorillaStateType.Idle);
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
public class GorillaReactState : State
{
    private GorillaFSM       manager;
    private GorillaParameter parameter;

    //Get the progress of attack animation.
    private AnimatorStateInfo animationInfo;

    public GorillaReactState(GorillaFSM manager)
    {
        this.manager   = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter()
    {
        parameter.animator.Play("GorillaBeAttacked");
    }

    public void OnUpdate()
    {
        //Get animation state.
        animationInfo = parameter.animator.GetCurrentAnimatorStateInfo(0);

        if (parameter.beAttacked)
        {
            manager.TransitionState(GorillaStateType.BeAttacked);
        }

        // When the animation progress = 1, means the attack animation already finish then switch to the chase state.
        if (animationInfo.normalizedTime >= .99f)
        {
            manager.TransitionState(GorillaStateType.Chase);
        }
    }

    public void OnExit()
    {
    }
}

/**
 * Class for ChaseState.
 */
public class GorillaChaseState : State
{
    private GorillaFSM       manager;
    private GorillaParameter parameter;

    public GorillaChaseState(GorillaFSM manager)
    {
        this.manager   = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter()
    {
        if (parameter.health > parameter.transitionStateHealth)
        {
            parameter.animator.Play("GorillaMove");
        }
        else
        {
            parameter.animator.Play("GorillaMove2");
        }
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
            manager.TransitionState(GorillaStateType.BeAttacked);
        }

        //When the player leaves, switch to idle state.
        if (parameter.target             == null                               ||
            manager.transform.position.x < parameter.chasePoints[0].position.x ||
            manager.transform.position.x > parameter.chasePoints[1].position.x ||
            manager.transform.position.y > parameter.chasePoints[2].position.y ||
            manager.transform.position.y < parameter.chasePoints[3].position.y)
        {
            manager.TransitionState(GorillaStateType.Idle);
        }

        if (parameter.health < parameter.transitionStateHealth &&
            Physics2D.OverlapCircle(parameter.ShootAttackPoint.position, parameter.ShootArea, parameter.targetLayer))
        {
            manager.TransitionState(GorillaStateType.ShootAttack);
        }
        //Detect the attack range, if there is a player, switch to the attack state.
        else if (Physics2D.OverlapCircle(parameter.attackPoint.position, parameter.attackArea, parameter.targetLayer))
        {
            manager.TransitionState(GorillaStateType.Attack);
        }
    }

    public void OnExit()
    {
    }
}

/**
 * Class for ShootAttackState.
 */
public class GorillaShootAttackState : State
{
    private GorillaFSM manager;

    private GorillaParameter parameter;

    //Get the progress of attack animation.
    private AnimatorStateInfo animationInfo;

    public GorillaShootAttackState(GorillaFSM manager)
    {
        this.manager   = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter()
    {
        try
        {
            parameter.animator.Play("GorillaShootAttack");
            parameter.player       = parameter.tar.transform;
            parameter.timeBtwShots = parameter.startTimeBtwShoots;
        }
        catch (NullReferenceException)
        {
        }
    }

    public void OnUpdate()
    {
        //Get animation state.
        animationInfo = parameter.animator.GetCurrentAnimatorStateInfo(0);
        //face to player.
        manager.FlipTo(parameter.target);
        if (parameter.target)
            manager.transform.position = Vector2.MoveTowards(manager.transform.position,
                parameter.target.position, 0);

        if (parameter.beAttacked)
        {
            manager.TransitionState(GorillaStateType.BeAttacked);
        }

        if (Physics2D.OverlapCircle(parameter.attackPoint.position, parameter.attackArea, parameter.targetLayer))
        {
            manager.TransitionState(GorillaStateType.Attack);
        }

        manager.shoot();

        if (animationInfo.normalizedTime >= .99f)
        {
            manager.TransitionState(GorillaStateType.Chase);
        }
    }

    public void OnExit()
    {
    }
}

/**
 * Class for DashState.
 */
public class GorillaDashState : State
{
    private GorillaFSM        manager;
    private GorillaParameter  parameter;
    private AnimatorStateInfo animationInfo;

    public GorillaDashState(GorillaFSM manager)
    {
        this.manager   = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter()
    {
        if (parameter.health > parameter.transitionStateHealth)
        {
            parameter.animator.Play("GorillaDash1");
        }
        else
        {
            parameter.animator.Play("GorillaDash2");
        }

        manager.Dash();
        parameter.chaseSpeed += parameter.speed;
    }

    public void OnUpdate()
    {
        //Get animation state.
        animationInfo = parameter.animator.GetCurrentAnimatorStateInfo(0);

        //face to player.
        manager.FlipTo(parameter.target);
        if (parameter.target)
            manager.transform.position = Vector2.MoveTowards(manager.transform.position,
                parameter.target.position, parameter.chaseSpeed * Time.deltaTime);

        if (animationInfo.normalizedTime >= .99f)
        {
            parameter.chaseSpeed -= parameter.speed;
            manager.TransitionState(GorillaStateType.Chase);
        }
    }

    public void OnExit()
    {
    }
}

/**
 * Class for AttackState.
 */
public class GorillaAttackState : State
{
    private GorillaFSM manager;

    private GorillaParameter parameter;

    //Get the progress of attack animation.
    private AnimatorStateInfo animationInfo;

    public GorillaAttackState(GorillaFSM manager)
    {
        this.manager   = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter()
    {
        if (parameter.health > parameter.transitionStateHealth)
        {
            parameter.animator.Play("GorillaAttack");
        }
        else
        {
            parameter.animator.Play("GorillaAttack2");
        }
    }

    public void OnUpdate()
    {
        //Get animation state.
        animationInfo = parameter.animator.GetCurrentAnimatorStateInfo(0);

        if (parameter.beAttacked)
        {
            manager.TransitionState(GorillaStateType.BeAttacked);
        }

        // When the animation progress = 1, means the attack animation already finish then switch to the chase state.
        if (animationInfo.normalizedTime >= .99f)
        {
            if (parameter.health > parameter.transitionStateHealth && parameter.target != null)
            {
                manager.State1MeleeAttack(parameter.target);
            }
            else
            {
                manager.State2MeleeAttack(parameter.target);
            }

            manager.TransitionState(GorillaStateType.Chase);
        }
    }

    public void OnExit()
    {
    }
}

/**
 * Class for BeAttackedState.
 */
public class GorillaBeAttackedState : State
{
    private AnimatorStateInfo animationInfo;
    private GorillaFSM        manager;
    private GorillaParameter  parameter;

    public GorillaBeAttackedState(GorillaFSM manager)
    {
        this.manager   = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter()
    {
        if (parameter.health > parameter.transitionStateHealth)
        {
            parameter.animator.Play("GorillaBeAttacked");
        }
        else
        {
            parameter.animator.Play("GorillaBeAttacked2");
        }

        parameter.health--;
        parameter.dashCounter--;
        if (parameter.health <= 0)
        {
            parameter.health = 0;
        }

        GorillaHealth.HealthCurrent = parameter.health;

        //When health == ?, switch state.
        if (parameter.health == parameter.transitionStateHealth)
        {
            manager.TransitionState(GorillaStateType.TransitionState);
        }

        if (parameter.dashCounter == 0)
        {
            manager.TransitionState(GorillaStateType.Dash);
        }
    }

    public void OnUpdate()
    {
        animationInfo = parameter.animator.GetCurrentAnimatorStateInfo(0);
        //When health == 0, switch to dead state.
        if (parameter.health <= 0)
        {
            manager.TransitionState(GorillaStateType.Death);
            GameManager.Instance.GameData.enemies.Add(manager.enemy);
            manager.fight.SetActive(false);
        }

        if (animationInfo.normalizedTime >= .99f)
        {
            parameter.target = GameObject.FindWithTag("Player").transform;

            manager.TransitionState(GorillaStateType.Chase);
        }
    }

    public void OnExit()
    {
        parameter.beAttacked = false;
    }
}

/**
 * Class for TransitionState.
 */
public class GorillaTransitionState : State
{
    private GorillaFSM        manager;
    private GorillaParameter  parameter;
    private AnimatorStateInfo animationInfo;

    public GorillaTransitionState(GorillaFSM manager)
    {
        this.manager   = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter()
    {
        parameter.animator.Play("GorillaTransitionState");
    }

    public void OnUpdate()
    {
        animationInfo = parameter.animator.GetCurrentAnimatorStateInfo(0);

        if (animationInfo.normalizedTime >= .99f)
        {
            if (parameter.beAttacked)
            {
                manager.TransitionState(GorillaStateType.BeAttacked);
            }

            manager.TransitionState(GorillaStateType.Chase);
        }
    }

    public void OnExit()
    {
    }
}

/**
 * Class for DeathState.
 */
public class GorillaDeathState : State
{
    private GorillaFSM       manager;
    private GorillaParameter parameter;

    public GorillaDeathState(GorillaFSM manager)
    {
        this.manager   = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter()
    {
        EnemyCounter.bossCounter                  += 1;
        GameManager.Instance.GameData.bossCounter =  EnemyCounter.bossCounter;
        parameter.animator.Play("GorillaDeath");
        GameEnding.gorillaDead = true;
        manager.GenLoots();
        manager.GetComponent<QuestGiver>().GiveQuest();
        ChangeColor.color = true;
        manager.fight.SetActive(false);
    }

    public void OnUpdate()
    {
    }

    public void OnExit()
    {
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : BaseState
{
    private float losePlayerTimer;
    private float attackTimer;
    public override void Enter()
    {
        zombie.Agent.isStopped = true;
    }

    public override void Exit()
    {
    }

    public override void Perform()
    {
        if(zombie.PlayerInAttackRange())
        {
            losePlayerTimer = 0;
            attackTimer += Time.deltaTime;
            zombie.targetLook.transform.LookAt(zombie.target.transform);
            zombie.transform.rotation = Quaternion.Euler(0f, zombie.targetLook.transform.eulerAngles.y, 0f);
            if(attackTimer > zombie.attackRate)
            {
                Attack();
            }
        }
        else
        {
            losePlayerTimer += Time.deltaTime;
            if(losePlayerTimer > zombie.attackCycleTimer)
            {
                stateMachine.ChangeState(new FollowingState());
            }
        }
    }

    public void Attack()
    {
        //play attack animation
        attackTimer = 0;
        zombie.target.GetComponent<Player>().TakeDamage(zombie.attackDamage);
    }
}

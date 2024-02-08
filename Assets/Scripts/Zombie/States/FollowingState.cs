using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingState : BaseState
{
    public override void Enter()
    {
        zombie.Agent.isStopped = false;
    }
    public override void Perform()
    {
        FollowingCycle();
    }
    public override void Exit()
    {
    }
    public void FollowingCycle()
    {
        zombie.Agent.SetDestination(zombie.target.transform.position);
        if(zombie.PlayerInAttackRange())
        {
            stateMachine.ChangeState(new AttackState());
        }
    }
}

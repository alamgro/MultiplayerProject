using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anim_DemonAttack : StateMachineBehaviour
{
    [Range(0.1f, 1f)]
    [SerializeField] private float attackPercentageThreshold;
    private Soldier soldier; 
    private float realNormalizedTime = 0f;
    private bool isAttacking;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        isAttacking = false;
        soldier = animator.GetComponentInParent<Soldier>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        realNormalizedTime = stateInfo.normalizedTime % 1f; 
       if(!isAttacking && realNormalizedTime >= attackPercentageThreshold)
        {
            isAttacking = true;
            soldier.SpawnProjectile();
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        isAttacking = false;
        soldier.CurrentMovementSpeed = soldier.BaseMovementSpeed;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}

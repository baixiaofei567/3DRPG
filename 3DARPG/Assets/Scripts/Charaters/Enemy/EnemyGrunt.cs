using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyGrunt : EnemyController
{
    [Header("Skill")]
    public float kickForce = 10;

    public void KickOff()
    {
        if(attackTarget != null)
        {
            transform.LookAt(attackTarget.transform.position);

            Vector3 direction = attackTarget.transform.position - transform.position;
            direction.Normalize();

            //无论player在干嘛都让他停下
            attackTarget.GetComponent<NavMeshAgent>().isStopped = true;
            attackTarget.GetComponent<NavMeshAgent>().velocity = direction * kickForce;
            attackTarget.GetComponent<Animator>().SetTrigger("Dizzy");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Golem : EnemyController
{
    [Header("Skill")]
    public float kickForce = 25;

    public GameObject rockPrefab;

    public Transform rockPos;

    //Animation Event
    public void KickOff()
    {
        if (attackTarget != null && transform.IsFacingTarget(attackTarget.transform))
        {
            var targetStatus = attackTarget.GetComponent<CharactersStats>();

            Vector3 direction = attackTarget.transform.position - transform.position;
            direction.Normalize();

            targetStatus.GetComponent<NavMeshAgent>().isStopped = true;
            targetStatus.GetComponent<NavMeshAgent>().velocity = direction * kickForce;
            //把player眩晕
            targetStatus.GetComponent<Animator>().SetTrigger("Dizzy");
            targetStatus.TakeDamage(charactersStats, targetStatus);
        }
    }

    public void ThrowRock()
    {
        if(attackTarget != null)
        {
            var rock = Instantiate(rockPrefab, rockPos.position,Quaternion.identity);
            rock.GetComponent<Rock>().target = attackTarget;
        }
    }
    
}

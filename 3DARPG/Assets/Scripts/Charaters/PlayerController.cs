using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class PlayerController : MonoBehaviour
{
    //因为mousemanager为单例模式，所以可以快速访问它

    private NavMeshAgent agent;
    private Animator anim;

    private GameObject attackTarget;
    private float lastAttackTime;
    private float stopDistance;

    private CharactersStats characterStats;

    bool isDead;

    //rigibody等都放在awake中初始化，awake是最先运行的，这样就不会出现空引用的情况
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStats = GetComponent<CharactersStats>();
        stopDistance = agent.stoppingDistance;
    }
    private void OnEnable()
    {
        MouseManager.Instance.OnMouseClick += MoveToTarget;
        MouseManager.Instance.OnEnemyClick += EventAttack;
        GameManager.Instance.RigisterPlayer(characterStats);
    }

    private void Start()
    {
        //将move函数加入到click事件中，只要这个事件触发了，事件内的所有函数都会执行
        characterStats.MaxHealth = 100;

        //注册gamemanager
        SaveManager.Instance.LoadPlayerData();
    }


    private void OnDisable()
    {
        MouseManager.Instance.OnMouseClick -= MoveToTarget;
        MouseManager.Instance.OnEnemyClick -= EventAttack;
    }

    private void Update()
    {
        isDead = characterStats.CurrentHealth == 0;
        //如果player死了就向所有敌人广播player死的消息，让敌人执行对应的函数
        if (isDead) GameManager.Instance.NotifyObservers();

        SwitchAnimation();
        lastAttackTime -= Time.deltaTime;
    }

    private void SwitchAnimation()
    {
        anim.SetFloat("Speed", agent.velocity.sqrMagnitude);
        anim.SetBool("Death", isDead);
    }

    public void MoveToTarget(Vector3 target)
    {
        //将所有协程都终止，攻击就被打断了
        StopAllCoroutines();
        if (isDead) return;

        agent.stoppingDistance = stopDistance;

        agent.isStopped = false;
        agent.destination = target;
    }
    private void EventAttack(GameObject target)
    {
        if (isDead) return;
        if (target != null)
        {
            attackTarget = target;
            characterStats.isCritical = UnityEngine.Random.value < characterStats.attackData.criticalChance;
            //判断敌人距离和攻击距离，如果大于就一直移动
            StartCoroutine(MoveToAttackTarget());
        }
    }

    IEnumerator MoveToAttackTarget()
    {
        agent.isStopped = false;
        agent.stoppingDistance = characterStats.attackData.attackRange;

        transform.LookAt(attackTarget.transform);

        //TODO:修改攻击范围参数
        while (Vector3.Distance(attackTarget.transform.position, transform.position) > characterStats.attackData.attackRange)
        {
            agent.destination = attackTarget.transform.position;
            //每一帧来循环的时候记得yield return null
            // yield return null表示暂缓一帧，在下一帧接着往下处理,推迟一帧执行下面的语句
            yield return null;
        }

        agent.isStopped = true;
        //Attack
        if(lastAttackTime < 0)
        {
            anim.SetBool("Critical", characterStats.isCritical);
            anim.SetTrigger("Attack");
            //重置冷却时间
            lastAttackTime = characterStats.attackData.coolDown;
        }
    }

    //Animation Event
    void Hit()
    {
        if (attackTarget.CompareTag("Attackable"))
        {
            if (attackTarget.GetComponent<Rock>())
            {
                attackTarget.GetComponent<Rock>().rockStates = Rock.RockStates.HitEnemy;
                attackTarget.GetComponent<Rigidbody>().velocity = Vector3.one;
                attackTarget.GetComponent<Rigidbody>().AddForce(transform.forward * 20, ForceMode.Impulse);
            }
        }
        else
        {
            var targetStats = attackTarget.GetComponent<CharactersStats>();

            targetStats.TakeDamage(characterStats, targetStats);
        }
    }
}

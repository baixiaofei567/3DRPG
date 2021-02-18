using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyStates
{
    GUARD,PATROL,CHASE,DEAD
}

//脚本加到物体上，自动挂载这些组件
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CharactersStats))]
public class EnemyController : MonoBehaviour,IEndGameObserver
{
    private EnemyStates enemyStates;

    private NavMeshAgent agent;

    private Animator anim;

    protected CharactersStats charactersStats;

    private Collider coll;

    [Header("Basic Settings")]
    public float sightRadius;//敌人的可视范围
    protected GameObject attackTarget;
    public bool isGuard;
    private float speed;
    public float LookAtTime;//到达一个巡视点后东张西望的时间
    private float remainLookTime;
    private float lastAttackTime;

    //原始角度，防止复位后旋转方向不对
    private Quaternion guardRotation;

    [Header("Patrol State")]
    public float patrolRange;
    private Vector3 wayPoint;
    private Vector3 guardPos;

    //bool配合状态
    bool isWalk;
    bool isChase;
    bool isFollow;
    bool isDead;
    bool playerDead;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        speed = agent.speed;
        anim = GetComponent<Animator>();
        guardPos = transform.position;
        guardRotation = transform.rotation;
        remainLookTime = LookAtTime;
        charactersStats = GetComponent<CharactersStats>();
        coll = GetComponent<Collider>();
    }

    private void Start()
    {
        if (isGuard)
        {
            enemyStates = EnemyStates.GUARD;
        }
        else
        {
            enemyStates = EnemyStates.PATROL;
            GetNewWayPoint();
        }
    }

    private void OnEnable()
    {
        GameManager.Instance.AddObserver(this);
    }

    private void OnDisable()
    {
        if (!GameManager.isInitialized) return;
        GameManager.Instance.RemoveObserver(this);
        if(GetComponent<LootSpawner>() && isDead)
        {
            GetComponent<LootSpawner>().SpawnLoot();
        }
        //如果qm已经实例化，且敌人死了就更新一下这个任务，如果没有对应的任务就不会更新
        if(QuestManager.isInitialized && isDead)
        {
            QuestManager.Instance.UpdateQuestProgress(this.name, 1);
        }
    }

    private void Update()
    {
        if (charactersStats.CurrentHealth == 0) isDead = true;
        //如果player没死才执行下面这些
        if (!playerDead)
        {
            SwitchStates();
            SwitchAnimation();
            lastAttackTime -= Time.deltaTime;
        }
    }

    void SwitchAnimation()
    {
        anim.SetBool("Walk", isWalk);
        anim.SetBool("Chase", isChase);
        anim.SetBool("Follow", isFollow);
        anim.SetBool("Critical", charactersStats.isCritical);
        anim.SetBool("Death", isDead);
    }

    void SwitchStates()
    {
        if (isDead)
        {
            enemyStates = EnemyStates.DEAD;
        }
        //如果发现player就切换到chase
        else if (FoundPlayer())
        {
            enemyStates = EnemyStates.CHASE;
        }
        switch (enemyStates)
        {
            case EnemyStates.GUARD:
                isChase = false;

                if(transform.position != guardPos)
                {
                    isWalk = true;
                    agent.isStopped = false;
                    agent.destination = guardPos;

                    //也是判断两点的距离和distance差不多，distance消耗更大
                    if (Vector3.SqrMagnitude(guardPos - transform.position) <= agent.stoppingDistance)
                    {
                        isWalk = false;
                        transform.rotation = Quaternion.Lerp(transform.rotation, guardRotation, 0.01f);
                    }
                }
                break;
            case EnemyStates.PATROL:
                isChase = false;
                agent.speed = speed * 0.5f;

                //判断是否到了随机巡逻点
                if(Vector3.Distance(wayPoint,transform.position) <= agent.stoppingDistance)
                {
                    isWalk = false;
                    if(remainLookTime > 0)
                    {
                        remainLookTime -= Time.deltaTime;
                    }
                    else 
                        GetNewWayPoint();
                }
                else
                {
                    isWalk = true;
                    agent.destination = wayPoint;
                }
                break;
            case EnemyStates.CHASE:
                //TODO:追Player，如果超出范围就返回之前的状态
                //TODO:攻击和动画
                isWalk = false;
                isChase = true;

                agent.speed = speed;
                if (!FoundPlayer())
                {
                    //拉脱回到上一个状态
                    isFollow = false;
                    if(remainLookTime > 0)
                    {
                        agent.destination = transform.position;
                        remainLookTime -= Time.deltaTime;
                    }
                    else if(isGuard)
                    {
                        enemyStates = EnemyStates.GUARD;
                    }
                    else
                    {
                        enemyStates = EnemyStates.PATROL;
                    }
                }
                else
                {
                    isFollow = true;
                    agent.isStopped = false;
                    agent.destination = attackTarget.transform.position ;
                }
                //如果玩家在攻击范围内
                if(TargetInAttackRange() || TargetInSkillRange())
                {
                    isFollow = false;
                    agent.isStopped = true;

                    if(lastAttackTime < 0)
                    {
                        lastAttackTime = charactersStats.attackData.coolDown;

                        //暴击判断
                        charactersStats.isCritical = Random.value < charactersStats.attackData.criticalChance;

                        //执行攻击
                        Attack();
                    }
                }

                break;
            case EnemyStates.DEAD:
                coll.enabled = false;
                //agent.enabled = false;
                agent.radius = 0;
                Destroy(gameObject, 2f);
                break;
        }
    }

    void Attack()
    {
        transform.LookAt(attackTarget.transform);
        if (TargetInAttackRange())
        {
            //近身攻击动画
            anim.SetTrigger("Attack");
        }
        if (TargetInSkillRange())
        {
            //远程攻击动画
            anim.SetTrigger("Skill");
        }
    }

    bool FoundPlayer()
    {
        var colliders = Physics.OverlapSphere(transform.position, sightRadius);
        foreach(var target in colliders)
        {
            if (target.CompareTag("Player"))
            {
                attackTarget = target.gameObject;
                return true;
            }
        }
        attackTarget = null;
        return false;
    }

    bool TargetInAttackRange()
    {
        if(attackTarget != null)
        {
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= charactersStats.attackData.attackRange;
        }
        else 
            return false;
    }

    bool TargetInSkillRange()
    {
        if (attackTarget != null)
        {
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= charactersStats.attackData.skillRange;
        }
        else
            return false;
    }

    void GetNewWayPoint()
    {
        remainLookTime = LookAtTime;

        float randomX = Random.Range(-patrolRange, patrolRange);
        float randomZ = Random.Range(-patrolRange, patrolRange);

        Vector3 randomPoint = new Vector3(guardPos.x + randomX, transform.position.y, guardPos.z + randomZ);


        //不能简单的得到这个点，因为如果要移动到notwalkable的地方就会卡在那里
        NavMeshHit hit;
        wayPoint = NavMesh.SamplePosition(randomPoint, out hit, patrolRange, 1) ? hit.position : transform.position;
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.blue;
    //    Gizmos.DrawWireSphere(transform.position, sightRadius);
    //}

    //Animation Event
    void Hit()
    {
        if(attackTarget != null && transform.IsFacingTarget(attackTarget.transform))
        {
            var targetStatus = attackTarget.GetComponent<CharactersStats>();
            targetStatus.TakeDamage(charactersStats, targetStatus);
        }
    }

    public void EndNotify()
    {
        //获胜动画
        //停止所有移动，停止agent
        playerDead = true;
        anim.SetBool("Win", true);
        isChase = false;
        isWalk = false;
        attackTarget = null;
    }
}

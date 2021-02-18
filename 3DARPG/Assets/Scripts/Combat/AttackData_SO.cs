using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Attack", menuName = "Attack/Attack Data")]
public class AttackData_SO : ScriptableObject
{
    public float attackRange;

    public float skillRange;

    public float coolDown;

    public int minDamage;

    public int maxDamage;

    //暴击加成百分比
    public float criticalMultiplier;
    //暴击率
    public float criticalChance;

    public void ApplyWeaponData(AttackData_SO weapon, bool equip)
    {
        attackRange = weapon.attackRange;
        skillRange = weapon.skillRange;
        coolDown = weapon.coolDown;

        if (equip)
        {
            minDamage += weapon.minDamage;
            maxDamage += weapon.maxDamage;
        }
        else
        {
            minDamage = weapon.minDamage;
            maxDamage = weapon.maxDamage;
        }

        criticalMultiplier = weapon.criticalMultiplier;
        criticalChance = weapon.criticalChance;
    }
}

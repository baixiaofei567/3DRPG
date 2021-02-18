using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactersStats : MonoBehaviour
{
    //想要订阅该事件的函数也要有两个参数
    public event Action<int, int> UpdateHealthBarOnAttack;

    //控制人物属性的脚本
    public CharacterData_SO characterData;

    public CharacterData_SO templateData;

    //控制攻击的脚本
    public AttackData_SO attackData;

    private AttackData_SO baseAttackData;

    private RuntimeAnimatorController baseAnimator;

    [Header("weapon")]
    public Transform weaponSlot;

    [HideInInspector]
    public bool isCritical;

    private void Awake()
    {
        if(templateData != null)
        {
            characterData = Instantiate(templateData);
        }
        baseAttackData = Instantiate(attackData);
        baseAnimator = GetComponent<Animator>().runtimeAnimatorController;
    }

    #region Read from Data_SO
    public int MaxHealth
    {
        get
        {
            if (characterData != null) return characterData.maxHealth;
            else return 0;
        }
        set
        {
            characterData.maxHealth = value;
        }
    }
    public int CurrentHealth
    {
        get
        {
            if (characterData != null) return characterData.currentHealth;
            else return 0;
        }
        set
        {
            characterData.currentHealth = value;
        }
    }
    public int BaseDefence
    {
        get
        {
            if (characterData != null) return characterData.baseDefence;
            else return 0;
        }
        set
        {
            characterData.baseDefence = value;
        }
    }
    public int CurrentDefence
    {
        get
        {
            if (characterData != null) return characterData.currentDefence;
            else return 0;
        }
        set
        {
            characterData.currentDefence = value;
        }
    }
    #endregion

    #region Character Combat

    public void TakeDamage(CharactersStats attacker, CharactersStats defener)
    {
        //实际伤害是实际攻击力(随机生成的攻击力*暴力倍数)-对方的防御力，保证不会加血，要和0比
        int damage = Mathf.Max(0,attacker.CurrentDamage() - defener.CurrentDefence);

        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);
        if (attacker.gameObject.tag == "Player" && !attacker.isCritical) AudioManager.Instance.attack01Audio();
        if (attacker.isCritical)
        {
            defener.GetComponent<Animator>().SetTrigger("Hit");
            if (attacker.gameObject.tag == "Player") AudioManager.Instance.attack02Audio();
        }

        //TODO:UPDATA UI
        UpdateHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);
        //TODO:经验值
        if(CurrentHealth <= 0)
        {
            attacker.characterData.UpdateExp(characterData.killPoint);
        }
    }

    //重载TakeDamage，因为石头没有CharactersStats这个脚本
    public void TakeDamage(int damage,CharactersStats defener)
    {
        int currentDamage = Mathf.Max(damage - defener.CurrentDefence, 0); 
        UpdateHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);
        CurrentHealth = Mathf.Max(CurrentHealth - currentDamage, 0);

        if(CurrentHealth <= 0)
        {
            GameManager.Instance.playerStats.characterData.UpdateExp(characterData.killPoint);
        }
    }

    private int CurrentDamage()
    {
        float coreDamage = UnityEngine.Random.Range(attackData.minDamage, attackData.maxDamage);

        if (isCritical)
        {
            coreDamage *= attackData.criticalMultiplier;
        }

        return (int)coreDamage;
    }

    #endregion

    #region Equip Weapon
    public void ChangeWeapon(ItemData_SO weapon)
    {
        UnEquipWeapon();
        EquipWeapon(weapon);
        //用武器的attackdata来更新人物的attackdata
        attackData.ApplyWeaponData(weapon.weaponData,true);
        InventoryManager.Instance.UpdateStatsText(MaxHealth, attackData.minDamage, attackData.maxDamage);
    }

    public void EquipWeapon(ItemData_SO weapon)
    {
        if(weapon.weaponPrefab != null)
        {
            Instantiate(weapon.weaponPrefab,weaponSlot);

        }
        //切换动画状态机
        GetComponent<Animator>().runtimeAnimatorController = weapon.anim;
    }

    //卸下武器的方法
    public void UnEquipWeapon()
    {
        if(weaponSlot.transform.childCount != 0)
        {
            for(int i = 0; i < weaponSlot.transform.childCount; ++i)
            {
                Destroy(weaponSlot.transform.GetChild(i).gameObject);
            }
        }
        attackData.ApplyWeaponData(baseAttackData,false);
        //TODO::切换动画
        GetComponent<Animator>().runtimeAnimatorController = baseAnimator;
    }
    #endregion

    #region Apply Data Change
    public void ApplyHealth(int amount)
    {
        if (CurrentHealth + amount <= MaxHealth) CurrentHealth += amount;
        else CurrentHealth = MaxHealth;
    }

    #endregion
}

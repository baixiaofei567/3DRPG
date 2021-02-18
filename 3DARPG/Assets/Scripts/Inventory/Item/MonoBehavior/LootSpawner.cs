using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootSpawner : MonoBehaviour
{
    [System.Serializable]
    public class LootItem
    {
        public GameObject item;

        [Range(0,1)]
        public float weight;
    }

    public LootItem[] lootItems;

    public void SpawnLoot()
    {
        //每次怪物死亡时调用该函数，生成一个随机值，遍历怪物可能会掉的所有物品，如果这个值小于该物品掉落概率，就在怪物头顶生成这个物品
        float currentValue = Random.value;

        for(int i = 0; i <lootItems.Length; ++i)
        {
            if(currentValue <= lootItems[i].weight)
            {
                GameObject obj = Instantiate(lootItems[i].item);
                obj.transform.position = transform.position + Vector3.up * 2;
                //加上break的话就是只掉落一个物品，不加的话就会生成所有概率大于currentValue的物品
                break;
            }
        }
    }
}

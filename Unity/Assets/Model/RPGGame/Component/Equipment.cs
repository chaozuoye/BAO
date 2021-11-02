using System;
using System.Collections.Generic;
using UnityEngine;

// 用于实体对象的装备组件基础类,其它类似的还有:
// Skills - PlayerSkills
// Equipment - PlayerEquipment
// Inventory - PlayerInventory

[DisallowMultipleComponent]
public abstract class Equipment
{
    /***************************************************************************
    * 用于持有实体的装备Scriptable资源数据对象的数组                                 *
    * ItemSC[] equipments                                                      *
    *=========================================================================*/
    public ItemSC[] equipments;
    ////////////////////////////////////////////////////////////////////////////
    // 帮助函数
    /// <summary>查找武器装备的索引,如果没有武器装备，返回-1。</summary>
    public int GetEquippedWeaponIndex()
    {
        // (avoid FindIndex to minimize allocations)
        for (int i = 0; i < slots.Count; ++i)
        {
            ItemSlot slot = slots[i];
            if (slot.amount > 0 && slot.item.data is WeaponItem)
                return i;
        }
        return -1;
    }
}
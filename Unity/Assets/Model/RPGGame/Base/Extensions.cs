using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using System;
using System.Collections.Generic;
using System.Linq;

public static class Extensions
{
    // 查找列表中的所有重复项
    // 注意：这只在开始时调用一次，所以Linq在这里很好！
    public static List<U> FindDuplicates<T, U>(this List<T> list, Func<T, U> keySelector)
    {
        return list.GroupBy(keySelector)
                   .Where(group => group.Count() > 1)
                   .Select(group => group.Key).ToList();
    }
}
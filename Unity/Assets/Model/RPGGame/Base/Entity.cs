using System;
using UnityEngine;

//Player，NPC，Monster等实体的父对象类
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public partial class Entity : NetBehaviourNonAlloc
{
    [Header("Components")]
    public Animator animator;
    new public Collider collider;
    public AudioSource audioSource;

    /// <summary>
    /// 基类绑定装备组件  
    /// </summary>
    public Equipment baseEquipment;

    /// <summary>
    /// 角色动作状态
    /// </summary>
    [SerializeField] string _state = "IDLE";
    public string state => _state;
    GameObject _target;
    /// <summary>
    /// 角色目标 
    /// </summary>
    public Entity target
    {
        get { return _target != null ? _target.GetComponent<Entity>() : null; }
        set { _target = value != null ? value.gameObject : null; }
    }

    void Update()
    {
        // 暂时什么也不用写,肯定会有需要写点什么的时候的
        // ...
    }
}

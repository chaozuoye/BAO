using System.Collections.Generic;
using UnityEngine;
using MMOGame;
// MMOManager单例，对网络层的管理与调用
// 只有进入地图场景后才有游戏层要通过MMOManager来调用网络层的需求
// MMOManager Awake初始化时， Manager即获取了MMOManager的实例

namespace Mirror
{

    public partial class MMOManager : MonoBehaviour, IMMOManager
    {
        public static MMOManager singleton { get; private set; }

        public void Awake()
        {
            if (singleton == null) singleton = this;
            Manager.MMO = (MMOManager)(singleton as IMMOManager);
        }

        public void Start()
        {
        }

        /*public void RpcQuitWorld()
        {
            MapHelper.Back2LobbyAsync().Coroutine();
        }

        public void RpcSendStateFrame(Move move)
        {
            MapHelper.SendStateFrame(move).Coroutine();
        }*/

    }
}
using System;
using UnityEngine;
using Multiplay;
using System.Collections.Generic;

namespace NetWork
{
    /// <summary>
    /// 一个游戏客户端只能存在一个网络玩家
    /// </summary>
    public class NetworkPlayer : MonoBehaviour
    {
        //单例
        private NetworkPlayer() { }
        public static NetworkPlayer Instance { get; private set; }
        [HideInInspector]
        public Gamer Gamer;                     //玩家类型
        [HideInInspector]
        public int RoomId = 0;                  //房间号码
        [HideInInspector]
        public bool Playing = false;            //正在游戏
        [HideInInspector]
        public bool menu;            //是否打开menu
        [HideInInspector]
        public string Name;                     //名字
        [HideInInspector]
        public string NewGamerName;              //新添加进游戏的用户名
        public NetworkGameplay Gameplay;         //游戏层对象
        public Dictionary<string, NetworkGameplay> Gamers;
        public Dictionary<string, AudioSourceController> Audios;
        public Dictionary<string, GameObject> Players;
        public Action<int> OnRoomIdChange;      //房间ID改变
        public Action<bool> OnPlayingChange;    //游戏状态改变
        public Action<string> OnNameChange;     //名字改变

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            menu = false;
            Gamers = new Dictionary<string, NetworkGameplay>();
            Audios = new Dictionary<string, AudioSourceController>();
            Players = new Dictionary<string, GameObject>();
            OnRoomIdChange += (roomId) => RoomId = roomId;

            OnPlayingChange += (playing) => Playing = playing;

            OnNameChange += (name) => Name = name;
        }
        public void PlayMoveRequest(Vector3 position)
        {
            Network.Instance.PlayMoveRequest(position);
        }
        public void PlayVoiceRequest(byte[] data)
        {
            Network.Instance.PlayVoiceRequest(data);
        }
        public void ExitGameRequest()
        {
            Network.Instance.ExitGameRequest(RoomId);
        }
        public void DestroyMenu(GameObject Object)
        {
            menu = false;
            Destroy(Object);
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.M)&&!menu&&Playing)
            {

                GameObject Menu = (GameObject)Resources.Load("Menu");
                Instantiate(Menu);
                menu = true;
            }
        }
    }
}
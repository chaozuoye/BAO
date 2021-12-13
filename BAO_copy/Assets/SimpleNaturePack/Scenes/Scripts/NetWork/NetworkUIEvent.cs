﻿using UnityEngine;
using UnityEngine.UI;
using General;

namespace NetWork
{
    /// <summary>
    /// 网络UI事件
    /// </summary>
    public class NetworkUIEvent : MonoBehaviour
    {
        [SerializeField]
        private GameObject _hide;             //可隐藏UI
        [SerializeField]
        private InputField _roomIdIpt;        //房间号码输入框
        [SerializeField]
        private InputField _nameIpt;          //名字输入框

        /*[SerializeField]
        private Button _connectServerBtn;     //连接服务器按钮*/
        [SerializeField]
        private Button _enrollBtn;            //注册按钮
        [SerializeField]
        private Button _creatRoomBtn;         //创建房间按钮
        [SerializeField]
        private Button _enterRoomBtn;         //加入房间按钮
        [SerializeField]
        private Button _exitRoomBtn;          //退出房间按钮
        [SerializeField]
        private Button _startGameBtn;         //开始游戏按钮
        [SerializeField]
        private Button _exitGameBtn;          //结束游戏按钮

        [SerializeField]
        private Text _gameStateTxt;           //游戏状态文本
        [SerializeField]
        private Text _roomIdTxt;              //房间号码文本
        [SerializeField]
        private Text _nameTxt;                //名字文本

        private void Start()
        {
            //绑定按钮事件
            _enrollBtn.onClick.AddListener(_EnrollBtn);
            _creatRoomBtn.onClick.AddListener(_CreatRoomBtn);
            _enterRoomBtn.onClick.AddListener(_EnterRoomBtn);
            _exitRoomBtn.onClick.AddListener(_ExitRoomBtn);
            _startGameBtn.onClick.AddListener(_StartGameBtn);
            _exitGameBtn.onClick.AddListener(_ExitGameBtn);
            NetworkPlayer.Instance.OnPlayingChange += (playing) =>
            {
                if (playing)
                    _gameStateTxt.text = "游戏";
                else
                    _gameStateTxt.text = "空闲";
            };

            NetworkPlayer.Instance.OnRoomIdChange += (roomId) =>
            {
                _roomIdTxt.text = "" + roomId;
            };

            NetworkPlayer.Instance.OnNameChange += (name) =>
            {
                _nameTxt.text = name;
            };
        }
        private void _EnrollBtn()
        {
            if (_nameIpt.text != string.Empty)
                Network.Instance.EnrollRequest(_nameIpt.text);
            else
            {
                Info.Instance.Print("名字不能为空");
            }
        }

        private void _CreatRoomBtn()
        {
            int roomId;
            int.TryParse(_roomIdIpt.text, out roomId);

            if (roomId != 0)
            {
                Network.Instance.CreatRoomRequest(roomId);
                Info.Instance.Print("创建房间" + (roomId));
            }
            else
            {
                Info.Instance.Print("不能以0作为房间号");
            }

        }

        private void _EnterRoomBtn()
        {
            int roomId;
            int.TryParse(_roomIdIpt.text, out roomId);

            if (roomId != 0)
                Network.Instance.EnterRoomRequest(roomId);
            else
            {
                Info.Instance.Print("不能以0作为房间号");
            }
        }

        private void _ExitRoomBtn()
        {
            Network.Instance.ExitRoomRequest(NetworkPlayer.Instance.RoomId);
        }

        private void _StartGameBtn()
        {
            Network.Instance.StartGameRequest(NetworkPlayer.Instance.RoomId);
        }

        private void _ExitGameBtn()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
Application.Quit();
#endif
        }
    }
}
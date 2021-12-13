using UnityEngine;
using General;
using Multiplay;
using Game;
namespace NetWork
{
    public class Network : MonoBehaviour
    {
        private GameObject Object;
        private Vector3 Err;
        [HideInInspector]
        public GameObject loginui;
        private void Awake()
        {
            Err = new Vector3(-1, -1, -1);
        }
        private Network() { }
        public static Network Instance { get; private set; }

        /// <summary>
        /// 注册
        /// </summary>
        public void EnrollRequest(string name)
        {
            Enroll request = new Enroll();
            request.Name = name;
            byte[] data = NetworkUtils.Serialize(request);
            NetworkClient.Enqueue(MessageType.Enroll, data);
        }

        /// <summary>
        /// 创建房间
        /// </summary>
        public void CreatRoomRequest(int roomId)
        {
            CreatRoom request = new CreatRoom();
            request.RoomId = roomId;
            byte[] data = NetworkUtils.Serialize(request);
            NetworkClient.Enqueue(MessageType.CreatRoom, data);
        }

        /// <summary>
        /// 加入房间
        /// </summary>
        public void EnterRoomRequest(int roomId)
        {
            EnterRoom request = new EnterRoom();
            request.RoomId = roomId;
            byte[] data = NetworkUtils.Serialize(request);
            NetworkClient.Enqueue(MessageType.EnterRoom, data);
        }

        /// <summary>
        /// 退出房间
        /// </summary>
        public void ExitRoomRequest(int roomId)
        {
            ExitRoom request = new ExitRoom();
            request.RoomId = roomId;
            byte[] data = NetworkUtils.Serialize(request);
            NetworkClient.Enqueue(MessageType.ExitRoom, data);
        }

        /// <summary>
        /// 开始游戏
        /// </summary>
        public void StartGameRequest(int roomId)
        {
            StartGame request = new StartGame();
            request.RoomId = roomId;
            byte[] data = NetworkUtils.Serialize(request);
            NetworkClient.Enqueue(MessageType.StartGame, data);
        }
        public void ExitGameRequest(int roomId)
        {
            ExitGame request = new ExitGame();
            request.Name = NetworkPlayer.Instance.Name;
            request.RoomId = roomId;
            byte[] data = NetworkUtils.Serialize(request);
            NetworkClient.Enqueue(MessageType.ExitGame, data);
        }
        /// <summary>
        /// 玩家移动请求
        /// </summary>
        public void PlayMoveRequest(Vector3 position)
        {
            Vector3 pos = position;

            if (pos==Err) return;

            Move request = new Move();
            request.RoomId = NetworkPlayer.Instance.RoomId;
            request.Gamer = NetworkPlayer.Instance.Gamer;
            request.X = pos.x;
            request.Y = pos.y;
            request.Z = pos.z;
            request.Name = NetworkPlayer.Instance.Name;
            byte[] data = NetworkUtils.Serialize(request);
            NetworkClient.Enqueue(MessageType.Move, data);
        }

        /// <summary>
        /// 玩家语音请求
        /// </summary>
        public void PlayVoiceRequest(byte[] voiceData)
        {
            Voice request = new Voice();
            request.RoomId = NetworkPlayer.Instance.RoomId;
            request.Gamer = NetworkPlayer.Instance.Gamer;
            request.data = voiceData;
            request.Name = NetworkPlayer.Instance.Name;
            byte[] data = NetworkUtils.Serialize(request);
            NetworkClient.Enqueue(MessageType.PlayVoice, data);
        }
        private void Start()
        {
            if (Instance == null)
                Instance = this;
            NetworkClient.Register(MessageType.HeartBeat, _Heartbeat);
            NetworkClient.Register(MessageType.Enroll, _Enroll);
            NetworkClient.Register(MessageType.CreatRoom, _CreatRoom);
            NetworkClient.Register(MessageType.EnterRoom, _EnterRoom);
            NetworkClient.Register(MessageType.ExitRoom, _ExitRoom);
            NetworkClient.Register(MessageType.StartGame, _StartGame);
            NetworkClient.Register(MessageType.ExitGame, _EixtGame);
            NetworkClient.Register(MessageType.Move, _PlayMove);
            NetworkClient.Register(MessageType.PlayVoice, _PlayVoice);
        }

        #region 发送消息回调事件

        private void _Heartbeat(byte[] data)
        {
            NetworkClient.Received = true;
            //Debug.Log("收到心跳包回应");
        }

        private void _Enroll(byte[] data)
        {
            Enroll result = NetworkUtils.Deserialize<Enroll>(data);
            if (result.Suc)
            {
                NetworkPlayer.Instance.OnNameChange(result.Name);

                Info.Instance.Print("注册成功");
            }
            else
            {
                Info.Instance.Print("注册失败");
            }
        }

        private void _CreatRoom(byte[] data)
        {
            CreatRoom result = NetworkUtils.Deserialize<CreatRoom>(data);

            if (result.Suc)
            {
                NetworkPlayer.Instance.OnRoomIdChange(result.RoomId);

                Info.Instance.Print(string.Format("创建房间成功, 你的房间号是{0}", NetworkPlayer.Instance.RoomId));
            }
            else
            {
                Info.Instance.Print("创建房间失败");
            }
        }

        private void _EnterRoom(byte[] data)
        {
            EnterRoom result = NetworkUtils.Deserialize<EnterRoom>(data);

            if (result.result == EnterRoom.Result.Player)
            {
                Info.Instance.Print("加入房间成功, 你是一名玩家");
            }
            else if (result.result == EnterRoom.Result.Observer)
            {
                Info.Instance.Print("加入房间成功, 你是一名观察者");
            }
            else
            {
                Info.Instance.Print("加入房间失败");
                return;
            }

            //进入房间
            NetworkPlayer.Instance.OnRoomIdChange(result.RoomId);
        }

        private void _ExitRoom(byte[] data)
        {
            ExitRoom result = NetworkUtils.Deserialize<ExitRoom>(data);

            if (result.Suc)
            {
                //房间号变为默认
                NetworkPlayer.Instance.OnRoomIdChange(0);
                //玩家状态改变
                NetworkPlayer.Instance.OnPlayingChange(false);

                Info.Instance.Print("退出房间成功");
            }
            else
            {
                Info.Instance.Print("退出房间失败");
            }
        }

        private void _StartGame(byte[] data)
        {

            StartGame result = NetworkUtils.Deserialize<StartGame>(data);

            if (result.Suc)
            {
                NetworkPlayer.Instance.NewGamerName = result.Name;
                GameObject gameObject = (GameObject)Resources.Load("Player");
                Object = Instantiate(gameObject) as GameObject;
                NetworkPlayer.Instance.Players.Add(result.Name, Object);
                Info.Instance.Print("init Player");
                if (result.Name == NetworkPlayer.Instance.Name)
                {
                    /*                    GameObject vedio = (GameObject)Resources.Load("GameStartVedio");
                                        Instantiate(vedio);*/
                    //Handheld.PlayFullScreenMovie("test.mp4", Color.black, FullScreenMovieControlMode.CancelOnInput);
                    Destroy(loginui);
                    //开始游戏事件
                    NetworkPlayer.Instance.OnPlayingChange(true);
                }

            }
            else
            {
                Info.Instance.Print("开始游戏失败");
            }
        }
        private void _EixtGame(byte[] data)
        {
            ExitGame result = NetworkUtils.Deserialize<ExitGame>(data);
            if (NetworkPlayer.Instance.Players.ContainsKey(result.Name))
            {
                Destroy(NetworkPlayer.Instance.Players[result.Name]);
            }

        }
        private void _PlayMove(byte[] data)
        {
            Move result = NetworkUtils.Deserialize<Move>(data);
            Info.Instance.Print(result.Name + " (" + result.X + "," + result.Y + "," + result.Z + ")");
            if (NetworkPlayer.Instance.Name == result.Name)
            {
                if (NetworkPlayer.Instance.Gameplay != null)
                {
                    NetworkPlayer.Instance.Gameplay.FixedMove(result);
                }
            }
            else
            {
                if (NetworkPlayer.Instance.Gamers.ContainsKey(result.Name))
                {
                    NetworkPlayer.Instance.Gamers[result.Name].FixedMove(result);
                }
            }

        }
        private void _PlayVoice(byte[] data)
        {
            Voice result = NetworkUtils.Deserialize<Voice>(data);
            if (NetworkPlayer.Instance.Audios.ContainsKey(result.Name))
            {
                NetworkPlayer.Instance.Audios[result.Name].PlayVoice(result.data);
            }

        }

        #endregion
    }
}
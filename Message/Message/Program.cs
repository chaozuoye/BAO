using System;

namespace Multiplay
{
    /// <summary>
    /// 消息类型
    /// </summary>
    public enum MessageType
    {
        None,         //空类型
        HeartBeat,    //心跳包验证
        Enroll,       //注册
        CreatRoom,    //创建房间
        EnterRoom,    //进入房间
        ExitRoom,     //退出房间
        StartGame,    //开始游戏
        Move,           //移动
        PlayVoice,      //语音通信
        ExitGame,       //退出游戏
    }
    /// <summary>
    /// 玩家
    /// </summary>
    public enum Gamer
    {
        Normal,
        Administrator,
    }
    [Serializable]
    public class Enroll
    {
        public string Name;//姓名
        public bool Suc;   //是否成功
    }

    [Serializable]
    public class CreatRoom
    {
        public int RoomId; //房间号码
        public bool Suc;   //是否成功
    }

    [Serializable]
    public class EnterRoom
    {
        public int RoomId;      //房间号码
        public Result result;   //结果

        public enum Result
        {
            None,
            Player,
            Observer,
        }
    }

    [Serializable]
    public class ExitRoom
    {
        public int RoomId;  //房间号码
        public bool Suc;    //是否成功
    }

    [Serializable]
    public class StartGame
    {
        public int RoomId;            //房间号码
        public string Name;           //玩家名
        public bool Suc;              //是否成功
    }
    [Serializable]
    public class ExitGame
    {
        public int RoomId;            //房间号码
        public string Name;           //玩家名
        public bool Suc;              //是否成功
    }
    [Serializable]
    public class Move
    {
        public int RoomId;       //房间号码
        public Gamer Gamer;      //玩家类型
        public string Name;     //玩家名
        public float X;            //
        public float Y;            //
        public float Z;
        public float Amount;    //插值

        public bool Suc;         //操作结果
    }

    [Serializable]
    public class Voice
    {
        public int RoomId;       //房间号码
        public Gamer Gamer;      //玩家类型
        public string Name;     //玩家名
        public byte[] data;     //音频数据

        public bool Suc;         //操作结果
    }

}
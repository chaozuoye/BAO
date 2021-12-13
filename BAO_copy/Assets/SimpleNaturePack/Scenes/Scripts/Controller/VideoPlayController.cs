using UnityEngine;
using UnityEngine.Video;
using General;
using System;
namespace NetWork
{
    public class VideoPlayController:MonoBehaviour
    {
        public GameObject video;
        private VideoPlayer m_VideoPlayer;
        void Awake()
        {
            m_VideoPlayer = GetComponent<VideoPlayer>();
           // m_VideoPlayer.loopPointReached += OnMovieFinished; // loopPointReached is the event for the end of the video
        }
        private void Update()
        {
            if (m_VideoPlayer.isPlaying)
            {
                if (m_VideoPlayer.frame+1 >= (long)m_VideoPlayer.frameCount)
                {
                    //链接服务器
                    NetworkClient.Connect("192.168.220.1");
                    GameObject gameLogin = (GameObject)Resources.Load("GameLogin");
                    GameObject loginui = Instantiate(gameLogin) as GameObject;
                    Network.Instance.loginui = loginui;
                    Destroy(video);
                }
            }
        }
    }
}

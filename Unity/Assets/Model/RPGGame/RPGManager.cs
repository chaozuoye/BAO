using System.Collections.Generic;
using UnityEngine;

namespace Mirror
{
    public enum UIState { UILogin, UIRegister, UISelection, UICreation }
    public partial class RPGManager : NetworkManager
    {
        public static RPGManager singleton { get; private set; }
        public static NetworkIdentity localIdentity { get; private set; }
        [HideInInspector] public List<Player> playerClasses = new List<Player>();
        public Transform loginCameraLocation;
        public Transform selectionCameraLocation;
        public Transform SpawnLocation;

        public int selection = -1;
        // 网络状态
        public NetworkState state = NetworkState.Offline;
        public void CameraTo(NetworkState state)
        {
            switch (state)
            {
                case NetworkState.Login:
                    Camera.main.transform.position = loginCameraLocation.position;
                    Camera.main.transform.rotation = loginCameraLocation.rotation;
                    break;
                case NetworkState.Lobby:
                    Camera.main.transform.position = selectionCameraLocation.position;
                    Camera.main.transform.rotation = selectionCameraLocation.rotation;
                    break;
                    // 世界地图摄像机跟随角色的
            }
        }
    }
}

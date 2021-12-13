using UnityEngine;
using UnityEngine.UI;
using General;

namespace NetWork
{
    /// <summary>
    /// 网络UI事件
    /// </summary>
    public class NetworkMenu : MonoBehaviour
    {
        public GameObject prefab;
        [SerializeField]
        private GameObject _hide;             //可隐藏UI
        [SerializeField]
        private Button _startGameBtn;         //返回游戏按钮
        [SerializeField]
        private Button _exitGameBtn;          //结束游戏按钮

        private void Awake()
        {
            //绑定按钮事件
            _startGameBtn.onClick.AddListener(_StartGameBtn);
            _exitGameBtn.onClick.AddListener(_ExitGameBtn);
        }

        private void _StartGameBtn()
        {
            NetworkPlayer.Instance.DestroyMenu(prefab);
        }

        private void _ExitGameBtn()
        {
            NetworkPlayer.Instance.ExitGameRequest();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
Application.Quit();
#endif
        }
    }
}
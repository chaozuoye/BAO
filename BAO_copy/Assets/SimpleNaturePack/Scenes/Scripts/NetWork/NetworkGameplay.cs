using UnityEngine;
using Multiplay;
using General;
namespace NetWork
{
    /// <summary>
    /// 处理下棋逻辑
    /// </summary>
    public class NetworkGameplay : MonoBehaviour
    {
        //单例
        private NetworkGameplay() { }
        /*        public NetworkGameplay Instance { get; private set; }*/
        [SerializeField]
        private GameObject _gamer;     //需要实例化的玩家 
        private string GamerName;   //玩家名
        private CharacterController cc;
        private Vector3 dir;
        private Vector3 velocity;
        public float moveSpeed;
        public float jumpSeep;
        public float horizontalMove, verticalMove;
        public float gravity;
        public Transform groundCheck;
        public float checkRadius;
        public LayerMask groundLayer;
        public bool isGround;
        public Vector3 targetPosition;
        public Vector3 startPosition;

        private void Awake()
        {
            GamerName = NetworkPlayer.Instance.NewGamerName;
            cc = GetComponent<CharacterController>();
            startPosition = transform.position;
            if (GamerName == NetworkPlayer.Instance.Name)
            {
                NetworkPlayer.Instance.Gameplay = this;
                NetworkPlayer.Instance.Gamers.Add(GamerName, NetworkPlayer.Instance.Gameplay);
                Info.Instance.Print("创建当前NetworkGamePlay 实例");
            }
            else
            {
                NetworkGameplay newGamer = this;
                NetworkPlayer.Instance.Gamers.Add(GamerName, newGamer);
                Info.Instance.Print("创建新NetworkGamePlay 实例");
            }

        }
        private void FixedUpdate()
        {

            if (GamerName == NetworkPlayer.Instance.Name)
            {
                isGround = Physics.CheckSphere(groundCheck.position, checkRadius, groundLayer);
                if (isGround && velocity.y < 0)
                {
                    velocity.y = -2f;
                }
                horizontalMove = Input.GetAxis("Horizontal") * moveSpeed;
                verticalMove = Input.GetAxis("Vertical") * moveSpeed;

                dir = transform.forward * verticalMove + transform.right * horizontalMove;
                cc.Move(dir * Time.deltaTime);
                if (Input.GetButton("Jump") && isGround)
                {
                    velocity.y = jumpSeep;
                }
                velocity.y -= gravity * Time.deltaTime;
                cc.Move(velocity * Time.deltaTime);
                if (startPosition != transform.position)
                {
                    startPosition = transform.position;
                    NetworkPlayer.Instance.PlayMoveRequest(transform.position); //调用网络层
                }
            }
        }
        //被网络层调用
        public void FixedMove(Move move)
        {
            if (NetworkPlayer.Instance.Name != move.Name)
            {
                //如果不是本地玩家
                Vector3 p = new Vector3(move.X - transform.position.x, move.Y - transform.position.y, move.Z - transform.position.z);
                if (startPosition.x != move.X || startPosition.y != move.Y || startPosition.z != move.Z)
                {
                    cc.Move(p);
                    startPosition = transform.position;
                }
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using General;
namespace NetWork
{
    public class CameraControlller : MonoBehaviour
    {
        [SerializeField]
        public new GameObject camera;
        public Transform player;
        private float mouseX, mouseY;//��ȡ����ƶ�ֵ
        private string NewGamerName;    //�����
        public float mouseSensitivity;//���������
        public float xRotation;
        // Start is called before the first frame update
        private void Awake()
        {
            NewGamerName = NetworkPlayer.Instance.NewGamerName;
            if (NewGamerName == NetworkPlayer.Instance.Name)
            {
                Info.Instance.Print("���õ�ǰ"+NewGamerName+"������");  
                camera.SetActive(true);
            }
            else
            {
                camera.SetActive(false);
                Info.Instance.Print("�ر�"+NewGamerName+"���������");
            }
               
        }
        // Update is called once per frame
        private void Update()
        {

            mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -70, 70);
            player.Rotate(Vector3.up * mouseX);
            transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        }
    }
}
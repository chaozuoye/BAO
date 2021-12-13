using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using Multiplay;
using General;
namespace NetWork
{
    /// <summary>
    /// �ص�ί��
    /// </summary>
    public delegate void CallBack(byte[] data);

    /// <summary>
    /// <see langword="static"/>
    /// </summary>
    public static class NetworkClient
    {
        private class NetworkCoroutine : MonoBehaviour
        {
            private event Action ApplicationQuitEvent;

            private static NetworkCoroutine _instance;

            /// <summary>
            /// ��������(���泡���ı������)
            /// </summary>
            public static NetworkCoroutine Instance
            {
                get
                {
                    if (!_instance)
                    {
                        GameObject socketClientObj = new GameObject("NetworkCoroutine");
                        _instance = socketClientObj.AddComponent<NetworkCoroutine>();
                        DontDestroyOnLoad(socketClientObj);
                    }
                    return _instance;
                }
            }

            /// <summary>
            /// �����˳��¼�
            /// </summary>
            public void SetQuitEvent(Action func)
            {
                if (ApplicationQuitEvent != null)
                    return;
                ApplicationQuitEvent += func;
            }

            /// <summary>
            /// �����˳�
            /// </summary>
            private void OnApplicationQuit()
            {
                if (ApplicationQuitEvent != null)
                    ApplicationQuitEvent();
            }
        }

        /// <summary>
        /// �ͻ�������״̬ö��
        /// </summary>
        private enum ClientState
        {
            None,        //δ����
            Connected,   //���ӳɹ�
        }

        //��Ϣ������ص��ֵ�
        private static Dictionary<MessageType, CallBack> _callBacks = new Dictionary<MessageType, CallBack>();
        //��������Ϣ����
        private static Queue<byte[]> _messages;
        //��ǰ״̬
        private static ClientState _curState;
        //�����������TCP���Ӳ���ȡ����ͨѶ��
        private static TcpClient _client;
        //������ͨѶ���ж�д����
        private static NetworkStream _stream;

        //Ŀ��ip
        private static IPAddress _address;
        //�˿ں�
        private static int _port;

        //����������
        private const float HEARTBEAT_TIME = 3;         //���������ͼ��ʱ��
        private static float _timer = HEARTBEAT_TIME;   //�����ϴν�����������ʱ��
        public static bool Received = true;             //�յ�����������

        private static IEnumerator _Connect()
        {
            _client = new TcpClient();

            //�첽����
            IAsyncResult async = _client.BeginConnect(_address, _port, null, null);
            while (!async.IsCompleted)
            {
                Debug.Log("���ӷ�������");
                yield return null;
            }
            //�쳣����
            try
            {
                _client.EndConnect(async);
            }
            catch (Exception ex)
            {
                Info.Instance.Print("���ӷ�����ʧ��:" + ex.Message, true);
                yield break;
            }

            //��ȡͨ����
            try
            {
                _stream = _client.GetStream();
            }
            catch (Exception ex)
            {
                Info.Instance.Print("���ӷ�����ʧ��:" + ex.Message, true);
                yield break;
            }
            if (_stream == null)
            {
                Info.Instance.Print("���ӷ�����ʧ��:������Ϊ��", true);
                yield break;
            }

            _curState = ClientState.Connected;
            _messages = new Queue<byte[]>();
            Info.Instance.Print("���ӷ������ɹ�");

            //�����첽������Ϣ
            NetworkCoroutine.Instance.StartCoroutine(_Send());
            //�����첽������Ϣ
            NetworkCoroutine.Instance.StartCoroutine(_Receive());
            //�����˳��¼�
            NetworkCoroutine.Instance.SetQuitEvent(() => { _client.Close(); _curState = ClientState.None; });
        }

        private static IEnumerator _Send()
        {
            //����������Ϣ
            while (_curState == ClientState.Connected)
            {
                _timer += Time.deltaTime;
                //�д�������Ϣ
                if (_messages.Count > 0)
                {
                    byte[] data = _messages.Dequeue();
                    yield return _Write(data);
                }

                //����������(ÿ��һ��ʱ�������������������)
                if (_timer >= HEARTBEAT_TIME)
                {
                    //���û���յ���һ�η��������Ļظ�
                    if (!Received)
                    {
                        _curState = ClientState.None;
                        Info.Instance.Print("����������ʧ��,�Ͽ�����", true);
                        yield break;
                    }
                    _timer = 0;
                    //��װ��Ϣ
                    byte[] data = _Pack(MessageType.HeartBeat);
                    //������Ϣ
                    yield return _Write(data);

                    //Debug.Log("�ѷ���������");
                }
                yield return null; //��ֹ��ѭ��
            }
        }

        private static IEnumerator _Receive()
        {
            //����������Ϣ
            while (_curState == ClientState.Connected)
            {
                //�������ݰ�����(��������ͻ�����Ҫ�ϸ���һ����Э���ƶ����ݰ�)
                byte[] data = new byte[8];

                uint length;         //��Ϣ����
                MessageType type;   //����
                int receive = 0;    //���ճ���

                //�첽��ȡ
                IAsyncResult async = _stream.BeginRead(data, 0, data.Length, null, null);
                while (!async.IsCompleted)
                {
                    yield return null;
                }
                //�쳣����
                try
                {
                    receive = _stream.EndRead(async);
                }
                catch (Exception ex)
                {
                    _curState = ClientState.None;
                    Info.Instance.Print("��Ϣ��ͷ����ʧ��:" + ex.Message, true);
                    yield break;
                }
                if (receive < data.Length)
                {
                    _curState = ClientState.None;
                    Info.Instance.Print("��Ϣ��ͷ����ʧ��", true);
                    yield break;
                }

                using (MemoryStream stream = new MemoryStream(data))
                {
                    BinaryReader binary = new BinaryReader(stream, Encoding.UTF8); //UTF-8��ʽ����
                    try
                    {
                        length = binary.ReadUInt32();
                        type = (MessageType)binary.ReadUInt32();
                    }
                    catch (Exception)
                    {
                        _curState = ClientState.None;
                        Info.Instance.Print("��Ϣ��ͷ����ʧ��", true);
                        yield break;
                    }
                }

                //����а���
                if (length - 8 > 0)
                {
                    data = new byte[length - 8];
                    //�첽��ȡ
                    async = _stream.BeginRead(data, 0, data.Length, null, null);
                    while (!async.IsCompleted)
                    {
                        yield return null;
                    }
                    //�쳣����
                    try
                    {
                        receive = _stream.EndRead(async);
                    }
                    catch (Exception ex)
                    {
                        _curState = ClientState.None;
                        Info.Instance.Print("��Ϣ��ͷ����ʧ��:" + ex.Message, true);
                        yield break;
                    }
                    if (receive < data.Length)
                    {
                        _curState = ClientState.None;
                        Info.Instance.Print("��Ϣ��ͷ����ʧ��", true);
                        yield break;
                    }
                }
                //û�а���
                else
                {
                    data = new byte[0];
                    receive = 0;
                }

                if (_callBacks.ContainsKey(type))
                {
                    //ִ�лص��¼�
                    CallBack method = _callBacks[type];
                    method(data);
                }
                else
                {
                    Debug.Log("δע������͵Ļص��¼�");
                }
            }
        }

        private static IEnumerator _Write(byte[] data)
        {
            //�������������, �ͻ�����Ȼ���������Ϣ
            if (_curState != ClientState.Connected || _stream == null)
            {
                Info.Instance.Print("����ʧ��,�޷�������Ϣ", true);
                yield break;
            }

            //�첽������Ϣ
            IAsyncResult async = _stream.BeginWrite(data, 0, data.Length, null, null);
            while (!async.IsCompleted)
            {
                yield return null;
            }
            //�쳣����
            try
            {
                _stream.EndWrite(async);
            }
            catch (Exception ex)
            {
                _curState = ClientState.None;
                Info.Instance.Print("������Ϣʧ��:" + ex.Message, true);
            }
        }

        /// <summary>
        /// ���ӷ�����
        /// </summary>
        public static void Connect(string address = null, int port = 8848)
        {
            //�����Ϻ����ظ�����
            if (_curState == ClientState.Connected)
            {
                Info.Instance.Print("�Ѿ������Ϸ�����");
                return;
            }
            if (address == null)
                address = NetworkUtils.GetLocalIPv4();

            //��ȡʧ����ȡ������
            if (!IPAddress.TryParse(address, out _address))
            {
                Info.Instance.Print("IP��ַ����, �����³���", true);
                return;
            }

            _port = port;
            //���������������
            NetworkCoroutine.Instance.StartCoroutine(_Connect()); //(����ip���˿ںųɹ�����֤�����������ɹ�)
        }

        /// <summary>
        /// ע����Ϣ�ص��¼�
        /// </summary>
        public static void Register(MessageType type, CallBack method)
        {
            if (!_callBacks.ContainsKey(type))
                _callBacks.Add(type, method);
            else
                Debug.LogWarning("ע������ͬ�Ļص��¼�");
        }

        /// <summary>
        /// ������Ϣ����
        /// </summary>
        public static void Enqueue(MessageType type, byte[] data = null)
        {
            Info.Instance.Print("byte.length��" + data.Length);
            //�����ݽ��з�װ
            byte[] bytes = _Pack(type, data);

            if (_curState == ClientState.Connected)
            {
                //�������                                 
                _messages.Enqueue(bytes);
            }
        }

        /// <summary>
        /// ��װ����
        /// </summary>
        private static byte[] _Pack(MessageType type, byte[] data = null)
        {
            MessagePacker packer = new MessagePacker();
            if (data != null)
            {
                packer.Add((uint)(8 + data.Length)); //��Ϣ����
                packer.Add((uint)type);              //��Ϣ����
                packer.Add(data);                      //��Ϣ����
            }
            else
            {
                packer.Add((uint)8);                         //��Ϣ����
                packer.Add((uint)type);              //��Ϣ����
            }
            return packer.Package;
        }
    }
}
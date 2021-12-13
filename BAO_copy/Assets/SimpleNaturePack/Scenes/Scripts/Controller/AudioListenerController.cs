using UnityEngine;
using General;
using System;

namespace NetWork
{
    public class AudioListenerController : MonoBehaviour
    {
        [SerializeField]
        public GameObject audiolistener;
        private string NewGamerName;    //玩家名
        private AudioClip micRecord;
        public bool isRecord;
        string device;
        void Awake()
        {
            micRecord = null;
            NewGamerName = NetworkPlayer.Instance.NewGamerName;
            isRecord = false;
            if (NewGamerName == NetworkPlayer.Instance.Name)
            {
                audiolistener.SetActive(true);
                Info.Instance.Print("启用当前" + NewGamerName + "玩家AudioListener"); 
            }
            else
            {
                audiolistener.SetActive(false);
                Info.Instance.Print("关闭" + NewGamerName + "玩家AudioListener");
            }
            //micRecord = Microphone.Start(device, true, 999, 44100);//44100音频采样率   固定格式
            
        }
        private void Update()
        {
            if (NewGamerName == NetworkPlayer.Instance.Name)
            {
                if (Input.GetKeyDown(KeyCode.V))
                {
                    Info.Instance.Print("V键按下");
                    isRecord = !isRecord;
                    if (isRecord)
                    {
                        RecordVoice();
                        Info.Instance.Print("开始录音");
                    }
                    else
                    {
                        StopRecord();
                        Info.Instance.Print("停止录音,发送录音数据");
                    }
                }
            }
        }
        public void RecordVoice()
        {
            if (Microphone.devices.Length > 0)
            {
                Info.Instance.Print("存在可用麦克风"+Microphone.devices[0]);
            }
            device = Microphone.devices[0];//获取设备麦克风
            micRecord = Microphone.Start(device, true, 10, 12000);//44100音频采样率   固定格式
        }
        public void StopRecord()
        {
            if (micRecord != null)
            {
                byte[] data = GetVoiceData(ref micRecord);
                Microphone.End(null);
                micRecord = null;
                NetworkPlayer.Instance.PlayVoiceRequest(data);
            }
        }
        public byte[] GetVoiceData(ref AudioClip recordedClip)
        {
            /*int position = Microphone.GetPosition(device);
            if (position <= 0 || position > recordedClip.samples)
            {
                position = recordedClip.samples;
            }
            float[] soundata = new float[position * recordedClip.channels];
            Info.Instance.Print("soundatalength：" + soundata.Length);
            recordedClip.GetData(soundata, 0);
            recordedClip = AudioClip.Create(recordedClip.name, position,
            recordedClip.channels, recordedClip.frequency, false);
            recordedClip.SetData(soundata, 0);
            int rescaleFactor = 32767;
            byte[] outData = new byte[soundata.Length * 2];
            for(int i = 0; i < soundata.Length; i++)
            {
                short temshort = (short)(soundata[i] * rescaleFactor);
                byte[] temdata = BitConverter.GetBytes(temshort);
                outData[i * 2] = temdata[0];
                outData[i * 2 + 1] = temdata[1];
            }*/
            //string str = System.Text.Encoding.UTF8.GetString(outData);
            return WavUtility.FromAudioClip(recordedClip);
        }
    }
}

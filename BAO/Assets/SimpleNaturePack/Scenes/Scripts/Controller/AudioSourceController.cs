using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using General;
using UnityEngine.Networking;

namespace NetWork
{
    public class AudioSourceController : MonoBehaviour
    {
        public AudioSource Source;
        private string GamerName;   //�����
        // Start is called before the first frame update
        void Awake()
        {
            GamerName = NetworkPlayer.Instance.NewGamerName;
            NetworkPlayer.Instance.Audios.Add(GamerName, this);
        }

        public void PlayVoice(byte[] data)
        {
            Info.Instance.Print("׼����������");
            Source.clip = WavUtility.ToAudioClip(data);
            Source.Play();
            Info.Instance.Print("�����������");

        }

        private IEnumerator LoadMusic(string filepath)
        {
            var www = new WWW(filepath);
            yield return www;
            var clip = www.GetAudioClip(); 
            Source.clip = clip;
            Source.Play();
        }
    }
}

using NetWork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Game
{
    public class Init : MonoBehaviour
    {
        private GameObject gameLogin;
        private static GameObject a;
        // Start is called before the first frame update
        void Awake()
        {
            GameObject vedio = (GameObject)Resources.Load("GameStartVedio");
            Instantiate(vedio);
        }
    }
}
using System;
using System.Threading;
using UnityEngine;
using Mirror;
namespace ETModel
{
	public class Init : MonoBehaviour
	{
		public void Awake()
		{

			// 返回选角大厅场景时，删除重复的Global物体
			if (Manager.RPG && Manager.RPG.state == NetworkState.World)
				Destroy(this.gameObject);
		}
		private void Start()
		{
			Time.fixedDeltaTime = 1f / 60;
			// 取得管理组件
			Manager.RPG = RPGManager.singleton;
			this.StartAsync().Coroutine();
		}
		
		private async ETVoid StartAsync()
		{
			try
			{
				SynchronizationContext.SetSynchronizationContext(OneThreadSynchronizationContext.Instance);

				DontDestroyOnLoad(gameObject);
				Game.EventSystem.Add(DLLType.Model, typeof(Init).Assembly);

				Game.Scene.AddComponent<TimerComponent>();
				Game.Scene.AddComponent<GlobalConfigComponent>();
				Game.Scene.AddComponent<NetOuterComponent>();
				Game.Scene.AddComponent<ResourcesComponent>();
				Game.Scene.AddComponent<PlayerComponent>();
				Game.Scene.AddComponent<UnitComponent>();
				Game.Scene.AddComponent<UIComponent>();

				// 下载ab包
				await BundleHelper.DownloadBundle();

				Game.Hotfix.LoadHotfixAssembly();

				// 加载配置
				Game.Scene.GetComponent<ResourcesComponent>().LoadBundle("config.unity3d");
				Game.Scene.AddComponent<ConfigComponent>();
				Game.Scene.GetComponent<ResourcesComponent>().UnloadBundle("config.unity3d");
				Game.Scene.AddComponent<OpcodeTypeComponent>();
				Game.Scene.AddComponent<MessageDispatcherComponent>();
				UnitConfig unitConfig = (UnitConfig)Game.Scene.GetComponent<ConfigComponent>().Get(typeof(UnitConfig), 1001);
				Log.Debug($"config {JsonHelper.ToJson(unitConfig)}");
                Game.Hotfix.GotoHotfix();

               // Game.EventSystem.Run(EventIdType.TestHotfixSubscribMonoEvent, "TestHotfixSubscribMonoEvent");
				Game.EventSystem.Run(EventIdType.GameLogin);
			}
			catch (Exception e)
			{
				Log.Error(e);
			}
		}

		private void Update()
		{
			OneThreadSynchronizationContext.Instance.Update();
			Game.Hotfix.Update?.Invoke();
			Game.EventSystem.Update();
		}

		private void LateUpdate()
		{
			Game.Hotfix.LateUpdate?.Invoke();
			Game.EventSystem.LateUpdate();
		}

		private void OnApplicationQuit()
		{
			Game.Hotfix.OnApplicationQuit?.Invoke();
			Game.Close();
		}
	}
}
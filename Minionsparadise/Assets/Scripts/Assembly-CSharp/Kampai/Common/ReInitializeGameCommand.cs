namespace Kampai.Common
{
	public class ReInitializeGameCommand : global::strange.extensions.command.impl.Command
	{
		private static readonly global::System.Collections.Generic.IList<string> DO_NOT_DESTROY_GOS = new global::System.Collections.Generic.List<string> { "UnityFacebookSDKPlugin", "PlayGames_QueueRunner" };

		[Inject]
		public string levelToLoad { get; set; }



		[Inject]
		public global::Kampai.Game.EnvironmentState state { get; set; }

		[Inject]
		public global::Kampai.Splash.IDownloadService downloadService { get; set; }

		[Inject]
		public global::Kampai.Game.ITimeService timeService { get; set; }

		[Inject]
		public global::Kampai.Main.IAssetsPreloadService assetsPreloadService { get; set; }

		public override void Execute()
		{
			state.DisplayOn = false;
			state.EnvironmentBuilt = false;
			state.GridConstructed = false;
			global::Kampai.Util.Native.LogInfo(string.Format("ReInitializeGame, realtimeSinceStartup: {0}", timeService.RealtimeSinceStartup()));
			downloadService.Shutdown();
			global::Kampai.Util.KampaiView.ClearContextCache();
			assetsPreloadService.StopAssetsPreload();
		global::UnityEngine.Object[] array = global::UnityEngine.Object.FindObjectsByType(typeof(global::UnityEngine.GameObject), global::UnityEngine.FindObjectsSortMode.None);
			for (int i = 0; i < array.Length; i++)
			{
				global::UnityEngine.GameObject gameObject = (global::UnityEngine.GameObject)array[i];
				string name = gameObject.name;
				if (name == null || !DO_NOT_DESTROY_GOS.Contains(name))
				{
					global::UnityEngine.Object.Destroy(gameObject);
				}
			}

			Go.killAllTweens();
			global::Kampai.Util.KampaiResources.ClearCache();
			global::Kampai.Main.LoadState.Set(global::Kampai.Main.LoadStateType.BOOTING);
			string sceneName = (string.IsNullOrEmpty(levelToLoad) ? "Initialize" : levelToLoad);
			global::UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
		}
	}
}

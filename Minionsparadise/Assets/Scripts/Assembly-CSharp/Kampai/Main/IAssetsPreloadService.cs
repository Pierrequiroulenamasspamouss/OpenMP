namespace Kampai.Main
{
	public interface IAssetsPreloadService
	{
		bool IsPreloading { get; }

		void AddAssetToPreloadQueue(global::Kampai.Main.PreloadableAsset asset);

		void PreloadAllAssets();

		void StopAssetsPreload();

		void SetIntegrationStepLength(int msec);
	}
}

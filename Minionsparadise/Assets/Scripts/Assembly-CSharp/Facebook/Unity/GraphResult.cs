namespace Discord.Unity
{
	internal class GraphResult : global::Discord.Unity.ResultBase, global::Discord.Unity.IGraphResult, global::Discord.Unity.IResult
	{
		public global::System.Collections.Generic.IList<object> ResultList { get; private set; }

		public global::UnityEngine.Texture2D Texture { get; private set; }

		internal GraphResult(global::UnityEngine.Networking.UnityWebRequest result)
			: base(new global::Discord.Unity.ResultContainer(result.downloadHandler.text), result.error, false)
		{
			Init(RawResult);
		}

		private void Init(string rawResult)
		{
			if (string.IsNullOrEmpty(rawResult))
			{
				return;
			}
			object obj = global::Discord.MiniJSON.Json.Deserialize(RawResult);
			global::System.Collections.Generic.IDictionary<string, object> dictionary = obj as global::System.Collections.Generic.IDictionary<string, object>;
			if (dictionary != null)
			{
				ResultDictionary = dictionary;
				return;
			}
			global::System.Collections.Generic.IList<object> list = obj as global::System.Collections.Generic.IList<object>;
			if (list != null)
			{
				ResultList = list;
			}
		}
	}
}

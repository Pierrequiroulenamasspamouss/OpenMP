namespace Kampai.Game
{
	public class TimedSocialEventDefinition : global::Kampai.Game.Definition
	{
		public override int TypeCode
		{
			get
			{
				return 1147;
			}
		}

		public int StartTime { get; set; }

		public int FinishTime { get; set; }

		private int _originalStartTime;
		public int OriginalStartTime
		{
			get { return _originalStartTime == 0 ? StartTime : _originalStartTime; }
			set { _originalStartTime = value; }
		}

		private int _originalFinishTime;
		public int OriginalFinishTime
		{
			get { return _originalFinishTime == 0 ? FinishTime : _originalFinishTime; }
			set { _originalFinishTime = value; }
		}

		public int MaxTeamSize { get; set; }

		public global::System.Collections.Generic.IList<global::Kampai.Game.SocialEventOrderDefinition> Orders { get; set; }

		public int RewardTransaction { get; set; }

		public override void Write(global::System.IO.BinaryWriter writer)
		{
			base.Write(writer);
			writer.Write(StartTime);
			writer.Write(FinishTime);
			writer.Write(MaxTeamSize);
			global::Kampai.Util.BinarySerializationUtil.WriteList(writer, global::Kampai.Util.BinarySerializationUtil.WriteSocialEventOrderDefinition, Orders);
			writer.Write(RewardTransaction);
		}

		public override void Read(global::System.IO.BinaryReader reader)
		{
			base.Read(reader);
			StartTime = reader.ReadInt32();
			FinishTime = reader.ReadInt32();
			MaxTeamSize = reader.ReadInt32();
			Orders = global::Kampai.Util.BinarySerializationUtil.ReadList(reader, global::Kampai.Util.BinarySerializationUtil.ReadSocialEventOrderDefinition, Orders);
			RewardTransaction = reader.ReadInt32();
		}

		protected override bool DeserializeProperty(string propertyName, global::Newtonsoft.Json.JsonReader reader, JsonConverters converters)
		{
			switch (propertyName)
			{
			case "STARTTIME":
				reader.Read();
				StartTime = global::System.Convert.ToInt32(reader.Value);
				break;
			case "FINISHTIME":
				reader.Read();
				FinishTime = global::System.Convert.ToInt32(reader.Value);
				break;
			case "MAXTEAMSIZE":
				reader.Read();
				MaxTeamSize = global::System.Convert.ToInt32(reader.Value);
				break;
			case "ORDERS":
				reader.Read();
				Orders = global::Kampai.Util.ReaderUtil.PopulateList(reader, converters, global::Kampai.Util.ReaderUtil.ReadSocialEventOrderDefinition, Orders);
				break;
			case "REWARDTRANSACTION":
				reader.Read();
				RewardTransaction = global::System.Convert.ToInt32(reader.Value);
				break;
			default:
				return base.DeserializeProperty(propertyName, reader, converters);
			}
			return true;
		}

		public virtual global::Kampai.Game.Transaction.TransactionDefinition GetReward(global::Kampai.Game.IDefinitionService definitionService)
		{
			return definitionService.Get<global::Kampai.Game.Transaction.TransactionDefinition>(RewardTransaction);
		}
	}
}

namespace Kampai.Game.Trigger
{
	public abstract class TriggerInstance<TDefinition> : global::Kampai.Game.Trigger.IIsTriggerable, global::Kampai.Game.IGameTimeTracker, global::Kampai.Game.Trigger.TriggerInstance, global::System.IComparable<global::Kampai.Game.Trigger.TriggerInstance>, global::System.IEquatable<global::Kampai.Game.Trigger.TriggerInstance>, global::System.IComparable<global::Kampai.Game.Trigger.TriggerInstance<TDefinition>>, global::System.IEquatable<global::Kampai.Game.Trigger.TriggerInstance<TDefinition>>, global::Kampai.Util.IFastJSONDeserializable, global::Kampai.Util.IFastJSONSerializable where TDefinition : global::Kampai.Game.Trigger.TriggerDefinition
	{
		private global::System.Collections.Generic.IList<int> m_recievedRewardIds = new global::System.Collections.Generic.List<int>();

		global::Kampai.Game.Trigger.TriggerDefinition global::Kampai.Game.Trigger.TriggerInstance.Definition
		{
			get
			{
				return Definition;
			}
		}

		public int ID
		{
			get
			{
				int result;
				if (Definition == null)
				{
					result = -1;
				}
				else
				{
					TDefinition definition = Definition;
					result = definition.ID;
				}
				return result;
			}
		}

		public global::System.Collections.Generic.IList<int> RecievedRewardIds
		{
			get
			{
				return m_recievedRewardIds;
			}
			set
			{
				m_recievedRewardIds = value;
			}
		}

		public int StartGameTime { get; set; }

		public TDefinition Definition { get; protected set; }

		protected TriggerInstance(TDefinition definition)
		{
			Definition = definition;
			if (definition != null)
			{
			}
		}

		public virtual object Deserialize(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters = null)
		{
			if (reader.TokenType == global::Newtonsoft.Json.JsonToken.None)
			{
				reader.Read();
			}
			global::Kampai.Util.ReaderUtil.EnsureToken(global::Newtonsoft.Json.JsonToken.StartObject, reader);
			while (reader.Read())
			{
				switch (reader.TokenType)
				{
				case global::Newtonsoft.Json.JsonToken.PropertyName:
				{
					string propertyName = ((string)reader.Value).ToUpper();
					if (!DeserializeProperty(propertyName, reader, converters))
					{
						reader.Skip();
					}
					break;
				}
				case global::Newtonsoft.Json.JsonToken.EndObject:
					return this;
				default:
					reader.Skip();
					break;
case global::Newtonsoft.Json.JsonToken.Comment:
					break;
				}
			}
			return this;
		}

		protected virtual bool DeserializeProperty(string propertyName, global::Newtonsoft.Json.JsonReader reader, JsonConverters converters)
		{
			switch (propertyName)
			{
			case "STARTGAMETIME":
				reader.Read();
				StartGameTime = global::System.Convert.ToInt32(reader.Value);
				break;
			default:
				return false;
			}
			return true;
		}

		public virtual void Serialize(global::Newtonsoft.Json.JsonWriter writer)
		{
			writer.WriteStartObject();
			SerializeProperties(writer);
			writer.WriteEndObject();
		}

		protected virtual void SerializeProperties(global::Newtonsoft.Json.JsonWriter writer)
		{
			global::Kampai.Game.Trigger.FastTriggerInstanceSerializationHelper.SerializeTriggerInstanceData(writer, this);
		}

		public void OnDefinitionHotSwap(global::Kampai.Game.Trigger.TriggerDefinition definition)
		{
			Definition = definition as TDefinition;
		}

		public abstract void RewardPlayer(global::Kampai.Game.IPlayerService playerService);

		public virtual bool IsTriggered(global::strange.extensions.context.api.ICrossContextCapable gameContext)
		{
			if (StartGameTime > 0)
			{
				return false;
			}
			TDefinition definition = Definition;
			return definition.IsTriggered(gameContext);
		}

		public override string ToString()
		{
			return string.Format("{0}, ID: {1}, Start GameTime: {2}, {3}", base.ToString(), ID, StartGameTime, Definition);
		}

		public override bool Equals(object obj)
		{
			if (object.ReferenceEquals(null, obj))
			{
				return false;
			}
			if (object.ReferenceEquals(this, obj))
			{
				return true;
			}
			global::Kampai.Game.Trigger.TriggerInstance<TDefinition> triggerInstance = obj as global::Kampai.Game.Trigger.TriggerInstance<TDefinition>;
			return !object.ReferenceEquals(null, triggerInstance) && CompareTo(triggerInstance) == 0;
		}

		public override int GetHashCode()
		{
			return new { Definition, StartGameTime, ID }.GetHashCode();
		}

		public bool Equals(global::Kampai.Game.Trigger.TriggerInstance<TDefinition> obj)
		{
			return obj != null && Equals((object)obj);
		}

		public bool Equals(global::Kampai.Game.Trigger.TriggerInstance obj)
		{
			return obj != null && Equals((object)obj);
		}

		public int CompareTo(global::Kampai.Game.Trigger.TriggerInstance other)
		{
			if (other == null)
			{
				return 1;
			}
			TDefinition definition = Definition;
			int num = definition.CompareTo(other.Definition);
			if (num != 0)
			{
				return num;
			}
			int num2 = other.StartGameTime.CompareTo(StartGameTime);
			if (num2 != 0)
			{
				return num2;
			}
			return ID.CompareTo(other.ID);
		}

		public virtual int CompareTo(global::Kampai.Game.Trigger.TriggerInstance<TDefinition> other)
		{
			return (other == null) ? 1 : CompareTo((global::Kampai.Game.Trigger.TriggerInstance)other);
		}
	}
	[global::Kampai.Util.RequiresJsonConverter]
	[global::Kampai.Util.Serializer("FastTriggerInstanceSerializationHelper.SerializeTriggerInstanceData")]
	public interface TriggerInstance : global::Kampai.Game.Trigger.IIsTriggerable, global::Kampai.Game.IGameTimeTracker, global::System.IComparable<global::Kampai.Game.Trigger.TriggerInstance>, global::System.IEquatable<global::Kampai.Game.Trigger.TriggerInstance>, global::Kampai.Util.IFastJSONDeserializable, global::Kampai.Util.IFastJSONSerializable
	{
		int ID { get; }

		global::System.Collections.Generic.IList<int> RecievedRewardIds { get; set; }

		global::Kampai.Game.Trigger.TriggerDefinition Definition { get; }

		void OnDefinitionHotSwap(global::Kampai.Game.Trigger.TriggerDefinition definition);
	}
}

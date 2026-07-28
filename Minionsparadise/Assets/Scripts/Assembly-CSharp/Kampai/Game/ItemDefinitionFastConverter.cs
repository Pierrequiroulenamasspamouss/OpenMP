namespace Kampai.Game
{
	public class ItemDefinitionFastConverter : global::Kampai.Util.FastJsonCreationConverter<global::Kampai.Game.ItemDefinition>
	{
		private ItemType.ItemTypeIdentifier itemType;

		public override global::Kampai.Game.ItemDefinition ReadJson(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters)
		{
			if (reader.TokenType == global::Newtonsoft.Json.JsonToken.Null)
			{
				return null;
			}
			global::Newtonsoft.Json.Linq.JObject jObject = global::Newtonsoft.Json.Linq.JObject.Load(reader);
			global::Newtonsoft.Json.Linq.JProperty jProperty = jObject.Property("type");
			if (jProperty != null)
			{
				string value = jProperty.Value.ToString();
				itemType = (ItemType.ItemTypeIdentifier)(int)global::System.Enum.Parse(typeof(ItemType.ItemTypeIdentifier), value);
			}
			else
			{
				itemType = ItemType.ItemTypeIdentifier.DEFAULT;
			}
			reader = jObject.CreateReader();
			return base.ReadJson(reader, converters);
		}

		public override global::Kampai.Game.ItemDefinition Create()
		{
			switch (itemType)
			{
			case ItemType.ItemTypeIdentifier.DEFAULT:
				return new global::Kampai.Game.ItemDefinition();
			case ItemType.ItemTypeIdentifier.INGREDIENTS:
				return new global::Kampai.Game.IngredientsItemDefinition();
			case ItemType.ItemTypeIdentifier.DYNAMIC_INGREDIENTS:
				return new global::Kampai.Game.DynamicIngredientsDefinition();
			case ItemType.ItemTypeIdentifier.COSTUME:
				return new global::Kampai.Game.CostumeItemDefinition();
			case ItemType.ItemTypeIdentifier.UNLOCK:
				return new global::Kampai.Game.UnlockDefinition();
			case ItemType.ItemTypeIdentifier.BRIDGE:
				return new global::Kampai.Game.BridgeDefinition();
			case ItemType.ItemTypeIdentifier.DROP:
				return new global::Kampai.Game.DropItemDefinition();
			case ItemType.ItemTypeIdentifier.PARTY_ANIMATION_ITEM:
				return new global::Kampai.Game.PartyFavorAnimationItemDefinition();
			case ItemType.ItemTypeIdentifier.SPECIAL_EVENT:
				return new global::Kampai.Game.SpecialEventItemDefinition();
			default:
				return new global::Kampai.Game.ItemDefinition();
			}
		}
	}
}

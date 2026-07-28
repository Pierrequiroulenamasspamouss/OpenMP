namespace Kampai.Game
{
	public class InventoryConverter : global::Newtonsoft.Json.Converters.CustomCreationConverter<global::Kampai.Game.Instance>
	{
		private global::Kampai.Game.IDefinitionService definitionService;

		private global::Kampai.Util.IKampaiLogger logger;

		private global::Kampai.Game.Definition def;

		private bool isSaleItem;

		public InventoryConverter(global::Kampai.Game.IDefinitionService definitionService, global::Kampai.Util.IKampaiLogger logger)
		{
			this.definitionService = definitionService;
			this.logger = logger;
		}

		public override object ReadJson(global::Newtonsoft.Json.JsonReader reader, global::System.Type objectType, object existingValue, global::Newtonsoft.Json.JsonSerializer serializer)
		{
			if (reader.TokenType != global::Newtonsoft.Json.JsonToken.Null)
			{
				def = null;
				global::Newtonsoft.Json.Linq.JObject jObject = global::Newtonsoft.Json.Linq.JObject.Load(reader);
				global::Newtonsoft.Json.Linq.JProperty jProperty = ((jObject.Property("def") != null) ? jObject.Property("def") : jObject.Property("Definition"));
				isSaleItem = jObject.Property("BuyQuantity") == null;
				if (jProperty != null)
				{
					global::Newtonsoft.Json.Linq.JProperty jProperty2 = jObject.Property("isDynamicSaleDefinition");
					if (jProperty2 != null && jProperty2.Value.ToString().ToLower().Equals("true"))
					{
						def = global::Newtonsoft.Json.JsonConvert.DeserializeObject<global::Kampai.Game.SalePackDefinition>(jProperty.Value.ToString());
					}
					else
					{
						int num = global::Newtonsoft.Json.Linq.LinqExtensions.Value<int>(jProperty.Value);
						def = definitionService.Get(num);
						if (num == 77777)
						{
							global::Newtonsoft.Json.Linq.JProperty jProperty3 = jObject.Property("dynamicDefinition");
							def = global::Newtonsoft.Json.JsonConvert.DeserializeObject<global::Kampai.Game.DynamicQuestDefinition>(jProperty3.Value.ToString());
						}
					}
				}
				reader = jObject.CreateReader();
			}
			return base.ReadJson(reader, objectType, existingValue, serializer);
		}

		public override global::Kampai.Game.Instance Create(global::System.Type objectType)
		{
			if (def == null)
			{
				logger.Error("InventoryConverter.Create(): null definition, using fallback."); def = new global::Kampai.Game.ItemDefinition { ID = 999999 };
			}
			global::Kampai.Util.IBuilder<global::Kampai.Game.Instance> builder = def as global::Kampai.Util.IBuilder<global::Kampai.Game.Instance>;
			if (builder != null)
			{
				return builder.Build();
			}
			global::System.Type type = def.GetType();
			if (type == typeof(global::Kampai.Game.MarketplaceItemDefinition))
			{
				if (isSaleItem)
				{
					return new global::Kampai.Game.MarketplaceSaleItem((global::Kampai.Game.MarketplaceItemDefinition)def);
				}
				return new global::Kampai.Game.MarketplaceBuyItem((global::Kampai.Game.MarketplaceItemDefinition)def);
			}
			if (type == typeof(global::Kampai.Game.VillainLairDefinition))
			{
				return new global::Kampai.Game.VillainLair((global::Kampai.Game.VillainLairDefinition)def);
			}
			logger.Error("Unable to map inventory type of {0}", type);
			logger.Error("InventoryConverter.Create: Unable to map inventory type of {0}, using fallback.", type); return new global::Kampai.Game.Item(new global::Kampai.Game.ItemDefinition { ID = (def != null ? def.ID : 0) });
		}
	}
}

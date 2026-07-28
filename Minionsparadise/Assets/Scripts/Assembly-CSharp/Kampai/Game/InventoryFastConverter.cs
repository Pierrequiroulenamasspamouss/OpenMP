namespace Kampai.Game
{
	public class InventoryFastConverter : global::Kampai.Util.FastJsonCreationConverter<global::Kampai.Game.Instance>
	{
		private global::Kampai.Game.IDefinitionService definitionService;

		private global::Kampai.Util.IKampaiLogger logger;

		private int defId = -1;

		private int id = -1;

		private global::Kampai.Game.Definition def;

		private bool isSaleItem;

		public InventoryFastConverter(global::Kampai.Game.IDefinitionService definitionService, global::Kampai.Util.IKampaiLogger logger)
		{
			this.definitionService = definitionService;
			this.logger = logger;
		}

		public override global::Kampai.Game.Instance ReadJson(global::Newtonsoft.Json.JsonReader reader, JsonConverters converters)
		{
			if (reader.TokenType == global::Newtonsoft.Json.JsonToken.Null)
			{
				return null;
			}
			def = null;
			global::Newtonsoft.Json.Linq.JObject jObject = global::Newtonsoft.Json.Linq.JObject.Load(reader);
			global::Newtonsoft.Json.Linq.JProperty jProperty = jObject.Property("def");
			global::Newtonsoft.Json.Linq.JProperty jProperty2 = ((jProperty != null) ? jProperty : jObject.Property("Definition"));
			isSaleItem = jObject.Property("BuyQuantity") == null;
			if (jProperty2 != null)
			{
				global::Newtonsoft.Json.Linq.JProperty jProperty3 = jObject.Property("isDynamicSaleDefinition");
				if (jProperty3 != null)
				{
					if (jProperty3.Value.ToString().ToLower().Equals("true"))
					{
						if (jProperty2.Value.Type == global::Newtonsoft.Json.Linq.JTokenType.Object)
						{
							global::Newtonsoft.Json.Linq.JObject jObject2 = (global::Newtonsoft.Json.Linq.JObject)jProperty2.Value;
							global::Newtonsoft.Json.Linq.JProperty jProperty4 = jObject2.Property("PlatformStoreSku");
							if (jProperty4 != null && jProperty4.Value.Type != global::Newtonsoft.Json.Linq.JTokenType.Array)
							{
								global::Newtonsoft.Json.Linq.JToken value = jProperty4.Value;
								global::Newtonsoft.Json.Linq.JArray jArray = new global::Newtonsoft.Json.Linq.JArray();
								jArray.Add(value);
								jProperty4.Value.Replace(jArray);
							}
						}
						def = global::Newtonsoft.Json.JsonConvert.DeserializeObject<global::Kampai.Game.SalePackDefinition>(jProperty2.Value.ToString());
					}
					else
					{
						defId = global::Newtonsoft.Json.Linq.LinqExtensions.Value<int>(jProperty2.Value);
						if (!definitionService.TryGet<global::Kampai.Game.Definition>(defId, out def))
						{
							def = new global::Kampai.Game.SalePackDefinition
							{
								ID = defId,
								Type = global::Kampai.Game.SalePackType.Store
							};
						}
					}
				}
				else
				{
					defId = global::Newtonsoft.Json.Linq.LinqExtensions.Value<int>(jProperty2.Value);
					def = definitionService.Get(defId);
					if (defId == 77777)
					{
						global::Newtonsoft.Json.Linq.JProperty jProperty5 = jObject.Property("dynamicDefinition");
						def = global::Newtonsoft.Json.JsonConvert.DeserializeObject<global::Kampai.Game.DynamicQuestDefinition>(jProperty5.Value.ToString());
					}
				}
			}
			reader = jObject.CreateReader();
			return base.ReadJson(reader, converters);
		}

		public override global::Kampai.Game.Instance Create()
		{
			if (def == null)
			{
				logger.Error("InventoryFastConverter.Create(): null definition id={0} defId={1}, creating fallback item def.", id, defId); def = new global::Kampai.Game.ItemDefinition { ID = (defId > 0 ? defId : 999999) };
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
			if (type == typeof(global::Kampai.Game.MarketplaceRefreshTimerDefinition))
			{
				return new global::Kampai.Game.MarketplaceRefreshTimer((global::Kampai.Game.MarketplaceRefreshTimerDefinition)def);
			}
			if (type == typeof(global::Kampai.Game.MinionPartyDefinition))
			{
				return new global::Kampai.Game.MinionParty((global::Kampai.Game.MinionPartyDefinition)def);
			}
			if (type == typeof(global::Kampai.Game.AchievementDefinition))
			{
				return new global::Kampai.Game.Achievement((global::Kampai.Game.AchievementDefinition)def);
			}
			if (type == typeof(global::Kampai.Game.VillainLairDefinition))
			{
				return new global::Kampai.Game.VillainLair((global::Kampai.Game.VillainLairDefinition)def);
			}
			if (type == typeof(global::Kampai.Game.CurrencyStorePackDefinition))
			{
				global::Kampai.Game.SalePackDefinition salePackDefinition = new global::Kampai.Game.SalePackDefinition();
				salePackDefinition.ID = def.ID;
				salePackDefinition.Type = global::Kampai.Game.SalePackType.Store;
				return new global::Kampai.Game.Sale(salePackDefinition);
			}
			logger.Error("Unable to map inventory type of {0}", type);
			logger.Error("InventoryFastConverter.Create: Unable to map inventory type of {0}, using fallback Item.", type); return new global::Kampai.Game.Item(new global::Kampai.Game.ItemDefinition { ID = (def != null ? def.ID : 0) });
		}
	}
}

namespace Kampai.Game
{
	public class PopulateEnvironmentCommand : global::strange.extensions.command.impl.Command
	{
		public global::Kampai.Util.IKampaiLogger logger = global::Elevation.Logging.LogManager.GetClassLogger("PopulateEnvironmentCommand") as global::Kampai.Util.IKampaiLogger;

		private global::UnityEngine.GameObject parent;

		private global::UnityEngine.Color darkGreen = new global::UnityEngine.Color(0f, 0.5f, 0f, 1f);

		private global::UnityEngine.Color darkBlue = new global::UnityEngine.Color(0f, 0f, 0.5f, 1f);

		private global::UnityEngine.Color lightBlue = new global::UnityEngine.Color(0f, 1f, 1f, 1f);

		private global::UnityEngine.Color brown = new global::UnityEngine.Color(0.5f, 0.4f, 0.3f, 1f);

		private global::UnityEngine.Color green = new global::UnityEngine.Color(0f, 1f, 0f, 1f);

		private global::UnityEngine.Color red = new global::UnityEngine.Color(1f, 0f, 0f, 1f);

		private global::UnityEngine.Color blue = new global::UnityEngine.Color(0f, 0f, 1f, 1f);

		private global::UnityEngine.Color locked = new global::UnityEngine.Color(1f, 1f, 0f, 1f);

		private global::UnityEngine.Color sidewalk = new global::UnityEngine.Color(0.8f, 0f, 1f, 1f);

		private global::UnityEngine.Color occupied = new global::UnityEngine.Color(0.6f, 0f, 0f, 1f);

		private global::UnityEngine.Color error = new global::UnityEngine.Color(1f, 0f, 0.8f, 1f);

		[Inject]
		public global::Kampai.Game.Environment environment { get; set; }

		[Inject]
		public global::Kampai.Game.EnvironmentBuilder environmentBuilder { get; set; }

		[Inject]
		public global::Kampai.Game.DebugUpdateGridSignal DebugUpdateGridSignal { get; set; }

		[Inject]
		public global::Kampai.Game.IDefinitionService definitionService { get; set; }

		[Inject]
		public bool display { get; set; }

		[Inject]
		public global::Kampai.Game.EnvironmentState state { get; set; }

		public override void Execute()
		{
			logger.EventStart("PopulateEnvironmentCommand.Execute");
			if (!state.EnvironmentBuilt)
			{
				global::System.Collections.Generic.IList<string> environemtDefinition = definitionService.GetEnvironemtDefinition();
				definitionService.ReclaimEnfironmentDefinitions();
				global::System.Collections.Generic.Dictionary<string, object> dictionary = new global::System.Collections.Generic.Dictionary<string, object>();
				dictionary.Add("x", environemtDefinition[0].Length);
				dictionary.Add("y", environemtDefinition.Count);
				global::System.Collections.Generic.List<int> list = new global::System.Collections.Generic.List<int>(environemtDefinition[0].Length * environemtDefinition.Count);
				foreach (string item in environemtDefinition)
				{
					foreach (char c in item)
					{
						if (c >= '0' && c <= '9')
						{
							list.Add(c - 48);
						}
					}
				}
				dictionary.Add("definitionLayout", list);
				environmentBuilder.Build(dictionary);
				state.EnvironmentBuilt = true;
			}
			else
			{
				logger.Debug("PopulateEnvironmentCommand: Environment already built.");
			}
			if (display)
			{
				logger.Debug("PopulateEnvironmentCommand: Setting up grid and textures.");
				SetupGrid();
				UpdateTextures();
				DebugUpdateGridSignal.AddListener(UpdateTextures);
				state.DisplayOn = true;
			}
			else if (state.DisplayOn)
			{
				RemoveGrid();
				DebugUpdateGridSignal.RemoveListener(UpdateTextures);
				state.DisplayOn = false;
			}
			logger.EventStop("PopulateEnvironmentCommand.Execute");
		}

		private void SetupGrid()
		{
			if (!state.GridConstructed || state.EnvironmentObject == null)
			{
				logger.EventStart("PopulateEnvironmentCommand.SetupGrid");
				global::UnityEngine.Shader shader = global::UnityEngine.Shader.Find("Kampai/Standard/Texture");
				if (shader == null)
				{
					shader = global::UnityEngine.Shader.Find("Diffuse");
				}
				parent = global::UnityEngine.GameObject.CreatePrimitive(global::UnityEngine.PrimitiveType.Quad);
				parent.isStatic = true;
				parent.name = "Environment";
				global::UnityEngine.Object.Destroy(parent.GetComponent<global::UnityEngine.Collider>());
				parent.transform.localScale = new global::UnityEngine.Vector3(250f, 250f, 0f);
				parent.transform.Rotate(new global::UnityEngine.Vector3(90f, 0f, 0f));
				global::UnityEngine.Renderer component = parent.GetComponent<global::UnityEngine.Renderer>();
				component.shadowCastingMode = global::UnityEngine.Rendering.ShadowCastingMode.Off;
				component.receiveShadows = false;
				component.material = new global::UnityEngine.Material(shader);
				global::UnityEngine.Material material = component.material;
				material.EnableKeyword("TEXTURE_ALPHA");
				material.SetFloat("_Alpha", 0f);
				material.color = new global::UnityEngine.Color(1f, 1f, 1f, 0.5f);
				material.SetFloat("_OffsetFactor", -6f);
				material.SetFloat("_OffsetUnits", -1f);
				int length = environment.Definition.DefinitionGrid.GetLength(0);
				int length2 = environment.Definition.DefinitionGrid.GetLength(1);
				parent.transform.position = new global::UnityEngine.Vector3((float)length / 2f - 0.5f, 0f, (float)length2 / 2f - 0.5f);
				global::UnityEngine.Texture2D texture2D = new global::UnityEngine.Texture2D(length, length2, global::UnityEngine.TextureFormat.ARGB32, false);
				texture2D.filterMode = global::UnityEngine.FilterMode.Point;
				texture2D.wrapMode = global::UnityEngine.TextureWrapMode.Clamp;
				material.mainTexture = texture2D;
				state.GridConstructed = true;
				state.EnvironmentObject = parent;
				AddGridLines(parent.transform, length, length2);
				logger.EventStop("PopulateEnvironmentCommand.SetupGrid");
			}
			else if (state.EnvironmentObject != null)
			{
				state.EnvironmentObject.SetActive(true);
			}
		}

		private void RemoveGrid()
		{
			if (state.EnvironmentObject != null)
			{
				state.EnvironmentObject.SetActive(false);
			}
		}

		private void AddGridLines(global::UnityEngine.Transform parent, int rows, int cols)
		{
			global::UnityEngine.Color color = new global::UnityEngine.Color(0.4f, 0.4f, 0.4f, 1f);
			global::UnityEngine.Shader shader = global::UnityEngine.Shader.Find("Kampai/Standard/Texture");
			if (shader == null)
			{
				shader = global::UnityEngine.Shader.Find("Diffuse");
			}
			global::UnityEngine.Material material = new global::UnityEngine.Material(shader);
			material.color = color;
			material.SetFloat("_OffsetFactor", -6f);
			material.SetFloat("_OffsetUnits", -6f);
			global::UnityEngine.GameObject gameObject = new global::UnityEngine.GameObject("Grid Lines");
			gameObject.transform.parent = parent;
			global::UnityEngine.LineRenderer lineRenderer = gameObject.AddComponent<global::UnityEngine.LineRenderer>();
			lineRenderer.material = material;
			lineRenderer.startWidth = 0.025f;
			lineRenderer.endWidth = 0.025f;
			lineRenderer.startColor = color;
			lineRenderer.endColor = color;
			int num = 0;
			lineRenderer.positionCount = 2 * cols + 2;
			for (int i = 0; i <= cols; i++)
			{
				float x = (float)i - 0.5f;
				float z = -0.5f;
				float z2 = (float)rows - 0.5f;
				if (i % 2 == 1)
				{
					z = (float)rows - 0.5f;
					z2 = -0.5f;
				}
				lineRenderer.SetPosition(num++, new global::UnityEngine.Vector3(x, 0f, z));
				lineRenderer.SetPosition(num++, new global::UnityEngine.Vector3(x, 0f, z2));
			}
			gameObject = new global::UnityEngine.GameObject("Grid Lines");
			gameObject.transform.parent = parent;
			lineRenderer = gameObject.AddComponent<global::UnityEngine.LineRenderer>();
			lineRenderer.material = material;
			lineRenderer.startWidth = 0.025f;
			lineRenderer.endWidth = 0.025f;
			lineRenderer.startColor = color;
			lineRenderer.endColor = color;
			num = 0;
			lineRenderer.positionCount = 2 * rows + 2;
			for (int j = 0; j <= rows; j++)
			{
				float z3 = (float)j - 0.5f;
				float x2 = -0.5f;
				float x3 = (float)cols - 0.5f;
				if (j % 2 == 1)
				{
					x2 = (float)cols - 0.5f;
					x3 = -0.5f;
				}
				lineRenderer.SetPosition(num++, new global::UnityEngine.Vector3(x2, 0f, z3));
				lineRenderer.SetPosition(num++, new global::UnityEngine.Vector3(x3, 0f, z3));
			}
		}

		private void UpdateTextures()
		{
			if (parent == null)
			{
				return;
			}
			logger.EventStart("PopulateEnvironmentCommand.UpdateTextures");
			int length = environment.Definition.DefinitionGrid.GetLength(0);
			int length2 = environment.Definition.DefinitionGrid.GetLength(1);
			global::Kampai.Game.EnvironmentGridSquareDefinition[,] definitionGrid = environment.Definition.DefinitionGrid;
			global::Kampai.Game.EnvironmentGridSquare[,] playerGrid = environment.PlayerGrid;
			global::UnityEngine.Texture2D texture2D = parent.GetComponent<global::UnityEngine.Renderer>().material.mainTexture as global::UnityEngine.Texture2D;
			for (int i = 0; i < length2; i++)
			{
				for (int j = 0; j < length; j++)
				{
					if (definitionGrid[j, i].Water)
					{
						if (definitionGrid[j, i].Usable)
						{
							if (definitionGrid[j, i].Pathable)
							{
								texture2D.SetPixel(j, i, blue);
							}
							else
							{
								texture2D.SetPixel(j, i, lightBlue);
							}
						}
						else if (definitionGrid[j, i].Pathable)
						{
							texture2D.SetPixel(j, i, red);
						}
						else
						{
							texture2D.SetPixel(j, i, darkBlue);
						}
					}
					else if (!definitionGrid[j, i].Usable && !playerGrid[j, i].Occupied)
					{
						if (definitionGrid[j, i].Pathable)
						{
							texture2D.SetPixel(j, i, brown);
						}
						else
						{
							texture2D.SetPixel(j, i, darkGreen);
						}
					}
					else if (!playerGrid[j, i].Unlocked)
					{
						if (playerGrid[j, i].Walkable)
						{
							texture2D.SetPixel(j, i, sidewalk);
						}
						else
						{
							texture2D.SetPixel(j, i, locked);
						}
					}
					else if (playerGrid[j, i].Occupied && playerGrid[j, i].Walkable)
					{
						texture2D.SetPixel(j, i, sidewalk);
					}
					else if (playerGrid[j, i].Occupied)
					{
						texture2D.SetPixel(j, i, occupied);
					}
					else if (!playerGrid[j, i].Occupied)
					{
						texture2D.SetPixel(j, i, green);
					}
					else
					{
						texture2D.SetPixel(j, i, error);
					}
				}
			}
			texture2D.Apply();
			parent.GetComponent<global::UnityEngine.Renderer>().material.mainTexture = texture2D;
			logger.EventStop("PopulateEnvironmentCommand.UpdateTextures");
		}
	}
}

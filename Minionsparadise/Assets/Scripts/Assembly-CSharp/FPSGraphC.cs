public class FPSGraphC : global::UnityEngine.MonoBehaviour
{
	private enum FPSGraphViewMode
	{
		graphing = 0,
		totalperformance = 1,
		assetbreakdown = 2
	}

	private global::UnityEngine.Material mat;

	public bool showPerformanceOnClick = true;

	public bool showFPSNumber;

	public int graphMultiply = 2;

	public global::UnityEngine.Vector2 graphPosition = new global::UnityEngine.Vector2(0f, 0f);

	public int frameHistoryLength = 120;

	public global::UnityEngine.Color CpuColor = new global::UnityEngine.Color(0.20784314f, 8f / 15f, 0.654902f, 1f);

	public global::UnityEngine.Color RenderColor = new global::UnityEngine.Color(0.4392157f, 52f / 85f, 2f / 85f, 1f);

	public global::UnityEngine.Color OtherColor = new global::UnityEngine.Color(0.75686276f, 36f / 85f, 0.003921569f, 1f);

	private readonly int[] numberBits = new int[195]
	{
		1, 1, 1, 0, 1, 1, 1, 0, 1, 1,
		1, 0, 1, 1, 1, 0, 0, 0, 1, 0,
		1, 1, 1, 0, 1, 1, 1, 0, 0, 1,
		0, 0, 1, 1, 1, 0, 0, 0, 1, 1,
		0, 1, 0, 0, 1, 0, 0, 0, 1, 0,
		0, 0, 0, 1, 0, 0, 0, 1, 0, 0,
		0, 1, 0, 1, 0, 1, 0, 0, 1, 0,
		0, 1, 0, 1, 0, 0, 0, 1, 1, 0,
		1, 0, 0, 1, 0, 0, 0, 0, 1, 0,
		0, 1, 1, 0, 1, 1, 1, 0, 1, 1,
		1, 0, 1, 1, 1, 0, 0, 0, 1, 0,
		1, 1, 1, 0, 1, 1, 1, 1, 0, 1,
		0, 1, 1, 0, 0, 1, 0, 1, 0, 0,
		0, 1, 0, 1, 0, 1, 0, 1, 0, 0,
		0, 1, 0, 0, 0, 0, 0, 1, 0, 1,
		0, 1, 0, 1, 0, 1, 1, 1, 1, 0,
		0, 1, 0, 0, 0, 1, 0, 0, 1, 1,
		1, 0, 0, 0, 1, 0, 1, 1, 1, 0,
		1, 1, 1, 0, 1, 1, 1, 0, 1, 1,
		1, 0, 1, 1, 1
	};

	private readonly int[] fpsBits = new int[44]
	{
		1, 0, 0, 0, 1, 0, 0, 0, 1, 1,
		1, 1, 1, 0, 0, 1, 1, 1, 0, 0,
		1, 1, 1, 0, 0, 0, 1, 0, 1, 0,
		1, 1, 0, 0, 1, 1, 0, 1, 1, 1,
		0, 1, 1, 1
	};

	private readonly int[] mbBits = new int[36]
	{
		1, 0, 1, 0, 1, 0, 1, 1, 1, 1,
		0, 1, 0, 1, 0, 1, 0, 1, 1, 1,
		1, 1, 1, 0, 1, 1, 1, 0, 0, 0,
		0, 0, 0, 1, 0, 0
	};

	private readonly int[] gbBits = new int[28]
	{
		1, 1, 1, 0, 1, 1, 1, 1, 0, 1,
		0, 1, 0, 1, 1, 0, 0, 0, 1, 1,
		1, 1, 1, 1, 0, 1, 0, 0
	};

	private readonly float[] graphKeysAlpha = new float[475]
	{
		0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 1f,
		1f, 1f, 0f, 0f, 1f, 0f, 0f, 1f, 0f, 1f,
		0f, 1f, 1f, 1f, 0f, 1f, 0f, 0f, 0f, 0f,
		0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
		0f, 0f, 1f, 1f, 1f, 0f, 1f, 0f, 0f, 0f,
		1f, 1f, 1f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
		0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 1f,
		0f, 0f, 0f, 1f, 1f, 1f, 0f, 1f, 0f, 1f,
		0f, 1f, 1f, 1f, 0f, 1f, 1f, 1f, 0f, 1f,
		0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
		0f, 0f, 0f, 0f, 1f, 0f, 1f, 0f, 0f, 1f,
		0f, 0f, 1f, 0f, 1f, 0f, 1f, 0f, 0f, 0f,
		1f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
		0f, 0f, 0f, 0f, 0f, 0f, 0f, 1f, 0f, 0f,
		0f, 1f, 1f, 1f, 0f, 1f, 0f, 1f, 0f, 0f,
		0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
		0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 1f, 0f,
		0f, 0f, 1f, 0f, 1f, 0f, 1f, 0f, 1f, 0f,
		1f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 0f,
		0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 1f,
		0f, 1f, 0f, 0f, 1f, 0f, 0f, 1f, 0f, 1f,
		0f, 1f, 1f, 1f, 0f, 1f, 1f, 0f, 0f, 0f,
		0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
		0f, 0f, 1f, 0f, 0f, 0f, 1f, 0f, 1f, 0f,
		1f, 0f, 1f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
		0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 1f,
		1f, 0f, 0f, 1f, 1f, 1f, 0f, 1f, 0f, 1f,
		0f, 1f, 0f, 1f, 0f, 1f, 1f, 1f, 0f, 1f,
		1f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
		0f, 0f, 0f, 0f, 1f, 1f, 1f, 0f, 1f, 1f,
		1f, 0f, 1f, 1f, 1f, 0f, 1f, 1f, 1f, 0f,
		1f, 0f, 1f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
		0f, 0f, 0f, 0f, 0f, 0f, 0f, 1f, 1f, 1f,
		0f, 1f, 1f, 1f, 0f, 1f, 0f, 1f, 0f, 0f,
		0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
		0f, 0f, 0f, 0f, 1f, 0f, 1f, 0f, 1f, 1f,
		1f, 0f, 1f, 1f, 1f, 0f, 1f, 1f, 1f, 0f,
		1f, 1f, 1f, 0f, 1f, 0f, 1f, 0f, 0f, 0f,
		0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
		0f, 0f, 0f, 0f, 1f, 0f, 0f, 1f, 0f, 0f,
		0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
		0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
		0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
		0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
		0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
		0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
		0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 0f, 0f,
		0f, 0f, 0f, 0f, 0f
	};

	private global::UnityEngine.Texture2D graphTexture;

	private int graphHeight = 100;

	private int[,] textOverlayMask;

	private float[,] dtHistory;

	private int[] gcHistory;

	private int i;

	private int j;

	private int x;

	private int y;

	private float val;

	private global::UnityEngine.Color32 color32;

	private float maxFrame;

	private float actualFPS;

	private float lastUpdate;

	private float yMulti;

	private float beforeRender;

	private float[] fpsVals = new float[3];

	private float x1;

	private float x2;

	private float y1;

	private float y2;

	private float xOff;

	private float yOff;

	private int[] lineY = new int[3] { 25, 50, 99 };

	private int[] lineY2 = new int[3] { 21, 46, 91 };

	private int[] keyOffX = new int[3] { 61, 34, 1 };

	private string[] splitMb;

	private int first;

	private int second;

	private int third;

	private float lowestDt = 10000f;

	private float highestDt;

	private float totalDt;

	private int totalFrames;

	private float totalGPUTime;

	private float totalCPUTime;

	private float totalOtherTime;

	private float totalTimeElapsed;

	private float totalSeconds;

	private float renderSeconds;

	private float lateSeconds;

	private float dt;

	private int frameCount;

	private int frameIter = 1;

	private float eTotalSeconds;

	private int lastCollectionCount = -1;

	private float mem;

	private bool memGb;

	private static global::UnityEngine.Color[] fpsColors;

	private static global::UnityEngine.Color[] fpsColorsTo;

	private global::UnityEngine.Color lineColor = new global::UnityEngine.Color(1f, 1f, 1f, 0.25f);

	private global::UnityEngine.Color darkenedBack = new global::UnityEngine.Color(0f, 0f, 0f, 0.5f);

	private global::UnityEngine.Color darkenedBackWhole = new global::UnityEngine.Color(0f, 0f, 0f, 0.25f);

	private global::UnityEngine.Color32[] colorsWrite;

	private global::UnityEngine.Rect graphSizeGUI;

	private global::System.Diagnostics.Stopwatch stopWatch;

	private float lastElapsed;

	private float fps;

	private int graphSizeMin;

	private FPSGraphC.FPSGraphViewMode viewMode;

	private int xExtern;

	private int yExtern;

	private int startAt;

	private int yOffset;

	private int xLength;

	private int k;

	private int z;

	private bool hasFormated;

	private global::UnityEngine.Rect wRect;

	private global::UnityEngine.GUIStyle backupLabel;

	private global::UnityEngine.GUIStyle backupButton;

	private global::UnityEngine.GUIStyle h1;

	private global::UnityEngine.GUIStyle h2;

	private global::UnityEngine.GUIStyle h3;

	private global::UnityEngine.GUIStyle guiButton;

	private global::UnityEngine.GUIStyle graphTitles;

	private global::UnityEngine.Vector2[] circleGraphLabels;

	private float w;

	private float h;

	public void CreateLineMaterial()
	{
		mat = new global::UnityEngine.Material(global::UnityEngine.Shader.Find("GUI/Text Shader"));
	}

	private void Awake()
	{
		if (base.gameObject.GetComponent<global::UnityEngine.Camera>() == null)
		{
			global::UnityEngine.Debug.LogWarning("FPS Graph needs to be attached to a Camera object");
		}
		CreateLineMaterial();
		fpsColors = new global::UnityEngine.Color[3] { RenderColor, CpuColor, OtherColor };
		fpsColorsTo = new global::UnityEngine.Color[3]
		{
			fpsColors[0] * 0.7f,
			fpsColors[1] * 0.7f,
			fpsColors[2] * 0.7f
		};
	}

	private global::System.Collections.IEnumerator Start()
	{
		graphSizeMin = ((frameHistoryLength <= 95) ? 95 : frameHistoryLength);
		textOverlayMask = new int[graphHeight, graphSizeMin];
		reset();
		graphTexture = new global::UnityEngine.Texture2D(graphSizeMin, 7, global::UnityEngine.TextureFormat.ARGB32, false, false);
		colorsWrite = new global::UnityEngine.Color32[graphTexture.width * 7];
		graphTexture.filterMode = global::UnityEngine.FilterMode.Point;
		graphSizeGUI = new global::UnityEngine.Rect(0f, 0f, graphTexture.width * graphMultiply, graphTexture.height * graphMultiply);
		addFPSAt(14, 23);
		addFPSAt(14, 48);
		addFPSAt(14, 93);
		if (showFPSNumber)
		{
			addFPSAt(14, 0);
		}
		for (int x = 0; x < graphTexture.width; x++)
		{
			for (int y = 0; y < 7; y++)
			{
				global::UnityEngine.Color color = global::UnityEngine.Color.white;
				if (x < 95 && y < 5)
				{
					color.a = graphKeysAlpha[y * 95 + x];
				}
				else
				{
					color.a = 0f;
				}
				graphTexture.SetPixel(x, y, color);
				colorsWrite[y * graphTexture.width + x] = color;
			}
		}
		graphTexture.Apply();
		while (true)
		{
			yield return new global::UnityEngine.WaitForEndOfFrame();
			eTotalSeconds = (float)stopWatch.Elapsed.TotalSeconds;
			dt = eTotalSeconds - lastElapsed;
		}
	}

	private void reset()
	{
		dtHistory = new float[3, frameHistoryLength];
		gcHistory = new int[frameHistoryLength];
		stopWatch = new global::System.Diagnostics.Stopwatch();
		stopWatch.Start();
		lowestDt = 10000f;
		highestDt = 0f;
		totalDt = 0f;
		totalFrames = 0;
		totalGPUTime = 0f;
		totalCPUTime = 0f;
		totalOtherTime = 0f;
		totalTimeElapsed = 0f;
		frameIter = 0;
		frameCount = 1;
	}

	private void addFPSAt(int startX, int startY)
	{
		yExtern = startY;
		for (int i = 0; i < 4; i++)
		{
			xExtern = startX;
			yOffset = i * 11;
			for (int j = 0; j < 11; j++)
			{
				textOverlayMask[yExtern, xExtern] = fpsBits[yOffset + j];
				xExtern++;
			}
			yExtern++;
		}
	}

	private void addNumberAt(int startX, int startY, int num, bool isLeading)
	{
		if (isLeading && num == 0)
		{
			num = -1;
		}
		startAt = num * 4;
		xLength = startAt + 3;
		yExtern = startY;
		for (z = 0; z < 5; z++)
		{
			xExtern = startX;
			yOffset = z * 39;
			for (k = startAt; k < xLength; k++)
			{
				if (num != -1 && numberBits[yOffset + k] == 1)
				{
					x1 = (float)(xExtern * graphMultiply) + xOff;
					y1 = (float)(yExtern * graphMultiply) + yOff;
					global::UnityEngine.GL.Vertex3(x1, y1, 0f);
					global::UnityEngine.GL.Vertex3(x1, y1 + (float)(1 * graphMultiply), 0f);
					global::UnityEngine.GL.Vertex3(x1 + (float)(1 * graphMultiply), y1 + (float)(1 * graphMultiply), 0f);
					global::UnityEngine.GL.Vertex3(x1 + (float)(1 * graphMultiply), y1, 0f);
				}
				xExtern++;
			}
			yExtern++;
		}
	}

	private void addPeriodAt(int startX, int startY)
	{
		x1 = (float)(startX * graphMultiply) + xOff;
		x2 = (float)((startX + 1) * graphMultiply) + xOff;
		y1 = (float)(startY * graphMultiply) + yOff;
		y2 = (float)((startY - 1) * graphMultiply) + yOff;
		global::UnityEngine.GL.Vertex3(x1, y1, 0f);
		global::UnityEngine.GL.Vertex3(x1, y2, 0f);
		global::UnityEngine.GL.Vertex3(x2, y2, 0f);
		global::UnityEngine.GL.Vertex3(x2, y1, 0f);
	}

	private void Update()
	{
		if (viewMode != FPSGraphC.FPSGraphViewMode.graphing)
		{
			return;
		}
		lastElapsed = (float)stopWatch.Elapsed.TotalSeconds;
		if (frameCount > 4)
		{
			dtHistory[0, frameIter] = dt;
			if (dt < lowestDt)
			{
				lowestDt = dt;
			}
			else if (dt > highestDt)
			{
				highestDt = dt;
			}
			if (frameIter % 10 == 0)
			{
				actualFPS = 10f / (global::UnityEngine.Time.realtimeSinceStartup - lastUpdate);
				lastUpdate = global::UnityEngine.Time.realtimeSinceStartup;
			}
			totalGPUTime += dtHistory[0, frameIter] - dtHistory[1, frameIter];
			totalCPUTime += dtHistory[1, frameIter] - dtHistory[2, frameIter];
			totalOtherTime += dtHistory[2, frameIter];
			if (lastCollectionCount != global::System.GC.CollectionCount(0))
			{
				gcHistory[frameIter] = 1;
				lastCollectionCount = global::System.GC.CollectionCount(0);
			}
			totalDt += dt;
			totalFrames++;
			frameIter++;
			if (frameIter >= frameHistoryLength)
			{
				frameIter = 0;
			}
			beforeRender = (float)stopWatch.Elapsed.TotalSeconds;
		}
		frameCount++;
	}

	private void LateUpdate()
	{
		eTotalSeconds = (float)stopWatch.Elapsed.TotalSeconds;
		dt = eTotalSeconds - beforeRender;
		dtHistory[2, frameIter] = dt;
		beforeRender = eTotalSeconds;
	}

	private void OnPostRender()
	{
		global::UnityEngine.GL.PushMatrix();
		mat.SetPass(0);
		global::UnityEngine.GL.LoadPixelMatrix();
		global::UnityEngine.GL.Begin(7);
		if (viewMode == FPSGraphC.FPSGraphViewMode.graphing)
		{
			xOff = graphPosition.x * (w - (float)(frameHistoryLength * graphMultiply));
			yOff = h - (float)(100 * graphMultiply) - graphPosition.y * (h - (float)(graphMultiply * 107));
			global::UnityEngine.GL.Color(darkenedBackWhole);
			global::UnityEngine.GL.Vertex3(xOff, yOff - (float)(8 * graphMultiply), 0f);
			global::UnityEngine.GL.Vertex3(xOff, (float)(100 * graphMultiply) + yOff, 0f);
			global::UnityEngine.GL.Vertex3((float)(graphSizeMin * graphMultiply) + xOff, 100f * (float)graphMultiply + yOff, 0f);
			global::UnityEngine.GL.Vertex3((float)(graphSizeMin * graphMultiply) + xOff, yOff - (float)(8 * graphMultiply), 0f);
			maxFrame = 0f;
			for (x = 0; x < frameHistoryLength; x++)
			{
				totalSeconds = dtHistory[0, x];
				if (totalSeconds > maxFrame)
				{
					maxFrame = totalSeconds;
				}
				totalSeconds *= yMulti;
				fpsVals[0] = totalSeconds;
				renderSeconds = dtHistory[1, x];
				renderSeconds *= yMulti;
				fpsVals[1] = renderSeconds;
				lateSeconds = dtHistory[2, x];
				lateSeconds *= yMulti;
				fpsVals[2] = lateSeconds;
				i = x - frameIter - 1;
				if (i < 0)
				{
					i = frameHistoryLength + i;
				}
				x1 = (float)(i * graphMultiply) + xOff;
				x2 = (float)((i + 1) * graphMultiply) + xOff;
				for (j = 0; j < fpsVals.Length; j++)
				{
					y1 = ((j >= fpsVals.Length - 1) ? yOff : (fpsVals[j + 1] * (float)graphMultiply + yOff));
					y2 = fpsVals[j] * (float)graphMultiply + yOff;
					global::UnityEngine.GL.Color(fpsColorsTo[j]);
					global::UnityEngine.GL.Vertex3(x1, y1, 0f);
					global::UnityEngine.GL.Vertex3(x2, y1, 0f);
					global::UnityEngine.GL.Color(fpsColors[j]);
					global::UnityEngine.GL.Vertex3(x2, y2, 0f);
					global::UnityEngine.GL.Vertex3(x1, y2, 0f);
				}
				if (gcHistory[x] == 1)
				{
					y1 = (float)(0 * graphMultiply) + yOff;
					y2 = (float)(-2 * graphMultiply) + yOff;
					global::UnityEngine.GL.Color(global::UnityEngine.Color.red);
					global::UnityEngine.GL.Vertex3(x1, y1, 0f);
					global::UnityEngine.GL.Vertex3(x2, y1, 0f);
					global::UnityEngine.GL.Vertex3(x2, y2, 0f);
					global::UnityEngine.GL.Vertex3(x1, y2, 0f);
				}
			}
			if (maxFrame < 1f / 120f)
			{
				maxFrame = 1f / 120f;
			}
			else if (maxFrame < 1f / 60f)
			{
				maxFrame = 1f / 60f;
			}
			else if (maxFrame < 1f / 30f)
			{
				maxFrame = 1f / 30f;
			}
			else if (maxFrame < 1f / 15f)
			{
				maxFrame = 1f / 15f;
			}
			else if (maxFrame < 0.1f)
			{
				maxFrame = 0.1f;
			}
			else if (maxFrame < 0.2f)
			{
				maxFrame = 0.2f;
			}
			yMulti = (float)graphHeight / maxFrame;
			global::UnityEngine.GL.Color(lineColor);
			x1 = (float)(28 * graphMultiply) + xOff;
			x2 = (float)(graphSizeMin * graphMultiply) + xOff;
			for (i = 0; i < lineY.Length; i++)
			{
				y1 = (float)(lineY[i] * graphMultiply) + yOff;
				y2 = (float)((lineY[i] + 1) * graphMultiply) + yOff;
				global::UnityEngine.GL.Vertex3(x1, y1, 0f);
				global::UnityEngine.GL.Vertex3(x1, y2, 0f);
				global::UnityEngine.GL.Vertex3(x2, y2, 0f);
				global::UnityEngine.GL.Vertex3(x2, y1, 0f);
			}
			global::UnityEngine.GL.Color(darkenedBack);
			x2 = (float)(27 * graphMultiply) + xOff;
			for (i = 0; i < lineY.Length; i++)
			{
				y1 = (float)(lineY2[i] * graphMultiply) + yOff;
				y2 = (float)((lineY2[i] + 9) * graphMultiply) + yOff;
				global::UnityEngine.GL.Vertex3(xOff, y1, 0f);
				global::UnityEngine.GL.Vertex3(xOff, y2, 0f);
				global::UnityEngine.GL.Vertex3(x2, y2, 0f);
				global::UnityEngine.GL.Vertex3(x2, y1, 0f);
			}
			for (i = 0; i < keyOffX.Length; i++)
			{
				x1 = (float)(keyOffX[i] * graphMultiply) + xOff + (float)(1 * graphMultiply);
				x2 = (float)((keyOffX[i] + 4) * graphMultiply) + xOff + (float)(1 * graphMultiply);
				y1 = (float)(5 * graphMultiply) + yOff - (float)(9 * graphMultiply);
				y2 = (float)(1 * graphMultiply) + yOff - (float)(9 * graphMultiply);
				global::UnityEngine.GL.Color(fpsColorsTo[i]);
				global::UnityEngine.GL.Vertex3(x1, y1, 0f);
				global::UnityEngine.GL.Vertex3(x1, y2, 0f);
				global::UnityEngine.GL.Vertex3(x2, y2, 0f);
				global::UnityEngine.GL.Vertex3(x2, y1, 0f);
			}
			for (i = 0; i < keyOffX.Length; i++)
			{
				x1 = (float)(keyOffX[i] * graphMultiply) + xOff;
				x2 = (float)((keyOffX[i] + 4) * graphMultiply) + xOff;
				y1 = (float)(5 * graphMultiply) + yOff - (float)(8 * graphMultiply);
				y2 = (float)(1 * graphMultiply) + yOff - (float)(8 * graphMultiply);
				global::UnityEngine.GL.Color(fpsColors[i]);
				global::UnityEngine.GL.Vertex3(x1, y1, 0f);
				global::UnityEngine.GL.Vertex3(x1, y2, 0f);
				global::UnityEngine.GL.Vertex3(x2, y2, 0f);
				global::UnityEngine.GL.Vertex3(x2, y1, 0f);
			}
			global::UnityEngine.GL.Color(global::UnityEngine.Color.white);
			for (x = 0; x < graphTexture.width; x++)
			{
				for (y = 0; y < graphHeight; y++)
				{
					if (textOverlayMask[y, x] == 1)
					{
						x1 = (float)(x * graphMultiply) + xOff;
						x2 = (float)(x * graphMultiply + 1 * graphMultiply) + xOff;
						y1 = (float)(y * graphMultiply) + yOff;
						y2 = (float)(y * graphMultiply + 1 * graphMultiply) + yOff;
						global::UnityEngine.GL.Vertex3(x1, y1, 0f);
						global::UnityEngine.GL.Vertex3(x1, y2, 0f);
						global::UnityEngine.GL.Vertex3(x2, y2, 0f);
						global::UnityEngine.GL.Vertex3(x2, y1, 0f);
					}
				}
			}
			if (maxFrame > 0f)
			{
				fps = global::UnityEngine.Mathf.Round(1f / maxFrame);
				if (showFPSNumber && actualFPS > 0f)
				{
					float num = global::UnityEngine.Mathf.Round(actualFPS);
					addNumberAt(1, 0, (int)(num / 100f % 10f), true);
					addNumberAt(5, 0, (int)((double)num / 10.0 % 10.0), false);
					addNumberAt(9, 0, (int)(num % 10f), false);
				}
				addNumberAt(1, 93, (int)(fps / 100f % 10f), true);
				addNumberAt(5, 93, (int)((double)fps / 10.0 % 10.0), true);
				addNumberAt(9, 93, (int)(fps % 10f), false);
				fps *= 2f;
				addNumberAt(1, 48, (int)(fps / 100f % 10f), true);
				addNumberAt(5, 48, (int)(fps / 10f % 10f), true);
				addNumberAt(9, 48, (int)(fps % 10f), false);
				fps *= 1.5f;
				addNumberAt(1, 23, (int)(fps / 100f % 10f), true);
				addNumberAt(5, 23, (int)(fps / 10f % 10f), true);
				addNumberAt(9, 23, (int)(fps % 10f), false);
				if (frameIter % 30 == 1)
				{
					mem = (float)global::Kampai.Util.Native.GetMemoryUsage() / 1000000f;
					if (mem > 100f)
					{
						memGb = true;
						mem /= 1000f;
					}
					else
					{
						memGb = false;
					}
					lastCollectionCount = global::System.GC.CollectionCount(0);
				}
				if ((double)mem < 1.0)
				{
					splitMb = mem.ToString("F3").Split("."[0]);
					if (splitMb[1][0] == "0"[0])
					{
						first = 0;
						second = int.Parse(splitMb[1]);
						third = second % 10;
						second = second / 10 % 10;
					}
					else
					{
						first = int.Parse(splitMb[1]);
						third = first % 10;
						second = first / 10 % 10;
						first = first / 100 % 10;
					}
					addPeriodAt(96, -6);
					addNumberAt(98, -7, first, false);
					addNumberAt(102, -7, second, false);
					addNumberAt(106, -7, third, false);
				}
				else
				{
					splitMb = mem.ToString("F1").Split("."[0]);
					first = int.Parse(splitMb[0]);
					if (first >= 10)
					{
						addNumberAt(96, -7, first / 10, false);
					}
					second = first % 10;
					if (second < 0)
					{
						second = 0;
					}
					addNumberAt(100, -7, second, false);
					addPeriodAt(104, -6);
					addNumberAt(106, -7, int.Parse(splitMb[1]), false);
				}
			}
			if (memGb)
			{
				for (x = 0; x < 7; x++)
				{
					for (y = 0; y < 4; y++)
					{
						if (gbBits[y * 7 + x] == 1)
						{
							x1 = (float)(x * graphMultiply) + xOff + (float)(111 * graphMultiply);
							x2 = (float)(x * graphMultiply + 1 * graphMultiply) + xOff + (float)(111 * graphMultiply);
							y1 = (float)(y * graphMultiply) + yOff + (float)(-7 * graphMultiply);
							y2 = (float)(y * graphMultiply + 1 * graphMultiply) + yOff + (float)(-7 * graphMultiply);
							global::UnityEngine.GL.Vertex3(x1, y1, 0f);
							global::UnityEngine.GL.Vertex3(x1, y2, 0f);
							global::UnityEngine.GL.Vertex3(x2, y2, 0f);
							global::UnityEngine.GL.Vertex3(x2, y1, 0f);
						}
					}
				}
			}
			else
			{
				for (x = 0; x < 9; x++)
				{
					for (y = 0; y < 4; y++)
					{
						if (mbBits[y * 9 + x] == 1)
						{
							x1 = (float)(x * graphMultiply) + xOff + (float)(111 * graphMultiply);
							x2 = (float)(x * graphMultiply + 1 * graphMultiply) + xOff + (float)(111 * graphMultiply);
							y1 = (float)(y * graphMultiply) + yOff + (float)(-7 * graphMultiply);
							y2 = (float)(y * graphMultiply + 1 * graphMultiply) + yOff + (float)(-7 * graphMultiply);
							global::UnityEngine.GL.Vertex3(x1, y1, 0f);
							global::UnityEngine.GL.Vertex3(x1, y2, 0f);
							global::UnityEngine.GL.Vertex3(x2, y2, 0f);
							global::UnityEngine.GL.Vertex3(x2, y1, 0f);
						}
					}
				}
			}
		}
		else
		{
			if (circleGraphLabels == null)
			{
				circleGraphLabels = new global::UnityEngine.Vector2[3];
			}
			global::UnityEngine.Rect rect = new global::UnityEngine.Rect(w * 0.05f, h * 0.05f, w * 0.9f, h * 0.9f);
			global::UnityEngine.GL.Color(new global::UnityEngine.Color(0f, 0f, 0f, 0.8f));
			global::UnityEngine.GL.Vertex3(rect.x, rect.y, 0f);
			global::UnityEngine.GL.Vertex3(rect.x + rect.width, rect.y, 0f);
			global::UnityEngine.GL.Vertex3(rect.x + rect.width, rect.y + rect.height, 0f);
			global::UnityEngine.GL.Vertex3(rect.x, rect.y + rect.height, 0f);
			if (viewMode == FPSGraphC.FPSGraphViewMode.totalperformance)
			{
				float num2 = totalCPUTime + totalGPUTime + totalOtherTime;
				float[] array = new float[3]
				{
					totalGPUTime / num2,
					totalCPUTime / num2,
					totalOtherTime / num2
				};
				float[] array2 = new float[3]
				{
					array[0],
					array[0] + array[1],
					1f
				};
				float num3 = w * 0.15f;
				float num4 = 0f;
				float num5 = (float)global::System.Math.PI / 120f;
				global::UnityEngine.Vector2 vector = new global::UnityEngine.Vector2(w * 0.7f, h * 0.5f);
				int num6 = 0;
				float num7 = 0f;
				for (num6 = 0; num6 < 3; num6++)
				{
					float f = (array2[num6] - array[num6] * 0.5f) * ((float)global::System.Math.PI * 2f);
					float num8 = num3 * 0.3f * global::UnityEngine.Mathf.Cos(f);
					num8 = ((!(num8 < 0f)) ? num8 : (num8 + num8));
					num8 = vector.x + num8;
					float num9 = vector.y + num3 * 0.5f * global::UnityEngine.Mathf.Sin(f) + 0.02f * h;
					circleGraphLabels[num6] = new global::UnityEngine.Vector2(num8, (float)global::UnityEngine.Screen.height - num9);
				}
				num6 = 0;
				while (num4 < (float)global::System.Math.PI * 2f)
				{
					float num10 = num4 / ((float)global::System.Math.PI * 2f);
					if (num10 > array2[num6])
					{
						num6++;
						num7 = 0f;
					}
					else
					{
						num7 += num5 / ((float)global::System.Math.PI * 2f);
					}
					global::UnityEngine.Color color = fpsColors[num6] - fpsColors[num6] * 0.4f;
					float num11 = num7 / array[num6];
					global::UnityEngine.GL.Color(fpsColors[num6] * 0.85f + color * num11);
					float num8 = vector.x + num3 * global::UnityEngine.Mathf.Cos(num4);
					float num9 = vector.y + num3 * global::UnityEngine.Mathf.Sin(num4);
					num4 += num5;
					global::UnityEngine.GL.Vertex3(vector.x, vector.y, 0f);
					global::UnityEngine.GL.Vertex3(num8, num9, 0f);
					num8 = vector.x + num3 * global::UnityEngine.Mathf.Cos(num4);
					num9 = vector.y + num3 * global::UnityEngine.Mathf.Sin(num4);
					global::UnityEngine.GL.Vertex3(num8, num9, 0f);
					global::UnityEngine.GL.Vertex3(vector.x, vector.y, 0f);
				}
			}
			rect = new global::UnityEngine.Rect(w * 0.375f, h * 0.08f, w * 0.25f, h * 0.11f);
			global::UnityEngine.GL.Color(fpsColorsTo[1]);
			global::UnityEngine.GL.Vertex3(rect.x, rect.y, 0f);
			global::UnityEngine.GL.Vertex3(rect.x + rect.width, rect.y, 0f);
			global::UnityEngine.GL.Color(fpsColors[1]);
			global::UnityEngine.GL.Vertex3(rect.x + rect.width, rect.y + rect.height, 0f);
			global::UnityEngine.GL.Vertex3(rect.x, rect.y + rect.height, 0f);
			float num12 = ((viewMode != FPSGraphC.FPSGraphViewMode.assetbreakdown) ? (0.5f * w) : (w * 0.05f));
			rect = new global::UnityEngine.Rect(num12, h * 0.84f, w * 0.45f, h * 0.11f);
			global::UnityEngine.GL.Color(fpsColorsTo[1]);
			global::UnityEngine.GL.Vertex3(rect.x, rect.y, 0f);
			global::UnityEngine.GL.Vertex3(rect.x + rect.width, rect.y, 0f);
			global::UnityEngine.GL.Color(fpsColors[1]);
			global::UnityEngine.GL.Vertex3(rect.x + rect.width, rect.y + rect.height, 0f);
			global::UnityEngine.GL.Vertex3(rect.x, rect.y + rect.height, 0f);
		}
		global::UnityEngine.GL.End();
		global::UnityEngine.GL.PopMatrix();
		dt = (float)stopWatch.Elapsed.TotalSeconds - beforeRender;
		dtHistory[1, frameIter] = dt;
		eTotalSeconds = (float)stopWatch.Elapsed.TotalSeconds;
		dt = eTotalSeconds - lastElapsed;
	}

	private void format()
	{
		if (!hasFormated)
		{
			hasFormated = true;
			h1 = global::UnityEngine.GUI.skin.GetStyle("Label");
			backupLabel = new global::UnityEngine.GUIStyle(h1);
			backupButton = new global::UnityEngine.GUIStyle(global::UnityEngine.GUI.skin.GetStyle("Button"));
			h1.alignment = global::UnityEngine.TextAnchor.UpperLeft;
			h1.fontSize = (int)((float)global::UnityEngine.Screen.height * 0.08f);
			h2 = new global::UnityEngine.GUIStyle(h1);
			h2.fontSize = (int)((float)global::UnityEngine.Screen.height * 0.05f);
			h3 = new global::UnityEngine.GUIStyle(h1);
			h3.fontSize = (int)((float)global::UnityEngine.Screen.height * 0.037f);
			graphTitles = new global::UnityEngine.GUIStyle(h1);
			graphTitles.fontSize = (int)((float)global::UnityEngine.Screen.height * 0.037f);
			guiButton = new global::UnityEngine.GUIStyle(h1);
			guiButton.normal.background = null;
		}
	}

	private void OnGUI()
	{
		w = global::UnityEngine.Screen.width;
		h = global::UnityEngine.Screen.height;
		if (viewMode != FPSGraphC.FPSGraphViewMode.graphing)
		{
			global::UnityEngine.Time.timeScale = 0f;
			format();
			global::UnityEngine.Color color = global::UnityEngine.GUI.color;
			global::UnityEngine.GUI.color = global::UnityEngine.Color.black;
			global::UnityEngine.Rect rect = new global::UnityEngine.Rect(w * 0.05f, h * 0.05f, w * 0.9f, h * 0.9f);
			global::UnityEngine.GUI.color = global::UnityEngine.Color.black;
			global::UnityEngine.GUI.color = global::UnityEngine.Color.white;
			global::UnityEngine.GUI.skin.label = h2;
			global::UnityEngine.GUI.Label(new global::UnityEngine.Rect(w * 0.1f, h * 0.07f, w, h * 0.2f), "Performance Results");
			global::UnityEngine.GUI.Label(new global::UnityEngine.Rect(w * 0.62f, h * 0.07f, w, h * 0.2f), "Assets Used");
			if (viewMode == FPSGraphC.FPSGraphViewMode.totalperformance)
			{
				global::UnityEngine.GUI.skin.label = h2;
				global::UnityEngine.GUI.Label(new global::UnityEngine.Rect(w * 0.1f, h * 0.2f, w, h * 0.2f), "Score:");
				global::UnityEngine.GUI.skin.label = h1;
				global::UnityEngine.GUI.Label(new global::UnityEngine.Rect(w * 0.1f, h * 0.27f, w, h * 0.2f), (totalDt * 1000f).ToString("n0") + "ms");
				global::UnityEngine.GUI.skin.label = h2;
				global::UnityEngine.GUI.Label(new global::UnityEngine.Rect(w * 0.1f, h * 0.38f, w, h * 0.2f), "Time Elapsed:");
				global::UnityEngine.GUI.skin.label = h1;
				global::UnityEngine.GUI.Label(new global::UnityEngine.Rect(w * 0.1f, h * 0.43f, w, h * 0.2f), totalTimeElapsed.ToString("F1") + "s");
				global::UnityEngine.GUI.skin.label = h3;
				float num = totalDt / (float)totalFrames;
				string[] array = new string[3]
				{
					"lowest: " + (1f / highestDt).ToString("n0") + "fps",
					"highest: " + (1f / lowestDt).ToString("n0") + "fps",
					"avg: " + (1f / num).ToString("n0") + "fps"
				};
				for (int i = 0; i < array.Length; i++)
				{
					global::UnityEngine.GUI.Label(new global::UnityEngine.Rect(w * 0.1f, h * 0.57f + w * 0.04f * (float)i, w, h * 0.2f), array[i]);
				}
				global::UnityEngine.GUI.color = global::UnityEngine.Color.black;
				global::UnityEngine.GUI.skin.label = graphTitles;
				array = new string[3] { "Render", "CPU", "Other" };
				float[] array2 = new float[3] { 0.12f, 0.12f, 0.12f };
				float num2 = 0.0023f * w;
				int num3 = 0;
				while (circleGraphLabels != null && num3 < circleGraphLabels.Length)
				{
					global::UnityEngine.GUI.color = global::UnityEngine.Color.black;
					global::UnityEngine.GUI.Label(new global::UnityEngine.Rect(circleGraphLabels[num3].x + num2, circleGraphLabels[num3].y + num2, w * array2[num3], h * 0.047f), array[num3]);
					global::UnityEngine.GUI.color = global::UnityEngine.Color.white;
					global::UnityEngine.GUI.Label(new global::UnityEngine.Rect(circleGraphLabels[num3].x, circleGraphLabels[num3].y, w * array2[num3], h * 0.047f), array[num3]);
					num3++;
				}
			}
			else
			{
				global::UnityEngine.GUILayout.BeginArea(new global::UnityEngine.Rect(w * 0.08f, h * 0.175f, w * 0.9f, h * 0.7f));
				global::UnityEngine.GUI.skin.label = h2;
				global::UnityEngine.GUILayout.Label("All: " + global::UnityEngine.Resources.FindObjectsOfTypeAll(typeof(global::UnityEngine.Object)).Length.ToString("n0"));
				global::UnityEngine.GUILayout.Label("Textures: " + global::UnityEngine.Resources.FindObjectsOfTypeAll(typeof(global::UnityEngine.Texture)).Length.ToString("n0"));
				global::UnityEngine.GUILayout.Label("AudioClips: " + global::UnityEngine.Resources.FindObjectsOfTypeAll(typeof(global::UnityEngine.AudioClip)).Length.ToString("n0"));
				global::UnityEngine.GUILayout.Label("Meshes: " + global::UnityEngine.Resources.FindObjectsOfTypeAll(typeof(global::UnityEngine.Mesh)).Length.ToString("n0"));
				global::UnityEngine.GUILayout.Label("Materials: " + global::UnityEngine.Resources.FindObjectsOfTypeAll(typeof(global::UnityEngine.Material)).Length.ToString("n0"));
				global::UnityEngine.GUILayout.Label("GameObjects: " + global::UnityEngine.Resources.FindObjectsOfTypeAll(typeof(global::UnityEngine.GameObject)).Length.ToString("n0"));
				global::UnityEngine.GUILayout.Label("Components: " + global::UnityEngine.Resources.FindObjectsOfTypeAll(typeof(global::UnityEngine.Component)).Length.ToString("n0"));
				global::UnityEngine.GUILayout.EndArea();
			}
			global::UnityEngine.GUI.skin.button = guiButton;
			if (global::UnityEngine.GUI.Button(new global::UnityEngine.Rect(w * 0.05f, h * 0.05f, w * 0.45f, h * 0.15f), string.Empty))
			{
				viewMode = FPSGraphC.FPSGraphViewMode.totalperformance;
			}
			if (global::UnityEngine.GUI.Button(new global::UnityEngine.Rect(w * 0.5f, h * 0.05f, w * 0.45f, h * 0.15f), string.Empty))
			{
				viewMode = FPSGraphC.FPSGraphViewMode.assetbreakdown;
			}
			if (global::UnityEngine.GUI.Button(new global::UnityEngine.Rect(w * 0.3f, h * 0.8f, w * 0.4f, h * 0.15f), string.Empty))
			{
				reset();
				viewMode = FPSGraphC.FPSGraphViewMode.graphing;
				global::UnityEngine.Time.timeScale = 1f;
			}
			rect = new global::UnityEngine.Rect(w * 0.435f, h * 0.83f, w * 0.25f, h * 0.11f);
			global::UnityEngine.GUI.skin.label = h2;
			global::UnityEngine.GUI.Label(rect, "Dismiss");
			global::UnityEngine.GUI.skin.label = backupLabel;
			global::UnityEngine.GUI.skin.button = backupButton;
			global::UnityEngine.GUI.color = color;
		}
		else if (global::UnityEngine.Time.frameCount > 4)
		{
			global::UnityEngine.GUI.DrawTexture(new global::UnityEngine.Rect(graphPosition.x * (w - (float)(graphMultiply * frameHistoryLength)), graphPosition.y * (h - (float)(graphMultiply * 107)) + (float)(100 * graphMultiply), graphSizeGUI.width, graphSizeGUI.height), graphTexture);
		}
		if (showPerformanceOnClick && didPressOnGraph() && highestDt > 0f)
		{
			showPerformance();
		}
	}

	public void showPerformance()
	{
		if (viewMode != FPSGraphC.FPSGraphViewMode.totalperformance)
		{
			totalTimeElapsed = global::UnityEngine.Time.time;
			viewMode = FPSGraphC.FPSGraphViewMode.totalperformance;
		}
	}

	public bool didPressOnGraph()
	{
		if (global::Kampai.Game.InputUtils.touchCount > 0 || (UnityEngine.InputSystem.Mouse.current != null && UnityEngine.InputSystem.Mouse.current.leftButton.wasPressedThisFrame))
		{
			global::UnityEngine.Rect rect = new global::UnityEngine.Rect(graphPosition.x * (w - (float)(graphMultiply * frameHistoryLength)), graphPosition.y * (h - (float)(graphMultiply * 107)), graphSizeGUI.width, 107 * graphMultiply);
			if (global::Kampai.Game.InputUtils.touchCount > 0)
			{
				for (int i = 0; i < global::Kampai.Game.InputUtils.touchCount; i++)
				{
					var t = global::Kampai.Game.InputUtils.GetTouch(i);
					if (t.phase == global::UnityEngine.TouchPhase.Ended && checkWithinRect(t.position, rect))
					{
						return true;
					}
				}
			}
			else if (UnityEngine.InputSystem.Mouse.current != null && UnityEngine.InputSystem.Mouse.current.leftButton.wasPressedThisFrame)
			{
				var mp = UnityEngine.InputSystem.Mouse.current.position.ReadValue();
				if (checkWithinRect(mp, rect))
				{
					return true;
				}
			}
		}
		return false;
	}

	public static bool checkWithinRect(global::UnityEngine.Vector2 vec2, global::UnityEngine.Rect rect)
	{
		vec2.y = (float)global::UnityEngine.Screen.height - vec2.y;
		return vec2.x > rect.x && vec2.x < rect.x + rect.width && vec2.y > rect.y && vec2.y < rect.y + rect.height;
	}
}

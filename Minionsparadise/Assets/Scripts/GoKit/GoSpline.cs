public class GoSpline
{
    private bool _isReversed;
    private AbstractGoSplineSolver _solver;

    public int currentSegment { get; private set; }
    public bool isClosed { get; private set; }
    public GoSplineType splineType { get; private set; }

    public global::System.Collections.Generic.List<global::UnityEngine.Vector3> nodes
    {
        get { return _solver.nodes; }
    }

    public float pathLength
    {
        get { return _solver.pathLength; }
    }

    public GoSpline(global::System.Collections.Generic.List<global::UnityEngine.Vector3> nodes, bool useStraightLines = false)
    {
        if (useStraightLines || nodes.Count == 2)
        {
            splineType = GoSplineType.StraightLine;
            _solver = new GoSplineStraightLineSolver(nodes);
        }
        else if (nodes.Count == 3)
        {
            splineType = GoSplineType.QuadraticBezier;
            _solver = new GoSplineQuadraticBezierSolver(nodes);
        }
        else if (nodes.Count == 4)
        {
            splineType = GoSplineType.CubicBezier;
            _solver = new GoSplineCubicBezierSolver(nodes);
        }
        else
        {
            splineType = GoSplineType.CatmullRom;
            _solver = new GoSplineCatmullRomSolver(nodes);
        }
    }

    public GoSpline(global::UnityEngine.Vector3[] nodes, bool useStraightLines = false)
        : this(new global::System.Collections.Generic.List<global::UnityEngine.Vector3>(nodes), useStraightLines)
    {
    }

    public GoSpline(string pathAssetName, bool useStraightLines = false)
        : this(nodeListFromAsset(pathAssetName), useStraightLines)
    {
    }

    private static global::System.Collections.Generic.List<global::UnityEngine.Vector3> nodeListFromAsset(string pathAssetName)
    {
        string empty = string.Empty;

        if (!pathAssetName.EndsWith(".asset"))
        {
            pathAssetName += ".asset";
        }

        if (global::UnityEngine.Application.platform == global::UnityEngine.RuntimePlatform.Android)
        {
            empty = global::System.IO.Path.Combine("jar:file://" + global::UnityEngine.Application.dataPath + "!/assets/", pathAssetName);
            global::UnityEngine.WWW wWW = new global::UnityEngine.WWW(empty);

            while (!wWW.isDone) { }

            return bytesToVector3List(wWW.bytes);
        }

        empty = ((global::UnityEngine.Application.platform != global::UnityEngine.RuntimePlatform.IPhonePlayer)
            ? global::System.IO.Path.Combine(global::UnityEngine.Application.streamingAssetsPath, pathAssetName)
            : global::System.IO.Path.Combine(global::System.IO.Path.Combine(global::UnityEngine.Application.dataPath, "Raw"), pathAssetName));

#if !UNITY_WEBPLAYER
        byte[] bytes = global::System.IO.File.ReadAllBytes(empty);
        return bytesToVector3List(bytes);
#else
		return null;
#endif
    }

    public static global::System.Collections.Generic.List<global::UnityEngine.Vector3> bytesToVector3List(byte[] bytes)
    {
        global::System.Collections.Generic.List<global::UnityEngine.Vector3> list = new global::System.Collections.Generic.List<global::UnityEngine.Vector3>();

        for (int i = 0; i < bytes.Length; i += 12)
        {
            global::UnityEngine.Vector3 item =
                new global::UnityEngine.Vector3(
                    global::System.BitConverter.ToSingle(bytes, i),
                    global::System.BitConverter.ToSingle(bytes, i + 4),
                    global::System.BitConverter.ToSingle(bytes, i + 8)
                );

            list.Add(item);
        }

        return list;
    }

    public global::UnityEngine.Vector3 getLastNode()
    {
        return _solver.nodes[_solver.nodes.Count - 1];
    }

    public void buildPath()
    {
        _solver.buildPath();
    }

    private global::UnityEngine.Vector3 getPoint(float t)
    {
        return _solver.getPoint(t);
    }

    public global::UnityEngine.Vector3 getPointOnPath(float t)
    {
        if (t < 0f || t > 1f)
        {
            t = ((!isClosed) ? global::UnityEngine.Mathf.Clamp01(t) : ((!(t < 0f)) ? (t - 1f) : (t + 1f)));
        }

        return _solver.getPointOnPath(t);
    }

    public void closePath()
    {
        if (!isClosed)
        {
            isClosed = true;
            _solver.closePath();
        }
    }

    public void reverseNodes()
    {
        if (!_isReversed)
        {
            _solver.reverseNodes();
            _isReversed = true;
        }
    }

    public void unreverseNodes()
    {
        if (_isReversed)
        {
            _solver.reverseNodes();
            _isReversed = false;
        }
    }

    public void drawGizmos(float resolution)
    {
        _solver.drawGizmos();

        global::UnityEngine.Vector3 to = _solver.getPoint(0f);
        resolution *= (float)_solver.nodes.Count;

        for (int i = 1; (float)i <= resolution; i++)
        {
            float t = (float)i / resolution;
            global::UnityEngine.Vector3 point = _solver.getPoint(t);
            global::UnityEngine.Gizmos.DrawLine(point, to);
            to = point;
        }
    }

    public static void drawGizmos(global::UnityEngine.Vector3[] path, float resolution = 50f)
    {
        GoSpline goSpline = new GoSpline(path);
        goSpline.drawGizmos(resolution);
    }
}
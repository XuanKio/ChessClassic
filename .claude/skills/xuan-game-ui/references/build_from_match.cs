// Template for mcp__UnityMCP__execute_code (CodeDom, C# 6). Builds ONE widget from `pixels.py match --json` output,
// placing every tile exactly where the preview has it. Sprite rects are read at run time, so auto-trimmed slices
// (16x14, 12x16, ...) still land on the preview's pixels.
//
// renderCheck = true  -> builds into a temporary additive scene, renders PNGs at several fill levels to
//                        Temp/Captures/<widget>_<pct>.png with an off-screen camera, then closes the scene unsaved.
//                        Use this first: it is pixel-exact regardless of the Game view's zoom or aspect.
// renderCheck = false -> builds under `canvasName` in the open scene and wires HealthBarView (if fillLayer is used).

// ---- CONFIG ----
const bool renderCheck = true;
const string widget = "HealthBar";
const string canvasName = "HUDCanvas";
const string dir = "Assets/_Game/Art/Sprites/Interface/Tiles/";
const float uiScale = 3f, marginX = 16f, marginY = 16f;    // top-left corner placement on the real canvas
const float frameInset = 3f;                                // fill pixels hidden under the frame at each end
// tile, x, y, layer  (x/y = match output; layer 0 = static, 1 = fill that shrinks)
int[] entries = {
    139, 12, 6, 1,
    141, 22, 6, 1,
    141, 38, 6, 1,
    142, 48, 6, 1,
};
// Frame drawn over each fill tile at the same spot, so the fill can shrink under an intact frame.
// Red 139-142 and blue 157-160 are "frame + fill" tiles; 121-124 are the matching empty frames.
var frameFor = new System.Collections.Generic.Dictionary<int, int> {
    {139,121},{140,122},{141,123},{142,124}, {157,121},{158,122},{159,123},{160,124},
};
float[] checkFractions = { 1f, 0.6f, 0.01f };
// ---------------

System.Func<int, Sprite> load = n => {
    foreach (var a in AssetDatabase.LoadAllAssetsAtPath(dir + "tile_" + n.ToString("0000") + ".png"))
        if (a is Sprite s) return s;
    throw new System.Exception("No sprite for tile " + n);
};
// Preview-space rect (top-left origin, y down) that a sprite covers when its PNG's top-left sits at (x, y).
System.Func<Sprite, float, float, Rect> previewRect = (s, x, y) =>
    new Rect(x + s.rect.x, y + (s.texture.height - s.rect.yMax), s.rect.width, s.rect.height);

// Bounds of everything placed, so the widget's own rect hugs the visible pixels.
float minX = float.MaxValue, minY = float.MaxValue, maxX = float.MinValue, maxY = float.MinValue;
bool hasFill = false;
for (int i = 0; i < entries.Length; i += 4) {
    var r = previewRect(load(entries[i]), entries[i + 1], entries[i + 2]);
    minX = Mathf.Min(minX, r.xMin); minY = Mathf.Min(minY, r.yMin);
    maxX = Mathf.Max(maxX, r.xMax); maxY = Mathf.Max(maxY, r.yMax);
    if (entries[i + 3] == 1) {
        hasFill = true;
        if (!frameFor.ContainsKey(entries[i])) throw new System.Exception("No frame mapping for fill tile " + entries[i]);
    }
}
float width = maxX - minX, height = maxY - minY;

UnityEngine.SceneManagement.Scene tempScene = default(UnityEngine.SceneManagement.Scene);
Camera cam = null;
RenderTexture target = null;
Transform parent;
if (renderCheck) {
    tempScene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(
        UnityEditor.SceneManagement.NewSceneSetup.EmptyScene, UnityEditor.SceneManagement.NewSceneMode.Additive);
    target = new RenderTexture(Mathf.CeilToInt(width * uiScale) + 32, Mathf.CeilToInt(height * uiScale) + 32, 24);
    var camGo = new GameObject("RenderCheckCamera");
    UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(camGo, tempScene);
    cam = camGo.AddComponent<Camera>();
    cam.orthographic = true; cam.enabled = false; cam.targetTexture = target;
    cam.clearFlags = CameraClearFlags.SolidColor; cam.backgroundColor = new Color32(243, 205, 172, 255);
    var canvasGo = new GameObject("RenderCheckCanvas", typeof(Canvas), typeof(UnityEngine.UI.CanvasScaler));
    UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(canvasGo, tempScene);
    var c = canvasGo.GetComponent<Canvas>();
    c.renderMode = RenderMode.ScreenSpaceCamera; c.worldCamera = cam; c.planeDistance = 1f;
    parent = canvasGo.transform;
} else {
    var canvas = GameObject.Find(canvasName);
    if (canvas == null) throw new System.Exception(canvasName + " not found");
    if (canvas.transform.Find(widget) != null) throw new System.Exception(widget + " already exists under " + canvasName);
    parent = canvas.transform;
}

System.Func<string, Transform, RectTransform> make = (name, p) => {
    var go = new GameObject(name, typeof(RectTransform));
    if (!renderCheck) Undo.RegisterCreatedObjectUndo(go, "Create " + widget);
    go.layer = LayerMask.NameToLayer("UI");
    var rt = (RectTransform)go.transform;
    rt.SetParent(p, false);
    rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0f, 1f);
    return rt;
};
System.Action<Transform, int, float, float, string> put = (p, n, x, y, name) => {
    var s = load(n);
    var r = previewRect(s, x, y);
    var rt = make(name, p);
    rt.anchoredPosition = new Vector2(r.x - minX, -(r.y - minY));
    rt.sizeDelta = new Vector2(r.width, r.height);
    var img = rt.gameObject.AddComponent<UnityEngine.UI.Image>();
    img.sprite = s; img.raycastTarget = false;
};

var root = make(widget, parent);
root.sizeDelta = new Vector2(width, height);
root.localScale = new Vector3(uiScale, uiScale, 1f);
root.anchoredPosition = renderCheck ? new Vector2(16f, -16f) : new Vector2(marginX, -marginY);

RectTransform mask = null;
if (hasFill) {
    mask = make("FillMask", root);
    mask.sizeDelta = new Vector2(width, height);
    mask.gameObject.AddComponent<UnityEngine.UI.RectMask2D>();
}
var frame = make("Frame", root);
frame.sizeDelta = new Vector2(width, height);
for (int i = 0, k = 0; i < entries.Length; i += 4, k++) {
    int n = entries[i]; float x = entries[i + 1], y = entries[i + 2];
    if (entries[i + 3] == 1) {
        put(mask, n, x, y, "Fill" + k + "_tile" + n);
        put(frame, frameFor[n], x, y, "Frame" + k + "_tile" + frameFor[n]);
    } else {
        put(frame, n, x, y, "Static" + k + "_tile" + n);
    }
}

if (renderCheck) {
    var sb = new System.Text.StringBuilder("widget " + width + "x" + height + " rendered:");
    var readback = new Texture2D(target.width, target.height, TextureFormat.RGBA32, false);
    System.IO.Directory.CreateDirectory("Temp/Captures");
    foreach (float f in checkFractions) {
        if (mask != null) {
            float window = Mathf.Max(0f, width - 2f * frameInset);
            mask.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, frameInset + Mathf.Ceil(f * window));
        }
        Canvas.ForceUpdateCanvases();
        cam.Render();
        RenderTexture.active = target;
        readback.ReadPixels(new Rect(0, 0, target.width, target.height), 0, 0);
        readback.Apply();
        RenderTexture.active = null;
        string file = "Temp/Captures/" + widget + "_" + Mathf.RoundToInt(f * 100) + ".png";
        System.IO.File.WriteAllBytes(file, readback.EncodeToPNG());
        sb.Append(" " + file);
        if (mask == null) break;
    }
    UnityEngine.Object.DestroyImmediate(readback);
    UnityEditor.SceneManagement.EditorSceneManager.CloseScene(tempScene, true);
    target.Release();
    return sb.Append(" (origin 16,16 at x" + uiScale + ")").ToString();
}

if (mask != null) {
    var view = root.gameObject.AddComponent<HealthBarView>();
    var player = GameObject.Find("Player");
    var so = new SerializedObject(view);
    so.FindProperty("health").objectReferenceValue = player != null ? player.GetComponent<CharacterHealth>() : null;
    so.FindProperty("fillMask").objectReferenceValue = mask;
    so.FindProperty("frameInset").floatValue = frameInset;
    so.ApplyModifiedPropertiesWithoutUndo();
}
UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(root.gameObject.scene);
return "created " + widget + " " + width + "x" + height;

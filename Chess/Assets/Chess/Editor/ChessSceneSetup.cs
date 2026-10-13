using System;
using System.IO;
using System.Linq;
using ChessClassic.Presentation;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace ChessClassic.Editor
{
    public static class ChessSceneSetup
    {
        public const string ScenePath = "Assets/Scenes/ChessGame.unity";
        private const string Art = "Assets/Art/ChessCom/";
        private static Font font;
        private static readonly Color White = Hex("F4F2EF");
        private static readonly Color Muted = Hex("AAA79F");

        [MenuItem("Chess Classic/Create Board Scene")]
        public static void Build()
        {
            if (File.Exists(ScenePath)) throw new InvalidOperationException("ChessGame already exists. Open the saved scene to edit it.");
            if (!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            ImportSprites();
            font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var camera = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener)).GetComponent<Camera>();
            camera.tag = "MainCamera";
            camera.transform.position = new Vector3(0, 0, -10);
            camera.orthographic = true;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Hex("302E2B");
            camera.nearClipPlane = .1f;
            camera.farClipPlane = 100;

            var canvas = new GameObject("Chess Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster)).GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = camera;
            canvas.planeDistance = 1;
            var scaler = canvas.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1373, 911);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            var root = Rect("Game Layout", canvas.transform, 0, 0, 1373, 911);
            root.anchorMin = root.anchorMax = new Vector2(.5f, .5f);
            root.pivot = new Vector2(.5f, .5f);
            root.anchoredPosition = Vector2.zero;

            var board = Rect("Board", root, 26, 68, 784, 784);
            var boardImage = board.gameObject.AddComponent<Image>();
            boardImage.sprite = SpriteAt("Board/green.png");
            boardImage.raycastTarget = false;
            var preview = board.gameObject.AddComponent<ChessBoardPreview>();
            preview.squares = new RectTransform[64];
            preview.fileLabels = new Text[8];
            preview.rankLabels = new Text[8];
            preview.lastMoveFrom = Panel("Last Move From", board, 0, 0, 98, 98, new Color(1, 1, 0, .5f));
            preview.lastMoveTo = Panel("Last Move To", board, 0, 0, 98, 98, new Color(1, 1, 0, .5f));
            preview.lastMoveFrom.pivot = preview.lastMoveTo.pivot = new Vector2(.5f, .5f);
            var cells = Rect("Squares", board, 0, 0, 784, 784);
            const string back = "rnbqkbnr";
            for (int rank = 0; rank < 8; rank++)
            for (int file = 0; file < 8; file++)
            {
                int index = rank * 8 + file;
                string squareName = "" + (char)('a' + file) + (rank + 1);
                var cell = Rect(squareName, cells, (file + .5f) * 98, (rank + .5f) * 98, 98, 98);
                cell.pivot = new Vector2(.5f, .5f);
                var hit = cell.gameObject.AddComponent<Image>();
                hit.color = Color.clear;
                var button = cell.gameObject.AddComponent<Button>();
                button.targetGraphic = hit;
                button.transition = Selectable.Transition.None;
                button.navigation = new Navigation { mode = Navigation.Mode.None };
                UnityEventTools.AddIntPersistentListener(button.onClick, preview.SelectSquare, index);
                preview.squares[index] = cell;
                string code = rank == 0 ? "w" + back[file] : rank == 1 ? "wp" : rank == 6 ? "bp" : rank == 7 ? "b" + back[file] : null;
                if (code == null) continue;
                var piece = Rect("Piece_" + code, cell, 0, 0, 98, 98);
                piece.anchorMin = piece.anchorMax = piece.pivot = new Vector2(.5f, .5f);
                var picture = piece.gameObject.AddComponent<Image>();
                picture.sprite = SpriteAt("Pieces/" + code + ".png");
                picture.preserveAspect = true;
                picture.raycastTarget = false;
            }
            preview.selection = Panel("Selected Square", board, 0, 0, 98, 98, new Color(1, .85f, .18f, .32f));
            preview.selection.pivot = new Vector2(.5f, .5f);
            preview.selection.GetComponent<Image>().raycastTarget = false;
            preview.selection.gameObject.SetActive(false);
            for (int i = 0; i < 8; i++)
            {
                preview.fileLabels[i] = Label("File " + i, board, ((char)('a' + i)).ToString(), i * 98 + 78, 2, 19, 26, 22, i % 2 == 0 ? Hex("EBECD0") : Hex("739552"));
                preview.rankLabels[i] = Label("Rank " + i, board, (i + 1).ToString(), 5, i * 98 + 68, 23, 29, 22, i % 2 == 0 ? Hex("EBECD0") : Hex("739552"));
            }
            preview.whiteAvatar = SpriteAt("Avatars/white.png");
            preview.blackAvatar = SpriteAt("Avatars/black.png");
            preview.topAvatar = Avatar(root, "Top Avatar", 26, 861, preview.whiteAvatar);
            preview.bottomAvatar = Avatar(root, "Bottom Avatar", 26, 18, preview.blackAvatar);
            preview.topPlayer = Label("Top Player", root, "Đối thủ", 76, 878, 340, 25, 14, White, FontStyle.Bold);
            preview.bottomPlayer = Label("Bottom Player", root, "Xuân", 76, 35, 340, 25, 14, White, FontStyle.Bold);
            preview.topColor = Label("Top Color", root, "Trắng", 76, 860, 250, 20, 12, Muted);
            preview.bottomColor = Label("Bottom Color", root, "Đen", 76, 17, 250, 20, 12, Muted);
            var topClock = Panel("Top Clock", root, 670, 861, 140, 40, Hex("999896"));
            preview.topClock = Label("Time", topClock, "9:56", 10, 0, 120, 40, 26, Hex("545350"), FontStyle.Bold, TextAnchor.MiddleRight);
            var bottomClock = Panel("Bottom Clock", root, 670, 18, 140, 40, Hex("262421"));
            preview.bottomClock = Label("Time", bottomClock, "9:54", 32, 0, 98, 40, 26, White, FontStyle.Bold, TextAnchor.MiddleRight);
            Icon("Clock", bottomClock, "clock", 10, 10, 20, 20);

            var panel = Panel("Game Panel", root, 842, 12, 498, 890, Hex("262421"));
            Panel("Play Header", panel, 0, 834, 498, 56, Hex("211F1C"));
            Icon("Play Clock", panel, "clock", 236, 859, 25, 25);
            Label("Play", panel, "Chơi", 0, 835, 498, 22, 14, White, FontStyle.Bold, TextAnchor.MiddleCenter);
            Label("Moves Tab", panel, "Các nước đi", 0, 780, 498, 54, 14, White, FontStyle.Normal, TextAnchor.MiddleCenter);
            Panel("Active Tab Line", panel, 0, 778, 498, 3, Hex("B9B8B6"));
            Label("Opening", panel, "English Opening", 16, 737, 430, 38, 14, White);
            Panel("Moves Divider", panel, 0, 736, 498, 1, Hex("3C3A37"));
            preview.moveText = Label("Move Row", panel, "1.       c4", 16, 704, 430, 28, 14, White, FontStyle.Bold);
            preview.status = Label("Status", panel, "Xuân · Đối thủ  |  10 phút", 16, 145, 466, 28, 14, White, FontStyle.Normal, TextAnchor.MiddleCenter);
            Label("Preview Note", panel, "Cờ tiêu chuẩn", 16, 121, 466, 24, 12, Muted, FontStyle.Normal, TextAnchor.MiddleCenter);
            Panel("Playback Background", panel, 0, 0, 498, 112, Hex("211F1C"));
            var first = ActionButton("First Move", panel, "|‹", 16, 48, 86, 48, Hex("302E2B"), Hex("BFBEBB"));
            var previous = ActionButton("Previous Move", panel, "‹", 111, 48, 86, 48, Hex("302E2B"), Hex("BFBEBB"));
            var replay = ActionButton("Replay", panel, "▶", 206, 48, 86, 48, Hex("302E2B"), Hex("BFBEBB"));
            var next = ActionButton("Next Move", panel, "›", 301, 48, 86, 48, Hex("302E2B"), Hex("BFBEBB"));
            var last = ActionButton("Last Move", panel, "›|", 396, 48, 86, 48, Hex("302E2B"), Hex("BFBEBB"));
            UnityEventTools.AddPersistentListener(first.onClick, preview.PreviousMove);
            UnityEventTools.AddPersistentListener(previous.onClick, preview.PreviousMove);
            UnityEventTools.AddPersistentListener(replay.onClick, preview.ToggleReplay);
            UnityEventTools.AddPersistentListener(next.onClick, preview.NextMove);
            UnityEventTools.AddPersistentListener(last.onClick, preview.NextMove);
            var draw = ActionButton("Offer Draw", panel, "½  Hòa cờ", 12, 5, 100, 35, Hex("211F1C"), Muted);
            var resign = ActionButton("Resign", panel, "⚑  Đầu hàng", 119, 5, 114, 35, Hex("211F1C"), Muted);
            draw.GetComponentInChildren<Text>().fontSize = 14;
            resign.GetComponentInChildren<Text>().fontSize = 14;
            draw.interactable = resign.interactable = false;
            var reset = ActionButton("Reset Board", panel, "↺", 397, 5, 40, 35, Hex("211F1C"), Muted);
            var flip = ActionButton("Flip Board", panel, "", 446, 5, 40, 35, Hex("211F1C"), Muted);
            Icon("Flip Icon", flip.transform, "flip", 9, 7, 22, 22);
            UnityEventTools.AddPersistentListener(reset.onClick, preview.ResetBoard);
            UnityEventTools.AddPersistentListener(flip.onClick, preview.FlipBoard);
            preview.ResetBoard();

            new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            Directory.CreateDirectory("Assets/Scenes");
            EditorSceneManager.SaveScene(scene, ScenePath);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) }
                .Concat(EditorBuildSettings.scenes.Where(s => s.path != ScenePath)).ToArray();
            AssetDatabase.SaveAssets();
            Validate();
            Capture(camera);
            Debug.Log("CHESS_SCENE_READY: " + ScenePath);
        }

        private static void ImportSprites()
        {
            foreach (var path in Directory.GetFiles(Art, "*.png", SearchOption.AllDirectories))
            {
                string assetPath = path.Replace('\\', '/');
                AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceSynchronousImport);
                var importer = (TextureImporter)AssetImporter.GetAtPath(assetPath);
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.alphaIsTransparency = true;
                importer.mipmapEnabled = false;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.maxTextureSize = 2048;
                importer.filterMode = FilterMode.Bilinear;
                importer.SaveAndReimport();
            }
        }

        [MenuItem("Chess Classic/Validate Board Scene")]
        public static void Validate()
        {
            var preview = UnityEngine.Object.FindFirstObjectByType<ChessBoardPreview>();
            if (preview == null || preview.squares.Length != 64) throw new InvalidOperationException("Expected 64 board squares.");
            int pieces = preview.GetComponentsInChildren<Image>(true).Count(i => i.name.StartsWith("Piece_"));
            if (pieces != 32) throw new InvalidOperationException("Expected 32 pieces, found " + pieces);
            foreach (var image in preview.GetComponentsInChildren<Image>(true).Where(i => i.name.StartsWith("Piece_")))
                if (image.sprite == null) throw new InvalidOperationException("Missing piece sprite: " + image.name);
            if (preview.squares[0].GetChild(0).name != "Piece_wr" || preview.squares[4].GetChild(0).name != "Piece_wk" || preview.squares[60].GetChild(0).name != "Piece_bk")
                throw new InvalidOperationException("Incorrect starting position.");
            // Exercise actual serialized UI callbacks, then leave the scene at its initial position.
            InvokePreviewButton(preview.squares[12].GetComponent<Button>());
            if (preview.status.text != "Ô e2") throw new InvalidOperationException("Square callback is not wired.");
            InvokePreviewButton(GameObject.Find("Flip Board").GetComponent<Button>());
            if (preview.fileLabels[0].text != "a" || preview.squares[0].anchoredPosition.x != 49)
                throw new InvalidOperationException("Board flip failed.");
            InvokePreviewButton(GameObject.Find("Reset Board").GetComponent<Button>());
            if (preview.fileLabels[0].text != "h" || preview.selection.gameObject.activeSelf)
                throw new InvalidOperationException("Board reset failed.");
            InvokePreviewButton(GameObject.Find("Previous Move").GetComponent<Button>());
            if (preview.moveText.text != "") throw new InvalidOperationException("Previous move failed.");
            InvokePreviewButton(GameObject.Find("Next Move").GetComponent<Button>());
            if (preview.moveText.text != "1.       c4") throw new InvalidOperationException("Next move failed.");
            Debug.Log("CHESS_VALIDATION_OK: 64 squares, 32 sprites, selection, flip, reset and c4 replay callbacks.");
        }

        public static void ValidateSavedScene()
        {
            EditorSceneManager.OpenScene(ScenePath);
            Validate();
            Capture(Camera.main);
            Debug.Log("CHESS_SCENE_READY: " + ScenePath);
        }

        [MenuItem("Chess Classic/Capture Board Preview")]
        public static void CaptureCurrent() { Capture(Camera.main); }

        private static void InvokePreviewButton(Button button)
        {
            var original = button.onClick.GetPersistentListenerState(0);
            try
            {
                button.onClick.SetPersistentListenerState(0, UnityEventCallState.EditorAndRuntime);
                button.onClick.Invoke();
            }
            finally { button.onClick.SetPersistentListenerState(0, original); }
        }

        private static void Capture(Camera camera)
        {
            string directory = Path.GetFullPath("../Artifacts");
            Directory.CreateDirectory(directory);
            var target = new RenderTexture(1373, 911, 24);
            camera.targetTexture = target;
            Canvas.ForceUpdateCanvases();
            camera.Render();
            var previous = RenderTexture.active;
            RenderTexture.active = target;
            var texture = new Texture2D(1373, 911, TextureFormat.RGB24, false);
            texture.ReadPixels(new Rect(0, 0, 1373, 911), 0, 0);
            texture.Apply();
            File.WriteAllBytes(Path.Combine(directory, "ChessGame-preview.png"), texture.EncodeToPNG());
            camera.targetTexture = null;
            RenderTexture.active = previous;
            UnityEngine.Object.DestroyImmediate(texture);
            target.Release();
            UnityEngine.Object.DestroyImmediate(target);
        }

        private static RectTransform Rect(string name, Transform parent, float x, float y, float width, float height)
        {
            var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = rect.anchorMax = rect.pivot = Vector2.zero;
            rect.anchoredPosition = new Vector2(x, y);
            rect.sizeDelta = new Vector2(width, height);
            return rect;
        }

        private static RectTransform Panel(string name, Transform parent, float x, float y, float width, float height, Color color)
        {
            var rect = Rect(name, parent, x, y, width, height);
            var image = rect.gameObject.AddComponent<Image>();
            image.color = color;
            image.raycastTarget = false;
            return rect;
        }

        private static Text Label(string name, Transform parent, string value, float x, float y, float width, float height, int size, Color color, FontStyle style = FontStyle.Normal, TextAnchor alignment = TextAnchor.MiddleLeft)
        {
            var label = Rect(name, parent, x, y, width, height).gameObject.AddComponent<Text>();
            label.font = font ?? Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.text = value;
            label.fontSize = size;
            label.fontStyle = style;
            label.color = color;
            label.alignment = alignment;
            label.raycastTarget = false;
            return label;
        }

        private static Button ActionButton(string name, Transform parent, string value, float x, float y, float width, float height, Color background, Color foreground)
        {
            var rect = Panel(name, parent, x, y, width, height, background);
            rect.GetComponent<Image>().raycastTarget = true;
            var button = rect.gameObject.AddComponent<Button>();
            button.targetGraphic = rect.GetComponent<Image>();
            Label("Label", rect, value, 0, 0, width, height, 22, foreground, FontStyle.Bold, TextAnchor.MiddleCenter);
            return button;
        }

        private static void Icon(string name, Transform parent, string file, float x, float y, float width, float height)
        {
            var image = Rect(name, parent, x, y, width, height).gameObject.AddComponent<Image>();
            image.sprite = SpriteAt("Icons/" + file + ".png");
            image.preserveAspect = true;
            image.raycastTarget = false;
        }

        private static Sprite SpriteAt(string path)
        {
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(Art + path);
            if (sprite == null) throw new InvalidOperationException("Missing sprite: " + Art + path);
            return sprite;
        }

        private static Image Avatar(Transform parent, string name, float x, float y, Sprite sprite)
        {
            var image = Rect(name, parent, x, y, 40, 40).gameObject.AddComponent<Image>();
            image.sprite = sprite;
            image.preserveAspect = true;
            image.raycastTarget = false;
            return image;
        }

        private static Color Hex(string value)
        {
            ColorUtility.TryParseHtmlString("#" + value, out var color);
            return color;
        }
    }
}

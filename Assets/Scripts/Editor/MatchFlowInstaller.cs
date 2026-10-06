using System;
using MarioPie.Match;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace MarioPie.EditorTools
{
    [InitializeOnLoad]
    static class MatchFlowInstaller
    {
        const string ArenaPath = "Assets/Scenes/Arena.unity";
        const string FlowPath = "Assets/Resources/MatchFlow.asset";
        const string PrefabDir = "Assets/Prefabs/Match";

        static bool installing;

        static MatchFlowInstaller()
        {
            EditorApplication.delayCall += Install;
            EditorApplication.playModeStateChanged += OnPlayMode;
        }

        static void OnPlayMode(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredEditMode)
                EditorApplication.delayCall += Install;
        }

        [MenuItem("MarioPie/Ligar fluxo da partida")]
        static void Install()
        {
            if (installing || EditorApplication.isPlayingOrWillChangePlaymode)
                return;
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                EditorApplication.delayCall += Install;
                return;
            }

            var args = Environment.GetCommandLineArgs();
            for (var i = 0; i < args.Length; i++)
            {
                if (args[i] == "-runTests")
                    return;
            }

            installing = true;
            try
            {
                EnsureFolder();
                var start = Card("CardStart", "Start", new Color(1f, 0.92f, 0.35f));
                var timeUp = Card("CardTimeUp", "ACABOU", Color.white);
                var winLeft = Card("CardWinLeft", "AZUL GANHA", new Color(0.45f, 0.72f, 1f));
                var winRight = Card("CardWinRight", "VERMELHO GANHA", new Color(1f, 0.45f, 0.38f));
                var tie = Card("CardTie", "EMPATE", new Color(0.98f, 0.95f, 0.86f));
                var popup = Card("ScorePopup", "+1", new Color(0.58f, 1f, 0.28f), floater: true);
                var hudPrefab = Hud();
                var flow = Flow(start, timeUp, winLeft, winRight, tie, popup);
                WireScene(flow, hudPrefab);
                AssetDatabase.SaveAssets();
            }
            finally
            {
                installing = false;
            }
        }

        static void EnsureFolder()
        {
            if (!AssetDatabase.IsValidFolder(PrefabDir))
                AssetDatabase.CreateFolder("Assets/Prefabs", "Match");
        }

        static MatchFlow Flow(GameObject start, GameObject timeUp, GameObject winLeft, GameObject winRight, GameObject tie, GameObject popup)
        {
            var flow = AssetDatabase.LoadAssetAtPath<MatchFlow>(FlowPath);
            if (flow == null)
            {
                flow = ScriptableObject.CreateInstance<MatchFlow>();
                AssetDatabase.CreateAsset(flow, FlowPath);
            }

            Assign(flow, "cardStart", start);
            Assign(flow, "cardTimeUp", timeUp);
            Assign(flow, "cardWinLeft", winLeft);
            Assign(flow, "cardWinRight", winRight);
            Assign(flow, "cardTie", tie);
            Assign(flow, "scorePopup", popup);
            return flow;
        }

        static void WireScene(MatchFlow flow, GameObject hudPrefab)
        {
            var active = EditorSceneManager.GetActiveScene();
            var opened = false;
            if (active.path != ArenaPath)
            {
                if (active.isDirty)
                {
                    Debug.LogWarning("A cena Arena não está aberta. Abre-a para ligar o fluxo da partida.");
                    return;
                }

                EditorSceneManager.OpenScene(ArenaPath);
                opened = true;
            }

            var camera = Camera.main;
            if (camera == null)
            {
                Debug.LogWarning("A Arena não tem câmara principal.");
                return;
            }

            var shots = GameObject.Find("CameraShots");
            if (shots == null)
                shots = new GameObject("CameraShots");

            var playFov = camera.fieldOfView;
            var play = camera.transform;
            var intro = Shot(shots.transform, "ShotIntro", play.position - play.forward * 6f + Vector3.up * 3f, play.rotation, playFov + 6f);
            var game = Shot(shots.transform, "ShotPlay", play.position, play.rotation, playFov);
            var left = Shot(shots.transform, "ShotWinLeft", play.position - play.right * 1.6f + play.forward * 2f, play.rotation, playFov);
            var right = Shot(shots.transform, "ShotWinRight", play.position + play.right * 1.6f + play.forward * 2f, play.rotation, playFov);

            var rig = camera.GetComponent<MatchCamera>();
            if (rig == null)
                rig = camera.gameObject.AddComponent<MatchCamera>();
            var rigObject = new SerializedObject(rig);
            Assign(rigObject, "view", camera);
            Assign(rigObject, "intro", intro);
            Assign(rigObject, "play", game);
            Assign(rigObject, "winLeft", left);
            Assign(rigObject, "winRight", right);
            rigObject.ApplyModifiedPropertiesWithoutUndo();

            var hud = UnityEngine.Object.FindAnyObjectByType<MatchScoreHud>();
            if (hud == null && hudPrefab != null)
            {
                var instance = (GameObject)PrefabUtility.InstantiatePrefab(hudPrefab);
                instance.name = "MatchHud";
                hud = instance.GetComponent<MatchScoreHud>();
            }

            var cards = hud != null ? hud.GetComponent<MatchCards>() : null;
            var match = GameObject.Find("Match");
            if (match == null)
            {
                Debug.LogWarning("A Arena não tem o objecto Match.");
                return;
            }

            var director = match.GetComponent<MatchDirector>();
            if (director != null && Wired(director) && GameObject.Find("ShotPlay") != null && UnityEngine.Object.FindAnyObjectByType<MatchScoreHud>() != null)
                return;

            if (director == null)
                director = match.AddComponent<MatchDirector>();

            var directorObject = new SerializedObject(director);
            Assign(directorObject, "flow", flow);
            Assign(directorObject, "cameraRig", rig);
            Assign(directorObject, "cards", cards);
            Assign(directorObject, "hud", hud);
            var spawnProperty = directorObject.FindProperty("spawns");
            var spawnLeft = GameObject.Find("SpawnP1");
            var spawnRight = GameObject.Find("SpawnP2");
            if (spawnProperty != null && spawnLeft != null && spawnRight != null && spawnProperty.arraySize < 2)
            {
                spawnProperty.arraySize = 2;
                spawnProperty.GetArrayElementAtIndex(0).objectReferenceValue = spawnLeft.transform;
                spawnProperty.GetArrayElementAtIndex(1).objectReferenceValue = spawnRight.transform;
            }

            directorObject.ApplyModifiedPropertiesWithoutUndo();
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            Debug.Log(opened ? "Fluxo da partida ligado na Arena." : "Fluxo da partida ligado.");
        }

        static bool Wired(MatchDirector director)
        {
            var so = new SerializedObject(director);
            return Linked(so, "flow") && Linked(so, "cameraRig") && Linked(so, "hud") && Linked(so, "cards");
        }

        static bool Linked(SerializedObject so, string property)
        {
            var prop = so.FindProperty(property);
            return prop != null && prop.objectReferenceValue != null;
        }

        static CameraAnchor Shot(Transform parent, string name, Vector3 position, Quaternion rotation, float fov)
        {
            var existing = GameObject.Find(name);
            if (existing != null)
            {
                var ready = existing.GetComponent<CameraAnchor>();
                if (ready == null)
                    ready = existing.AddComponent<CameraAnchor>();
                return ready;
            }

            var shot = new GameObject(name);
            shot.transform.SetParent(parent, true);
            shot.transform.SetPositionAndRotation(position, rotation);
            var anchor = shot.AddComponent<CameraAnchor>();
            anchor.fieldOfView = fov;
            return anchor;
        }

        static GameObject Hud()
        {
            var path = PrefabDir + "/MatchHud.prefab";
            var existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (existing != null)
                return existing;

            var root = new GameObject("MatchHud", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(MatchScoreHud), typeof(MatchCards));
            var canvas = root.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;
            var scaler = root.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;

            var clock = Label(root.transform, "Clock", "30", 92, Color.white, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -28f), new Vector2(360f, 120f), TextAnchor.MiddleCenter);
            var leftRow = Cluster(root.transform, "LeftCluster", new Vector2(0f, 1f), new Vector2(16f, -12f), TextAnchor.MiddleLeft);
            var rightRow = Cluster(root.transform, "RightCluster", new Vector2(1f, 1f), new Vector2(-16f, -12f), TextAnchor.MiddleRight);
            var leftFace = Badge(leftRow, "LeftPortrait", new Color(0.3f, 0.62f, 0.98f), new Rect(0f, 0f, 0.5f, 1f), out var leftBadge);
            var rightFace = Badge(rightRow, "RightPortrait", new Color(0.95f, 0.32f, 0.28f), new Rect(0.5f, 0f, 0.5f, 1f), out var rightBadge);
            var left = Label(leftRow, "LeftScore", "0", 80, Color.white, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(80f, 96f), TextAnchor.MiddleLeft);
            var right = Label(rightRow, "RightScore", "0", 80, Color.white, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(80f, 96f), TextAnchor.MiddleRight);
            OutlineOn(left);
            OutlineOn(right);
            var mount = Anchor(root.transform, "Cards", new Vector2(0.5f, 0.5f), new Vector2(0f, 40f), new Vector2(1000f, 260f));
            var leftPopup = Anchor(root.transform, "LeftPopup", new Vector2(0f, 1f), new Vector2(150f, -170f), new Vector2(220f, 90f));
            var rightPopup = Anchor(root.transform, "RightPopup", new Vector2(1f, 1f), new Vector2(-150f, -170f), new Vector2(220f, 90f));

            var hud = root.GetComponent<MatchScoreHud>();
            var hudObject = new SerializedObject(hud);
            hudObject.FindProperty("leftScore").objectReferenceValue = left;
            hudObject.FindProperty("rightScore").objectReferenceValue = right;
            hudObject.FindProperty("clock").objectReferenceValue = clock;
            hudObject.FindProperty("leftPopup").objectReferenceValue = leftPopup;
            hudObject.FindProperty("rightPopup").objectReferenceValue = rightPopup;
            hudObject.FindProperty("leftPortrait").objectReferenceValue = leftBadge;
            hudObject.FindProperty("rightPortrait").objectReferenceValue = rightBadge;
            hudObject.FindProperty("leftFace").objectReferenceValue = leftFace;
            hudObject.FindProperty("rightFace").objectReferenceValue = rightFace;
            hudObject.FindProperty("portraitPrefab").objectReferenceValue = AssetDatabase.LoadAssetAtPath<CapsulePortrait>(PrefabDir + "/CapsulePortrait.prefab");
            hudObject.ApplyModifiedPropertiesWithoutUndo();

            var cards = root.GetComponent<MatchCards>();
            var cardsObject = new SerializedObject(cards);
            cardsObject.FindProperty("mount").objectReferenceValue = mount;
            cardsObject.ApplyModifiedPropertiesWithoutUndo();

            var prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
            UnityEngine.Object.DestroyImmediate(root);
            return prefab;
        }

        static GameObject Card(string name, string label, Color color, bool floater = false)
        {
            var path = PrefabDir + "/" + name + ".prefab";
            var existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (existing != null)
                return existing;

            var root = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            if (floater)
                root.AddComponent<ScoreFloater>();

            var rect = root.GetComponent<RectTransform>();
            rect.sizeDelta = floater ? new Vector2(220f, 110f) : new Vector2(1000f, 220f);
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            LabelOn(root.GetComponent<Text>(), label, floater ? 88 : 110, color, TextAnchor.MiddleCenter);
            if (floater)
            {
                var outline = root.AddComponent<Outline>();
                outline.effectColor = new Color(0.05f, 0.24f, 0.02f, 0.95f);
                outline.effectDistance = new Vector2(3f, -3f);
                outline.useGraphicAlpha = true;
            }

            var prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
            UnityEngine.Object.DestroyImmediate(root);
            return prefab;
        }

        static Text Label(Transform parent, string name, string value, int size, Color color, Vector2 anchorMin, Vector2 anchorMax, Vector2 position, Vector2 sizeDelta, TextAnchor alignment)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = new Vector2(anchorMin.x, anchorMin.y);
            rect.anchoredPosition = position;
            rect.sizeDelta = sizeDelta;
            return LabelOn(go.GetComponent<Text>(), value, size, color, alignment);
        }

        static Text LabelOn(Text text, string value, int size, Color color, TextAnchor alignment)
        {
            text.font = BuiltinFont();
            text.fontSize = size;
            text.fontStyle = FontStyle.Bold;
            text.alignment = alignment;
            text.color = color;
            text.text = value;
            text.raycastTarget = false;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            return text;
        }

        static RectTransform Cluster(Transform parent, string name, Vector2 anchor, Vector2 position, TextAnchor alignment)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(HorizontalLayoutGroup));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = new Vector2(anchor.x, 1f);
            rect.anchoredPosition = position;
            rect.sizeDelta = new Vector2(420f, 112f);
            var row = go.GetComponent<HorizontalLayoutGroup>();
            row.spacing = 10f;
            row.childAlignment = alignment;
            row.childControlWidth = true;
            row.childControlHeight = true;
            row.childForceExpandWidth = false;
            row.childForceExpandHeight = false;
            return rect;
        }

        static RawImage Badge(Transform parent, string name, Color ringColor, Rect uv, out RectTransform root)
        {
            var badge = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(LayoutElement));
            badge.transform.SetParent(parent, false);
            root = badge.GetComponent<RectTransform>();
            root.sizeDelta = new Vector2(108f, 108f);
            var layout = badge.GetComponent<LayoutElement>();
            layout.preferredWidth = 108f;
            layout.preferredHeight = 108f;

            var knob = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
            var ring = badge.GetComponent<Image>();
            ring.sprite = knob;
            ring.color = ringColor;
            ring.raycastTarget = false;
            if (knob == null)
                Debug.LogWarning("Falta o círculo do retrato.");

            var windowGo = new GameObject("Window", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Mask));
            windowGo.transform.SetParent(root, false);
            var window = windowGo.GetComponent<RectTransform>();
            window.anchorMin = window.anchorMax = new Vector2(0.5f, 0.5f);
            window.sizeDelta = new Vector2(86f, 86f);
            var windowImage = windowGo.GetComponent<Image>();
            windowImage.sprite = knob;
            windowImage.raycastTarget = false;
            windowGo.GetComponent<Mask>().showMaskGraphic = false;

            var photoGo = new GameObject("Photo", typeof(RectTransform), typeof(CanvasRenderer), typeof(RawImage));
            photoGo.transform.SetParent(window, false);
            var photoRect = photoGo.GetComponent<RectTransform>();
            photoRect.anchorMin = Vector2.zero;
            photoRect.anchorMax = Vector2.one;
            photoRect.offsetMin = Vector2.zero;
            photoRect.offsetMax = Vector2.zero;
            var photo = photoGo.GetComponent<RawImage>();
            photo.raycastTarget = false;
            photo.uvRect = uv;
            return photo;
        }

        static void OutlineOn(Text text)
        {
            var outline = text.gameObject.AddComponent<Outline>();
            outline.effectColor = new Color(0.12f, 0.08f, 0.18f, 0.9f);
            outline.effectDistance = new Vector2(3f, -3f);
            outline.useGraphicAlpha = true;
        }

        static RectTransform Anchor(Transform parent, string name, Vector2 anchor, Vector2 position, Vector2 size)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = new Vector2(anchor.x, anchor.y);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
            return rect;
        }

        static Font BuiltinFont()
        {
            var font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (font == null)
                font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            return font;
        }

        static void Assign(UnityEngine.Object target, string property, UnityEngine.Object value)
        {
            if (target == null || value == null)
                return;

            var so = new SerializedObject(target);
            Assign(so, property, value);
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(target);
        }

        static void Assign(SerializedObject so, string property, UnityEngine.Object value)
        {
            var prop = so.FindProperty(property);
            if (prop == null || value == null || prop.objectReferenceValue != null)
                return;

            prop.objectReferenceValue = value;
        }
    }
}

using MarioPie.Player;
using UnityEngine;
using UnityEngine.UI;

namespace MarioPie.Match
{
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(60)]
    public sealed class MatchScoreHud : MonoBehaviour
    {
        [SerializeField] Text leftScore;
        [SerializeField] Text rightScore;
        [SerializeField] Text clock;
        [SerializeField] RectTransform leftPopup;
        [SerializeField] RectTransform rightPopup;
        [SerializeField] RectTransform leftPortrait;
        [SerializeField] RectTransform rightPortrait;
        [SerializeField] Image leftFace;
        [SerializeField] Image rightFace;
        [SerializeField] float popupSide = 1.15f;
        [SerializeField] float popupLift = 1.35f;

        PlayerPieActor left;
        PlayerPieActor right;
        MatchDirector director;
        MatchFlow flow;
        int leftShown = int.MinValue;
        int rightShown = int.MinValue;
        float leftPunch;
        float rightPunch;
        bool warnedBoard;
        bool warnedPopup;
        bool warnedFace;

        public void Bind(PlayerPieActor sideZero, PlayerPieActor sideOne, MatchDirector match, MatchFlow matchFlow)
        {
            left = sideZero;
            right = sideOne;
            director = match;
            flow = matchFlow;
            leftShown = int.MinValue;
            rightShown = int.MinValue;
            ShowFace(leftFace, sideZero);
            ShowFace(rightFace, sideOne);
        }

        void ShowFace(Image face, PlayerPieActor actor)
        {
            if (face == null)
            {
                WarnFaces();
                return;
            }

            var portrait = actor != null && actor.Look != null ? actor.Look.portrait : null;
            if (portrait == null)
            {
                WarnPortrait();
                return;
            }

            face.sprite = portrait;
            face.preserveAspect = true;
            face.type = Image.Type.Simple;
        }

        void Update()
        {
            if (director == null || flow == null)
                return;

            if (clock == null || leftScore == null || rightScore == null)
            {
                WarnBoard();
                return;
            }

            WarnFaces();
            var shownTime = MatchRound.DisplaySeconds(director.SecondsLeft);
            clock.text = shownTime.ToString();
            var hurry = director.Hurry;
            clock.color = hurry ? flow.hurryColor : flow.timeColor;
            var pulse = hurry ? 1f + Mathf.Sin(Time.time * 10f) * 0.08f : 1f;
            clock.rectTransform.localScale = new Vector3(pulse, pulse, 1f);

            Present(left, leftScore, leftPortrait, leftPopup, ref leftShown, ref leftPunch, 0, right);
            Present(right, rightScore, rightPortrait, rightPopup, ref rightShown, ref rightPunch, 1, left);
        }

        void Present(
            PlayerPieActor actor,
            Text label,
            RectTransform portrait,
            RectTransform popup,
            ref int shown,
            ref float punch,
            int side,
            PlayerPieActor victim)
        {
            var score = actor != null ? actor.Score : 0;
            if (shown == int.MinValue)
                shown = score;
            else if (score > shown)
            {
                punch = flow.scorePunchSeconds;
                shown = score;
                SpawnPopup(popup, victim);
            }
            else
            {
                if (score < shown)
                    punch = 0f;
                shown = score;
            }

            label.text = score.ToString();
            var scale = 1f;
            if (director.Phase == MatchPhase.Result && director.Winner == side)
                scale = flow.winScoreScale;
            else if (punch > 0f)
            {
                var duration = flow.scorePunchSeconds > 0.01f ? flow.scorePunchSeconds : 0.28f;
                var t = 1f - Mathf.Clamp01(punch / duration);
                scale = Mathf.Lerp(1.45f, 1f, t);
                punch -= Time.deltaTime;
            }

            label.rectTransform.localScale = new Vector3(scale, scale, 1f);
            if (portrait != null)
            {
                var badge = scale > 1f ? 1f + (scale - 1f) * 0.42f : 1f;
                portrait.localScale = new Vector3(badge, badge, 1f);
            }
        }

        void SpawnPopup(RectTransform fallback, PlayerPieActor victim)
        {
            if (flow.scorePopup == null)
            {
                WarnPopup();
                return;
            }

            var popup = Instantiate(flow.scorePopup, transform);
            var rect = popup.transform as RectTransform;
            if (rect != null && TryPlaceBeside(victim, rect))
                return;

            if (rect == null || fallback == null)
                return;

            rect.SetParent(fallback, false);
            rect.anchoredPosition = Vector2.zero;
        }

        bool TryPlaceBeside(PlayerPieActor victim, RectTransform popup)
        {
            var cam = Camera.main;
            var canvas = transform as RectTransform;
            if (victim == null || cam == null || canvas == null)
                return false;

            var world = victim.transform.position + Vector3.up * popupLift + cam.transform.right * popupSide;
            var screen = cam.WorldToScreenPoint(world);
            if (screen.z <= 0f)
                return false;

            var view = GetComponent<Canvas>();
            Camera uiCam = null;
            if (view != null && view.renderMode != RenderMode.ScreenSpaceOverlay)
                uiCam = view.worldCamera;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas, screen, uiCam, out var local))
                return false;

            local.x += Random.Range(-14f, 24f);
            local.y += Random.Range(-4f, 28f);
            var area = canvas.rect;
            if (area.width > 200f && area.height > 200f)
            {
                var min = new Vector2(-canvas.pivot.x * area.width, -canvas.pivot.y * area.height);
                var max = min + area.size;
                local.x = Mathf.Clamp(local.x, min.x + 80f, max.x - 80f);
                local.y = Mathf.Clamp(local.y, min.y + 48f, max.y - 48f);
            }

            popup.anchorMin = canvas.pivot;
            popup.anchorMax = canvas.pivot;
            popup.pivot = new Vector2(0.5f, 0.5f);
            popup.anchoredPosition = local;
            return true;
        }

        void WarnBoard()
        {
            if (warnedBoard)
                return;

            warnedBoard = true;
            Debug.LogWarning("Falta o placar da partida.", this);
        }

        void WarnFaces()
        {
            if (warnedFace)
                return;
            if (leftPortrait != null && rightPortrait != null && leftFace != null && rightFace != null)
                return;

            warnedFace = true;
            Debug.LogWarning("Falta o retrato do jogador.", this);
        }

        void WarnPortrait()
        {
            if (warnedFace)
                return;

            warnedFace = true;
            Debug.LogWarning("Falta a imagem do retrato no personagem.", this);
        }

        void WarnPopup()
        {
            if (warnedPopup)
                return;

            warnedPopup = true;
            Debug.LogWarning("Falta o prefab de +1.", this);
        }
    }
}

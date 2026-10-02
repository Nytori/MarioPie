using MarioPie.Player;
using UnityEngine;

namespace MarioPie.Match
{
    public sealed class MatchScoreHud : MonoBehaviour
    {
        PlayerPieActor left;
        PlayerPieActor right;
        GUIStyle leftStyle;
        GUIStyle rightStyle;

        public void Bind(PlayerPieActor sideZero, PlayerPieActor sideOne)
        {
            left = sideZero;
            right = sideOne;
        }

        void OnGUI()
        {
            if (leftStyle == null)
            {
                leftStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 56,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.UpperLeft
                };
                leftStyle.normal.textColor = new Color(0.35f, 0.55f, 1f);
                rightStyle = new GUIStyle(leftStyle)
                {
                    alignment = TextAnchor.UpperRight
                };
                rightStyle.normal.textColor = new Color(1f, 0.38f, 0.32f);
            }

            const float margin = 28f;
            var area = new Rect(margin, margin, Screen.width - margin * 2f, 80f);
            GUI.Label(area, left != null ? left.Score.ToString() : "0", leftStyle);
            GUI.Label(area, right != null ? right.Score.ToString() : "0", rightStyle);
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

namespace ChessClassic.Presentation
{
    /// <summary>Scene preview controls only. This component does not implement chess rules.</summary>
    public sealed class ChessBoardPreview : MonoBehaviour
    {
        public RectTransform[] squares;
        public Text[] fileLabels;
        public Text[] rankLabels;
        public RectTransform selection;
        public Text status;
        public Text topPlayer;
        public Text bottomPlayer;
        public Text topColor;
        public Text bottomColor;
        public RectTransform lastMoveFrom;
        public RectTransform lastMoveTo;
        public Text moveText;
        public Text topClock;
        public Text bottomClock;
        public Image topAvatar;
        public Image bottomAvatar;
        public Sprite whiteAvatar;
        public Sprite blackAvatar;
        [SerializeField] private bool flipped = true;
        [SerializeField] private bool showMove = true;
        private int selected = -1;
        public const float CellSize = 98;

        public void SelectSquare(int index)
        {
            if (index < 0 || index >= squares.Length) return;
            selected = index;
            selection.gameObject.SetActive(true);
            selection.anchoredPosition = squares[index].anchoredPosition;
            status.text = "Ô " + (char)('a' + index % 8) + (index / 8 + 1);
        }

        public void FlipBoard()
        {
            flipped = !flipped;
            RefreshBoard();
        }

        public void ResetBoard()
        {
            flipped = true;
            selected = -1;
            selection.gameObject.SetActive(false);
            status.text = "Xuân · Đối thủ  |  10 phút";
            showMove = true;
            RefreshBoard();
        }

        public void PreviousMove() { showMove = false; RefreshBoard(); }
        public void NextMove() { showMove = true; RefreshBoard(); }
        public void ToggleReplay() { showMove = !showMove; RefreshBoard(); }

        private void RefreshBoard()
        {
            for (int i = 0; i < squares.Length; i++)
            {
                int file = i % 8, rank = i / 8;
                squares[i].anchoredPosition = new Vector2(
                    ((flipped ? 7 - file : file) + .5f) * CellSize,
                    ((flipped ? 7 - rank : rank) + .5f) * CellSize);
            }
            for (int i = 0; i < 8; i++)
            {
                fileLabels[i].text = ((char)('a' + (flipped ? 7 - i : i))).ToString();
                rankLabels[i].text = ((flipped ? 8 - i : i + 1)).ToString();
            }
            topPlayer.text = flipped ? "Đối thủ" : "Xuân";
            bottomPlayer.text = flipped ? "Xuân" : "Đối thủ";
            topColor.text = flipped ? "Trắng" : "Đen";
            bottomColor.text = flipped ? "Đen" : "Trắng";
            topClock.transform.parent.GetComponent<Image>().color = flipped ? new Color32(153,152,150,255) : new Color32(38,36,33,255);
            bottomClock.transform.parent.GetComponent<Image>().color = flipped ? new Color32(38,36,33,255) : new Color32(153,152,150,255);
            topClock.color = flipped ? new Color32(84,83,80,255) : Color.white;
            bottomClock.color = flipped ? Color.white : new Color32(84,83,80,255);
            topAvatar.sprite = flipped ? whiteAvatar : blackAvatar;
            bottomAvatar.sprite = flipped ? blackAvatar : whiteAvatar;
            var pawn = (RectTransform)squares[10].GetChild(0);
            pawn.anchoredPosition = showMove ? new Vector2(0, (flipped ? -2 : 2) * CellSize) : Vector2.zero;
            lastMoveFrom.anchoredPosition = squares[10].anchoredPosition;
            lastMoveTo.anchoredPosition = squares[26].anchoredPosition;
            lastMoveFrom.gameObject.SetActive(showMove);
            lastMoveTo.gameObject.SetActive(showMove);
            moveText.text = showMove ? "1.       c4" : "";
            topClock.text = showMove ? (flipped ? "9:56" : "9:54") : "10:00";
            bottomClock.text = showMove ? (flipped ? "9:54" : "9:56") : "10:00";
            if (selected >= 0) selection.anchoredPosition = squares[selected].anchoredPosition;
        }
    }
}

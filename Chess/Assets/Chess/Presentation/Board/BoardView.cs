using System;
using ChessClassic.Application;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ChessClassic.Presentation
{
    /// <summary>Renders a board snapshot and emits square input. Owns no game rules or HUD.</summary>
    public sealed class BoardView : MonoBehaviour, IBoardView
    {
        [SerializeField] private RectTransform[] squares;
        [SerializeField] private Image[] pieces;
        [SerializeField] private Text[] fileLabels;
        [SerializeField] private Text[] rankLabels;
        [SerializeField] private RectTransform selection;
        [SerializeField] private RectTransform lastMoveFrom;
        [SerializeField] private RectTransform lastMoveTo;
        [SerializeField] private Sprite[] pieceSprites;
        [SerializeField] private Color hintColor = new Color(0, 0, 0, .18f);
        private const string PieceCodes = "PNBRQKpnbrqk";
        private MoveHintGraphic[] hints;
        private UnityAction[] listeners;
        public event Action<int> SquareSelected;

        private void Awake() => Initialize();

        private void Initialize()
        {
            if (hints != null) return;
            hints = new MoveHintGraphic[64];
            listeners = new UnityAction[64];
            for (int i = 0; i < 64; i++)
            {
                int square = i;
                listeners[i] = () => SquareSelected?.Invoke(square);
                squares[i].GetComponent<Button>().onClick.AddListener(listeners[i]);
                var marker = new GameObject("Move Hint", typeof(RectTransform), typeof(MoveHintGraphic));
                marker.transform.SetParent(squares[i], false);
                var rect = (RectTransform)marker.transform;
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.offsetMin = rect.offsetMax = Vector2.zero;
                hints[i] = marker.GetComponent<MoveHintGraphic>();
                hints[i].raycastTarget = false;
                hints[i].color = hintColor;
                marker.SetActive(false);
            }
        }

        public void Render(BoardFrame frame)
        {
            Initialize();
            float cellSize = ((RectTransform)transform).rect.width / 8;
            for (int i = 0; i < 64; i++)
            {
                int file = i % 8, rank = i / 8;
                var position = new Vector2(((frame.Flipped ? 7 - file : file) + .5f) * cellSize,
                    ((frame.Flipped ? 7 - rank : rank) + .5f) * cellSize);
                squares[i].anchoredPosition = position;
                squares[i].sizeDelta = new Vector2(cellSize, cellSize);
                pieces[i].rectTransform.anchoredPosition = position;
                pieces[i].rectTransform.sizeDelta = new Vector2(cellSize, cellSize);
                int sprite = PieceCodes.IndexOf(frame.Pieces[i]);
                pieces[i].enabled = sprite >= 0;
                pieces[i].sprite = sprite >= 0 ? pieceSprites[sprite] : null;
                hints[i].gameObject.SetActive(false);
            }
            for (int i = 0; i < 8; i++)
            {
                fileLabels[i].text = ((char)('a' + (frame.Flipped ? 7 - i : i))).ToString();
                rankLabels[i].text = (frame.Flipped ? 8 - i : i + 1).ToString();
            }
            PlaceHighlight(selection, frame.Selected);
            PlaceHighlight(lastMoveFrom, frame.LastMoveFrom);
            PlaceHighlight(lastMoveTo, frame.LastMoveTo);
            foreach (int square in frame.Destinations)
            {
                hints[square].SetCapture(frame.Pieces[square] != '\0');
                hints[square].gameObject.SetActive(true);
            }
        }

        private void PlaceHighlight(RectTransform highlight, int square)
        {
            highlight.gameObject.SetActive(square >= 0 && square < 64);
            if (square >= 0 && square < 64)
            {
                highlight.anchoredPosition = squares[square].anchoredPosition;
                highlight.sizeDelta = squares[square].sizeDelta;
            }
        }

        private void OnDestroy()
        {
            if (listeners == null) return;
            for (int i = 0; i < listeners.Length; i++)
                if (squares[i] != null) squares[i].GetComponent<Button>().onClick.RemoveListener(listeners[i]);
        }
    }
}

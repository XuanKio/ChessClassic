using System;
using ChessClassic.Application;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ChessClassic.Presentation
{
    /// <summary>Player bars and game controls. Communicates through events, never BoardView.</summary>
    public sealed class GameHUD : MonoBehaviour, IGameHud
    {
        [SerializeField] private Text status;
        [SerializeField] private Text topPlayer, bottomPlayer, topColor, bottomColor;
        [SerializeField] private Text moveText, topClock, bottomClock;
        [SerializeField] private Image topAvatar, bottomAvatar;
        [SerializeField] private Sprite whiteAvatar, blackAvatar;
        [SerializeField] private Button[] commandButtons;
        private UnityAction[] listeners;
        public event Action<GameCommand> CommandRequested;

        private void Awake()
        {
            listeners = new UnityAction[commandButtons.Length];
            for (int i = 0; i < commandButtons.Length; i++)
            {
                var command = (GameCommand)i;
                listeners[i] = () => CommandRequested?.Invoke(command);
                commandButtons[i].onClick.AddListener(listeners[i]);
            }
        }

        public void Render(HudFrame frame)
        {
            bool whiteTop = frame.WhiteOnTop;
            topPlayer.text = whiteTop ? frame.WhiteName : frame.BlackName;
            bottomPlayer.text = whiteTop ? frame.BlackName : frame.WhiteName;
            topColor.text = whiteTop ? "Trắng" : "Đen";
            bottomColor.text = whiteTop ? "Đen" : "Trắng";
            topClock.text = whiteTop ? frame.WhiteClock : frame.BlackClock;
            bottomClock.text = whiteTop ? frame.BlackClock : frame.WhiteClock;
            topAvatar.sprite = whiteTop ? whiteAvatar : blackAvatar;
            bottomAvatar.sprite = whiteTop ? blackAvatar : whiteAvatar;
            StyleClock(topClock, whiteTop);
            StyleClock(bottomClock, !whiteTop);
            status.text = frame.Status;
            moveText.text = frame.MoveText;
        }

        private static void StyleClock(Text clock, bool white)
        {
            clock.transform.parent.GetComponent<Image>().color = white
                ? new Color32(153, 152, 150, 255) : new Color32(38, 36, 33, 255);
            clock.color = white ? new Color32(84, 83, 80, 255) : Color.white;
        }

        private void OnDestroy()
        {
            if (listeners == null) return;
            for (int i = 0; i < listeners.Length; i++)
                if (commandButtons[i] != null) commandButtons[i].onClick.RemoveListener(listeners[i]);
        }
    }
}

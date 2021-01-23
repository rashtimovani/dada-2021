using UnityEngine;

namespace RasHack.GapOverlap.Main
{
    public enum BackgroundColor
    {
        Black = 0,
        White = 1,
    }

    public class Background : MonoBehaviour
    {
        #region Serialized fields

        [SerializeField] private Color black = Color.black;

        [SerializeField] private Color white = Color.white;

        [SerializeField] private SpriteRenderer sprite;

        [SerializeField] private BackgroundColor usedColor = BackgroundColor.White;

        #endregion

        #region API

        public void SetBackground(BackgroundColor color)
        {
            usedColor = color;
            UpdateColor();
        }

        #endregion
        
        #region Mono methods

        private void Start()
        {
            UpdateColor();
        }

        #endregion

        #region Helpers

        private void UpdateColor()
        {
            switch (usedColor)
            {
                case BackgroundColor.Black:
                    sprite.color = black;
                    break;
                default:
                    sprite.color = white;
                    break;
            }
        }

        #endregion
    }
}
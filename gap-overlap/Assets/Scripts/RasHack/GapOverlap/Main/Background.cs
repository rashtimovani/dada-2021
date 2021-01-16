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
        [SerializeField] private BackgroundColor usedColor = BackgroundColor.White;

        [SerializeField] private Color black = Color.black;

        [SerializeField] private Color white = Color.white;

        [SerializeField] private SpriteRenderer sprite;

        void Start()
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
    }
}
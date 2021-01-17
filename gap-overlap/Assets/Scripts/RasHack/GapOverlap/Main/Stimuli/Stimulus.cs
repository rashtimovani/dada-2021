using UnityEngine;

namespace RasHack.GapOverlap.Main.Stimuli
{
    public class Stimulus : MonoBehaviour
    {
        #region Sprites

        [SerializeField] private Sprite bee;
        [SerializeField] private Sprite octopus;
        [SerializeField] private Sprite rainbow;
        [SerializeField] private Sprite rainCloud;
        [SerializeField] private Sprite umbrella;
        [SerializeField] private Sprite yellowBird;

        #endregion

        #region Internal fields

        private SpriteRenderer renderer;
        private float spentLifetime;

        #endregion

        #region Provided fields from simulator

        private StimuliType type;
        private Simulator owner;
        private float lifetime;

        #endregion

        #region API

        public void StartSimulating(StimuliType type, Simulator owner, float lifetime)
        {
            this.type = type;
            this.owner = owner;
            this.lifetime = lifetime;
            if (renderer != null) SetUpSprite();
        }

        #endregion

        #region Mono methods

        private void Start()
        {
            renderer = GetComponent<SpriteRenderer>();
            SetUpSprite();
        }

        private void Update()
        {
            spentLifetime += Time.deltaTime;
            if (spentLifetime < lifetime) return;

            owner.ReportStimulusDied(this);
            Destroy(gameObject);
        }

        #endregion

        #region Helpers

        private void SetUpSprite()
        {
            switch (type)
            {
                case StimuliType.Octopus:
                    renderer.sprite = octopus;
                    break;
                case StimuliType.Rainbow:
                    renderer.sprite = rainbow;
                    break;
                case StimuliType.RainCloud:
                    renderer.sprite = rainCloud;
                    break;
                case StimuliType.Umbrella:
                    renderer.sprite = umbrella;
                    break;
                case StimuliType.YellowBird:
                    renderer.sprite = yellowBird;
                    break;
                default:
                    renderer.sprite = bee;
                    break;
            }
        }

        #endregion
    }
}
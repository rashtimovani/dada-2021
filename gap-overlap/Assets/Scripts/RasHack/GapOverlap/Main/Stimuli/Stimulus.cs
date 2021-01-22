using RasHack.GapOverlap.Main.Inputs;
using UnityEngine;

namespace RasHack.GapOverlap.Main.Stimuli
{
    public class Stimulus : MonoBehaviour
    {
        #region Sprites

        [SerializeField] private Sprite bee;
        [SerializeField] private Sprite bird;
        [SerializeField] private Sprite owl;
        [SerializeField] private Sprite rainbow;
        [SerializeField] private Sprite rainCloud;
        [SerializeField] private Sprite umbrella;

        #endregion

        #region Internal fields

        private SpriteRenderer sprite;
        private float? spentLifetime;
        private bool wasFocusedOn;

        #endregion

        #region Provided fields from simulator

        private StimuliType type;
        private Task.Task owner;
        private float lifetime;

        #endregion

        #region API

        public void StartSimulating(StimuliType type, Task.Task owner, float lifetime)
        {
            this.type = type;
            this.owner = owner;

            this.lifetime = lifetime;
            spentLifetime = 0;
            if (sprite != null) SetUpSprite();
        }

        #endregion

        #region Mono methods

        private void Start()
        {
            sprite = GetComponent<SpriteRenderer>();
            SetUpSprite();
        }

        private void Update()
        {
            if (!spentLifetime.HasValue) return;

            spentLifetime += Time.deltaTime;
            if (spentLifetime < lifetime) return;

            owner.ReportStimulusDied(this);
            spentLifetime = null;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (wasFocusedOn || !spentLifetime.HasValue) return;
            var pointer = other.gameObject.GetComponent<Pointer>();
            if (pointer == null) return;
            wasFocusedOn = true;
            owner.ReportFocusedOn(this, spentLifetime.Value);
        }

        #endregion

        #region Helpers

        private void SetUpSprite()
        {
            switch (type)
            {
                case StimuliType.Bird:
                    sprite.sprite = bird;
                    break;
                case StimuliType.Owl:
                    sprite.sprite = owl;
                    break;
                case StimuliType.Rainbow:
                    sprite.sprite = rainbow;
                    break;
                case StimuliType.RainCloud:
                    sprite.sprite = rainCloud;
                    break;
                case StimuliType.Umbrella:
                    sprite.sprite = umbrella;
                    break;
                default:
                    sprite.sprite = bee;
                    break;
            }
        }

        #endregion
    }
}
using RasHack.GapOverlap.Main.Inputs;
using RasHack.GapOverlap.Main.Stimuli.Animaition;
using UnityEngine;

namespace RasHack.GapOverlap.Main.Stimuli
{
    public class Stimulus : ScalableStimulus
    {
        #region Sprites

        [SerializeField] private Sprite bee;
        [SerializeField] private AudioClip beeSound;

        [SerializeField] private Sprite bird;
        [SerializeField] private AudioClip birdSound;

        [SerializeField] private Sprite owl;
        [SerializeField] private AudioClip owlSound;

        [SerializeField] private Sprite rainbow;
        [SerializeField] private AudioClip rainbowSound;

        [SerializeField] private Sprite rainCloud;
        [SerializeField] private AudioClip rainCloudSound;

        [SerializeField] private Sprite umbrella;
        [SerializeField] private AudioClip umbrellaSound;

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

            DoFadeIn(lifetime, owner.FadeInOut, owner.RotationFactor);
        }

        #endregion

        #region Mono methods

        private void Start()
        {
            sprite = GetComponent<SpriteRenderer>();
            SetUpSprite();

            var audioSource = GetComponent<AudioSource>();
            switch (type)
            {
                case StimuliType.Bird:
                    audioSource.clip = birdSound;
                    break;
                case StimuliType.Owl:
                    audioSource.clip = owlSound;
                    break;
                case StimuliType.Rainbow:
                    audioSource.clip = rainbowSound;
                    break;
                case StimuliType.RainCloud:
                    audioSource.clip = rainCloudSound;
                    break;
                case StimuliType.Umbrella:
                    audioSource.clip = umbrellaSound;
                    break;
                default:
                    audioSource.clip = beeSound;
                    break;
            }

            audioSource.Play();
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
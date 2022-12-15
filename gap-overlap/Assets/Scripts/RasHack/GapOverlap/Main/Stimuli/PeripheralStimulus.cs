using RasHack.GapOverlap.Main.Inputs;
using UnityEngine;

namespace RasHack.GapOverlap.Main.Stimuli
{
    public class PeripheralStimulus : ScalableStimulus
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

        #endregion

        #region Provided fields from simulator

        private StimuliType type;
        private Task.Task owner;
        private float lifetime;

        #endregion

        #region API

        public int TaskOrder => owner.TaskOrder;

        public void StartSimulating(StimuliType type, StimulusSide side, Task.Task owner, float lifetime)
        {
            StartSimulating(type, side, owner, lifetime, owner.FadeInOut);
        }

        public void StartSimulating(StimuliType type, StimulusSide side, Task.Task owner, float lifetime, float fadeIn)
        {
            this.type = type;
            this.owner = owner;
            detectable.AreaScreenSide = side;

            spentLifetime = 0;
            this.lifetime = lifetime;
            if (sprite != null) SetUpSprite();

            detectable.RegisterOnDetect(owner.Owner, OnPeripheralPointerDetection, OnPeripheralPointerGettingCloser);
            DoFadeIn(lifetime, fadeIn, owner.FadeInOut, owner.RotationFactor);
        }

        public override float ShortenAnimation(float shorterLifetime, bool keepIdling)
        {
            if (!spentLifetime.HasValue) return 0f;
            var remaining = base.ShortenAnimation(shorterLifetime, keepIdling);
            lifetime = spentLifetime.Value + remaining;
            return remaining;
        }

        #endregion

        #region Mono methods

        private void Start()
        {
            sprite = imageToRotate.GetComponent<SpriteRenderer>();
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

            if (owner.Owner.Settings.SoundEnabled) audioSource.Play();
            owner.Owner.Sampler.StartPeripheral(this);
        }

        private void Update()
        {
            if (!spentLifetime.HasValue) return;

            spentLifetime += Time.deltaTime;
            if (spentLifetime < lifetime) return;

            owner.Owner.Sampler.CompletePeripheral();
            owner.ReportPeripheralStimulusDied(this);
            spentLifetime = null;
        }

        private void OnPeripheralPointerDetection(Pointer pointer)
        {
            owner.ReportFocusedOnPeripheral(this, pointer.Eye, spentLifetime.GetValueOrDefault(lifetime));
        }

        private void OnPeripheralPointerGettingCloser(Pointer pointer)
        {
            owner.ReportPeripheralGotCloser(this, pointer.Eye, pointer);
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
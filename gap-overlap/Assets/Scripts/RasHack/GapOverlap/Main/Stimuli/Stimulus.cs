using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RasHack.GapOverlap.Main.Stimuli
{
    public class Stimulus : MonoBehaviour
    {
        [SerializeField] private Sprite bee;
        [SerializeField] private Sprite octopus;
        [SerializeField] private Sprite rainbow;
        [SerializeField] private Sprite rainCloud;
        [SerializeField] private Sprite umbrella;
        [SerializeField] private Sprite yellowBird;
        
        private StimuliType type;
        private Simulator owner;
        private SpriteRenderer renderer;

        // Start is called before the first frame update
        private void Start()
        {
            renderer = GetComponent<SpriteRenderer>();
            SetUpSprite();
        }

        public void StartSimulating(StimuliType type, Simulator owner)
        {
            this.type = type;
            this.owner = owner;
            if (renderer != null)
            {
                SetUpSprite();
            }
        }

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
    }
}
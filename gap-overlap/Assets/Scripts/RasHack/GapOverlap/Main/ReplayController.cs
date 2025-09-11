using System;
using System.IO;
using Newtonsoft.Json;
using RasHack.GapOverlap.Main.Result;
using RasHack.GapOverlap.Main.Settings;
using UnityEngine;

namespace RasHack.GapOverlap.Main
{
    public class ReplayedTest
    {
        #region Fields

        public readonly SampledTest test;
        private float spentTime;
        private int currentSampleIndex;

        #endregion

        #region Constructors

        public ReplayedTest(SampledTest test)
        {
            this.test = test;
            currentSampleIndex = 0;
            spentTime = test.Samples.AllSamples[currentSampleIndex].Time;
        }

        #endregion

        #region Methods

        public void Tick(float deltaTime, Action<Sample> onNextSample)
        {
            var toTime = spentTime + deltaTime;
            while (currentSampleIndex < test.Samples.AllSamples.Count)
            {
                var sample = test.Samples.AllSamples[currentSampleIndex];
                if (toTime < sample.Time) break;

                currentSampleIndex++;
                spentTime = sample.Time;
                onNextSample.Invoke(sample);
            }

            spentTime = toTime;
        }

        #endregion
    }

    public class ReplayController : MonoBehaviour
    {
        #region Fields

        [SerializeField] private SpriteRenderer bottomLeft;
        [SerializeField] private SpriteRenderer bottomRight;
        [SerializeField] private SpriteRenderer topLeft;
        [SerializeField] private SpriteRenderer topRight;

        private MainSettings settings = new MainSettings();

        private Scaler debugScaler;

        private ReplayedTest toReplay;

        #endregion

        #region Properties

        private bool ShowPointer => settings.ShowPointer;

        #endregion

        #region API

        public void StartReplay()
        {
            toReplay = null;
            var bytes = File.ReadAllBytes("../gap-overlap-results/ANIKA IZDA/ANIKAIZDA_57946701-fd62-444c-b4af-43570d1c0b56.json");

            var deserializer = JsonSerializer.CreateDefault();

            using var reader = new JsonTextReader(new StreamReader(new MemoryStream(bytes)));
            {
                toReplay = new ReplayedTest(deserializer.Deserialize<SampledTest>(reader));
            }

            var mainCamera = Camera.main;
            var screen = ScreenArea.WholeScreen;
            var overlayScreen = new ScreenArea((int)toReplay.test.ScreenPixelsX, (int)toReplay.test.ScreenPixelsY);
            screen = screen.Overlay(settings.ReferencePoint.ScreenDiagonalInInches, (float)toReplay.test.ScreenDiagonalInInches, overlayScreen);
            debugScaler = new Scaler(mainCamera, -2, settings, screen);
        }

        #endregion

        #region Methods

        public void Tick(float deltaTime)
        {
            if (toReplay == null) return;

            toReplay.Tick(deltaTime, OnNextSample);
        }

        private void OnNextSample(Sample sample)
        {
            Debug.Log($"Replaying sample at {sample.Time}s: {sample.Task.TaskType}");
        }

        private void UpdateDebugVisibility()
        {
            bottomLeft.enabled = ShowPointer;
            bottomRight.enabled = ShowPointer;
            topLeft.enabled = ShowPointer;
            topRight.enabled = ShowPointer;
        }

        private void Awake()
        {
            var loadedSettings = MainSettings.Load();
            if (loadedSettings != null) settings = loadedSettings;
        }

        private void Update()
        {
            var deltaTime = Time.deltaTime;
            Tick(deltaTime);
            if (debugScaler != null) UpdateBounds();
            UpdateDebugVisibility();
        }

        private void UpdateBounds()
        {
            topLeft.transform.position = debugScaler.TopLeft;
            bottomLeft.transform.position = debugScaler.BottomLeft;
            bottomRight.transform.position = debugScaler.BottomRight;
            topRight.transform.position = debugScaler.TopRight;
        }

        #endregion

    }
}
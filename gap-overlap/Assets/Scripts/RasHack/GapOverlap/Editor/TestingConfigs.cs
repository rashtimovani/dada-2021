using System.IO;
using RasHack.GapOverlap.Main.Settings;
using RasHack.GapOverlap.Main.Task;
using UnityEditor;
using UnityEngine;

namespace RasHack.GapOverlap.Editor
{
    public class TestingConfigs : MonoBehaviour
    {
        #region Menu entries

        [MenuItem("Gap-Overlap/Restore previous settings")]
        public static void RestorePrevious()
        {
            if (!File.Exists(BACK_UP_FILENAME)) return;

            File.Delete(MainSettings.FILENAME);
            File.Copy(BACK_UP_FILENAME, MainSettings.FILENAME);
            File.Delete(BACK_UP_FILENAME);
            Debug.Log($"{BACK_UP_FILENAME} restored from following file {MainSettings.FILENAME}...");
        }

        [MenuItem("Gap-Overlap/Test only Gaps")]
        public static void SetUpGapsTestingSettings()
        {
            BackUpSettings();

            new MainSettings
            {
                LastUsedName = "GapTester",
                ShowPointer = true,
                TaskCount = new TaskCount
                    { Gaps = 3, LeftGaps = 2, Overlaps = 0, LeftOverlaps = 0, Baselines = 0, LeftBaselines = 0 }
            }.Store();
            Debug.Log("Testing only gaps!");
        }

        [MenuItem("Gap-Overlap/Test only Overlaps")]
        public static void SetUpOverlapsTestingSettings()
        {
            BackUpSettings();

            new MainSettings
            {
                LastUsedName = "OverlapTester",
                ShowPointer = true,
                TaskCount = new TaskCount
                    { Gaps = 0, LeftGaps = 0, Overlaps = 3, LeftOverlaps = 2, Baselines = 0, LeftBaselines = 0 }
            }.Store();
            Debug.Log("Testing only overlaps!");
        }

        [MenuItem("Gap-Overlap/Test only Baselines")]
        public static void SetUpBaselineTestingSettings()
        {
            BackUpSettings();

            new MainSettings
            {
                LastUsedName = "BaselineTester",
                ShowPointer = true,
                TaskCount = new TaskCount
                    { Gaps = 0, LeftGaps = 0, Overlaps = 0, LeftOverlaps = 0, Baselines = 3, LeftBaselines = 2 }
            }.Store();
            Debug.Log("Testing only baselines!");
        }

        #endregion

        #region Helpers

        private const string BACK_UP_FILENAME = MainSettings.FILENAME + ".back-up";

        private static void BackUpSettings()
        {
            if (!File.Exists(MainSettings.FILENAME)) return;
            if (File.Exists(BACK_UP_FILENAME)) return;

            File.Delete(BACK_UP_FILENAME);
            File.Copy(MainSettings.FILENAME, BACK_UP_FILENAME);
            Debug.Log($"{MainSettings.FILENAME} backed up to following file {BACK_UP_FILENAME}...");
        }

        #endregion
    }
}
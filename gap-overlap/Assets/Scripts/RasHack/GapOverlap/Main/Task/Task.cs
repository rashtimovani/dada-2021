using RasHack.GapOverlap.Main.Stimuli;

namespace RasHack.GapOverlap.Main.Task
{
    public interface Task
    {
        void StartTask(Simulator owner);
        
        void ReportCentralStimulusDied(CentralStimulus central);

        void ReportStimulusDied(Stimulus active);
    }
}
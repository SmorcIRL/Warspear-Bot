namespace WarspearBot.Models
{
    public class EngineInitConfiguration
    {
        public (int W, int H) WindowExpectedSize { get; set; }

        public bool MoveWindowOnStart { get; set; }

        public (int X, int Y) WindowInitialLocation { get; set; }

        public bool PinWindowToTop { get; set; }

        public (int X, int Y) GameScreenLeftTopOffset { get; set; }

        public (int W, int H) GameScreenSize { get; set; }

        public string ScenarioName { get; set; }

        public string WindowName { get; set; }

        public (int X, int Y) CollectLootClickLocation { get; set; }

        public (int X, int Y) Skill2Location { get; set; }

        public (int X, int Y) CloseEnemySelectionClickLocation { get; set; }

        public (int X, int Y) DeclineInventoryExtendingClickLocation { get; set; }

        public (int X, int Y) CloseLootWindowClickLocation { get; set; }
    }
}
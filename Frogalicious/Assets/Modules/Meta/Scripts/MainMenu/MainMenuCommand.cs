using System;

namespace Frog.Meta.MainMenu
{
    public readonly struct MainMenuCommand
    {
        public readonly MainMenuCommandId Id;
        public readonly int LevelIndex;

        private MainMenuCommand(MainMenuCommandId id, int levelIndex)
        {
            Id = id;
            LevelIndex = levelIndex;
        }

        public static MainMenuCommand PlayLevel(int index) => new MainMenuCommand(MainMenuCommandId.PlayLevel, index);
        public static MainMenuCommand Continue() => new MainMenuCommand(MainMenuCommandId.Continue, -1);
        public static MainMenuCommand ExitGame() => new MainMenuCommand(MainMenuCommandId.ExitGame, -1);

        public static MainMenuCommand FromUiCommand(MainMenuUi.Command uiCommand)
        {
            return uiCommand switch
            {
                MainMenuUi.Command.Continue => Continue(),
                MainMenuUi.Command.Exit => ExitGame(),
                _ => throw new ArgumentOutOfRangeException(nameof(uiCommand), uiCommand, null),
            };
        }
    }

    public enum MainMenuCommandId
    {
        PlayLevel,
        Continue,
        ExitGame,
    }
}
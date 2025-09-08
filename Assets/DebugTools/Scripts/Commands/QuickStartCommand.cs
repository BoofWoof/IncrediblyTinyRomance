using UnityEngine;

namespace DebugTools.DeveloperConsole.Commands
{
    [CreateAssetMenu(fileName = "NewQuickStartCommand", menuName = "DebugCommands/QuickStartCommand")]
    public class QuickStartCommand : ConsoleCommand
    {
        public override bool Process(string[] args)
        {
            HudScript.instance.SkipTutorial();

            BalconyEventsScript.instance.StartSystem();

            if (ShutterScript.instance.ShuttersLowered)
            {
                ShutterScript.instance.ActivateShutters();
            }

            if (args.Length <= 0) return true;

            if (int.Parse(args[0]) == 2)
            {
                QuestManager.ChangeQuest("Visions");
                AppMenuScript.SetAppsRevealed(2);
            }

            return true;
        }
    }
}
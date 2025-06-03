using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;


namespace DebugTools.DeveloperConsole.Commands
{
    public class DeveloperConsoleBehaviour : MonoBehaviour
    {
        [SerializeField] private string prefix = string.Empty;
        [SerializeField] private ConsoleCommand[] commands = new ConsoleCommand[0];

        [Header("UI")]
        [SerializeField] private GameObject uiCanvas = null;
        [SerializeField] private TMP_InputField inputField = null;

        private float pausedTimeScale;

        private static DeveloperConsoleBehaviour instance;

        private DeveloperConsole developerConsole;

        private DeveloperConsole DeveleoperConsole
        {
            get
            {
                if (developerConsole != null) { return developerConsole; }
                return developerConsole = new DeveloperConsole(prefix, commands);
            }
        }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;

            DontDestroyOnLoad(gameObject);
        }

        public void Toggle(InputAction.CallbackContext context)
        {
            if (!context.action.triggered) { return; }

            if (uiCanvas.activeSelf)
            {
                //Time.timeScale = pausedTimeScale;
                uiCanvas.SetActive(false);
            } else
            {
                //pausedTimeScale = Time.timeScale;
                //Time.timeScale = 0;
                uiCanvas.SetActive(true);
                inputField.ActivateInputField();
            }
        }
        public void SubmitCommand(InputAction.CallbackContext context)
        {
            if (!context.action.triggered) { return; }

            if (uiCanvas.activeSelf)
            {
                ProcessCommand(inputField.text);
            }
        }

        public void ProcessCommand(string inputValue)
        {
            DeveleoperConsole.ProcessCommand(inputValue);

            inputField.text = string.Empty;
        }
    }
}

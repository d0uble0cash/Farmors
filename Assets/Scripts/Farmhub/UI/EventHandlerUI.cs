using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using TMPro;
public class EventHandlerUI : MonoBehaviour{
    public InputActionAsset InputActions;
    public GameObject invScreen, mapScreen, journalScreen, optionsScreen;
    private InputAction invActionButton, mapActionButton, journalActionButton, optionsActionButton, escapeActionButton;
    [Header("Clock UI")]
    [SerializeField] private TMP_Text clockText;
    private float elapsedTime;
    [Header("How much time is in a day")]
    [SerializeField] private float dayLength = 86400f;
    [Header("How fast time should pass")]
    [SerializeField] private float timeScale = 2.0f;
    private int currentDay = 1;
    private void Awake(){
        invActionButton = InputSystem.actions.FindAction("Inventory");
        mapActionButton = InputSystem.actions.FindAction("Map");
        journalActionButton = InputSystem.actions.FindAction("Journal");
        optionsActionButton = InputSystem.actions.FindAction("OptionsMenu");
        escapeActionButton = InputSystem.actions.FindAction("EscKey");
        elapsedTime = 8 * 3600f; // Start at 8:00 AM
    }

    private void Update(){
        /*InputSystem.onActionChange +=
        (obj, change) => {
            
            bool invwas = invActionButton.WasPressedThisFrame();
            bool mapwas = mapActionButton.WasPressedThisFrame();
            bool journalwas = journalActionButton.WasPressedThisFrame();
            bool optionswas = optionsActionButton.WasPressedThisFrame();
            bool escapewas = escapeActionButton.WasPressedThisFrame();
            switch (change){
                case invActionButton:
                 invScreen.SetActive(true);
                    disablePlayer();
                    break;
                case mapActionButton:
                    mapScreen.SetActive(true);
                    disablePlayer();
                    break;
                case journalActionButton:
                    journalScreen.SetActive(true);
                    disablePlayer();
                    break;
                case optionsActionButton:
                    optionsScreen.SetActive(true);
                    disablePlayer();
                    break;
                case escapeActionButton:
                    disableUI();
                    break;
            }
        };*/
        
        elapsedTime += Time.deltaTime * timeScale;
        UpdateClockUI();

        if(invActionButton.WasPressedThisFrame()){
            invScreen.SetActive(true);
            enableUIActionMap();
        }
        else if(mapActionButton.WasPressedThisFrame()){
            mapScreen.SetActive(true);
            enableUIActionMap();
        }
        else if(journalActionButton.WasPressedThisFrame()){
            journalScreen.SetActive(true);
            enableUIActionMap();
        }
        else if (optionsActionButton.WasPressedThisFrame()){
            optionsScreen.SetActive(true);
            enableUIActionMap();
        }
        else if(escapeActionButton.WasPressedThisFrame()){
            enablePlayerActionMap();
        }
    }

    public void enableUIActionMap(){
        InputActions.FindActionMap("Player").Disable();
        InputActions.FindActionMap("UI").Enable();
    }
    public void enablePlayerActionMap(){
        journalScreen.SetActive(false);
        mapScreen.SetActive(false);
        invScreen.SetActive(false);
        optionsScreen.SetActive(false);
        InputActions.FindActionMap("Player").Enable();
        InputActions.FindActionMap("UI").Disable();
    }
    public void UpdateClockUI(){
        currentDay = Mathf.FloorToInt(elapsedTime / dayLength) + 1;
        elapsedTime %= dayLength; // Loop back to 0 after a full day
        int hours = Mathf.FloorToInt(elapsedTime / 3600);
        int minutes = Mathf.FloorToInt((elapsedTime % 3600) / 60);
        clockText.text = string.Format("DAY:{0} {1:00}:{2:00}", currentDay, hours, minutes);
    }
}

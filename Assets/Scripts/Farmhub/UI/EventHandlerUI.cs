using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
public class EventHandlerUI : MonoBehaviour{
    public InputActionAsset InputActions;
    public GameObject invScreen, mapScreen, journalScreen, optionsScreen;
    private InputAction invActionButton, mapActionButton, journalActionButton, optionsActionButton, escapeActionButton;
    private void Awake(){
        invActionButton = InputSystem.actions.FindAction("Inventory");
        mapActionButton = InputSystem.actions.FindAction("Map");
        journalActionButton = InputSystem.actions.FindAction("Journal");
        optionsActionButton = InputSystem.actions.FindAction("OptionsMenu");
        escapeActionButton = InputSystem.actions.FindAction("EscKey");
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
}

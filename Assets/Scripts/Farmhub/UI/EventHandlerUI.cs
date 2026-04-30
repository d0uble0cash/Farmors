using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class EventHandlerUI : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionAsset inputActions;

    private InputAction invActionButton;
    private InputAction mapActionButton;
    private InputAction journalActionButton;
    private InputAction optionsActionButton;
    private InputAction escapeActionButton;

    [Header("Player Screens")]
    [SerializeField] private GameObject invScreen;
    [SerializeField] private GameObject mapScreen;
    [SerializeField] private GameObject journalScreen;
    [SerializeField] private GameObject optionsScreen;

    [Header("World Interaction Screens")]
    [SerializeField] private GameObject vendorScreen;
    [SerializeField] private GameObject chestScreen;
    [SerializeField] private GameObject blacksmithScreen;

    [Header("Clock UI")]
    [SerializeField] private TMP_Text clockText;

    [Header("Time Settings")]
    [SerializeField] private float dayLength = 86400f;
    [SerializeField] private float timeScale = 2.0f;

    private float elapsedTime;
    private int currentDay = 1;

    private void Awake()
    {
        invActionButton = InputSystem.actions.FindAction("Inventory");
        mapActionButton = InputSystem.actions.FindAction("Map");
        journalActionButton = InputSystem.actions.FindAction("Journal");
        optionsActionButton = InputSystem.actions.FindAction("OptionsMenu");
        escapeActionButton = InputSystem.actions.FindAction("EscKey");

        elapsedTime = 8 * 3600f;

        CloseAllScreens();
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime * timeScale;
        UpdateClockUI();

        if (invActionButton != null && invActionButton.WasPressedThisFrame())
        {
            OpenScreen(invScreen);
        }
        else if (mapActionButton != null && mapActionButton.WasPressedThisFrame())
        {
            OpenScreen(mapScreen);
        }
        else if (journalActionButton != null && journalActionButton.WasPressedThisFrame())
        {
            OpenScreen(journalScreen);
        }
        else if (optionsActionButton != null && optionsActionButton.WasPressedThisFrame())
        {
            OpenScreen(optionsScreen);
        }
        else if (escapeActionButton != null && escapeActionButton.WasPressedThisFrame())
        {
            EnablePlayerActionMap();
        }
    }

    public void OpenScreen(GameObject screen)
    {
        if (screen == null)
            return;

        CloseAllScreens();
        screen.SetActive(true);
        EnableUIActionMap();
    }

    public void CloseAllScreens()
    {
        if (invScreen != null) invScreen.SetActive(false);
        if (mapScreen != null) mapScreen.SetActive(false);
        if (journalScreen != null) journalScreen.SetActive(false);
        if (optionsScreen != null) optionsScreen.SetActive(false);

        if (vendorScreen != null) vendorScreen.SetActive(false);
        if (chestScreen != null) chestScreen.SetActive(false);
        if (blacksmithScreen != null) blacksmithScreen.SetActive(false);
    }

    public void EnableUIActionMap()
    {
        if (inputActions == null)
            return;

        InputActionMap playerMap = inputActions.FindActionMap("Player");
        InputActionMap uiMap = inputActions.FindActionMap("UI");

        if (playerMap != null) playerMap.Disable();
        if (uiMap != null) uiMap.Enable();
    }

    public void EnablePlayerActionMap()
    {
        CloseAllScreens();

        if (inputActions == null)
            return;

        InputActionMap playerMap = inputActions.FindActionMap("Player");
        InputActionMap uiMap = inputActions.FindActionMap("UI");

        if (playerMap != null) playerMap.Enable();
        if (uiMap != null) uiMap.Disable();
    }

    public void OpenVendorUI()
    {
        OpenScreen(vendorScreen);
    }

    public void OpenChestUI()
    {
        OpenScreen(chestScreen);
    }

    public void OpenBlacksmithUI()
    {
        OpenScreen(blacksmithScreen);
    }

    private void UpdateClockUI()
    {
        if (clockText == null)
            return;

        currentDay = Mathf.FloorToInt(elapsedTime / dayLength) + 1;
        elapsedTime %= dayLength;

        int hours = Mathf.FloorToInt(elapsedTime / 3600f);
        int minutes = Mathf.FloorToInt((elapsedTime % 3600f) / 60f);

        clockText.text = $"DAY:{currentDay} {hours:00}:{minutes:00}";
    }
}
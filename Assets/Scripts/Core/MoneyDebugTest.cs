using UnityEngine;
using UnityEngine.InputSystem;

public class MoneyDebug : MonoBehaviour
{
    void Update()
    {
        if (Keyboard.current.mKey.wasPressedThisFrame)
        {
            GameState.I.AddMoney(10);
            Debug.Log("Money: " + GameState.I.money);
        }

        if (Keyboard.current.f5Key.wasPressedThisFrame)
        {
            SaveSystem.I.Save();
        }

        if (Keyboard.current.f9Key.wasPressedThisFrame)
        {
            SaveSystem.I.Load();
            Debug.Log("Money after load: " + GameState.I.money);
        }
    }
}
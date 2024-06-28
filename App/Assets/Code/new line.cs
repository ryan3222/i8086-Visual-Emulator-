using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InputFieldNewLine : MonoBehaviour
{
    public TMP_InputField inputField;

    private void Start()
    {
        inputField.onValueChanged.AddListener(OnValueChanged);
    }

    private void OnValueChanged(string newValue)
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            inputField.text += "\n";
        }
    }
}
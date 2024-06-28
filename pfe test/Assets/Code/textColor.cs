using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class textColor : MonoBehaviour
{
   

[SerializeField] private TMP_InputField inputField;

    private string[] keywords = new string[] {"if", "else", "while", "for", "switch"};

    private void Start()
    {
        inputField.onValueChanged.AddListener(OnInputFieldValueChanged);
    }

    private void OnInputFieldValueChanged(string newValue)
    {
        foreach (string keyword in keywords)
        {
            newValue = newValue.Replace(keyword, $"<color=red>{keyword}</color>");
        }

        inputField.text = newValue;
    }

}

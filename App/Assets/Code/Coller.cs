using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;
using System.IO;
using UnityEngine.EventSystems;

public class Coller : MonoBehaviour
{

    public TMP_InputField myInputField;

    public void OnButtonClick()
    {
        if (GUIUtility.systemCopyBuffer.Length > 0)
        {
            myInputField.text = GUIUtility.systemCopyBuffer;
        }
    }


}

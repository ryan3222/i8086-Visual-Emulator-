using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;
using System.IO;
using UnityEngine.EventSystems;


public class New : MonoBehaviour
{
    public TMP_InputField myInputField;

    public void OnButtonClick()
    {
        myInputField.text = string.Empty;
    }

}

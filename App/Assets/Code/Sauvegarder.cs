using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;
using System.IO;
using UnityEngine.EventSystems;
using SFB;


public class Sauvegarder : MonoBehaviour
{
    public TMP_InputField myInputField;

    public void OnButtonClick()
    {
        string code = myInputField.text;

        if (!string.IsNullOrEmpty(code))
        {
            // Ouvre une bo�te de dialogue pour choisir l'emplacement de sauvegarde
            string savePath = StandaloneFileBrowser.SaveFilePanel("Save Code", "", "savedCode", "txt");

            // V�rifie si l'utilisateur a s�lectionn� un emplacement de sauvegarde valide
            if (!string.IsNullOrEmpty(savePath))
            {
                // �crit le contenu du code dans le fichier de sauvegarde
                File.WriteAllText(savePath, code);
            }
        }
    }
}

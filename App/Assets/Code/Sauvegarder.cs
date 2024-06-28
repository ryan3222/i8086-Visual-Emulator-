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
            // Ouvre une boîte de dialogue pour choisir l'emplacement de sauvegarde
            string savePath = StandaloneFileBrowser.SaveFilePanel("Save Code", "", "savedCode", "txt");

            // Vérifie si l'utilisateur a sélectionné un emplacement de sauvegarde valide
            if (!string.IsNullOrEmpty(savePath))
            {
                // Écrit le contenu du code dans le fichier de sauvegarde
                File.WriteAllText(savePath, code);
            }
        }
    }
}

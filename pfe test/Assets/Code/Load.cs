using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;
using System.IO;
using UnityEngine.EventSystems;
using SFB;

public class Load : MonoBehaviour
{
    public TMP_InputField myInputField;

    public void OnButtonClick()
    {
        string code = myInputField.text;
        // Ouvre une boîte de dialogue pour choisir le fichier à charger
        string[] loadPaths = StandaloneFileBrowser.OpenFilePanel("Load Code", "", "txt", false);

        // Vérifie si l'utilisateur a sélectionné un fichier valide
        if (loadPaths.Length > 0)
        {
            string loadPath = loadPaths[0];

            // Lit le contenu du fichier de chargement
            string loadedCode = File.ReadAllText(loadPath);

            // Place le code chargé dans le champ de saisie
            myInputField.text = loadedCode;
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Setram : MonoBehaviour
{
    
    public TMP_InputField searchtext;
    public AnimationController an;
     public void OnButtonClick()
    {

       string input=searchtext.text;

        string[] parts = input.Split(':');

// Trim any leading or trailing whitespace from the parts
     string part1 = parts[0].Trim();
     string part2 = parts[1].Trim();

     int intValue1, intValue2;

// Attempt to parse each part as an integer
    bool success1 = int.TryParse(part1, out intValue1);
     bool success2 = int.TryParse(part2, out intValue2);

// Check if both conversions were successful
    if (success1 && success2)
   {
    // Both values were successfully converted to integers, do something with them
     Debug.Log("Parsed values: " + intValue1 + ", " + intValue2);
   }
   else
   {
    // At least one of the values could not be converted to an integer
      Debug.Log("Could not parse values");
}
       an.GetRam(Emulateur.cpu_core.memory,intValue1,intValue2);

    }
}

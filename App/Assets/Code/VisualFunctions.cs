using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualFunctions : MonoBehaviour
{
    public static string IntToHex(int value)
    {
        return "" + value.ToString("X4") + "H";
    }

     public static string IntToHexByte(int value)
    {
        return "" + value.ToString("X2") + "H";
    }
}

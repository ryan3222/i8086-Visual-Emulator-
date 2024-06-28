using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayFlags : MonoBehaviour
{
    public void OnButtonClick()
    {
      
   
    
     
        Debug.Log("Aux : "+ Emulateur.cpu_core.registers.flagRegister.GetFlag(Flags.Auxilliary)+" Carry : "+ Emulateur.cpu_core.registers.flagRegister.GetFlag(Flags.Carry)+" Directional : "+ Emulateur.cpu_core.registers.flagRegister.GetFlag(Flags.Directional)+" Interrupt : "+ Emulateur.cpu_core.registers.flagRegister.GetFlag(Flags.Interrupt)+" Overflow : "+ Emulateur.cpu_core.registers.flagRegister.GetFlag(Flags.Overflow)+" Parity : "+ Emulateur.cpu_core.registers.flagRegister.GetFlag(Flags.Parity)+" Sign : "+ Emulateur.cpu_core.registers.flagRegister.GetFlag(Flags.Sign)+" Trap : "+ Emulateur.cpu_core.registers.flagRegister.GetFlag(Flags.Trap)+" Zero : "+ Emulateur.cpu_core.registers.flagRegister.GetFlag(Flags.Zero));
        Debug.Log("PSW: "+Emulateur.cpu_core.registers.flagRegister.Get());

    }
    
}

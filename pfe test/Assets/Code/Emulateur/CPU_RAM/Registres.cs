using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections.Generic;

public class Registres {
    
    
    public Dictionary<string, Register> regs;
    public FlagRegister flagRegister;

    public Registres()
    {
        regs = new Dictionary<string, Register>(StringComparer.OrdinalIgnoreCase)
        {
            { "AX", new Register() },
            { "BX", new Register() },
            { "CX", new Register() },
            { "DX", new Register() },
            { "IP", new Register() },
            { "DI", new Register() },
            { "SI", new Register() },
            { "BP", new Register() },
            { "SP", new Register(500) },
            { "DS", new Register(1000) },
            { "ES", new Register() },
            { "CS", new Register(0) },
            { "SS", new Register(2000) },
             { "PS", new Register() }
             
        };

        flagRegister=new FlagRegister();

    }




}





public class Register
{

    
    private int value;
    private int h;
    private int l;
     
    public Register(ushort initial = 0) //ushort is 16bit int
    {
        this.value = initial;
        this.h = 0;
        this.l = 0;
    }


    public int Get(char? half = null)
    {
        if (!half.HasValue)
        {
            return this.value;
        }

        if (char.ToUpper(half.Value) == 'L')
        {
            return this.l;
        }

        if (char.ToUpper(half.Value) == 'H')
        {
            return this.h;
        }

        return -1;
    }

    public void Set(int value, char? half = null)
    {
        if (half.HasValue)
        {
            if (value > Math.Pow(2, 8))   //range is -127 to 127
            {
                throw new  Exception("Can't set more than 8 bit value to an 8 bit register");
            }
        }
        else if (value > Math.Pow(2, 16))   
        {
            throw new Exception("Can't set more than 8 bit value to an 16 bit register");
        }
        if (!half.HasValue)
        {
            this.value = value;
            this.l = value & 255;
            this.h = value >> 8;
            return;
        }

        if (char.ToUpper(half.Value) == 'L')
        {
            this.l = value;
        }
        else if (char.ToUpper(half.Value) == 'H')
        {
            this.h = value;
        }

        this.value = (this.h << 8) + this.l;
    }

}

public class FlagRegister : Register
{
    public FlagRegister(ushort initial = 0) : base(initial)
    {
    }

    public void SetFlag(int flag)
    {
        this.Set(this.Get() | flag);
    }

    public void UnsetFlag(int flag)
    {
        this.Set(this.Get() & ~flag);
    }

    public int GetFlag(int flag)
    {
        return (this.Get() & flag) == 0 ? 0 : 1;
    }
}

public static class Flags
{
    public static int Sign = Bitmask(7);
    public static int Zero = Bitmask(6);
    public static int Auxilliary = Bitmask(4);
    public static int Carry = Bitmask(0);
    public static int Overflow = Bitmask(11);
    public static int Directional = Bitmask(10);
    public static int Interrupt = Bitmask(9);
    public static int Trap = Bitmask(8);
    public static int Parity = Bitmask(2);

    private static int Bitmask(int bit)
    {
        return 1 << bit;    //to get a binary number with all zeros but for the bit's position set to 1 example , bitmask(3)=100 = 4 in decimal
    }
}







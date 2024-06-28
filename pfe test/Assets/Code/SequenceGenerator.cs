using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[DefaultExecutionOrder(-1)]
public class SequenceGenerator : MonoBehaviour
{





    //1 droite gauche / up to down 
    public AnimationController an;
    public AnimationController anp;
    bool FetchNext = true;
    int nbrFil = 0;
    Stack<int> historiqueFIvert = Emulateur.historiqueFIvert;
    Stack<int> historiqueFIbleu = Emulateur.historiqueFIbleu;

    int n;
    public void fetchInstruction(Memory mem, int segment, int adr, bool isFirst)
    {  //generer la sequence d'animation pour la recuperation des instructions en utilisanat fetchmemoire, et on ajoutant animation pour la fil d'instructions avec selectionement avec des couleurs les instructions present dans la fil 
        an.clearQueue();
        Debug.Log("fetching");
        //Instruction current_instruction=Emulateur.cpu_core.memory.Get((segment<<4)+adr).Instruction;


        Instruction current_instruction = Emulateur.cpu_core.memory.Get((Emulateur.cpu_core.GetRegValue("IP") + Emulateur.cpu_core.GetRegValue("CS"))).Instruction;
        Instruction previous_instruction;
        int plength;
        try
        {
            previous_instruction = Emulateur.cpu_core.memory.Get((segment << 4) + adr - 1).Instruction;
            plength = previous_instruction.Length;
        }
        catch (System.Exception)
        {
            plength = 0;

        }

        if (an.statusFilInstruction() == 6 || !isFirst)
        {   //seulement si la file est vide

            Debug.Log("first");
            List<string> l = new List<string>();
            if (isFirst)
                l.AddRange(updateRegs());
       //     l.Add("ARLU");
         //   l.Add("RT5");
           // l.Add("ah6");
            //l.Add("dh3");
            //l.Add("bl66");
            //l.Add("dx2556");
            //l.Add("AX;");
            //l.Add("DL;");
            l.Add("CS");
            l.Add("26,0");
            l.Add("27,0");
            l.Add("IP;");
            l.Add("26,0");
            l.Add("27,0");
            l.Add("SOM");
            l.Add("25,0");
            l.Add("23,1");

            //test if instruction queue has space
            //  Debug.Log((segment<<4)+adr);

            int i = current_instruction.Length;
            int k = 0;
            int c = 0;
            while (i >= 2)
            {
                l.AddRange(fetchRam(segment, adr + c, true));
                l.Add("22,1");
                l.Add("21,1");
                l.Add("BYTE" + (6 - k));
                k++;
                l.Add("BYTE" + (6 - k));
                k++;
                i--;
                i--;
                c = c + 2;
                //  Debug.Log("statusFilInstruction"+an.statusFilInstruction());
            }

            n = k;
            if (i > 0)
            {
                l.AddRange(fetchRam(segment, adr + c, false));
                l.Add("22,1");
                l.Add("21,1");
                l.Add("BYTE" + (6 - k));
                n++;
            }
            historiqueFIvert.Push(n);
            nbrFil = 6 - k;
            an.fetchFirstInstruction = true;
            l.Add("IP" + (adr + current_instruction.Length));
            // l.AddRange(updateIp());
            an.activerSequence(l);
            //placer la dans la fil d'instruction
        }
        else
        {

            executeNextInstruction();
            if (FetchNext)
            {

                if (current_instruction.Keyword.ToLower() == "call")
                {  //add jump case later
                    Debug.Log("CALL" + Emulateur.cpu_core.GetRegValue("CS") + Emulateur.cpu_core.ST.GetSymbol(current_instruction.Destination.Value).Address);
                    int nextIp = Emulateur.cpu_core.GetRegValue("CS") + Emulateur.cpu_core.ST.GetSymbol(current_instruction.Destination.Value).Address;
                    Debug.Log("CALL" + nextIp);
                    fetchNextInstruction(mem, segment, nextIp, false);

                }
                else if (current_instruction.Keyword.ToLower() == "ret")
                {
                    int nextIp = (int)Emulateur.cpu_core.memory.Get(Emulateur.cpu_core.GetRegValue("SP") + Emulateur.cpu_core.GetRegValue("SS")).NumericalValue;
                    Debug.Log("RET" + nextIp);
                    fetchNextInstruction(mem, segment, nextIp, false);
                }
                else
                    fetchNextInstruction(mem, segment, adr + current_instruction.Length, false);
            }
            else
                FetchNext = true;
            Debug.Log(an.statusFilInstruction() + " fetced");


        }



    }


    public void fetchNextInstruction(Memory mem, int segment, int adr, bool isCalledFromFirst)  //en paralel pendant l'execution d'une instrution
    {
        int n = 0;
        Instruction current_instruction = Emulateur.cpu_core.memory.Get((segment << 4) + adr).Instruction;
        Instruction previous_instruction;
        Instruction beforePreviousInstruction;
        int plength = 0;
        // Debug.Log("ADRR"+adr+"length"+previous_instruction.Length);
        try
        {
            previous_instruction = Emulateur.cpu_core.memory.Get((segment << 4) + adr - 1).Instruction;
            Debug.Log("PREVIOUS ADR : " + ((segment << 4) + adr - previous_instruction.Length));
            beforePreviousInstruction = Emulateur.cpu_core.memory.Get((segment << 4) + adr - previous_instruction.Length - 1).Instruction;
        }
        catch (System.Exception)
        {
            beforePreviousInstruction = null;

        }

        string ipval = "IP" + adr;
        List<string> l = new List<string>();
        string b = "null";
        if (beforePreviousInstruction != null && !isCalledFromFirst)
        {
            plength = beforePreviousInstruction.Length;
            b = beforePreviousInstruction.getOriginalString();
        }
        Debug.Log("BB current length: " + current_instruction.Length + "previous instruction : " + b + plength + "status : " + (an.statusFilInstruction() + plength));
        if (current_instruction != null && current_instruction.Length <= (an.statusFilInstruction() + plength))
        {
            int k = current_instruction.Length;
            //ya de l'espace dans la fil d'instruction
            Debug.Log("parAAAAAAAAAAAAAAAAAAAAAAAA" + an.statusFilInstruction());
            l.Add("IP;");
            l.Add("CS");
            l.Add("26,0");
            l.Add("27,0");
            l.Add("26,0");
            l.Add("27,0");
            l.Add("SOM");
            l.Add("25,0");
            l.Add("23,1");




            Debug.Log("statusFilInstruction and k " + an.statusFilInstruction() + k);
            int c = 0;
            while (k >= 2)
            {
                l.AddRange(fetchRam(segment, adr + c, true));
                k--;
                k--;
                c = c + 2;
            }

            if (k > 0)
            {
                l.AddRange(fetchRam(segment, adr + c, false));
                l.Add("22,1");
                l.Add("21,1");
            }
            for (int i = 1; i <= current_instruction.Length; i++)
            {

                l.Add("BYTE" + i + ",b");
                Debug.Log("BYTE" + i + ",b");
                n++;
            }
            historiqueFIbleu.Push(n);

            l.Add("IP" + (adr + current_instruction.Length));
            anp.activerSequence(l);




        }
    }


    public List<string> updateRegs()
    {
        List<string> l = new List<string>();
        l.Add("CS." + Emulateur.cpu_core.GetRegValue("CS"));
        l.Add("DS." + Emulateur.cpu_core.GetRegValue("DS"));
        l.Add("ES." + Emulateur.cpu_core.GetRegValue("ES"));
        l.Add("SS." + Emulateur.cpu_core.GetRegValue("SS"));
        l.Add("IP." + Emulateur.cpu_core.GetRegValue("IP"));
        l.Add("SP."+Emulateur.cpu_core.GetRegValue("SP"));
        return l;
    }

    public List<string> updateIp()
    {
        List<string> l = new List<string>();
        l.Add("IP");
        return l;
    }


    List<string> decodeInstr(string instName)
    {
        List<string> l = new List<string>();
        l.Add("20,1");
        l.Add("34,1");
        l.Add("IR," + instName);
        l.Add("35,1");
        l.Add("/DECINST");
        l.Add("20,0");
        l.Add("19,0");
        l.Add("3,1");
        return l;
    }

    List<string> decodeInstr2(string instName)
    {
        List<string> l = new List<string>();
        l.Add("20,1");
        l.Add("34,1");
        l.Add("IR," + instName);
        l.Add("35,1");
        l.Add("/DECINST");
        return l;
    }


    void UpdateFlags(List<string> l)
    {
        List<string> list = new List<string>();
        list.Add((Emulateur.cpu_core.registers.flagRegister.GetFlag(Flags.Carry) == 0 ? "-" : "") + "CF");
        list.Add((Emulateur.cpu_core.registers.flagRegister.GetFlag(Flags.Overflow) == 0 ? "-" : "") + "OF");
        list.Add((Emulateur.cpu_core.registers.flagRegister.GetFlag(Flags.Directional) == 0 ? "-" : "") + "DF");
        list.Add((Emulateur.cpu_core.registers.flagRegister.GetFlag(Flags.Sign) == 0 ? "-" : "") + "SF");
        list.Add((Emulateur.cpu_core.registers.flagRegister.GetFlag(Flags.Zero) == 0 ? "-" : "") + "ZF");
        list.Add((Emulateur.cpu_core.registers.flagRegister.GetFlag(Flags.Parity) == 0 ? "-" : "") + "PF");
        list.Add((Emulateur.cpu_core.registers.flagRegister.GetFlag(Flags.Auxilliary) == 0 ? "-" : "") + "AF");

        list.Add("ps" + Emulateur.cpu_core.registers.flagRegister.Get());
        l.AddRange(list);

    }

    public void LEAInstruction(Operand op1, Operand op2)
    {
        List<string> afterSelection = new List<string>();
        List<string> l = new List<string>();
        l.AddRange(decodeInstr("LEA"));
        switch (op1.Type)
        {
            case OperandType.variable:

                l.Add("17,0");
                l.Add("18,0");
                l.Add("DS");
                l.Add("26,0");
                l.Add("25,0");
                l.Add("23,1");

                l.AddRange(fetchRam(Emulateur.cpu_core.GetRegValue("DS"), Emulateur.cpu_core.ST.Lookup(op1.Value).Address, op1.size == 16 ? true : false));
                l.Add("23,0");
                l.Add("25,1");
                l.Add("26,1");
                l.Add("18,1");
                l.Add("17,1");
                l.Add("3,1");
                l.Add("0,0");
                l.Add(op2.Value.ToUpper() + Emulateur.cpu_core.memory.Get(Emulateur.cpu_core.GetRegValue("DS") + Emulateur.cpu_core.ST.Lookup(op1.Value).Address)); 
                break;

            case OperandType.MemoryAdresse:
                l.Add("17,0");
                l.Add("18,0");
                l.Add("DS");
                l.Add("26,0");
                l.Add("25,0");
                l.Add("23,1");

                l.AddRange(fetchRam(Emulateur.cpu_core.GetRegValue("DS"), int.Parse(op1.Value), op1.size == 16 ? true : false));
                l.Add("23,0");
                l.Add("25,1");
                l.Add("26,1");
                l.Add("18,1");
                l.Add("17,1");
                l.Add("3,1");
                l.Add("0,0");

                l.Add(op2.Value.ToUpper() + Emulateur.cpu_core.memory.Get(Emulateur.cpu_core.GetRegValue("DS") + int.Parse(op1.Value)));
                break;
            default:
                break;


        }

    }

        
                        //         source        destination
        public void movInstruction(Operand op1, Operand op2)
        {
        List<string> afterSelection = new List<string>();
        List<string> l = new List<string>();
        //cycle decodage
        l.AddRange(decodeInstr("MOV"));

        switch (op1.Type)
        {

            case OperandType.Immediate:
                switch (op2.Type)
                {
                    case OperandType.Register:
                        l.Add("0,0");
                        l.Add(op2.Value.ToString() + "" + CPU.GetTwosComplement(int.Parse(op1.Value), Mathf.Abs(int.Parse(op1.Value)) <= 255 ? true : false));
                        break;

                    case OperandType.variable:
                        afterSelection.Clear();
                        afterSelection.Add("20,0");
                        afterSelection.Add("19,0");
                        afterSelection.Add("3,1");
                        afterSelection.Add("17,0");
                        afterSelection.Add("18,0");
                        afterSelection.Add("27,0");
                        afterSelection.Add("25,0");
                        afterSelection.Add("23,1");
                        l.Add("17,0");
                        l.Add("18,0");
                        l.Add("DS");
                        l.Add("26,0");
                        l.Add("25,0");
                        l.Add("23,1");
                        l.AddRange(SetRam(Emulateur.cpu_core.GetRegValue("DS"), Emulateur.cpu_core.ST.Lookup(op2.Value).Address, Parser.IsByte(int.Parse(op1.Value)) ? false : true, int.Parse(op1.Value), afterSelection));
                        break;
                    case OperandType.MemoryAdresse:
                        afterSelection.Clear();
                        afterSelection.Add("20,0");
                        afterSelection.Add("19,0");
                        afterSelection.Add("3,1");
                        afterSelection.Add("17,0");
                        afterSelection.Add("18,0");
                        afterSelection.Add("27,0");
                        afterSelection.Add("25,0");
                        afterSelection.Add("23,1");
                        l.Add("17,0");
                        l.Add("18,0");
                        l.Add("DS");
                        l.Add("27,0");
                        l.Add("26,0");
                        l.Add("25,0");
                        l.Add("23,1");
                        l.AddRange(SetRam(Emulateur.cpu_core.GetRegValue("DS"), int.Parse(op2.Value), Parser.IsByte(int.Parse(op1.Value)) ? false : true, int.Parse(op1.Value), afterSelection));
                        break;
                    default:
                        break;
                }
                break;
            case OperandType.Register:
                switch (op2.Type)
                {
                    case OperandType.Register:

                        l.Add("0,0");
                        l.Add(op1.Value); //select source register
                        l.Add(op2.Value.ToString() + "" + Emulateur.cpu_core.GetRegValue(op1.Value.ToUpper()));   //set reg to reg
                        break;
                    case OperandType.variable:
                        afterSelection.Clear();
                        afterSelection.Add("20,0");
                        afterSelection.Add("19,0");
                        afterSelection.Add("3,1");
                        afterSelection.Add("0,0");
                        afterSelection.Add(op1.Value.ToUpper()); //select source register
                        afterSelection.Add("0,1");
                        afterSelection.Add("3,1");
                        afterSelection.Add("17,0");
                        afterSelection.Add("18,0");
                        afterSelection.Add("27,0");
                        afterSelection.Add("25,0");
                        afterSelection.Add("23,1");
                        l.Add("17,0");
                        l.Add("18,0");
                        l.Add("DS");
                        l.Add("26,0");
                        l.Add("25,0");
                        l.Add("23,1");

                        l.AddRange(SetRam(Emulateur.cpu_core.GetRegValue("DS"), Emulateur.cpu_core.ST.Lookup(op2.Value).Address, op1.size == 16 ? true : false, Emulateur.cpu_core.GetRegValue(op1.Value.ToUpper()), afterSelection)); break;
                    case OperandType.MemoryAdresse:
                        afterSelection.Clear();
                        afterSelection.Add("20,0");
                        afterSelection.Add("19,0");
                        afterSelection.Add("3,1");
                        afterSelection.Add("0,0");
                        afterSelection.Add(op1.Value.ToUpper()); //select source register
                        afterSelection.Add("0,1");
                        afterSelection.Add("3,1");
                        afterSelection.Add("17,0");
                        afterSelection.Add("18,0");
                        afterSelection.Add("27,0");
                        afterSelection.Add("25,0");
                        afterSelection.Add("23,1");
                        l.Add("17,0");
                        l.Add("18,0");
                        l.Add("DS");
                        l.Add("26,0");
                        l.Add("25,0");
                        l.Add("23,1");

                        l.AddRange(SetRam(Emulateur.cpu_core.GetRegValue("DS"), int.Parse(op2.Value), op1.size == 16 ? true : false, Emulateur.cpu_core.GetRegValue(op1.Value.ToUpper()), afterSelection)); break;
                    default:
                        break;
                }

                break;
            case OperandType.variable:
                switch (op2.Type)
                {
                    case OperandType.Register:
                        l.Add("17,0");
                        l.Add("18,0");
                        l.Add("DS");
                        l.Add("26,0");
                        l.Add("25,0");
                        l.Add("23,1");

                        l.AddRange(fetchRam(Emulateur.cpu_core.GetRegValue("DS"), Emulateur.cpu_core.ST.Lookup(op1.Value).Address, op2.size == 16 ? true : false)); 
                        l.Add("23,0");
                        l.Add("25,1");
                        l.Add("26,1");
                        l.Add("18,1");
                        l.Add("17,1");
                        l.Add("3,1");
                        l.Add("0,0");
                        l.Add(op2.Value.ToUpper() + Emulateur.cpu_core.memory.Get(Emulateur.cpu_core.GetRegValue("DS") + Emulateur.cpu_core.ST.Lookup(op1.Value).Address));
                        break;
                    default:
                        break;
                }


                break;



            case OperandType.MemoryAdresse:
                switch (op2.Type)
                {
                    case OperandType.Register:
                        l.Add("17,0");
                        l.Add("18,0");
                        l.Add("DS");
                        l.Add("26,0");
                        l.Add("25,0");
                        l.Add("23,1");

                        l.AddRange(fetchRam(Emulateur.cpu_core.GetRegValue("DS"), int.Parse(op1.Value), op2.size == 16 ? true : false)); l.Add("23,0");
                        l.Add("25,1");
                        l.Add("26,1");
                        l.Add("18,1");
                        l.Add("17,1");
                        l.Add("3,1");
                        l.Add("0,0");

                        l.Add(op2.Value.ToUpper() + Emulateur.cpu_core.memory.Get(Emulateur.cpu_core.GetRegValue("DS") + int.Parse(op1.Value)));
                        break;
                    default:
                        break;
                }
                break;





            default:
                break;
        }

        an.activerSequence(l);


    }

    //      source    destination
    public void ArithmeticInstruction(Operand op1, Operand op2, string operation)
    {
        List<string> afterSelection = new List<string>();
        List<string> l = new List<string>();
        l.AddRange(decodeInstr(operation));
        switch (op1.Type)
        {

            case OperandType.Immediate:
                switch (op2.Type)
                {
                    case OperandType.Register:
                        


                        l.Add("4,1");
                        l.Add("RT" + op1.Value);
                        l.Add("20,0");
                        l.Add("19,0");
                        l.Add("3,1");
                        l.Add("0,0");
                        l.Add(op2.Value+";"); //pour selectioner le registre sans changer la valuer (recuperer la valuer avant le changement)
                        l.Add("0,1");
                        l.Add("3,1");
                        l.Add("5,1");
                        l.Add("10,1");


                        l.Add("11,1"); 
                        l.Add("ARLU");

                        l.Add("13,1");
                        l.Add("16,1");
                        l.Add("1,0");
                        l.Add("3,0");
                        l.Add("0,0");

                        l.Add(op2.Value);
                        break;

                    case OperandType.variable:

                        l.Add("4,1");
                        l.Add("RT" + op1.Value);
                        l.Add("20,0");
                        l.Add("19,0");
                        l.Add("3,1");
                        l.Add("17,0");
                        l.Add("18,0");
                        l.Add("DS");
                        l.Add("26,0");
                        l.Add("25,0");
                        l.Add("23,1");
        
                        l.AddRange(fetchRam(Emulateur.cpu_core.GetRegValue("DS"), Emulateur.cpu_core.ST.Lookup(op2.Value).Address, false,Emulateur.historiqueRam[Emulateur.cpu_core.ST.Lookup(op2.Value).Address].Peek().value));
                        l.Add("23,0");
                        l.Add("25,1");
                        l.Add("26,1");
                        l.Add("18,1");
                        l.Add("17,1");
                        l.Add("3,1");
                        l.Add("5,1");
                        l.Add("10,1");


                        l.Add("11,1"); 
                        l.Add("ARLU");


                        l.Add("13,1");
                        l.Add("16,1");
                        l.Add("1,0");
                        l.Add("3,0");
                        l.Add("17,0");
                        l.Add("18,0");
                        l.Add("26,0");
                        l.Add("25,0");
                        l.Add("23,1");
                       int val=((int) Emulateur.cpu_core.memory.Get(Emulateur.cpu_core.ST.Lookup(op2.Value).Address+Emulateur.cpu_core.GetRegValue("DS")).NumericalValue);
                        int low=((int) Emulateur.cpu_core.memory.Get(Emulateur.cpu_core.ST.Lookup(op2.Value).Address+Emulateur.cpu_core.GetRegValue("DS")).NumericalValue);
                        int high=((int) Emulateur.cpu_core.memory.Get(Emulateur.cpu_core.ST.Lookup(op2.Value).Address+1+Emulateur.cpu_core.GetRegValue("DS")).NumericalValue)>>8;
                        int valueToSet=op1.size==16?(high+low):val;
                        Debug.Log("ADD RESULT IS :"+valueToSet+" high");
                        l.AddRange(SetRam2(Emulateur.cpu_core.GetRegValue("DS"), Emulateur.cpu_core.ST.Lookup(op2.Value).Address,op1.size==16?true:false,valueToSet, afterSelection));
                        break;
                    case OperandType.MemoryAdresse:
                       
                        l.Add("4,1");
                        l.Add("RT" + op1.Value);

                        l.Add("20,0");
                        l.Add("19,0");
                        l.Add("3,1");
                        l.Add("17,0");
                        l.Add("18,0");
                        l.Add("DS");
                        l.Add("26,0");
                        l.Add("25,0");
                        l.Add("23,1");

                        l.AddRange(fetchRam(Emulateur.cpu_core.GetRegValue("DS"), int.Parse(op2.Value), false,Emulateur.historiqueRam[int.Parse(op2.Value)].Peek().value));
                        l.Add("23,0");
                        l.Add("25,1");
                        l.Add("26,1");
                        l.Add("18,1");
                        l.Add("17,1");
                        l.Add("3,1");
                        l.Add("5,1");
                        l.Add("10,1");

                        l.Add("11,1");
                        l.Add("ARLU");

                        l.Add("13,1");
                        l.Add("16,1");
                        l.Add("1,0");
                        l.Add("3,0");
                        l.Add("17,0");
                        l.Add("18,0");
                        l.Add("26,0");
                        l.Add("25,0");
                        l.Add("23,1");

                        int val2=((int) Emulateur.cpu_core.memory.Get(int.Parse(op2.Value)+Emulateur.cpu_core.GetRegValue("DS")).NumericalValue);
                        int low2=((int) Emulateur.cpu_core.memory.Get(int.Parse(op2.Value)+Emulateur.cpu_core.GetRegValue("DS")).NumericalValue);
                        int high2=((int) Emulateur.cpu_core.memory.Get(int.Parse(op2.Value)+1+Emulateur.cpu_core.GetRegValue("DS")).NumericalValue)>>8;
                        int valueToSet2=op1.size==16?(high2+low2):val2;
                        Debug.Log("ADD RESULT IS :"+valueToSet2+" high");
                        l.AddRange(SetRam2(Emulateur.cpu_core.GetRegValue("DS"), int.Parse(op2.Value),op1.size==16?true:false,valueToSet2, afterSelection));
                        break;
                    default:
                        break;
                }
                break;
            case OperandType.Register:
                switch (op2.Type)
                {
                    case OperandType.Register:


                        l.Add("0,0");
                        l.Add(op1.Value + ";");

                        l.Add("0,1");
                        l.Add("3,1");
                        l.Add("4,1");
                        l.Add("RT" + Emulateur.cpu_core.GetRegValue(op1.Value));
                        
                        l.Add("20,0");
                        l.Add("19,0");
                        l.Add("3,1");
                        l.Add("0,0");
                        l.Add(op2.Value + ";");
                        l.Add("0,1");
                        l.Add("3,1");
                        l.Add("5,1");
                        l.Add("10,1");
                        l.Add("11,1");
                        l.Add("ARLU");

                        l.Add("13,1");
                        l.Add("16,1");
                        l.Add("1,0");
                        l.Add("3,0");
                        l.Add("0,0");

                        l.Add(op2.Value);
                        break;
                    case OperandType.variable:

                        l.Add("0,0");
                        l.Add(op1.Value + ";");
                        l.Add("0,1");
                        l.Add("3,1");
                        l.Add("4,1");
                        l.Add("RT" + Emulateur.cpu_core.GetRegValue(op1.Value));

                        l.Add("20,0");
                        l.Add("19,0");
                        l.Add("3,1");
                        l.Add("17,0");
                        l.Add("18,0");
                        l.Add("DS");
                        l.Add("26,0");
                        l.Add("25,0");
                        l.Add("23,1");

                        l.AddRange(fetchRam(Emulateur.cpu_core.GetRegValue("DS"), Emulateur.cpu_core.ST.Lookup(op2.Value).Address, false,Emulateur.historiqueRam[Emulateur.cpu_core.ST.Lookup(op2.Value).Address].Peek().value));
                        l.Add("23,0");
                        l.Add("25,1");
                        l.Add("26,1");
                        l.Add("18,1");
                        l.Add("17,1");
                        l.Add("3,1");
                        l.Add("5,1");
                        l.Add("10,1");
                        l.Add("11,1");
                        l.Add("ARLU");

                        l.Add("13,1");
                        l.Add("16,1");
                        l.Add("1,0");
                        l.Add("3,0");
                        l.Add("0,0");

                         int val=((int) Emulateur.cpu_core.memory.Get(Emulateur.cpu_core.ST.Lookup(op2.Value).Address+Emulateur.cpu_core.GetRegValue("DS")).NumericalValue);
                        int low=((int) Emulateur.cpu_core.memory.Get(Emulateur.cpu_core.ST.Lookup(op2.Value).Address+Emulateur.cpu_core.GetRegValue("DS")).NumericalValue);
                        int high=((int) Emulateur.cpu_core.memory.Get(Emulateur.cpu_core.ST.Lookup(op2.Value).Address+1+Emulateur.cpu_core.GetRegValue("DS")).NumericalValue)>>8;
                        int valueToSet=op1.size==16?(high+low):val;
                        Debug.Log("ADD RESULT IS :"+valueToSet+" high");
                        l.AddRange(SetRam2(Emulateur.cpu_core.GetRegValue("DS"), Emulateur.cpu_core.ST.Lookup(op2.Value).Address,op1.size==16?true:false,valueToSet, afterSelection));
                        break;
                    case OperandType.MemoryAdresse:
                        l.Add("0,0");
                        l.Add(op1.Value + ";");
                        l.Add("0,1");
                        l.Add("3,1");
                        l.Add("4,1");
                        l.Add("RT" + Emulateur.cpu_core.GetRegValue(op1.Value));

                        l.Add("20,0");
                        l.Add("19,0");
                        l.Add("3,1");
                        l.Add("17,0");
                        l.Add("18,0");
                        l.Add("DS");
                        l.Add("26,0");
                        l.Add("25,0");
                        l.Add("23,1");
                  //       int topIndex = Emulateur.historiqueRam[int.Parse(op2.Value)].ToArray().Length - 1;
                     //    int elementToSelect=(op1.size==16?Emulateur.historiqueRam[int.Parse(op2.Value)].ToArray()[topIndex].value:Emulateur.historiqueRam[int.Parse(op2.Value)].Peek().value);
                        l.AddRange(fetchRam(Emulateur.cpu_core.GetRegValue("DS"), int.Parse(op2.Value), false,Emulateur.historiqueRam[int.Parse(op2.Value)].Peek().value));
                        l.Add("23,0");
                        l.Add("25,1");
                        l.Add("26,1");
                        l.Add("18,1");
                        l.Add("17,1");
                        l.Add("3,1");
                        l.Add("5,1");
                        l.Add("10,1");

                        l.Add("11,1");
                        l.Add("ARLU");

                        l.Add("13,1");
                        l.Add("16,1");
                        l.Add("1,0");
                        l.Add("3,0");
                        l.Add("17,0");
                        l.Add("18,0");
                        l.Add("26,0");
                        l.Add("25,0");
                        l.Add("23,1");
                        int val2=((int) Emulateur.cpu_core.memory.Get(int.Parse(op2.Value)+Emulateur.cpu_core.GetRegValue("DS")).NumericalValue);
                        int low2=((int) Emulateur.cpu_core.memory.Get(int.Parse(op2.Value)+Emulateur.cpu_core.GetRegValue("DS")).NumericalValue);
                        int high2=((int) Emulateur.cpu_core.memory.Get(int.Parse(op2.Value)+1+Emulateur.cpu_core.GetRegValue("DS")).NumericalValue)>>8;
                        int valueToSet2=op1.size==16?(high2+low2):val2;
                        Debug.Log("ADD RESULT IS :"+valueToSet2+" high");
                        l.AddRange(SetRam2(Emulateur.cpu_core.GetRegValue("DS"), int.Parse(op2.Value),op1.size==16?true:false,valueToSet2, afterSelection));
                        break;
                    default:
                        break;
                }

                break;
            case OperandType.variable:
                switch (op2.Type)
                {
                    case OperandType.Register:
                        l.Add("17,0");
                        l.Add("18,0");
                        l.Add("DS");
                        l.Add("26,0");
                        l.Add("25,0");
                        l.Add("23,1");
                        l.AddRange(fetchRam(Emulateur.cpu_core.GetRegValue("DS"), Emulateur.cpu_core.ST.Lookup(op1.Value).Address, op1.size == 16 ? true : false));
                        l.Add("23,0");
                        l.Add("25,1");
                        l.Add("26,1");
                        l.Add("18,1");
                        l.Add("17,1");
                        l.Add("3,1");
                        l.Add("4,1");
                        l.Add("RT" + (int)Emulateur.cpu_core.memory.Get(Emulateur.cpu_core.GetRegValue("ds") + Emulateur.cpu_core.ST.Lookup(op1.Value).Address).NumericalValue);

                        l.Add("20,0");
                        l.Add("19,0");
                        l.Add("3,1");
                        l.Add("0,0");
                        l.Add(op2.Value + ";");
                        l.Add("0,1");
                        l.Add("3,1");
                        l.Add("5,1");
                        l.Add("10,1");


                        l.Add("11,1");
                        l.Add("ARLU");

                        l.Add(op2.Value);
                        //l.Add(op2.Value.ToUpper() + Emulateur.cpu_core.memory.Get(Emulateur.cpu_core.GetRegValue("DS") + Emulateur.cpu_core.ST.Lookup(op1.Value).Address));
                        break;
                    default:
                        break;
                }


                break;



            case OperandType.MemoryAdresse:
                switch (op2.Type)
                {
                    case OperandType.Register:
                        l.Add("17,0");
                        l.Add("18,0");
                        l.Add("DS");
                        l.Add("26,0");
                        l.Add("25,0");
                        l.Add("23,1");
                        l.AddRange(fetchRam(Emulateur.cpu_core.GetRegValue("DS"), int.Parse(op1.Value), op1.size == 16 ? true : false));
                        l.Add("23,0");
                        l.Add("25,1");
                        l.Add("26,1");
                        l.Add("18,1");
                        l.Add("17,1");
                        l.Add("3,1");
                        l.Add("4,1");
                        //l.Add("RT" + Emulateur.cpu_core.memory.Get(Emulateur.cpu_core.GetRegValue("ds") + op1.Value).NumericalValue);
                        //l.Add("RT" + (int)Emulateur.cpu_core.memory.Get(Emulateur.cpu_core.GetRegValue("ds") + op1.Value).NumericalValue.ToString());

                        l.Add("20,0");
                        l.Add("19,0");
                        l.Add("3,1");
                        l.Add("0,0");
                        l.Add(op2.Value + ";");
                        l.Add("0,1");
                        l.Add("3,1");
                        l.Add("5,1");
                        l.Add("10,1");


                        l.Add("11,1");
                        l.Add("ARLU");
                        l.Add("13,1");
                        l.Add("16,1");
                        l.Add("1,0");
                        l.Add("3,0");

                        l.Add(op2.Value);
                        //l.Add(op2.Value.ToUpper() + Emulateur.cpu_core.memory.Get(Emulateur.cpu_core.GetRegValue("DS") + int.Parse(op1.Value)));
                        break;
                    default:
                        break;
                }
                break;





            default:
                break;
        }

        UpdateFlags(l);
        an.activerSequence(l);



    }

    public void CMPInstruction(Operand op1, Operand op2)
    {
        List<string> afterSelection = new List<string>();
        List<string> l = new List<string>();
        l.AddRange(decodeInstr("CMP"));
        switch (op1.Type)
        {

            case OperandType.Immediate:
                switch (op2.Type)
                {
                    case OperandType.Register:

                        l.Add("4,1");
                        l.Add(op2.Value + ";");

                        l.Add("20,0");
                        l.Add("19,0");
                        l.Add("3,1");
                        l.Add("0,0");
                        l.Add(op2.Value);
                        l.Add("0,1");
                        l.Add("3,1");
                        l.Add("5,1");
                        l.Add("10,1");


                        l.Add("11,1");
                        l.Add("ARLU");

                        break;

                    case OperandType.variable:
      
                        l.Add("4,1");
                        l.Add(op2.Value + ";");

                        l.Add("20,0");
                        l.Add("19,0");
                        l.Add("3,1");
                        l.Add("17,0");
                        l.Add("18,0");
                        l.Add("DS");
                        l.Add("26,0");
                        l.Add("25,0");
                        l.Add("23,1");

                        l.AddRange(fetchRam(Emulateur.cpu_core.GetRegValue("DS"), Emulateur.cpu_core.ST.Lookup(op2.Value).Address, op1.size == 16 ? true : false));
                        l.Add("23,0");
                        l.Add("25,1");
                        l.Add("26,1");
                        l.Add("18,1");
                        l.Add("17,1");
                        l.Add("3,1");
                        l.Add("5,1");
                        l.Add("10,1");


                        l.Add("11,1");
                        l.Add("ARLU");

                        break;
                    case OperandType.MemoryAdresse:
                        l.Add("4,1");
                        l.Add(op2.Value + ";");

                        l.Add("20,0");
                        l.Add("19,0");
                        l.Add("3,1");
                        l.Add("17,0");
                        l.Add("18,0");
                        l.Add("DS");
                        l.Add("26,0");
                        l.Add("25,0");
                        l.Add("23,1");

                        l.AddRange(fetchRam(Emulateur.cpu_core.GetRegValue("DS"), int.Parse(op2.Value), op2.size == 16 ? true : false));
                        l.Add("23,0");
                        l.Add("25,1");
                        l.Add("26,1");
                        l.Add("18,1");
                        l.Add("17,1");
                        l.Add("3,1");
                        l.Add("5,1");
                        l.Add("10,1");


                        l.Add("11,1");
                        l.Add("ARLU");

                        break;
                    default:
                        break;
                }
                break;
            case OperandType.Register:
                switch (op2.Type)
                {
                    case OperandType.Register:


                        l.Add("0,0");
                        l.Add(op1.Value);
                        l.Add("0,1");
                        l.Add("3,1");
                        l.Add("4,1");
                        l.Add("RT" + Emulateur.cpu_core.GetRegValue(op1.Value));


                        l.Add("20,0");
                        l.Add("19,0");
                        l.Add("3,1");
                        l.Add("0,0");
                        l.Add(op2.Value);
                        l.Add("0,1");
                        l.Add("3,1");
                        l.Add("5,1");
                        l.Add("10,1");

                        l.Add("11,1");
                        l.Add("ARLU");


                        break;
                    case OperandType.variable:

                        l.Add("0,0");
                        l.Add(op1.Value);
                        l.Add("0,1");
                        l.Add("3,1");
                        l.Add("4,1");
                        l.Add("RT5");

                        l.Add("20,0");
                        l.Add("19,0");
                        l.Add("3,1");
                        l.Add("17,0");
                        l.Add("18,0");
                        l.Add("DS");
                        l.Add("26,0");
                        l.Add("25,0");
                        l.Add("23,1");

                        l.AddRange(fetchRam(Emulateur.cpu_core.GetRegValue("DS"), Emulateur.cpu_core.ST.Lookup(op2.Value).Address, op1.size == 16 ? true : false));
                        l.Add("23,0");
                        l.Add("25,1");
                        l.Add("26,1");
                        l.Add("18,1");
                        l.Add("17,1");
                        l.Add("3,1");
                        l.Add("5,1");
                        l.Add("10,1");

                        l.Add("11,1");
                        l.Add("ARLU");

                        break;
                    case OperandType.MemoryAdresse:
                        l.Add("0,0");
                        l.Add(op1.Value);
                        l.Add("0,1");
                        l.Add("3,1");
                        l.Add("4,1");
                        l.Add("RT5");

                        l.Add("20,0");
                        l.Add("19,0");
                        l.Add("3,1");
                        l.Add("17,0");
                        l.Add("18,0");
                        l.Add("DS");
                        l.Add("26,0");
                        l.Add("25,0");
                        l.Add("23,1");

                        l.AddRange(fetchRam(Emulateur.cpu_core.GetRegValue("DS"), int.Parse(op2.Value), op2.size == 16 ? true : false));
                        l.Add("23,0");
                        l.Add("25,1");
                        l.Add("26,1");
                        l.Add("18,1");
                        l.Add("17,1");
                        l.Add("3,1");
                        l.Add("5,1");
                        l.Add("10,1");

                        l.Add("11,1");
                        l.Add("ARLU");

                        break;
                    default:
                        break;
                }

                break;
            case OperandType.variable:
                switch (op2.Type)
                {
                    case OperandType.Register:
                        l.Add("17,0");
                        l.Add("18,0");
                        l.Add("DS");
                        l.Add("26,0");
                        l.Add("25,0");
                        l.Add("23,1");
                        l.AddRange(fetchRam(Emulateur.cpu_core.GetRegValue("DS"), Emulateur.cpu_core.ST.Lookup(op1.Value).Address, op1.size == 16 ? true : false));
                        l.Add("23,0");
                        l.Add("25,1");
                        l.Add("26,1");
                        l.Add("18,1");
                        l.Add("17,1");
                        l.Add("3,1");
                        l.Add("4,1");
                        l.Add("RT" + (int)Emulateur.cpu_core.memory.Get(Emulateur.cpu_core.GetRegValue("ds") + Emulateur.cpu_core.ST.Lookup(op1.Value).Address).NumericalValue);

                        l.Add("20,0");
                        l.Add("19,0");
                        l.Add("3,1");
                        l.Add("0,0");
                        l.Add(op2.Value);
                        l.Add("0,1");
                        l.Add("3,1");
                        l.Add("5,1");
                        l.Add("10,1");


                        l.Add("11,1");
                        l.Add("ARLU");

                        l.Add(op2.Value.ToUpper() + Emulateur.cpu_core.memory.Get(Emulateur.cpu_core.GetRegValue("DS") + Emulateur.cpu_core.ST.Lookup(op1.Value).Address));
                        break;
                    default:
                        break;
                }


                break;



            case OperandType.MemoryAdresse:
                switch (op2.Type)
                {
                    case OperandType.Register:
                        l.Add("17,0");
                        l.Add("18,0");
                        l.Add("DS");
                        l.Add("26,0");
                        l.Add("25,0");
                        l.Add("23,1");
                        l.AddRange(fetchRam(Emulateur.cpu_core.GetRegValue("DS"), int.Parse(op1.Value), op1.size == 16 ? true : false));
                        l.Add("23,0");
                        l.Add("25,1");
                        l.Add("26,1");
                        l.Add("18,1");
                        l.Add("17,1");
                        l.Add("3,1");
                        l.Add("4,1");
                        //l.Add("RT" + (int)Emulateur.cpu_core.memory.Get(Emulateur.cpu_core.GetRegValue("ds") + op1.Value).NumericalValue.ToString());

                        l.Add("20,0");
                        l.Add("19,0");
                        l.Add("3,1");
                        l.Add("0,0");
                        l.Add(op2.Value);
                        l.Add("0,1");
                        l.Add("3,1");
                        l.Add("5,1");
                        l.Add("10,1");


                        l.Add("11,1");
                        l.Add("ARLU");

                        break;
                    default:
                        break;
                }
                break;





            default:
                break;
        }
        UpdateFlags(l);
        an.activerSequence(l);



    }

    public void DivMulInstruction(Operand op, string operation)
    {
        List<string> afterSelection = new List<string>();
        List<string> l = new List<string>();
        l.AddRange(decodeInstr(operation));
        switch (op.Type)
        {
            case OperandType.MemoryAdresse:
                l.Add("17,0");
                l.Add("18,0");
                l.Add("DS");
                l.Add("26,0");
                l.Add("25,0");
                l.Add("23,1");
                l.AddRange(fetchRam(Emulateur.cpu_core.GetRegValue("DS"), int.Parse(op.Value), false)); 
                l.Add("23,0");
                l.Add("25,1");
                l.Add("26,1");
                l.Add("18,1");
                l.Add("17,1");
                l.Add("3,1");
                l.Add("4,1");
                //l.Add("RT" + (int)Emulateur.cpu_core.memory.Get(Emulateur.cpu_core.GetRegValue("ds") + op1.Value).NumericalValue.ToString());


                l.Add("20,0");
                l.Add("19,0");
                l.Add("3,1");
                l.Add("0,0");
                l.Add("AL ;");
                l.Add("0,1");
                l.Add("3,1");
                l.Add("5,1");
                l.Add("10,1");


                l.Add("11,1");
                l.Add("ARLU");

                l.Add("13,1");
                l.Add("16,1");
                l.Add("1,0");
                l.Add("3,0");
                l.Add("0,0");
                l.Add("DX");


                l.Add("13,1");
                l.Add("16,1");
                l.Add("1,0");
                l.Add("3,0");
                l.Add("0,0");
                l.Add("AX");


                break;




            case OperandType.variable:

                l.Add("17,0");
                l.Add("18,0");
                l.Add("DS");
                l.Add("26,0");
                l.Add("25,0");
                l.Add("23,1");
                l.AddRange(fetchRam(Emulateur.cpu_core.GetRegValue("DS"), Emulateur.cpu_core.ST.Lookup(op.Value).Address, op.size == 16 ? true : false)); l.Add("23,0");
                l.Add("25,1");
                l.Add("26,1");
                l.Add("18,1");
                l.Add("17,1");
                l.Add("3,1");
                l.Add("4,1");
                l.Add("RT" + (int)Emulateur.cpu_core.memory.Get(Emulateur.cpu_core.GetRegValue("ds") + Emulateur.cpu_core.ST.Lookup(op.Value).Address).NumericalValue);

                l.Add("20,0");
                l.Add("19,0");
                l.Add("3,1");
                l.Add("0,0");
                l.Add("AL ;");
                l.Add("0,1");
                l.Add("3,1");
                l.Add("5,1");
                l.Add("10,1");


                l.Add("11,1");
                l.Add("ARLU");

                l.Add("13,1");
                l.Add("16,1");
                l.Add("1,0");
                l.Add("3,0");
                l.Add("0,0");
                l.Add("DX");

                l.Add("13,1");
                l.Add("16,1");
                l.Add("1,0");
                l.Add("3,0");
                l.Add("0,0");
                l.Add("AX");


                break;

            case OperandType.Register:
                
                l.Add("0,0");
                l.Add(op.Value+";");
                l.Add("0,1");
                l.Add("3,1");
                l.Add("4,1");
                l.Add("RT" + Emulateur.cpu_core.GetRegValue(op.Value));

                l.Add("20,0");
                l.Add("19,0");
                l.Add("3,1");
                l.Add("0,0");
                l.Add("AL ;");
                l.Add("0,1");
                l.Add("3,1");
                l.Add("5,1");
                l.Add("10,1");


                l.Add("11,1");
                l.Add("ARLU");

                l.Add("13,1");
                l.Add("16,1");
                l.Add("1,0");
                l.Add("3,0");
                l.Add("0,0");
                l.Add("DX");

                l.Add("13,1");
                l.Add("16,1");
                l.Add("1,0");
                l.Add("3,0");
                l.Add("0,0");
                l.Add("AX");

                break;

            default:
                break;

        }
        UpdateFlags(l);

        an.activerSequence(l);

    }

    public void DecIncInstruction(Operand op, string operation)
    {

        List<string> l = new List<string>();
        l.AddRange(decodeInstr(operation));


        switch (op.Type)
        {

            case OperandType.MemoryAdresse:

                l.Add("17,0");
                l.Add("18,0");
                l.Add("DS");
                l.Add("26,0");
                l.Add("25,0");
                l.Add("23,1");
                l.AddRange(fetchRam(Emulateur.cpu_core.GetRegValue("DS"), int.Parse(op.Value), false));
                l.Add("23,0");
                l.Add("25,1");
                l.Add("26,1");
                l.Add("18,1");
                l.Add("17,1");
                l.Add("3,1");
                l.Add("5,1");
                l.Add("10,1");

                l.Add("ARLU");


                l.Add("13,1");
                l.Add("16,1");
                l.Add("1,0");
                l.Add("3,0");
                l.Add("17,0");
                l.Add("18,0");
                l.Add("DS");
                l.Add("26,0");
                l.Add("25,0");
                l.Add("23,1");

                l.AddRange(SetRam2(Emulateur.cpu_core.GetRegValue("DS"), int.Parse(op.Value), false, (int)Emulateur.cpu_core.memory.Get(Emulateur.cpu_core.GetRegValue("ds") + int.Parse(op.Value)).NumericalValue, null)); 
                break;




            case OperandType.variable:
                l.Add("17,0");
                l.Add("18,0");
                l.Add("DS");
                l.Add("26,0");
                l.Add("25,0");
                l.Add("23,1");
                l.AddRange(fetchRam(Emulateur.cpu_core.GetRegValue("DS"), Emulateur.cpu_core.ST.Lookup(op.Value).Address, op.size == 16 ? true : false));
                l.Add("23,0");
                l.Add("25,1");
                l.Add("26,1");
                l.Add("18,1");
                l.Add("17,1");
                l.Add("3,1");
                l.Add("5,1");
                l.Add("10,1");

                l.Add("ARLU");



                l.Add("13,1");
                l.Add("16,1");
                l.Add("1,0");
                l.Add("3,0");
                l.Add("17,0");
                l.Add("18,0");
                l.Add("DS");
                l.Add("26,0");
                l.Add("25,0");
                l.Add("23,1");
                l.AddRange(SetRam2(Emulateur.cpu_core.GetRegValue("DS"), Emulateur.cpu_core.ST.Lookup(op.Value).Address, op.size == 16 ? true : false, (int)Emulateur.cpu_core.memory.Get(Emulateur.cpu_core.GetRegValue("ds") + Emulateur.cpu_core.ST.Lookup(op.Value).Address).NumericalValue, null));
                break;

            case OperandType.Register:
                l.Add("0,0");
                l.Add(op.Value+";");

                l.Add("0,1");
                l.Add("3,1");
                l.Add("5,1");
                l.Add("10,1");

                l.Add("ARLU");

                l.Add("13,1");
                l.Add("16,1");
                l.Add("1,0");
                l.Add("3,0");
                l.Add("0,0");


                l.Add(op.Value);
                //l.Add(op.Value.ToString() + "" + Emulateur.cpu_core.GetRegValue(op.Value));
                break;

            default:
                break;
        }

        UpdateFlags(l);

        an.activerSequence(l);


    }

    public void PushInstruction(Operand op)
    {
        List<string> afterSelection = new List<string>();
        List<string> l = new List<string>();
        l.AddRange(decodeInstr("PUSH"));
        afterSelection.Clear();
        afterSelection.Add("20,0");
        afterSelection.Add("19,0");
        afterSelection.Add("3,1");
        afterSelection.Add("0,0");
        afterSelection.Add(op.Value);
        afterSelection.Add("0,1");
        afterSelection.Add("3,0");
        afterSelection.Add("17,0");
        afterSelection.Add("18,0");
        afterSelection.Add("27,0");
        afterSelection.Add("25,0");
        afterSelection.Add("23,1");

        //calculer l'adresse
        l.Add("0,0");
        l.Add("SP"+Emulateur.cpu_core.GetRegValue("sp"));
        l.Add("0,1");
        l.Add("3,0");
        l.Add("17,0");
        l.Add("18,0");
        l.Add("27,0");
        l.Add("SS");
        l.Add("26,0");
        l.Add("SOM");
        l.Add("25,0");
        l.Add("23,1");


        l.AddRange(SetRam(Emulateur.cpu_core.GetRegValue("SS"), Emulateur.cpu_core.GetRegValue("SP"), true, Emulateur.cpu_core.GetRegValue(op.Value.ToUpper()), afterSelection));


        //l.Add("SP"-2);


        UpdateFlags(l);
        an.activerSequence(l);
    }

    public void PopInstruction(Operand op)
    {
        List<string> afterSelection = new List<string>();
        List<string> l = new List<string>();
        l.AddRange(decodeInstr("POP"));

        //calculer l'adresse
        l.Add("0,0");
        l.Add("SP"+(Emulateur.cpu_core.GetRegValue("sp")-2));
        l.Add("0,1");
        l.Add("3,0");
        l.Add("17,0");
        l.Add("18,0");
        l.Add("27,0");
        l.Add("SS");
        l.Add("26,0");
        l.Add("SOM");
        l.Add("25,0");
        l.Add("23,1");

        l.AddRange(fetchRam(Emulateur.cpu_core.GetRegValue("SS"), Emulateur.cpu_core.GetRegValue("SP")-2    , true));
        l.Add("23,0");
        l.Add("25,1");
        l.Add("26,1");
        l.Add("18,1");
        l.Add("17,1");
        l.Add("3,1");
        l.Add("0,0");
        l.Add(op.Value);
         l.Add("SP"+(Emulateur.cpu_core.GetRegValue("sp")));


      //  l.Add("SP"+(Emulateur.cpu_core.GetRegValue("sp")-2).ToString());
        UpdateFlags(l);
        an.activerSequence(l);
    }
    public void Call(int ip)
    {
        List<string> l = new List<string>();
        List<string> afterSelection = new List<string>();
        decodeInstr("CALL");
        afterSelection.Clear();
        afterSelection.Add("20,0");
        afterSelection.Add("19,0");
        afterSelection.Add("3,1");
        afterSelection.Add("17,0");
        afterSelection.Add("18,0");
        afterSelection.Add("IP;");
        afterSelection.Add("27,0");
        afterSelection.Add("25,0");
        afterSelection.Add("23,1");

        //calculer l'adresse
        l.Add("0,0");
        l.Add("SP");
        l.Add("0,1");
        l.Add("3,0");
        l.Add("17,0");
        l.Add("18,0");
        l.Add("27,0");
        l.Add("SS");
        l.Add("26,0");
        l.Add("SOM");
        l.Add("25,0");
        l.Add("23,1");


        l.AddRange(SetRam(Emulateur.cpu_core.GetRegValue("SS"), Emulateur.cpu_core.GetRegValue("SP"), true, Emulateur.cpu_core.GetRegValue("IP"), afterSelection));


        //l.Add("SP"-2);
        l.Add("IP");
        an.activerSequence(l);


    }

    public void Ret(){
        List<string> l = new List<string>();
        List<string> afterSelection = new List<string>();
        //calculer l'adresse
        decodeInstr("ret");
        l.Add("0,0");
        l.Add("SP");
        l.Add("0,1");
        l.Add("3,0");
        l.Add("17,0");
        l.Add("18,0");
        l.Add("27,0");
        l.Add("SS");
        l.Add("26,0");
        l.Add("SOM");
        l.Add("25,0");
        l.Add("23,1");

        l.AddRange(fetchRam(Emulateur.cpu_core.GetRegValue("SS"), Emulateur.cpu_core.GetRegValue("SP")-2    , true));
        
        l.Add("23,0");
        l.Add("25,1");
        l.Add("26,1");
        
        l.Add("IP");

        an.activerSequence(l);

    }


    public void Ret(int ip)
    {
        List<string> l = new List<string>();
        l.Add("IP" + ip);
        an.activerSequence(l);


    }


    void executeNextInstruction()
    {


        List<string> l = new List<string>();
        Instruction current_instruction = Emulateur.cpu_core.memory.Get(Emulateur.cpu_core.GetRegValue("IP")).Instruction;
        int k = an.statusAwaitingInstructions();
        for (int i = 1; i <= 6; i++)
        {
            l.Add("-BYTE" + i + "");

        }
        Debug.Log("THE K IS " + k + "the S is " + an.statusFilInstruction());
        if (k == 0)
        {
            FetchNext = false;
            fetchInstruction(Emulateur.cpu_core.memory, Emulateur.cpu_core.GetRegValue("CS"), Emulateur.cpu_core.GetRegValue("IP"), false);
            anp.activerSequence(l);
            return;
        }

        Debug.Log("QQQQQQQQQQQQQQ");
        int n = 0;
        for (int i = 0; i < current_instruction.Length; i++)
        {
            l.Add("BYTE" + (6 - i) + "");
            Debug.Log("BYTE" + (6 - i) + "");
            n++;
        }
        historiqueFIvert.Push(n);
        l.Add("DECINST");

        Debug.Log("Added dec");
        anp.activerSequence(l);


    }





    void Start()
    {
        // Debug.Log("aze");
        // fetchInstruction();
    }



    public List<string> fetchRam(int segment, int adr, bool Isword,int? customValue=null)
    {

        List<string> l = new List<string>();
        string bheora0;
        string ram;

        l.Add("ITR");
        l.Add("29,0");
        l.Add("RDY");
        l.Add("DEC");
        l.Add("-ITR");
        l.Add("33,1");
        l.Add("-DEC");

        if (an.getCurrentRamAdresses().Contains((segment << 4) + adr))
        {

            if (adr % 2 == 0)
            {
                bheora0 = "BBHE";
            }
            else
            {

                bheora0 = "A0";
            }
            //          Debug.Log("found : "+((segment<<4)+ adr));
            l.Add("ram:" + an.getCurrentRamAdresses().IndexOf((segment << 4) + adr).ToString() + ":" + segment + ":" + adr);// to test if there was any sudden changes to ram display, ca regle un bug
            ram = "ram" + an.getCurrentRamAdresses().IndexOf((segment << 4) + adr).ToString();
          if(customValue!=null){
            ram=ram+","+customValue;
          }
        }
        else
        {
            //      Debug.Log("not found");

            l.Add("GetRam," + segment + "," + adr);
            if (adr % 2 == 0)
            {
                ram = "ram0";
                bheora0 = "BBHE";
            }
            else
            {
                ram = "ram1";
                if(customValue!=null){
                    ram=ram+","+customValue;
                }
                bheora0 = "A0";
            }

        }
        l.Add(bheora0);
        l.Add(ram);
        if (Isword && adr % 2 == 0 && an.getCurrentRamAdresses().Contains((segment << 4) + adr + 1))
        {   //fetch word from adresse pair
            l.Add("ram" + an.getCurrentRamAdresses().IndexOf((segment << 4) + adr + 1).ToString());
        }
        else if (Isword && adr % 2 == 0)
        {
            l.Add("ram1");
        }
        l.Add("32,1");
        l.Add("31,1");
        l.Add("28,0");
        l.Add("30,1");
        l.Add("ITR");
        l.Add("32,1");
        l.Add("31,1");
        l.Add("28,0");
        l.Add("30,1");
        l.Add("-ALL");
        if (Isword && adr % 2 != 0)
        {   //fetch word from impair 
            fetchRam(segment, adr + 1, false, l);
        }
        return l;

    }
    public List<string> fetchRam(int segment, int adr, bool Isword, List<string> ls)
    {

        List<string> l = ls;
        string bheora0;
        string ram;

        l.Add("ITR");
        l.Add("29,0");
        l.Add("RDY");
        l.Add("DEC");
        l.Add("-ITR");
        l.Add("33,1");
        l.Add("-DEC");

        if (an.getCurrentRamAdresses().Contains((segment << 4) + adr))
        {

            if (adr % 2 == 0)
            {
                bheora0 = "BBHE";
            }
            else
            {

                bheora0 = "A0";
            }
            Debug.Log("found : " + ((segment << 4) + adr));
            l.Add("ram:" + an.getCurrentRamAdresses().IndexOf((segment << 4) + adr).ToString() + ":" + segment + ":" + adr);// to test if there was any sudden changes to ram display
            ram = "ram" + an.getCurrentRamAdresses().IndexOf((segment << 4) + adr).ToString();

        }
        else
        {
            Debug.Log("not found");
            // an.GetRam(Emulateur.cpu_core.memory,segment,adr);  //bug normalment j'enfile get ram
            l.Add("GetRam," + segment + "," + adr);

            if (adr % 2 == 0)
            {
                ram = "ram0";
                bheora0 = "BBHE";
            }
            else
            {
                ram = "ram1";
                bheora0 = "A0";
            }

        }
        l.Add(bheora0);
        l.Add(ram);
        if (Isword && adr % 2 == 0 && an.getCurrentRamAdresses().Contains((segment << 4) + adr + 1))
        {   //fetch word from adresse pair
            l.Add("ram" + an.getCurrentRamAdresses().IndexOf((segment << 4) + adr + 1).ToString());
        }
        else if (Isword && adr % 2 == 0)
        {
            l.Add("ram1");
        }
        l.Add("32,1");
        l.Add("31,1");
        l.Add("28,0");
        l.Add("30,1");
        l.Add("ITR");
        l.Add("32,1");
        l.Add("31,1");
        l.Add("28,0");
        l.Add("30,1");
        l.Add("-RDY");
        l.Add("-ALL");

        if (Isword && adr % 2 != 0)
        {   //fetch word from impair 
            fetchRam(segment, adr + 1, false);
        }



        return l;

    }




    List<string> SetRam(int segment, int adr, bool Isword, int value, List<string> afterSelection)
    {  // a ecrire pour animation d'ecrir sur la ram
        List<string> l = new List<string>();
        string bheora0;
        string ram;
        l.Add("ITR");
        l.Add("29,0");
        l.Add("DEC");
        l.Add("-ITR");
        l.Add("33,1");
        l.Add("-DEC");

        if (an.getCurrentRamAdresses().Contains((segment << 4) + adr))
        {

            if (adr % 2 == 0)
            {
                bheora0 = "BBHE";
            }
            else
            {

                bheora0 = "A0";
            }
            //          Debug.Log("found : "+((segment<<4)+ adr));
            l.Add("ram:" + an.getCurrentRamAdresses().IndexOf((segment << 4) + adr).ToString() + ":" + segment + ":" + adr);// to test if there was any sudden changes to ram display, ca regle un bug
            ram = "ram" + an.getCurrentRamAdresses().IndexOf((segment << 4) + adr).ToString();

        }
        else
        {
            //      Debug.Log("not found");
            l.Add("GetRam," + segment + "," + adr);

            //an.GetRam(Emulateur.cpu_core.memory,segment,adr);
            if (adr % 2 == 0)
            {
                ram = "ram0";
                bheora0 = "BBHE";
            }
            else
            {
                ram = "ram1";
                bheora0 = "A0";
            }

        }
        l.Add(ram + "-");
        l.Add(bheora0);
        l.Add(ram);
        if (Isword && adr % 2 == 0 && an.getCurrentRamAdresses().Contains((segment << 4) + adr + 1))
        {   //fetch word from adresse pair
            l.Add("ram" + an.getCurrentRamAdresses().IndexOf((segment << 4) + adr + 1).ToString() + "-");
        }
        else if (Isword && adr % 2 == 0)
        {
            l.Add("ram1-");
        }
        if (afterSelection.Count != 0)
        {
            l.AddRange(afterSelection);
        }




        l.Add("ITR");
        l.Add("30,0");
        l.Add("28,1");
        l.Add("31,0");
        l.Add("32,0");
        l.Add("30,0");
        l.Add("28,1");
        l.Add("31,0");
        l.Add("32,0");
        l.Add(ram);
        if (Isword && adr % 2 == 0 && an.getCurrentRamAdresses().Contains((segment << 4) + adr + 1))
        {   //fetch word from adresse pair
            l.Add("ram" + an.getCurrentRamAdresses().IndexOf((segment << 4) + adr + 1).ToString());
        }
        else if (Isword && adr % 2 == 0)
        {
            l.Add("ram1");
        }
        if (Isword && adr % 2 == 0)
        {   //set word to adresse pair
            l.Add(ram + "," + (value & 255)); //set low 
            l.Add("ram" + (an.getCurrentRamAdresses().Contains((segment << 4) + adr + 1) ? an.getCurrentRamAdresses().IndexOf((segment << 4) + adr + 1).ToString() : "1") + "," + (value >> 8));//set high
        }
        else if (Isword && adr % 2 != 0)
        {   //set word to impair 
            l.Add(ram + "," + (value & 255)); //set low 
            SetRam(segment, adr + 1, false, l, value >> 8, afterSelection);//set high
        }
        else
        {
            l.Add(ram + "," + (CPU.GetTwosComplement(value, Mathf.Abs(value) <= 255 ? true : false)));
        }

        l.Add("-ALL");
        return l;

    }

    List<string> SetRam(int segment, int adr, bool Isword, List<string> ls, int value, List<string> afterSelection)
    {  // a ecrire pour animation d'ecrir sur la ram
        List<string> l = ls;
        string bheora0;
        string ram;
        l.Add("ITR");
        l.Add("29,0");
        l.Add("DEC");
        l.Add("-ITR");
        l.Add("33,1");
        l.Add("-DEC");

        if (an.getCurrentRamAdresses().Contains((segment << 4) + adr))
        {

            if (adr % 2 == 0)
            {
                bheora0 = "BBHE";
            }
            else
            {

                bheora0 = "A0";
            }
            //          Debug.Log("found : "+((segment<<4)+ adr));
            l.Add("ram:" + an.getCurrentRamAdresses().IndexOf((segment << 4) + adr).ToString() + ":" + segment + ":" + adr);// to test if there was any sudden changes to ram display, ca regle un bug
            ram = "ram" + an.getCurrentRamAdresses().IndexOf((segment << 4) + adr).ToString();

        }
        else
        {
            //      Debug.Log("not found");
            l.Add("GetRam," + segment + "," + adr);
            if (adr % 2 == 0)
            {
                ram = "ram0";
                bheora0 = "BBHE";
            }
            else
            {
                ram = "ram1";
                bheora0 = "A0";
            }

        }
        l.Add(ram);
        l.Add(bheora0);
        l.Add(ram + "-");
        if (Isword && adr % 2 == 0 && an.getCurrentRamAdresses().Contains((segment << 4) + adr + 1))
        {   //fetch word from adresse pair
            l.Add("ram" + an.getCurrentRamAdresses().IndexOf((segment << 4) + adr + 1).ToString() + "-");
        }
        else if (Isword && adr % 2 == 0)
        {
            l.Add("ram1-");
        }
        if (afterSelection.Count != 0)
        {
            l.AddRange(afterSelection);
        }



        l.Add("ITR");
        l.Add("30,0");
        l.Add("28,1");
        l.Add("31,0");
        l.Add("32,0");
        l.Add("30,0");
        l.Add("-RDY");
        l.Add("28,1");
        l.Add("31,0");
        l.Add("32,0");
        l.Add(ram + "," + (CPU.GetTwosComplement(value, Mathf.Abs(value) <= 255 ? true : false)));

        if (Isword && adr % 2 == 0 && an.getCurrentRamAdresses().Contains((segment << 4) + adr + 1))
        {   //fetch word from adresse pair
            l.Add("ram" + an.getCurrentRamAdresses().IndexOf((segment << 4) + adr + 1).ToString());
        }
        else if (Isword && adr % 2 == 0)
        {
            l.Add("ram1");
        }
        l.Add("-ALL");
        if (Isword && adr % 2 != 0)
        {   //fetch word from impair 
            SetRam(segment, adr + 1, false, value, afterSelection);
        }
        return l;
    }

    public void clearPreviousValues()
    {
        an.clearPreviousVlaues();
    }

    public void reset(){
        an.resetALL();
    }


    List<string> SetRam2(int segment, int adr, bool Isword, int value, List<string> afterSelection)
    {  // a ecrire pour animation d'ecrir sur la ram
        List<string> l = new List<string>();
        string bheora0;
        string ram;


        if (an.getCurrentRamAdresses().Contains((segment << 4) + adr))
        {

            if (adr % 2 == 0)
            {
                bheora0 = "BBHE";
            }
            else
            {

                bheora0 = "A0";
            }
            //          Debug.Log("found : "+((segment<<4)+ adr));
            l.Add("ram:" + an.getCurrentRamAdresses().IndexOf((segment << 4) + adr).ToString() + ":" + segment + ":" + adr);// to test if there was any sudden changes to ram display, ca regle un bug
            ram = "ram" + an.getCurrentRamAdresses().IndexOf((segment << 4) + adr).ToString();

        }
        else
        {
            //      Debug.Log("not found");
            l.Add("GetRam," + segment + "," + adr);

            //an.GetRam(Emulateur.cpu_core.memory,segment,adr);
            if (adr % 2 == 0)
            {
                ram = "ram0";
                bheora0 = "BBHE";
            }
            else
            {
                ram = "ram1";
                bheora0 = "A0";
            }

        }
        l.Add(ram + "-");
        l.Add(bheora0);
        l.Add(ram);
        if (Isword && adr % 2 == 0 && an.getCurrentRamAdresses().Contains((segment << 4) + adr + 1))
        {   //fetch word from adresse pair
            l.Add("ram" + an.getCurrentRamAdresses().IndexOf((segment << 4) + adr + 1).ToString() + "-");
        }
        else if (Isword && adr % 2 == 0)
        {
            l.Add("ram1-");
        }
        if (afterSelection.Count != 0)
        {
            l.AddRange(afterSelection);
        }




        l.Add("ITR");
        l.Add("30,0");
        l.Add("28,1");
        l.Add("31,0");
        l.Add("32,0");
        l.Add("30,0");
        l.Add("28,1");
        l.Add("31,0");
        l.Add("32,0");
        l.Add(ram);
        if (Isword && adr % 2 == 0 && an.getCurrentRamAdresses().Contains((segment << 4) + adr + 1))
        {   //fetch word from adresse pair
            l.Add("ram" + an.getCurrentRamAdresses().IndexOf((segment << 4) + adr + 1).ToString());
        }
        else if (Isword && adr % 2 == 0)
        {
            l.Add("ram1");
        }
        if (Isword && adr % 2 == 0)
        {   //set word to adresse pair
            l.Add(ram + "," + (value & 255)); //set low 
            l.Add("ram" + (an.getCurrentRamAdresses().Contains((segment << 4) + adr + 1) ? an.getCurrentRamAdresses().IndexOf((segment << 4) + adr + 1).ToString() : "1") + "," + (value >> 8));//set high
        }
        else if (Isword && adr % 2 != 0)
        {  
            //set word to impair 
            l.Add(ram + "," + (value & 255)); //set low 
            SetRam(segment, adr + 1, false, l, value >> 8, afterSelection);//set high
        }
        else
        {
            l.Add(ram + "," + (CPU.GetTwosComplement(value, Mathf.Abs(value) <= 255 ? true : false)));
        }

        l.Add("-ALL");
        return l;

    }



    //generateur de sequence fetch memoire (segment,adr) selectionne zone memoire, selectione bhe ou a0, alum les bus jusque au interface memoire et retourne une list de strings



}







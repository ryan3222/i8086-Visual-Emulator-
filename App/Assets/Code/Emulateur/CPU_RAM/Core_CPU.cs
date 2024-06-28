using System;
using System.Collections.Generic;
using UnityEngine;


    public class CPU
    {
        public Registres registers;
        public Memory memory;
        public Addressing addressing;

        public SymbolTable ST;

        public Queue<Instruction> FilInst=new Queue<Instruction>();
        private SequenceGenerator sq;

        public SequenceGenerator seq{get{return sq;}set{sq=value;}}

   
    //meme chose pour les flags et fils instructions

        public CPU()
        {
            registers = new Registres();
            memory = new Memory();
            addressing = new Addressing(registers, memory);
            
        }
        public void setSymbolTable(SymbolTable st){
             ST=st;
             addressing.setSymbolTable(st);
        }
        
        
        public void labelerr(List<Instruction> lst, SymbolTable st)
        {
            foreach (Instruction instruction in lst)
            {
                if (instruction.Keyword == "jmp") {
                     if (st.Lookup(instruction.Destination.Value) == null)
                     {
                        throw new Exception("NOM DE Label N'EXISTE PAS");
                     }
                }
            }
        } 


        public void LoadDataDefintions(List<DataDefinitons>? ld){
                 
                  int ds = GetRegValue("DS");
                  int i = 0;
                  int adr=0;
                  while ( i < ld.Count)  //stockage de code en memoire
                  {
                    //define the adresse for the variable in symbol table
                     ST.GetSymbol(ld[i].varName).Address=adr;
                    if(ld[i].initVals.Count==0)
                    { 
                       //lesser la place pour en moin une valeur
                        ld[i].initVals.Add(0);
                        Debug.Log("empty");
                    }
                    foreach (int initVal in ld[i].initVals)
                    {
                        
                         intiliaserval(initVal,ref ds,ref adr, ld[i].size);
                    }
                    i++;
                    
                      
                  }
        }

        

         public void intiliaserval(int ValInitial,ref int ds,ref int adr, string size)
         {
            if (size == "dw")
            {

                ushort value = (ushort)ValInitial;
                byte lowerHalf = (byte)(value & 0x00FF);
                byte higherHalf = (byte)(value >> 8);
                memory.Set(ds+adr, lowerHalf);//little endian
                adr++;
                memory.Set(ds + adr, higherHalf);
                adr++;
            }
            else if (size == "db")
            {
                memory.Set(ds + adr, ValInitial);
                adr++;
            }

         }


    public void LoadCode(List<Instruction> code)
     {
            int cs = GetRegValue("CS");
            int adr=0;
              foreach (Instruction instruction in code)
                {
                        instruction.code_Machine=codeMachine.getM(instruction);
                        instruction.Length =codeMachine.calcLength(instruction);
                }    
            for (int i = 0; i < code.Count; i++)  //stockage de code en memoire
            {
                if(code[i].Type==InstructionType.LABELDECLARATION || code[i].Type==InstructionType.PROCDELARATION){//declaration label , pas de stockage dans la memoire
                    ST.GetSymbol(code[i].Keyword).Address = adr; //adresse logique
                    Debug.Log("GGGGGGGGGGGGGG"+ ST.GetSymbol(code[i].Keyword).Name);
                } else{
                     memory.Set(cs + adr, code[i]);
                     adr=adr+code[i].Length;
                     if(code[i].Type==InstructionType.RET){   //end of procedure declaration , cpu will start executing code after it
                                 registers.regs["IP"].Set(adr);
                                 Debug.Log("RET DETECTED : "+adr);
                     }
                }  
            }
            labelerr(code, ST);
         
    }
    public static ushort GetTwosComplement(int value,bool isByte)
{
    ushort result;

    if (value >= 0)
    {
        result = (ushort)value;
    }
    else
    {
        
        result = (ushort)(~(-value) + 1);
        if(isByte)
           result=(byte)result;
    }

    return result;
}

        public Instruction getCurrentInstruction(){
            int ip = GetRegValue("IP");
             MemByte instbyte = memory.Get(GetRegValue("CS") + ip);
            Instruction instruction=instbyte.Instruction;
            return instruction;
        }

        public int GetRegValue(string registre)
        {
            if(registre[1].ToString().ToLower()=="l")
                return registers.regs[registre[0]+"x"].Get('l');
            else if(registre[1].ToString().ToLower()=="h")
               return registers.regs[registre[0]+"x"].Get('h');
            else
                return registers.regs[registre].Get();
        }

        public int numberOfOnes(int number)
        {
            int count = 0;
            for (int i = 0; i < 8; i++)
            {
             if ((number & (1 << i)) != 0)
             {
                count++;
             }
            }
            return count;
            
        }

    public void Step()
        {
            int ip = GetRegValue("IP");
            MemByte instbyte = memory.Get(GetRegValue("CS") + ip);
            Instruction instruction=instbyte.Instruction;
            Debug.Log("STEPING");   
            string mnemonic = instruction.Keyword;
            Operand  op1 = instruction.Destination;
            Operand op2 = instruction.Source;

            Dictionary<string,Register> regs = registers.regs;
            Func<Operand, int> getAddr = (target) => addressing.Get(target);
            Action<Operand, int, Operand> setAddr = (target, value, Operand) => addressing.Set(target, value, Operand);

            if (mnemonic == null)
            {
                throw new SyntaxException("Invalid instruction at the current instruction pointer");
            }

      /*      if(FilInst.Count>0){
                int s=0;
                foreach (Instruction item in FilInst)
                {
                   s+=item.Length;
                    
                }
                if(getCurrentInstruction().Length<=6-s){
                    FilInst.Enqueue(getCurrentInstruction());
                }
            }*/
            if(mnemonic.ToUpper()[0]!='J'){   //c'est pas un jmp
                sq.fetchInstruction(memory,GetRegValue("CS"),ip,true);  
            }
           
            switch (mnemonic.ToUpper())
            {
                
                
                case "MOV":
                    if (op1.size < op2.size && (op2.Type!=OperandType.MemoryAdresse && op2.Type!=OperandType.variable))
                    {
                        
                        sq.reset();
                        throw new SyntaxException($"Can't move larger {op2.Value} {op2.size} bit value to {op1.Value}{op1.size } bit location");
                    }
                //Debug.Log("8 bit value is "+GetTwosComplement(getAddr(op2),op1.size==8?true:false));
                setAddr(op1, GetTwosComplement(getAddr(op2), op1.size == 8 ? true : false), op2);

                sq.movInstruction(op2,op1);
                    break;

                case "JS":
                    if (registers.flagRegister.GetFlag(Flags.Sign) == 1)
                    {
                        ip = ST.GetSymbol(op1.Value).Address - 1;
                    }
                    break;


                  case "JMP":  //accepts only labels as argument
                    if (ST.Lookup(op1.Value)!=null) //label exist
                    {
                        ip = ST.GetSymbol(op1.Value).Address - 1;
                    }

                    sq.fetchInstruction(memory,GetRegValue("CS"),ip,true);  
                    break;

                case "JNS":
                    if (registers.flagRegister.GetFlag(Flags.Sign) == 0)
                    {
                     ip = ST.GetSymbol(op1.Value).Address - 1;   
                    }
                    break;

                case "JO":
                    if (registers.flagRegister.GetFlag(Flags.Overflow) == 1)
                    {
                        ip = ST.GetSymbol(op1.Value).Address - 1;
                    }
                    break;

                case "JNO":
                    if (registers.flagRegister.GetFlag(Flags.Overflow) == 0)
                    {
                        ip = ST.GetSymbol(op1.Value).Address - 1;
                    }
                    break;

                case "JP":
                case "JPE":
                    if (registers.flagRegister.GetFlag(Flags.Parity) == 1)
                    {
                        ip = ST.GetSymbol(op1.Value).Address - 1;
                    }
                    break;

                case "JNP":
                    if (registers.flagRegister.GetFlag(Flags.Parity) == 0)
                    {
                        ip = ST.GetSymbol(op1.Value).Address - 1;
                    }
                    break;

                case "ADD":
                    bool zero,carry,overflow,parity,auxcarry,sign;
                    if (op2 == null)
                    {
                        int s = op1.size == 8 ? regs["AX"].Get('l') : regs["AX"].Get();
                        s += getAddr(op1);
                        regs["AX"].Set(s);
                    }
                    else
                    {
                    int x =GetTwosComplement(getAddr(op1),op1.size==8?true:false);
                    int y =GetTwosComplement(getAddr(op2),op1.size==8?true:false);
                    int s = x + y;


                    
                       
                       carry = (s < x || s < y); // check if the addition caused a carry out of the MSB
                       overflow = ((x & 0x8000) == (y & 0x8000)) && ((x & 0x8000) != (s & 0x8000)); // check if the addition caused a signed overflow
                       sign = (s & 0x8000) != 0;

                        // Set the zero flag based on the result
                        zero = s == 0;

                        // Set the parity flag based on the number of set bits in the result
                        parity = ((s & 0xFF) ^ ((s >> 8) & 0xFF)) == 0;

                        // Set the auxiliary carry flag based on the low nibble of the result
                        auxcarry = ((x & 0xF) + (y & 0xF)) > 0xF;

                    if (sign)
                        registers.flagRegister.SetFlag(Flags.Sign);
                    if (zero)
                        registers.flagRegister.SetFlag(Flags.Zero);
                    if(carry)
                         registers.flagRegister.SetFlag(Flags.Carry);
                    if(overflow)
                         registers.flagRegister.SetFlag(Flags.Overflow);
                    if(auxcarry)
                         registers.flagRegister.SetFlag(Flags.Auxilliary);
                    if(parity)
                         registers.flagRegister.SetFlag(Flags.Parity);
                    setAddr(op1, s, op2);

                    }
                    sq.ArithmeticInstruction(op2, op1,"ADD");
                    break;

                 case "DIV":
                    int z = getAddr(op1); //diviseur
                    int w = registers.regs["AX"].Get();
                     Debug.Log("ZERO INTERED");
                    if(z==0){
                        Debug.Log("ZERO INTERED");
                        Emulateur.reload();
                        throw new Exception("division by 0 !");
                        
                    }
                    int resultat = w / z;
                    int reste=w%z;
                    
                    
                    if((Parser.IsWord(resultat)&& !Parser.IsByte(resultat)) ||( Parser.IsWord(reste)&& !Parser.IsByte(reste))){
                      registers.regs["AX"].Set(resultat);
                      registers.regs["DX"].Set(reste);

                    }else{
                      registers.regs["AX"].Set(resultat,'l');
                      registers.regs["AX"].Set(reste,'h');
                    }

                    sq.DivMulInstruction(op1,"DIV");
                    break;

                    case "MUL":
                    int m = getAddr(op1);
                    int n = registers.regs["AX"].Get();
                    int reslt = m * n;
                    if (Parser.IsWord(reslt)) { 
                       registers.regs["AX"].Set(reslt);
                    }
                    int regdx = (reslt >> 16) & 0xFFFF; 
                    registers.regs["AX"].Set(reslt & 0xFFFF); 
                    registers.regs["DX"].Set(regdx); 
                    sq.DivMulInstruction(op1,"MUL");
                    break;

                 case "SUB":
                    if (op2 == null)
                    {
                        int s = op1.size == 8 ? regs["AX"].Get('l') : regs["AX"].Get();
                        s += getAddr(op1);
                        regs["AX"].Set(s);
                    }
                    else
                    {
                    int x = getAddr(op1);
                    int y = getAddr(op2);
                    int s = x - y;
                    if (Parser.IsByte(x) && Parser.IsByte(y) && !Parser.IsByte(s))
                    {  //carry
                        registers.flagRegister.SetFlag(Flags.Carry);
                    }
                    if (Parser.IsDouble(s) && !Parser.IsWord(s) && op1.size <= 16) //overflow 
                        registers.flagRegister.SetFlag(Flags.Overflow);
                    if (Parser.IsWord(s) && !Parser.IsByte(s) && op1.size == 8) //overflow 
                        registers.flagRegister.SetFlag(Flags.Overflow);

                    if (s < 0)
                        registers.flagRegister.SetFlag(Flags.Sign);
                    if (s == 0)
                        registers.flagRegister.SetFlag(Flags.Zero);



                    setAddr(op1, s, op2);
                    if (numberOfOnes(getAddr(op1)) % 2 == 0)
                    {
                        registers.flagRegister.SetFlag(Flags.Parity);
                    }
                    }
                    sq.ArithmeticInstruction(op2, op1, "SUB");
                    
                    break;

                case "INC":  //accepts only registres as operand
                   Emulateur.historiqueRegs["PS"].Push(new Info(registers.flagRegister.Get(),AnimationController.instNumber-1));
            //      if(op1.Type!=OperandType.Register){
                //    sq.reset();
                //    throw new SyntaxException($"expected register");
                //  }
                     if(op1.size==16){
                        ushort r1 =GetTwosComplement(getAddr(op1) + 1,false);
                        ushort value=GetTwosComplement(getAddr(op1) ,false);
                        if(r1==0) 
                          registers.flagRegister.SetFlag(Flags.Zero);
                        if((r1& 0x8000) != 0) 
                          registers.flagRegister.SetFlag(Flags.Sign);
                        if(value == 0x7FFF) 
                          registers.flagRegister.SetFlag(Flags.Overflow);
                    }else{
                        byte r1 =(byte)GetTwosComplement(getAddr(op1) + 1,true);
                        byte value=(byte)GetTwosComplement(getAddr(op1) ,true);
                        if(r1==0) 
                          registers.flagRegister.SetFlag(Flags.Zero);
                        if((r1& 0x80) != 0) 
                          registers.flagRegister.SetFlag(Flags.Sign);
                        if(value == 0x7F) 
                          registers.flagRegister.SetFlag(Flags.Overflow);
                    }

                setAddr(op1, GetTwosComplement(getAddr(op1) + 1, op1.size == 8 ? true : false), null);
                Debug.Log(getAddr(op1));
                    sq.DecIncInstruction(op1,"INC");
                    
                break;

                case "DEC":  //accepts only registres as operand
                 //   if(op1.Type!=OperandType.Register){
                      //  sq.reset();
              //         throw new SyntaxException($"expected register");
                 //  }
                    if(op1.Type!=OperandType.Register){
                       throw new SyntaxException($"expected register");
                   }
                    //if(op1.Type!=OperandType.Register){
                      // throw new SyntaxException($"expected register");
                   //}
                   Emulateur.historiqueRegs["PS"].Push(new Info(registers.flagRegister.Get(),AnimationController.instNumber-1));
   
                    //test des flags
                    if(op1.size==16){
                        ushort r1 =GetTwosComplement(getAddr(op1) - 1,false);
                        ushort value=GetTwosComplement(getAddr(op1) ,false);
                        if(r1==0) 
                          registers.flagRegister.SetFlag(Flags.Zero);
                        if((r1& 0x8000) != 0) 
                          registers.flagRegister.SetFlag(Flags.Sign);
                        if(value == 0x7FFF) 
                          registers.flagRegister.SetFlag(Flags.Overflow);
                    }else{
                        byte r1 =(byte)GetTwosComplement(getAddr(op1) - 1,true);
                        byte value=(byte)GetTwosComplement(getAddr(op1) ,true);
                        if(r1==0) 
                          registers.flagRegister.SetFlag(Flags.Zero);
                        if((r1& 0x80) != 0) 
                          registers.flagRegister.SetFlag(Flags.Sign);
                        if(value == 0x7F) 
                          registers.flagRegister.SetFlag(Flags.Overflow);
                    }
                setAddr(op1, GetTwosComplement(getAddr(op1) - 1, op1.size == 8 ? true : false), null);
                Debug.Log("DEC RESULT IS "+GetTwosComplement(getAddr(op1) - 1,op1.size==8?true:false)+" "+(getAddr(op1) - 1));

                    
                    sq.DecIncInstruction(op1,"DEC");
                    break;

               case "XOR":
            
                if (op2 == null) //si la 2�me operande est null, l'operation NOT va etre execut� sur la premiere operande
                {
                    int x = getAddr(op1);
                    x = ~x;
                    setAddr(op1, x, null);
                }
                else
                {
                    int x = getAddr(op1);
                    int y = getAddr(op2);
                    int s = x ^ y;

                    
                    registers.flagRegister.UnsetFlag(Flags.Overflow); //XOR est une op�ration de bit � bit, il n'y a pas de d�passement de la plage de valeurs permises, et donc le flag OF est toujours mis � 0
                    registers.flagRegister.UnsetFlag(Flags.Carry); //car il n'y a pas de report de retenue � partir du bit de signe
                    if (s < 0)
                        registers.flagRegister.SetFlag(Flags.Sign);
                    if (s == 0)
                        registers.flagRegister.SetFlag(Flags.Zero);

                    setAddr(op1, s, op2);
                    if (numberOfOnes(getAddr(op1)) % 2 == 0)
                    {
                        registers.flagRegister.SetFlag(Flags.Parity);
                    }
                }
                sq.ArithmeticInstruction(op2, op1, "XOR");

                break;

               case "OR":
                int value1 = getAddr(op1);
                int value2 = getAddr(op2);
                int resltat = value1 | value2;
                setAddr(op1, resltat, op2);
                registers.flagRegister.UnsetFlag(Flags.Carry);
                registers.flagRegister.UnsetFlag(Flags.Overflow);
                if (resltat == 0) { 
                registers.flagRegister.SetFlag(Flags.Zero);
                }
                if ((resltat & 0x8000) != 0) //si le r�sultat de l'op�ration a le bit de poids fort � 1
                {
                    registers.flagRegister.SetFlag(Flags.Sign);
                }
                if (numberOfOnes(getAddr(op1)) % 2 == 0)
                {
                    registers.flagRegister.SetFlag(Flags.Parity);
                }
                sq.ArithmeticInstruction(op2, op1, "OR");

                break;

            case "AND":
                    int val1 = getAddr(op1);
                    int val2 = getAddr(op2);
                    int rslt = val1 & val2;
                    if (rslt == 0)
                    {
                        registers.flagRegister.SetFlag(Flags.Zero);
                    }
                    if (rslt < 0)
                    {
                        registers.flagRegister.SetFlag(Flags.Sign);
                    }
                    setAddr(op1, rslt, op2);
                    if (numberOfOnes(getAddr(op1)) % 2 == 0)
                    {
                        registers.flagRegister.SetFlag(Flags.Parity);
                    }
                sq.ArithmeticInstruction(op2, op1, "AND");

                break;

               case "CMP":
                 int op1Value = getAddr(op1);
                 int op2Value = getAddr(op2);
                 int result = op1Value - op2Value;
                 //les flags changent de la meme maniere que dans une instruction SUB
                 if (Parser.IsByte(op1Value) && Parser.IsByte(op2Value) && !Parser.IsByte(result)) // Carry
                    registers.flagRegister.SetFlag(Flags.Carry);

                 if (Parser.IsDouble(result) && !Parser.IsWord(result) && op1.size <= 16) // Overflow
                    registers.flagRegister.SetFlag(Flags.Overflow);

                 if (Parser.IsWord(result) && !Parser.IsByte(result) && op1.size == 8) // Overflow
                    registers.flagRegister.SetFlag(Flags.Overflow);

                 if (result < 0){ // Sign
                    registers.flagRegister.SetFlag(Flags.Sign);
                 }
                 if (result == 0){ // Zero
                    registers.flagRegister.SetFlag(Flags.Zero);
                 }
                 if (numberOfOnes(result) % 2 == 0)
                 {
                    registers.flagRegister.SetFlag(Flags.Parity);
                 }

                sq.CMPInstruction(op2, op1);


                break;

            case "NOT": //l'operation NOT change le resultat en complement de 2
                int val = getAddr(op1);
                int res = ~val;
                setAddr(op1, res, null);
                if (res == 0) { 
                    registers.flagRegister.SetFlag(Flags.Zero);
                }
                if (res < 0)
                {
                    registers.flagRegister.SetFlag(Flags.Sign);
                }
                Debug.Log(getAddr(op1));
                break;




            case "LEA":
                if (op1.Type != OperandType.Register)
                {
                //    sq.reset();
                 //   throw new SyntaxException($"expected Register in destination operand");
                }
                if (op2.Type != OperandType.variable && op2.Type != OperandType.MemoryAdresse)
                {
                 //   sq.reset();
                  //  throw new SyntaxException($"expected memory variable in source operand");
                }
                if (op2.Type == OperandType.variable) {
                    setAddr(op1, ST.GetSymbol(op2.Value).Address, op2);
                }
                if (op2.Type == OperandType.MemoryAdresse)
                {
                    setAddr(op1, int.Parse(op2.Value), op2);
                }
                sq.LEAInstruction(op2, op1);
                break; 
                    
                    default :
                    Debug.Log("Not found ");
                    break;
            case "CALL":
                if(ST.Lookup(op1.Value).Type==SymbolType.procName){
                    int sp=GetRegValue("SP");
                    Debug.Log("SYMBOL FOUND");
                    sp-=2;
                    setAddr(new Operand(OperandType.Register, "SP", 16), sp, null);
                    Emulateur.historiqueRam.TryAdd(sp,new Stack<Info>());
                    Emulateur.historiqueRam[sp].Push(new Info((int)memory.Get(GetRegValue("SS")+ sp).NumericalValue,AnimationController.instNumber-1){Type=1});
                    memory.Set(GetRegValue("SS")+sp,ip+instruction.Length);
                    ip=ST.GetSymbol(op1.Value).Address-instruction.Length;
                    sq.Call(ST.GetSymbol(op1.Value).Address);
               }else{
                    sq.reset();
                     throw new SystemException("CALL  error");
               }
            

            break;

             case "RET":
                 
                     int sp2=GetRegValue("SP");
                     int peek2=(int)memory.Get(GetRegValue("SS")+sp2).NumericalValue;
                     
                     ip=peek2-instruction.Length;
                     setAddr(new Operand(OperandType.Register, "SP", 16), sp2 - 2, null);
                     sq.Ret();
               
              



            break;

            case "PUSH":
            if(op1.size==16 && op1.Type==OperandType.Register){
                    int sp=GetRegValue("SP");
                    sp-=2;
                    setAddr(new Operand(OperandType.Register, "SP", 16), sp, null);
                    Emulateur.historiqueRam.TryAdd(sp,new Stack<Info>());
                    Emulateur.historiqueRam[sp].Push(new Info((int)memory.Get(GetRegValue("SS")+ sp).NumericalValue,AnimationController.instNumber-1){Type=1});
                    memory.Set(GetRegValue("SS")+sp,GetRegValue(op1.Value));
                   

            }else{
               // sq.reset();
                    // throw new SystemException("PUSH accepts only reg 16 bit operand");
            }
            sq.PushInstruction(op1);

            break;

            case "POP":

             if(op1.size==16 && op1.Type==OperandType.Register){
                    int sp=GetRegValue("SP");
                    int peek=(int)memory.Get(GetRegValue("SS")+sp).NumericalValue;
                    setAddr(op1, peek, null);
                    setAddr(new Operand(OperandType.Register, "SP", 16), sp + 2, null);
            }
            else{
             //   sq.reset();
                  //   throw new SystemException("POP accepts only reg 16 bit operand");
            }

             sq.PopInstruction(op1);

             break;

                
            }

              Emulateur.historiqueRegs["IP"].Push(new Info(ip,AnimationController.instNumber-1));
            regs["IP"].Set(ip + instruction.Length);
            
         


        }
    }


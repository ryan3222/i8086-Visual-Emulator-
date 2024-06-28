using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;

public class codeMachine : MonoBehaviour
{

    public static List<Instruction> list;
    static Dictionary<string,string> registerNumbers;
    static Dictionary<string,string> tailleReg;
    private void Start() {
         registerNumbers = new Dictionary<string, string>
            {
                {"AL", "000"},
                {"AH", "100"},
                {"BL", "011"},
                {"BH", "111"},
                {"CL", "001"},
                {"CH", "101"},
                {"DL", "010"},
                {"DH", "110"},
                {"AX", "000"},
                {"BX", "011"},
                {"CX", "001"},
                {"DX", "010"},
                {"SI", "110"},
                {"DI", "111"},
                {"BP", "101"},
                {"SP", "100"}
            };

        tailleReg=new Dictionary<string, string>{
                {"AL", "0"},
                {"AH", "0"},
                {"BL", "0"},
                {"BH", "0"},
                {"CL", "0"},
                {"CH", "0"},
                {"DL", "0"},
                {"DH", "0"},
                {"AX", "1"},
                {"BX", "1"},
                {"CX", "1"},
                {"DX", "1"},
                {"SI", "1"},
                {"DI", "1"},
                {"BP", "1"},
                {"SP", "1"},
                {"CS", "1"},
                {"DS", "1"},
                {"SS", "1"},
                {"ES", "1"},
                };
    }
    public static string DecimalToBinary(string decimalNumber)
{
    int decimalValue;
    if (!int.TryParse(decimalNumber, out decimalValue))
    {
        return "";
    }
    decimalValue=(int)CPU.GetTwosComplement(decimalValue,Parser.IsByte(decimalValue));
    string binary = string.Empty;

    while (decimalValue > 0)
    {
        binary = (decimalValue % 2) + binary;
        decimalValue /= 2;
    }

    return binary;
}
public static string ConvertBinaryToHex(string binaryInput)
{
    StringBuilder hexOutput = new StringBuilder();

    string[] binaryStrings = binaryInput.Split(new[] { "\n", "\r\n" }, System.StringSplitOptions.RemoveEmptyEntries);

    foreach (string binary in binaryStrings)
    {
        int padding = binary.Length % 4 == 0 ? 0 : 4 - binary.Length % 4;
        string paddedBinary = binary.PadLeft(binary.Length + padding, '0');

        StringBuilder hex = new StringBuilder(paddedBinary.Length / 4);

        for (int i = 0; i < paddedBinary.Length; i += 4)
        {
            string nibble = paddedBinary.Substring(i, 4);
            int decimalValue = System.Convert.ToInt32(nibble, 2);
            hex.Append(decimalValue.ToString("X"));
        }

        hexOutput.AppendLine(hex.ToString());
    }

    return hexOutput.ToString().TrimEnd();
}
public static string BinaryToHex(string binaryString)
{
     byte binaryByte = Convert.ToByte(binaryString, 2);
    string hexString = binaryByte.ToString("X2");
    return hexString;
}
    public static string GenerateMachineCode(List<Instruction> instructions)
    {
        string machineCodeBuilder="\n";

        foreach (Instruction instruction in instructions)
        {
         /*   string opcode = GetOpcode(instruction);
            string sourceOperandCode = GetOperandCode(instruction.Source);
            string destinationOperandCode = GetOperandCode(instruction.Destination);

            string machineCode = opcode + sourceOperandCode + destinationOperandCode+"\n";
            if(instruction.Keyword.ToLower()=="mov" || instruction.Keyword.ToLower()=="add" || instruction.Keyword.ToLower()=="mul"  )
               // machineCode=opcode+"\n";*/
            
            machineCodeBuilder+=(getM(instruction)+"\n");
        }

        return machineCodeBuilder.ToString();
    }

private static string GetOpcode( Instruction instruction)
{
    switch (instruction.Keyword.ToLower())
    {
        case "mov":
            return movInstructionMC(instruction);
        case "mul":
            return MulIntructionMC(instruction);
        case "sub":
            return "0010101";
        case "add":
            return AddInstructionMC(instruction);
        case "div":
            return "1111011"; // Div uses the same opcode as Mul
        case "not":
            return "1111011"; // Not uses the same opcode as Mul
        case "cmp":
            return "0011111";
        case "jmp":
            return "1110100";
        case "jz":
            return "0110111";
        case "xor":
            return "0011011";
        case "and":
            return "0010010";
        case "or":
            return "0001101";
        case "lea":
            return "1000111";
        case "call":
            return "1110100"; // Call uses the same opcode as Jmp
        case "push":
            return "0101000";
        case "pop":
            return "0101100";
        case "inc":
            return "0000001";
        case "dec":
            return "0000011";
        case "ret":
            return "1100001";
        default:
            return"" ;
}
}




public static string getM(Instruction instruction){
  
    switch (instruction.Keyword.ToLower())
    {
        case "mov":
            return movInstructionMC(instruction);
        case "add":
            return AddInstructionMC(instruction);
        case "mul":
          string s="";
             if(instruction.Destination.Type==OperandType.Register){         
                s="1111011"+tailleReg[instruction.Destination.Value.ToUpper()]+"11100"+registerNumbers[instruction.Destination.Value.ToUpper()];
               
             }else{
                if(instruction.Destination.Type==OperandType.MemoryAdresse)
                    s="1111011000100110"+adressOperand(instruction.Destination.Value);
                else if(instruction.Destination.Type==OperandType.variable)
                      s="1111011000100110"+adressOperand(Emulateur.cpu_core.ST.GetSymbol(instruction.Destination.Value).Address.ToString());
             }
        return s;
        case "sub":
            return SubInstructionMC(instruction);
        case "div":
           
             if(instruction.Destination.Type==OperandType.Register){         
                return "1111011"+tailleReg[instruction.Destination.Value.ToUpper()]+"11110"+registerNumbers[instruction.Destination.Value.ToUpper()];     
             }else{
                if(instruction.Destination.Type==OperandType.MemoryAdresse)
                    return "1111011100110110"+adressOperand(instruction.Destination.Value);
                else if(instruction.Destination.Type==OperandType.variable)
                      return "1111011100110110"+adressOperand(Emulateur.cpu_core.ST.GetSymbol(instruction.Destination.Value).Address.ToString());
             }
             return"";
       
        case "not":     
             if(instruction.Destination.Type==OperandType.Register){         
                return "1111011"+tailleReg[instruction.Destination.Value.ToUpper()]+"11010"+registerNumbers[instruction.Destination.Value.ToUpper()];   
             }else{
                if(instruction.Destination.Type==OperandType.MemoryAdresse)
                    return "1111011100010110"+adressOperand(instruction.Destination.Value);
                else if(instruction.Destination.Type==OperandType.variable)
                      return "1111011100010110"+adressOperand(Emulateur.cpu_core.ST.GetSymbol(instruction.Destination.Value).Address.ToString());
             }
             return "";
         

        case "cmp":
            return CMPinstrctuionMC(instruction); 
        case "jmp":
            return "11101010"+adressOperand(Emulateur.cpu_core.ST.GetSymbol(instruction.Destination.Value).Address.ToString());
        case "jz":
            return "01110100"+adressOperand(Emulateur.cpu_core.ST.GetSymbol(instruction.Destination.Value).Address.ToString());;
        case "xor":
            return XORinstrctuionMC(instruction);
        case "and":
            return ANDinstrctuionMC(instruction);
        case "or":
            return "0001101";
        case "lea":
            return "1000111";
        case "call":
            return "1110100"; // Call uses the same opcode as Jmp
        case "push":
            return "0101000";
        case "pop":
            return "0101100";
        case "inc":
            return "0000001";
        case "dec":
            return "0000011";
        case "ret":
            return "1100001";
        default:
            return"0" ;
}
}

static string  adressOperand(string adr){
    string binary=DecimalToBinary(adr);  
    binary=TransformTo16Bit(binary);
     string highBits = binary.Substring(0, 8);
      string lowBits = binary.Substring(8);
      return lowBits+highBits;

}
static string  ImmOperand(string adr){

   
    string binary=DecimalToBinary(adr);  
     if(Parser.IsByte(int.Parse(adr)))
           binary=TransformTo8Bit(binary);
     else {
         binary=TransformTo16Bit(binary);
          string highBits = binary.Substring(0, 8);
         string lowBits = binary.Substring(8);
            binary=lowBits+highBits;
     } 
    return binary;

}
static string TransformTo8Bit(string binaryString)
        {
            if (binaryString.Length > 8)
            {
                Console.WriteLine("Input binary string has more than 8 bits. Truncating to 8 bits.");
                binaryString = binaryString.Substring(0, 8);
            }
            else if (binaryString.Length < 8)
            {
                Console.WriteLine("Input binary string has less than 8 bits. Padding with leading zeros.");
                binaryString = binaryString.PadLeft(8, '0');
            }

            return binaryString;
        }
static string TransformTo16Bit(string binaryString)
        {
            string paddedString = binaryString.PadLeft(16, '0');
            return paddedString;
        }



  static string MulIntructionMC(Instruction instruction)
 {
        string machineCode = "";
    string opcode = "1111011"; // Opcode for MUL instruction

    switch (instruction.Source.Type)
    {
        case OperandType.Register:
            machineCode = opcode + "0110100" + registerNumbers[instruction.Source.Value.ToUpper()];
            break;

        case OperandType.MemoryAdresse:
            machineCode = opcode + "0001100" + adressOperand(instruction.Source.Value);
            break;

        case OperandType.variable:
            machineCode = opcode + "0001100" + adressOperand(Emulateur.cpu_core.ST.GetSymbol(instruction.Source.Value).Address.ToString());
            break;

        case OperandType.Immediate:
            machineCode = opcode + "1101100" + ImmOperand(instruction.Source.Value);
            break;
    }

    return machineCode;
 }



  
public static string movInstructionMC(Instruction instruction){
    string s="";
    string opcode;


    switch(instruction.Destination.Type){

        case OperandType.Register:
            switch(instruction.Source.Type){
                 case OperandType.Register:
                        opcode="10001011";
                        s=opcode+"11"+registerNumbers[instruction.Destination.Value.ToUpper()]+registerNumbers[instruction.Source.Value.ToUpper()];
                break;
                 case OperandType.MemoryAdresse:
                      opcode="1000101"+tailleReg[instruction.Destination.Value.ToUpper()];
                      s=opcode+"00"+registerNumbers[instruction.Destination.Value.ToUpper()]+"110"+adressOperand(instruction.Source.Value);
                       if(instruction.Destination.Value.ToLower()=="al"){ //acc,mem
                                s="10100000"+adressOperand(instruction.Source.Value.ToUpper());
                        }else if(instruction.Destination.Value.ToLower()=="ax") {
                             s="10100001"+adressOperand(instruction.Source.Value.ToUpper());
                        }
                      break;
                 case OperandType.variable:
                       opcode="1000101"+tailleReg[instruction.Destination.Value.ToUpper()];
                      s=opcode+"00"+registerNumbers[instruction.Destination.Value.ToUpper()]+"110"+adressOperand(Emulateur.cpu_core.ST.GetSymbol( instruction.Source.Value).Address.ToString());
                      if(instruction.Destination.Value.ToLower()=="al"){ //mem,acc
                                s="10100000"+registerNumbers[instruction.Destination.Value.ToUpper()]+adressOperand(Emulateur.cpu_core.ST.GetSymbol( instruction.Source.Value).Address.ToString());
                        }else if(instruction.Destination.Value.ToLower()=="ax") {
                             s="10100001"+registerNumbers[instruction.Destination.Value.ToUpper()]+adressOperand(Emulateur.cpu_core.ST.GetSymbol( instruction.Source.Value).Address.ToString());
                        }
                      break;
                 case OperandType.Immediate:
                      s="1011"+(tailleReg[instruction.Destination.Value.ToUpper()])+registerNumbers[instruction.Destination.Value.ToUpper()]+ImmOperand(instruction.Source.Value);
                break;


            }
            break;

        case OperandType.MemoryAdresse:
            switch(instruction.Source.Type){
                 case OperandType.Register:
                        opcode="1000100"+tailleReg[instruction.Source.Value.ToUpper()];
                        s=opcode+"00"+registerNumbers[instruction.Source.Value.ToUpper()]+"110"+adressOperand(instruction.Destination.Value.ToUpper());
                        if(instruction.Source.Value.ToLower()=="al"){ //mem,acc
                                s="10100010"+adressOperand(instruction.Destination.Value.ToUpper());
                        }else if(instruction.Source.Value.ToLower()=="ax") {
                             s="10100011"+adressOperand(instruction.Destination.Value.ToUpper());
                        }
                break;
                 case OperandType.Immediate:
                     opcode="1100011"+(Parser.IsByte(int.Parse(instruction.Source.Value))==true?"0":"1")+"00000";
                      s=opcode+"110"+adressOperand(instruction.Destination.Value.ToUpper())+ImmOperand(instruction.Source.Value.ToUpper());
                      Debug.Log("SPOOTED : "+ImmOperand(instruction.Source.Value.ToUpper()));
                break;
            }

        break;
          case OperandType.variable:
            switch(instruction.Source.Type){
                 case OperandType.Register:
                        opcode="1000100"+tailleReg[instruction.Source.Value.ToUpper()];
                         s=opcode+"00"+registerNumbers[instruction.Source.Value.ToUpper()]+"110"+adressOperand(Emulateur.cpu_core.ST.GetSymbol( instruction.Destination.Value).Address.ToString());
                         if(instruction.Source.Value.ToLower()=="al"){ //mem,acc
                                s="10100010"+adressOperand(Emulateur.cpu_core.ST.GetSymbol( instruction.Destination.Value).Address.ToString());
                        }else if(instruction.Source.Value.ToLower()=="ax") {
                             s="10100011"+adressOperand(Emulateur.cpu_core.ST.GetSymbol( instruction.Destination.Value).Address.ToString());
                        }
                break;
                 case OperandType.Immediate:
                      opcode="1100011"+(Parser.IsByte(int.Parse(instruction.Source.Value))==true?"0":"1")+"00000";
                      s=opcode+"110"+adressOperand(Emulateur.cpu_core.ST.GetSymbol( instruction.Destination.Value).Address.ToString())+ImmOperand(instruction.Source.Value.ToUpper());
                break;
            }

        break;
            

    }
    return s;
}

static string AddInstructionMC(Instruction instruction){
        string s="";
    string opcode;
    switch (instruction.Destination.Type)
    {       
        case OperandType.Register:
        switch(instruction.Source.Type){
            case OperandType.Register:  
                opcode="0000001"+tailleReg[instruction.Destination.Value.ToUpper()];
                s=opcode+"11"+registerNumbers[instruction.Destination.Value.ToUpper()]+registerNumbers[instruction.Source.Value.ToUpper()];

            break;
            case OperandType.MemoryAdresse:
                   opcode="0000001"+tailleReg[instruction.Destination.Value.ToUpper()];
                   s=opcode+"00"+registerNumbers[instruction.Destination.Value.ToUpper()]+"110"+adressOperand(instruction.Source.Value);
                break;

            case OperandType.variable:
                  opcode="0000001"+tailleReg[instruction.Destination.Value.ToUpper()];
                   s=opcode+"00"+registerNumbers[instruction.Destination.Value.ToUpper()]+"110"+adressOperand(Emulateur.cpu_core.ST.GetSymbol( instruction.Source.Value).Address.ToString());
                break;

            case OperandType.Immediate:
                opcode="1000001"+tailleReg[instruction.Destination.Value.ToUpper()];
                s=opcode+"11000"+registerNumbers[instruction.Destination.Value.ToUpper()]+ImmOperand(instruction.Source.Value);
                  if(instruction.Destination.Value.ToLower()=="al"){ //acc,data
                           s="00000100"+ImmOperand(instruction.Source.Value);
                        }else if(instruction.Destination.Value.ToLower()=="ax") {
                              s="00000101"+ImmOperand(instruction.Source.Value);
                        }
            break;
        }
            break;
        case OperandType.MemoryAdresse:
            switch(instruction.Source.Type){
            case OperandType.Register:
                   opcode="0000000"+tailleReg[instruction.Source.Value.ToUpper()];
                   s=opcode+"00"+registerNumbers[instruction.Source.Value.ToUpper()]+"110"+adressOperand(instruction.Destination.Value);
                   break;
            case OperandType.Immediate:
               opcode="1000001"+(Parser.IsByte(int.Parse(instruction.Source.Value))==true?"0":"1");
               s=opcode+"00000110"+adressOperand(instruction.Destination.Value)+ImmOperand(instruction.Source.Value);


            break;
        }
        break;

        case OperandType.variable:
          
           switch(instruction.Source.Type){
            case OperandType.Register:
                   opcode="0000000"+tailleReg[instruction.Source.Value.ToUpper()];
                   s=opcode+"00"+registerNumbers[instruction.Source.Value.ToUpper()]+"110"+adressOperand(Emulateur.cpu_core.ST.GetSymbol( instruction.Destination.Value).Address.ToString());
                   break;
            case OperandType.Immediate:
               opcode="1000000"+(Parser.IsByte(int.Parse(instruction.Source.Value))==true?"0":"1");
               s=opcode+"00000110"+adressOperand(Emulateur.cpu_core.ST.GetSymbol(instruction.Destination.Value).Address.ToString())+ImmOperand(instruction.Source.Value);


            break;
        }
        break;
        
        
    }

    return s;
 }
public static string BinaryAddition(string binary1, string binary2)
{
    // Convert binary strings to integers
    int decimal1 = Convert.ToInt32(binary1, 2);
    int decimal2 = Convert.ToInt32(binary2, 2);

    // Perform addition
    int sum = decimal1 + decimal2;

    // Convert sum to binary string
    string binarySum = Convert.ToString(sum, 2);

    return binarySum;
}
static string CMPinstrctuionMC(Instruction instruction){
        string s="";
    string opcode;
    switch (instruction.Destination.Type)
    {       
        case OperandType.Register:
        switch(instruction.Source.Type){
            case OperandType.Register:  
                opcode=BinaryAddition ("0000001"+tailleReg[instruction.Destination.Value.ToUpper()],"00111000");
                s=opcode+"11"+registerNumbers[instruction.Destination.Value.ToUpper()]+registerNumbers[instruction.Source.Value.ToUpper()];

            break;
            case OperandType.MemoryAdresse:
                  opcode=BinaryAddition ("0000001"+tailleReg[instruction.Destination.Value.ToUpper()],"00111000");
                   s=opcode+"00"+registerNumbers[instruction.Destination.Value.ToUpper()]+"110"+adressOperand(instruction.Source.Value);
                break;

            case OperandType.variable:
              opcode=BinaryAddition ("0000001"+tailleReg[instruction.Destination.Value.ToUpper()],"00111000");
                   s=opcode+"00"+registerNumbers[instruction.Destination.Value.ToUpper()]+"110"+adressOperand(Emulateur.cpu_core.ST.GetSymbol( instruction.Source.Value).Address.ToString());
                break;

            case OperandType.Immediate:
                opcode="1000001"+tailleReg[instruction.Destination.Value.ToUpper()];
                s=opcode+"11000"+registerNumbers[instruction.Destination.Value.ToUpper()]+ImmOperand(instruction.Source.Value);
                  if(instruction.Destination.Value.ToLower()=="al"){ //acc,data
                           s=BinaryAddition("00000100", "00111000")+ImmOperand(instruction.Source.Value);
                        }else if(instruction.Destination.Value.ToLower()=="ax") {
                              s=BinaryAddition("00000101", "00111000")+ImmOperand(instruction.Source.Value);
                        }
            break;
        }
            break;
        case OperandType.MemoryAdresse:
            switch(instruction.Source.Type){
            case OperandType.Register:
                   opcode=BinaryAddition("0000000"+tailleReg[instruction.Source.Value.ToUpper()],"00111000");
                   s=opcode+"00"+registerNumbers[instruction.Source.Value.ToUpper()]+"110"+adressOperand(instruction.Destination.Value);
                   break;
            case OperandType.Immediate:
               opcode="1000001"+(Parser.IsByte(int.Parse(instruction.Source.Value))==true?"0":"1");
               s=opcode+"00111110"+adressOperand(instruction.Destination.Value)+ImmOperand(instruction.Source.Value);


            break;
        }
        break;

        case OperandType.variable:
          
           switch(instruction.Source.Type){
            case OperandType.Register:
                   opcode="0000000"+tailleReg[instruction.Source.Value.ToUpper()];
                   s=opcode+"00"+registerNumbers[instruction.Source.Value.ToUpper()]+"110"+adressOperand(Emulateur.cpu_core.ST.GetSymbol( instruction.Destination.Value).Address.ToString());
                   break;
            case OperandType.Immediate:
               opcode="1000000"+(Parser.IsByte(int.Parse(instruction.Source.Value))==true?"0":"1");
               s=opcode+"00111110"+adressOperand(Emulateur.cpu_core.ST.GetSymbol(instruction.Destination.Value).Address.ToString())+ImmOperand(instruction.Source.Value);


            break;
        }
        break;
        
        
    }

    return s;
 }

 static string XORinstrctuionMC(Instruction instruction){
        string s="";
    string opcode;
    switch (instruction.Destination.Type)
    {       
        case OperandType.Register:
        switch(instruction.Source.Type){
            case OperandType.Register:  
                opcode=BinaryAddition ("0000001"+tailleReg[instruction.Destination.Value.ToUpper()],"00110000");
                s=opcode+"11"+registerNumbers[instruction.Destination.Value.ToUpper()]+registerNumbers[instruction.Source.Value.ToUpper()];

            break;
            case OperandType.MemoryAdresse:
                  opcode=BinaryAddition ("0000001"+tailleReg[instruction.Destination.Value.ToUpper()],"00110000");
                   s=opcode+"00"+registerNumbers[instruction.Destination.Value.ToUpper()]+"110"+adressOperand(instruction.Source.Value);
                break;

            case OperandType.variable:
              opcode=BinaryAddition ("0000001"+tailleReg[instruction.Destination.Value.ToUpper()]," 00110000");
                   s=opcode+"00"+registerNumbers[instruction.Destination.Value.ToUpper()]+"110"+adressOperand(Emulateur.cpu_core.ST.GetSymbol( instruction.Source.Value).Address.ToString());
                break;

            case OperandType.Immediate:
                opcode="1000001"+tailleReg[instruction.Destination.Value.ToUpper()];
                s=opcode+"11000"+registerNumbers[instruction.Destination.Value.ToUpper()]+ImmOperand(instruction.Source.Value);
                  if(instruction.Destination.Value.ToLower()=="al"){ //acc,data
                           s=BinaryAddition("00000100","00110000")+ImmOperand(instruction.Source.Value);
                        }else if(instruction.Destination.Value.ToLower()=="ax") {
                              s=BinaryAddition("00000101","00110000")+ImmOperand(instruction.Source.Value);
                        }
            break;
        }
            break;
        case OperandType.MemoryAdresse:
            switch(instruction.Source.Type){
            case OperandType.Register:
                   opcode=BinaryAddition("0000000"+tailleReg[instruction.Source.Value.ToUpper()],"00110000");
                   s=opcode+"00"+registerNumbers[instruction.Source.Value.ToUpper()]+"110"+adressOperand(instruction.Destination.Value);
                   break;
            case OperandType.Immediate:
               opcode="1000001"+(Parser.IsByte(int.Parse(instruction.Source.Value))==true?"0":"1");
               s=opcode+"00110110"+adressOperand(instruction.Destination.Value)+ImmOperand(instruction.Source.Value);


            break;
        }
        break;

        case OperandType.variable:
          
           switch(instruction.Source.Type){
            case OperandType.Register:
                   opcode="0000000"+tailleReg[instruction.Source.Value.ToUpper()];
                   s=opcode+"00"+registerNumbers[instruction.Source.Value.ToUpper()]+"110"+adressOperand(Emulateur.cpu_core.ST.GetSymbol( instruction.Destination.Value).Address.ToString());
                   break;
            case OperandType.Immediate:
               opcode="1000000"+(Parser.IsByte(int.Parse(instruction.Source.Value))==true?"0":"1");
               s=opcode+"00110110"+adressOperand(Emulateur.cpu_core.ST.GetSymbol(instruction.Destination.Value).Address.ToString())+ImmOperand(instruction.Source.Value);


            break;
        }
        break;
        
        
    }

    return s;
 }
 static string ANDinstrctuionMC(Instruction instruction){
        string s="";
    string opcode;
    switch (instruction.Destination.Type)
    {       
        case OperandType.Register:
        switch(instruction.Source.Type){
            case OperandType.Register:  
                opcode=BinaryAddition ("0000001"+tailleReg[instruction.Destination.Value.ToUpper()],"00100000");
                s=opcode+"11"+registerNumbers[instruction.Destination.Value.ToUpper()]+registerNumbers[instruction.Source.Value.ToUpper()];

            break;
            case OperandType.MemoryAdresse:
                  opcode=BinaryAddition ("0000001"+tailleReg[instruction.Destination.Value.ToUpper()],"00100000");
                   s=opcode+"00"+registerNumbers[instruction.Destination.Value.ToUpper()]+"110"+adressOperand(instruction.Source.Value);
                break;

            case OperandType.variable:
              opcode=BinaryAddition ("0000001"+tailleReg[instruction.Destination.Value.ToUpper()],"00100000");
                   s=opcode+"00"+registerNumbers[instruction.Destination.Value.ToUpper()]+"110"+adressOperand(Emulateur.cpu_core.ST.GetSymbol( instruction.Source.Value).Address.ToString());
                break;

            case OperandType.Immediate:
                opcode="1000001"+tailleReg[instruction.Destination.Value.ToUpper()];
                s=opcode+"11000"+registerNumbers[instruction.Destination.Value.ToUpper()]+ImmOperand(instruction.Source.Value);
                  if(instruction.Destination.Value.ToLower()=="al"){ //acc,data
                           s=BinaryAddition("00000100","00100000")+ImmOperand(instruction.Source.Value);
                        }else if(instruction.Destination.Value.ToLower()=="ax") {
                              s=BinaryAddition("00000101","00100000")+ImmOperand(instruction.Source.Value);
                        }
            break;
        }
            break;
        case OperandType.MemoryAdresse:
            switch(instruction.Source.Type){
            case OperandType.Register:
                   opcode=BinaryAddition("0000000"+tailleReg[instruction.Source.Value.ToUpper()],"00100000");
                   s=opcode+"00"+registerNumbers[instruction.Source.Value.ToUpper()]+"110"+adressOperand(instruction.Destination.Value);
                   break;
            case OperandType.Immediate:
               opcode="1000001"+(Parser.IsByte(int.Parse(instruction.Source.Value))==true?"0":"1");
               s=opcode+"00100110"+adressOperand(instruction.Destination.Value)+ImmOperand(instruction.Source.Value);


            break;
        }
        break;

        case OperandType.variable:
          
           switch(instruction.Source.Type){
            case OperandType.Register:
                   opcode="0000000"+tailleReg[instruction.Source.Value.ToUpper()];
                   s=opcode+"00"+registerNumbers[instruction.Source.Value.ToUpper()]+"110"+adressOperand(Emulateur.cpu_core.ST.GetSymbol( instruction.Destination.Value).Address.ToString());
                   break;
            case OperandType.Immediate:
               opcode="1000000"+(Parser.IsByte(int.Parse(instruction.Source.Value))==true?"0":"1");
               s=opcode+"00100110"+adressOperand(Emulateur.cpu_core.ST.GetSymbol(instruction.Destination.Value).Address.ToString())+ImmOperand(instruction.Source.Value);


            break;
        }
        break;
        
        
    }

    return s;
 }
static string SubInstructionMC(Instruction instruction){
        string s="";
    string opcode;
    switch (instruction.Destination.Type)
    {       
        case OperandType.Register:
        switch(instruction.Source.Type){
            case OperandType.Register:  
                opcode="0010101"+tailleReg[instruction.Destination.Value.ToUpper()];
                s=opcode+"11"+registerNumbers[instruction.Destination.Value.ToUpper()]+registerNumbers[instruction.Source.Value.ToUpper()];

            break;
            case OperandType.MemoryAdresse:
                   opcode="0010101"+tailleReg[instruction.Destination.Value.ToUpper()];
                   s=opcode+"00"+registerNumbers[instruction.Destination.Value.ToUpper()]+"110"+adressOperand(instruction.Source.Value);
                break;

            case OperandType.variable:
                  opcode="0010101"+tailleReg[instruction.Destination.Value.ToUpper()];
                   s=opcode+"00"+registerNumbers[instruction.Destination.Value.ToUpper()]+"110"+adressOperand(Emulateur.cpu_core.ST.GetSymbol( instruction.Source.Value).Address.ToString());
                break;

            case OperandType.Immediate:
                opcode="0010101"+tailleReg[instruction.Destination.Value.ToUpper()];
                s=opcode+"11000"+registerNumbers[instruction.Destination.Value.ToUpper()]+ImmOperand(instruction.Source.Value);
                  if(instruction.Destination.Value.ToLower()=="al"){ //acc,data
                           s="00000100"+ImmOperand(instruction.Source.Value);
                        }else if(instruction.Destination.Value.ToLower()=="ax") {
                              s="00000101"+ImmOperand(instruction.Source.Value);
                        }
            break;
        }
            break;
        case OperandType.MemoryAdresse:
            switch(instruction.Source.Type){
            case OperandType.Register:
                   opcode="0010100"+tailleReg[instruction.Source.Value.ToUpper()];
                   s=opcode+"00"+registerNumbers[instruction.Source.Value.ToUpper()]+"110"+adressOperand(instruction.Destination.Value);
                   break;
            case OperandType.Immediate:
               opcode="0010101"+(Parser.IsByte(int.Parse(instruction.Source.Value))==true?"0":"1");
               s=opcode+"00000110"+adressOperand(instruction.Destination.Value)+ImmOperand(instruction.Source.Value);


            break;
        }
        break;

        case OperandType.variable:
          
           switch(instruction.Source.Type){
            case OperandType.Register:
                   opcode="0010100"+tailleReg[instruction.Source.Value.ToUpper()];
                   s=opcode+"00"+registerNumbers[instruction.Source.Value.ToUpper()]+"110"+adressOperand(Emulateur.cpu_core.ST.GetSymbol( instruction.Destination.Value).Address.ToString());
                   break;
            case OperandType.Immediate:
               opcode="0010100"+(Parser.IsByte(int.Parse(instruction.Source.Value))==true?"0":"1");
               s=opcode+"00000110"+adressOperand(Emulateur.cpu_core.ST.GetSymbol(instruction.Destination.Value).Address.ToString())+ImmOperand(instruction.Source.Value);


            break;
        }
        break;
        
        
    }

    return s;
 }



private static string GetOperandCode(Operand operand)
{
    if (operand.Type == OperandType.Register)
    {
        // Register
        return GetRegisterCode(operand.Value);
    }
    else if (operand.Type == OperandType.Immediate)
    {
        // Immediate value
        return "0" +DecimalToBinary(operand.Value);
    }
    else if (operand.Type == OperandType.MemoryAdresse)
    {
        // Indirect addressing (variable)
        return "11";
    }
    else if(operand.Type==OperandType.variable)
    {
        // Direct addressing (variable)
        return "00";
    }else{
        return "";
    }
}

private static string GetRegisterCode(string registerName)
{
    switch (registerName.ToUpper())
    {
        case "AX":
            return "000";
        case "BX":
            return "011";
        case "CX":
            return "001";
        case "DX":
            return "010";
        case "SI":
            return "100";
        case "DI":
            return "101";
        case "BP":
            return "110";
        case "SP":
            return "111";
        case "AH":
            return "100";
        case "AL":
            return "000";
        case "BH":
            return "101";
        case "BL":
            return "001";
        case "CH":
            return "110";
        case "CL":
            return "010";
        case "DH":
            return "111";
        case "DL":
            return "011";
        default:
            return "";
    }
}
  public static int calcLength(Instruction instruction){
        return GetLengthInBytes(getM(instruction));

  }
public static int GetLengthInBytes(string binaryString)
    {
        // Assuming the binary string represents a valid sequence of bytes
        int numBits = binaryString.Length;
        int numBytes = (numBits + 7) / 8; // Round up to the nearest byte

        return numBytes;
    }

    public void OnButtonClick(){


        Operand d=new Operand(OperandType.Register,"bx",16);
        Instruction ins=new Instruction();
        ins.Destination=d;
        ins.Keyword="div";
        ins.Source=null;
        ins.Type=InstructionType.OneOperand;
        ins.code_Machine=getM(ins);
    Debug.Log("TEEST BINARY TO HEX 11110000"+"   "+BinaryToHex("11110000"));

    Debug.Log("TTESST INSTRUCTION"+ins.getByteMC(0));
        Debug.Log("DIV BX --------"+ins.code_Machine+" FIRST BYTE ITS "+ins.getByteMC(0));

        string s=ConvertBinaryToHex(GenerateMachineCode(list));
        string[] lines = s.Split(new[] { "\n", "\r\n" }, System.StringSplitOptions.RemoveEmptyEntries);
        string s2=GenerateMachineCode(list);
        string[] lines2 = s2.Split(new[] { "\n", "\r\n" }, System.StringSplitOptions.RemoveEmptyEntries);
        for(int i=0;i<lines.Length;i++){
                Debug.Log(list[i].Keyword+ "   : "+lines[i]+ "   "+lines2[i]);
        }

    Debug.Log("50   " +ImmOperand("50"));


    }
}


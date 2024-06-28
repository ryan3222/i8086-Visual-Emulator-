using System;
using System.Collections.Generic;
using UnityEngine;

public class Parser {
    private List<Token> tokens;
    private int position;

    
        public List<Instruction> Glist;
        public List<DataDefinitons> Gdata=new List<DataDefinitons>();
        private SymbolTable ST;
        Token tk;
        bool assume = true;

     





    public Parser(List<Token> tokens) {
        this.tokens = tokens;
        this.position = 0;
        
    }

    private Token Consume(TokenType type) {
        if (position >= tokens.Count) {
            throw new SyntaxException($"Unexpected end of input. Expected {type}");
        }

        Token token = tokens[position];
        if (token.Type != type) {
            throw new SyntaxException($"Unexpected token {token.Value}. Expected {type}");
        }
       


        position++;
        return token;
    }
    private Token Advance() {
        if (!IsAtEnd()) {
            position++;
        }
        return Previous();
    }
    private void GoBack() {
        
            position--;
        
        
    }

    private Token Peek() {
        return tokens[position];
    }

    private Token Previous() {
        return tokens[position - 1];
    }

    private bool IsAtEnd() {
        return Peek().Type == TokenType.EOF;
    }

    private bool Match(TokenType type) {
        if (position >= tokens.Count) {
            return false;
        }

        if (tokens[position].Type != type) {
            return false;
        }

        position++;
        return true;
    }

     private bool Peek(TokenType type) {
        if (position >= tokens.Count) {
            return false;
        }

        if (tokens[position].Type != type) {
            return false;
        }

   
        return true;
    }

    private void AssumeStatement(List<Instruction> list) {
        if(Match(TokenType.assumeKW)){
            Instruction assumeInstruction=new Instruction();
        assumeInstruction.Keyword=Previous().Value;
        assumeInstruction.Type=GetInstructionType(Previous().Value);
        assumeInstruction.Length=2;
        Consume(TokenType.Registre);   //cs
        Consume(TokenType.twopoints);
        Consume(TokenType.Identifier);
        assumeInstruction.Source=ParseOperand(Previous(),false); //cs segment name
        ST.AddSymbol(Previous().Value,-1,-1,null,SymbolType.segmentName);
        Consume(TokenType.separator);
        Consume(TokenType.Registre); //ds
        Consume(TokenType.twopoints);
        Consume(TokenType.Identifier);
         assumeInstruction.Destination=ParseOperand(Previous(),false); //ds segment name
         assume = false;
          ST.AddSymbol(Previous().Value,-1,-1,null,SymbolType.segmentName);
        ExpectNewLines();
        //addinstruction(list, assumeInstruction);
        }else{
            //expected Assume
        }
    }
     private Operand ParseOperand(Token token,bool isAdress) {

        if (token == null) {
            return null;
        }else if(isAdress){
            return new Operand(OperandType.MemoryAdresse,token.Value,16);  //DIRECT sous forme [adresse] MAIS NORMALMENT C 8 PUSIQUE UNE ADRESSE POINTE DIRECTEMENT SUR UN OCTET 
        }
        else if (token.Type == TokenType.Number) {
            if (int.Parse(token.Value, System.Globalization.NumberStyles.AllowLeadingSign) >= -128 && int.Parse(token.Value, System.Globalization.NumberStyles.AllowLeadingSign) <= 127)
            {
                return new Operand(OperandType.Immediate, token.Value, 8);
            }
            else if (int.Parse(token.Value, System.Globalization.NumberStyles.AllowLeadingSign) >= -32768 && int.Parse(token.Value, System.Globalization.NumberStyles.AllowLeadingSign) <= 32767)
            {
                return new Operand(OperandType.Immediate, token.Value, 16);
            }
        }
        else { //donc soit registre ou bien reference a une variable exemple : mov reg,reg ou mov reg,Var

            if (token.Type==TokenType.Registre){
                switch (token.Value)
                {
                    case "ax":
                    case "bx":
                    case "cx":
                    case "dx":
                    case "si":
                    case "bp":
                    case "ds":
                        return new Operand(OperandType.Register, token.Value, 16);//later test if actually reg or string
                    case "al":
                    case "ah":
                    case "bl":
                    case "bh":
                    case "cl":
                    case "ch":
                    case "dl":
                    case "dh":
                        return new Operand(OperandType.Register, token.Value, 8);//later test if actually reg or string
                }
            }
            else if(token.Type==TokenType.Identifier){  //peut etre une variable , tester dans les teste semantique apr

                if (assume == true) {
                    return new Operand( OperandType.variable, token.Value, -1);//later test if actually reg or string
                }
                Symbol sb ;
                try{
                      sb=ST.GetSymbol(token.Value);
                      return new Operand(OperandType.variable, token.Value, sb.Size); //if variable
                }catch{
                   return new Operand(OperandType.variable, token.Value, -1 ); //if label
                }
                
               

            }
        }
        throw new SyntaxException("erreur operand type");
    }
    private void DataStatementList(List<DataDefinitons> list) {
        DataDefinitons newData=new DataDefinitons();
        if (Match(TokenType.Identifier)&&tokens[position].Type==TokenType.dataDef) { //ambuiguité bricolé !!!
            if (tokens[position].Value=="db")
            {
                ST.AddSymbol(Previous().Value, 8, position, null, SymbolType.Variable);               
            }
            else if(tokens[position].Value == "dw")
            {
                ST.AddSymbol(Previous().Value, 16, position, null, SymbolType.Variable);
            }
            newData.varName=Previous().Value;
            Consume(TokenType.dataDef);
            newData.size=Previous().Value;  
            ValueOrDup(newData,list);
            list.Add(newData);
            ExpectNewLines();
            DataStatementList(list);
        }else{
            GoBack();
        }
    }

   
    private void ValueOrDup(DataDefinitons data,List<DataDefinitons> list) {
        if (Match(TokenType.Number)) {

            if(Match(TokenType.dupKW)){//DUP NOT WORKING
              data.size=tokens[position-2].Value;
              Consume(TokenType.openSqrBracket);
              Consume(TokenType.Number);
              data.initVals=new List<int>();
              for (int i = 0; i < int.Parse(data.size, System.Globalization.NumberStyles.AllowLeadingSign); i++)
              {//intial n values of the array with the value routine
                  data.initVals.Add(int.Parse(Previous().Value, System.Globalization.NumberStyles.AllowLeadingSign));
              }
               Consume(TokenType.closeSqrBracket);
            
            }else{
                data.initVals=new List<int>();
                LstValue(data,list);
        
            }
        } 
    }
    public static bool IsByte(int value)
    {
         return value >= -128 && value <= 127;
    }

   public static bool IsWord(int value)
   {
        return value >= -32768 && value <= 32767;
   }
   public static bool IsDouble(int value)
   {
        return value >= - 2147483648 && value <=  2147483648;
   }

    private void LstValue(DataDefinitons data,List<DataDefinitons> list) {//init values




        Debug.Log(Previous().Value);
        if (data.size == "db")
        {  //ROUTINE CHECK SIZE FOR INIT VALUES
            if (!IsByte(int.Parse(Previous().Value, System.Globalization.NumberStyles.AllowLeadingSign)))
            {
                throw new Exception("init value is not byte");
            }

        }
        else if (data.size == "dw")
        {
            if (!IsWord(int.Parse(Previous().Value, System.Globalization.NumberStyles.AllowLeadingSign)))
            {
                throw new Exception("init value is not word");
            }

        }
        else if (data.size == "dd")
        {
            if (!IsDouble(int.Parse(Previous().Value, System.Globalization.NumberStyles.AllowLeadingSign)))
            {
                throw new Exception("init value is not double");
            }

        }
        data.initVals.Add(int.Parse(Previous().Value, System.Globalization.NumberStyles.AllowLeadingSign));
        MoreValues(data, list);
    }

    private void MoreValues(DataDefinitons data,List<DataDefinitons> list) {
        if (Match(TokenType.separator)) {
            Advance();
            LstValue(data,list);
       
        }
        

    }

   
      
    

    private void SegmentStatement() {
        Consume(TokenType.Identifier);//ROUTINE verfiyer si le nom de segment exsite ,B U G : acceptes nom de segment data comme cs et l'inverse 
        tk = Previous();
        if(ST.Lookup(Previous().Value)==null){
            throw new Exception("NOM DE SEGMENT N'EXISTE PAS");
        }
        Consume(TokenType.segmentKw);
        ExpectNewLines();
    }

    private void EndsStatement() {
        if (tokens[position+1].Type==TokenType.endsKw){  //pour eliminer l'ambiguité
            Consume(TokenType.Identifier);
            if (Previous().Value != tk.Value)
            {
                throw new Exception("Nom de segment different");
            }
            Consume(TokenType.endsKw);
            
            ExpectNewLines();
      }
    }
    private void ExpectNewLines(){ 

        //Attendre en moin un seul \n et ignorer les autres
        Consume(TokenType.newline);
        while(Match(TokenType.newline));
    }
    private void ProcStatementList(List<Instruction> list){
      if(Match(TokenType.Identifier)){
        Proc(list);
        ExpectNewLines();
        ProcStatementList(list);
      }

    }
    private void Proc(List<Instruction> list){

         Match(TokenType.Proc);
         ST.AddSymbol( tokens[position-2].Value,-1,0,null,SymbolType.procName); 
       
         Instruction ThisInstruction= new Instruction {  //on consider la declaration d'une etiquette comme etant une instruction pour pouvoir l'associer avec une adresse plus tard dans l'execution
            Type = InstructionType.PROCDELARATION,
            Keyword =tokens[position-2].Value,
            Destination = new Operand(OperandType.Immediate,"ADRESSE",0),
            Source = null ,//
            Length = 1 // a changer plus tard pour representer la taille physique reel de chaque instructions 
        };

        addinstruction(list, ThisInstruction);
        ExpectNewLines();
         InstructionStatementList(list);
         Match(TokenType.ret);
         Instruction ThisInstructionret= new Instruction {  //on consider la declaration d'une etiquette comme etant une instruction pour pouvoir l'associer avec une adresse plus tard dans l'execution
            Type = InstructionType.RET,
            Keyword =Previous().Value,
            Destination = new Operand(OperandType.Immediate,"",0),
            Source = null ,//
            Length = 1 // a changer plus tard pour representer la taille physique reel de chaque instructions 
        };
        ExpectNewLines();
        addinstruction(list, ThisInstructionret);
        Match(TokenType.Identifier);
        RoutineDeclarationProc();
        Match(TokenType.ENDP);
    }

    private void RoutineDeclarationProc(){//a ecrire

    }
    private void InstructionStatementList(List<Instruction> list) {
        if (Match(TokenType.InstKeyword)) {
            Token kw;
            Token op1=null;
            Token op2=null;
            bool isOp2Adress=false;
            bool isOp1Adress=false;
            kw=Previous();
              if (Match(TokenType.Identifier)|| Match(TokenType.Number)||Match(TokenType.Registre)) { 
                op1 = Previous();
                
                if (op1.Type== TokenType.Identifier && tokens[position].Type != TokenType.newline && ST.Lookup(Previous().Value) == null)
                {
                    throw new Exception("Variable non-declaré");
                }
            }
            else if(Match(TokenType.openSqrBracket)){
                if(Match(TokenType.Number)){
                    
                }else{
                    throw new SyntaxException("adress must be a number in hexadecimal");
                }
                if(Match(TokenType.closeSqrBracket)){
                    op1=tokens[position-2];//syntaxe correct [op]
                    isOp1Adress=true;

                }

            }
            else {
                throw new SyntaxException("");
            }
           if (Match(TokenType.separator)) {
            if (Match(TokenType.Identifier)|| Match(TokenType.Number)|| Match(TokenType.Registre)) {
                op2 = Previous();
                if (op2.Type == TokenType.Identifier && ST.Lookup(Previous().Value) == null)
                  {
                      throw new Exception("Variable non-declaré");
                  }
            }else if(Match(TokenType.openSqrBracket)){
                if(Match(TokenType.Number)){
                    
                }else{
                    throw new SyntaxException("adress must be a number in hexadecimal");
                }
                if(Match(TokenType.closeSqrBracket)){
                    op2=tokens[position-2];//syntaxe correct [op]
                    isOp2Adress=true;

                }

            }
            else {
                throw new SyntaxException("Expected second operand after comma");
            }
            
      
        }
       

        if(GetInstructionType(kw.Value)==InstructionType.OneOperand && op2!=null){ //routine test if instruction is with 2 op or 1 
            throw new SyntaxException("instruction only support one operand");

        }else if(GetInstructionType(kw.Value)==InstructionType.TwoOperand && op2==null){
                
              throw new SyntaxException("instruction only support two operands");
        }
          Instruction ThisInstruction= new Instruction {
            Type = GetInstructionType(kw.Value),
            Keyword = kw.Value,
            Destination = ParseOperand(op1,isOp1Adress),
            Source = ParseOperand(op2,isOp2Adress) ,//
          
        };
       
          
            //ROUTINE TEST VALIDITE DE L INSTRUCTION
            RoutineInstruction(ThisInstruction);
            ExpectNewLines();
            addinstruction(list, ThisInstruction);
            InstructionStatementList(list);


        }
        else if (tokens[position + 1].Type == TokenType.twopoints)
        { //DECLARATION LABEL    AMBUGUITEE

            DeclareLabel(list);
           ExpectNewLines();
           InstructionStatementList(list);











        }
    }


    public int getInstLengthMachinCode(Instruction instruction){
        return codeMachine.calcLength(instruction);
    }
    
   public int GetInstructionLength(string instruction, Operand[] operands)
{
    switch (instruction.ToLower())
    {
        case "mov":
            return GetMovInstructionLength(operands);
        case "mul":
        case "sub":
        case "add":
        case "div":
        case "and":
        case "or":
        case "xor":
        case "not":
        case "cmp":
        case "inc":
        case "dec":
            return GetFixedOperandInstructionLength(instruction, operands[0]);
        case "jmp":
        case "jz":
        case "call":
            return GetVariableOperandInstructionLength(instruction, operands[0]);
        case "lea":
            return GetLeaInstructionLength(operands);
        case "push":
        case "pop":
            return GetFixedOperandInstructionLength(instruction, operands[0]);
        case "ret":
            return 1;
        default:
            throw new ArgumentException("Invalid instruction keyword.");
    }
}

private int GetMovInstructionLength(Operand[] operands)
{
    Operand sourceOperand = operands[0];
    Operand destinationOperand = operands[1];

    if (IsRegister(sourceOperand))
    {
        if (IsRegister(destinationOperand))
        {
            // Register to register move
            return 2;
        }
        else if (IsMemoryOperand(destinationOperand))
        {
            // Register to memory move
            return 1+GetMemoryOperandLength(destinationOperand);
        }
        else
        {
            throw new ArgumentException("Invalid destination operand.");
        }
    }
    else if (IsMemoryOperand(sourceOperand))
    {
        if (IsRegister(destinationOperand))
        {
            // Memory to register move
            return 1+GetMemoryOperandLength(sourceOperand);
        }
        else
        {
            throw new ArgumentException("Invalid destination operand.");
        }
    }else if (IsImmediate(sourceOperand)){

        if (IsRegister(destinationOperand))
        {
            // Immediate to register move
            return 1 + (IsByte( int.Parse(sourceOperand.Value))?1:2);
        }
        else if (IsMemoryOperand(destinationOperand))
        {
            // Immediate to memory move
            return GetMemoryOperandLength(destinationOperand) + (IsByte( int.Parse(sourceOperand.Value))?1:2);;
        }
        else
        {
            throw new ArgumentException("Invalid destination operand.");
        }
    }
    else
    {
        throw new ArgumentException("Invalid source operand.");
    }
}

private int GetFixedOperandInstructionLength(string instruction, Operand operand)
{
    switch (instruction.ToLower())
    {
        case "mul":
            return 1 + GetOperandLength(operand);
        case "sub":
        case "add":
        case "div":
        case "and":
        case "or":
        case "xor":
        case "not":
        case "cmp":
        case "inc":
        case "dec":
            return 1 + GetOperandLength(operand);
        case "push":
        case "pop":
            if (IsRegister(operand))
            {
                return 1;
            }
            else if (IsMemoryOperand(operand))
            {
                return 1 + GetMemoryOperandLength(operand);
            }
            else
            {
                throw new ArgumentException("Invalid operand format.");
            }
        default:
            throw new ArgumentException("Invalid instruction keyword.");
    }
}

private int GetVariableOperandInstructionLength(string instruction, Operand operand)
{
    switch (instruction.ToLower())
    {
        case "jmp":
        case "jz":
        case "call":
            if (IsRegister(operand))
            {
                return 2; // Short jump
            }
            else if (IsImmediate(operand))
            {
                int immediate = int.Parse(operand.Value);
                if (immediate >= -128 && immediate <= 127)
                {
                    return 2; // Short jump
                }
                else
                {
                    return 5; // Near jump
                }
            }
            else
            {
                throw new ArgumentException("Invalid operand format.");
            }
        default:
            throw new ArgumentException("Invalid instruction keyword.");
    }
}

private int GetLeaInstructionLength(Operand[] operands)
{
    Operand destinationOperand = operands[0];
    Operand sourceOperand = operands[1];

    if (IsRegister(destinationOperand) && IsMemoryOperand(sourceOperand))
    {
        return 2 + GetMemoryOperandLength(sourceOperand);
    }
    else
    {
        throw new ArgumentException("Invalid operand format.");
    }
}

private int GetOperandLength(Operand operand)
{
    if (IsRegister(operand))
    {
        return 1; // Register operand length
    }
    else if (IsMemoryOperand(operand))
    {
        return GetMemoryOperandLength(operand);
    }
    else if(IsImmediate(operand)){
        return (IsByte( int.Parse(operand.Value))?1:2);
    }
    else
    {
        throw new ArgumentException("Invalid operand format.");
    }
}

private bool IsRegister(Operand operand)
{
    // Check if the operand is a valid register name
    // Implement your own logic here to validate the register name format
    // For simplicity, assuming register names are single letters (e.g., 'a', 'b', 'c')
    return operand.Type==OperandType.Register;
}

private bool IsImmediate(Operand operand)
{
    // Check if the operand is an immediate value
    // Implement your own logic here to validate the immediate value format
    // For simplicity, assuming immediate values are numeric strings (e.g., '123', '-456')
    int immediateValue;
    return operand.Type==OperandType.Immediate;
}

private bool IsMemoryOperand(Operand operand)
{
    // Check if the operand is a memory operand
    // Implement your own logic here to validate the memory operand format
    // For simplicity, assuming memory operands are enclosed in square brackets (e.g., '[mem]')
    return operand.Type==OperandType.MemoryAdresse || operand.Type==OperandType.variable;
}

private int GetMemoryOperandLength(Operand memoryOperand)
{
    // Determine the length of the memory operand based on its format
    // Implement your own logic here to handle different memory operand formats
    // For simplicity, assuming memory operands have fixed length
    return 3; // Assuming memory operands in the format '[mem]'
}


    private void DeclareLabel(List<Instruction> list){

       //pour eliminzer l'ambiguité
       
        Consume(TokenType.Identifier);
            
        RoutineDeclarationLabel(list);
        Consume(TokenType.twopoints);


        


    }


    private void RoutineInstruction(Instruction ThisIntruction)
    {
      if(ThisIntruction.Destination!=null)  {


        if(ThisIntruction.Source!=null){
        
        if (ThisIntruction.Destination.Type == OperandType.MemoryAdresse && ThisIntruction.Source.Type == OperandType.MemoryAdresse) {

    
                throw new SyntaxException("erreur mem,mem");
            
        }

        if (ThisIntruction.Destination.Type == OperandType.variable && ThisIntruction.Source.Type == OperandType.variable) {

    
                throw new SyntaxException("erreur mem,mem");
            
        }
        }
         if (ThisIntruction.Destination.Type == OperandType.Immediate && ThisIntruction.Keyword!="div") {

    
                throw new SyntaxException("destination cannot be immediate ");
            
        }


      }
       
        Operand op1=ThisIntruction.Destination;
        Operand op2=ThisIntruction.Source;
        //tester la taille des operands, les erreurs semantique aussi
         switch(ThisIntruction.Keyword.ToLower()){
                case "mov":
                        if (op1.size < op2.size && (op2.Type!=OperandType.MemoryAdresse && op2.Type!=OperandType.variable))
                    {
                        
                    
                        throw new SyntaxException($"Can't move larger {op2.Value} {op2.size} bit value to {op1.Value}{op1.size } bit location");
                    }

                break;
                case "inc":
                case "dec":
                   if(op1.Type!=OperandType.Register){
                //    sq.reset();
                    throw new SyntaxException($"expected register");
                  }
                  break;
                case "lea":
                     if (op1.Type != OperandType.Register)
                    {
                    //    sq.reset();
                        throw new SyntaxException($"expected Register in destination operand");
                    }
                    if (op2.Type != OperandType.variable)
                    {
                    //   sq.reset();
                        throw new SyntaxException($"expected memory variable in source operand");
                    }
                break;
                case "push":
                case "pop":
                     if(!(op1.size==16 && op1.Type==OperandType.Register)){
                            throw new SystemException("PUSH accepts only reg 16 bit operand");
                     }
                break;
                case "mul":
                case "div":
                case "not":

                    if(op1.Type==OperandType.Immediate){
                            throw new SystemException("div||mul|not doesnt accept immediate operand");
                     }
                 
                break;

        }

       


        //test if label exists if instruction is jump   //tester apr dans l'execution quand la table des symboles est complete !!!!!
        if (ThisIntruction.Keyword == "jmp")
        {
           ThisIntruction.Destination.Type=OperandType.label;
        }
    }

    private void addinstruction(List<Instruction> list, Instruction instruction)
    {
        list.Add(instruction);
    }

    private void RoutineDeclarationLabel(List<Instruction> list){
            ST.AddSymbol(Previous().Value,-1,position,null,SymbolType.label);//sauvgarder le nom du labler dans la table des symboles
            Instruction ThisInstruction= new Instruction {  //on consider la declaration d'une etiquette comme etant une instruction pour pouvoir l'associer avec une adresse plus tard dans l'execution
            Type = InstructionType.LABELDECLARATION,
            Keyword = Previous().Value,
            Destination = new Operand(OperandType.Immediate,"ADRESSE",0),
            Source = null ,//
            Length = 1 // a changer plus tard pour representer la taille physique reel de chaque instructions 
        };

        addinstruction(list, ThisInstruction);

        







    }
    private InstructionType GetInstructionType(string keyword) {
        switch (keyword) {
            case "mov":
            case "lea":
            case "add":
            case "sub":
            case "xor":
            case "and":
            case "assume":
            case "cmp":
            case "or":
           
                return InstructionType.TwoOperand;
            case "loop":
            case "not":
            case "inc":
             case "div":
            case "dec":
            case "jmp":
            case "jz":
            case "jnz":
            case "je":
            case "jne":
            case "jg":
            case "jge":
            case "jl":
            case "jle":
            case "call":
            case "push":
            case "pop":
            case "mul":
                return InstructionType.OneOperand;

            case "ret":
            case "nop":
                return InstructionType.ZeroOperand;

            default:
                throw new SyntaxException($"Unknown keyword: {keyword}");
        }
    }

    private void Program(List<Instruction> list,List<DataDefinitons> data) {
        AssumeStatement(list);
        SegmentStatement();
        DataStatementList(data);
        EndsStatement();
        SegmentStatement();
        ProcStatementList(list);
        InstructionStatementList(list);
        EndsStatement();
        Consume(TokenType.endKw);
    }

    public List<Instruction> ParseInstructions() {
        List<Instruction> list=new List<Instruction>();
        List<DataDefinitons> data=new List<DataDefinitons>();
        ST=new SymbolTable();
        Program(list,data);
        Gdata=data;
        Consume(TokenType.EOF);
       
        return list;
       
       

    }
       public List<DataDefinitons> ParseDataDefinitions() {
        
        return Gdata;
    }
    
   



public SymbolTable getST(){
    return ST;
}
}
 public enum InstructionType {
    TwoOperand,
    OneOperand,
    ZeroOperand,
    LABELDECLARATION,
    PROCDELARATION,
    RET
}
public class Operand
{
    public OperandType Type  { get;set; }
    public string Value { get;set; }
    public int size {get;set;} //8bit operand or 16 bit 

    public Operand(OperandType type, string value, int Size)
    {
        Type = type;
        Value = value;
        size = Size; 
    }
    public Operand(){}
}

public enum OperandType
{
    Register,
    MemoryAdresse,
    Immediate,
    variable,
    NON,
    label
}



public class Instruction :parseOutput{
    public InstructionType Type { get; set; }
    public string Keyword { get; set; }
    public Operand Source { get; set; }
    public Operand Destination { get; set; }
    public int Length;
    public string code_Machine;
     public override string ToString()
    {
        if(Source==null){ //pour les operation unaire
            Source=new Operand();
            Source.Type=OperandType.NON;
            Source.Value="NON";

        }
        return $"Type : {Keyword} keyword: {Keyword}, SourceType: {Source.Type},SourceValue: {Source.Value}, DestinationType: {Destination.Type}, DestinationValue: {Destination.Value}, Length: {Length}";
    }
    public string getOriginalString(){

        if((Source==null )|| (Source.Type==OperandType.NON)){//no source
               return Keyword+" "+Destination.Value;
        }
        if(Source.Type==OperandType.MemoryAdresse){
            return Keyword+" "+Destination.Value+","+"["+Source.Value+"]";
        }
         if(Destination.Type==OperandType.MemoryAdresse){
            return Keyword+" "+"["+Destination.Value+"]"+","+Source.Value;
        }
        return Keyword+" "+Destination.Value+","+Source.Value;
    }
    public string getCodeMachine(){
       return codeMachine.BinaryToHex(code_Machine.Trim());
        
    }
   public string getByteMC(int index){   //return one byte from machine code with index (1st byte 2nd byte...)
                int startIndex = index * 8;
            if (startIndex < 0 || startIndex >= code_Machine.Length)
            {
                throw new IndexOutOfRangeException("Invalid byte index");
            }
            
            int length = Math.Min(8, code_Machine.Length - startIndex);
            string byteString = code_Machine.Substring(startIndex, length);
            return codeMachine.BinaryToHex(byteString);
   }
}


public abstract class parseOutput{

}


public enum SymbolType{
    Variable,
    label,
    segmentName,
    procName,
    
}

public class Symbol
{
    public string Name { get; }
    public int Size { get; }
    public int Address { get; set; }
    public List<int> InitialValues { get; }
    public SymbolType Type;

    public Symbol(string name, int size, int address, List<int> initialValues,SymbolType type)
    {
        Name = name;
        Size = size;
        Address = address;
        InitialValues = initialValues;
        Type=type;
    }

    public override string ToString()
    {
        return $"Name: {Name}, Type: {Size}, Address: {Address}";
    }
}
public class SymbolTable
{
    public Dictionary<string, Symbol> symbols = new Dictionary<string, Symbol>();

    public void AddSymbol(string symbolName, int size, int address, List<int> initialVal,SymbolType type)
    {
        if (symbols.ContainsKey(symbolName))
        {
            throw new Exception($"Symbol {symbolName} already exists in the symbol table.");
        }

        symbols[symbolName] = new Symbol(symbolName, size, address, initialVal, type);
    }

    public Symbol GetSymbol(string symbolName)
    {
        if (!symbols.ContainsKey(symbolName))
        {
            throw new Exception($"Symbol {symbolName} does not exist in the symbol table.");
        }

        return symbols[symbolName];
    }
     public Symbol Lookup(string name)
    {
        if (symbols.ContainsKey(name))
        {
            return symbols[name];
        }

        return null;
    }
}


public class DataDefinitons : parseOutput {
    
    public string size {get; set;}
    public string varName{get; set;}

    public List<int> initVals{get; set;}
    public override string ToString()
    {
            if(initVals==null){ //pour les operation unaire
                initVals=new List<int>();

            }
        return $"size : {size} varname: {varName}, init vals: {string.Join(",",initVals)}";
    }
}



using System.Collections.Generic;
using System;
public class Lexer { //TO ADD STRING OF CHARACTERS
    private static readonly Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType> { // a revioner
        { "mov", TokenType.InstKeyword },//2
        { "add", TokenType.InstKeyword },//2
        { "sub", TokenType.InstKeyword},//2
        { "xor", TokenType.InstKeyword },//2
        { "and", TokenType.InstKeyword },//2
        { "not", TokenType.InstKeyword },//1
        { "cmp", TokenType.InstKeyword },//2
        { "inc", TokenType.InstKeyword },//1
        { "dec", TokenType.InstKeyword },//1
        { "jmp", TokenType.InstKeyword },//1
        { "lea", TokenType.InstKeyword },//1*
        { "push", TokenType.InstKeyword },//1
        { "pop", TokenType.InstKeyword },//1
        { "jz", TokenType.InstKeyword },//1
        { "or", TokenType.InstKeyword },//2
        { "div", TokenType.InstKeyword },//2
        { "mul", TokenType.InstKeyword },//2
        { "segment", TokenType.segmentKw },
        { "ends", TokenType.endsKw },
        { "assume", TokenType.assumeKW },
         { "end", TokenType.endKw },
          { "dup", TokenType.dupKW },
         { "dw", TokenType.dataDef },
          { "db", TokenType.dataDef },
          { "dd", TokenType.dataDef },
          {"call",TokenType.InstKeyword},
          {"PROC",TokenType.Proc},
          {"ret",TokenType.ret},
          {"ENDP",TokenType.ENDP},
      
        { ",", TokenType.separator },
    };
    private static readonly Dictionary<string, TokenType> registres = new Dictionary<string, TokenType> { // a revisioner
        { "ah", TokenType.Registre },
        { "al", TokenType.Registre },
        { "bh", TokenType.Registre },
        { "bl", TokenType.Registre },
        { "ch", TokenType.Registre },
        { "cl", TokenType.Registre },
        { "dh", TokenType.Registre },
        { "dl", TokenType.Registre },
        { "ax", TokenType.Registre },
        { "bx", TokenType.Registre },
        { "cx", TokenType.Registre },
        { "dx", TokenType.Registre },
        { "ds", TokenType.Registre },
        { "cs", TokenType.Registre },
    };

    private readonly string input;
    private int position;

    public Lexer(string input) {
        this.input = input;
        this.position = 0;
    }

    public Token NextToken() {
        if (position >= input.Length) {
            return new Token(TokenType.EOF, "");
        }

        char currentChar = input[position];

        if (currentChar == ' ' || currentChar == '\t') {
            SkipWhitespace();
            return NextToken();
        }
        if(currentChar=='\n'){
            position++;
            return new Token(TokenType.newline,"\n");
        }

        if(currentChar=='['){
            position++;
            return new Token(TokenType.openSqrBracket,"[");
        }
        if(currentChar=='-'){
            position++;
            return ReadNumber(true);
        }
         if(currentChar==']'){
            position++;
            return new Token(TokenType.closeSqrBracket,"]");
        }

         if(currentChar=='"'){
            position++;
            return new Token(TokenType.quote,"\"");
        }
         if(currentChar=='\''){
            position++;
            return new Token(TokenType.quote,"\'");
        }
         if(currentChar==':'){
            position++;
            return new Token(TokenType.twopoints,":");
        }
         if (currentChar == ';') {
            SkipComment();
            return NextToken();
        }
     if (currentChar == ',') {
        position++;
        return new Token(TokenType.separator, ",");
    }
       
        if (char.IsDigit(currentChar)) {
            return ReadNumber(false);
        }

        if (char.IsLetter(currentChar)) {
            return ReadIdentifier();
        }

        throw new SyntaxException($"Invalid character: {currentChar}");
    }

    private void SkipWhitespace() {
        while (position < input.Length && (input[position] == ' ' || input[position] == '\t')) {
            position++;
        }
    }


       private void SkipComment() {
        while (position < input.Length && input[position] != '\n') {
            position++;
        }   
    }

    private Token ReadNumber(bool negative) {
        string neg=string.Empty;
        string number;
        if (negative){
            neg="-";
        }
        int start = position;
        if(char.IsDigit(input[start])==false){
            throw new System.Exception("expected number");
        }
        while (position < input.Length && (char.IsDigit(input[position]) || "ABCDEFabcdef".Contains(input[position])) ) {
            position++;
        }
        if(input[position].ToString().ToUpper()=="H"){//nombre hexadecimal
              number = input.Substring(start, position - start); 
              int decimalNumber = Convert.ToInt32(number, 16);
              position++;
              return new Token(TokenType.Number, neg+decimalNumber.ToString());
              
        }
        
        
         number = neg+input.Substring(start, position - start);
        return new Token(TokenType.Number, number);
    }

    private Token ReadIdentifier() {
        int start = position;
        while (position < input.Length && char.IsLetterOrDigit(input[position])) {
            position++;
        }
        string identifier = input.Substring(start, position - start);
        if (keywords.TryGetValue(identifier, out TokenType type)) {
            return new Token(type, identifier);
        }
        if(registres.TryGetValue(identifier, out TokenType typae)){
              return new Token(TokenType.Registre, identifier);
        }else{
             return new Token(TokenType.Identifier,identifier); }
        throw new SyntaxException("expected either register or adresse or number");
    }
    public List<Token> Tokenize(){     //eclater le code source en tokens
        List<Token> tokens = new List<Token>();
        Token token;
        do {
             token = this.NextToken();
             tokens.Add(token);
          } while (token.Type != TokenType.EOF);    

        return tokens;
    }
}

public enum TokenType {
    EOF,
    Identifier,
    Number ,
    NumberHEX,
    Mov,
    Add,
    Sub,
    Xor,
    And,
    dec,
    inc,
    loop,
    Not,
    cmp,
    Proc,
    ret,
    ENDP,
    jmp,
    separator,
    newline,
    openSqrBracket,
    closeSqrBracket,
    InstKeyword,
    Registre,
    quote,
    Singlequote,
    twopoints,
    str,
    segmentKw,
    endsKw,
    endKw,
    assumeKW,
    dupKW,
    dataDef,
    minus
}

public class Token {
    public readonly TokenType Type;
    public readonly string Value;

    public Token(TokenType type, string value) {
        this.Type = type;
        this.Value = value;
    }

    public override string ToString() {
        return $"Token({Type}, {Value})";
    }
}

public class SyntaxException : System.Exception {
    public SyntaxException(string message) : base(message) {}
}

using System;
using UnityEngine;
using System.Collections.Generic;
public class Addressing
{
    private Registres Registres;
    private Memory memory;
    private SymbolTable st;

     


    
    //pour la ram chaque fois on modifer une valeur on ajoute un key pair value sur dict(adr,pile<int>) si exist pas faire add


       
    public Addressing(Registres Registres, Memory memory)
    {
        this.Registres = Registres;
        this.memory = memory;

       
    }
    
     public void setSymbolTable(SymbolTable st){
             this.st=st;
        }

    public int Get(Operand op)
    {
        if (op == null)
        {
            return 0;
        }

       Dictionary<string, Register> regs; regs = this.Registres.regs;

        switch (op.Type)
        {
            case OperandType.Immediate:
                return int.Parse(op.Value, System.Globalization.NumberStyles.AllowLeadingSign);

            case OperandType.Register:
                if (op.Value.Contains("L") || op.Value.Contains("H")|| op.Value.Contains("h")|| op.Value.Contains("l"))
                {
                    if (!"ABCDabcd".Contains(op.Value[0]))
                    {
                        throw new SyntaxException("Only AX,BX,CX,DX Registres can have 'L' or 'H' suffix");
                    }
                  
                    return (int)regs[op.Value[0].ToString()+'x'].Get(op.Value[1]);
                }
                return (int)regs[op.Value].Get();
            
            case OperandType.MemoryAdresse:
                return (int)this.memory.Get(regs["DS"].Get() + int.Parse(op.Value, System.Globalization.NumberStyles.AllowLeadingSign)).NumericalValue;

            case OperandType.variable:
                if(op.size==16){  //fetch 16bit value
                        int low=(int)this.memory.Get(regs["DS"].Get() +  st.GetSymbol(op.Value).Address).NumericalValue;
                        int high=(int)this.memory.Get(regs["DS"].Get() +  st.GetSymbol(op.Value).Address+1).NumericalValue;
                        int val=(high << 8) | low;
                        return (int)val;
                }
                return (int)this.memory.Get(regs["DS"].Get() + st.GetSymbol(op.Value).Address).NumericalValue;   //add test if get 8bit or 16bit

            //indirect not implemented yet
            default:
                throw new SyntaxException("Invalid addressing mode");
        }
    }

    public void Set(Operand op, int  value,Operand? source=null)
    {  //AJOUTER LE CAS OU LA DESTINATION EST UNE ADRESSE MEMOIRE ET LA SOURCE EST UN REGISTRE DE 16 BIT SI LADRESSE MEMOIRE EST ALIGNEE (L ADRESSE EST PAIR ) ALORS LE CONTENUE DU REGISTRE 16BIT VAS ETRE COPIEE DANS ADRESS ET ADRESS+1 
       //AJOOUTER LE CAS OU LA DESTINATION EST UN REGISTRE DE 16 BIT ET LA SOURCE EST SUR 8BIT  ALORS LES BITS FABILE DU REGISTRE VONT ETRE MIS A JOUR A LA VALEUR DE LA SOURCE ET LES BTS DU POID FORTS VONT ETRE MAJ EN BIT DE SIGNE 0 SI POSITIVE 1 SI NEGATIVE
        Dictionary<string, Register> regs; regs = this.Registres.regs;

        switch (op.Type)
        {
            case OperandType.Immediate:
                throw new SyntaxException("Can't set to immediate value");

            case OperandType.Register:
                if (op.Value.Contains("L") || op.Value.Contains("H")|| op.Value.Contains("h")|| op.Value.Contains("l"))
                {
                    if (!"ABCDabcd".Contains(op.Value[0]))
                    {
                        throw new SyntaxException("Only AX,BX,CX,DX Registres can have 'L' or 'H' suffix");
                    }
 
                    Emulateur.historiqueRegs[op.Value[0].ToString().ToUpper()+op.Value[1].ToString().ToUpper()].Push(new Info(regs[op.Value[0].ToString()+'X'].Get(op.Value[1]),AnimationController.instNumber-1)); //sauvegarder la valeur precedente
                    regs[op.Value[0].ToString()+'X'].Set(value, op.Value[1]);
                }
                else
                {
                    //registre 16 bit destination ,source adresse memoire brute
                    if(source!=null && source.Type==OperandType.MemoryAdresse && op.Value.ToUpper().Contains("X")){
                        Operand newop=new Operand();
                        newop.size=source.size;
                        newop.Type=source.Type;
                        newop.Value=(int.Parse(source.Value)+1).ToString();
                        int nextvalue=Get(newop);
                        Emulateur.historiqueRegs[op.Value.ToUpper()].Push(new Info(regs[op.Value].Get('l'),AnimationController.instNumber-1));  //sauvegrader la valeur precedente
                        regs[op.Value].Set(value,'l');
                        Emulateur.historiqueRegs[op.Value.ToUpper()].Push(new Info(regs[op.Value].Get('h'),AnimationController.instNumber-1));  //sauvegrader la valeur precedente
                        regs[op.Value].Set(nextvalue,'h');

                    }else{

                        
                        Emulateur.historiqueRegs[op.Value.ToUpper()].Push(new Info(regs[op.Value].Get(),AnimationController.instNumber-1));  //sauvegrader la valeur precedente
                        regs[op.Value].Set(value);
                    }
                }
                break;

            case OperandType.MemoryAdresse:    //tester si valeur est sur 16 bit ,donc ecrire dans adr et adr+1
                
                int adrs=(regs["DS"].Get() + int.Parse(op.Value, System.Globalization.NumberStyles.AllowLeadingSign));
                int adrLog= int.Parse(op.Value, System.Globalization.NumberStyles.AllowLeadingSign);
                if (source!=null &&  source.size==16 ){

                    Emulateur.historiqueRam.TryAdd(adrLog,new Stack<Info>());//sauvegarder lhistorique de cette adresse memoire
                    Emulateur.historiqueRam[adrLog].Push(new Info((int)memory.Get(adrs).NumericalValue,AnimationController.instNumber-1));
                    this.memory.Set(adrs, value & 0x00FF);  //set low
                    Emulateur.historiqueRam.TryAdd(adrLog+1,new Stack<Info>());//sauvegarder lhistorique de cette adresse memoire
                    Emulateur.historiqueRam[adrLog+1].Push(new Info((int)memory.Get(adrs+1).NumericalValue,AnimationController.instNumber-1));
                    this.memory.Set(adrs + 1, value >> 8);   //set high
                    
                }
                else{  //si byte
                     Emulateur.historiqueRam.TryAdd(adrLog,new Stack<Info>());//sauvegarder lhistorique de cette adresse memoire
                        Debug.Log(Emulateur.historiqueRam[adrLog].Count+"COUNT PILE");
                        Debug.Log(adrs+"LOOK AT ME"+" PUSHING VALUE :"+(int)memory.Get(adrs).NumericalValue+"IN STACK WITH KEY : "+adrLog);
                        Debug.Log((int)memory.Get(adrs).NumericalValue+"meme adrs");
                     Emulateur.historiqueRam[adrLog].Push(new Info((int)memory.Get(adrs).NumericalValue,AnimationController.instNumber-1));
                    
                     this.memory.Set(adrs, value);
                    
                     

                    }


                break;
            case OperandType.variable:
             int adr=regs["DS"].Get() + st.GetSymbol(op.Value).Address;
              int adrLog2= st.GetSymbol(op.Value).Address;
                if(source!=null && source.size==16){
                      
                     

        
                      Emulateur.historiqueRam.TryAdd(adrLog2,new Stack<Info>());//sauvegarder lhistorique de cette adresse memoire
                      Emulateur.historiqueRam[adrLog2].Push(new Info((int)memory.Get(adr).NumericalValue,AnimationController.instNumber-1));
                      this.memory.Set(adr, value & 0x00FF);  //set low
                      Emulateur.historiqueRam.TryAdd((adrLog2+1),new Stack<Info>());//sauvegarder lhistorique de cette adresse memoire
                      Emulateur.historiqueRam[adrLog2+1].Push(new Info((int)memory.Get(adr+1).NumericalValue,AnimationController.instNumber-1));
                      this.memory.Set(adr+1, value >> 8);   //set high
                    


                }else{
                     Emulateur.historiqueRam.TryAdd(adrLog2,new Stack<Info>());//sauvegarder lhistorique de cette adresse memoire
                     Emulateur.historiqueRam[adrLog2].Push(new Info((int)memory.Get(adr).NumericalValue,AnimationController.instNumber-1));
                    this.memory.Set(adr,value);
                }
                 
                break;
           

            default:
                throw new SyntaxException("Invalid addressing mode");
        }
    }
}

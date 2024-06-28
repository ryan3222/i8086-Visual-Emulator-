using System;

public class Memory
{
    const int MEM_SIZE = 5000; //for testing purposes  size is small
    private MemByte[] mem;//later change to membyte

    public Memory()
    {
        mem = new MemByte[MEM_SIZE];
        InitializeMem();
    }

    private void InitializeMem()
    {
        for (int i = 0; i < MEM_SIZE; i++)
        {
            mem[i] = new MemByte(0,i);
        }
    }

    public void Set(int addr, int val)
    {
        if (val > Math.Pow(2, 16))
        {
            throw new Exception("Can't set greater than 16 bit value in memory location");
        }
        mem[addr] = new MemByte((ushort)val,addr); //set lower half and upper half later  (memory is an array of bytes)
    }
      public void Set(int addr, Instruction inst)
    {
       for(int i=0;i<inst.Length; i++)
         mem[addr+i] = new MemByte(inst,addr+i,i);                 //decouper linstruction plus tard 

    }              
    public MemByte Get(int addr)
    {
        return mem[addr];
    }
    public MemByte[] GetMemBytes(){
        return mem;
    }
    

    }


    public class MemByte 
    {   //smallest storable unit in ram
            public ushort? NumericalValue { get; set; }
            public Instruction Instruction { get; set; }
            
            public string instrcuctionByte;
            public int address{get;set;}
            int index;
            public MemByte(ushort value,int address) {
               NumericalValue = value;
               this.address=address;
            }
    
            public MemByte(Instruction instruction,int address,int index) {
             Instruction = instruction;
             this.address=address;
             instrcuctionByte=instruction.getByteMC(index);
             this.index=index;
          }
    
          public bool IsNumericalValue() {
             return NumericalValue.HasValue;
         }
    
           public bool IsInstruction() {
                 return Instruction != null;
          }
          public object get(){
            return IsInstruction() ? (object)Instruction : (object) NumericalValue; 
          }

          public override string ToString(){
            string ib="";      
            if(Instruction!=null)
              ib=Instruction.getOriginalString()+" "+(index+1)+"/"+Instruction.Length+'-'+"("+instrcuctionByte+")";

            string s=IsInstruction() ?ib  :NumericalValue.ToString() ;

            string returnString=""+s+"\n";
             return  returnString;
          }

          
        }

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


[DefaultExecutionOrder(-1)]
public class Emulateur : MonoBehaviour
{
    // Start is called before the first frame update
    public static CPU cpu_core;
    public static int instNum=0;

       //historique des changements
    //dictionary (reg,pile<int> ) pour chaque registre son historique
    public static Dictionary<string,Stack<Info>> historiqueRegs=new Dictionary<string, Stack<Info>>();


    
    //pour la ram chaque fois on modifer une valeur on ajoute un key pair value sur dict(adr,pile<int>) si exist pas faire add
    public static  Dictionary<int,Stack<Info>> historiqueRam=new Dictionary<int, Stack<Info>>();


    public static Stack<int> historiqueFIvert=new Stack<int>();
    public static Stack<int> historiqueFIbleu=new Stack<int>();
    
       public TMP_InputField textMeshPro;
  
    
    public  SequenceGenerator sq;
    void Start()
    {
          
         historiqueRegs=new Dictionary<string, Stack<Info>>();
         historiqueRam=new Dictionary<int, Stack<Info>>();
         initHistoriqueReg();
          cpu_core=new CPU();
          cpu_core.seq=sq;
      //    HighlightLine(0);
          
         


    }

    public static void reload(){
        Debug.Log("RELOAD");
        GameObject.Find("Emulate").GetComponent<Emulate>().OnButtonClick();
    }
      private void HighlightLine(int index)
    {
        Color highlightColor=Color.yellow;
        TMP_Text textComponent = textMeshPro.textComponent;
        TMP_TextInfo textInfo = textComponent.textInfo;
        Debug.Log(textInfo.lineCount);

        if (index >= 0 && index < textInfo.lineCount)
        {
            TMP_LineInfo lineInfo = textInfo.lineInfo[index];

            for (int i = lineInfo.firstCharacterIndex; i <= lineInfo.lastCharacterIndex; i++)
            {
                int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
                int vertexIndex = textInfo.characterInfo[i].vertexIndex;

                Color32[] vertexColors = textInfo.meshInfo[materialIndex].colors32;
                vertexColors[vertexIndex + 0] = highlightColor;
                vertexColors[vertexIndex + 1] = highlightColor;
                vertexColors[vertexIndex + 2] = highlightColor;
                vertexColors[vertexIndex + 3] = highlightColor;
            }

            textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
        }
        else
        {
            Debug.LogError("Invalid line index!");
        }
    }
    public static void initHistoriqueReg(){
        historiqueRegs.Add("AX",new Stack<Info>());
        historiqueRegs.Add("AL",new Stack<Info>());
        historiqueRegs.Add("AH",new Stack<Info>());
        historiqueRegs.Add("BX",new Stack<Info>());
        historiqueRegs.Add("BL",new Stack<Info>());
        historiqueRegs.Add("BH",new Stack<Info>());
        historiqueRegs.Add("CX",new Stack<Info>());
        historiqueRegs.Add("CL",new Stack<Info>());
        historiqueRegs.Add("CH",new Stack<Info>());
        historiqueRegs.Add("DL",new Stack<Info>());
        historiqueRegs.Add("SP",new Stack<Info>());
        historiqueRegs.Add("BP",new Stack<Info>());
        historiqueRegs.Add("DX",new Stack<Info>());
        historiqueRegs.Add("DH",new Stack<Info>());
        historiqueRegs.Add("CS",new Stack<Info>());
        historiqueRegs.Add("DS",new Stack<Info>());
        historiqueRegs.Add("SS",new Stack<Info>());
        historiqueRegs.Add("ES",new Stack<Info>());
        historiqueRegs.Add("IP",new Stack<Info>());
        historiqueRegs.Add("PS",new Stack<Info>());
        historiqueRegs["PS"].Push(new Info(0,0));
     

    }

    public static void loadData(List<DataDefinitons> ld,SymbolTable st)
    {
      
        cpu_core.setSymbolTable(st);
        cpu_core.LoadDataDefintions(ld);
    }

    public static void loadInstruction(List<Instruction> ld)
    {
             cpu_core.LoadCode(ld);
    }

    public static void ExecuteStep(){
          cpu_core.Step();

          instNum++;
    }

  
    
   
    
   
   
}
  public class Info{
        public int value;
        public int instNum;

        public int Type=0;

        public Info(int value,int instNum){
            this.value=value;
            this.instNum=instNum;
           
        }


    }


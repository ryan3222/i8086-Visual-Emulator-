using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;
using System.IO;
using UnityEngine.EventSystems;
using SFB;


public class Emulate : MonoBehaviour
{

    public TMP_InputField myInputField;
      public  CPU cpu_core;

      public AnimationController anp;
     public AnimationController an;
      public SequenceGenerator sq;
    public void OnButtonClick()
    {
       Debug.Log("LOADING.....");
       string code= myInputField.text;
        code = Regex.Replace(code, @"<.*?>", string.Empty);
       Lexer lex=new Lexer(code);
        List<Token> tokens=lex.Tokenize();
        Parser parser = new Parser(tokens);
        Debug.Log("Instructions");
        List<Instruction> list=parser.ParseInstructions();
        foreach (Instruction instruction in list) {
            Debug.Log(instruction.ToString());
        }
        Debug.Log("Variables");
        List<DataDefinitons> listdat=parser.ParseDataDefinitions();
         foreach (DataDefinitons data in listdat) {
            Debug.Log(data.ToString());
        }
        Emulateur.loadData(listdat,parser.getST());
      

        Emulateur.loadInstruction(list);
        Step.visitedLines.Clear();
        myInputField.text= highlight.RemoveHighlight(myInputField.text);
       codeMachine.list=list;
     
        an.reload();
        anp.reload();
        


         /*sq.fetchFirstInstruction(Emulateur.cpu_core.memory,0,0);
         sq.fetchNextInstruction(Emulateur.cpu_core.memory,0,Emulateur.cpu_core.registers.regs["ip"].Get()+1);*/
         
    
    }
}

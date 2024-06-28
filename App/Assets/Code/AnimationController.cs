using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Text.RegularExpressions;
public class AnimationController : MonoBehaviour
{
    public SequenceGenerator sq;
    public GameObject[] bus;
    public bool isSequenceRuning=false;  
    bool CanAnimate=true;
    
    public  bool toggleCodeMachine;
    bool CanAnimatePar=true;
    private ManualResetEventSlim _event = new ManualResetEventSlim(false);

    private object _lockObject = new object();

    public AnimationController anp;

    public GameObject[] FilInstruction;
    fill fillScript;
    public List<string> CurrentSequence;
     List<string> CurrentSequencePar;
    string animation;
    int i=0;
    public Text[] RamSlots;

    public string[] SecondRamSlots=new string[50];
    public Text[] RamAdrs;
    public Text ah;
    public Text al;
    public Text bh;
    public Text bl;
    public Text ch;
    public Text cl;
    public Text dh;
    public Text dl;
    public Text si;
    public Text di;
    public Text bp;
    public Text sp;
    public Text cs;
    public Text ds;
    public Text ss;
    public Text es;
   
    public Text ip;

    public Text ir;
    public Text psw;
    public Text rt;
    static List<Color> stateFilIntructon=new List<Color>();

    public bool fetchFirstInstruction=false;
     List<string> currentReplayableSeqeunce=new List<string>();

     public List<string> firstFetchSequence=new List<string>();

    static int sequenceID=0;

    List<List<string>> sequenceHistory=new List<List<string>>();
    int[] localRamHistroy;
    

   public static int instNumber=0;

    
    Dictionary<string, Text> values;

    Dictionary<string, int> PreviousAffectedValues;
    bool isUnitAnimating=false;
     Queue<List<string>> queue=new Queue<List<string>>();
    List<Renderer> selectedUnits=new List<Renderer>();
     List<Renderer> permenantSelectedUnits=new List<Renderer>(); //units that only reset if reset by code , or reloaded the whole eumalation
    Dictionary<Text,string> modifiedValues=new Dictionary<Text, string>();  //string is the old value to get back to
    public bool  lireToggle=false;
    
     CPU CPU=Emulateur.cpu_core;

    Transform t;
    void Start()
    {
       init();
       GetRam(Emulateur.cpu_core.memory,10,0);
        
       
      
      List<string> l=new List<string>(){"0,0","4,0","5,0","3,1","ax55","bh","wr","adle","4,0","5,0","ax55","bh","-wr","dx0","3,0","5,0"};
        //activerSequence(l);
        /*List<string> seq=new List<string>();
        seq.Add("01");
        seq.Add("10");
        activerSequence(seq);
         seq.Add("00");
        seq.Add("10");
         seq.Add("21");
        activerSequence(seq);
        SetRegistre("ax",55);*/
 

     /* if(gameObject.name!="animationControllerParalel"){
        activerSequence(l);
      }*/
       
      
     
    }

    void init(){
      CurrentSequence=new List<string>();
        values= new Dictionary<string, Text>();
        values.Add("ah",ah);
        values.Add("al",al);
        values.Add("bh",bh);
        values.Add("bl",bl);
        values.Add("ch",ch);
        values.Add("cl",cl);
        values.Add("dh",dh);
        values.Add("dl",dl);
        values.Add("bp",bp);
        values.Add("sp",sp);
        values.Add("cs",cs);
        values.Add("ds",ds);
        values.Add("ss",ss);
        values.Add("es",es);
        values.Add("ip",ip);
        values.Add("ir",ir);
        values.Add("ps",psw);
        values.Add("rt",rt);
        PreviousAffectedValues=new Dictionary<string, int>();
        SecondRamSlots=new string[50];
    }

    // Update is called once per frame
    void Update()
    {
          runSequence();      
    }


    public int statusFilInstruction(){
      int i=0;
      foreach (GameObject item in FilInstruction)
      {
        if(item.GetComponent<Renderer>().material.color==Color.white){
          i++;
        }
      }

      return i;
    }


    public int statusAwaitingInstructions(){
      int i=0;
      foreach (GameObject item in FilInstruction)
      {
        if(item.GetComponent<Renderer>().material.color==Color.cyan){
          i++;
        }
      }

      return i;
    }

    public void clearQueue(){
      queue.Clear();
    }


    void runSequence()
    {

        if (isSequenceRuning){
              animation =CurrentSequence[i];
         if(char.IsDigit(animation[0]))  //bus animations
            {

                runBusAnimations();
         }else  
         if(char.IsLetter(animation[0]) || animation[0]=='/' ){//other animations
            List<string> regs = new List<string> { "ax", "bx", "cx","dx","al","ah","bh","bl","ch","cl","dh","dl","es","ds","ss","cs","ip","ir","ps","rt","sp"} ;
            if(regs.Contains(animation.Substring(0,2).ToLower())){  //reg animation 
                    runRegAnimation();    
            }else  if((animation.Length>5 && animation.Substring(0,3)=="ram" )||(animation.Contains('-') && animation.Substring(0,3)=="ram") ){// ram animations
                    ramAnimations();
             }else if(CanAnimate){  //selectioner une autre unité apart un registre
                    otherAnimations();
            }
             if(!isUnitAnimating)
               {//run the next animation
                    if (lireToggle)
                        i++;
                       CanAnimate=true;
               }
           }else if(animation[0]=='-'){ //deselectionner une unité
                runDeselectUnit();
           }
           endOfSequence();
       }
    }

    
    void runDeselectUnit(){
               if(animation=="-ALL"){  //deselect all units
                 deselectAllUnits();

               }else{
              deselectUnit(animation.Substring(1));
              }  
              CanAnimate=true;     
              if(lireToggle) 
                i++; 
    }
    void ramAnimations(){
       if(CanAnimate){

                if(animation.Contains('-')){
                       Debug.Log(animation.Substring(3, animation.Length - 4)+"RAM");
                     RamSlots[int.Parse(animation.Substring(3, animation.Length - 4))].text="-----";
                   
                }else if(animation.Contains(':')){
                    string segment=animation.Split(':')[2];
                    string adr =animation.Split(':')[3];
                    if(!getCurrentRamAdresses().Contains((int.Parse(segment)<<4) + int.Parse(adr))){
                          GetRam(Emulateur.cpu_core.memory,int.Parse(segment),int.Parse(adr),int.Parse(animation.Split(':')[1]));
                    }


                } 
                else{


                   string[] substrings = animation.Split(',');
                  Debug.Log("ANIMATION " + animation);
                   SetRam(substrings[0],substrings[1]);
                  

                }
                CanAnimate=false;
                }
    }
    void otherAnimations(){
       if(animation[0]=='/'){   //selection une unité pour un certain temmp
              if(animation.Contains(',')){
                       switch (animation.Split(',')[1])
                       {
                        case "b": selectUnitForTime(animation.Split(',')[0].ToUpper(),Color.cyan);
                        break;
                        
                        default:
                        break;
                       }
                       
              }else{
                      Debug.Log(animation.ToUpper().Substring(1));
                      selectUnitForTime(animation.ToUpper().Substring(1));       
                  }
                  Debug.Log(animation.ToUpper().Substring(1));

                 CanAnimate=false;     
                
       }else{
              if(animation.Contains(',')){
                   if(animation.Split(',')[0]=="GetRam"){  //la fonction get ram
                       GetRam(Emulateur.cpu_core.memory,int.Parse(animation.Split(',')[1]),int.Parse(animation.Split(',')[2]));
                    
                       CanAnimate=false;
 
                   }else{
                       switch (animation.Split(',')[1])
                       {
                        case "b": selectUnitForTime3(animation.Split(',')[0].ToUpper(),Color.cyan);
                        break;
                        
                        default:
                        break;
                       }
                        CanAnimate=false; 
                   }

               }else{
                      selectUnitForTime3(animation.ToUpper());
                       CanAnimate=false;        
                  }
              }
         
    }
    bool  accessValide(){
      return(!(int.Parse(animation.Split(',')[0])==29) || isAceessValide() ); //verfiyer si les buses externes sont pas occupé sinon attendre

    }

    bool isAceessValide(){
      return GameObject.Find("RDY").GetComponent<Renderer>().material.color!=Color.green;
    }
    void runBusAnimations(){

      if(accessValide()){
            if(CanAnimate){
                    
                  
                  if(animation.Split(',')[1][0]=='0'){
                  
                      bus[int.Parse(animation.Split(',')[0])].GetComponent<bus>().animate(0);
                      CanAnimate=false;

                          }
                  else if(animation.Split(',')[1][0]=='1'){
                      bus[int.Parse(animation.Split(',')[0])].GetComponent<bus>().animate(1);
                      CanAnimate=false;
              
                    }    
                }
                  if(!bus[int.Parse(animation.Split(',')[0])].GetComponent<bus>().isAnimated){//run the next animation

                      if(lireToggle)
                        i++;
                      CanAnimate=true;
                  }

      }
    }
    void runRegAnimation(){
        if(CanAnimate && animation.Length>2){  //set reg value
                  if(animation.Contains(",")){  //set le IR
                        SetIR(animation.Split(',')[0].ToLower(),animation.Split(',')[1].ToLower());  
                       CanAnimate=false;
                  }else{
                        if(animation[2]=='.'){  
                            SetRegistre2(animation.Substring(0,2).ToLower(),int.Parse(animation.Substring(3)));
//                            Debug.Log(animation.Substring(1,3));
                            CanAnimate=false;
                           
                       }else{
                            if(animation.Contains(';')){ //select reg without chaning value
                             if(animation[1].ToString().ToLower()=="x"){
                                selectUnitForTime(animation.Substring(0,1)+"h");
                                selectUnitForTime(animation.Substring(0,1)+"l");
                             }else
                                   selectUnitForTime(animation.Substring(0,2));
                            }else{
                                  Debug.Log(animation.Substring(0,2).ToLower()+int.Parse(animation.Substring(2)));
                                  SetRegistre(animation.Substring(0,2).ToLower(),int.Parse(animation.Substring(2)));
                            }


                          CanAnimate=false;
                          
                          }}
                   
             }else if(CanAnimate){ //select with setting current value
              SetRegistre(animation.Substring(0,2).ToLower(),Emulateur.cpu_core.GetRegValue(animation.Substring(0,2).ToLower()));
               CanAnimate=false;
             }
            //end of reg animation
    }

    void endOfSequence(){
       if(i==CurrentSequence.Count){//end of sequence
              i=0;
              isSequenceRuning=false;    
              //sauvegarder lhistorique des sequences
              List<string> s=new List<string>();
              s.AddRange(CurrentSequence);
              sequenceHistory.Add(s);
              //
              Debug.Log("le i est "+ i);
              currentReplayableSeqeunce.Clear();
            
              currentReplayableSeqeunce.AddRange(CurrentSequence);           
              CurrentSequence.Clear();   //on appel la prochaine sequence si y'en a 
              if(queue.Count>0){    
                 List<string> l=queue.Dequeue();
                  activerSequence(l);       
                  Debug.Log("next" + l.Count);  
                }

              if(fetchFirstInstruction){  //lancner le fetch deuxieme
                  
//                    Debug.Log("azeazsqdq");
                   sq.fetchNextInstruction(Emulateur.cpu_core.memory,Emulateur.cpu_core.GetRegValue("CS"),Emulateur.cpu_core.GetRegValue("IP"),true);
                  
                   fetchFirstInstruction=false;
              }
               
           //    Emulateur.historiqueFIvert.TryPop(out int nbrVerts);
      
           //   Emulateur.historiqueFIbleu.TryPop(out int nbrBleu);

             }
    }
   public  void activerSequence(List<string> list){ //first char is number of bus , second is direction
    if(!isSequenceRuning){
      //save currect fil instrcuction state to rewind later
      for (int i = 0; i < FilInstruction.Length; i++)
      {
        stateFilIntructon.Add(FilInstruction[i].GetComponent<Renderer>().material.color);
      }


//       Debug.Log("will execute "+list[0]);      
       CurrentSequence.AddRange(list);
        isSequenceRuning=true;
        return;
       }else{ //ajouter l'appel au fil d'attente
           queue.Enqueue(list);
//            Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA"+list[0]);   
           
       } 

    }

    void rewindFilInstruction(){
      for (int i = 0; i < FilInstruction.Length; i++)
      {
        if(FilInstruction[i].GetComponent<Renderer>().material.color!=Color.green)
            FilInstruction[i].GetComponent<Renderer>().material.color=stateFilIntructon[i];
        Debug.Log("the cureent color is : "+stateFilIntructon[i].ToString());
      }

    }

    public  void updateRamVals(){
    try
    {
       for(int i=0;i<RamSlots.Length;i++ ){
        if(i<=SecondRamSlots.Length )
          RamSlots[i].text=(toggleCodeMachine?SecondRamSlots[i].Split('-')[1]:SecondRamSlots[i].Split('-')[0]);
      }
    }
    catch (System.Exception)
    {
      
      
    }  
     
    }

    public void GetRam(Memory mem,int segment,int adr){
     
       int start;
       if(adr % 2 ==0){
        start=0;
       }else{
        RamAdrs[0].text=segment+":"+(adr-1);
        string val=mem.Get(segment+adr-1).ToString();
        SecondRamSlots[0]=val; //sauvegarder code machine + nom instruction
        if(val.Contains('-')){//cest une instructiion
            if(toggleCodeMachine)
              val=val.Split('-')[1];//afficher code machine seulement 
            else 
              val=val.Split('-')[0];
        }else{//val est une valeur numerique
            val=VisualFunctions.IntToHex(int.Parse(val));
        }
        RamSlots[0].text=val;
        start=1;
       }
       for(int i=start;i<RamAdrs.Length;i++){
        RamAdrs[i].text=segment + ":"+ (adr+i-start);
      } 
      string val2;
      for(int i=start;i<RamSlots.Length;i++){
        val2=mem.Get(segment+adr+i-start).ToString();
        Debug.Log("VAL2 "+val2+" I "+i);
        SecondRamSlots[i]=val2;
        
        if(val2.Contains('-')){//cest une instructiion
            if(toggleCodeMachine)
              val2=val2.Split('-')[1];//afficher code machine seulement 
            else 
              val2=val2.Split('-')[0];
        }else{//val est une valeur numerique
            val2=VisualFunctions.IntToHexByte(int.Parse(val2));
        }
        RamSlots[i].text=val2;
      }



    }
     public void GetRam(Memory mem,int segment,int adr,int decalage){
     
       int start=decalage;
      string val;
        for(int i=1;i<=decalage;i++){
            RamAdrs[decalage-i].text=segment+":"+(adr-i);
            val=mem.Get(segment+adr-i).ToString();
            RamSlots[decalage-i].text=val;
        
       }
       for(int i=start;i<RamAdrs.Length;i++){
        RamAdrs[i].text=segment + ":"+ (adr+i-start);
      } 
      string val2;
      for(int i=start;i<RamSlots.Length;i++){
        val2=mem.Get(segment+adr+i-start).ToString();
        RamSlots[i].text=val2;
      }

    }
    public void SetRegistre(string reg,int value)
    {
      
      if(reg[1].ToString().ToUpper()=="X"){
        SetRegistre(reg[0]+"h",value >> 8);
        SetRegistre(reg[0]+"l",value & 255);
      }else{
        string tp=values[reg.ToLower()].text.Replace("h", "");
        tp=tp.Replace("H","");
       
        int precval=int.Parse( tp,System.Globalization.NumberStyles.HexNumber); 
//         Debug.Log("PREC OF "+reg+" IS "+precval);  
       
           PreviousAffectedValues.TryAdd(reg,precval);
        
       
        if(reg[1].ToString().ToLower()=='l'.ToString() || reg[1].ToString().ToLower()=='h'.ToString())
           values[reg.ToLower()].text=VisualFunctions.IntToHexByte(value);
        else
           values[reg.ToLower()].text=VisualFunctions.IntToHex(value);
        selectUnitForTime(reg.ToUpper());

        
      }

    }
   public static void AddRange<TKey, TValue>(Dictionary<TKey, TValue> dict, IEnumerable<KeyValuePair<TKey, TValue>> rangeToAdd)
    {
        foreach (KeyValuePair<TKey, TValue> kvp in rangeToAdd)
        {
            dict[kvp.Key] = kvp.Value;
        }
    }

    public void rewindRegValues(){

      Dictionary<string,int> d=new Dictionary<string, int>();
      AddRange(d,PreviousAffectedValues);
      
      foreach (KeyValuePair<string, int> x in d)
      {

        Debug.Log("RESETING "+x.Key+" WITTH "+x.Value);
        SetRegistre(x.Key,x.Value);
      }

    }

    public void SetRegistre2(string reg,int value)  //sans bloque la sequence
    {
      if(reg[1].ToString().ToUpper()=="X"){
        SetRegistre(reg[0]+"h",value >> 8);
        SetRegistre(reg[0]+"l",value & 255);
      }else{
        string tp=values[reg].text.Replace("h", "");
        tp=tp.Replace("H","");  
        int precval=int.Parse( tp,System.Globalization.NumberStyles.HexNumber); 
//        Debug.Log("PREC OF "+reg+" IS "+precval);
        PreviousAffectedValues.TryAdd(reg,precval);

        if(reg[1].ToString().ToLower()=='l'.ToString() || reg[1].ToString().ToLower()=='h'.ToString())
           values[reg].text=VisualFunctions.IntToHexByte(value);
        else
           values[reg].text=VisualFunctions.IntToHex(value);
        selectUnitForTime2(reg.ToUpper());

        
      }

    }
    public void SetIR(string reg,string value)
    {
        values[reg].text=value.ToString();
        selectUnitForTime(reg.ToUpper());

    }
    public void SetRam(string ram,string value)
    {
       
        string numberString = ram.Substring(3);
        int number = int.Parse(numberString);
        RamSlots[number].text=VisualFunctions.IntToHexByte(int.Parse(value));
        selectUnitForTime(ram.ToUpper());
    }



    public void selectUnitForTime(string u){
      GameObject o=GameObject.Find(u.ToUpper());
       StartCoroutine(SetColorCoroutine(o,2));
       

 
    }

     public void selectUnitForTime(string u,Color c){
      GameObject o=GameObject.Find(u.ToUpper());
       StartCoroutine(SetColor3(o,2,c));
      if(!u.Contains("BYTE") && u[1]!='F')
         selectedUnits.Add(o.GetComponent<Renderer>());
      else
         permenantSelectedUnits.Add(o.GetComponent<Renderer>());
         
    }


  
    public  void deselectUnit(string u){
      GameObject o=GameObject.Find(u.ToUpper());
       o.GetComponent<Renderer>().material.color = Color.white;
        selectedUnits.Remove(o.GetComponent<Renderer>());
        permenantSelectedUnits.Remove(o.GetComponent<Renderer>());

      
    }
     public void selectUnitForTime2(string u){
      GameObject o=GameObject.Find(u.ToUpper());
       StartCoroutine(SetColor2(o,2));
       
    }
    public void selectUnitForTime3(string u){
      GameObject o=GameObject.Find(u.ToUpper());
       StartCoroutine(SetColor3(o,2));
       if(!u.Contains("BYTE")&& u[1]!='F')
         selectedUnits.Add(o.GetComponent<Renderer>());
        else
         permenantSelectedUnits.Add(o.GetComponent<Renderer>());
    }
     public void selectUnitForTime3(string u,Color c){
      GameObject o=GameObject.Find(u.ToUpper());
       StartCoroutine(SetColor3(o,2,c));
      if(!u.Contains("BYTE")&& u[1]!='F')
         selectedUnits.Add(o.GetComponent<Renderer>());
      else
         permenantSelectedUnits.Add(o.GetComponent<Renderer>());
    }

    IEnumerator SetColorCoroutine(GameObject o,float time)
    {
     
//        Debug.Log("select");
         o.GetComponent<Renderer>().material.color = Color.red;
        isUnitAnimating = true;
    // Wait for the specified amount of time
    yield return new WaitForSeconds(time);

    // Return object to original color
    o.GetComponent<Renderer>().material.color = Color.white;

    // Set boolean value to true
    isUnitAnimating = false;
        
         
    }

    public List<int> getCurrentRamAdresses(){
      List<int> l=new List<int>();
      foreach (Text item in RamAdrs)
      { 
        l.Add((int.Parse(item.text.Split(':')[0])<<4)+int.Parse(item.text.Split(':')[1]));
 
      }
      return l;
    }

    IEnumerator SetColor2(GameObject o,float time)   //selectionner et passer vers prochaine animation sans terminer lanimation , terminer l'animation apr
    {
     
//        Debug.Log("select");
         o.GetComponent<Renderer>().material.color = Color.red;
     isUnitAnimating = true;
    // Wait for the specified amount of time
    yield return new WaitForSeconds(0.001f*time);
      isUnitAnimating = false;
      yield return new WaitForSeconds(time-0.001f*time);

    // Return object to original color
    o.GetComponent<Renderer>().material.color = Color.white;

    // Set boolean value to true
 
        
         
    }
    IEnumerator SetColor3(GameObject o,float time)   //selectionner et passer vers prochaine animation sans terminer lanimation (l'unité reste selectioné)
    {
     
//        Debug.Log("select");
         o.GetComponent<Renderer>().material.color = Color.green;
     isUnitAnimating = true;
    // Wait for the specified amount of time
    yield return new WaitForSeconds(0.001f*time);
      isUnitAnimating = false;
      yield return new WaitForSeconds(time-0.001f*time);

    // Return object to original color
    

    // Set boolean value to true
 
        
         
    }
    IEnumerator SetColor3(GameObject o,float time,Color c)   //selectionner et passer vers prochaine animation sans terminer lanimation (l'unité reste selectioné) avec une couleur specifie
    {
     
//        Debug.Log("select");
         o.GetComponent<Renderer>().material.color = c;
     isUnitAnimating = true;
    // Wait for the specified amount of time
    yield return new WaitForSeconds(0.001f*time);
      isUnitAnimating = false;
      yield return new WaitForSeconds(time-0.001f*time);

    // Return object to original color
    

    // Set boolean value to true
 
        
         
    }

    void deselectAllUnits(){  //add rewind for all previous values
     List<Renderer> l=new List<Renderer>();
     l.AddRange(selectedUnits);
     foreach (Renderer unit in l)
     {
        deselectUnit(unit.gameObject.name); 
        
     }
   }

    void deselectAllPermanentUnits(){  //add rewind for all previous values
      List<Renderer> l=new List<Renderer>();
      l.AddRange(permenantSelectedUnits);
      foreach (Renderer unit in l)
      {
         deselectUnit(unit.gameObject.name); 
        
      }
    }


   void setFlags(){
                       if(Emulateur.cpu_core.registers.flagRegister.GetFlag(Flags.Zero)==1){
                         selectUnitForTime3("ZF");
                       }else
                          deselectUnit("ZF");
                       if(Emulateur.cpu_core.registers.flagRegister.GetFlag(Flags.Auxilliary)==1){
                           selectUnitForTime3("AF");
                       }else
                          deselectUnit("AF");
                       if(Emulateur.cpu_core.registers.flagRegister.GetFlag(Flags.Parity)==1){
                           selectUnitForTime3("PF");
                       }else
                          deselectUnit("PF");
                       if(Emulateur.cpu_core.registers.flagRegister.GetFlag(Flags.Carry)==1){
                           selectUnitForTime3("CF");
                       }else
                          deselectUnit("CF");
                       if(Emulateur.cpu_core.registers.flagRegister.GetFlag(Flags.Directional)==1){
                           selectUnitForTime3("DF");
                       }else
                          deselectUnit("DF");
                       if(Emulateur.cpu_core.registers.flagRegister.GetFlag(Flags.Overflow)==1){
                         selectUnitForTime3("PF");
                       }else
                          deselectUnit("PF");
                       if(Emulateur.cpu_core.registers.flagRegister.GetFlag(Flags.Sign)==1){
                         selectUnitForTime3("SF");
                       }else
                          deselectUnit("SF");
                       if(Emulateur.cpu_core.registers.flagRegister.GetFlag(Flags.Trap)==1){
                         selectUnitForTime3("TF");
                       }else
                          deselectUnit("TF");
                       if(Emulateur.cpu_core.registers.flagRegister.GetFlag(Flags.Interrupt)==1){
                         selectUnitForTime3("IF");
                       }else
                          deselectUnit("IF");

   }
    public void stepBack(){

        int nbrVerts=0;
        int nbrBleu=0;

        Info ip=null;
         int IP;
        
        Emulateur.historiqueRegs["IP"].TryPeek(out ip);
        try
        {
          IP=ip.value;
        }
        catch (System.Exception)
        {
          IP=0;
         
        }
       
        Emulateur.cpu_core.registers.regs["IP"].Set(IP);
        Emulateur.historiqueFIvert.TryPop(out nbrVerts);
        if(StepBack.popTwice)
             Emulateur.historiqueFIvert.TryPop(out nbrVerts);
      
        Emulateur.historiqueFIbleu.TryPop(out nbrBleu);
         if(StepBack.popTwice)
             Emulateur.historiqueFIbleu.TryPop(out nbrBleu);

//        Debug.Log("nbrvert " +nbrVerts);
    //    Debug.Log("nbrbleu "+ nbrBleu);
        for (int i = 1; i <= 6; i++)
        {
         deselectUnit("BYTE"+i); 
        }
        
        for (int i = 1; i <= nbrBleu; i++)
        {
         selectUnitForTime3("BYTE"+i,Color.cyan); 
        }
         for (int i = 0; i < nbrVerts; i++)
        {
         selectUnitForTime3("BYTE"+(6-i)); 
        }
      int IPerrorFix;

        //CHANGER LES VALEURS PRECEDENTE JUST POUR LES REG/ MEM CASES QUI SONT CHANGé RECEMENT DONC FAUT AVOIR LA PILE QUI LE MAX DES VALEURS
       

       /* int maxReg = Emulateur.historiqueRegs.Values.Any() ? Emulateur.historiqueRegs.Values.Max(s => s.Count) : 0;
        int maxRam = Emulateur.historiqueRam.Values.Any() ? Emulateur.historiqueRam.Values.Max(s => s.Count) : 0;*/
        
       foreach (KeyValuePair<string, Stack<Info>> kvp in Emulateur.historiqueRegs)
        {
           /* if(StepBack.popTwice)   //pour depiler la valeur courante car on a besoir seulement de la valeur precendente
                kvp.Value.TryPop(out int r);*/


          if(kvp.Value.ToArray().Length >0 && kvp.Value.Peek().instNum==instNumber ){
            
              Debug.Log("RESETTING "+kvp.Key.ToLower()+" WITH "+kvp.Value.Peek());
              IPerrorFix=kvp.Key.ToLower()=="ip"?statusAwaitingInstructions():0;

              if(kvp.Key.ToUpper()[1]=='L' || kvp.Key.ToUpper()[1]=='H')  
                  Emulateur.cpu_core.registers.regs[kvp.Key.ToUpper()[0]+"X"].Set(kvp.Value.Peek().value,kvp.Key.ToUpper()[1]);
              else
                   if(kvp.Key=="PS") { //reseting the flags
                       Emulateur.cpu_core.registers.flagRegister.Set(kvp.Value.Peek().value);
                       setFlags();
                       
                   }
                   else
                      Emulateur.cpu_core.registers.regs[kvp.Key.ToUpper()].Set(kvp.Value.Peek().value);
                 
                   
                   SetRegistre2(kvp.Key.ToLower(),kvp.Value.Pop().value+IPerrorFix);
              

          }
          
        }
        foreach (KeyValuePair<int, Stack<Info>> kvp in Emulateur.historiqueRam)
        {
         //if changed recently

              
              if(kvp.Value.ToArray().Length >0 && kvp.Value.Peek().instNum==instNumber)  {//value has changed recetnly
                
                /* if(StepBack.popTwice)   //pour depiler la valeur courante car on a besoir seulement de la valeur precendente
                        kvp.Value.TryPop(out int r);*/


                 int segment=Emulateur.cpu_core.GetRegValue("DS");
                 if(kvp.Value.Peek().Type==1)
                    segment=Emulateur.cpu_core.GetRegValue("SS");
                Emulateur.cpu_core.memory.Set(segment+kvp.Key,kvp.Value.Pop().value);
             //   GetRam(Emulateur.cpu_core.memory,Emulateur.cpu_core.GetRegValue("DS"),kvp.Key);
               /* for (int i = 0; i < RamAdrs.Length; i++)
                {
                  if(RamAdrs[i].text.Split(':')[1]==kvp.Key.ToString())
                  
                  RamSlots[i].text=kvp.Value.Pop().ToString();

                  
                }*/
          
                }
    }
    
    }
    public  void Advance(){
      
      i++;
    }

    public  void Lire(){
       lireToggle=!lireToggle;
       
    }

    public void ReLire(){
      i=0;

      Debug.Log("CURRENT REPLAY  "+currentReplayableSeqeunce.Count);
     /* if(firstFetchSequence.Count!=0){
        Debug.Log("relre" + sequenceHistory.Count);
        activerSequence(sequenceHistory[0]);
        firstFetchSequence.Clear();
      }*/ 
        
      activerSequence(currentReplayableSeqeunce);
      rewindFilInstruction();
       deselectAllUnits();
       rewindRegValues();
        

    }
    


    public void clearPreviousVlaues(){
       PreviousAffectedValues.Clear();


    }

    public void resetALL(){

      foreach (Renderer item in permenantSelectedUnits)
      {
        Debug.Log("CC"+item.gameObject.name+"  "+item.material.color);
      }
      deselectAllPermanentUnits();
     
      deselectAllUnits();
    

       Emulateur.historiqueFIbleu.Clear();
       Emulateur.historiqueFIvert.Clear();
       Emulateur.historiqueRam.Clear();
       Emulateur.historiqueRegs.Clear();
       Emulateur.initHistoriqueReg();
      
    }
    void reintRegs(){
      foreach(KeyValuePair<string,Text> a in values){
      if(a.Key.ToLower().Contains('l')||a.Key.ToLower().Contains('h'))  {
         a.Value.text="00H";
      }
      else
      {
          a.Value.text="0000H";
      }
    }}
    public void reload(){
      resetALL();
      isSequenceRuning=false; 
      CurrentSequence.Clear();
      i=0;
     clearQueue();
     reintRegs();
    }
    
    


}



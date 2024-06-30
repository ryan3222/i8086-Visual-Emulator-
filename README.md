# i8086-Visual-Emulator-
a unity 2d interface that visualises in real time the internal state, data flow of the intel 8086 while executing i8086 assembly code in a virtual machine

the virtual machine consists of : 
CPU_core module: virtual representation of the execution unit in a form of a class , responsible for memroy allocation and instruction execution , emulates the behaviour of the ALU and BIU 
Memory module: virtual represenation of the RAM, contains data structures in form of USHORTS to hold the ram data, negative values are  stored the same way in i8086..


the commpiler consists of : 

lexer module: reads the text instruction from the unity 2d interface one by one , and splits them into Tokens 
parser module: recieves the tokens from the lexer , uses recursive decente methode to performe syntaxical analysisn and the end of every rule, performs semantical analysis if needed...

the visualiser consists of: 
AnimationGenerator module:  using a pseudo-code language we made , it takes the current instruction and transforms it into code in this language 
AnimationController module: the last layer between that communicates directly with the 2d interface, takes in input code wirtten in language mentioned above , in output makes changes in the interface (such color change of a compnenet, register value change....)

brief overview of the execution mechanisme: 

-the lexer class reads the instruction as is transforms it into tokens and sends it to the parser
-the parser does the syntaxique analysis using recursive decente, transforms the text instruction into objects of  type "instruction" that are readable by "cpu_core"
-for the semantical analysis and virtual memory allocation is usually done in "cpu_core"
-cpu core reads the instructions one by one and updates the regsiters , memory respectively 
-for the animation part we made a pseudo-code language consists of keywords 
-each instruction is also transformed into a text instruction in the languaged mentioned above
-the animation modules is responsible for interpreting these instructions and executes them throu the animationController module
-the animations are eather text updates, color changes , moving objects....


PS: TO LEARN MORE ABOUT THIS PROJECT PLEASE CONSULT THE MEMOIRE





TEST: 

to launche:
-write your code or keep the one written
-press "emuler" 
-press "pas avant" to start execution the code instruction by instruction (the current instruction is highlited in yellow)
-press "lire" to see the animation 
-you can also reduce  or increase the speed of the animation by pressing vitess + or vitess -
-to go to the next instruction  press pas avant again...

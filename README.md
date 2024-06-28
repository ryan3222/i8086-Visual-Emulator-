# i8086-Visual-Emulator-
a unity 2d game that visualises in real time the internal state, data flow of the intel 8086 while executing assembly code

-the cpu is emulated/represented by a class containing other components represented also by thier respective data structure/classe
-the interal components such as registers are represnted by a classe, data is held within attributes of the class

brief overview of the execution mechanisme: 

-the lexer class reads the instruction as is transforms it into tokens and sends it to the parser
-the parser does the syntaxique analysis using recursive decente, transforms the text instruction into objects of  type "instruction" that are readable by "cpu_core"
-for the semantical analysis and virtual memory allocation is usually done in "cpu_core"
-cpu core reads the instructions one by one and updates the regsiters , memory respectively 
-for the animation part we made a pseudo-code language consists of keywords 
-each instruction is also transformed into a text instruction in the languaged mentioned above
-the animation modules is responsible for interpreting these instructions and executes them throu the animationController module
-the animations are eather text updates, color changes , moving objects....





TEST: 

to launche:
-write your code or keep the one written
-press "emuler" 
-press "pas avant" to start execution the code instruction by instruction (the current instruction is highlited in yellow)
-press "lire" to see the animation 
-you can also reduce  or increase the speed of the animation by pressing vitess + or vitess -
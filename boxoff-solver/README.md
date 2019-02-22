## Analysis of Procedural Puzzle Challenge Generation for BoxOff

Challenges for solitaire puzzle games are typically limited in number and designed in advance by humans. Alternately, some games incorporate stochastic setup rules, in which the solver randomly sets up the game board before solving the challenge, which can greatly increase the number of possible challenges. However, these setup rules can often generate unsolvable or uninteresting challenges. 

For the game [BoxOff](https://pure.tue.nl/ws/files/4284803/598855.pdf), we examine how different stochastic challenge generation algorithms affect challenge solvability and interest.  We find that algorithms can generate challenges with a higher rate of solvability without sacrificing challenge interest by constraining randomness through embedding sub-elements of the puzzle mechanics into the physical pieces or setup of the game, which we label component entanglement.

## Requirements

* Microsoft Visual Studio 

## To Execute From Visual Studio

1. Open "BoxOff.sln" project in Simulator directory.
2. In the Main method of the Program.cs class, comment out the Command Line Execution Template.
3. In the Main method of the Program.cs class, uncomment the Debug Execution Template and adjust parameters as desired.
4. Run the program.

## To Execute From Console

1. Open "BoxOff.sln" project in Simulator directory.
2. In the Main method of the Program.cs class, comment out the Debug Execution Template.
3. In the Main method of the Program.cs class, uncomment the Command Line Execution Template.
4. Build (compile) the program.
5. Find the resulting executable (look in [project directory]/bin/Debug) and run it with desired arguments and values.

Sample usage: BoxOff.exe h=4 w=6 c=4 t=10 e=100 s=0 a=0

Usage help: BoxOff.exe ?

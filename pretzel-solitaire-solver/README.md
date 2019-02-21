# Analysis of Procedural Puzzle Challenge Generation for Pretzel Solitaire

Challenges for solitaire puzzle games are typically limited in number and designed in advance by humans. Alternately, some games incorporate stochastic setup rules, in which the solver randomly sets up the game board before solving the challenge, which can greatly increase the number of possible challenges. However, these setup rules can often generate unsolvable or uninteresting challenges. 

For the game [Pretzel Solitaire](https://pure.tue.nl/ws/files/4284803/598855.pdf), we examine how different stochastic challenge generation algorithms affect ease of physical setup, challenge solvability, and challenge difficulty.  We find that algorithms can be simple for the solver yet generate solvable and difficult challenges, by constraining randomness through embedding sub-elements of the puzzle mechanics into the physical pieces or setup of the game.

## Requirements

* Microsoft Visual Studio 

## To Execute From Visual Studio

1. Open "PretzelSolitaireSolver.sln" project in Simulator directory.
2. In the Main method of the Program.cs class, comment out the Command Line Execution Template.
3. In the Main method of the Program.cs class, uncomment the Debug Execution Template and adjust parameters as desired.
4. Run the program.

## To Execute From Console

1. Open "PretzelSolitaireSolver.sln" project in Simulator directory.
2. In the Main method of the Program.cs class, comment out the Debug Execution Template.
3. In the Main method of the Program.cs class, uncomment the Command Line Execution Template.
4. Build (compile) the program.
5. Find the resulting executable (look in [project directory]/bin/Debug) and run it with desired arguments and values.

Sample usage: PretzelSolitaireSolver.exe s=4 v=4 t=10 i=100 d=0 a=0 o=0

Usage help: PretzelSolitaireSolver.exe ?

## Output

1. Output to the console will be reported in a table of tab-delimited rows, each displaying:
    a. The trial number
    b. The number of iterations in the trial
    c. The number of solvable challenges in the trial
    d. The total number of moves taken for all solvable challenges in the trial
    e. The total number of anti-goal moves taken for all solvable challenges in the trial
2. The final row of the table displays roll-up totals for all trials

## Terminology and Notation

Terminology used for variable and method naming within the code, as well as output notation, follow de Brujin (1981).  See [https://pure.tue.nl/ws/files/4284803/598855.pdf](https://pure.tue.nl/ws/files/4284803/598855.pdf)

## Memory Warning

If the suit count x value count exceeds 40 or so, you might experience memory issues on some platforms.  Increasing heap space might help.  Consult documentation for your platform.

For grids larger than RAM allows, modification to store previously attained positions (board states) in a database should be considered.

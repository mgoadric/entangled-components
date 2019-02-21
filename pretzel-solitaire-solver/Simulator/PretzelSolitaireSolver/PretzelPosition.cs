using System;
using System.Collections.Generic;

namespace PretzelSolitaireSolver {

    public class PretzelPosition {
        const short None = -1;
        const ushort NoCard = 0; // 0 is the Ace of Spades, but aces get pulled out, so 0 is hole

        public ushort[] Tableau;
        public short[] HoleIndices;
        private ushort SuitCount;
        private ushort ValueCount;
        private Random rng = new Random();

        public PretzelPosition(ushort[] tableau, short[] holeIndices, ushort suitCount, ushort valueCount) {
            Tableau = tableau;
            HoleIndices = holeIndices;
            SuitCount = suitCount;
            ValueCount = valueCount;
        }

        public PretzelPosition(CardDeck deck) {
            Tableau = deck.Cards;
            HoleIndices = new short[deck.SuitCount];
            SuitCount = deck.SuitCount;
            ValueCount = deck.ValueCount;
            ushort holeCounter = 0;
            for (short i = 0; i < Tableau.Length; ++i) {
                // pull out aces, leaving holes
                if (Tableau[i] % ValueCount == 0) {
                    Tableau[i] = NoCard;
                    HoleIndices[holeCounter] = i;
                    ++holeCounter;
                }
            }
        }

        public PretzelPosition(PretzelPosition previousPosition) {
            Tableau = new ushort[previousPosition.Tableau.Length];
            Array.Copy(previousPosition.Tableau, Tableau, previousPosition.Tableau.Length);
            HoleIndices = new short[previousPosition.HoleIndices.Length];
            Array.Copy(previousPosition.HoleIndices, HoleIndices, previousPosition.HoleIndices.Length);
            SuitCount = previousPosition.SuitCount;
            ValueCount = previousPosition.ValueCount;
        }

        public List<PretzelPosition> GetSubsequentPositions() {
            List<PretzelPosition> newPositions = new List<PretzelPosition>();
            for (short i = 0; i < SuitCount; ++i) {
                // find card that fits this hole
                ushort cardNumberThatFitsHole = NoCard;
                if (HoleIndices[i] % ValueCount == 0) {
                    // hole is in first column, so the 2 of row's suit fits hole
                    cardNumberThatFitsHole = (ushort)(HoleIndices[i] + 1);
                } else if (Tableau[HoleIndices[i] - 1] == NoCard) {
                    // hole follows another hole
                    cardNumberThatFitsHole = NoCard; // NOTE: redundant
                } else if (Tableau[HoleIndices[i] - 1] % ValueCount < (ValueCount - 1)) {
                    // card before hole is not King, so next sequential card fits hole
                    // NOTE: In most versions of the game, the conditional for this else is extraneous
                    //       since the next card number after a King is an Ace and all Aces were
                    //       pulled out of the deck after the shuffle.  Left in for future variants.
                    cardNumberThatFitsHole = (ushort)(Tableau[HoleIndices[i] - 1] + 1);
                }
                // if possible, create position resulting from moving card into hole
                if (cardNumberThatFitsHole != NoCard) {
                    PretzelPosition newPosition = new PretzelPosition(this);
                    short indexOfCardThatFitsHole = (short)Array.IndexOf(newPosition.Tableau, cardNumberThatFitsHole);
                    newPosition.Tableau[HoleIndices[i]] = newPosition.Tableau[indexOfCardThatFitsHole];
                    newPosition.Tableau[indexOfCardThatFitsHole] = NoCard;
                    newPosition.HoleIndices[i] = indexOfCardThatFitsHole;
                    bool isAntiGoalMove = cardNumberThatFitsHole == indexOfCardThatFitsHole + 1;
                    newPositions.Add(newPosition);
                }
            }
            return newPositions;
        }

        public bool IsSolved() {
            bool solved = true;
            short i = 0;
            while (solved && (i < Tableau.Length)) {
                if ((Tableau[i] != NoCard) && (Tableau[i] != i + 1)) {
                    solved = false;
                }
                ++i;
            }
            return solved;
        }

        // NOTE: "donut hole" refers to a hole following a max-value card; nothing can be placed in it
        //       "extended hole" refers to hole following another hole; nothing can be placed in it
        //       "black hole" refers to a hole where all preceding cards in the row are in solution state; pulls you toward solution
        public short CalculateScore() {
            const short donutHoleScore = -1;
            const short extendedHoleScore = 0;
            short plainHoleScore = 1;
            short blackHoleScore = 2;//(short)SuitCount;
            short score = 0;
            for (int i = 0; i < HoleIndices.Length; ++i) {
                short firstColumnIndex = (short)((HoleIndices[i] / ValueCount) * ValueCount);
                if (HoleIndices[i] == firstColumnIndex) {
                    score += blackHoleScore; // hole in first column is always black hole
                } else if ((Tableau[HoleIndices[i] - 1] % ValueCount) == (ValueCount - 1)) {
                    score += donutHoleScore;
                } else if (Tableau[HoleIndices[i] - 1] == NoCard) {
                    score += extendedHoleScore;
                } else {
                    // check for black hole
                    bool isBlackHole = true;
                    for (int j = HoleIndices[i] - 1; j >= firstColumnIndex; --j) {
                        if (Tableau[i] != i + 1) {
                            isBlackHole = false;
                        }
                    }
                    if (isBlackHole) {
                        score += blackHoleScore;
                    } else {
                        score += plainHoleScore;
                    }
                }
            }
            return score;
        }

        // looks for 2s in suit-mismatched leftmost columns forming a cyclic blockade
        public bool HasDuelingDeuces() {
            List<int> duellingDeuceIndexes = new List<int>();
            for (ushort i = 0; i < SuitCount; ++i) {
                ushort leftmostCardValue = (ushort)(Tableau[i * ValueCount] % ValueCount);
                if ((leftmostCardValue == 1) && (Tableau[i * ValueCount] != (i * ValueCount + 1))) {
                    duellingDeuceIndexes.Add(i * ValueCount);
                }
            }
            for (int i = duellingDeuceIndexes.Count - 1; i >= 0; --i) {
                if (!duellingDeuceIndexes.Contains(Tableau[duellingDeuceIndexes[i]] - 1)) {
                    duellingDeuceIndexes.RemoveAt(i);
                }
            }
            return (duellingDeuceIndexes.Count > 1);
        }

        // looks for 3 in suit-matched leftmost column with suit-matched 2 in any rightmost column
        public bool HasDuckingCrab() {
            bool duckingCrabFound = false;
            for (ushort i = 0; i < SuitCount; ++i) {
                ushort threeCardNumber = (ushort)(i * ValueCount + 2);
                int threeCardIndex = Array.IndexOf(Tableau, threeCardNumber);
                if (threeCardIndex == (threeCardNumber - 2)) {
                    ushort twoCardNumber = (ushort)(threeCardNumber - 1);
                    int twoCardIndex = Array.IndexOf(Tableau, twoCardNumber);
                    if (twoCardIndex % ValueCount == ValueCount - 1) {
                        duckingCrabFound = true;
                    }
                }
            }
            return duckingCrabFound;
        }

        public override string ToString() {
            string output = string.Empty;
            // WARNING: range breaks for suit count > 20
            string[] suitNames = { "S", "H", "D", "C", "A", "B", "E", "F", "G", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "T" };
            for (short i = 0; i < Tableau.Length; ++i) {
                if (Tableau[i] > 0) {
                    ushort suit = (ushort)(Tableau[i] / ValueCount);
                    ushort value = (ushort)(Tableau[i] % ValueCount);
                    output += suitNames[suit] + (value + 1).ToString() + " ";
                } else {
                    output += "-- "; // hole
                }
                if (i % ValueCount == ValueCount - 1) {
                    output += "* "; // end of row
                }
            }
            return output;
        }
    }

}

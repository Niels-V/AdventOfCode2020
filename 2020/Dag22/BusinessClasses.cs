using System;
using System.Linq;
using System.Collections.Generic;

namespace Dag22
{
    public class Deck : Queue<int>
    {
        public int Draw()
        {
            return Dequeue();
        }

        public void Return(int card1, int card2)
        {
            Enqueue(card1);
            Enqueue(card2);
        }

        public Deck(IEnumerable<int> cards) : base(cards)
        { }

        public long FinishingScore()
        {
            var weights = Enumerable.Range(1, Count).Reverse();
            long acc = 0;
            foreach (var weight in weights)
            {
                acc += weight * Dequeue();
            }
            return acc;
        }

        public int[] CopyOfDeck()
        {
            var playerCards = new int[Count];
            CopyTo(playerCards, 0);
            return playerCards;
        }
        public int[] CopyOfDeck(int numberOfCards)
        {
            return CopyOfDeck().Take(numberOfCards).ToArray();
        }
    }

    public class CombatGame
    {
        public Deck Player1Deck { get; private set; }
        public Deck Player2Deck { get; private set; }
        public bool Finished { get; private set; }
        public CombatGame(IEnumerable<int> deck1, IEnumerable<int> deck2)
        {
            Player1Deck = new Deck(deck1);
            Player2Deck = new Deck(deck2);
        }

        public int PlayRound()
        {
            int roundWinner = 0;
            if (Finished) { return roundWinner; }
            var player1Card = Player1Deck.Draw();
            var player2Card = Player2Deck.Draw();
            if (player1Card > player2Card)
            {
                Player1Deck.Return(player1Card, player2Card);
                roundWinner = 1;
            }
            else
            {
                Player2Deck.Return(player2Card, player1Card);
                roundWinner = 2;
            }

            if (Player1Deck.Count == 0 || Player2Deck.Count == 0)
            {
                Finished = true;
            }
            return roundWinner;
        }

        public long PlayGame()
        {
            int lastWinner = 0;
            while(!Finished)
            {
                lastWinner = PlayRound();
            }
            if (lastWinner == 1)
            {
                return Player1Deck.FinishingScore();
            }
            return Player2Deck.FinishingScore();
        }
    }

    public class RecursiveCombatGame
    {
        public int GameId { get; private set; }
        public int RoundId { get; private set; }
        HashSet<string> Player1History { get; set; }
        HashSet<string> Player2History { get; set; }

        public Deck Player1Deck { get; private set; }
        public Deck Player2Deck { get; private set; }
        public bool Finished { get; private set; }
        public RecursiveCombatGame(IEnumerable<int> deck1, IEnumerable<int> deck2, int gameId)
        {
            GameId = gameId;
            RoundId = 1;
            Player1Deck = new Deck(deck1);
            Player2Deck = new Deck(deck2);
            Player1History = new HashSet<string>();
            Player2History = new HashSet<string>();
        }

        public int PlayRound()
        {
            Console.WriteLine($"Playing round {RoundId} of game {GameId}");
            RoundId++;
            int roundWinner = 0;
            if (Finished) { return roundWinner; }
            if (RecursiveCheck()) {
                //the game instantly ends in a win for player 1
                Finished = true;
                return 1; 
            }
            var player1Card = Player1Deck.Draw();
            var player2Card = Player2Deck.Draw();
            //If both players have at least as many cards remaining in their deck as the value of the card they just drew, 
            //the winner of the round is determined by playing a new game of Recursive Combat (see below).
            if (player1Card <= Player1Deck.Count && player2Card <= Player2Deck.Count)
            {
                var innerGame = new RecursiveCombatGame(Player1Deck.CopyOfDeck(player1Card), Player2Deck.CopyOfDeck(player2Card),GameId+1);
                var winnerInner = innerGame.PlayGame();
                if (winnerInner == 1)
                {
                    Player1Deck.Return(player1Card, player2Card);
                    roundWinner = 1;
                }
                else
                {
                    Player2Deck.Return(player2Card, player1Card);
                    roundWinner = 2;
                }
            }
            else
            {
                //at least one player must not have enough cards left in their deck to recurse; the winner of the round is the player with the higher-value card
                if (player1Card > player2Card)
                {
                    Player1Deck.Return(player1Card, player2Card);
                    roundWinner = 1;
                }
                else
                {
                    Player2Deck.Return(player2Card, player1Card);
                    roundWinner = 2;
                }
            }
            if (Player1Deck.Count == 0 || Player2Deck.Count == 0)
            {
                Finished = true;
            }
            return roundWinner;
        }

        private bool RecursiveCheck()
        {
            //Before either player deals a card, if there was a previous round in this game that had exactly the same cards in the same order in the same players' decks
            var player1Cards = string.Join("-",Player1Deck.CopyOfDeck());
            if (Player1History.Contains(player1Cards)) { return true; }

            var player2Cards = string.Join("-", Player2Deck.CopyOfDeck());
            if (Player2History.Contains(player2Cards)) { return true; }

            Player1History.Add(player1Cards);
            Player2History.Add(player2Cards);
            return false;
        }

        public int PlayGame()
        {
            int lastWinner = 0;
            while (!Finished)
            {
                lastWinner = PlayRound();
            }
            return lastWinner;
        }
        public long CalculateScore(int lastWinner)
        {
            if (lastWinner == 1)
            {
                return Player1Deck.FinishingScore();
            }
            return Player2Deck.FinishingScore();
        }
    }

}

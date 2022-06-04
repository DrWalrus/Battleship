using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShipCore
{

    public class BattleShipGame
    {
        public static readonly int GRID_SIZE = 10;
        public static readonly int MAX_TURNS = (GRID_SIZE * GRID_SIZE * 2) + 1;

        private Player[] Players { get; set; } = new Player[2];
        private int PlayerTurn = 0;
        private MatchInfo MatchInfo { get; set; }

        private Queue<Turn> Turns { get; set; }


        private Player CurrentPlayer
        {
            get { return Players[PlayerTurn % Players.Length]; }
        }

        private Player OpponentPlayer
        {
            get { return Players[(PlayerTurn + 1) % Players.Length];  }
        }

        public BattleShipGame()
        {
            Dictionary<ShipType, int> shipSizes = new Dictionary<ShipType, int>();
            shipSizes.Add(ShipType.Carrier, 5);
            shipSizes.Add(ShipType.Battleship, 4);
            shipSizes.Add(ShipType.Cruiser, 3);
            shipSizes.Add(ShipType.Submarine, 3);
            shipSizes.Add(ShipType.PatrolBoat, 2);


            MatchInfo = new MatchInfo(GRID_SIZE, shipSizes);
        }

        /**
         * Run a game of battleship with the given AI players.
         * Each player places their ships, 
         * then the players exchange shots until all of a player's ships are sunk or the max turned are reached
         *  
         */
        public MatchResult RunMatch(IBattleshipAI player1, IBattleshipAI player2)
        {
            Turns = new Queue<Turn>();

            // Initialze players and AI
            Players[0] = new Player(MatchInfo, player1);
            Players[1] = new Player(MatchInfo, player2);

            CurrentPlayer.AI.Initialise();
            OpponentPlayer.AI.Initialise();

            // Place ships
            CurrentPlayer.PlaceAllShips();
            OpponentPlayer.PlaceAllShips();

            CurrentPlayer.AI.PostPlaceAllShips();
            OpponentPlayer.AI.PostPlaceAllShips();

            MatchWinner winner = ExchangeShots();

            return new MatchResult(winner, Turns, Players[0].GetShipLocations(), Players[1].GetShipLocations());
        }

        private MatchWinner ExchangeShots()
        {
            // Exchange shots until all a player's ships are sunk or the max turned are reached
            while (MAX_TURNS > PlayerTurn)
            {
                // Allow the current player's AI to make a shot and pass it to the opponent player
                Coordinate coordinate = CurrentPlayer.AI.MakeShot();
                ShotResult shotResult = OpponentPlayer.UpdateGrid(coordinate);
                CurrentPlayer.AI.HandleShotResult(shotResult);
                
                // Record turn info
                Turns.Enqueue(new Turn(PlayerTurn % Players.Length, PlayerTurn, shotResult, OpponentPlayer.GetShipLocations()));

                //Round end: check win condition
                if((PlayerTurn % Players.Length) == 1)
                {
                    // Check if all ships are sunk
                    if (OpponentPlayer.GetHitsLeft() <= 0 && CurrentPlayer.GetHitsLeft() <= 0) 
                    {
                        return MatchWinner.Draw;
                           
                    }
                    else if(OpponentPlayer.GetHitsLeft() <= 0)
                    {
                        return ((MatchWinner)((PlayerTurn + 1) % Players.Length));
                    }
                    else if(CurrentPlayer.GetHitsLeft() <= 0)
                    {
                        return ((MatchWinner)((PlayerTurn) % Players.Length));
                    }
                }

                PlayerTurn++;
            }

            return MatchWinner.Draw;
        }


    }
}

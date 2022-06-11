using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BattleShipCore
{
    internal class Player: IPlayer
    {
        public static readonly double AI_TIME_LIMIT_SECONDS = 2;

        private Dictionary<Coordinate, ShipInfo> shipLocations = new Dictionary<Coordinate, ShipInfo>();

        public MatchInfo MatchInfo { get; private set; }

        public int MaxHits { get; private set; }

        public IBattleshipAI AI { get; private set; }

        public Player(MatchInfo matchInfo, IBattleshipAI ai)
        {
            MatchInfo = matchInfo;
            AI = ai;
            AI.Player = this;
        }


        /**
         * Get the number of hits this player has left
         */
        public int GetHitsLeft()
        {
            var search = from x in shipLocations.Values
                         where x.IsHit == true
                         select x;

            return MaxHits - search.Count();
        }

        /**
         * Apply given coordinate to the grid.
         * Will shot result with information about hit or miss
         */
        public ShotResult UpdateGrid(Coordinate coordinate)
        {
            if (!shipLocations.ContainsKey(coordinate))
                return new ShotResult(new ShipInfo(ShipType.Empty), coordinate, false);

            ShipInfo info = shipLocations[coordinate];
            info.IsHit = true;
            shipLocations[coordinate] = info;

            return new ShotResult(info, coordinate, IsShipSunk(info.ShipType));
        }

        
        private bool IsShipSunk(ShipType shipType)
        {
            var search = from x in shipLocations.Values
                         where x.IsHit == true && x.ShipType == shipType
                         select x;

            return search.Count() >= MatchInfo.ShipSizes[shipType];
        }

        /**
         * Place all ships available in the MatchInfo
         */ 
        public void PlaceAllShips()
        {
            foreach (var ship in MatchInfo.ShipSizes.Keys)
            {
                PlaceShip(ship);
            }

            MaxHits = shipLocations.Count;
        }

        /**
         * Attempt to place the given ship type
         * This is passed to the AI to do with a time limit
         * The AI's ship is added if the coordinates are valid
         */
        public void PlaceShip(ShipType shipType)
        {
           var task = RunAITask<PlaceShipResult>(() => { return AI.PlaceShip(shipType); });

            PlaceShipResult placeShipResult;
            
            if (task.IsCompletedSuccessfully)
                placeShipResult = task.Result;
            else
                return;

            Coordinate[] coordinates = CreateShipCoordinates(shipType, placeShipResult);

            if (coordinates.Length == 0)
                return;

            foreach (Coordinate newCoordinate in coordinates)
            {
                shipLocations.Add(newCoordinate, new ShipInfo(shipType));
            }
        }

        /**
         * Create set of coordinates for the proposed ship placement and ship type
         */ 
        private Coordinate[] CreateShipCoordinates(ShipType shipType, PlaceShipResult placeShipResult)
        {
            Coordinate[] coordinates = new Coordinate[MatchInfo.ShipSizes[shipType]];
            Coordinate coordinate = placeShipResult.Coordinate;

            for (int i = 0; i < coordinates.Length; i++)
            {
                if (placeShipResult.Orientation == Orientation.Horizontal)
                    coordinate.X += 1;
                else
                    coordinate.Y += 1;

                if (IsValidCoordinate(coordinate) == false)
                    return new Coordinate[0];

                coordinates[i] = coordinate;
            }

            return coordinates;
        }

        /**
         * Attempt to make a shot.
         * Pass to the AI to create a coordinate with a time limit
         */ 
        public Coordinate AIMakeShot()
        {
            Coordinate coordinate = new Coordinate(-1, -1);
            var task = RunAITask<Coordinate>(() => { return AI.MakeShot(); });
            
            if (task.IsCompletedSuccessfully)
            {
                coordinate = task.Result;
            }

            return coordinate;
        }

        /**
         * Run AI Initialise hook
         */
        public void AIInitialise()
        {
            RunAITask(() =>
            {
                AI.Initialise();
            });
        }

        /**
         * Run AI post place all ships hook
         */
        public void AIPostPlaceAllShips()
        {
            RunAITask(() =>
            {
                AI.PostPlaceAllShips();
            });
        }

        /**
         * Pass the shot result to the AI
         */
        public void AIHandleShotResult(ShotResult shotResult)
        {
            RunAITask(() =>
            {
                AI.HandleShotResult(shotResult);
            });
        }

        /**
         * Run the given AI method that does not return a value.
         * Limits amount of time AI method can run
         */
        protected Task RunAITask(Action action)
        {
            return AwaitTask(Task.Run(action));
        }

        /**
         * Run the given AI method that returns a value of type T
         * Limits amount of time AI method can run
         */
        protected Task<T> RunAITask<T>(Func<T> func)
        {
            return (Task<T>)AwaitTask(Task<T>.Run(func));
        }

        /**
         * Wait for given task to complete. Used for AI methods
         * Limits amount of time the task can run
         */
        protected Task AwaitTask(Task task)
        {
            if (task != null)
            {
                task.Wait(TimeSpan.FromSeconds(AI_TIME_LIMIT_SECONDS));
            }

            return task;
        }

        /**
         * Check if the given coordinate is within the grid and not currently in this player's ship locations
         */ 
        public bool IsValidCoordinate(Coordinate coordinate)
        {
            if (coordinate.X < 0 || coordinate.Y < 0 || coordinate.X > MatchInfo.GridSize || coordinate.Y > MatchInfo.GridSize ||
                shipLocations.ContainsKey(coordinate))
                return false;

            return true;
        }

        /**
         * Get a copy of this player's ship locations
         */ 
        public Dictionary<Coordinate, ShipInfo> GetShipLocations()
        {
            return new Dictionary<Coordinate, ShipInfo>(shipLocations);
        }
    }
}

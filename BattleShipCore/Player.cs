using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShipCore
{
    internal class Player: IPlayer
    {
        public Player(MatchInfo matchInfo, IBattleshipAI ai)
        {
            MatchInfo = matchInfo;
            AI = ai;
            AI.Player = this;
        }

        private Dictionary<Coordinate, ShipInfo> shipLocations = new Dictionary<Coordinate, ShipInfo>();
        public MatchInfo MatchInfo { get; private set; }

        public int MaxHits { get; private set; }

        public IBattleshipAI AI { get; private set; }


        public int GetHitsLeft()
        {
            var search = from x in shipLocations.Values
                         where x.IsHit == true
                         select x;

            return MaxHits - search.Count();
        }

        /**
         * TODO: change to return result object, return ship type and is sunk
         */
        public ShotResult UpdateGrid(Coordinate coordinate)
        {
            if (!shipLocations.ContainsKey(coordinate))
                return new ShotResult(new ShipInfo(ShipType.Unknown), coordinate, false);

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

        public void PlaceAllShips()
        {
            foreach (var ship in MatchInfo.ShipSizes.Keys)
            {
                PlaceShip(ship);
            }

            MaxHits = shipLocations.Count;
        }
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

        public void AIInitialise()
        {
            RunAITask(() =>
            {
                AI.Initialise();
            });
        }

        public void AIPostPlaceAllShips()
        {
            RunAITask(() =>
            {
                AI.PostPlaceAllShips();
            });
        }

        public void AIHandleShotResult(ShotResult shotResult)
        {
            RunAITask(() =>
            {
                AI.HandleShotResult(shotResult);
            });
        }

        protected Task RunAITask(Action action)
        {
            var task = Task.Run(action);

            task.Wait(TimeSpan.FromSeconds(2));

            return task;
        }

        protected Task<T> RunAITask<T>(Func<T> func)
        {
            var task = Task<T>.Run(func);

            task.Wait(TimeSpan.FromSeconds(2));

            return task;
        }

        public bool IsValidCoordinate(Coordinate coordinate)
        {
            if (coordinate.X < 0 || coordinate.Y < 0 || coordinate.X > MatchInfo.GridSize || coordinate.Y > MatchInfo.GridSize ||
                shipLocations.ContainsKey(coordinate))
                return false;

            return true;
        }

        public Dictionary<Coordinate, ShipInfo> GetShipLocations()
        {
            return new Dictionary<Coordinate, ShipInfo>(shipLocations);
        }
    }
}

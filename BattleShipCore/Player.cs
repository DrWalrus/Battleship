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
            PlaceShipResult placeShipResult = AI.PlaceShip(shipType);
            
            int shipSize = MatchInfo.ShipSizes[shipType];
            Coordinate[] coordinates = new Coordinate[shipSize];
            Coordinate coordinate = placeShipResult.Coordinate;
            
            for (int i = 0; i < shipSize; i++)
            {
                switch (placeShipResult.Orientation)
                {
                    case Orientation.Horizontal:
                        coordinate.X += 1;
                        break;
                    case Orientation.Vertical:
                        coordinate.Y += 1;
                        break;
                    default:
                        break;
                }

                if (IsValidCoordinate(coordinate) == false)
                    return;
                
                coordinates[i] = coordinate;
            }

            foreach (Coordinate newCoordinate in coordinates)
            {
                shipLocations.Add(newCoordinate, new ShipInfo(shipType));
            }
            
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

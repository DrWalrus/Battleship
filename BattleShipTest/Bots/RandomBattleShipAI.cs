using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BattleShipCore;

namespace BattleShipPublicSDK.Bots
{
    /**
     * Sample AI
     * Implements IBattleshipAI interface
     * Places ships and makes shots randomly
     * 
     * 
     */
    internal class RandomBattleShipAI : IBattleshipAI
    {
        public IPlayer Player { get; set; }
        public string Name { get { return "Random Battleship AI";  } }

        private Random random = new Random();

        public RandomBattleShipAI()
        {
            
        }

        public void Initialise()
        {
            
        }

        public Coordinate MakeShot()
        {
            return RandomCoordinate();
        }

        public PlaceShipResult PlaceShip(ShipType shipType)
        {
            PlaceShipResult result = new PlaceShipResult();
            result.Coordinate = RandomCoordinate();

            int orientation = random.Next(0, 1);
            result.Orientation = (Orientation)orientation;

            return result;
        }

        private Coordinate RandomCoordinate()
        {
            int x = random.Next(0, Player.MatchInfo.GridSize);
            int y = random.Next(0, Player.MatchInfo.GridSize);

            return new Coordinate(x, y);
        }

        public void HandleShotResult(ShotResult shotResult)
        {
            
        }
        public void PostPlaceAllShips()
        {

        }
    }
}

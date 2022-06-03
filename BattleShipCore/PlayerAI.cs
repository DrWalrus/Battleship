using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShipCore
{
    internal class PlayerAI
    {
        public IPlayer Player { private get; set; }

        public string Name 
        {
            get { return ""; }
        }

        public MatchInfo MatchInfo
        {
            get { return Player.MatchInfo; }
        }

        public int MaxHits
        { 
            get { return Player.MaxHits; }
        }  

        public Dictionary<Coordinate, ShipInfo> GetShipLocations()
        { 
            return Player.GetShipLocations();
        }

        public Coordinate MakeShot() { return new Coordinate(); }

        public PlaceShipResult PlaceShip(ShipType shipType) { return new PlaceShipResult(); }   
    }
}

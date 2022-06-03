using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShipCore
{
    /**
     * Holds info about the match available to the AI.
     */ 
    public interface IPlayer
    {
        public MatchInfo MatchInfo { get; }

        public int MaxHits { get; }

        public Dictionary<Coordinate, ShipInfo> GetShipLocations();
    }
}

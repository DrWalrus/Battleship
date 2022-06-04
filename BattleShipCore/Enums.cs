using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShipCore
{
    /**
     * Available ship types
     */
    public enum ShipType
    {
        Carrier,
        Battleship,
        Cruiser,
        Submarine,
        PatrolBoat,
        Unknown,
        None
    }

    public enum Orientation
    {
        Vertical,
        Horizontal
    }

    public enum MatchWinner
    {
        Draw = -1,
        Player1 = 0,
        Player2 = 1,
    }
}

namespace BattleShipCore
{
    /**
     * Coordinate in 2D space
     * Holds X and Y values
     */ 
    public struct Coordinate
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Coordinate(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override bool Equals(object? obj) => obj is Coordinate c && this.Equals(c);

        public bool Equals(Coordinate c) => X == c.X && Y == c.Y;

        public override int GetHashCode() => (X, Y).GetHashCode();

        public static bool operator ==(Coordinate l, Coordinate r) => l.Equals(r);

        public static bool operator !=(Coordinate l, Coordinate r) => !(l == r);
    }

    /**
     * Used by AI to attempt to place a ship.
     * Orientation is either horizonal or vertical
     * Coordinate is the top most point in vertical mode and the left most point in horizontal
     */
    public struct PlaceShipResult
    {
        public Coordinate Coordinate { get; set; }
        public Orientation Orientation { get; set; }
    }


    /**
     * Public information about the current match available to players and AI
     */
    public struct MatchInfo
    {
        public int GridSize { get; private set; }

        public Dictionary<ShipType, int> ShipSizes { get; private set; }

        public MatchInfo(int gridSize, Dictionary<ShipType, int> shipSizes)
        {
            GridSize = gridSize;
            ShipSizes = shipSizes;
        }
    }

    /**
     * Info about a turn of battleship.
     * A turn is a single player's shot
     * Includes the player whose turn it was, and the ship locations of the opposing player
     */ 
    public struct Turn
    {
        public int PlayerIndex { get; private set; }

        public int TurnNumber { get; private set; }

        public ShotResult ShotResult { get; private set; }

        public Dictionary<Coordinate, ShipInfo> ShipLocations { get; private set; }

        public Turn(int playerIndex, int turnNumber, ShotResult shotResult, Dictionary<Coordinate, ShipInfo> shipLocations)
        {
            PlayerIndex = playerIndex;
            TurnNumber = turnNumber;
            ShotResult = shotResult;
            ShipLocations = shipLocations;
        }

    }

    /**
     * Result of a single battleship match
     * Who won and record of turns
     */ 
    public struct MatchResult
    {
        public MatchWinner Winner { get; set; }

        public Queue<Turn> Turns { get; set; }

        public Dictionary<Coordinate, ShipInfo> Player1Ships { get; private set; }

        public Dictionary<Coordinate, ShipInfo> Player2Ships { get; private set; }

        public MatchResult(MatchWinner winner, Queue<Turn> turns, Dictionary<Coordinate, ShipInfo>  p1Ships, Dictionary<Coordinate, ShipInfo> p2Ships)
        {
            Winner = winner;
            Turns = turns;
            Player1Ships = p1Ships;
            Player2Ships = p2Ships;
        }
    }

    /**
     * Information about ship grid cell
     */ 
    public struct ShipInfo
    {
        public bool IsHit { get; set; }

        public ShipType ShipType { get; private set; }

        public ShipInfo(ShipType shipType)
        {
            ShipType = shipType;
            IsHit = false;
        }
    }

    /**
     * Information about a shot that has been made and updated the opposing player's grid
     */ 
    public struct ShotResult
    {
        public bool IsSunk { get; private set; }
        public Coordinate Coordinate { get; private set; }

        public ShipInfo ShipInfo { get; private set; }

        public ShotResult(ShipInfo shipInfo, Coordinate coordinate, bool isSunk)
        {
            ShipInfo = shipInfo;
            Coordinate = coordinate;
            IsSunk = isSunk;
        }
    }
}
namespace BattleShipCore
{
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
     * Public information about the current match
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

    public struct Turn
    {
        public int PlayerIndex { get; private set; }

        public int TurnNumber { get; private set; }

        public Dictionary<Coordinate, ShipInfo> ShipLocations { get; private set; }

        public Turn(int playerIndex, int turnNumber, Dictionary<Coordinate, ShipInfo> shipLocations)
        {
            PlayerIndex = playerIndex;
            TurnNumber = turnNumber;
            ShipLocations = shipLocations;
        }

    }

    public struct MatchResult
    {
        public MatchWinner Winner { get; set; }

        public Queue<Turn> Turns { get; set; }

        public MatchResult(MatchWinner winner, Queue<Turn> turns)
        {
            Winner = winner;
            Turns = turns;
        }
    }

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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShipCore
{
    /**
     * AI that controls a battleship.
     * The primary actions are: MakeShot and PlaceShip
     * 
     * Note: the Player will not be available in the constructor
     */
    public interface IBattleshipAI
    {
        /**
         * The player of this AI. 
         * Contains data such as the match info, current shots and max hits.
         * If the AI is the brain, the player is the body
         */
        public IPlayer Player { get; set; }

        /**
         * The name of this AI
         */ 
        public string Name { get; }

        /**
         * Do your init work here. The player will be available here, but not MaxHits
         */
        public void Initialise();

        /**
         * Create coordinates for a new shot. Called by the IPlayer during gameplay
         */
        public Coordinate MakeShot();

        /**
         * Attempt to place a ship.
         * If a ship placement is invalid the ship will not be placed, you will have fewer max hits.
         */
        public PlaceShipResult PlaceShip(ShipType shipType);

        /**
        * Called once after both players have place all ships. 
        */
        public void PostPlaceAllShips();

        /**
         * TODO: add method to handle shot result
         */

        public void HandleShotResult(ShotResult shotResult);

    }
}

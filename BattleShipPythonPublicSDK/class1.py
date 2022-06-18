from BattleShipCore import IBattleshipAI, ShotResult, ShipType, PlaceShipResult, ShotResult
  

class BattleshipAI(IBattleshipAI):
    __namespace__ = "BattleShipCore"
    def Name(self):
        return "Python AI"

    def Initialise(self):
        return

    def MakeShot(self) -> ShotResult:
        return None

    def PlaceShip(self, shipType: ShipType) -> PlaceShipResult:
        return None

    def PostPlaceAllShips(self):
        return

    def HandleShotResult(self, shotResult: ShotResult):
        return


import sys
import os
import clr

# Add the BattleShipCore dll
clr.AddReference("BattleShipCore")

# Import everything from BattleShipCore
from BattleShipCore import *

from class1 import BattleshipAI

ai = BattleshipAI();

c = Coordinate(2, 4)
print(c.Y)

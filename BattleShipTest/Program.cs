// See https://aka.ms/new-console-template for more information
using BattleShipCore;
using BattleShipPublicSDK;
Console.WriteLine("Hello, World!");
BattleShipGame game = new BattleShipGame();
game.RunMatch(new RandomBattleShipAI(), new RandomBattleShipAI());
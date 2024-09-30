using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

public class Bomb
{
    public int BombX { get; set; }
    public int BombY { get; set; }
    public static bool IsBomb = false;

    public static Dictionary<(int, int), (int bombCountDown, bool isArmed)> bombDics = new Dictionary<(int, int), (int, bool)>();
    public Bomb(int startX, int startY)
    {
        BombX = startX;
        BombY = startY;
    }

    public void CheckBombAndTriggerCountdown(int currX, int currY, Dictionary<(int, int), (int countDown, bool isArmed)> bombDics)
    {
        foreach (var bomb in bombDics)
        {
            var bombPosition = bomb.Key;
            var bombData = bomb.Value;

            if (IsPlayerNextToBomb(currX, currY, bombPosition.Item1, bombPosition.Item2))
            {
                if (!bombData.isArmed)
                {
                    NinjaGame.AddMessage("Bomb is active");
                    bombDics[bombPosition] = (bombData.countDown, true);
                }
            }
        }
    }

    private static bool IsPlayerNextToBomb(int ninjaX, int ninjaY, int bombX, int bombY)
    {
        return (ninjaX == bombX && ninjaY == bombY - 1) ||
               (ninjaX == bombX && ninjaY == bombY + 1) ||
               (ninjaX == bombX - 1 && ninjaY == bombY) ||
               (ninjaX == bombX + 1 && ninjaY == bombY);
    }

    public void BombCountDownAndExplode(int ninjaX, int ninjaY, char[,] map)
    {
        foreach (var bomb in bombDics)
        {
            var bombPosition = bomb.Key;
            var bombData = bomb.Value;

            if (bombData.isArmed && bombData.bombCountDown > 0)
            {
                bombData.bombCountDown--;
                bombDics[bombPosition] = (bombData.bombCountDown, true);
                map[bombPosition.Item1, bombPosition.Item2] = bombData.bombCountDown.ToString()[0];
                NinjaGame.PrintMap();
                if(bombData.bombCountDown == 0)
                {
                    Explode(ninjaX, ninjaY, bombPosition, bombData, map);
                    NinjaGame.AddMessage("Bomb(s) exploded!");
                }
            }
        }
    }

    private void Explode(int ninjaX, int ninjaY, (int,int) bombPosition, (int, bool) bombData, char[,] map)
    {
        DestroyObstacles(ninjaX, ninjaY, bombPosition, bombData, map);
        TriggerNearbyBombs(ninjaX, ninjaY, bombPosition, bombData, map);        
    }

    private bool CheckPosition(int ninjaX, int ninjaY, int bombX, int bombY, char[,] map)
    {
        if (bombX < 0 || bombY < 0 || bombX >= map.GetLength(0) || bombY >= map.GetLength(1))
            return false;

        if (map[bombX, bombY] == '#')
            return false;

        if (ninjaX == bombX && ninjaY == bombY)
        {
            NinjaGame.AddMessage("Ninja destroyed by bomb!");
            throw new Exception("Ninja destroyed by bomb!");
        }

        if (map[bombX, bombY] == 'X')
            map[bombX, bombY] = ' ';

        return true;
    }
    private void DestroyObstacles(int ninjaX, int ninjaY, (int, int) bombPosition, (int, bool) bombData, char[,] map)
    {
        int radius = 2;

        for (int i = 1; i <= radius; i++)
        {
            if (!CheckPosition(ninjaX, ninjaY, bombPosition.Item1 - i, bombPosition.Item2, map))
                break;
        }
        NinjaGame.PrintMap();
        for (int i = 1; i <= radius; i++)
        {
            if (!CheckPosition(ninjaX, ninjaY, bombPosition.Item1 + i, bombPosition.Item2, map))
                break;
        }

        for (int j = 1; j <= radius; j++)
        {
            if (!CheckPosition(ninjaX, ninjaY, bombPosition.Item1, bombPosition.Item2 - j, map))
                break;
        }

        for (int j = 1; j <= radius; j++)
        {
            if (!CheckPosition(ninjaX, ninjaY, bombPosition.Item1, bombPosition.Item2 + j, map))
                break;
        }        
    }

    private void TriggerNearbyBombs(int ninjaX, int ninjaY, (int, int) bombPosition, (int, bool) bombData, char[,] map)
    {
        foreach (var surroundingBomb in bombDics)
        {
            var surroundingBombPosition = surroundingBomb.Key;
            var surroundingBombData = surroundingBomb.Value;

            if (surroundingBombData.bombCountDown > 0 && surroundingBombPosition != bombPosition)
            {
                int distance = Math.Abs(surroundingBombPosition.Item1 - bombPosition.Item1) + Math.Abs(surroundingBombPosition.Item2 - bombPosition.Item2);

                if (distance <= 2)
                {
                    surroundingBombData.bombCountDown = 0;
                    bombDics[surroundingBombPosition] = (surroundingBombData.bombCountDown, true);
                    map[surroundingBombPosition.Item1, surroundingBombPosition.Item2] = surroundingBombData.bombCountDown.ToString()[0];
                    NinjaGame.PrintMap();
                    Explode(ninjaX, ninjaY, surroundingBombPosition, surroundingBombData, map);
                }
            }
        }
    }
}


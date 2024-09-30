using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

public class Ninja
{
    public int NinjaX { get; set; }
    public int NinjaY { get; set; }

    public int _holySymbolCounter { get; set; }
    public int _shurikensOnMap { get; set; }
    public int _shurikenCount { get; private set; }

    public static int _currDirection = 0;
    public static char _prevSecretPath = '\0';
    public static char _prevDirChange = '\0';
    public static char _currDir = '\0';
    public static char _prevMirror = '\0';
    public static char _prevBomb = '\0';
    public static bool _isMirrored = false;
    public static bool _inBreakerMode = false;
    private Bomb bomb;

    public Ninja(int startX, int startY, Bomb bomb)
    {
        NinjaX = startX;
        NinjaY = startY;
        _shurikenCount = 3;//starting limit
        this.bomb = bomb;
    }

    private bool IsValidMove(int x, int y, char[,] map)
    {
        if (_inBreakerMode)
        {
            if (map[x, y] == 'X')
            {
                map[x, y] = ' ';
                //_shurikenCount++;//not sure if you get a * if destroying X in breakerMode
                NinjaGame.AddMessage("BreakerMode (destroyed X)");
            }
        }

        // Check for unbreakables
        return x >= 0 && x < map.GetLength(0) &&
               y >= 0 && y < map.GetLength(1) &&
               map[x, y] != '#' && map[x, y] != 'X' &&
               map[x, y] != '$';
    }

    public void Movement(char[,] map)
    {
        bool moved = false;
        bool enteredMirrored = false;

        while (!moved)
        {
            switch (_currDirection)
            {
                case 0://south
                    while (IsValidMove(NinjaX + 1, NinjaY, map) || (_isMirrored && IsValidMove(NinjaX, NinjaY - 1, map)))
                    {
                        if (_isMirrored)
                        {
                            enteredMirrored = true;
                            while (IsValidMove(NinjaX, NinjaY - 1, map))//west
                            {
                                ThrowShuriken(map);
                                if (_holySymbolCounter == 0)
                                {
                                    NinjaGame.AddMessage("All holy symbols destroyed");
                                    NinjaGame.PrintMap();
                                    return;
                                }
                                else if (_shurikenCount == 0 && _shurikensOnMap == 0 && _holySymbolCounter > 1)
                                {
                                    NinjaGame.AddMessage("LOOP");
                                    throw new Exception("LOOP");
                                }

                                if (Bomb.IsBomb)//check if there is bomb on map otherwise we dont need to hit this code
                                {
                                    bomb.CheckBombAndTriggerCountdown(NinjaX, NinjaY, Bomb.bombDics);//Checks if ninja is next to bomb
                                    bomb.BombCountDownAndExplode(NinjaX, NinjaY, map);
                                    NinjaGame.PrintMap();
                                }
                                NinjaGame.AddMessage("Mirrored WEST (current direction)");
                                MoveNinja(NinjaX, NinjaY - 1, map);

                                if (!_isMirrored)//we need to check if we are still mirrored since in the MoveNinja() we can get out of it, if so break out of the mirror
                                {
                                    moved = true;
                                    break;
                                }
                                NinjaGame.PrintMap();
                                moved = true;
                            }
                            break;
                        }

                        ThrowShuriken(map);
                        if (_holySymbolCounter == 0)
                        {
                            NinjaGame.AddMessage("All holy symbols destroyed");
                            NinjaGame.PrintMap();
                            return;
                        }
                        else if (_shurikenCount == 0 && _shurikensOnMap == 0 && _holySymbolCounter > 1)
                        {
                            NinjaGame.AddMessage("LOOP");
                            throw new Exception("LOOP");
                        }

                        if (Bomb.IsBomb)//check if there is bomb on map otherwise we dont need to hit this code
                        {
                            bomb.CheckBombAndTriggerCountdown(NinjaX, NinjaY, Bomb.bombDics);//Checks if ninja is next to bomb
                            bomb.BombCountDownAndExplode(NinjaX, NinjaY, map);
                            NinjaGame.PrintMap();
                        }
                        NinjaGame.AddMessage("SOUTH (initial direction)");
                        MoveNinja(NinjaX + 1, NinjaY, map);

                        NinjaGame.PrintMap();
                        moved = true;
                    }

                    if (enteredMirrored && !_isMirrored)//not sure if we are supposed to stay in the same direction after exiting mirrored - but both should work
                        _currDirection = 0;
                    else
                        _currDirection = 1;

                    break;
                case 1://east             
                    while (IsValidMove(NinjaX, NinjaY + 1, map) || (_isMirrored && IsValidMove(NinjaX - 1, NinjaY, map)))
                    {
                        if (_isMirrored)
                        {
                            enteredMirrored = true;

                            while (IsValidMove(NinjaX - 1, NinjaY, map))//north
                            {
                                ThrowShuriken(map);
                                if (_holySymbolCounter == 0)
                                {
                                    NinjaGame.AddMessage("All holy symbols destroyed");
                                    NinjaGame.PrintMap();
                                    return;
                                }
                                else if (_shurikenCount == 0 && _shurikensOnMap == 0 && _holySymbolCounter > 1)
                                {
                                    NinjaGame.AddMessage("LOOP");
                                    throw new Exception("LOOP");
                                }

                                if (Bomb.IsBomb)//check if there is bomb on map otherwise we dont need to hit this code
                                {
                                    bomb.CheckBombAndTriggerCountdown(NinjaX, NinjaY, Bomb.bombDics);//Checks if ninja is next to bomb
                                    bomb.BombCountDownAndExplode(NinjaX, NinjaY, map);
                                    NinjaGame.PrintMap();
                                }
                                NinjaGame.AddMessage("Mirrored NORTH (current direction)");
                                MoveNinja(NinjaX - 1, NinjaY, map);

                                if (!_isMirrored)//we need to check if we are still mirrored since in the MoveNinja() we can get out of it, if so break out of the mirror
                                {
                                    moved = true;
                                    break;
                                }
                                NinjaGame.PrintMap();
                                moved = true;
                            }
                            break;
                        }

                        ThrowShuriken(map);
                        if (_holySymbolCounter == 0)
                        {
                            NinjaGame.AddMessage("All holy symbols destroyed");
                            NinjaGame.PrintMap();
                            return;
                        }
                        else if (_shurikenCount == 0 && _shurikensOnMap == 0 && _holySymbolCounter > 1)
                        {
                            NinjaGame.AddMessage("LOOP");
                            throw new Exception("LOOP");
                        }

                        if (Bomb.IsBomb)//check if there is bomb on map otherwise we dont need to hit this code
                        {
                            bomb.CheckBombAndTriggerCountdown(NinjaX, NinjaY, Bomb.bombDics);//Checks if ninja is next to bomb
                            bomb.BombCountDownAndExplode(NinjaX, NinjaY, map);
                            NinjaGame.PrintMap();
                        }
                        NinjaGame.AddMessage("EAST (current direction)");
                        MoveNinja(NinjaX, NinjaY + 1, map);

                        NinjaGame.PrintMap();
                        moved = true;
                    }

                    if (enteredMirrored && !_isMirrored)//not sure if we are supposed to stay in the same direction after exiting mirrored - but both should work
                        _currDirection = 1;
                    else
                        _currDirection = 2;

                    break;
                case 2://north
                    while (IsValidMove(NinjaX - 1, NinjaY, map) || (_isMirrored && IsValidMove(NinjaX, NinjaY + 1, map)))
                    {
                        if (_isMirrored)
                        {
                            enteredMirrored = true;

                            while (IsValidMove(NinjaX, NinjaY + 1, map))//east
                            {
                                ThrowShuriken(map);
                                if (_holySymbolCounter == 0)
                                {
                                    NinjaGame.AddMessage("All holy symbols destroyed");
                                    NinjaGame.PrintMap();
                                    return;
                                }
                                else if (_shurikenCount == 0 && _shurikensOnMap == 0 && _holySymbolCounter > 1)
                                {
                                    NinjaGame.AddMessage("LOOP");
                                    throw new Exception("LOOP");
                                }

                                if (Bomb.IsBomb)//check if there is bomb on map otherwise we dont need to hit this code
                                {
                                    bomb.CheckBombAndTriggerCountdown(NinjaX, NinjaY, Bomb.bombDics);//Checks if ninja is next to bomb
                                    bomb.BombCountDownAndExplode(NinjaX, NinjaY, map);
                                    NinjaGame.PrintMap();
                                }

                                NinjaGame.AddMessage("Mirrored EAST (current direction)");
                                MoveNinja(NinjaX, NinjaY + 1, map);

                                if (!_isMirrored)//we need to check if we are still mirrored since in the MoveNinja() we can get out of it, if so break out of the mirror
                                {
                                    moved = true;
                                    break;
                                }
                                NinjaGame.PrintMap();
                                moved = true;
                            }
                            break;
                        }

                        ThrowShuriken(map);
                        if (_holySymbolCounter == 0)
                        {
                            NinjaGame.AddMessage("All holy symbols destroyed");
                            NinjaGame.PrintMap();
                            return;
                        }
                        else if (_shurikenCount == 0 && _shurikensOnMap == 0 && _holySymbolCounter > 1)
                        {
                            NinjaGame.AddMessage("LOOP");
                            throw new Exception("LOOP");
                        }

                        if (Bomb.IsBomb)//check if there is bomb on map otherwise we dont need to hit this code
                        {
                            bomb.CheckBombAndTriggerCountdown(NinjaX, NinjaY, Bomb.bombDics);//Checks if ninja is next to bomb
                            bomb.BombCountDownAndExplode(NinjaX, NinjaY, map);
                            NinjaGame.PrintMap();
                        }

                        NinjaGame.AddMessage("NORTH (current direction)");
                        MoveNinja(NinjaX - 1, NinjaY, map);

                        NinjaGame.PrintMap();
                        moved = true;
                    }
                    if (enteredMirrored && !_isMirrored)//not sure if we are supposed to stay in the same direction after exiting mirrored - but both should work
                        _currDirection = 2;
                    else
                        _currDirection = 3;

                    break;
                case 3://west
                    while (IsValidMove(NinjaX, NinjaY - 1, map) || (_isMirrored && IsValidMove(NinjaX + 1, NinjaY, map)))
                    {
                        if (_isMirrored)
                        {
                            enteredMirrored = true;

                            while (IsValidMove(NinjaX + 1, NinjaY, map))//south
                            {
                                ThrowShuriken(map);
                                if (_holySymbolCounter == 0)
                                {
                                    NinjaGame.AddMessage("All holy symbols destroyed");
                                    NinjaGame.PrintMap();
                                    return;
                                }
                                else if (_shurikenCount == 0 && _shurikensOnMap == 0 && _holySymbolCounter > 1)
                                {
                                    NinjaGame.AddMessage("LOOP");
                                    throw new Exception("LOOP");
                                }

                                if (Bomb.IsBomb)//check if there is bomb on map otherwise we dont need to hit this code
                                {
                                    bomb.CheckBombAndTriggerCountdown(NinjaX, NinjaY, Bomb.bombDics);//Checks if ninja is next to bomb
                                    bomb.BombCountDownAndExplode(NinjaX, NinjaY, map);
                                    NinjaGame.PrintMap();
                                }
                                NinjaGame.AddMessage("Mirrored SOUTH (current direction)");
                                MoveNinja(NinjaX + 1, NinjaY, map);

                                if (!_isMirrored)//we need to check if we are still mirrored since in the MoveNinja() we can get out of it, if so break out of the mirror
                                {
                                    moved = true;
                                    break;
                                }
                                NinjaGame.PrintMap();
                                moved = true;
                            }
                            break;
                        }

                        ThrowShuriken(map);
                        if (_holySymbolCounter == 0)
                        {
                            NinjaGame.AddMessage("All holy symbols destroyed");
                            NinjaGame.PrintMap();
                            return;
                        }
                        else if (_shurikenCount == 0 && _shurikensOnMap == 0 && _holySymbolCounter > 1)
                        {
                            NinjaGame.AddMessage("LOOP");
                            throw new Exception("LOOP");
                        }

                        if (Bomb.IsBomb)//check if there is bomb on map otherwise we dont need to hit this code
                        {
                            bomb.CheckBombAndTriggerCountdown(NinjaX, NinjaY, Bomb.bombDics);//Checks if ninja is next to bomb
                            bomb.BombCountDownAndExplode(NinjaX, NinjaY, map);
                            NinjaGame.PrintMap();
                        }

                        NinjaGame.AddMessage("WEST (current direction)");
                        MoveNinja(NinjaX, NinjaY - 1, map);

                        NinjaGame.PrintMap();
                        moved = true;
                    }

                    if (enteredMirrored && !_isMirrored)//not sure if we are supposed to stay in the same direction after exiting mirrored - but both should work
                        _currDirection = 3;
                    else
                        _currDirection = 0;

                    break;
            }
        }        
    }

    private void MoveNinja(int newX, int newY, char[,] map)
    {
        switch (map[newX, newY])
        {
            case '*':
                CollectShuriken(newX, newY, map);
                break;
            case 'F':
                SecretPath(newX, newY, map);
                break;
            case 'G':
                SecretPath(newX, newY, map);
                break;
            case 'H':
                SecretPath(newX, newY, map);
                break;
            case 'I':
                SecretPath(newX, newY, map);
                break;
            case 'J':
                SecretPath(newX, newY, map);
                break;
            case 'K':
                SecretPath(newX, newY, map);
                break;
            case 'L':
                SecretPath(newX, newY, map);
                break;
            case 'W':
                MoveWest(newX, newY, map);
                break;
            case 'S':
                MoveSouth(newX, newY, map);
                break;
            case 'N':
                MoveNorth(newX, newY, map);
                break;
            case 'E':
                MoveEast(newX, newY, map);
                break;
            case 'M':
                Mirror(newX, newY, map);
                break;
            case 'B':
                BreakerMode(newX, newY, map);
                break;
            case >= '0' and <= '9':
                BombLocation(newX, newY, map);
                break;
            default:
                
                ClearOutPreviousPositions(map);//This will ensure that the positions of certain obstacles do not get wiped out

                //Update ninja position
                NinjaX = newX;
                NinjaY = newY;
                // Place the ninja in the new position
                map[NinjaX, NinjaY] = '@';
                NinjaGame.PrintMap();
                NinjaGame.TrackPosition(NinjaX, NinjaY);

                if (NinjaGame.LoopingCheck())
                {
                    NinjaGame.AddMessage("LOOP");
                    throw new Exception("LOOP");
                }
                break;
        }
    }

    private void CollectShuriken(int newX, int newY, char[,] map)
    {
        _shurikenCount++;
        NinjaGame.AddMessage("Picks up shuriken");

        ClearOutPreviousPositions(map);

        NinjaX = newX;
        NinjaY = newY;
        map[NinjaX, NinjaY] = '@';
    }

    public void ThrowShuriken(char[,] map)
    {
        //we throw the Shuriken based on the specified movement (downwards,rightwards,upwards,leftwards)
        //check first if we have any left in order to throw one otherwise we just move
        if (_shurikenCount >= 1)
        {
            // 1. Check downwards on the same column
            for (int i = NinjaX + 1; i < map.GetLength(0); i++)
            {
                if ((map[i, NinjaY] == '$' || map[i, NinjaY] == 'X') && _shurikenCount >= 1)
                {
                    if (map[i, NinjaY] == 'X')
                    {
                        map[i, NinjaY] = '*';
                        _shurikenCount--;//decrease count if we hit a obstacle/symbol
                        NinjaGame.AddMessage("THROW (hit the X symbol downwards)");
                    }
                    else
                    {
                        map[i, NinjaY] = ' '; // Remove $
                        _shurikenCount--;//decrease count if we hit a obstacle/symbol
                        NinjaGame.AddMessage("THROW (hit the $ symbol downwards)");
                        _holySymbolCounter--;//decrease count of $ if 0 the ninja has won
                    }
                }
                else if (map[i, NinjaY] == '#')
                    break; // Hit a wall, stop
            }

            // 2. Check rightwards on the same row
            for (int j = NinjaY + 1; j < map.GetLength(1); j++)
            {
                if ((map[NinjaX, j] == '$' || map[NinjaX, j] == 'X') && _shurikenCount >= 1)
                {
                    if (map[NinjaX, j] == 'X')
                    {
                        map[NinjaX, j] = '*';
                        _shurikenCount--;//decrease count if we hit a obstacle/symbol
                        NinjaGame.AddMessage("THROW (hit the X symbol rightwards)");
                    }
                    else
                    {
                        map[NinjaX, j] = ' ';//Remove $
                        _shurikenCount--;//decrease count if we hit a obstacle/symbol
                        NinjaGame.AddMessage("THROW (hit the $ symbol rightwards)");
                        _holySymbolCounter--;
                    }
                }
                else if (map[NinjaX, j] == '#')
                    break; // Hit a wall, stop
            }

            // 3. Check upwards on the same column
            for (int i = NinjaX - 1; i >= 0; i--)
            {
                if ((map[i, NinjaY] == '$' || map[i, NinjaY] == 'X') && _shurikenCount >= 1)
                {
                    if (map[i, NinjaY] == 'X')
                    {
                        map[i, NinjaY] = '*';
                        _shurikenCount--;//decrease count if we hit a obstacle/symbol
                        NinjaGame.AddMessage("THROW (hit the X symbol upwards)");
                    }
                    else
                    {
                        map[i, NinjaY] = ' '; // Remove $
                        _shurikenCount--;//decrease count if we hit a obstacle/symbol
                        NinjaGame.AddMessage("THROW (hit the $ symbol upwards)");
                        _holySymbolCounter--;
                    }
                }
                else if (map[i, NinjaY] == '#')
                    break; // Hit a wall, stop
            }

            // 4. Check leftwards on the same row
            for (int j = NinjaY - 1; j >= 0; j--)
            {
                if ((map[NinjaX, j] == '$' || map[NinjaX, j] == 'X') && _shurikenCount >= 1)
                {
                    if (map[NinjaX, j] == 'X')
                    {
                        map[NinjaX, j] = '*';
                        _shurikenCount--;//decrease count if we hit a obstacle/symbol
                        NinjaGame.AddMessage("THROW (hit the X symbol leftwards)");
                    }
                    else
                    {
                        map[NinjaX, j] = ' '; // Remove $
                        _shurikenCount--;//decrease count if we hit a obstacle/symbol
                        NinjaGame.AddMessage("THROW (hit the $ symbol leftwards)");
                        _holySymbolCounter--;
                    }
                }
                else if (map[NinjaX, j] == '#')
                    break; // Hit a wall, stop
            }
        }
    }

    private void MoveWest(int currX, int currY, char[,] map)
    {
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                if ((map[i, j] == 'W') && (i == currX && j == currY) && (map[currX, currY] == map[i, j]))
                {
                    _currDir = 'W';

                    ClearOutPreviousPositions(map);

                    _prevDirChange = map[i, j];
                    NinjaX = i; // Update ninja's X position
                    NinjaY = j; // Update ninja's Y position
                    map[NinjaX, NinjaY] = '@'; // Set new position

                    NinjaGame.TrackPosition(NinjaX, NinjaY);

                    ThrowShuriken(map);
                    if (_holySymbolCounter == 0)
                    {
                        NinjaGame.AddMessage("All holy symbols destroyed");
                        NinjaGame.PrintMap();
                        return;
                    }
                    else if (_shurikenCount == 0 && _shurikensOnMap == 0 && _holySymbolCounter > 1)
                    {
                        NinjaGame.AddMessage("LOOP");
                        throw new Exception("LOOP");
                    }

                    while (IsValidMove(NinjaX, NinjaY - 1, map))
                    {
                        if (Bomb.IsBomb)//check if there is bomb on map otherwise we dont need to hit this code
                        {
                            bomb.CheckBombAndTriggerCountdown(NinjaX, NinjaY, Bomb.bombDics);//Checks if ninja is next to bomb
                            bomb.BombCountDownAndExplode(NinjaX, NinjaY, map);
                            NinjaGame.PrintMap();
                        }

                        MoveNinja(NinjaX, NinjaY - 1, map);
                        NinjaGame.PrintMap();

                        if (_currDir != 'W')
                            break;

                        NinjaGame.AddMessage("WEST because of W");
                    }

                    _currDir = '\0';
                    return;
                }
            }
        }
    }

    private void MoveEast(int currX, int currY, char[,] map)
    {
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                if ((map[i, j] == 'E') && (i == currX && j == currY) && (map[currX, currY] == map[i, j]))
                {
                    _currDir = 'E';

                    ClearOutPreviousPositions(map);

                    _prevDirChange = map[i, j];
                    NinjaX = i; // Update ninja's X position
                    NinjaY = j; // Update ninja's Y position
                    map[NinjaX, NinjaY] = '@'; // Set new position

                    NinjaGame.TrackPosition(NinjaX, NinjaY);

                    ThrowShuriken(map);
                    if (_holySymbolCounter == 0)
                    {
                        NinjaGame.AddMessage("All holy symbols destroyed");
                        NinjaGame.PrintMap();
                        return;
                    }
                    else if (_shurikenCount == 0 && _shurikensOnMap == 0 && _holySymbolCounter > 1)
                    {
                        NinjaGame.AddMessage("LOOP");
                        throw new Exception("LOOP");
                    }

                    while (IsValidMove(NinjaX, NinjaY + 1, map))
                    {
                        if (Bomb.IsBomb)//check if there is bomb on map otherwise we dont need to hit this code
                        {
                            bomb.CheckBombAndTriggerCountdown(NinjaX, NinjaY, Bomb.bombDics);//Checks if ninja is next to bomb
                            bomb.BombCountDownAndExplode(NinjaX, NinjaY, map);
                            NinjaGame.PrintMap();
                        }

                        MoveNinja(NinjaX, NinjaY + 1, map);
                        NinjaGame.PrintMap();

                        if (_currDir != 'E')
                            break;

                        NinjaGame.AddMessage("EAST because of E");
                    }

                    _currDir = '\0';
                    return;
                }
            }
        }
    }

    private void MoveSouth(int currX, int currY, char[,] map)
    {
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                if ((map[i, j] == 'S') && (i == currX && j == currY) && (map[currX, currY] == map[i, j]))
                {
                    _currDir = 'S';

                    ClearOutPreviousPositions(map);

                    _prevDirChange = map[i, j];
                    NinjaX = i; // Update ninja's X position
                    NinjaY = j; // Update ninja's Y position
                    map[NinjaX, NinjaY] = '@'; // Set new position

                    NinjaGame.TrackPosition(NinjaX, NinjaY);

                    ThrowShuriken(map);
                    if (_holySymbolCounter == 0)
                    {
                        NinjaGame.AddMessage("All holy symbols destroyed");
                        NinjaGame.PrintMap();
                        return;
                    }
                    else if (_shurikenCount == 0 && _shurikensOnMap == 0 && _holySymbolCounter > 1)
                    {
                        NinjaGame.AddMessage("LOOP");
                        throw new Exception("LOOP");
                    }

                    while (IsValidMove(NinjaX + 1, NinjaY, map))
                    {
                        if (Bomb.IsBomb)//check if there is bomb on map otherwise we dont need to hit this code
                        {
                            bomb.CheckBombAndTriggerCountdown(NinjaX, NinjaY, Bomb.bombDics);//Checks if ninja is next to bomb
                            bomb.BombCountDownAndExplode(NinjaX, NinjaY, map);
                            NinjaGame.PrintMap();
                        }

                        MoveNinja(NinjaX + 1, NinjaY, map);
                        NinjaGame.PrintMap();

                        if (_currDir != 'S')
                            break;

                        NinjaGame.AddMessage("SOUTH because of S");
                    }

                    _currDir = '\0';
                    return;
                }
            }
        }
    }

    private void MoveNorth(int currX, int currY, char[,] map)
    {
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                if ((map[i, j] == 'N') && (i == currX && j == currY) && (map[currX, currY] == map[i, j]))
                {
                    _currDir = 'N';

                    ClearOutPreviousPositions(map);

                    _prevDirChange = map[i, j];
                    NinjaX = i; // Update ninja's X position
                    NinjaY = j; // Update ninja's Y position
                    map[NinjaX, NinjaY] = '@'; // Set new position

                    NinjaGame.TrackPosition(NinjaX, NinjaY);

                    ThrowShuriken(map);
                    if (_holySymbolCounter == 0)
                    {
                        NinjaGame.AddMessage("All holy symbols destroyed");
                        NinjaGame.PrintMap();
                        return;
                    }
                    else if (_shurikenCount == 0 && _shurikensOnMap == 0 && _holySymbolCounter > 1)
                    {
                        NinjaGame.AddMessage("LOOP");
                        throw new Exception("LOOP");
                    }

                    while (IsValidMove(NinjaX - 1, NinjaY, map))
                    {
                        NinjaGame.PrintMap();
                        //check if there is bomb on map otherwise we dont need to hit this code
                        if (Bomb.IsBomb)
                        {
                            bomb.CheckBombAndTriggerCountdown(NinjaX, NinjaY, Bomb.bombDics);//Checks if ninja is next to bomb
                            bomb.BombCountDownAndExplode(NinjaX, NinjaY, map);
                            NinjaGame.PrintMap();
                        }
                        MoveNinja(NinjaX - 1, NinjaY, map);
                        NinjaGame.PrintMap();

                        if (_currDir != 'N')
                            break;

                        NinjaGame.AddMessage("NORTH because of N");
                    }

                    _currDir = '\0';
                    return;
                }
            }
        }
    }

    private void SecretPath(int currX, int currY, char[,] map)
    {
        // Find the other matching path to move to
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                if ((map[i, j] == 'F' || map[i, j] == 'G' || map[i, j] == 'H' || map[i, j] == 'I' || map[i, j] == 'J'
                    || map[i, j] == 'K' || map[i, j] == 'L') && (i != currX || j != currY) && (map[currX, currY] == map[i, j]))
                {

                    ClearOutPreviousPositions(map);

                    _prevSecretPath = map[i, j];
                    NinjaX = i; // Update ninja's X position
                    NinjaY = j; // Update ninja's Y position
                    map[NinjaX, NinjaY] = '@'; // Set new position
                    NinjaGame.TrackPosition(NinjaX, NinjaY);
                    NinjaGame.PrintMap();
                    return; // Exit once we move
                }
            }
        }
    }

    private void Mirror(int currX, int currY, char[,] map)
    {
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                if ((map[i, j] == 'M') && (i == currX && j == currY) && (map[currX, currY] == map[i, j]))
                {
                    if (_isMirrored)
                        _isMirrored = false;
                    else
                        _isMirrored = true;

                    ClearOutPreviousPositions(map);

                    _prevMirror = map[i, j];
                    NinjaX = i; // Update ninja's X position
                    NinjaY = j; // Update ninja's Y position
                    map[NinjaX, NinjaY] = '@'; // Set new position
                    NinjaGame.TrackPosition(NinjaX, NinjaY);
                    NinjaGame.PrintMap();
                    return;
                }
            }
        }
    }

    private void BreakerMode(int currX, int currY, char[,] map)
    {
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                if ((map[i, j] == 'B') && (i == currX && j == currY) && (map[currX, currY] == map[i, j]))
                {
                    if (_inBreakerMode)
                    {
                        _inBreakerMode = false;
                        NinjaGame.AddMessage("Out of BreakerMode");
                    }
                    else
                    {
                        _inBreakerMode = true;
                        NinjaGame.AddMessage("In BreakerMode");
                    }

                    ClearOutPreviousPositions(map);

                    NinjaX = i; // Update ninja's X position
                    NinjaY = j; // Update ninja's Y position
                    map[NinjaX, NinjaY] = '@'; // Set new position

                    return;
                }
            }
        }        
    }

    private void BombLocation(int currX, int currY, char[,] map)
    {
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                if ((map[i, j] >= '1' && map[i, j] <= '9') && (i == currX && j == currY))
                {

                    ClearOutPreviousPositions(map);

                    _prevBomb = map[i, j];
                    NinjaX = i; // Update ninja's X position
                    NinjaY = j; // Update ninja's Y position
                    map[NinjaX, NinjaY] = '@'; // Set new position
                    NinjaGame.PrintMap();
                    return;
                }
            }
        }
    }

    private void ClearOutPreviousPositions(char[,] map)
    {
        if (_prevDirChange != '\0')
        {
            map[NinjaX, NinjaY] = _prevDirChange;
            _prevDirChange = '\0';
        }
        else if (_prevSecretPath != '\0')
        {
            map[NinjaX, NinjaY] = _prevSecretPath;
            _prevSecretPath = '\0';
        }
        else if (_prevMirror != '\0')
        {
            map[NinjaX, NinjaY] = _prevMirror;
            _prevMirror = '\0';
        }
        else if (_prevBomb != '\0')
        {
            map[NinjaX, NinjaY] = _prevBomb;
            _prevBomb = '\0';
        }
        else
            map[NinjaX, NinjaY] = ' ';

        NinjaGame.PrintMap();
    }
}


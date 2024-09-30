using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class Ninja
{
    public int _ninjaX { get; set; }
    public int _ninjaY { get; set; }

    public int _holySymbolCounter { get; set; }
    public int _shurikensOnMap { get; set; }
    public int _shurikenCount { get; private set; }

    public static int _currDirection = 0;
    public static char _prevSecretPath = '\0';
    public static char _prevDirChange = '\0';
    public static char _currDir = '\0';
    public static char _prevMirror = '\0';
    public static bool _isMirrored = false;
    public static bool _inBreakerMode = false;

    public Ninja(int startX, int startY)
    {
        _ninjaX = startX;
        _ninjaY = startY;
        _shurikenCount = 3;//starting limit
    }

    public bool IsValidMove(int x, int y, char[,] map)
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
                    while (IsValidMove(_ninjaX + 1, _ninjaY, map) || (_isMirrored && IsValidMove(_ninjaX, _ninjaY - 1, map)))
                    {
                        if (_isMirrored)
                        {
                            enteredMirrored = true;
                            while (IsValidMove(_ninjaX, _ninjaY - 1, map))//west
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

                                NinjaGame.AddMessage("Mirrored WEST (current direction)");
                                MovePlayer(_ninjaX, _ninjaY - 1, map);

                                if (!_isMirrored)//we need to check if we are still mirrored since in the MovePlayer() we can get out of it, if so break out of the mirror
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

                        NinjaGame.AddMessage("SOUTH (initial direction)");
                        MovePlayer(_ninjaX + 1, _ninjaY, map);
                        NinjaGame.PrintMap();
                        moved = true;
                    }

                    if (enteredMirrored && !_isMirrored)//not sure if we are supposed to stay in the same direction after exiting mirrored - but both should work
                        _currDirection = 0;
                    else
                        _currDirection = 1;

                    break;
                case 1://east             
                    while (IsValidMove(_ninjaX, _ninjaY + 1, map) || (_isMirrored && IsValidMove(_ninjaX - 1, _ninjaY, map)))
                    {
                        if (_isMirrored)
                        {
                            enteredMirrored = true;

                            while (IsValidMove(_ninjaX - 1, _ninjaY, map))//north
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

                                NinjaGame.AddMessage("Mirrored NORTH (current direction)");
                                MovePlayer(_ninjaX - 1, _ninjaY, map);

                                if (!_isMirrored)//we need to check if we are still mirrored since in the MovePlayer() we can get out of it, if so break out of the mirror
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

                        NinjaGame.AddMessage("EAST (current direction)");
                        MovePlayer(_ninjaX, _ninjaY + 1, map);
                        NinjaGame.PrintMap();
                        moved = true;
                    }

                    if (enteredMirrored && !_isMirrored)//not sure if we are supposed to stay in the same direction after exiting mirrored - but both should work
                        _currDirection = 1;
                    else
                        _currDirection = 2;

                    break;
                case 2://north
                    while (IsValidMove(_ninjaX - 1, _ninjaY, map) || (_isMirrored && IsValidMove(_ninjaX, _ninjaY + 1, map)))
                    {
                        if (_isMirrored)
                        {
                            enteredMirrored = true;

                            while (IsValidMove(_ninjaX, _ninjaY + 1, map))//east
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

                                NinjaGame.AddMessage("Mirrored EAST (current direction)");
                                MovePlayer(_ninjaX, _ninjaY + 1, map);

                                if (!_isMirrored)//we need to check if we are still mirrored since in the MovePlayer() we can get out of it, if so break out of the mirror
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

                        NinjaGame.AddMessage("NORTH (current direction)");
                        MovePlayer(_ninjaX - 1, _ninjaY, map);
                        NinjaGame.PrintMap();
                        moved = true;
                    }
                    if (enteredMirrored && !_isMirrored)//not sure if we are supposed to stay in the same direction after exiting mirrored - but both should work
                        _currDirection = 2;
                    else
                        _currDirection = 3;

                    break;
                case 3://west
                    while (IsValidMove(_ninjaX, _ninjaY - 1, map) || (_isMirrored && IsValidMove(_ninjaX + 1, _ninjaY, map)))
                    {
                        if (_isMirrored)
                        {
                            enteredMirrored = true;

                            while (IsValidMove(_ninjaX + 1, _ninjaY, map))//south
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

                                NinjaGame.AddMessage("Mirrored SOUTH (current direction)");
                                MovePlayer(_ninjaX + 1, _ninjaY, map);

                                if (!_isMirrored)//we need to check if we are still mirrored since in the MovePlayer() we can get out of it, if so break out of the mirror
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

                        NinjaGame.AddMessage("WEST (current direction)");
                        MovePlayer(_ninjaX, _ninjaY - 1, map);
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

    public void MovePlayer(int newX, int newY, char[,] map)
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
            default:
                // Replace the player's old position with a space if there was no secret path
                if (_prevSecretPath != '\0')
                {
                    map[_ninjaX, _ninjaY] = _prevSecretPath;
                    _prevSecretPath = '\0';
                }
                else if (_prevDirChange != '\0') //Replace the direction position
                {
                    map[_ninjaX, _ninjaY] = _prevDirChange;
                    _prevDirChange = '\0';
                }
                else if (_prevMirror != '\0') //Replace the direction position
                {
                    map[_ninjaX, _ninjaY] = _prevMirror;
                    _prevMirror = '\0';
                }
                else
                    map[_ninjaX, _ninjaY] = ' ';

                //Update player position
                _ninjaX = newX;
                _ninjaY = newY;
                // Place the player in the new position
                map[_ninjaX, _ninjaY] = '@';
                NinjaGame.PrintMap();
                NinjaGame.TrackPosition(_ninjaX, _ninjaY);

                if (NinjaGame.LoopingCheck())
                {
                    NinjaGame.AddMessage("LOOP");
                    throw new Exception("LOOP");
                }
                break;
        }
    }

    public void CollectShuriken(int newX, int newY, char[,] map)
    {
        _shurikenCount++;
        NinjaGame.AddMessage("Picks up shuriken");
        if (_prevDirChange != '\0')
        {
            map[_ninjaX, _ninjaY] = _prevDirChange;
            _prevDirChange = '\0';
        }
        else if (_prevSecretPath != '\0')
        {
            map[_ninjaX, _ninjaY] = _prevSecretPath;
            _prevSecretPath = '\0';
        }
        else
            map[_ninjaX, _ninjaY] = ' ';

        _ninjaX = newX;
        _ninjaY = newY;
        map[_ninjaX, _ninjaY] = '@';
    }

    public void ThrowShuriken(char[,] map)
    {
        //we throw the Shuriken based on the specified movement (downwards,rightwards,upwards,leftwards)
        //check first if we have any left in order to throw one otherwise we just move
        if (_shurikenCount >= 1)
        {
            // 1. Check downwards on the same column
            for (int i = _ninjaX + 1; i < map.GetLength(0); i++)
            {
                if ((map[i, _ninjaY] == '$' || map[i, _ninjaY] == 'X') && _shurikenCount >= 1)
                {
                    if (map[i, _ninjaY] == 'X')
                    {
                        map[i, _ninjaY] = '*';
                        _shurikenCount--;//decrease count if we hit a obstacle/symbol
                        NinjaGame.AddMessage("THROW (hit the X symbol downwards)");
                    }
                    else
                    {
                        map[i, _ninjaY] = ' '; // Remove $
                        _shurikenCount--;//decrease count if we hit a obstacle/symbol
                        NinjaGame.AddMessage("THROW (hit the $ symbol downwards)");
                        _holySymbolCounter--;//decrease count of $ if 0 the ninja has won
                    }
                }
                else if (map[i, _ninjaY] == '#')
                    break; // Hit a wall, stop
            }

            // 2. Check rightwards on the same row
            for (int j = _ninjaY + 1; j < map.GetLength(1); j++)
            {
                if ((map[_ninjaX, j] == '$' || map[_ninjaX, j] == 'X') && _shurikenCount >= 1)
                {
                    if (map[_ninjaX, j] == 'X')
                    {
                        map[_ninjaX, j] = '*';
                        _shurikenCount--;//decrease count if we hit a obstacle/symbol
                        NinjaGame.AddMessage("THROW (hit the X symbol rightwards)");
                    }
                    else
                    {
                        map[_ninjaX, j] = ' ';//Remove $
                        _shurikenCount--;//decrease count if we hit a obstacle/symbol
                        NinjaGame.AddMessage("THROW (hit the $ symbol rightwards)");
                        _holySymbolCounter--;
                    }
                }
                else if (map[_ninjaX, j] == '#')
                    break; // Hit a wall, stop
            }

            // 3. Check upwards on the same column
            for (int i = _ninjaX - 1; i >= 0; i--)
            {
                if ((map[i, _ninjaY] == '$' || map[i, _ninjaY] == 'X') && _shurikenCount >= 1)
                {
                    if (map[i, _ninjaY] == 'X')
                    {
                        map[i, _ninjaY] = '*';
                        _shurikenCount--;//decrease count if we hit a obstacle/symbol
                        NinjaGame.AddMessage("THROW (hit the X symbol upwards)");
                    }
                    else
                    {
                        map[i, _ninjaY] = ' '; // Remove $
                        _shurikenCount--;//decrease count if we hit a obstacle/symbol
                        NinjaGame.AddMessage("THROW (hit the $ symbol upwards)");
                        _holySymbolCounter--;
                    }
                }
                else if (map[i, _ninjaY] == '#')
                    break; // Hit a wall, stop
            }

            // 4. Check leftwards on the same row
            for (int j = _ninjaY - 1; j >= 0; j--)
            {
                if ((map[_ninjaX, j] == '$' || map[_ninjaX, j] == 'X') && _shurikenCount >= 1)
                {
                    if (map[_ninjaX, j] == 'X')
                    {
                        map[_ninjaX, j] = '*';
                        _shurikenCount--;//decrease count if we hit a obstacle/symbol
                        NinjaGame.AddMessage("THROW (hit the X symbol leftwards)");
                    }
                    else
                    {
                        map[_ninjaX, j] = ' '; // Remove $
                        _shurikenCount--;//decrease count if we hit a obstacle/symbol
                        NinjaGame.AddMessage("THROW (hit the $ symbol leftwards)");
                        _holySymbolCounter--;
                    }
                }
                else if (map[_ninjaX, j] == '#')
                    break; // Hit a wall, stop
            }
        }
    }

    public void MoveWest(int currX, int currY, char[,] map)
    {
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                if ((map[i, j] == 'W') && (i == currX && j == currY) && (map[currX, currY] == map[i, j]))
                {
                    _currDir = 'W';

                    if (_prevDirChange != '\0')// need to check if prevDirChange was not empty. we want to make sure to keep the original position
                        map[_ninjaX, _ninjaY] = _prevDirChange;
                    else
                        map[_ninjaX, _ninjaY] = ' '; // Clear old position
                    _prevDirChange = map[i, j];
                    _ninjaX = i; // Update player's X position
                    _ninjaY = j; // Update player's Y position
                    map[_ninjaX, _ninjaY] = '@'; // Set new position

                    NinjaGame.TrackPosition(_ninjaX, _ninjaY);

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

                    while (IsValidMove(_ninjaX, _ninjaY - 1, map))
                    {
                        MovePlayer(_ninjaX, _ninjaY - 1, map);
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

    public void MoveEast(int currX, int currY, char[,] map)
    {
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                if ((map[i, j] == 'E') && (i == currX && j == currY) && (map[currX, currY] == map[i, j]))
                {
                    _currDir = 'E';

                    if (_prevDirChange != '\0')// need to check if prevDirChange was not empty. we want to make sure to keep the original position
                        map[_ninjaX, _ninjaY] = _prevDirChange;
                    else
                        map[_ninjaX, _ninjaY] = ' '; // Clear old position

                    _prevDirChange = map[i, j];
                    _ninjaX = i; // Update player's X position
                    _ninjaY = j; // Update player's Y position
                    map[_ninjaX, _ninjaY] = '@'; // Set new position

                    NinjaGame.TrackPosition(_ninjaX, _ninjaY);

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

                    while (IsValidMove(_ninjaX, _ninjaY + 1, map))
                    {
                        MovePlayer(_ninjaX, _ninjaY + 1, map);
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

    public void MoveSouth(int currX, int currY, char[,] map)
    {
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                if ((map[i, j] == 'S') && (i == currX && j == currY) && (map[currX, currY] == map[i, j]))
                {
                    _currDir = 'S';

                    if (_prevDirChange != '\0')// need to check if prevDirChange was not empty. we want to make sure to keep the original position
                        map[_ninjaX, _ninjaY] = _prevDirChange;
                    else
                        map[_ninjaX, _ninjaY] = ' '; // Clear old position
                    _prevDirChange = map[i, j];
                    _ninjaX = i; // Update player's X position
                    _ninjaY = j; // Update player's Y position
                    map[_ninjaX, _ninjaY] = '@'; // Set new position

                    NinjaGame.TrackPosition(_ninjaX, _ninjaY);

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

                    while (IsValidMove(_ninjaX + 1, _ninjaY, map))
                    {
                        MovePlayer(_ninjaX + 1, _ninjaY, map);
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

    public void MoveNorth(int currX, int currY, char[,] map)
    {
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                if ((map[i, j] == 'N') && (i == currX && j == currY) && (map[currX, currY] == map[i, j]))
                {
                    _currDir = 'N';

                    if (_prevDirChange != '\0')// need to check if prevDirChange was not empty. we want to make sure to keep the original position
                        map[_ninjaX, _ninjaY] = _prevDirChange;
                    else
                        map[_ninjaX, _ninjaY] = ' '; // Clear old position

                    _prevDirChange = map[i, j];
                    _ninjaX = i; // Update player's X position
                    _ninjaY = j; // Update player's Y position
                    map[_ninjaX, _ninjaY] = '@'; // Set new position

                    NinjaGame.TrackPosition(_ninjaX, _ninjaY);

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

                    while (IsValidMove(_ninjaX - 1, _ninjaY, map))
                    {
                        MovePlayer(_ninjaX - 1, _ninjaY, map);
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

    public void SecretPath(int currX, int currY, char[,] map)
    {
        // Find the other matching path to move to
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                if ((map[i, j] == 'F' || map[i, j] == 'G' || map[i, j] == 'H' || map[i, j] == 'I' || map[i, j] == 'J'
                    || map[i, j] == 'K' || map[i, j] == 'L') && (i != currX || j != currY) && (map[currX, currY] == map[i, j]))
                {
                    if (_prevSecretPath != '\0')// need to check if prevSecretPath was not empty. we want to make sure to keep the original position of the path
                        map[_ninjaX, _ninjaY] = _prevSecretPath;
                    else if (_prevDirChange != '\0')
                    {
                        map[_ninjaX, _ninjaY] = _prevDirChange;
                        _prevDirChange = '\0';
                    }
                    else
                        map[_ninjaX, _ninjaY] = ' '; // Clear old position

                    _prevSecretPath = map[i, j];
                    _ninjaX = i; // Update player's X position
                    _ninjaY = j; // Update player's Y position
                    map[_ninjaX, _ninjaY] = '@'; // Set new position
                    NinjaGame.TrackPosition(_ninjaX, _ninjaY);
                    return; // Exit once we move
                }
            }
        }
    }

    public void Mirror(int currX, int currY, char[,] map)
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

                    if (_prevMirror != '\0')// need to check if prevSecretPath was not empty. we want to make sure to keep the original position of the path
                        map[_ninjaX, _ninjaY] = _prevMirror;
                    else if (_prevSecretPath != '\0')
                    {
                        map[_ninjaX, _ninjaY] = _prevSecretPath;
                        _prevSecretPath = '\0';
                    }
                    else
                        map[_ninjaX, _ninjaY] = ' '; // Clear old position

                    _prevMirror = map[i, j];
                    _ninjaX = i; // Update player's X position
                    _ninjaY = j; // Update player's Y position
                    map[_ninjaX, _ninjaY] = '@'; // Set new position
                    NinjaGame.TrackPosition(_ninjaX, _ninjaY);

                    return;
                }
            }
        }
    }

    public void BreakerMode(int currX, int currY, char[,] map)
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

                    if (_prevDirChange != '\0')
                    {
                        map[_ninjaX, _ninjaY] = _prevDirChange;
                        _prevDirChange = '\0';
                    }
                    else if (_prevSecretPath != '\0')
                    {
                        map[_ninjaX, _ninjaY] = _prevSecretPath;
                        _prevSecretPath = '\0';
                    }
                    else if (_prevMirror != '\0')
                    {
                        map[_ninjaX, _ninjaY] = _prevMirror;
                        _prevMirror = '\0';
                    }
                    else
                        map[_ninjaX, _ninjaY] = ' ';

                    _ninjaX = i; // Update player's X position
                    _ninjaY = j; // Update player's Y position
                    map[_ninjaX, _ninjaY] = '@'; // Set new position

                    return;
                }
            }
        }
    }
}


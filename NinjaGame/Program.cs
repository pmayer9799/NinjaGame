using System.Numerics;

class NinjaGame
{
    //globals
    static char[,] _map;
    static int _mapWidth;
    static int _mapHeight = 5;
    static int _ninjaX;
    static int _ninjaY;
    static int _shurikenCount = 3;//starting limit
    static char _prevSecretPath = '\0';
    static char _prevDirChange = '\0';
    static char _currDir = '\0';
    static char _prevMirror = '\0';
    static int _currDirection = 0;
    static bool _isMirrored = false;
    static int _messageLine = 0;

    static void Main(string[] args)
    { //we will use the main loop to call the actual game to keep the Main() method clean
        Load_map("map3.txt");
        _messageLine = _mapHeight;
        GameLoop();
    }

    static void Load_map(string fileName)
    {
        var lines = File.ReadAllLines(fileName);//read the file that has the _map for the game
        _map = new char[lines.Length, lines[0].Length];

        for (int i = 0; i < lines.Length; i++)
        {
            for (int j = 0; j < lines[i].Length; j++)
            {
                _map[i, j] = lines[i][j];

                if (_map[i, j] == 'P')
                {
                    _ninjaX = i; // Store the row (i) where 'P' is found
                    _ninjaY = j; // Store the column (j) where 'P' is found
                }
            }
        }
    }  

    static void GameLoop()
    {
        while (true)
        {
            PrintMap();//need to print map after each change
            Console.SetCursorPosition(0, _mapHeight);
            ThrowShuriken();//we always have the option to throw shuriken if we have any
            Movement();
            ClearMapArea();
        }
    }

    static void PrintMap()
    {
        Console.SetCursorPosition(0, 0);
        for (int i = 0; i < _map.GetLength(0); i++)
        {
            for (int j = 0; j < _map.GetLength(1); j++)
            {
                Console.Write(_map[i, j]);
            }
            Console.WriteLine();
        }
    }    

    static void MovePlayer(int newX, int newY)
    {
        switch(_map[newX, newY])
        {
            case '*':
                _shurikenCount++;
                AddMessage("Picks up shiriken");
                if (_prevDirChange != '\0')
                {
                    _map[_ninjaX, _ninjaY] = _prevDirChange;
                    _prevDirChange = '\0';
                }
                else if(_prevSecretPath != '\0')
                {
                    _map[_ninjaX, _ninjaY] = _prevSecretPath;
                    _prevSecretPath = '\0';
                }
                else
                    _map[_ninjaX, _ninjaY] = ' ';
                _ninjaX = newX;
                _ninjaY = newY;
                _map[_ninjaX, _ninjaY] = 'P';
                break;
            case 'F':
                SecretPath(newX, newY);
                break;
            case 'G':
                SecretPath(newX, newY);
                break;
            case 'H':
                SecretPath(newX, newY);
                break;
            case 'I':
                SecretPath(newX, newY);
                break;
            case 'j':
                SecretPath(newX, newY);
                break;
            case 'K':
                SecretPath(newX, newY);
                break;
            case 'L':
                SecretPath(newX, newY);
                break;
            case 'W':
                MoveWest(newX, newY);
                break;
            case 'S':
                MoveSouth(newX, newY);
                break;
            case 'N':
                MoveNorth(newX, newY);
                break;
            case 'E':
                MoveEast(newX, newY);
                break;
            case 'M':
                Mirror(newX, newY);
                break;
            default:
                // Replace the player's old position with a space if there was no secret path
                if (_prevSecretPath != '\0')
                {
                    _map[_ninjaX, _ninjaY] = _prevSecretPath;
                    _prevSecretPath = '\0';
                }
                else if (_prevDirChange != '\0') //Replace the direction position
                {
                    _map[_ninjaX, _ninjaY] = _prevDirChange;
                    _prevDirChange = '\0';
                }
                else if (_prevMirror != '\0') //Replace the direction position
                {
                    _map[_ninjaX, _ninjaY] = _prevMirror;
                    _prevMirror = '\0';
                }
                else
                    _map[_ninjaX, _ninjaY] = ' ';
                //Update player position
                _ninjaX = newX;
                _ninjaY = newY;
                // Place the player in the new position
                _map[_ninjaX, _ninjaY] = 'P';
                break;
        }        
    }     

    static void ThrowShuriken()
    {
        //we throw the Shuriken based on the specified movement (downwards,rightwards,upwards,leftwards)
        //check first if we have any left in order to throw one otherwise we just move
        if (_shurikenCount >= 1)
        {
            // 1. Check downwards on the same column
            for (int i = _ninjaX + 1; i < _map.GetLength(0); i++)
            {
                if ((_map[i, _ninjaY] == '$' || _map[i, _ninjaY] == 'X') && _shurikenCount >= 1)
                {
                    if (_map[i, _ninjaY] == 'X')
                    {
                        _map[i, _ninjaY] = '*';
                        _shurikenCount--;//decrease count if we hit a obstacle/symbol
                        Console.SetCursorPosition(0, _mapHeight++);
                        AddMessage("THROW (hit the X symbol downwards)");
                    }
                    else
                    {
                        _map[i, _ninjaY] = ' '; // Remove $,X
                        _shurikenCount--;//decrease count if we hit a obstacle/symbol
                        AddMessage("THROW (hit the $ symbol downwards)");
                    }
                }
                else if (_map[i, _ninjaY] == '#')
                    break; // Hit a wall, stop
            }

            // 2. Check rightwards on the same row
            for (int j = _ninjaY + 1; j < _map.GetLength(1); j++)
            {
                if ((_map[_ninjaX, j] == '$' || _map[_ninjaX, j] == 'X') && _shurikenCount >= 1)
                {
                    if(_map[_ninjaX, j] == 'X')
                    {
                        _map[_ninjaX, j] = '*';
                        _shurikenCount--;//decrease count if we hit a obstacle/symbol
                        Console.SetCursorPosition(0, _mapHeight++);
                        AddMessage("THROW (hit the X symbol rightwards)");
                    }
                    else
                    {
                        _map[_ninjaX, j] = ' ';//Remove $,X
                        _shurikenCount--;//decrease count if we hit a obstacle/symbol
                        AddMessage("THROW (hit the $ symbol rightwards)");
                    }
                }
                else if (_map[_ninjaX, j] == '#')
                    break; // Hit a wall, stop
            }

            // 3. Check upwards on the same column
            for (int i = _ninjaX - 1; i >= 0; i--)
            {
                if ((_map[i, _ninjaY] == '$' || _map[i, _ninjaY] == 'X') && _shurikenCount >= 1)
                {
                    if (_map[i, _ninjaY] == 'X')
                    {
                        _map[i, _ninjaY] = '*';
                        _shurikenCount--;//decrease count if we hit a obstacle/symbol
                        AddMessage("THROW (hit the X symbol upwards)");
                    }
                    else
                    {
                        _map[i, _ninjaY] = ' '; // Remove $,X
                        _shurikenCount--;//decrease count if we hit a obstacle/symbol
                        AddMessage("THROW (hit the $ symbol upwards)");
                    }
                }
                else if (_map[i, _ninjaY] == '#')
                    break; // Hit a wall, stop
            }

            // 4. Check leftwards on the same row
            for (int j = _ninjaY - 1; j >= 0; j--)
            {
                if ((_map[_ninjaX, j] == '$' || _map[_ninjaX, j] == 'X') && _shurikenCount >= 1)
                {
                    if(_map[_ninjaX, j] == 'X')
                    {
                        _map[_ninjaX, j] = '*';
                        _shurikenCount--;//decrease count if we hit a obstacle/symbol
                        AddMessage("THROW (hit the X symbol leftwards)");
                    }
                    else
                    {
                        _map[_ninjaX, j] = ' '; // Remove $,X
                        _shurikenCount--;//decrease count if we hit a obstacle/symbol
                        AddMessage("THROW (hit the $ symbol leftwards)");
                    }
                }
                else if (_map[_ninjaX, j] == '#')
                    break; // Hit a wall, stop
            }
        }
    }

    static void MoveWest(int currX, int currY)
    {
        for (int i = 0; i < _map.GetLength(0); i++)
        {
            for (int j = 0; j < _map.GetLength(1); j++)
            {
                if ((_map[i, j] == 'W') && (i == currX && j == currY) && (_map[currX, currY] == _map[i, j]))
                {
                    if (_prevDirChange != '\0')// need to check if prevDirChange was not empty. we want to make sure to keep the original position
                        _map[_ninjaX, _ninjaY] = _prevDirChange;
                    else
                        _map[_ninjaX, _ninjaY] = ' '; // Clear old position
                    _prevDirChange = _map[i, j];
                    _ninjaX = i; // Update player's X position
                    _ninjaY = j; // Update player's Y position
                    _map[_ninjaX, _ninjaY] = 'P'; // Set new position
                    ThrowShuriken();
                    if (IsValidMove(_ninjaX, _ninjaY - 1))
                    {                        
                        MovePlayer(_ninjaX, _ninjaY - 1);                        
                    }
                    return;
                }
            }
        }
    }

    static void MoveEast(int currX, int currY)
    {
        for (int i = 0; i < _map.GetLength(0); i++)
        {
            for (int j = 0; j < _map.GetLength(1); j++)
            {
                if ((_map[i, j] == 'E') && (i == currX && j == currY) && (_map[currX, currY] == _map[i, j]))
                {
                    _currDir = 'E';

                    if (_prevDirChange != '\0')// need to check if prevDirChange was not empty. we want to make sure to keep the original position
                        _map[_ninjaX, _ninjaY] = _prevDirChange;
                    else
                        _map[_ninjaX, _ninjaY] = ' '; // Clear old position

                    _prevDirChange = _map[i, j];
                    _ninjaX = i; // Update player's X position
                    _ninjaY = j; // Update player's Y position
                    _map[_ninjaX, _ninjaY] = 'P'; // Set new position

                    ThrowShuriken();
                    while (IsValidMove(_ninjaX, _ninjaY + 1))//TODO: check if after a N there is a E then w
                    {
                        MovePlayer(_ninjaX, _ninjaY + 1);
                        AddMessage("EAST because of E");
                        if (_currDir != 'E')
                            break;
                    }
                    return;
                }
            }
        }
    }
    
    static void MoveSouth(int currX, int currY)
    {
        for (int i = 0; i < _map.GetLength(0); i++)
        {
            for (int j = 0; j < _map.GetLength(1); j++)
            {
                if ((_map[i, j] == 'S') && (i == currX && j == currY) && (_map[currX, currY] == _map[i, j]))
                {
                    if (_prevDirChange != '\0')// need to check if prevDirChange was not empty. we want to make sure to keep the original position
                        _map[_ninjaX, _ninjaY] = _prevDirChange;
                    else
                        _map[_ninjaX, _ninjaY] = ' '; // Clear old position
                    _prevDirChange = _map[i, j];
                    _ninjaX = i; // Update player's X position
                    _ninjaY = j; // Update player's Y position
                    _map[_ninjaX, _ninjaY] = 'P'; // Set new position

                    ThrowShuriken();
                    if (IsValidMove(_ninjaX + 1, _ninjaY))
                    {
                        MovePlayer(_ninjaX + 1, _ninjaY);
                    }
                    return;
                }
            }
        }
    }

    static void MoveNorth(int currX, int currY)
    {
        for (int i = 0; i < _map.GetLength(0); i++)
        {
            for (int j = 0; j < _map.GetLength(1); j++)
            {
                if ((_map[i, j] == 'N') && (i == currX && j == currY) && (_map[currX, currY] == _map[i, j]))
                {
                    _currDir = 'N';

                    if (_prevDirChange != '\0')// need to check if prevDirChange was not empty. we want to make sure to keep the original position
                        _map[_ninjaX, _ninjaY] = _prevDirChange;
                    else
                        _map[_ninjaX, _ninjaY] = ' '; // Clear old position

                    _prevDirChange = _map[i, j];
                    _ninjaX = i; // Update player's X position
                    _ninjaY = j; // Update player's Y position
                    _map[_ninjaX, _ninjaY] = 'P'; // Set new position

                    ThrowShuriken();
                    while (IsValidMove(_ninjaX - 1, _ninjaY))
                    {
                        MovePlayer(_ninjaX - 1, _ninjaY);
                        AddMessage("NORTH because of N");
                        if (_currDir != 'N')
                            break;
                    }
                    return;
                }
            }
        }
    }

    static bool IsValidMove(int x, int y)
    {
        // Check for unbreakables
        return x >= 0 && x < _map.GetLength(0) &&
               y >= 0 && y < _map.GetLength(1) &&
               _map[x, y] != '#' && _map[x, y] != 'X' &&
               _map[x, y] != '$';
    }

    static void Movement()
    {
        bool moved = false;
        
        while(!moved)
        {
            switch (_currDirection)
            {
                case 0://south
                    while(IsValidMove(_ninjaX + 1, _ninjaY))
                    {
                        if (_isMirrored)
                        {
                            while (IsValidMove(_ninjaX, _ninjaY - 1))//west
                            {
                                ThrowShuriken();
                                MovePlayer(_ninjaX, _ninjaY - 1);
                                moved = true;                                
                            }
                            break;
                        }
                        ThrowShuriken();
                        MovePlayer(_ninjaX + 1, _ninjaY);
                        AddMessage("SOUTH (initial direction)");
                        moved = true;
                    }
                    _currDirection = 1;
                    break;
                case 1://east             
                    while (IsValidMove(_ninjaX, _ninjaY + 1))
                    {                        
                        if (_isMirrored)
                        {
                            while (IsValidMove(_ninjaX - 1, _ninjaY))//north
                            {
                                ThrowShuriken();
                                MovePlayer(_ninjaX - 1, _ninjaY);
                                moved = true;
                            }
                            break;
                        }                        
                        ThrowShuriken();
                        MovePlayer(_ninjaX, _ninjaY + 1);
                        AddMessage("EAST (current direction)");
                        moved = true;
                    }
                    _currDirection = 2;
                    break;
                case 2://north
                    while (IsValidMove(_ninjaX - 1, _ninjaY))
                    {
                        if (_isMirrored)
                        {
                            while (IsValidMove(_ninjaX, _ninjaY + 1))//east
                            {
                                ThrowShuriken();
                                MovePlayer(_ninjaX, _ninjaY + 1);
                                moved = true;
                            }
                            break;
                        }
                        ThrowShuriken();
                        MovePlayer(_ninjaX - 1, _ninjaY);
                        AddMessage("NORTH (current direction)");
                        moved = true;
                    }
                    _currDirection = 3;
                    break;
                case 3://west
                    while (IsValidMove(_ninjaX, _ninjaY - 1))
                    {
                        if (_isMirrored)
                        {
                            while (IsValidMove(_ninjaX + 1, _ninjaY))//south
                            {
                                ThrowShuriken();
                                MovePlayer(_ninjaX + 1, _ninjaY);
                                moved = true;
                            }
                            break;
                        }
                        ThrowShuriken();
                        MovePlayer(_ninjaX, _ninjaY - 1);
                        AddMessage("WEST (current direction)");
                        moved = true;
                    }                    
                    _currDirection = 0;
                    break;
            }
        }
    }

    static void SecretPath(int currX, int currY)
    {        
        // Find the other matching path to move to
        for (int i = 0; i < _map.GetLength(0); i++)
        {
            for (int j = 0; j < _map.GetLength(1); j++)
            {
                if ((_map[i, j] == 'F' || _map[i, j] == 'G' || _map[i, j] == 'H' || _map[i, j] == 'I' || _map[i, j] == 'J' 
                    || _map[i, j] == 'K' || _map[i, j] == 'L') && (i != currX || j != currY) && (_map[currX, currY] == _map[i,j]))
                {                    
                    if (_prevSecretPath != '\0')// need to check if prevSecretPath was not empty. we want to make sure to keep the original position of the path
                        _map[_ninjaX, _ninjaY] = _prevSecretPath;
                    else
                        _map[_ninjaX, _ninjaY] = ' '; // Clear old position
                    _prevSecretPath = _map[i, j];
                    _ninjaX = i; // Update player's X position
                    _ninjaY = j; // Update player's Y position
                    _map[_ninjaX, _ninjaY] = 'P'; // Set new position
                    return; // Exit once we move
                }
            }
        }
    }

    static void Mirror(int currX, int currY)
    {
        for (int i = 0; i < _map.GetLength(0); i++)
        {
            for (int j = 0; j < _map.GetLength(1); j++)
            {
                if ((_map[i, j] == 'M') && (i == currX && j == currY) && (_map[currX, currY] == _map[i, j]))
                {
                    if (_isMirrored)
                        _isMirrored = false;
                    else
                        _isMirrored = true;

                    if (_prevMirror != '\0')// need to check if prevSecretPath was not empty. we want to make sure to keep the original position of the path
                        _map[_ninjaX, _ninjaY] = _prevMirror;
                    else
                        _map[_ninjaX, _ninjaY] = ' '; // Clear old position
                    _prevMirror = _map[i, j];
                    _ninjaX = i; // Update player's X position
                    _ninjaY = j; // Update player's Y position
                    _map[_ninjaX, _ninjaY] = 'P'; // Set new position
                    return;
                }
            }
        }
    }

    static void AddMessage(string message)
    {
        Console.SetCursorPosition(0, _messageLine);
        Console.WriteLine(message);
        _messageLine++;
    }

    static void ClearMapArea()
    {
        Console.SetCursorPosition(0, 0);

        for (int i = 0; i < _mapHeight; i++)
        {
            Console.Write(new string(' ', _mapWidth));
            Console.WriteLine();
        }

        Console.SetCursorPosition(0, 0);
    }
}
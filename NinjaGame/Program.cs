using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;

class NinjaGame
{   
    //globals
    static char[,] _map;
    static int _mapWidth;
    static int _mapHeight = 0;
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
    static bool _inBreakerMode = false;
    static int _holySymbolCounter = 0;
    static int _shurikensOnMap = 0;
    static char[] _validMapCharacters = { '@', '$', '#', 'X', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'S', 'E', 'N', 'W', 'M', 'B', '*',' ' };
    static Queue<(int, int)> _recentPositions = new Queue<(int, int)>();
    static int _maxRecentPositions = 100;
    static List<char> _secretPathList = new List<char>();
    
    static void Main(string[] args)
    { //we will use the main loop to call the actual game to keep the Main() method clean
        Dictionary<char, string> nameDic = new Dictionary<char, string>();
        nameDic.Add('A', "ka");
        nameDic.Add('B', "zu");
        nameDic.Add('C', "mi");
        nameDic.Add('D', "te");
        nameDic.Add('E', "ku");
        nameDic.Add('F', "lu");
        nameDic.Add('G', "ji");
        nameDic.Add('H', "ri");
        nameDic.Add('I', "ki");
        nameDic.Add('J', "zu");
        nameDic.Add('K', "me");
        nameDic.Add('L', "ta");
        nameDic.Add('M', "rin");
        nameDic.Add('N', "to");
        nameDic.Add('O', "mo");
        nameDic.Add('P', "no");
        nameDic.Add('Q', "ke");
        nameDic.Add('R', "shi");
        nameDic.Add('S', "ari");
        nameDic.Add('T', "chi");
        nameDic.Add('U', "do");
        nameDic.Add('V', "ru");
        nameDic.Add('W', "mei");
        nameDic.Add('X', "na");
        nameDic.Add('Y', "fu");
        nameDic.Add('Z', "zi");

        string lastNameSubString = "";
        string firstNameSubString = "";
        string lastName = "";
        string firstName = "";
        string lastNameResult = "";
        string firstNameResult = "";
        //create ninja name
        while (string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(firstName))
        {
            Console.Write("Enter your last name: ");
            lastName = Console.ReadLine();

            Console.Write("Enter your first name: ");
            firstName = Console.ReadLine();

            Console.Clear();
        }

        //check if valid name input
        if (!IsValidName(lastName) && !IsValidName(firstName))
        {
            Console.WriteLine("ERROR: name did not contain letters");
            Environment.Exit(0);
        }

        //now extract first 3 and 4 of names        
        lastNameSubString = lastName.Length >= 3 ? lastName.Substring(0, 3) : lastName;
        firstNameSubString = firstName.Length >= 4 ? firstName.Substring(0, 4) : firstName;

        foreach (char c in lastNameSubString)
        {
            foreach (KeyValuePair<char, string> key in nameDic)
            {
                if (key.Key == char.ToUpper(c))
                {
                    lastNameResult += key.Value;
                    break;
                }
            }
        }

        foreach (char c in firstNameSubString)
        {
            foreach (KeyValuePair<char, string> key in nameDic)
            {
                if (key.Key == char.ToUpper(c))
                {
                    firstNameResult += key.Value;
                    break;
                }
            }
        }

        Console.WriteLine($"Welcome {CapitalizeFirstLetter(lastNameResult)} {CapitalizeFirstLetter(firstNameResult)}!");

        try
        {
            Load_map("map2.txt");
            
            foreach(char path in _secretPathList)
            {
                if (VerifySecretPaths(_map, path, 2))
                {
                    continue;
                }
                else
                    throw new Exception($"Incorrect amount of a secret path. Must have 2. Path: {path}");
            }
            
            _messageLine = _mapHeight;

            GameLoop();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR: {ex.Message}");
        }
    }

    static void Load_map(string fileName)
    {
        var lines = File.ReadAllLines(fileName);//read the file that has the _map for the game
        _map = new char[lines.Length, lines[0].Length];
        bool hasPlayerSymbol = false;

        if (!IsMapValid(lines, out string errorMessage))        
            throw new Exception(errorMessage);        

        _mapHeight = lines.Length;
        _mapWidth = lines[0].Length;

        for (int i = 0; i < lines.Length; i++)
        {
            for (int j = 0; j < lines[i].Length; j++)
            {
                //need to verify that valid characters were added to the map
                char currentChar = lines[i][j];
                _map[i,j] = lines[i][j];

                if (!IsCharacterValid(currentChar))                
                    throw new Exception($"Invalid character was added to the map. If the character is a letter then it must be capitalized! character: '{currentChar}'");

                if (currentChar == '@')
                {
                    _ninjaX = i; // Store the row (i) where '@' is found
                    _ninjaY = j; // Store the column (j) where '@' is found
                    hasPlayerSymbol = true;
                }
                else if (currentChar == '$')
                    _holySymbolCounter++;
                else if (currentChar == '*')
                    _shurikensOnMap++;
                else if (currentChar == 'F' || currentChar == 'G' || currentChar == 'H' || currentChar == 'i' || currentChar == 'J' || currentChar == 'K' || currentChar == 'L')
                {
                    if(!_secretPathList.Contains(currentChar))
                        _secretPathList.Add(currentChar);
                }
            }
        }

        if (!hasPlayerSymbol)
            throw new Exception("The map does not have a player symbol");        

        _mapHeight = lines.Length + 3;// add 3 after error checks so it wont throw an error and so that when it prints the messages it doesnt cause issues
    }

    static bool IsCharacterValid(char currentChar)
    {
        return Array.Exists(_validMapCharacters, element => element == currentChar);
    }

    static bool IsMapValid(string[] lines, out string errorMessage)
    {
        errorMessage = "";
        if (lines.Length < 3)
        {
            errorMessage = "Map must have at least 3 rows for the border!";
            return false;
        }
        int maxWidth = lines[0].Length;

        //need to check if all rows have the same width
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].Length != maxWidth)
            {
                errorMessage = "Inconsisitent row lengths!";
                return false;
            }
        }

        //need to check for first and last characters are (#)
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i][0] != '#' || lines[i][maxWidth - 1] != '#')
            {
                errorMessage = "Left or right border does not contain #!";
                return false;
            }
        }

        //need to check if first and last rows are all #
        for (int i = 0; i < maxWidth; i++)
        {
            if (lines[0][i] != '#' || lines[lines.Length - 1][i] != '#')
            {
                errorMessage = "Top or bottom is not a wall of #";
                return false;
            }
        }       
        
        return true;
    }    

    static void GameLoop()
    {        
        while (_holySymbolCounter > 0)
        {
            PrintMap();//need to print map after each change
            ThrowShuriken();//we always have the option to throw shuriken if we have any
            if (_holySymbolCounter == 0)
            {
                AddMessage("All holy symbols destroyed");
                PrintMap();
                return;
            }
            else if (_shurikenCount == 0 && _shurikensOnMap == 0 && _holySymbolCounter > 1)
            {
                AddMessage("LOOP");
                throw new Exception("LOOP");
            }

            Movement();
            if (LoopingCheck())
            {
                AddMessage("LOOP");
                throw new Exception("LOOP");
            }
        }        
    }

    static void PrintMap()
    {
        Console.SetCursorPosition(0, 2);
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
                CollectShuriken(newX, newY);
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
            case 'J':
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
            case 'B':
                BreakerMode(newX, newY);
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
                _map[_ninjaX, _ninjaY] = '@';
                PrintMap();
                TrackPosition();

                if(LoopingCheck())
                {
                    AddMessage("LOOP");
                    throw new Exception("LOOP");
                }

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
                        AddMessage("THROW (hit the X symbol downwards)");
                    }
                    else
                    {
                        _map[i, _ninjaY] = ' '; // Remove $
                        _shurikenCount--;//decrease count if we hit a obstacle/symbol
                        AddMessage("THROW (hit the $ symbol downwards)");
                        _holySymbolCounter--;//decrease count of $ if 0 the ninja has won
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
                        AddMessage("THROW (hit the X symbol rightwards)");
                    }
                    else
                    {
                        _map[_ninjaX, j] = ' ';//Remove $
                        _shurikenCount--;//decrease count if we hit a obstacle/symbol
                        AddMessage("THROW (hit the $ symbol rightwards)");
                        _holySymbolCounter--;
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
                        _map[i, _ninjaY] = ' '; // Remove $
                        _shurikenCount--;//decrease count if we hit a obstacle/symbol
                        AddMessage("THROW (hit the $ symbol upwards)");
                        _holySymbolCounter--;
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
                        _map[_ninjaX, j] = ' '; // Remove $
                        _shurikenCount--;//decrease count if we hit a obstacle/symbol
                        AddMessage("THROW (hit the $ symbol leftwards)");
                        _holySymbolCounter--;
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
                    _currDir = 'W';

                    if (_prevDirChange != '\0')// need to check if prevDirChange was not empty. we want to make sure to keep the original position
                        _map[_ninjaX, _ninjaY] = _prevDirChange;
                    else
                        _map[_ninjaX, _ninjaY] = ' '; // Clear old position
                    _prevDirChange = _map[i, j];
                    _ninjaX = i; // Update player's X position
                    _ninjaY = j; // Update player's Y position
                    _map[_ninjaX, _ninjaY] = '@'; // Set new position

                    TrackPosition();

                    ThrowShuriken();
                    if (_holySymbolCounter == 0)
                    {
                        AddMessage("All holy symbols destroyed");
                        PrintMap();
                        return;
                    }
                    else if (_shurikenCount == 0 && _shurikensOnMap == 0 && _holySymbolCounter > 1)
                    {
                        AddMessage("LOOP");
                        throw new Exception("LOOP");
                    }

                    while (IsValidMove(_ninjaX, _ninjaY - 1))
                    {                        
                        MovePlayer(_ninjaX, _ninjaY - 1);
                        PrintMap();
                        
                        if (_currDir != 'W')
                            break;

                        AddMessage("WEST because of W");
                    }

                    _currDir = '\0';
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
                    _map[_ninjaX, _ninjaY] = '@'; // Set new position

                    TrackPosition();

                    ThrowShuriken();
                    if (_holySymbolCounter == 0)
                    {
                        AddMessage("All holy symbols destroyed");
                        PrintMap();
                        return;
                    }
                    else if (_shurikenCount == 0 && _shurikensOnMap == 0 && _holySymbolCounter > 1)
                    {
                        AddMessage("LOOP");
                        throw new Exception("LOOP");
                    }

                    while (IsValidMove(_ninjaX, _ninjaY + 1))
                    {
                        MovePlayer(_ninjaX, _ninjaY + 1);
                        PrintMap();
                        
                        if (_currDir != 'E')
                            break;

                        AddMessage("EAST because of E");
                    }

                    _currDir = '\0';
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
                    _currDir = 'S';

                    if (_prevDirChange != '\0')// need to check if prevDirChange was not empty. we want to make sure to keep the original position
                        _map[_ninjaX, _ninjaY] = _prevDirChange;
                    else
                        _map[_ninjaX, _ninjaY] = ' '; // Clear old position
                    _prevDirChange = _map[i, j];
                    _ninjaX = i; // Update player's X position
                    _ninjaY = j; // Update player's Y position
                    _map[_ninjaX, _ninjaY] = '@'; // Set new position

                    TrackPosition();

                    ThrowShuriken();
                    if (_holySymbolCounter == 0)
                    {
                        AddMessage("All holy symbols destroyed");
                        PrintMap();
                        return;
                    }
                    else if (_shurikenCount == 0 && _shurikensOnMap == 0 && _holySymbolCounter > 1)
                    {
                        AddMessage("LOOP");
                        throw new Exception("LOOP");
                    }

                    while (IsValidMove(_ninjaX + 1, _ninjaY))
                    {
                        MovePlayer(_ninjaX + 1, _ninjaY);
                        PrintMap();
                        
                        if (_currDir != 'S')
                            break;

                        AddMessage("SOUTH because of S");
                    }

                    _currDir = '\0';
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
                    _map[_ninjaX, _ninjaY] = '@'; // Set new position

                    TrackPosition();

                    ThrowShuriken();
                    if (_holySymbolCounter == 0)
                    {
                        AddMessage("All holy symbols destroyed");
                        PrintMap();
                        return;
                    }
                    else if (_shurikenCount == 0 && _shurikensOnMap == 0 && _holySymbolCounter > 1)
                    {
                        AddMessage("LOOP");
                        throw new Exception("LOOP");
                    }

                    while (IsValidMove(_ninjaX - 1, _ninjaY))
                    {
                        MovePlayer(_ninjaX - 1, _ninjaY);
                        PrintMap();
                        
                        if (_currDir != 'N')
                            break;

                        AddMessage("NORTH because of N");
                    }

                    _currDir = '\0';
                    return;
                }
            }
        }
    }

    static bool IsValidMove(int x, int y)
    {
        if(_inBreakerMode)
        {
            if (_map[x, y] == 'X')
            {
                _map[x, y] = ' ';
                //_shurikenCount++;//not sure if you get a * if destroying X in breakerMode
                AddMessage("BreakerMode (destroyed X)");
            }
        }

        // Check for unbreakables
        return x >= 0 && x < _map.GetLength(0) &&
               y >= 0 && y < _map.GetLength(1) &&
               _map[x, y] != '#' && _map[x, y] != 'X' &&
               _map[x, y] != '$';
    }

    static void Movement()
    {
        bool moved = false;
        bool enteredMirrored = false;
        
        while(!moved)
        {
            switch (_currDirection)
            {
                case 0://south
                    while(IsValidMove(_ninjaX + 1, _ninjaY) || (_isMirrored && IsValidMove(_ninjaX, _ninjaY - 1)))
                    {
                        if (_isMirrored)
                        {
                            enteredMirrored = true;
                            while (IsValidMove(_ninjaX, _ninjaY - 1))//west
                            {
                                ThrowShuriken();
                                if (_holySymbolCounter == 0)
                                {
                                    AddMessage("All holy symbols destroyed");
                                    PrintMap();
                                    return;
                                }
                                else if (_shurikenCount == 0 && _shurikensOnMap == 0 && _holySymbolCounter > 1)
                                {
                                    AddMessage("LOOP");
                                    throw new Exception("LOOP");
                                }

                                AddMessage("Mirrored WEST (current direction)");
                                MovePlayer(_ninjaX, _ninjaY - 1);

                                if (!_isMirrored)//we need to check if we are still mirrored since in the MovePlayer() we can get out of it, if so break out of the mirror
                                {
                                    moved = true;
                                    break;
                                }
                                PrintMap();
                                moved = true;                                
                            }
                            break;
                        }

                        ThrowShuriken();
                        if (_holySymbolCounter == 0)
                        {
                            AddMessage("All holy symbols destroyed");
                            PrintMap();
                            return;
                        }
                        else if (_shurikenCount == 0 && _shurikensOnMap == 0 && _holySymbolCounter > 1)
                        {
                            AddMessage("LOOP");
                            throw new Exception("LOOP");
                        }

                        AddMessage("SOUTH (initial direction)");
                        MovePlayer(_ninjaX + 1, _ninjaY);
                        PrintMap();
                        moved = true;
                    }

                    if (enteredMirrored && !_isMirrored)//not sure if we are supposed to stay in the same direction after exiting mirrored - but both should work
                        _currDirection = 0;
                    else
                        _currDirection = 1;

                    break;
                case 1://east             
                    while (IsValidMove(_ninjaX, _ninjaY + 1) || (_isMirrored && IsValidMove(_ninjaX - 1, _ninjaY)))
                    {                        
                        if (_isMirrored)
                        {
                            enteredMirrored = true;

                            while (IsValidMove(_ninjaX - 1, _ninjaY))//north
                            {
                                ThrowShuriken();
                                if (_holySymbolCounter == 0)
                                {
                                    AddMessage("All holy symbols destroyed");
                                    PrintMap();
                                    return;
                                }
                                else if (_shurikenCount == 0 && _shurikensOnMap == 0 && _holySymbolCounter > 1)
                                {
                                    AddMessage("LOOP");
                                    throw new Exception("LOOP");
                                }

                                AddMessage("Mirrored NORTH (current direction)");
                                MovePlayer(_ninjaX - 1, _ninjaY);

                                if (!_isMirrored)//we need to check if we are still mirrored since in the MovePlayer() we can get out of it, if so break out of the mirror
                                {
                                    moved = true;
                                    break;
                                }
                                PrintMap();
                                moved = true;
                            }
                            break;
                        }        
                        
                        ThrowShuriken();
                        if (_holySymbolCounter == 0)
                        {
                            AddMessage("All holy symbols destroyed");
                            PrintMap();
                            return;
                        }
                        else if (_shurikenCount == 0 && _shurikensOnMap == 0 && _holySymbolCounter > 1)
                        {
                            AddMessage("LOOP");
                            throw new Exception("LOOP");
                        }

                        AddMessage("EAST (current direction)");
                        MovePlayer(_ninjaX, _ninjaY + 1);
                        PrintMap();
                        moved = true;
                    }

                    if (enteredMirrored && !_isMirrored)//not sure if we are supposed to stay in the same direction after exiting mirrored - but both should work
                        _currDirection = 1;
                    else
                        _currDirection = 2;

                    break;
                case 2://north
                    while (IsValidMove(_ninjaX - 1, _ninjaY) || (_isMirrored && IsValidMove(_ninjaX, _ninjaY + 1)))
                    {
                        if (_isMirrored)
                        {
                            enteredMirrored = true;

                            while (IsValidMove(_ninjaX, _ninjaY + 1))//east
                            {
                                ThrowShuriken();
                                if (_holySymbolCounter == 0)
                                {
                                    AddMessage("All holy symbols destroyed");
                                    PrintMap();
                                    return;
                                }
                                else if (_shurikenCount == 0 && _shurikensOnMap == 0 && _holySymbolCounter > 1)
                                {
                                    AddMessage("LOOP");
                                    throw new Exception("LOOP");
                                }

                                AddMessage("Mirrored EAST (current direction)");
                                MovePlayer(_ninjaX, _ninjaY + 1);

                                if (!_isMirrored)//we need to check if we are still mirrored since in the MovePlayer() we can get out of it, if so break out of the mirror
                                {
                                    moved = true;
                                    break;
                                }
                                PrintMap();
                                moved = true;
                            }
                            break;
                        }
                        
                        ThrowShuriken();
                        if (_holySymbolCounter == 0)
                        {
                            AddMessage("All holy symbols destroyed");
                            PrintMap();
                            return;
                        }
                        else if (_shurikenCount == 0 && _shurikensOnMap == 0 && _holySymbolCounter > 1)
                        {
                            AddMessage("LOOP");
                            throw new Exception("LOOP");
                        }

                        AddMessage("NORTH (current direction)");
                        MovePlayer(_ninjaX - 1, _ninjaY);
                        PrintMap();
                        moved = true;
                    }
                    if (enteredMirrored && !_isMirrored)//not sure if we are supposed to stay in the same direction after exiting mirrored - but both should work
                        _currDirection = 2;
                    else
                        _currDirection = 3;

                    break;
                case 3://west
                    while (IsValidMove(_ninjaX, _ninjaY - 1) || (_isMirrored && IsValidMove(_ninjaX + 1, _ninjaY)))
                    {
                        if (_isMirrored)
                        {
                            enteredMirrored = true;

                            while (IsValidMove(_ninjaX + 1, _ninjaY))//south
                            {
                                ThrowShuriken();
                                if (_holySymbolCounter == 0)
                                {
                                    AddMessage("All holy symbols destroyed");
                                    PrintMap();
                                    return;
                                }
                                else if (_shurikenCount == 0 && _shurikensOnMap == 0 && _holySymbolCounter > 1)
                                {
                                    AddMessage("LOOP");
                                    throw new Exception("LOOP");
                                }

                                AddMessage("Mirrored SOUTH (current direction)");
                                MovePlayer(_ninjaX + 1, _ninjaY);

                                if (!_isMirrored)//we need to check if we are still mirrored since in the MovePlayer() we can get out of it, if so break out of the mirror
                                {
                                    moved = true;
                                    break;
                                }
                                PrintMap();
                                moved = true;
                            }
                            break;
                        }

                        ThrowShuriken();
                        if (_holySymbolCounter == 0)
                        {
                            AddMessage("All holy symbols destroyed");
                            PrintMap();
                            return;
                        }
                        else if (_shurikenCount == 0 && _shurikensOnMap == 0 && _holySymbolCounter > 1)
                        {
                            AddMessage("LOOP");
                            throw new Exception("LOOP");
                        }

                        AddMessage("WEST (current direction)");
                        MovePlayer(_ninjaX, _ninjaY - 1);
                        PrintMap();
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
                    else if (_prevDirChange != '\0')
                    {
                        _map[_ninjaX, _ninjaY] = _prevDirChange;
                        _prevDirChange = '\0';
                    }
                    else
                        _map[_ninjaX, _ninjaY] = ' '; // Clear old position

                    _prevSecretPath = _map[i, j];
                    _ninjaX = i; // Update player's X position
                    _ninjaY = j; // Update player's Y position
                    _map[_ninjaX, _ninjaY] = '@'; // Set new position
                    TrackPosition();
                    return; // Exit once we move
                }
            }
        }
    }

    static void CollectShuriken(int newX, int newY)
    {
        _shurikenCount++;
        AddMessage("Picks up shuriken");
        if (_prevDirChange != '\0')
        {
            _map[_ninjaX, _ninjaY] = _prevDirChange;
            _prevDirChange = '\0';
        }
        else if (_prevSecretPath != '\0')
        {
            _map[_ninjaX, _ninjaY] = _prevSecretPath;
            _prevSecretPath = '\0';
        }
        else
            _map[_ninjaX, _ninjaY] = ' ';

        _ninjaX = newX;
        _ninjaY = newY;
        _map[_ninjaX, _ninjaY] = '@';
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
                    else if(_prevSecretPath != '\0')
                    {
                        _map[_ninjaX, _ninjaY] = _prevSecretPath;
                        _prevSecretPath = '\0';
                    }
                    else
                        _map[_ninjaX, _ninjaY] = ' '; // Clear old position

                    _prevMirror = _map[i, j];
                    _ninjaX = i; // Update player's X position
                    _ninjaY = j; // Update player's Y position
                    _map[_ninjaX, _ninjaY] = '@'; // Set new position
                    TrackPosition();

                    return;
                }
            }
        }
    }

    static void BreakerMode(int currX, int currY)
    {
        for (int i = 0; i < _map.GetLength(0); i++)
        {
            for (int j = 0; j < _map.GetLength(1); j++)
            {
                if ((_map[i, j] == 'B') && (i == currX && j == currY) && (_map[currX, currY] == _map[i, j]))
                {
                    if (_inBreakerMode)
                    {
                        _inBreakerMode = false;
                        AddMessage("Out of BreakerMode");
                    }
                    else
                    {
                        _inBreakerMode = true;
                        AddMessage("In BreakerMode");
                    }

                    if (_prevDirChange != '\0')
                    {
                        _map[_ninjaX, _ninjaY] = _prevDirChange;
                        _prevDirChange = '\0';
                    }
                    else if (_prevSecretPath != '\0')
                    {
                        _map[_ninjaX, _ninjaY] = _prevSecretPath;
                        _prevSecretPath = '\0';
                    }
                    else if (_prevMirror != '\0')
                    {
                        _map[_ninjaX, _ninjaY] = _prevMirror;
                        _prevMirror = '\0';
                    }
                    else
                        _map[_ninjaX, _ninjaY] = ' ';

                    _ninjaX = i; // Update player's X position
                    _ninjaY = j; // Update player's Y position
                    _map[_ninjaX, _ninjaY] = '@'; // Set new position

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

    static string CapitalizeFirstLetter(string name)
    {
        if (string.IsNullOrEmpty(name))
            return string.Empty;

        return char.ToUpper(name[0]) + name.Substring(1).ToLower();
    }

    static bool VerifySecretPaths(char[,] map, char pathSymbol, int expectedCount)
    {
        int count = 0;

        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                if (map[i, j] == pathSymbol)
                {
                    count++;
                }
            }
        }

        return count == expectedCount;
    }

    static void TrackPosition()//When tracking we only care about tracking the player movement. Changing the map for example collecting a shuriken we dont need to track
    {
        var currPosition = (_ninjaX, _ninjaY);
        _recentPositions.Enqueue(currPosition);

        if(_recentPositions.Count > _maxRecentPositions)
        {
            _recentPositions.Dequeue();
        }
    }

    static bool LoopingCheck()
    {
        var positionList = new List<(int, int)>(_recentPositions);

        //Dictionary stores the positions as keys and the values are the number of times it was visited
        Dictionary<(int, int), int> positionCount = new Dictionary<(int, int), int>();

        foreach (var position in positionList) 
        {
            if(positionCount.ContainsKey(position))
            {
                positionCount[position]++;//if the position has been added increment the count
            }
            else
            {
                positionCount[position] = 1;
            }

            // this will check if this position has been visited more than 3 times
            if (positionCount[position] > 3)
            {
                return true;
            }
        }
        return false;
    }

    static bool IsValidName(string name)
    {
        string pattern = @"^[a-zA-Z]+$";

        return Regex.IsMatch(name, pattern);
    }
}
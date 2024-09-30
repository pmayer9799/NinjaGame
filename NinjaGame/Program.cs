using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;

class NinjaGame
{   
    //globals
    static char[,] _map;
    static int _mapWidth;
    static int _mapHeight = 0;
    static int _messageLine = 0;
    static char[] _validMapCharacters = { '@', '$', '#', 'X', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'S', 'E', 'N', 'W', 'M', 'B', '*',' ', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
    static Queue<(int, int)> _recentPositions = new Queue<(int, int)>();
    static int _maxRecentPositions = 100;
    static List<char> _secretPathList = new List<char>();    
    
    static void Main(string[] args)
    { //we will use the main loop to call the actual game to keep the Main() method clean
        Bomb bomb = new Bomb(0, 0);
        Ninja ninja = new Ninja(0,0, bomb);
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
            Load_map("map4.txt", ninja);
            
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

            GameLoop(ninja);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR: {ex.Message}");
        }
    }

    static void Load_map(string fileName, Ninja ninja)
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
                    ninja.NinjaX = i; // Store the row (i) where '@' is found
                    ninja.NinjaY = j; // Store the column (j) where '@' is found
                    hasPlayerSymbol = true;
                }
                else if (currentChar == '$')
                    ninja._holySymbolCounter++;                    
                else if (currentChar == '*')
                    ninja._shurikensOnMap++;
                else if (currentChar == 'F' || currentChar == 'G' || currentChar == 'H' || currentChar == 'i' || currentChar == 'J' || currentChar == 'K' || currentChar == 'L')
                {
                    if(!_secretPathList.Contains(currentChar))
                        _secretPathList.Add(currentChar);
                }
                else if (currentChar == '1' || currentChar == '2' || currentChar == '3' || currentChar == '4' || currentChar == '5' || currentChar == '6' || currentChar == '7' || currentChar == '8' || currentChar == '9')
                {
                    Bomb.IsBomb = true;
                    Bomb.bombDics.Add((i, j), (int.Parse(currentChar.ToString()), false));
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

    static void GameLoop(Ninja ninja)
    {        
        while (ninja._holySymbolCounter > 0)
        {
            PrintMap();//need to print map after each change
            ninja.ThrowShuriken(_map);//we always have the option to throw shuriken if we have any
            PrintMap();
            if (ninja._holySymbolCounter == 0)
            {
                AddMessage("All holy symbols destroyed");
                PrintMap();
                return;
            }
            //else if (ninja._shurikenCount == 0 && ninja._shurikensOnMap == 0 && ninja._holySymbolCounter > 1)
            //{
            //    AddMessage("LOOP");
            //    throw new Exception("LOOP");
            //}

            ninja.Movement(_map);
            if (LoopingCheck())
            {
                AddMessage("LOOP");
                throw new Exception("LOOP");
            }
        }        
    }

    public static void PrintMap()
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

    public static void AddMessage(string message)
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

    public static void TrackPosition(int currX, int currY)//When tracking we only care about tracking the player movement. Changing the map for example collecting a shuriken we dont need to track
    {
        var currPosition = (currX, currY);
        _recentPositions.Enqueue(currPosition);

        if(_recentPositions.Count > _maxRecentPositions)
        {
            _recentPositions.Dequeue();
        }
    }

    public static bool LoopingCheck()
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
// See https://aka.ms/new-console-template for more information
using System.Reflection.Metadata.Ecma335;

class NinjaGame

{

    static char[,] _map;

    static int _ninjaX;

    static int _ninjaY;

    static int _shurikenCount = 3;//starting limit

    static void Main(string[] args)
    { //we will use the main loop to call the actual game to keep the Main() method clean
        Load_map("map.txt");
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
            Console.Clear();
            Print_map();//need to print map after each change

            ThrowShuriken();//we always have the option to throw shuriken
            Movement();

            Console.WriteLine("Count: " + _shurikenCount);
        }
    }

    static void Print_map()
    {
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
        // Replace the player's old position with a space
        _map[_ninjaX, _ninjaY] = ' ';

        // Update the player's position
        _ninjaX = newX;
        _ninjaY = newY;

        // Place the player in the new position
        _map[_ninjaX, _ninjaY] = 'P';
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
                        _map[i, _ninjaY] = '*'; // Remove $
                        _shurikenCount--;//decrease count if we hit a obstacle/symbol
                        Console.WriteLine("You hit the $ symbol below!");
                    }
                    else
                    {
                        _map[i, _ninjaY] = ' '; // Remove $
                        _shurikenCount--;//decrease count if we hit a obstacle/symbol
                        Console.WriteLine("You hit the $ symbol below!");
                    }
                    //break; // Stop checking further down
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
                        Console.WriteLine("You hit the $ symbol to the right!");
                    }
                    else
                    {
                        _map[_ninjaX, j] = ' '; // Remove $
                        _shurikenCount--;//decrease count if we hit a obstacle/symbol
                        Console.WriteLine("You hit the $ symbol to the right!");
                    }
                    //break; // Stop checking further right
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
                        _map[i, _ninjaY] = '*'; // Remove $
                        _shurikenCount--;//decrease count if we hit a obstacle/symbol
                        Console.WriteLine("You hit the $ symbol above!");
                    }
                    else
                    {
                        _map[i, _ninjaY] = ' '; // Remove $
                        _shurikenCount--;//decrease count if we hit a obstacle/symbol
                        Console.WriteLine("You hit the $ symbol above!");
                    }
                    //break; // Stop checking further up
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
                        _map[_ninjaX, j] = '*'; // Remove $
                        _shurikenCount--;//decrease count if we hit a obstacle/symbol
                        Console.WriteLine("You hit the $ symbol to the left!");
                    }
                    else
                    {
                        _map[_ninjaX, j] = ' '; // Remove $
                        _shurikenCount--;//decrease count if we hit a obstacle/symbol
                        Console.WriteLine("You hit the $ symbol to the left!");
                    }
                    //break; // Stop checking further left
                }
                else if (_map[_ninjaX, j] == '#')
                    break; // Hit a wall, stop
            }
        }
    }

    static bool IsValidMove(int x, int y)
    {
        // Check for unbreakables
        return x >= 0 && x < _map.GetLength(0) &&
               y >= 0 && y < _map.GetLength(1) &&
               _map[x, y] != '#';
    }

    static void Movement()
    {
        // Try to move down
        //IsValidMove checks if a # is in the way
        if (IsValidMove(_ninjaX + 1, _ninjaY))
        {
            MovePlayer(_ninjaX + 1, _ninjaY);
        }

        // Try to move right
        else if (IsValidMove(_ninjaX, _ninjaY + 1))
        {
            MovePlayer(_ninjaX, _ninjaY + 1);
        }

        // Try to move up
        else if (IsValidMove(_ninjaX - 1, _ninjaY))
        {
            MovePlayer(_ninjaX - 1, _ninjaY);
        }

        // Try to move left
        else if (IsValidMove(_ninjaX, _ninjaY - 1))
        {
            MovePlayer(_ninjaX, _ninjaY - 1);
        }
    }
}
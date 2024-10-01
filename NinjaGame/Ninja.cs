using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

public class Ninja
{
    private int shurikenCounter = 3;
    public int NinjaX { get; set; }
    public int NinjaY { get; set; }

    public int HolySymbolCounter { get; set; }
    public int ShurikenCount
    {
        get { return shurikenCounter; }
        set { shurikenCounter = value; }
    }

    public static int CurrDirection = 0;
    public static char PrevSecretPath = '\0';
    public static char PrevDirChange = '\0';
    public static char CurrDir = '\0';
    public static char PrevMirror = '\0';
    public static char PrevBomb = '\0';
    public static bool IsMirrored = false;
    public static bool InBreakerMode = false;
    private Bomb bomb;
    private MessageManager messageManager;

    public Ninja(int startX, int startY, int shurikenCount, Bomb bomb, MessageManager messageManager)
    {
        NinjaX = startX;
        NinjaY = startY;
        ShurikenCount = shurikenCount;
        this.bomb = bomb;
        this.messageManager = messageManager;
    }

    private bool IsValidMove(int x, int y, char[,] map)
    {
        if (InBreakerMode)
        {
            if (map[x, y] == 'X')
            {
                map[x, y] = ' ';
                //ShurikenCount++;//not sure if you get a * if destroying X in breakerMode
                
                messageManager.AddMessage("BreakerMode (destroyed X)");
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
            switch (CurrDirection)
            {
                case 0://south
                    while (IsValidMove(NinjaX + 1, NinjaY, map) || (IsMirrored && IsValidMove(NinjaX, NinjaY - 1, map)))
                    {
                        if (IsMirrored)
                        {
                            enteredMirrored = true;
                            while (IsValidMove(NinjaX, NinjaY - 1, map))//west
                            {
                                ThrowShuriken(map);
                                if (HolySymbolCounter == 0)
                                {
                                    messageManager.AddMessage("All holy symbols destroyed");
                                    NinjaGame.PrintMap();
                                    return;
                                }

                                if (Bomb.IsBomb)//check if there is bomb on map otherwise we dont need to hit this code
                                {
                                    bomb.CheckBombAndTriggerCountdown(NinjaX, NinjaY, Bomb.bombDics);//Checks if ninja is next to bomb
                                    bomb.BombCountDownAndExplode(NinjaX, NinjaY, map);
                                    NinjaGame.PrintMap();
                                }
                                
                                messageManager.AddMessage("Mirrored WEST (current direction)");
                                MoveNinja(NinjaX, NinjaY - 1, map);

                                if (!IsMirrored)//we need to check if we are still mirrored since in the MoveNinja() we can get out of it, if so break out of the mirror
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
                        if (HolySymbolCounter == 0)
                        {
                            messageManager.AddMessage("All holy symbols destroyed");
                            NinjaGame.PrintMap();
                            return;
                        }

                        if (Bomb.IsBomb)//check if there is bomb on map otherwise we dont need to hit this code
                        {
                            bomb.CheckBombAndTriggerCountdown(NinjaX, NinjaY, Bomb.bombDics);//Checks if ninja is next to bomb
                            bomb.BombCountDownAndExplode(NinjaX, NinjaY, map);
                            NinjaGame.PrintMap();
                        }
                        messageManager.AddMessage("SOUTH (initial direction)");
                        MoveNinja(NinjaX + 1, NinjaY, map);

                        NinjaGame.PrintMap();
                        moved = true;
                    }

                    if (enteredMirrored && !IsMirrored)//not sure if we are supposed to stay in the same direction after exiting mirrored - but both should work
                        CurrDirection = 0;
                    else
                        CurrDirection = 1;

                    break;
                case 1://east             
                    while (IsValidMove(NinjaX, NinjaY + 1, map) || (IsMirrored && IsValidMove(NinjaX - 1, NinjaY, map)))
                    {
                        if (IsMirrored)
                        {
                            enteredMirrored = true;

                            while (IsValidMove(NinjaX - 1, NinjaY, map))//north
                            {
                                ThrowShuriken(map);
                                if (HolySymbolCounter == 0)
                                {
                                    messageManager.AddMessage("All holy symbols destroyed");
                                    NinjaGame.PrintMap();
                                    return;
                                }

                                if (Bomb.IsBomb)//check if there is bomb on map otherwise we dont need to hit this code
                                {
                                    bomb.CheckBombAndTriggerCountdown(NinjaX, NinjaY, Bomb.bombDics);//Checks if ninja is next to bomb
                                    bomb.BombCountDownAndExplode(NinjaX, NinjaY, map);
                                    NinjaGame.PrintMap();
                                }
                                messageManager.AddMessage("Mirrored NORTH (current direction)");
                                MoveNinja(NinjaX - 1, NinjaY, map);

                                if (!IsMirrored)//we need to check if we are still mirrored since in the MoveNinja() we can get out of it, if so break out of the mirror
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
                        if (HolySymbolCounter == 0)
                        {
                            messageManager.AddMessage("All holy symbols destroyed");
                            NinjaGame.PrintMap();
                            return;
                        }

                        if (Bomb.IsBomb)//check if there is bomb on map otherwise we dont need to hit this code
                        {
                            bomb.CheckBombAndTriggerCountdown(NinjaX, NinjaY, Bomb.bombDics);//Checks if ninja is next to bomb
                            bomb.BombCountDownAndExplode(NinjaX, NinjaY, map);
                            NinjaGame.PrintMap();
                        }
                        messageManager.AddMessage("EAST (current direction)");
                        MoveNinja(NinjaX, NinjaY + 1, map);

                        NinjaGame.PrintMap();
                        moved = true;
                    }

                    if (enteredMirrored && !IsMirrored)//not sure if we are supposed to stay in the same direction after exiting mirrored - but both should work
                        CurrDirection = 1;
                    else
                        CurrDirection = 2;

                    break;
                case 2://north
                    while (IsValidMove(NinjaX - 1, NinjaY, map) || (IsMirrored && IsValidMove(NinjaX, NinjaY + 1, map)))
                    {
                        if (IsMirrored)
                        {
                            enteredMirrored = true;

                            while (IsValidMove(NinjaX, NinjaY + 1, map))//east
                            {
                                ThrowShuriken(map);
                                if (HolySymbolCounter == 0)
                                {
                                    messageManager.AddMessage("All holy symbols destroyed");
                                    NinjaGame.PrintMap();
                                    return;
                                }

                                if (Bomb.IsBomb)//check if there is bomb on map otherwise we dont need to hit this code
                                {
                                    bomb.CheckBombAndTriggerCountdown(NinjaX, NinjaY, Bomb.bombDics);//Checks if ninja is next to bomb
                                    bomb.BombCountDownAndExplode(NinjaX, NinjaY, map);
                                    NinjaGame.PrintMap();
                                }

                                messageManager.AddMessage("Mirrored EAST (current direction)");
                                MoveNinja(NinjaX, NinjaY + 1, map);

                                if (!IsMirrored)//we need to check if we are still mirrored since in the MoveNinja() we can get out of it, if so break out of the mirror
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
                        if (HolySymbolCounter == 0)
                        {
                            messageManager.AddMessage("All holy symbols destroyed");
                            NinjaGame.PrintMap();
                            return;
                        }

                        if (Bomb.IsBomb)//check if there is bomb on map otherwise we dont need to hit this code
                        {
                            bomb.CheckBombAndTriggerCountdown(NinjaX, NinjaY, Bomb.bombDics);//Checks if ninja is next to bomb
                            bomb.BombCountDownAndExplode(NinjaX, NinjaY, map);
                            NinjaGame.PrintMap();
                        }

                        messageManager.AddMessage("NORTH (current direction)");
                        MoveNinja(NinjaX - 1, NinjaY, map);

                        NinjaGame.PrintMap();
                        moved = true;
                    }
                    if (enteredMirrored && !IsMirrored)//not sure if we are supposed to stay in the same direction after exiting mirrored - but both should work
                        CurrDirection = 2;
                    else
                        CurrDirection = 3;

                    break;
                case 3://west
                    while (IsValidMove(NinjaX, NinjaY - 1, map) || (IsMirrored && IsValidMove(NinjaX + 1, NinjaY, map)))
                    {
                        if (IsMirrored)
                        {
                            enteredMirrored = true;

                            while (IsValidMove(NinjaX + 1, NinjaY, map))//south
                            {
                                ThrowShuriken(map);
                                if (HolySymbolCounter == 0)
                                {
                                    messageManager.AddMessage("All holy symbols destroyed");
                                    NinjaGame.PrintMap();
                                    return;
                                }

                                if (Bomb.IsBomb)//check if there is bomb on map otherwise we dont need to hit this code
                                {
                                    bomb.CheckBombAndTriggerCountdown(NinjaX, NinjaY, Bomb.bombDics);//Checks if ninja is next to bomb
                                    bomb.BombCountDownAndExplode(NinjaX, NinjaY, map);
                                    NinjaGame.PrintMap();
                                }
                                messageManager.AddMessage("Mirrored SOUTH (current direction)");
                                MoveNinja(NinjaX + 1, NinjaY, map);

                                if (!IsMirrored)//we need to check if we are still mirrored since in the MoveNinja() we can get out of it, if so break out of the mirror
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
                        if (HolySymbolCounter == 0)
                        {
                            messageManager.AddMessage("All holy symbols destroyed");
                            NinjaGame.PrintMap();
                            return;
                        }

                        if (Bomb.IsBomb)//check if there is bomb on map otherwise we dont need to hit this code
                        {
                            bomb.CheckBombAndTriggerCountdown(NinjaX, NinjaY, Bomb.bombDics);//Checks if ninja is next to bomb
                            bomb.BombCountDownAndExplode(NinjaX, NinjaY, map);
                            NinjaGame.PrintMap();
                        }

                        messageManager.AddMessage("WEST (current direction)");
                        MoveNinja(NinjaX, NinjaY - 1, map);

                        NinjaGame.PrintMap();
                        moved = true;
                    }

                    if (enteredMirrored && !IsMirrored)//not sure if we are supposed to stay in the same direction after exiting mirrored - but both should work
                        CurrDirection = 3;
                    else
                        CurrDirection = 0;

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
                    messageManager.AddMessage("LOOP");
                    throw new Exception("LOOP");
                }
                break;
        }
    }

    private void CollectShuriken(int newX, int newY, char[,] map)
    {
        ShurikenCount++;
        messageManager.AddMessage("Picks up shuriken");

        ClearOutPreviousPositions(map);

        NinjaX = newX;
        NinjaY = newY;
        map[NinjaX, NinjaY] = '@';
    }

    public void ThrowShuriken(char[,] map)
    {
        //we throw the Shuriken based on the specified movement (downwards,rightwards,upwards,leftwards)
        //check first if we have any left in order to throw one otherwise we just move
        if (ShurikenCount >= 1)
        {
            // 1. Check downwards on the same column
            for (int i = NinjaX + 1; i < map.GetLength(0); i++)
            {
                if ((map[i, NinjaY] == '$' || map[i, NinjaY] == 'X') && ShurikenCount >= 1)
                {
                    if (map[i, NinjaY] == 'X')
                    {
                        map[i, NinjaY] = '*';
                        ShurikenCount--;//decrease count if we hit a obstacle/symbol
                        messageManager.AddMessage("THROW (hit the X symbol downwards)");
                    }
                    else
                    {
                        map[i, NinjaY] = ' '; // Remove $
                        ShurikenCount--;//decrease count if we hit a obstacle/symbol
                        messageManager.AddMessage("THROW (hit the $ symbol downwards)");
                        HolySymbolCounter--;//decrease count of $ if 0 the ninja has won
                    }
                }
                else if (map[i, NinjaY] == '#')
                    break; // Hit a wall, stop
            }

            // 2. Check rightwards on the same row
            for (int j = NinjaY + 1; j < map.GetLength(1); j++)
            {
                if ((map[NinjaX, j] == '$' || map[NinjaX, j] == 'X') && ShurikenCount >= 1)
                {
                    if (map[NinjaX, j] == 'X')
                    {
                        map[NinjaX, j] = '*';
                        ShurikenCount--;//decrease count if we hit a obstacle/symbol
                        messageManager.AddMessage("THROW (hit the X symbol rightwards)");
                    }
                    else
                    {
                        map[NinjaX, j] = ' ';//Remove $
                        ShurikenCount--;//decrease count if we hit a obstacle/symbol
                        messageManager.AddMessage("THROW (hit the $ symbol rightwards)");
                        HolySymbolCounter--;
                    }
                }
                else if (map[NinjaX, j] == '#')
                    break; // Hit a wall, stop
            }

            // 3. Check upwards on the same column
            for (int i = NinjaX - 1; i >= 0; i--)
            {
                if ((map[i, NinjaY] == '$' || map[i, NinjaY] == 'X') && ShurikenCount >= 1)
                {
                    if (map[i, NinjaY] == 'X')
                    {
                        map[i, NinjaY] = '*';
                        ShurikenCount--;//decrease count if we hit a obstacle/symbol
                        messageManager.AddMessage("THROW (hit the X symbol upwards)");
                    }
                    else
                    {
                        map[i, NinjaY] = ' '; // Remove $
                        ShurikenCount--;//decrease count if we hit a obstacle/symbol
                        messageManager.AddMessage("THROW (hit the $ symbol upwards)");
                        HolySymbolCounter--;
                    }
                }
                else if (map[i, NinjaY] == '#')
                    break; // Hit a wall, stop
            }

            // 4. Check leftwards on the same row
            for (int j = NinjaY - 1; j >= 0; j--)
            {
                if ((map[NinjaX, j] == '$' || map[NinjaX, j] == 'X') && ShurikenCount >= 1)
                {
                    if (map[NinjaX, j] == 'X')
                    {
                        map[NinjaX, j] = '*';
                        ShurikenCount--;//decrease count if we hit a obstacle/symbol
                        messageManager.AddMessage("THROW (hit the X symbol leftwards)");
                    }
                    else
                    {
                        map[NinjaX, j] = ' '; // Remove $
                        ShurikenCount--;//decrease count if we hit a obstacle/symbol
                        messageManager.AddMessage("THROW (hit the $ symbol leftwards)");
                        HolySymbolCounter--;
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
                    CurrDir = 'W';

                    ClearOutPreviousPositions(map);

                    PrevDirChange = map[i, j];
                    NinjaX = i; // Update ninja's X position
                    NinjaY = j; // Update ninja's Y position
                    map[NinjaX, NinjaY] = '@'; // Set new position

                    NinjaGame.TrackPosition(NinjaX, NinjaY);                    

                    while (IsValidMove(NinjaX, NinjaY - 1, map))
                    {
                        ThrowShuriken(map);
                        if (HolySymbolCounter == 0)
                        {
                            messageManager.AddMessage("All holy symbols destroyed");
                            NinjaGame.PrintMap();
                            return;
                        }

                        if (Bomb.IsBomb)//check if there is bomb on map otherwise we dont need to hit this code
                        {
                            bomb.CheckBombAndTriggerCountdown(NinjaX, NinjaY, Bomb.bombDics);//Checks if ninja is next to bomb
                            bomb.BombCountDownAndExplode(NinjaX, NinjaY, map);
                            NinjaGame.PrintMap();
                        }

                        MoveNinja(NinjaX, NinjaY - 1, map);
                        NinjaGame.PrintMap();

                        if (CurrDir != 'W')
                            break;

                        messageManager.AddMessage("WEST because of W");
                    }

                    CurrDir = '\0';
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
                    CurrDir = 'E';

                    ClearOutPreviousPositions(map);

                    PrevDirChange = map[i, j];
                    NinjaX = i; // Update ninja's X position
                    NinjaY = j; // Update ninja's Y position
                    map[NinjaX, NinjaY] = '@'; // Set new position

                    NinjaGame.TrackPosition(NinjaX, NinjaY);                    

                    while (IsValidMove(NinjaX, NinjaY + 1, map))
                    {
                        ThrowShuriken(map);
                        if (HolySymbolCounter == 0)
                        {
                            messageManager.AddMessage("All holy symbols destroyed");
                            NinjaGame.PrintMap();
                            return;
                        }

                        if (Bomb.IsBomb)//check if there is bomb on map otherwise we dont need to hit this code
                        {
                            bomb.CheckBombAndTriggerCountdown(NinjaX, NinjaY, Bomb.bombDics);//Checks if ninja is next to bomb
                            bomb.BombCountDownAndExplode(NinjaX, NinjaY, map);
                            NinjaGame.PrintMap();
                        }

                        MoveNinja(NinjaX, NinjaY + 1, map);
                        NinjaGame.PrintMap();

                        if (CurrDir != 'E')
                            break;

                        messageManager.AddMessage("EAST because of E");
                    }

                    CurrDir = '\0';
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
                    CurrDir = 'S';

                    ClearOutPreviousPositions(map);

                    PrevDirChange = map[i, j];
                    NinjaX = i; // Update ninja's X position
                    NinjaY = j; // Update ninja's Y position
                    map[NinjaX, NinjaY] = '@'; // Set new position

                    NinjaGame.TrackPosition(NinjaX, NinjaY);
                    
                    while (IsValidMove(NinjaX + 1, NinjaY, map))
                    {
                        ThrowShuriken(map);
                        if (HolySymbolCounter == 0)
                        {
                            messageManager.AddMessage("All holy symbols destroyed");
                            NinjaGame.PrintMap();
                            return;
                        }

                        if (Bomb.IsBomb)//check if there is bomb on map otherwise we dont need to hit this code
                        {
                            bomb.CheckBombAndTriggerCountdown(NinjaX, NinjaY, Bomb.bombDics);//Checks if ninja is next to bomb
                            bomb.BombCountDownAndExplode(NinjaX, NinjaY, map);
                            NinjaGame.PrintMap();
                        }

                        MoveNinja(NinjaX + 1, NinjaY, map);
                        NinjaGame.PrintMap();

                        if (CurrDir != 'S')
                            break;

                        messageManager.AddMessage("SOUTH because of S");
                    }

                    CurrDir = '\0';
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
                    CurrDir = 'N';

                    ClearOutPreviousPositions(map);

                    PrevDirChange = map[i, j];
                    NinjaX = i; // Update ninja's X position
                    NinjaY = j; // Update ninja's Y position
                    map[NinjaX, NinjaY] = '@'; // Set new position

                    NinjaGame.TrackPosition(NinjaX, NinjaY);

                    while (IsValidMove(NinjaX - 1, NinjaY, map))
                    {
                        NinjaGame.PrintMap();
                        ThrowShuriken(map);
                        if (HolySymbolCounter == 0)
                        {
                            messageManager.AddMessage("All holy symbols destroyed");
                            NinjaGame.PrintMap();
                            return;
                        }

                        //check if there is bomb on map otherwise we dont need to hit this code
                        if (Bomb.IsBomb)
                        {
                            bomb.CheckBombAndTriggerCountdown(NinjaX, NinjaY, Bomb.bombDics);//Checks if ninja is next to bomb
                            bomb.BombCountDownAndExplode(NinjaX, NinjaY, map);
                            NinjaGame.PrintMap();
                        }

                        MoveNinja(NinjaX - 1, NinjaY, map);
                        NinjaGame.PrintMap();

                        if (CurrDir != 'N')
                            break;

                        messageManager.AddMessage("NORTH because of N");
                    }

                    CurrDir = '\0';
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

                    PrevSecretPath = map[i, j];
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
                    if (IsMirrored)
                        IsMirrored = false;
                    else
                        IsMirrored = true;

                    ClearOutPreviousPositions(map);

                    PrevMirror = map[i, j];
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
                    if (InBreakerMode)
                    {
                        InBreakerMode = false;
                        messageManager.AddMessage("Out of BreakerMode");
                    }
                    else
                    {
                        InBreakerMode = true;
                        messageManager.AddMessage("In BreakerMode");
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

                    PrevBomb = map[i, j];
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
        if (PrevDirChange != '\0')
        {
            map[NinjaX, NinjaY] = PrevDirChange;
            PrevDirChange = '\0';
        }
        else if (PrevSecretPath != '\0')
        {
            map[NinjaX, NinjaY] = PrevSecretPath;
            PrevSecretPath = '\0';
        }
        else if (PrevMirror != '\0')
        {
            map[NinjaX, NinjaY] = PrevMirror;
            PrevMirror = '\0';
        }
        else if (PrevBomb != '\0')
        {
            map[NinjaX, NinjaY] = PrevBomb;
            PrevBomb = '\0';
        }
        else
            map[NinjaX, NinjaY] = ' ';

        NinjaGame.PrintMap();
    }
}


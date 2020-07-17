//Sajid
//Connect 4 Command Line C#

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public struct playerInfo
{
	public String playerName;
	public char playerID;
};

namespace Connect4
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter Something");
            String str = Console.ReadLine();
            Console.WriteLine("You said, \"" + str + "\" so I reply with Goodbye!");
            if(Console.ReadLine("no") || Console.ReadLine("yes")) {
                Console.WriteLine("I didn't really care tbh");
            }
            else {
                Console.WriteLine("Please write yes or no");
            }
            playerInfo playerOne = new playerInfo();
            playerInfo playerTwo = new playerInfo();
            char[,] board = new char[9, 10];
	        int dropChoice, win, full, again;

            Console.WriteLine("Let's Play Connect 4");
	        Console.WriteLine("Player One please enter your name: ");
	        playerOne.playerName = Console.ReadLine();
	        playerOne.playerID = 'X';
	        Console.WriteLine("Player Two please enter your name: ");
	        playerTwo.playerName = Console.ReadLine();
	        playerTwo.playerID = 'O';

	        full = 0;
	        win = 0;
	        again = 0;
	        DisplayBoard( board );
	        do
	        {
		        dropChoice = PlayerDrop( board, playerOne ); //choose coloumn
		        CheckBellow( board, playerOne, dropChoice ); //find out which row
		        DisplayBoard( board ); //show the board
		        win = CheckFour( board, playerOne ); //check if anyone won
		        if ( win == 1 )
		        {
			        PlayerWin(playerOne);
			        again = restart(board);
			        if (again == 2)
			        {
				        break;
			        }
		        }

		        dropChoice = PlayerDrop( board, playerTwo );
		        CheckBellow( board, playerTwo, dropChoice );
		        DisplayBoard( board );
		        win = CheckFour( board, playerTwo );
		        if ( win == 1 )
		        {
			        PlayerWin(playerTwo);
			        again = restart(board);
			        if (again == 2)
			        {
				        break;
			        }
		        }
		        full = FullBoard( board );
		        if ( full == 7 )
		        {
			        Console.WriteLine( "The board is full, it is a draw!" );
			        again = restart(board);
		        }

	        }while ( again != 2 );
        }
static int PlayerDrop( char[,] board, playerInfo activePlayer ) //what coloumn we can choose
{
	int dropChoice;

		Console.WriteLine(activePlayer.playerName + "'s Turn ");
        do
        {
            Console.WriteLine("Please enter a number between 1 and 7: ");
            dropChoice = Convert.ToInt32(Console.ReadLine());
        } while (dropChoice < 1 || dropChoice > 7);

		while ( board[1 , dropChoice] == 'X' || board[1 , dropChoice] == 'O' )
		{
			Console.WriteLine("That row is full, please enter a new row: ");
			dropChoice = Convert.ToInt32(Console.ReadLine());
		}

return dropChoice;
}

static void CheckBellow ( char[,] board, playerInfo activePlayer, int dropChoice ) //what row we can choose
{
	int length, turn;
	length = 6;
	turn = 0;

	do
	{
		if ( board[length , dropChoice] != 'X' && board[length , dropChoice] != 'O' )
		{
			board[length , dropChoice] = activePlayer.playerID;
			turn = 1;
		}
		else
		--length;
	}while (  turn != 1 );


}

static void DisplayBoard ( char[,] board )
{
	int rows = 6, columns = 7, i, ix;

	for(i = 1; i <= rows; i++)
	{
		Console.Write("|");
		for(ix = 1; ix <= columns; ix++)
		{
			if(board[i , ix] != 'X' && board[i , ix] != 'O')
				board[i , ix] = '*';

			Console.Write( board[i , ix] );

		}

		Console.Write("| \n");
	}

}

static int CheckFour ( char[,] board, playerInfo activePlayer )
{
	char XO;
	int win;

	XO = activePlayer.playerID;
	win = 0;

	for( int i = 8; i >= 1; --i )
	{

		for( int ix = 9; ix >= 1; --ix )
		{

			if( board[i , ix] == XO     &&
				board[i-1 , ix-1] == XO &&
				board[i-2 , ix-2] == XO &&
				board[i-3 , ix-3] == XO )
			{
				win = 1;
			}


			if( board[i , ix] == XO   &&
				board[i , ix-1] == XO &&
				board[i , ix-2] == XO &&
				board[i , ix-3] == XO )
			{
				win = 1;
			}

			if( board[i , ix] == XO   &&
				board[i-1 , ix] == XO &&
				board[i-2 , ix] == XO &&
				board[i-3 , ix] == XO )
			{
				win = 1;
			}

			if( board[i , ix] == XO     &&
				board[i-1 , ix+1] == XO &&
				board[i-2 , ix+2] == XO &&
				board[i-3 , ix+3] == XO )
			{
				win = 1;
			}

			if ( board[i , ix] == XO   &&
				 board[i , ix+1] == XO &&
				 board[i , ix+2] == XO &&
				 board[i , ix+3] == XO )
			{
				win = 1;
			}
		}

	}

return win;
}

static int FullBoard( char[,] board ) //checks to see if board is full only check top row because obiously rest at bottom are filled if top are filled
{
	int full;
	full = 0;
	for ( int i = 1; i <= 7; ++i )
	{
		if ( board[1 , i] != '*' )
			++full;
	}

return full;
}

static void PlayerWin ( playerInfo activePlayer )
{
	Console.WriteLine( activePlayer.playerName + " Connected Four, You Win!" );
}

static int restart ( char[,] board )
{
	int restart;

	Console.WriteLine("Would you like to restart? Yes(1) No(2): ");
	restart = Convert.ToInt32(Console.ReadLine());
	if ( restart == 1 )
	{
		for(int i = 1; i <= 6; i++)
		{
			for(int ix = 1; ix <= 7; ix++)
			{
				board[i , ix] = '*';
			}
		}
	}
	else
		Console.WriteLine("Goodbye!");
return restart;
}


     }
}

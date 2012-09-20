using System;
using Gtk;
using System.Collections.Generic;

public partial class MainWindow: Gtk.Window
{	

	const bool DEBUG = true;
	const string playerX = "X", player0 = "0", emptySpot = "..";
	const int maxMoves = 9;

	string currentPlayer;
	int currentMove;
	bool currentPlayerWon = false;

	//List will hold all used buttons.
	List<string> usedPositions = new List<string>();

	/**
	 * @var positions
	 * Hold all the cordinates and its filled charachter : X or 0
	 */
	Dictionary<Tuple<int,int>, string> filledPositions = new Dictionary<Tuple<int,int>, string>();

	/**
	 * @var board
	 * Mapping of button name to the corresponding cordinate
	 * @Tuple<int,int> cooridinate
	 * @string buton name
	 * 
	 */
	Dictionary<string,Tuple<int,int>> coordinates = new Dictionary<string,Tuple<int,int>> ();

	/**
	 * list all winning combinations
	 */
	Dictionary<int, int[,]> combinations = new Dictionary<int, int[,]>();

	public MainWindow (): base (Gtk.WindowType.Toplevel)
	{
		Build ();
		//@TODO add an randomizer that picks if X or 0 can start.
		this.currentPlayer = player0; //useless but used.
		this.currentPlayer = playerX;


		//Setup wizzzzard.... Special Sound Effects: Thum thum thummm!!!!
		//The board is a 3x3 array filled with X, O or _
		for (int x=0; x < 3; x++) {
			int[] range = {0,1,2};
			foreach (int y in range) {
				var coordinate = new Tuple<int,int> (x, y);
				//this could be used for Ai or whatever
				string buttonName = "button_" + x + y + "_pos";
				coordinates [buttonName] = coordinate;

				//use the _ char for empty so that the game engine could posible run on CLI (there null cannot be immitated easy)
				filledPositions [coordinate] = emptySpot; //pre empty all positions
			}
		}

		print ("The board: ");
		foreach (KeyValuePair<string, Tuple<int,int>> i in coordinates) {
			print ("buton: " + i.Key + " has cordinate: " + i.Value);
		}

		print ("The positions: ");
		foreach (KeyValuePair<Tuple<int,int>, string> i in filledPositions) {
			print ("pos: " + i.Key + " has char: " + i.Value);
		}


		/*
		 *+****+*****+*****+
		 * 0,0 | 0,1 | 0,2 *
		 *+****+*****+*****+
		 * 1,0 | 1,1 | 1,2 *
		 *+****+*****+*****+
		 * 2,0 | 2,1 | 2,2 *
		 *+****+*****+*****+
		 */

		// list the 8 winning combinations
		this.combinations = new Dictionary<int, int[,]>();
		//all horizontal matches
		combinations[0] = new int[3,2] {{0,0}, {0,1}, {0,2}};
		combinations[1] = new int[3,2] {{1,0}, {1,1}, {1,2}};
		combinations[2] = new int[3,2] {{2,0}, {2,1}, {2,2}};
		//vertical ones
		combinations[3] = new int[3,2] {{0,0}, {1,0}, {2,0}};
		combinations[4] = new int[3,2] {{0,1}, {1,1}, {2,1}};
		combinations[5] = new int[3,2] {{0,2}, {1,2}, {2,2}};
		//diagonal
		combinations[6] = new int[3,2] {{0,0}, {1,1}, {2,2}};
		combinations[7] = new int[3,2] {{0,2}, {1,1}, {2,0}};

	}

	protected void DrawStatus ()
	{
		if (currentPlayerWon) {
			print ("Speler " + currentPlayer + " heeft gewonnen");
		} else {
			print ("speler " + this.currentPlayer + " is aan zet");
		}
	}

	protected string getChar (Button button)
	{
		Tuple<int,int> coordinate = this.coordinates[button.Name];
		return filledPositions [coordinate];
	}

	protected string getChar (int x, int y)
	{
		var coordinate = new Tuple<int,int> (x, y);
		print ("getChar(" + x + "," + y + ")");
		return filledPositions [coordinate];
	}

	/**
	 * check all combinations
	 */
	protected bool isCurrentPlayerWinner ()
	{
		print ("Check board");
		foreach(KeyValuePair<int, int[,]> row in combinations) {
			bool skipCombo = false;
			//get all 3 coordinates for current combination
			var chars = new Dictionary<int,string> ();
			for (int i=0; i < 3; i++) {
				int x = row.Value[i, 0];
				int y = row.Value[i, 1];

				string pawnChar = this.getChar(x, y);
				System.Console.Write (pawnChar + " | ");
				if (pawnChar != currentPlayer) {
					//go to next combination: no pawn/char of current player found 
					skipCombo = true;
					break; 
				}
				chars[i] = pawnChar;
			}
			if (skipCombo) continue;
			//check all chars
			foreach(KeyValuePair<int,string> pawnChar in chars) {
				print (" matching Chars : " + pawnChar.Value);
			}
			return true;
		}
		return false;
	}

	protected bool isUsedPosition(Button button)
	{
		foreach (string name in usedPositions) {
			if (button.Name == name) {
				return true;
			}
		}
		return false;
	}
	
	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}

	/**
	 * This methods represents a move on a posible position.
	 * 
	 */
	protected void clickPosition (object sender, EventArgs e)
	{

		Button button = (Button)sender;

		//is the game done?
		if (currentMove == maxMoves) {
			print ("Who whon? the game is over");
			return;
		}

		//check if button is already pressed sometime before.
		if (isUsedPosition (button)) {
			print ("button : " + button.Name + " is already used");
			return;
		}

		//still going:
		button.Label = this.currentPlayer;
		this.usedPositions.Add (button.Name);
		//fill position with char from current player
		filledPositions [coordinates [button.Name]] = currentPlayer;
		//and check board
		this.currentPlayerWon = this.isCurrentPlayerWinner ();
		if (currentPlayerWon) {
			this.DrawStatus();
			return; //stop playing!!
		}

		//prepare next round
		currentMove++;

		if (currentPlayer == playerX) {
			currentPlayer = player0;
		} else {
			currentPlayer = playerX;
		}
		//update the screen

		this.DrawStatus();
	}

	protected string origLabelname;

	protected void enterPosition (object sender, EventArgs e)
	{
		Button button = (Button)sender;
		if (this.isUsedPosition (button)) return;
		origLabelname = button.Label; //store this to use in leavePosition method
		button.Label = this.currentPlayer;
		print(button.Name + " enter");
	}

	protected void leavePosition (object sender, EventArgs e)
	{
		Button button = (Button)sender;
		if (this.isUsedPosition (button)) return;
		button.Label = origLabelname; //restore name
		print(button.Name + " leave");
	}

	private void print(string value)
	{
		if (DEBUG) System.Console.WriteLine(value);
	}

}

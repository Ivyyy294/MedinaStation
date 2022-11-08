using System;
using System.Text;
using System.Collections.Generic;

namespace OurFirstUniProject
{
	class GameValues
	{
		static private byte currentPage = 254;
		static public Stack<byte> pageHistorie = new Stack<byte>();
		static private bool key = false;
		static private bool food = false;
		static private bool water = false;
		static private bool repairPossible = true;
		static private bool machineRepaired = false;

		static public void SetCurrentPage(byte newPage)
		{
			if (Page.GetCurrentPage().GetHistorie())
				pageHistorie.Push(currentPage);
			currentPage = newPage;
		}

		static public byte GetCurrentPage()
		{
			return currentPage;
		}

		static public void GoBack()
		{
			if (pageHistorie.Count > 0)
				currentPage = pageHistorie.Pop();
			else
				currentPage = 0;
		}

		static public bool GoingBackAvailable()
		{
			if (pageHistorie.Count == 0)
				return false;
			else
			{
				return pageHistorie.Peek() != 254;
			}
		}

		static public void SetKey() {key = true;}
		static public void SetFood() { food = true; }
		static public void SetRepairPosible() { repairPossible = false; }
		static public void SetWater() { water = true; }
		static public void SetMachineRepaired() { machineRepaired = true; }
		static public bool CheckKey() {return key;}
		static public bool GetLoose() { return !GetWin(); }
		static public bool GetWin() { return water && food; }
		static public bool GetRepairPossible() { return repairPossible && !machineRepaired; }
		static public bool GetMachineRepaired() { return machineRepaired; }
		static public bool GetMachineNotRepaired() { return !machineRepaired; }
		static public void ResetGameValues()
		{
			machineRepaired = false;
			repairPossible = true;
			water = false;
			key = false;
			food = false;
			pageHistorie.Clear();
			currentPage = 254;
		}
	}
	class Answer
	{
		private string keyword;
		private byte pageNr;
		private string lockMessage = null;
		Func<bool> lockCondition = null;
		Func<bool> visibleCondition = null;

		public Answer(string key, byte p)
		{
			keyword = key;
			pageNr = p;
		}

		public void SetLockCondition(Func<bool> c, string msg)
		{
			lockCondition = c;
			lockMessage = msg;
		}

		public void SetVisibleCondition(Func<bool> c) { visibleCondition = c; }

		public bool IsVisible ()
		{
			if (visibleCondition != null)
				return visibleCondition();
			else
				return true;
		}

		public bool CheckCondition ()
		{
			if (lockCondition == null)
				return true;
			else
			{
				if (lockCondition())
					return true;
				else
				{
					if (lockMessage != null)
						Drawings.DrawCenterTextLine("\n" + lockMessage + "\n");
					else
						Drawings.DrawCenterTextLine("\nThe path is blocked...\n");

					return false;
				}
			}
		}
		public string GetKeyword() { return keyword; }
		public byte GetPageNr() { return pageNr; }
	}

	class Page
	{
		static private Dictionary<byte, Page> pageMap = new Dictionary<byte, Page>();

		static public Page GetCurrentPage() { return pageMap[GameValues.GetCurrentPage()]; }

		static public void AddPage (Page p) { pageMap.Add(p.GetPageId(), p);}

		static public void ShowPage (byte pageNr)
		{
			if (pageMap.ContainsKey(pageNr))
				pageMap[pageNr].Run();
			else
			{
				Console.Clear();
				Console.WriteLine("ERROR: Page " + pageNr.ToString() + " not found!");
				Console.ReadLine();
			}
		}

		private bool historie = true;
		private bool goBackPossible = true;
		private string content = "";
		private Action decoration = null;
		private Action action = null;
		private byte pageId;
		private Stack <Answer> answers = new Stack <Answer> ();

		public Page (byte id)
		{
			pageId = id;
		}

		public void SetHistorie(bool val) { historie = val; }
		public bool GetHistorie() { return historie; }
		public byte GetPageId () { return pageId; }
		//public void SetGoBackPossible(bool val) { goBackPossible = val; }
		public void SetAction (Action a) { action = a; }
		public void SetContent (string c) {content = c;}
		public void SetDecoration (Action a) {decoration = a;}
		public void AddAnswer (Answer a) {answers.Push(a);}
		public void AddAnswer(string text, byte pageNr) { answers.Push(new Answer(text, pageNr)); }
		
		public void Run ()
		{
			Console.Clear();

			if (action != null)
				action();

			Console.WriteLine("\n\n");

			if (decoration != null)
				decoration();

			Console.WriteLine("");

			Drawings.DrawCenterTextLine (content);

			int cursorPos = Drawings.DrawAnswerButtons(answers, goBackPossible && GameValues.GoingBackAvailable());
			Console.WriteLine("");

			while (true)
			{
				Console.SetCursorPosition(Math.Max (0, cursorPos), Console.CursorTop);
				Console.Write("\\: ");
				string input = Console.ReadLine().ToLower();

				if (input.Contains("back"))
				{
					if (GameValues.GoingBackAvailable())
					{
						GameValues.GoBack();
						return;
					}
					Drawings.DrawCenterTextLine("You can't go back!\n");
				}
				else
				{
					bool validInput = false;

					foreach (Answer i in answers)
					{
						if (i.IsVisible() && input.Contains(i.GetKeyword()))
						{
							validInput = true;

							if (i.CheckCondition())
							{
								GameValues.SetCurrentPage(i.GetPageNr());
								return;
							}
						}
					}

					if (!validInput)
						Drawings.DrawCenterTextLine("Sorry i didn't understand that!\n");
				}
			}
		}
	}

	class Drawings
	{
		readonly static private string playerToken = "X";// "\u2573";
		readonly static private string dLH = "\u2550";
		readonly static private string dLV = "\u2551";
		readonly static private string lineHV = "\u256C";
		readonly static private string dLDR = "\u2554";
		readonly static private string dLDL = "\u2557";
		readonly static private string dLDH = "\u2566";
		readonly static private string dLUR = "\u255A";
		readonly static private string dLUL = "\u255D";
		readonly static private string dLUH = "\u2569";
		readonly static private string dLVR = "\u2560";
		readonly static private string dLVL = "\u2563";
		readonly static private string lightShade = "\u2591";
		readonly static private string mediumShade = "\u2592";
		readonly static private string darkShade = "\u2593";
		readonly static private string lineH = "\u2500";
		readonly static private string lineV = "\u2502";
		readonly static private string lADR = "\u256D";
		readonly static private string lADL = "\u256E";
		readonly static private string lAUL = "\u256F";
		readonly static private string lAUR = "\u2570";
		readonly static byte maxLineLength = 50;

		static private string GetStrSequence(string str, int count)
		{
			string line = "";

			while (line.Length < count)
				line += str;

			return line;
		}

		static public int DrawAnswerButtons (Stack<Answer> answers, bool back)
		{
			Console.WriteLine("\n");

			string line = "";
			foreach (Answer i in answers)
			{
				if (i.IsVisible())
					line += "[" + i.GetKeyword() + "] ";
			}

			if (back)
				line += "[back]";

			DrawCenterText(line + "\n");

			return (Console.WindowWidth - line.Length) / 2;
		}

		static private string GetTopLine(string title = null, int length = 0)
		{
			if (length == 0)
				length = maxLineLength;

			string line = dLDR;

			if (title != null)
				line += dLH + " " + title + " ";

			line += GetStrSequence(dLH, (length - 1 - line.Length)) + dLDL + "\n";

			return line;
		}
		static private string GetBottomLine (int length = 0)
		{
			if (length == 0)
				length = maxLineLength;

			//Bottom line
			string line = dLUR + GetStrSequence (dLH, length -2) + dLUL;

			return line;
		}

		static private string GetTextLine (string text)
		{
			string line = dLV + " "  + text;

			while (line.Length < maxLineLength - 1)
				line += " ";

			line += dLV + "\n";

			return line;
		}
		static public void GetMenuDrawing ()
		{
			//Title line
			string drawing = GetTopLine("Menu");
			drawing += GetTextLine ("You are a technician on a space station.");
			drawing += GetTextLine ("It was an easy job until one day");
			drawing += GetTextLine ("disaster struck! An asteroid hit the");
			drawing += GetTextLine("station! Are you able to escape?");
			drawing += GetBottomLine();
			DrawCenterTextLine(drawing);
		}

		static public void GetPicture ()
		{
			string drawing = GetTopLine("Graduation photo");
			drawing +=		 (dLV + GetStrSequence(" ", 23) + "/\\" + GetStrSequence(" ", 23) + dLV + "\n"
								+ dLV + GetStrSequence(" ", 12) + dLDR + GetStrSequence (dLH, 22) + dLDL + GetStrSequence(" ", 12) + dLV + "\n"
								+ dLV + GetStrSequence(" ", 12) + dLV + GetStrSequence(" ", 22) + dLV + GetStrSequence(" ", 12) + dLV + "\n"
								+ dLV + GetStrSequence(" ", 12) + dLV + "      O     O    O    " + dLV + GetStrSequence(" ", 12) + dLV + "\n"
								+ dLV + GetStrSequence(" ", 12) + dLV + "     ~|~   ~|~  ~|~   " + dLV + GetStrSequence(" ", 12) + dLV + "\n"
								+ dLV + GetStrSequence(" ", 12) + dLV + "     | |   | |  | |   " + dLV + GetStrSequence(" ", 12) + dLV + "\n"
								+ dLV + GetStrSequence(" ", 12) + dLV + GetStrSequence(" ", 22) + dLV + GetStrSequence(" ", 12) + dLV + "\n"
								+ dLV + GetStrSequence(" ", 12) + dLUR + GetStrSequence(dLH, 22) + dLUL + GetStrSequence(" ", 12) + dLV + "\n"
								+ dLV + GetStrSequence(" ", 48) + dLV + "\n");
			drawing += GetBottomLine();
			DrawCenterTextLine(drawing);
		}
		static public void GetBedroom()
		{
			string drawing = GetTopLine("Bedroom");
			drawing +=		 (dLV + GetStrSequence (" ", 12) + dLDR + GetStrSequence(dLH, 8) + lineV + GetStrSequence (lineH, 4) + lineV + GetStrSequence(dLH, 8) + dLDL + GetStrSequence (" ", 12) + dLV + "\n"
								+ dLV + GetStrSequence (" ", 12) + dLV + GetStrSequence(" ", 22) + dLV + GetStrSequence (" ", 12) + dLV + "\n"
								+ dLV + GetStrSequence (" ", 12) + dLV + GetStrSequence(" ", 17) + GetStrSequence (mediumShade, 5) + dLV + GetStrSequence (" ", 12) + dLV + "\n"
								+ dLV + GetStrSequence (" ", 12) + dLV + GetStrSequence(" ", 6) + playerToken + GetStrSequence(" ", 10) + GetStrSequence(mediumShade, 5) + dLV + GetStrSequence (" ", 12) + dLV + "\n"
								+ dLV + GetStrSequence (" ", 12) + dLV + GetStrSequence(" ", 17) + GetStrSequence(mediumShade, 5) + dLV + GetStrSequence (" ", 12) + dLV + "\n"
								+ dLV + GetStrSequence (" ", 12) + dLV + GetStrSequence(" ", 17) + GetStrSequence (mediumShade, 5) + dLV + GetStrSequence (" ", 12) + dLV + "\n"
								+ dLV + GetStrSequence (" ", 12) + dLV + GetStrSequence(" ", 22) + dLV + GetStrSequence (" ", 12) + dLV + "\n"
								+ dLV + GetStrSequence (" ", 12) + dLUR + GetStrSequence (dLH, 22) + dLUL + GetStrSequence (" ", 12) + dLV + "\n");
			drawing += GetBottomLine();
			DrawCenterTextLine (drawing);
		}

		static public void GetFirstHallway()
		{
			string drawing = GetTopLine("Hallway");
			drawing += (dLV + GetStrSequence(" ", 48) + dLV + "\n"
								+ dLV + GetStrSequence(" ", 4) + GetStrSequence(dLH, 40) + GetStrSequence(" ", 4) + dLV + "\n"
								+ dLV + GetStrSequence(" ", 4) + GetStrSequence (lightShade, 12) + GetStrSequence(" ", 22) + GetStrSequence(darkShade, 4) + GetStrSequence(" ", 6) + dLV + "\n"
								+ dLV + GetStrSequence(" ", 4) + GetStrSequence (lightShade, 12) + GetStrSequence (" ", 7) + playerToken + GetStrSequence(" ", 15) + GetStrSequence(darkShade, 2) + GetStrSequence(" ", 7) + dLV + "\n"
								+ dLV + GetStrSequence(" ", 4) + GetStrSequence (lightShade, 12) + GetStrSequence(" ", 15) + GetStrSequence (darkShade, 2) + GetStrSequence(" ", 15) + dLV + "\n"
								+ dLV + GetStrSequence(" ", 4) + GetStrSequence (lightShade, 12) + GetStrSequence(" ", 14) + GetStrSequence(darkShade, 4) + GetStrSequence(" ", 14) + dLV + "\n"
								+ dLV + GetStrSequence(" ", 4) + GetStrSequence(dLH, 17) + lineV + GetStrSequence(lineH, 4) + lineV + GetStrSequence(dLH, 17) + GetStrSequence(" ", 4) + dLV + "\n"
								+ dLV + GetStrSequence(" ", 48) + dLV + "\n");
			drawing += GetBottomLine();
			DrawCenterTextLine(drawing);
		}

		static public void GetDmgHallway()
		{
			string drawing = GetTopLine("Damaged Hallway");
			drawing += (dLV + GetStrSequence(" ", 48) + dLV + "\n"
								+ dLV + GetStrSequence(" ", 4) + GetStrSequence(dLH, 17) + lineV + GetStrSequence(lineH, 4) + lineV + GetStrSequence(dLH, 17) + GetStrSequence(" ", 4) + dLV + "\n"
								+ dLV + GetStrSequence(" ", 14) + GetStrSequence(darkShade, 4) + GetStrSequence(" ", 20) + GetStrSequence(darkShade, 4) + GetStrSequence(" ", 6) + dLV + "\n"
								+ dLV + GetStrSequence(" ", 15) + GetStrSequence(darkShade, 2) + GetStrSequence(" ", 6) + playerToken + GetStrSequence(" ", 15) + GetStrSequence(darkShade, 2) + GetStrSequence(" ", 7) + dLV + "\n"
								+ dLV + GetStrSequence(" ", 31) + GetStrSequence(darkShade, 2) + GetStrSequence(" ", 15) + dLV + "\n"
								+ dLV + GetStrSequence(" ", 30) + GetStrSequence(darkShade, 4) + GetStrSequence(" ", 14) + dLV + "\n"
								+ dLV + GetStrSequence(" ", 4) + GetStrSequence(dLH, 40) + GetStrSequence(" ", 4) + dLV + "\n"
								+ dLV + GetStrSequence(" ", 48) + dLV + "\n");
			drawing += GetBottomLine();
			DrawCenterTextLine(drawing);
		}
		static public void GetWaterHallway()
		{
			string drawing = GetTopLine("Water Hallway");
			drawing += (dLV + GetStrSequence(" ", 48) + dLV + "\n"
								+ dLV + GetStrSequence(" ", 4) + GetStrSequence(dLH, 40) + GetStrSequence(" ", 4) + dLV + "\n"
								+ dLV + GetStrSequence(" ", 22) + GetStrSequence (lightShade, 22) + GetStrSequence(" ", 4) + dLV + "\n"
								+ dLV + GetStrSequence(" ", 17) + playerToken + GetStrSequence(" ", 4) + GetStrSequence(lightShade, 6) + GetStrSequence(darkShade, 3) + GetStrSequence(lightShade, 13) + GetStrSequence(" ", 4) + dLV + "\n"
								+ dLV + GetStrSequence(" ", 22) + GetStrSequence(lightShade, 14) + GetStrSequence(darkShade, 3) + GetStrSequence (lightShade, 5) + GetStrSequence(" ", 4) + dLV + "\n"
								+ dLV + GetStrSequence(" ", 22) + lightShade + GetStrSequence(darkShade, 3) + GetStrSequence (lightShade, 18) + GetStrSequence(" ", 4) + dLV + "\n"
								+ dLV + GetStrSequence(" ", 4) + GetStrSequence(dLH, 40) + GetStrSequence(" ", 4) + dLV + "\n"
								+ dLV + GetStrSequence(" ", 48) + dLV + "\n");
			drawing += GetBottomLine();
			DrawCenterTextLine(drawing);
		}

		static public void GetDarkHallway()
		{
			string drawing = GetTopLine("Dark Hallway");
			drawing += (dLV + GetStrSequence(" ", 48) + dLV + "\n"
			+ dLV + " " + GetStrSequence(lightShade, 46) + " " + dLV + "\n"
			+ dLV + " " + GetStrSequence(lightShade, 46)+ " " + dLV + "\n"
			+ dLV + " " + GetStrSequence(lightShade, 46)+ " " + dLV + "\n"
			+ dLV + " " + GetStrSequence(lightShade, 46)+ " " + dLV + "\n"
			+ dLV + " " + GetStrSequence(lightShade, 46)+ " " + dLV + "\n"
			+ dLV + GetStrSequence(" ", 48) + dLV + "\n");
			drawing += GetBottomLine();
			DrawCenterTextLine(drawing);
		}
		static public void GetControlRoom()
		{
			string drawing = GetTopLine("Control room");
			drawing += (dLV + GetStrSequence(" ", 4) + dLDR + GetStrSequence(dLH, 16) + lineV + GetStrSequence(lineH, 4) + lineV + GetStrSequence(dLH, 16) + dLDL + GetStrSequence(" ", 4) + dLV + "\n"
								+ dLV + GetStrSequence(" ", 4) + dLV + GetStrSequence(" ", 38) + dLV + GetStrSequence(" ", 4) + dLV + "\n"
								+ dLV + GetStrSequence(" ", 4) + dLUH + GetStrSequence(" ", 38) + dLUH + GetStrSequence(" ", 4) + dLV + "\n"
								+ dLV + GetStrSequence(" ", 24) + playerToken + GetStrSequence(" ", 23) + dLV + "\n"
								+ dLV + GetStrSequence(" ", 48) + dLV + "\n"
								+ dLV + GetStrSequence(" ", 4) + dLDH + GetStrSequence(" ", 5) + dLDR + dLDL + GetStrSequence(" ", 7) + dLDR + dLDL + GetStrSequence(" ", 7) + dLDR + dLDL + GetStrSequence(" ", 7) + dLDR + dLDL + GetStrSequence(" ", 4) + dLDH + GetStrSequence(" ", 4) + dLV + "\n"
								+ dLV + GetStrSequence(" ", 4) + dLV + GetStrSequence(" ", 3) + GetStrSequence (darkShade, 6) + GetStrSequence(" ", 3) + GetStrSequence(darkShade, 6) + GetStrSequence(" ", 3) + GetStrSequence(darkShade, 6) + GetStrSequence(" ", 3) + GetStrSequence(darkShade, 6) + GetStrSequence (" ", 2) + dLV + GetStrSequence(" ", 4) + dLV + "\n"
								+ dLV + GetStrSequence(" ", 4) + dLUR + GetStrSequence(dLH, 38) + dLUL + GetStrSequence(" ", 4) + dLV + "\n");
			drawing += GetBottomLine();
			DrawCenterTextLine(drawing);
		}

		static public void GetCommonRoom()
		{
			string drawing = GetTopLine("Common room");
			drawing += (dLV + GetStrSequence(" ", 4) + dLDR + GetStrSequence(dLH, 38) + dLDL + GetStrSequence(" ", 4) + dLV + "\n"
								+ dLV + GetStrSequence(" ", 4) + dLV + GetStrSequence(" ", 3) + GetStrSequence(darkShade, 6) + GetStrSequence(" ", 3) + GetStrSequence(darkShade, 6) + GetStrSequence(" ", 3) + GetStrSequence(darkShade, 6) + GetStrSequence(" ", 3) + GetStrSequence(darkShade, 6) + GetStrSequence(" ", 2) + dLV + GetStrSequence(" ", 4) + dLV + "\n"
								+ dLV + GetStrSequence(" ", 4) + dLUH + GetStrSequence(" ", 38) + dLV + GetStrSequence(" ", 4) + dLV + "\n"
								+ dLV + GetStrSequence(" ", 24) + playerToken + GetStrSequence(" ", 18) + dLV + GetStrSequence(" ", 4) + dLV + "\n"
								+ dLV + GetStrSequence(" ", 43) + dLV + GetStrSequence(" ", 4) + dLV + "\n"
								+ dLV + GetStrSequence(" ", 4) + dLDH + GetStrSequence(" ", 38) + dLV + GetStrSequence(" ", 4) + dLV + "\n"
								+ dLV + GetStrSequence(" ", 4) + dLV + GetStrSequence(" ", 38) + dLV + GetStrSequence(" ", 4) + dLV + "\n"
								+ dLV + GetStrSequence(" ", 4) + dLUR + GetStrSequence (dLH, 2) + dLVL + GetStrSequence(lineH, 4) + dLVR + GetStrSequence(dLH, 3) + dLVL + GetStrSequence(lineH, 4) + dLVR + GetStrSequence(dLH, 4) + dLVL + lADR + GetStrSequence(lineH, 2) + lADL + dLVR + GetStrSequence(dLH, 3) + dLVL + GetStrSequence(lineH, 4) + dLVR + GetStrSequence(dLH, 2) + dLUL + GetStrSequence(" ", 4) + dLV + "\n");
			drawing += GetBottomLine();
			DrawCenterTextLine(drawing);
		}

		static public void GetMaintenance()
		{
			string drawing = GetTopLine("Maintenance");
			drawing += (dLV + GetStrSequence(" ", 6) + dLDR + GetStrSequence(dLH, 14) + lineV + GetStrSequence(lineH, 4) + lineV + GetStrSequence(dLH, 14) + dLDL + GetStrSequence(" ", 6) + dLV + "\n"
								+ dLV + GetStrSequence(" ", 6) + dLV + GetStrSequence(" ", 26) + GetStrSequence(darkShade, 6) + GetStrSequence(" ", 2) + dLV + GetStrSequence(" ", 6) + dLV + "\n"
								+ dLV + GetStrSequence(" ", 6) + lineH + GetStrSequence(" ", 34) + lineH + GetStrSequence(" ", 6) + dLV + "\n"
								+ dLV + GetStrSequence(" ", 6) + lineV + GetStrSequence(" ", 34) + lineV + GetStrSequence(" ", 6) + dLV + "\n"
								+ dLV + GetStrSequence(" ", 6) + lineV + GetStrSequence(" ", 17) + playerToken + GetStrSequence(" ", 16) + lineV + GetStrSequence(" ", 6) + dLV + "\n"
								+ dLV + GetStrSequence(" ", 6) + lineH + GetStrSequence(" ", 34) + lineH + GetStrSequence(" ", 6) + dLV + "\n"
								+ dLV + GetStrSequence(" ", 6) + dLV + GetStrSequence (" ", 34) + dLV + GetStrSequence(" ", 6) + dLV + "\n"
								+ dLV + GetStrSequence(" ", 6) + dLUR + GetStrSequence(dLH, 14) + lineV + GetStrSequence(lineH, 4) + lineV + GetStrSequence(dLH, 14) + dLUL + GetStrSequence(" ", 6) + dLV + "\n");
			drawing += GetBottomLine();
			DrawCenterTextLine(drawing);
		}

		static public void GetMRoom()
		{
			string drawing = GetTopLine("Administration office");
			drawing += (dLV + GetStrSequence(" ", 4) + dLDR + GetStrSequence(dLH, 16) + lineV + GetStrSequence(lineH, 4) + lineV + GetStrSequence(dLH, 16) + dLDL + GetStrSequence(" ", 4) + dLV + "\n"
								+ dLV + GetStrSequence(" ", 4) + dLUH + GetStrSequence(" ", 38) + dLV + GetStrSequence(" ", 4) + dLV + "\n"
								+ dLV + GetStrSequence(" ", 4) + lADL + GetStrSequence(" ", 38) + dLUH + GetStrSequence(" ", 4) + dLV + "\n"
								+ dLV + GetStrSequence(" ", 4) + lineV + GetStrSequence (" ", 19) + playerToken + GetStrSequence(" ", 23) + dLV + "\n"
								+ dLV + GetStrSequence(" ", 4) + lineV + GetStrSequence(" ", 2) + "[Body]" + GetStrSequence(" ", 35) + dLV + "\n"
								+ dLV + GetStrSequence(" ", 4) + lAUL + GetStrSequence(" ", 14) + dLDR + dLDL + GetStrSequence(" ", 7) + dLDR + dLDL + GetStrSequence(" ", 13) + dLDH + GetStrSequence(" ", 4) + dLV + "\n"
								+ dLV + GetStrSequence(" ", 4) + dLDH + GetStrSequence(" ", 9) + GetStrSequence(" ", 3) + GetStrSequence(darkShade, 6) + GetStrSequence(" ", 3) + GetStrSequence(darkShade, 6) + GetStrSequence(" ", 3) + GetStrSequence(" ", 8) + dLV + GetStrSequence(" ", 4) + dLV + "\n"
								+ dLV + GetStrSequence(" ", 4) + dLUR + GetStrSequence(dLH, 38) + dLUL + GetStrSequence(" ", 4) + dLV + "\n");
			drawing += GetBottomLine();
			DrawCenterTextLine(drawing);
		}

		static public void GetDeadDrawing()
		{
			string drawing = GetTopLine();
			drawing +=		 (dLV + "           ##      ##     ##      ##            " + dLV + "\n"
								+ dLV + "             ##  ##         ##  ##              " + dLV + "\n"
								+ dLV + "               ##             ##                " + dLV + "\n"
								+ dLV + "             ##  ##         ##  ##              " + dLV + "\n"
								+ dLV + "           ##      ##     ##      ##            " + dLV + "\n"
								+ dLV + "                                                " + dLV + "\n"
								+ dLV + "                ###############                 " + dLV + "\n"
								+ dLV + "                                                " + dLV + "\n");
			drawing += GetBottomLine();
			DrawCenterTextLine(drawing);
		}
		static public void GetBlockedDrawing()
		{
			string drawing = GetTopLine();
			drawing +=		 (dLV + "             ###               ###              " + dLV + "\n"
								+ dLV + "                ###         ###                 " + dLV + "\n"
								+ dLV + "                   ###   ###                    " + dLV + "\n"
								+ dLV + "                      ###                       " + dLV + "\n"
								+ dLV + "                      ###                       " + dLV + "\n"
								+ dLV + "                   ###   ###                    " + dLV + "\n"
								+ dLV + "                ###         ###                 " + dLV + "\n"
								+ dLV + "             ###               ###              " + dLV + "\n");
			drawing += GetBottomLine();
			DrawCenterTextLine(drawing);
		}
		static public void GetIDCard()
		{
			string drawing = GetTopLine("ID-Card");
			drawing +=		 (dLV + "                                                " + dLV + "\n"
								+ dLV + "          ############################          " + dLV + "\n"
								+ dLV + "          # Mr. Black                #          " + dLV + "\n"
								+ dLV + "          #--------------------------#          " + dLV + "\n"
								+ dLV + "          #                          #          " + dLV + "\n"
								+ dLV + "          # ID: XXXX-XXXX-XXXX-94    #          " + dLV + "\n"
								+ dLV + "          ############################          " + dLV + "\n"
								+ dLV + "                                                " + dLV + "\n");
			drawing += GetBottomLine();
			DrawCenterTextLine(drawing);
		}

		static public void GetComputer()
		{
			string drawing = GetTopLine("Computer");
			drawing +=		 (dLV + GetStrSequence(" ", 12 ) + dLDR + GetStrSequence(dLH, 22 ) + dLDL + GetStrSequence(" ", 12 ) + dLV + "\n"
								+ dLV + GetStrSequence(" ", 12 ) + dLV + GetStrSequence(" ", 22) + dLV + GetStrSequence(" ", 12 ) + dLV + "\n"
								+ dLV + GetStrSequence(" ", 12 ) + dLV + GetStrSequence(" ", 22)  + dLV + GetStrSequence(" ", 12 ) + dLV + "\n"
								+ dLV + GetStrSequence(" ", 12 ) + dLV + "   [CRITICAL ERROR]   "+ dLV + GetStrSequence(" ", 12 ) + dLV + "\n"
								+ dLV + GetStrSequence(" ", 12 ) + dLV + GetStrSequence(" ", 22 ) + dLV + GetStrSequence(" ", 12 ) + dLV + "\n"
								+ dLV + GetStrSequence(" ", 12) + dLV + GetStrSequence(" ", 22) + dLV + GetStrSequence(" ", 12) + dLV + "\n"
								+ dLV + GetStrSequence(" ", 12 ) + dLUR + GetStrSequence(dLH, 9) + dLDH + GetStrSequence(dLH, 2) + dLDH + GetStrSequence(dLH, 9) + dLUL + GetStrSequence(" ", 12 ) + dLV + "\n"
								+ dLV + GetStrSequence(" ", 10) + dLDR + GetStrSequence (dLH, 11) + dLUH + GetStrSequence(dLH, 2) + dLUH + GetStrSequence(dLH, 11) + dLDL + GetStrSequence(" ", 10) + dLV + "\n");
			drawing += GetBottomLine();
			DrawCenterTextLine(drawing);
		}
		static public void GetEscapePod()
		{
			string drawing = GetTopLine("Escape pod");
			drawing += dLV + GetStrSequence(" ", 18) + dLDR + dLH + dLDH + GetStrSequence(dLH, 6) + dLDH + dLH + dLDL + GetStrSequence(" ", 18) + dLV + "\n";
			drawing += dLV + GetStrSequence(" ", 18) + dLV + " " + dLV + GetStrSequence(lightShade, 6) + dLV + " " + dLV + GetStrSequence(" ", 18) + dLV+ "\n";
			drawing += dLV + GetStrSequence(" ", 18) + dLV + " " + dLV + GetStrSequence(" ", 6) + dLV + " " + dLV + GetStrSequence(" ", 18) + dLV+ "\n";
			drawing += dLV + GetStrSequence(" ", 18) + dLV + " " + dLV + GetStrSequence(" ", 6) + dLV + " " + dLV + GetStrSequence(" ", 18) + dLV+ "\n";
			drawing += dLV + GetStrSequence(" ", 18) + dLV + " " + dLV + "[PULL]" + dLV + " " + dLV + GetStrSequence(" ", 18) + dLV+ "\n";
			drawing += dLV + GetStrSequence(" ", 18) + dLV + " " + dLUR + GetStrSequence(dLH, 6) + dLUL + " " + dLV + GetStrSequence(" ", 18) + dLV + "\n";
			drawing += dLV + GetStrSequence(" ", 18) + dLUR + GetStrSequence(dLH, 10) + dLUL + GetStrSequence(" ", 18) + dLV + "\n";
			drawing += GetBottomLine();

			DrawCenterTextLine(drawing);
		}

		static public void GetFood()
		{
			string drawing = GetTopLine("Food");
			drawing += dLV + GetStrSequence(" ", 48) + dLV + "\n";
			drawing += dLV + GetStrSequence (" ", 9) + dLDR + GetStrSequence (dLH, 7)     + dLDL + GetStrSequence (" ", 2) + dLDR + GetStrSequence(dLH, 7)      + dLDL + GetStrSequence (" ", 2) + dLDR + GetStrSequence(dLH, 7) + dLDL + GetStrSequence(" ",8) + dLV + "\n";
			drawing += dLV + GetStrSequence (" ", 9) + dLV + GetStrSequence (" ", 7)      + dLV  + GetStrSequence (" ", 2) + dLV + GetStrSequence (" ", 7)      + dLV  + GetStrSequence (" ", 2) + dLV + GetStrSequence (" ", 7)      + dLV + GetStrSequence(" ", 8) + dLV + "\n";
			drawing += dLV + GetStrSequence (" ", 9) + dLV + GetStrSequence (darkShade, 7)+ dLV  + GetStrSequence (" ", 2) + dLV + GetStrSequence (darkShade, 7)+ dLV  + GetStrSequence (" ", 2) + dLV + GetStrSequence (darkShade, 7)+ dLV +  GetStrSequence(" ", 8) + dLV + "\n";
			drawing += dLV + GetStrSequence (" ", 9) + dLV + GetStrSequence(darkShade, 7) + dLV  + GetStrSequence (" ", 2) + dLV + GetStrSequence(darkShade, 7) + dLV  + GetStrSequence (" ", 2) + dLV + GetStrSequence(darkShade, 7) + dLV + GetStrSequence(" ", 8) + dLV + "\n";
			drawing += dLV + GetStrSequence (" ", 9) + dLV + GetStrSequence(" ", 7)       + dLV  + GetStrSequence (" ", 2) + dLV + GetStrSequence(" ", 7)       + dLV  + GetStrSequence (" ", 2) + dLV + GetStrSequence(" ", 7)       + dLV + GetStrSequence(" ", 8) + dLV + "\n";
			drawing += dLV + GetStrSequence (" ", 9)  + dLUR + GetStrSequence(dLH, 7)      + dLUL + GetStrSequence (" ", 2) + dLUR + GetStrSequence(dLH, 7)      + dLUL + GetStrSequence (" ", 2) + dLUR + GetStrSequence(dLH, 7)      + dLUL + GetStrSequence(" ", 8) + dLV + "\n";
			drawing += GetBottomLine();

			DrawCenterTextLine(drawing);
		}

		static public void DrawWinScreen()
		{
			string drawing = GetTopLine();
			drawing += dLV + GetStrSequence(" ", 48) + dLV + "\n";
			drawing += dLV + GetStrSequence(" ", 48) + dLV + "\n";
			drawing += dLV + GetStrSequence(" ", 10) + "██        ██  ██  ███    ██" + GetStrSequence(" ", 11) + dLV + "\n";
			drawing += dLV + GetStrSequence(" ", 10) + "██   ██   ██  ██  ██ ██  ██" + GetStrSequence(" ", 11) + dLV + "\n";
			drawing += dLV + GetStrSequence(" ", 10) + "██ ██  ██ ██  ██  ██  ██ ██" + GetStrSequence(" ", 11) + dLV + "\n";
			drawing += dLV + GetStrSequence(" ", 10) + "███      ███  ██  ██    ███" + GetStrSequence(" ", 11) + dLV + "\n";
			drawing += dLV + GetStrSequence(" ", 48) + dLV + "\n";
			drawing += GetBottomLine();

			DrawCenterTextLine(drawing);
		}

		static public void DrawVendingMachine ()
		{
			string drawing = GetTopLine("Old vending machine");
			drawing += dLV + GetStrSequence(" ", 18) + dLDR + GetStrSequence(dLH, 10) + dLDL + GetStrSequence(" ", 18) + dLV + "\n";
			drawing += dLV + GetStrSequence(" ", 18) + dLV + GetStrSequence(" ", 10) + dLV + GetStrSequence(" ", 18) + dLV + "\n";
			drawing += dLV + GetStrSequence(" ", 18) + dLV + lightShade + lightShade + darkShade + GetStrSequence(lightShade, 5) + darkShade + lightShade + dLV + GetStrSequence(" ", 18) + dLV + "\n";
			drawing += dLV + GetStrSequence(" ", 18) + dLV + GetStrSequence("¯", 10) + dLV + GetStrSequence(" ", 18) + dLV + "\n";
			drawing += dLV + GetStrSequence(" ", 18) + dLV + darkShade + GetStrSequence (lightShade, 2) + darkShade + lightShade + darkShade + lightShade + darkShade+ lightShade + lightShade + dLV + GetStrSequence(" ", 18) + dLV + "\n";
			drawing += dLV + GetStrSequence(" ", 18) + dLV + GetStrSequence("¯", 10) + dLV + GetStrSequence(" ", 18) + dLV + "\n";
			drawing += dLV + GetStrSequence(" ", 18) + dLUR + GetStrSequence(dLH, 10) + dLUL + GetStrSequence(" ", 18) + dLV + "\n";
			drawing += GetBottomLine();

			DrawCenterTextLine(drawing);
		}

		static public void DrawLoadingScreen()
		{
			DrawCenterText ("                                              *####*  \n");
			DrawCenterText ("                                           *##     ##*\n");
			DrawCenterText ("                                           *##     ##*\n");
			DrawCenterText ("                                             *####*   \n");
			DrawCenterText ("                                          *###*       \n");
			DrawCenterText ("                  *** ### ### ***       *###*         \n");
			DrawCenterText ("              *##                 ##* *###*           \n");
			DrawCenterText ("          *##                         ####*           \n");
			DrawCenterText ("       *##                               ##*          \n");
			DrawCenterText ("     *##            *## ### ##*            ##*        \n");
			DrawCenterText ("   *##          ##*             *##          ##*      \n");
			DrawCenterText ("  *##        ##*                   *##        ##*     \n");
			DrawCenterText (" *##       ##*                       *##       ##*    \n");
			DrawCenterText ("*##       ##*                         *##       ##*   \n");
			DrawCenterText ("*##      ##*           *###*           *##      ##*   \n");
			DrawCenterText ("*##      ##*          #######          *##      ##*   \n");
			DrawCenterText ("*##      ##*           *###*           *##      ##*   \n");
			DrawCenterText ("*##       ##*                         *##       ##*   \n");
			DrawCenterText (" *##       ##*                       *##       ##*    \n");
			DrawCenterText ("  *##        ##*                   *##        ##*     \n");
			DrawCenterText ("   *##          ##*             *##          ##*      \n");
			DrawCenterText ("     *##            *## ### *##            ##*        \n");
			DrawCenterText ("       *#                                ##*          \n");
			DrawCenterText ("          *##                         ##*             \n");
			DrawCenterText ("              *##                 ##*                 \n");
			DrawCenterText("                  *** ### ### ***                     \n");

			Console.WriteLine();
			Console.WriteLine();

			DrawCenterText("•••• <<<< Welcome to Medina station>>>> ••••", 30);
			
			Console.WriteLine();
			Console.WriteLine();

			for (int i = 0; i <= 100; ++i)
			{
				string loadingText = "<<<< Loading " + i.ToString() + "% >>>>";
				DrawCenterText(loadingText);
				System.Threading.Thread.Sleep(8);
			}

			DrawCenterText("<<<< Complete! >>>>");
			Console.WriteLine("\n\n\n");
			DrawCenterText("[Press enter to continue...]", 30);

			Console.ReadLine();
		}

		static public void SetCursorPositionForText (string text)
		{
			int cPos = Math.Max(0, (Console.WindowWidth - text.Length) / 2);
			Console.SetCursorPosition(cPos, Console.CursorTop);
		}

		static public void DrawCenterText(string text)
		{
			SetCursorPositionForText(text);
			Console.Write(text);
		}

		static public void DrawCenterTextLine(string text)
		{
			string[] lines = text.Split('\n');

			foreach (string i in lines)
			{
				SetCursorPositionForText(i);
				Console.WriteLine(i);
			}
		}
		static public void DrawCenterText(string text, int delayMs)
		{
			SetCursorPositionForText(text);

			foreach (char c in text)
			{
				Console.Write(c);
				System.Threading.Thread.Sleep(delayMs);
			}
		}
	}
	class Program
	{
		static void InitContent()
		{
			Page menu = new Page(254);
			Page bedroom = new Page(0);
			Page firstHallway = new Page(1);
			Page dmgHallway = new Page(2);
			Page darkHallway = new Page(3);
			Page waterHallway = new Page(4);
			Page redDoor = new Page(5);
			Page controlRoom = new Page(6);
			Page cable = new Page(7);
			Page maintenance = new Page(8);
			Page commonRoom = new Page(9);
			Page managementRoom = new Page(10);
			Page airlock = new Page(11);
			Page body = new Page(12);
			Page pod = new Page(13);
			Page picture = new Page(14);
			Page commandRoomComputer = new Page(16);
			Page food = new Page(17);
			Page inspectPodNoFood = new Page(18);
			Page win = new Page(19);
			Page starve = new Page(20);
			Page inspectPodFood = new Page(21);
			Page vendingMachineBroke = new Page(22);
			Page vendingMachine = new Page(23);
			Page water = new Page(24);
			Page repair = new Page(25);
			Page yellowCable = new Page(26);
			Page blackCable = new Page(27);

			//Menu
			menu.SetAction(GameValues.ResetGameValues);
			menu.SetDecoration (Drawings.GetMenuDrawing);
			menu.SetContent("Do you want to play or do you want to exit?");
			menu.AddAnswer("exit", 255);
			menu.AddAnswer("play", bedroom.GetPageId());
			Page.AddPage(menu);

			//bedroom
			bedroom.SetDecoration(Drawings.GetBedroom);
			bedroom.SetContent ("You are standing inside your room. The alarm is throbbing in your head.\nYour vision is blurred, but you can see a door in front of you");
			bedroom.AddAnswer("door", firstHallway.GetPageId());
			bedroom.AddAnswer("inspect", picture.GetPageId());
			Page.AddPage(bedroom);

			//Raum 1 start
			firstHallway.SetDecoration(Drawings.GetFirstHallway);
			firstHallway.SetContent ("You are standing in the hallway. The way to the left is pitch black.\nThe way to the right looks damaged.");
			firstHallway.AddAnswer ("right", 2);
			firstHallway.AddAnswer("left", 3);
			Page.AddPage(firstHallway);

			//damaged hallway
			dmgHallway.SetDecoration(Drawings.GetDmgHallway);
			dmgHallway.SetContent("You are standing in the damaged hallway. At the inner wall is a red door and\n"
			 + "you can hear water in the distance.");
			dmgHallway.AddAnswer("water", waterHallway.GetPageId());
			dmgHallway.AddAnswer("door", redDoor.GetPageId());
			Page.AddPage(dmgHallway);

			//Dark hallway
			darkHallway.SetDecoration(Drawings.GetDarkHallway);
			darkHallway.SetContent("You can't see a thing, but there is something that feels like a door.");
			darkHallway.AddAnswer("door", maintenance.GetPageId());
			darkHallway.AddAnswer("continue", managementRoom.GetPageId());
			Page.AddPage(darkHallway);

			//Water hallway
			waterHallway.SetDecoration(Drawings.GetWaterHallway);
			waterHallway.SetContent("The corridor in front of you is waist-deep in water, only a few crates stick out.");
			waterHallway.AddAnswer("climb", controlRoom.GetPageId());
			waterHallway.AddAnswer("swim", cable.GetPageId());
			Page.AddPage(waterHallway);

			//RedDoor
			redDoor.SetDecoration(Drawings.GetBlockedDrawing);
			redDoor.SetContent("The door is disabled and everything behind is lost to space...");
			Page.AddPage(redDoor);

			//controlRoom
			controlRoom.SetDecoration(Drawings.GetControlRoom);
			controlRoom.SetContent("You are standing in the control room. All the computers are flashing red.\n"
				+ "At the inner wall is a blue door and you can hear a distorted voice in the distance.");
			controlRoom.AddAnswer("voice", commonRoom.GetPageId());
			controlRoom.AddAnswer("door", maintenance.GetPageId());
			controlRoom.AddAnswer("computer", commandRoomComputer.GetPageId());
			Page.AddPage(controlRoom);

			//cable
			cable.SetAction(GameValues.ResetGameValues);
			cable.SetDecoration(Drawings.GetDeadDrawing);
			cable.SetContent("You try to swim through the water, when suddenly a sparking cable falls from the ceiling.\n"
				+ "You feel only a sharp pain, then everything goes black.\n\n"
				+ "Do you want to try again?");
			cable.AddAnswer("exit", 255);
			cable.AddAnswer("menu", menu.GetPageId());
			cable.AddAnswer("again", bedroom.GetPageId());
			Page.AddPage(cable);

			//maintenance
			maintenance.SetDecoration(Drawings.GetMaintenance);
			maintenance.SetContent("You enter the maintenance room, in a corner stands an old dusty drink machine.\nAdditionally you can see a red door,a yellow door, a green door and a blue door.");

			Answer aVenMaBroke = new Answer("machine", vendingMachineBroke.GetPageId());
			aVenMaBroke.SetVisibleCondition(GameValues.GetMachineNotRepaired);
			maintenance.AddAnswer(aVenMaBroke);

			Answer aVenMa = new Answer("machine", vendingMachine.GetPageId());
			aVenMa.SetVisibleCondition(GameValues.GetMachineRepaired);
			maintenance.AddAnswer(aVenMa);

			maintenance.AddAnswer("blue", controlRoom.GetPageId());
			maintenance.AddAnswer("green", managementRoom.GetPageId());
			maintenance.AddAnswer("yellow", darkHallway.GetPageId());
			maintenance.AddAnswer("red", redDoor.GetPageId());
			Page.AddPage(maintenance);

			//commonRoom
			commonRoom.SetDecoration(Drawings.GetCommonRoom);
			commonRoom.SetContent("You enter the common room. The station AI demands everyone to evacuate immediately!\n"
			+ "All but one escape pod are already gone!");
			Answer enterPod = new Answer("pod", pod.GetPageId());
			enterPod.SetLockCondition(GameValues.CheckKey, "Damn! You can't use them! It is reserved for the management!");
			commonRoom.AddAnswer(enterPod);
			commonRoom.AddAnswer("inspect", food.GetPageId());
			Page.AddPage(commonRoom);

			//Management
			managementRoom.SetDecoration(Drawings.GetMRoom);
			managementRoom.SetContent("You enter the administration office and find a body lying next to an closed airlock.\nAt the inner wall is a green door.");
			managementRoom.AddAnswer("airlock", airlock.GetPageId());
			managementRoom.AddAnswer("body", body.GetPageId());
			managementRoom.AddAnswer("door", maintenance.GetPageId());
			Page.AddPage(managementRoom);

			//Airlock
			airlock.SetAction(GameValues.ResetGameValues);
			airlock.SetDecoration(Drawings.GetDeadDrawing);
			airlock.SetContent("You press the red button and with a loud bang you are hurled into space.\n"
			+ "Slowly you choke on your silent screams.\n\n"
			+ "Do you want to try again?");
			airlock.AddAnswer("exit", 255);
			airlock.AddAnswer("menu", menu.GetPageId());
			airlock.AddAnswer("again", bedroom.GetPageId());
			Page.AddPage(airlock);

			//Body
			body.SetDecoration(Drawings.GetIDCard);
			body.SetContent("Oh no! It is the CEO and he is dead.\nGuess he won't need his ID-Card any more...");
			body.SetAction(GameValues.SetKey);
			Page.AddPage(body);

			//Pod
			pod.SetDecoration(Drawings.GetEscapePod);
			pod.SetContent("You enter the escape pod and to your absolute relief, everything seems to be working.");
			Answer aNoFood = new Answer ("inspect", inspectPodNoFood.GetPageId());
			aNoFood.SetVisibleCondition(GameValues.GetLoose);
			pod.AddAnswer(aNoFood);

			Answer aFood = new Answer("inspect", inspectPodFood.GetPageId());
			aFood.SetVisibleCondition(GameValues.GetWin);
			pod.AddAnswer(aFood);

			Answer aWin = new Answer("escape", win.GetPageId());
			aWin.SetVisibleCondition(GameValues.GetWin);
			pod.AddAnswer(aWin);

			Answer aStarve = new Answer("escape", starve.GetPageId());
			aStarve.SetVisibleCondition(GameValues.GetLoose);
			pod.AddAnswer(aStarve);
			Page.AddPage(pod);

			inspectPodNoFood.SetDecoration(Drawings.GetEscapePod);
			inspectPodNoFood.SetContent("With a closer look you notice that there is no water and no food...");
			Page.AddPage(inspectPodNoFood);

			inspectPodFood.SetDecoration(Drawings.GetEscapePod);
			inspectPodFood.SetContent("With a closer look you notice that there is no water and no food...\nLuckily you already found something!");
			Page.AddPage(inspectPodFood);

			//Picture
			picture.SetDecoration(Drawings.GetPicture);
			picture.SetContent("The room is devastated, but you can still see the picture from your graduation,\ntogether with Prof. Dr. Gundolf S. Freyermuth and Prof. Björn Bartholdy.\nGood old times...");
			Page.AddPage(picture);

			//Computer
			commandRoomComputer.SetDecoration(Drawings.GetComputer);
			commandRoomComputer.SetContent("You check the computer and man.... everything is on fire!");
			Page.AddPage(commandRoomComputer);

			//Food
			food.SetDecoration(Drawings.GetFood);
			food.SetAction(GameValues.SetFood);
			food.SetContent("You rummage around in the closets and find a supply of canned goods.\n"
				+ "This will come in handy.");
			Page.AddPage(food);

			//Win
			win.SetAction(GameValues.ResetGameValues);
			win.SetDecoration(Drawings.DrawWinScreen);
			win.SetContent("You did it! You managed to escape just in the nick time and even have enough supplies to hold out until help arrives!");
			win.AddAnswer("exit", 255);
			win.AddAnswer("menu", menu.GetPageId());
			win.AddAnswer("again", bedroom.GetPageId());
			Page.AddPage(win);

			//Starve
			starve.SetAction(GameValues.ResetGameValues);
			starve.SetDecoration(Drawings.GetDeadDrawing);
			starve.SetContent("You manage to escape from the station, but to your horror there are not enough supplies...\n"
				+ "By the time rescue arrives, you have long since starved to death...");
			starve.AddAnswer("exit", 255);
			starve.AddAnswer("menu", menu.GetPageId());
			starve.AddAnswer("again", bedroom.GetPageId());
			Page.AddPage(starve);

			vendingMachineBroke.SetHistorie (false);
			vendingMachineBroke.SetContent ("The vending machine seems to be out of order...\nBut there are still a few drinks inside.");
			vendingMachineBroke.SetDecoration(Drawings.DrawVendingMachine);
			Answer aRepair = new Answer("repair", repair.GetPageId());
			aRepair.SetLockCondition (GameValues.GetRepairPossible, "It's hopeless, the motherboard is fried...");
			vendingMachineBroke.AddAnswer (aRepair);
			Page.AddPage(vendingMachineBroke);

			vendingMachine.SetHistorie(false);
			vendingMachine.SetContent("The old vending machine seems to be working again!");
			vendingMachine.SetDecoration(Drawings.DrawVendingMachine);
			vendingMachine.AddAnswer("use", water.GetPageId());
			Page.AddPage(vendingMachine);

			repair.SetHistorie(false);
			repair.SetDecoration(Drawings.DrawVendingMachine);
			repair.SetContent ("You take a look at the machine and notice some broken cable.\nDo you want to fix the black cable first or the yellow?");
			repair.AddAnswer("yellow", yellowCable.GetPageId());
			repair.AddAnswer("black", blackCable.GetPageId());
			Page.AddPage(repair);

			repair.SetHistorie(false);
			water.SetDecoration(Drawings.DrawVendingMachine);
			water.SetContent("You manage to get some bottles of water!");
			water.SetAction(GameValues.SetWater);
			Page.AddPage(water);

			yellowCable.SetHistorie(false);
			yellowCable.SetDecoration(Drawings.DrawVendingMachine);
			yellowCable.SetContent("You fix the cable and the vending machine comes back to live!");
			yellowCable.SetAction(GameValues.SetMachineRepaired);
			yellowCable.AddAnswer("use", water.GetPageId());
			Page.AddPage(yellowCable);

			blackCable.SetHistorie(false);
			blackCable.SetDecoration(Drawings.DrawVendingMachine);
			blackCable.SetContent("You fix the black cable.... and BAAAAANG!\nShoot...the motherboard is fried....");
			blackCable.SetAction(GameValues.SetRepairPosible);
			Page.AddPage(blackCable);
		}
		static void Main(string[] args)
		{
			Console.OutputEncoding = Encoding.UTF8;

			Drawings.DrawLoadingScreen();

			InitContent();

			while (GameValues.GetCurrentPage() != 255)
				Page.ShowPage(GameValues.GetCurrentPage());

			Drawings.DrawCenterTextLine("Good bye! Take care :)");
			Console.ReadLine();
		}
	}
}
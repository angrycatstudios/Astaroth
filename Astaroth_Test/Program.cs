using System;
using System.Collections.Generic;
using System.Diagnostics;
using Astaroth;

namespace Astaroth_Test {

class Program {

	static void Main()
	{
		Console.CursorVisible = false;

		do {
			Console.Clear();
			TestPathFinderRectGrid2D(2, 2); // <-- set mapMode & testMode arguments here!
			Console.WriteLine();
			Console.WriteLine("Press any key to run another test, or [ESC] to exit!");
		} while (Console.ReadKey(true).Key != ConsoleKey.Escape);
	}

	private static void TestPathFinderRectGrid2D(int mapMode, int testMode)
	{
		Random random = new Random();

		const int mx = 200;
		const int my = 50;
		const int passabilityRate = 5;

		Console.BufferWidth = Console.WindowWidth = mx + 1;
		Console.BufferHeight = Console.WindowHeight = my + 10;

		bool[,] map = new bool[mx, my];
		for (int y = 0; y < my; y++) {
			for (int x = 0; x < mx; x++) {
				map[x, y] = random.Next(0, passabilityRate) > 0;
			}
		}

		switch (mapMode)
		{
		// Adding an almost full-length horizontal delimiter with one random gap
		case 1:
			int x = random.Next(0, mx);
			int y = random.Next(my / 4, my / 4 * 3 + 1);
			for (int i = 0; i < mx; i++) { map[i, y] = x == i; }
			break;
		// Adding two delimiters
		case 2:
			int x1 = random.Next(0, mx);
			int x2 = random.Next(0, mx);
			int y1 = random.Next(my / 4, my / 2);
			int y2 = random.Next(y1 + 1, my / 4 * 3 + 1);
			for (int i = 0; i < mx; i++) { map[i, y1] = x1 == i; map[i, y2] = x2 == i; }
			break;
		}

		PrintMap(map);
		Console.WriteLine();

		PathFinderRectGrid2D pf;
		List<PathNode2D> path = null;

		Stopwatch sw = new Stopwatch();

		switch (testMode)
		{
		// Only straight directions
		case 0:
			pf = new PathFinderRectGrid2D(map);
			path = pf.FindPath(0, 0, mx - 1, my - 1);
			break;
		// All 8 directions
		case 1:
			pf = new PathFinderRectGrid2D(map);
			path = pf.FindPath(0, 0, mx - 1, my - 1, 1, 8, 1);
			break;
		// Example special usage:
		// -- eliminate zig-zagging by weighting moves based on being diagonal or not, and pre-sorting the open stack;
		// -- only allow diagonal movement if not blocked by a cornering wall.
		// Since we do not differentiate between the "cost" of a straight and a diagonal move, this will result in the pathfinder moving diagonally just before it would "hit" an obstacle,
		// to save the one extra move that would arise from not being able to move diagonally to evade the obstacle when it's next to it.
		case 2:
			pf = new ExampleCustomPathFinder(map);
			PathFinderRectGrid2D.Options options = new PathFinderRectGrid2D.Options() {
				enableOpenStackSorting = true
			};
			Dictionary<string, int> passConditions = new Dictionary<string, int> {
				{"diagonalBlocking", 1}
			};
			sw.Start();
			path = pf.FindPath(0, 0, mx - 1, my - 1, 1, 8, 1, options, null, passConditions, null);
			sw.Stop();
			break;
		}

		if (path != null) {
			DrawPath(path);
			Console.SetCursorPosition(0, my);
			Console.WriteLine();
			Console.WriteLine("Path found in " + sw.ElapsedMilliseconds + " ms (" + sw.ElapsedTicks + " ticks).");
		} else {
			Console.WriteLine("No path found!");
		}
	}

	private static void PrintMap(bool[,] map)
	{
		Console.ForegroundColor = ConsoleColor.Gray;
		for (int y = 0; y < map.GetLength(1); y++) {
			string line = string.Empty;
			for (int x = 0; x < map.GetLength(0); x++) {
				line += map[x, y] ? "." : "#";
			}
			Console.WriteLine(line);
		}
	}

	private static void DrawPath(List<PathNode2D> path)
	{
		foreach (PathNode2D node in path) {
			Console.SetCursorPosition(node.X, node.Y);
			Console.ForegroundColor = ConsoleColor.Green;
			Console.Write("O");
			Console.ForegroundColor = ConsoleColor.Gray;
		}
	}

}

}
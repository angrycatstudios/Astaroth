using System;
using System.Collections.Generic;
using static Astaroth.Utilities;

namespace Astaroth {

[Serializable()]
public class PathFinderRectGrid2D {

#region Fields

	protected int mx, my;
	protected PathNode2D[,] map;

#endregion

#region Constructors

	public PathFinderRectGrid2D(int x, int y) { Initialize(x, y); }

	public PathFinderRectGrid2D(bool[,] map)
	{
		Initialize(map.GetLength(0), map.GetLength(1));
		for (int x = 0; x < mx; x++) {
			for (int y = 0; y < my; y++) {
				this.map[x, y].SetProperty("isPassable", map[x, y]);
			}
		}
	}

#endregion

#region Functions

	protected void Initialize(int sx, int sy)
	{
		mx = sx;
		my = sy;

		map = new PathNode2D[mx, my];
		for (int x = 0; x < mx; x++) {
			for (int y = 0; y < my; y++) {
				map[x, y] = new PathNode2D(x, y);
			}
		}
	}

	public List<PathNode2D> FindPath(
		int startX, int startY,
		int endX, int endY,
		int dirStart = 1, int dirEnd = 7, int dirStep = 2,
		Options options = null,
		Dictionary<string, int> sortingOptions = null,
		Dictionary<string, int> passConditions = null,
		Dictionary<string, int> pathWeights = null
	) {
		// Pre-check
		if (map == null) throw new Exception("Map array is not initialized!");
		if (!IsInMapBounds(startX, startY)) throw new Exception("Starting coordinates are out of map bounds!");
		if (!IsInMapBounds(endX, endY)) throw new Exception("Target coordinates are out of map bounds!");
		if (dirStart < 0 || dirEnd > 8) throw new Exception("Invalid directional unit vector array start/end index!");

		// Clear map/path flags
		for (int x = 0; x < mx; x++) {
			for (int y = 0; y < my; y++) {
				map[x, y].SetProperty("isProcessed", false);
				map[x, y].SetProperty("isPath", false);
			}
		}

		// Init.
		if (options == null) options = new Options();
		List<PathNode2D> toBeProcessed = new List<PathNode2D>();
		List<PathNode2D> processed = new List<PathNode2D>();
		map[startX, startY].SetProperty("isProcessed", true);

		// Starting node
		PathNode2D n = new PathNode2D(map[startX, startY]);
		n.SetProperty("parent", -1);
		n.SetProperty("stack", 0);
		processed.Add(n);
		toBeProcessed.Add(n);

		/// Find path
		/// ---------
		int pathFound = -1;
		while (toBeProcessed.Count > 0 && pathFound == -1)
		{

		// Sort open stack
		if (options.enableOpenStackSorting) SortOpenStack(ref toBeProcessed, sortingOptions);

		// Process open stack
		for (int i = toBeProcessed.Count - 1; i >= 0; i--) {
			for (int d = dirStart; d <= dirEnd; d += dirStep) {

				// Pre-check
				if (d < 0 || d > 8) throw new Exception("Directional unit vector array index out of bounds!");

				// Get & validate coords
				int newX = toBeProcessed[i].X + dirRectGrid2D[d, 0];
				int newY = toBeProcessed[i].Y + dirRectGrid2D[d, 1];
				if (!IsInMapBounds(newX, newY)) continue;

				// Check conditions
				PathNode2D mapNode = map[newX, newY];
				if (mapNode.GetBool("isProcessed")) continue;
				if (!VerifyPassConditions(mapNode, toBeProcessed[i], d, passConditions)) continue;

				// Process node
				map[newX, newY].SetProperty("isProcessed", true);
				PathNode2D node = new PathNode2D(map[newX, newY]);
				node.SetProperty("parent", toBeProcessed[i].GetInt("stack"));
				node.SetProperty("stack", processed.Count);
				AssignNodeProperties(node, toBeProcessed[i], d, passConditions, pathWeights); // use this to propagate additional data like accumulated custom weights (number of direction changes, hazards encountered, etc.) from source to target node
				processed.Add(node);
				toBeProcessed.Add(node);
				if (newX == endX && newY == endY) pathFound = processed.Count - 1;

			}
			toBeProcessed.RemoveAt(i);
		}

		}
		if (pathFound == -1) return null;
		/// ---------

		// Construct path
		List<PathNode2D> path = new List<PathNode2D>();

		map[processed[pathFound].X, processed[pathFound].Y].SetProperty("isPath", true);
		path.Add(processed[pathFound]);

		int parent = processed[pathFound].GetInt("parent");
		do {
			int px = processed[parent].X;
			int py = processed[parent].Y;
			map[px, py].SetProperty("isPath", true);
			if (px != startX || py != startY) path.Add(processed[parent]);
			parent = processed[parent].GetInt("parent");
		} while (parent != -1);

		return path;
	}

	public PathNode2D GetPathNode(int x, int y)
	{
		if (!IsInMapBounds(x, y)) return null;
		else return map[x, y];
	}

#endregion

#region Strategy

	protected virtual void SortOpenStack(ref List<PathNode2D> stack, Dictionary<string, int> sortingOptions) { }

	protected virtual bool VerifyPassConditions(PathNode2D node, PathNode2D parent, int d, Dictionary<string, int> passConditions)
	{
		return node.GetBool("isPassable");
	}

	protected virtual void AssignNodeProperties(PathNode2D node, PathNode2D parent, int d, Dictionary<string, int> passConditions, Dictionary<string, int> pathWeights) { }

#endregion

#region Miscellaneous

	public bool IsInMapBounds(int x, int y)
	{
		return x >= 0 && x < mx && y >= 0 && y < my;
	}

#endregion

#region Classes

	public class Options
	{
		public bool enableOpenStackSorting = false;
	}

#endregion

}

}
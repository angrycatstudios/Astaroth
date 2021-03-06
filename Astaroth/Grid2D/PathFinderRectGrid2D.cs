﻿using System;
using System.Collections.Generic;
using static Astaroth.Utilities;

namespace Astaroth.Grid2D {

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

	public GridPath2D FindPath(
		int startX, int startY,
		int endX, int endY,
		bool allowStraight = true,
		bool allowDiagonal = true,
		Options options = null,
		Dictionary<string, int> sortingOptions = null,
		Dictionary<string, int> passConditions = null,
		Dictionary<string, int> pathWeights = null
	) {
		// Pre-check
		if (map == null) throw new Exception("Map array is not initialized!");
		if (!IsInMapBounds(startX, startY)) throw new Exception("Starting coordinates are out of map bounds!");
		if (!IsInMapBounds(endX, endY)) throw new Exception("Target coordinates are out of map bounds!");

		// Clear map/path flags
		for (int x = 0; x < mx; x++) {
			for (int y = 0; y < my; y++) {
				map[x, y].SetProperty("isProcessed", false);
				map[x, y].SetProperty("isPath", false);
				map[x, y].SetProperty("isBetterCheckEnabled", Math.Sqrt(Math.Pow(x - endX, 2) + Math.Pow(y - endY, 2)) < options.isBetterCheckRadius);
			}
		}

		// Init.
		if (options == null) options = new Options();
		List<PathNode2D> toBeProcessed = new List<PathNode2D>();
		List<PathNode2D> processed = new List<PathNode2D>();
		List<PathNode2D> endPoints = new List<PathNode2D>();
		map[startX, startY].SetProperty("isProcessed", true);

		// Starting node
		PathNode2D n = new PathNode2D(map[startX, startY]);
		n.SetProperty("parent", -1);
		n.SetProperty("stack", 0);
		processed.Add(n);
		toBeProcessed.Add(n);

		/// Find path
		/// ---------
		while (toBeProcessed.Count > 0 && endPoints.Count < options.maxResults)
		{

		// Sort open stack
		if (options.enableOpenStackSorting) SortOpenStack(ref toBeProcessed, sortingOptions);

		// Process open stack
		for (int i = toBeProcessed.Count - 1; i >= 0; i--) {

			PathNode2D node = toBeProcessed[i];

			for (int j = 7; j >= 0; j--) {

				// Get direction
				int d = dirRectGrid2DEnumHelper[node.Dir][j];
				if (!allowStraight && d % 2 == 1) continue;
				if (!allowDiagonal && d % 2 == 0) continue;

				// Get & validate coords
				int newX = node.X + dirRectGrid2D[d, 0];
				int newY = node.Y + dirRectGrid2D[d, 1];
				if (!IsInMapBounds(newX, newY)) continue;

				// Check conditions
				PathNode2D mapNode = map[newX, newY];
				if (mapNode.GetBool("isProcessed") && (!mapNode.GetBool("isBetterCheckEnabled") || !IsBetterThan(node, mapNode))) continue;
				if (!VerifyPassConditions(mapNode, node, d, passConditions)) continue;

				// Process node
				PathNode2D nextNode = new PathNode2D(map[newX, newY]);
				nextNode.SetProperty("isProcessed", true);
				nextNode.SetProperty("parent", node.GetInt("stack"));
				nextNode.SetProperty("stack", processed.Count);
				SetDistance(nextNode, node);
				AssignNodeProperties(nextNode, node, d, passConditions, pathWeights); // use this to propagate additional data like accumulated custom weights (number of direction changes, hazards encountered, etc.) from source to target node
				processed.Add(nextNode);
				map[newX, newY] = nextNode;

				// Target found
				if (nextNode.X == endX && nextNode.Y == endY) {
					endPoints.Add(nextNode);
				} else {
					toBeProcessed.Add(nextNode);
				}

			}

			toBeProcessed.RemoveAt(i);

		}

		}
		/// ---------

		List<PathNode2D> path = new List<PathNode2D>();

		/// Construct path
		/// ---------
		if (endPoints.Count > 0) {

		SortOpenStack(ref endPoints, sortingOptions);
		int pathFound = endPoints.Count - 1;

		map[endPoints[pathFound].X, endPoints[pathFound].Y].SetProperty("isPath", true);
		path.Add(endPoints[pathFound]);

		int parent = endPoints[pathFound].GetInt("parent");
		do {
			int px = processed[parent].X;
			int py = processed[parent].Y;
			map[px, py].SetProperty("isPath", true);
			if (px != startX || py != startY) path.Add(processed[parent]);
			parent = processed[parent].GetInt("parent");
		} while (parent != -1);

		}
		/// ---------

		// Result
		GridPath2D result = new GridPath2D(path);

		return result;
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

	protected virtual void SetDistance(PathNode2D node, PathNode2D parent)
	{
		node.Dist = parent.Dist + 1;
	}

	protected virtual void AssignNodeProperties(PathNode2D node, PathNode2D parent, int d, Dictionary<string, int> passConditions, Dictionary<string, int> pathWeights)
	{
		node.Dir = d;
	}

	protected virtual bool IsBetterThan(PathNode2D a, PathNode2D b)
	{
		return (a.Dist + 1) < b.Dist;
	}

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
		public int isBetterCheckRadius = 3;
		public int maxResults = 3;
	}

#endregion

}

}
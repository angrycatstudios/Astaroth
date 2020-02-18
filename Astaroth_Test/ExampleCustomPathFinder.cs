using System.Collections.Generic;
using Astaroth.Grid2D;
using static Astaroth.Utilities;

namespace Astaroth_Test {

public class ExampleCustomPathFinder : PathFinderRectGrid2D {

	public ExampleCustomPathFinder(int x, int y) : base(x, y) { }

	public ExampleCustomPathFinder(bool[,] map) : base(map) { }

	protected override void SortOpenStack(ref List<PathNode2D> stack, Dictionary<string, int> sortingOptions)
	{
		stack.Sort(ComparePathWeights);
	}

	private static int ComparePathWeights(PathNode2D a, PathNode2D b)
	{
		return a.GetInt("pathWeight").CompareTo(b.GetInt("pathWeight")); // more is better (% results 1 for straight dirs)
	}

	protected override bool VerifyPassConditions(PathNode2D node, PathNode2D parent, int d, Dictionary<string, int> passConditions)
	{
		bool isPassable = true;

		// Example #1: block diagonal movement where a corner wall would have been hit
		if (d % 2 == 0 & passConditions["diagonalBlocking"] == 1) {
			if (IsInMapBounds(parent.X + dirRectGrid2D[RectGrid2D_NextDir(d), 0], parent.Y + dirRectGrid2D[RectGrid2D_NextDir(d), 1])) isPassable = isPassable && map[parent.X + dirRectGrid2D[RectGrid2D_NextDir(d), 0], parent.Y + dirRectGrid2D[RectGrid2D_NextDir(d), 1]].GetBool("isPassable");
			if (IsInMapBounds(parent.X + dirRectGrid2D[RectGrid2D_PrevDir(d), 0], parent.Y + dirRectGrid2D[RectGrid2D_PrevDir(d), 1])) isPassable = isPassable && map[parent.X + dirRectGrid2D[RectGrid2D_PrevDir(d), 0], parent.Y + dirRectGrid2D[RectGrid2D_PrevDir(d), 1]].GetBool("isPassable");
		}

		return isPassable && base.VerifyPassConditions(node, parent, d, passConditions);
	}

	protected override void AssignNodeProperties(PathNode2D node, PathNode2D parent, int d, Dictionary<string, int> passConditions, Dictionary<string, int> pathWeights)
	{
		// Example #1: use different path weights for straight/diagonal movement
		int pathWeight = parent.GetInt("pathWeight") + d % 2;
		node.SetProperty("pathWeight", pathWeight);

		base.AssignNodeProperties(node, parent, d, passConditions, pathWeights);
	}

}

}
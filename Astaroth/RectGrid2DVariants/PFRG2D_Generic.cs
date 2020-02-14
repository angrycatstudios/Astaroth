using System.Collections.Generic;
using static Astaroth.Utilities;

namespace Astaroth {

public class PFRG2D_Generic : PathFinderRectGrid2D {

	public PFRG2D_Generic(int x, int y) : base(x, y) { }

	public PFRG2D_Generic(bool[,] map) : base(map) { }

	public GridPath2D FindPath(int startX, int startY, int endX, int endY)
	{
		Options options = new Options() {
			enableOpenStackSorting = true
		};
		Dictionary<string, int> sortingOptions = new Dictionary<string, int> {

		};
		Dictionary<string, int> passConditions = new Dictionary<string, int> {
			{"diagonalBlocking", 1}
		};
		Dictionary<string, int> pathWeightsPrf = new Dictionary<string, int> {

		};
		return FindPath(startX, startY, endX, endY, 1, 8, 1, options, sortingOptions, passConditions, pathWeightsPrf);
	}

	protected override void SortOpenStack(ref List<PathNode2D> stack, Dictionary<string, int> sortingOptions)
	{
		stack.Sort(ComparePathWeights);
	}

	private static int ComparePathWeights(PathNode2D a, PathNode2D b)
	{
		return a.GetInt("pathWeight").CompareTo(b.GetInt("pathWeight"));
	}

	protected override bool VerifyPassConditions(PathNode2D node, PathNode2D parent, int d, Dictionary<string, int> passConditions)
	{
		bool isPassable = true;

		if (d % 2 == 0 & passConditions["diagonalBlocking"] == 1) {
			if (IsInMapBounds(parent.X + dirRectGrid2D[RectGrid2D_NextDir(d), 0], parent.Y + dirRectGrid2D[RectGrid2D_NextDir(d), 1])) isPassable = isPassable && map[parent.X + dirRectGrid2D[RectGrid2D_NextDir(d), 0], parent.Y + dirRectGrid2D[RectGrid2D_NextDir(d), 1]].GetBool("isPassable");
			if (IsInMapBounds(parent.X + dirRectGrid2D[RectGrid2D_PrevDir(d), 0], parent.Y + dirRectGrid2D[RectGrid2D_PrevDir(d), 1])) isPassable = isPassable && map[parent.X + dirRectGrid2D[RectGrid2D_PrevDir(d), 0], parent.Y + dirRectGrid2D[RectGrid2D_PrevDir(d), 1]].GetBool("isPassable");
		}

		return isPassable && base.VerifyPassConditions(node, parent, d, passConditions);
	}

	protected override void AssignNodeProperties(PathNode2D node, PathNode2D parent, int d, Dictionary<string, int> passConditions, Dictionary<string, int> pathWeights)
	{
		node.SetProperty("dir", d);

		int pathWeight = parent.GetInt("pathWeight") + d % 2;
		node.SetProperty("pathWeight", pathWeight);
	}

}

}
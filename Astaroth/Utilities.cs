using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Astaroth {

public static class Utilities {

	public static readonly int[,] dirRectGrid2D = new int[9, 2] { {0, 0}, {0, -1}, {1, -1}, {1, 0}, {1, 1}, {0, 1}, {-1, 1}, {-1, 0}, {-1, -1} };
	public static readonly List<int[]> dirRectGrid2DEnumHelper = new List<int[]> {
		new int[] {1, 2, 3, 4, 5, 6, 7, 8},
		new int[] {1, 8, 2, 7, 3, 6, 4, 5},
		new int[] {2, 1, 3, 8, 4, 7, 5, 6},
		new int[] {3, 2, 4, 1, 5, 8, 6, 7},
		new int[] {4, 3, 5, 2, 6, 1, 7, 8},
		new int[] {5, 4, 6, 3, 7, 2, 8, 1},
		new int[] {6, 5, 7, 4, 8, 3, 1, 2},
		new int[] {7, 6, 8, 5, 1, 4, 2, 3},
		new int[] {8, 7, 1, 6, 2, 5, 3, 4}
	};

	public static int RectGrid2D_FlipDir(int dr) { return dr > 4 ? dr - 4 : dr + 4; }
	public static int RectGrid2D_NextDir(int dr) { return dr + 1 >= 9 ? 1 : dr + 1; }
	public static int RectGrid2D_PrevDir(int dr) { return dr - 1 <= 0 ? 8 : dr - 1; }

	public static T DeepCopy<T>(T other)
	{
		using (MemoryStream ms = new MemoryStream())
		{
			BinaryFormatter formatter = new BinaryFormatter();
			formatter.Serialize(ms, other);
			ms.Position = 0;
			return (T) formatter.Deserialize(ms);
		}
	}

}

}
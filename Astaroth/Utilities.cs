using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Astaroth {

public static class Utilities {

	public static readonly int[,] dirRectGrid2D = new int[9, 2] { {0, 0}, {0, -1}, {1, -1}, {1, 0}, {1, 1}, {0, 1}, {-1, 1}, {-1, 0}, {-1, -1} };
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
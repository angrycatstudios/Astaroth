using System;
using System.Collections.Generic;

namespace Astaroth {

[Serializable()]
public class GridPath2D {

	public List<PathNode2D> Path	{ get; }
	public PathNode2D PathStart		{ get; }
	public PathNode2D PathEnd		{ get; }

	public GridPath2D(List<PathNode2D> path)
	{
		Path = path;
		PathStart	= Path.Count > 0 ? Path[Path.Count - 1] : null;
		PathEnd		= Path.Count > 0 ? Path[0] : null;
	}

}

}
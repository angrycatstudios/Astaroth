using System;
using System.Collections.Generic;

namespace Astaroth {

[Serializable()]
public class PathNode2D {

	public int X { get; }
	public int Y { get; }

	protected Dictionary<string, int>		properties_i = new Dictionary<string, int>();
	protected Dictionary<string, string>	properties_s = new Dictionary<string, string>();
	protected Dictionary<string, float>		properties_f = new Dictionary<string, float>();
	protected Dictionary<string, bool>		properties_b = new Dictionary<string, bool>();

	public PathNode2D(int x, int y)
	{
		X = x;
		Y = y;
	}

	public PathNode2D(PathNode2D source)
	{
		X = source.X;
		Y = source.Y;

		foreach (KeyValuePair<string, int>		p in source.properties_i) properties_i.Add(p.Key, p.Value);
		foreach (KeyValuePair<string, string>	p in source.properties_s) properties_s.Add(p.Key, p.Value);
		foreach (KeyValuePair<string, float>	p in source.properties_f) properties_f.Add(p.Key, p.Value);
		foreach (KeyValuePair<string, bool>		p in source.properties_b) properties_b.Add(p.Key, p.Value);
	}

	public int GetInt(string key)
	{
		if (properties_i.ContainsKey(key)) return properties_i[key];
		return default;
	}

	public string GetStr(string key)
	{
		if (properties_s.ContainsKey(key)) return properties_s[key];
		return default;
	}

	public float GetFloat(string key)
	{
		if (properties_f.ContainsKey(key)) return properties_f[key];
		return default;
	}

	public bool GetBool(string key)
	{
		if (properties_b.ContainsKey(key)) return properties_b[key];
		return default;
	}

	public void SetProperty(string key, int value)
	{
		if (!properties_i.ContainsKey(key)) properties_i.Add(key, value);
		else properties_i[key] = value;
	}

	public void SetProperty(string key, string value)
	{
		if (!properties_s.ContainsKey(key)) properties_s.Add(key, value);
		else properties_s[key] = value;
	}

	public void SetProperty(string key, float value)
	{
		if (!properties_f.ContainsKey(key)) properties_f.Add(key, value);
		else properties_f[key] = value;
	}

	public void SetProperty(string key, bool value)
	{
		if (!properties_b.ContainsKey(key)) properties_b.Add(key, value);
		else properties_b[key] = value;
	}

}

}
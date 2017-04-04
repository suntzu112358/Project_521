public struct Position2D
{
	public int x;
	public int y;

	public Position2D(int x, int y){
		this.x = x;
		this.y = y;
	}
		
	public override bool Equals (object obj)
	{
		if (obj is Position2D) 
		{
			Position2D item = (Position2D)obj;
			return x == item.x && y == item.y;
		} else {
			return false;
		}
	}

	public static bool operator ==(Position2D p1, Position2D p2) 
	{
		return p1.Equals(p2);
	}

	public static bool operator !=(Position2D p1, Position2D p2) 
	{
		return !p1.Equals(p2);
	}

	//Written in to suppress warnings. These objects are never used as keys for dictionaries
	public override int GetHashCode ()
	{
		return (x * y).GetHashCode ();
	}
}
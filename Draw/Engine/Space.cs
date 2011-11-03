using System;
namespace CA.Engine
{
	public class Space
	{
		public int Width = 0;
		public int Height = 0;
		public int[] Cells;
		public int[] Ages;
		
		public Space ()
		{
		}
		
		public Space (int w, int h)
		{
			Width = w;
			Height = h;
			int size = w * h;
			Cells = new int[size];
			Ages = new int[size];
		}
	}
}


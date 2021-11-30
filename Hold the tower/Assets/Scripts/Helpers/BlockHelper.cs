using System.Linq;

public static class BlockHelper
{
	public static int GetBlockID(string _input)
	{
		if(_input.Contains("("))
		{
			return int.Parse(new string(_input.Where(char.IsDigit).ToArray()));
		}
		else
		{
			return 0;
		}
	}
}

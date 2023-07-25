<Query Kind="Program" />

void Main()
{
}

IEnumerable<int> SomeMethod()
{
	try
	{
		yield return 13;
	}
	finally
	{
		"hi!".Dump();
	}
}

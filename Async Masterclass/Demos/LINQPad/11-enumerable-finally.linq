<Query Kind="Program" />

void Main()
{
	foreach (var item in SomeMethod())
		item.Dump("Yielded item");
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

<Query Kind="Program" />

void Main()
{
	foreach (var item in SomeMethod())
		item.Dump("Yielded item");
}

IEnumerable<int> SomeMethod()
{
	yield return 13;
}

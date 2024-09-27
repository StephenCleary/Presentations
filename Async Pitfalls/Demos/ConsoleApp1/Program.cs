int length = await DownloadSomethingAsync();
Console.WriteLine($"Length: {length}");

async Task<int> DownloadSomethingAsync()
{
	using HttpClient client = new();

	string response = await client.GetStringAsync("https://www.google.com");
	return response.Length;
	//Task<string> responseTask = client.GetStringAsync("https://www.google.com");
	//string response = await responseTask;
	//int result = response.Length; // (breakpoint here)
	//return result;

	// await == "asynchronous wait"
	// async/await == type wrapper / type unwrapper
	// Implemented using callbacks (call stack inverts when completing)
}

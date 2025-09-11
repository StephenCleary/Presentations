using System.Threading.Channels;

await BasicProducerConsumerExampleAsync();

async Task BasicProducerConsumerExampleAsync()
{
	Channel<int> ints = Channel.CreateBounded<int>(capacity: 5);

	var producer = Task.Run(async () =>
	{
		try
		{
			for (int i = 0; i != 10000; ++i)
				await ints.Writer.WriteAsync(i);
			// throw new InvalidOperationException("test");
			ints.Writer.TryComplete();
		}
		catch (Exception ex)
		{
			ints.Writer.TryComplete(ex);
		}
	});

	var consumer = Task.Run(async () =>
	{
		int count = 0;
		await foreach (var item in ints.Reader.ReadAllAsync())
			++count;
		Console.WriteLine(count);
	});

	await consumer;
}

async Task MoreComplexPipelineExampleAsync()
{
	Channel<int> ints = Channel.CreateBounded<int>(capacity: 5);
	Channel<string> strings = Channel.CreateBounded<string>(capacity: 5);

	var producer = Task.Run(async () =>
	{
		try
		{
			for (int i = 0; i != 10000; ++i)
				await ints.Writer.WriteAsync(i);
			// throw new InvalidOperationException("test");
			ints.Writer.TryComplete();
		}
		catch (Exception ex)
		{
			ints.Writer.TryComplete(ex);
		}
	});

	var transformer = Task.Run(async () =>
	{
		try
		{
			await foreach (var item in ints.Reader.ReadAllAsync())
				await strings.Writer.WriteAsync(item.ToString());
			strings.Writer.TryComplete();
		}
		catch (Exception ex)
		{
			strings.Writer.TryComplete(ex);
		}
	});

	var consumer = Task.Run(async () =>
	{
		int count = 0;
		await foreach (var item in strings.Reader.ReadAllAsync())
			++count;
		Console.WriteLine(count);
	});

	await consumer;
}

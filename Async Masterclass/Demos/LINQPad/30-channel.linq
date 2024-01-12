<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Threading.Channels</Namespace>
</Query>

async Task Main()
{
	Channel<int> buffer = Channel.CreateBounded<int>(capacity: 5);
	ChannelReader<int> reader = buffer.Reader;
	ChannelWriter<int> writer = buffer.Writer;
	
	var producer = Task.Run(async () =>
	{
		for (int i = 0; i != 10000; ++i)
			await writer.WriteAsync(i);
		writer.Complete();
	});
	var consumer = Task.Run(async () =>
	{
		int count = 0;
		await foreach (var item in reader.ReadAllAsync())
			++count;
		count.Dump();
	});
	
	await Task.WhenAll(producer, consumer);
}

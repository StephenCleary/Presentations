using System.Threading.Tasks;

namespace SystemUnderTest
{
    partial class Sut
    {
        // Poor man's DI...
        public IService Service { get; set; }

        public async Task UseServiceAsync()
        {
            var task = Service.DoSomethingAsync();
            await task;
        }
    }
}

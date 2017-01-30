using System;
using System.IO;
using RestSharp;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleApp2
{

    public interface IOperation
    {
        Guid OperationId { get; }
    }

    public interface IOperationTransient : IOperation
    {
    }
    public interface IOperationScoped : IOperation
    {
    }
    public interface IOperationSingleton : IOperation
    {
    }
    public interface IOperationSingletonInstance : IOperation
    {
    }

    public interface IOperationService
    {
    }


    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="ConsoleApp2.IOperation" />
    // [DependencyAttribute(typeof(IOperation))]
    public class Operation : IOperation, IOperationTransient, IOperationScoped, IOperationSingleton, IOperationSingletonInstance
    {
        private Guid operationId;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Operation"/> class.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        public Operation(Guid guid = new Guid())
        {
            operationId = guid;
        }

        /// <summary>
        /// Gets the operation identifier.
        /// </summary>
        /// <value>
        /// The operation identifier.
        /// </value>
        public Guid OperationId
        {
            get
            {
                return operationId;
            }
        }

    }


    /// <summary>
    /// 
    /// </summary>
    public class OperationService: IOperationService
    {
        public IOperationTransient TransientOperation { get; }
        public IOperationScoped ScopedOperation { get; }
        public IOperationSingleton SingletonOperation { get; }
        public IOperationSingletonInstance SingletonInstanceOperation { get; }

        public OperationService(IOperationTransient transientOperation,
            IOperationScoped scopedOperation,
            IOperationSingleton singletonOperation,
            IOperationSingletonInstance instanceOperation)
        {
            TransientOperation = transientOperation;
            ScopedOperation = scopedOperation;
            SingletonOperation = singletonOperation;
            SingletonInstanceOperation = instanceOperation;
        }
    }

    public class Program
    {

        public static IServiceProvider Services { get; set; }

        /// <summary>
        /// Mains the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void Main(string[] args)
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            Services = serviceCollection.BuildServiceProvider();
            var operationSingleInstance = Services.GetRequiredService<IOperationService>();
            //CallRest();
            Console.ReadLine();
        }

        /// <summary>
        /// Configures the services.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        static private void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IOperation, Operation>();
            serviceCollection.AddTransient<IOperationTransient, Operation>();
            serviceCollection.AddScoped<IOperationScoped, Operation>();
            serviceCollection.AddSingleton<IOperationSingleton, Operation>();
            serviceCollection.AddSingleton<IOperationSingletonInstance>(new Operation(Guid.Empty));
            serviceCollection.AddTransient<IOperationService, OperationService>();
        }

        /// <summary>
        /// Calls the rest.
        /// </summary>
        public static void CallRest()
        {
            var restClient = new RestClient("http://api.openweathermap.org");
            var request = new RestRequest(Method.GET);
            request.Resource = "data/2.5/weather?q=London,uk&appid={appId}";
            request.AddParameter("appId", "7389c67d6f64fc8d54622c3697420889", ParameterType.UrlSegment);
            var response = restClient.ExecuteAsync(request, r =>
            {
                if (r.ResponseStatus == ResponseStatus.Completed)
                {

                    string date = string.Format("text-{0:yyyy-MM-dd_hh-mm-ss-tt}.txt", DateTime.Now);
                    File.WriteAllText(String.Concat(Directory.GetCurrentDirectory(), "WeatherReport", date), r.Content);
                }
            });
        }
    }
}

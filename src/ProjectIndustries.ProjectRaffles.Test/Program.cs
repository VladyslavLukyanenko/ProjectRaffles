using System;
using Autofac;
using ProjectIndustries.KingHttpClient;

namespace ProjectIndustries.ProjectRaffles.Test
{
    using System.Net.Http;
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            ContainerBuilder container = new ContainerBuilder();
            container.Register(ctx =>
            {
                var spec = ClientHelloSpecPresets.CreateIos121ClientHelloSpec();
                HttpClient client = new HttpClient(UtlsHttpMessageHandler.Create(spec));
                return client;
            }).InstancePerDependency();
            container.RegisterType<ServiceA>().AsSelf();
            container.RegisterType<ServiceB>().AsSelf();
            var c = container.Build();
            var serviceA = c.Resolve<ServiceA>();
            var serviceB = c.Resolve<ServiceB>();
            
        }
    }
}
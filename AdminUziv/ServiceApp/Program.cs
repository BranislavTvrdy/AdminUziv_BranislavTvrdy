using System;
using System.ServiceModel;

namespace ServiceApp
{
    class Program
    {
        /// <summary>
        /// Vstupný bot programu pre WcfHostService
        /// </summary>
        static void Main() //string[] args
        {
            using (var host = new ServiceHost(typeof(WcfKangoService)))
            {
                System.ServiceModel.Description.ServiceThrottlingBehavior throttlingBehavior =
                    new System.ServiceModel.Description.ServiceThrottlingBehavior
                    {
                        MaxConcurrentCalls = Int32.MaxValue,
                        MaxConcurrentInstances = Int32.MaxValue,
                        MaxConcurrentSessions = Int32.MaxValue
                    };
                host.Description.Behaviors.Add(throttlingBehavior);
                host.Open();
                Console.WriteLine("Server odstartovany!");
                Console.ReadLine();
                host.Close();
            }
        }
    }
}

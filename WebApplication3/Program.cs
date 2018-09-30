using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibrary;
using LiteDB;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace WebApplication3
{
    public class Program
    {
        public static LiteDatabase db;
        public static string DbPath = @"serverdb.litedb";
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseKestrel()
                .UseStartup<Startup>()
                .Build();

            db = new LiteDatabase(Program.DbPath);
            var users = db.GetCollection<User>("users");
            var problems = db.GetCollection<Problem>("problems");
            var states = db.GetCollection<State>("states");
            var groups = db.GetCollection<Group>("groups");

            var factory = new ConnectionFactory() { HostName = "localhost" };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            
                channel.QueueDeclare("NewState", false, false, false,null);

                var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                State state = JsonConvert.DeserializeObject<State>(message);
                states.Update(state);
            };  
                channel.BasicConsume("NewState", true, consumer);

             
            
            host.Run();
            
        }

        
    }
}

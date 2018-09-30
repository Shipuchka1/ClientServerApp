using ClassLibrary;
using Nancy;
using Nancy.ModelBinding;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace WebApplication3.Modules
{
    public class Test:NancyModule
    {
        public Test()
        {
            //Get["/"] = parameters => "My category is ";

            Get("/users", args => 
            {
                string res = GetProcess<User>("users", new User(), this.Request.Query);
                return res;
            });

            Get("/problems", args =>
            {
                string res = GetProcess<Problem>("problems", new Problem(), this.Request.Query);
                return res;
            });

            Get("/states", args =>
            {
                string res = GetProcess<State>("states", new State(), this.Request.Query);
                return res;
            });

            Get("/groups", args =>
            {
                string res = GetProcess<Group>("groups", new Group(), this.Request.Query);
                return res;
            });


            Post("/getfilestudent", args =>
            {
                ConnectionFactory factory = new ConnectionFactory() { HostName = "localhost" }; ;
                IConnection connection = factory.CreateConnection();
                IModel channel = connection.CreateModel();
                channel.QueueDeclare("QStudentFiles", false, false, false, null);

                var rcvbody = this.Request.Body;
                int length = (int)rcvbody.Length;
                byte[] data = new byte[length];
                rcvbody.Read(data, 0, length);

                string message = System.Text.Encoding.Default.GetString(data);
                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish("", "QStudentFiles", null, body);


                
                //using (StreamWriter sw = new StreamWriter("Test.txt", false, System.Text.Encoding.Default))
                //{
                //    sw.WriteLine(System.Text.Encoding.Default.GetString(data));
                //}

                return message;
            });

            Post("/users", args =>
            {
                string res = PostProcess<User>(this.Request.Body, "users",new User());
                return res;
            });

            Post( "/problems", args =>
            {
                string res = PostProcess<Problem>(this.Request.Body, "problems", new Problem());
                return res;
            });


            Post("/states", args =>
            {
                string res = PostProcess<State>(this.Request.Body, "states", new State());
                return res;
            });

            Post("/groups", args =>
            {
                string res = PostProcess<Group>(this.Request.Body, "groups", new Group());
                return res;
            });



        }

        public string PostProcess<T>( Stream DataStream, string Table, T ObjType )
        {
            var TableInstance = Program.db.GetCollection<T>(Table);

            var rcvbody = DataStream;
            int length = (int)rcvbody.Length;
            byte[] data = new byte[length];
            rcvbody.Read(data, 0, length);

            T[] tmp;

            try
            {
                string message = System.Text.Encoding.Default.GetString(data);

                tmp = JsonConvert.DeserializeObject<T[]>(message);
                foreach (T item in tmp)
                {
                    TableInstance.Insert(item);
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return JsonConvert.SerializeObject(tmp);
        }

        public string GetProcess<T>(string Table, T ObjType, dynamic query)
        {
            var TableInstance = Program.db.GetCollection<T>(Table);

            List<T> tmp;

            try
            {
                LiteDB.Query q = LiteDB.Query.All();
                Type tt = typeof(T);
                PropertyInfo[] props = tt.GetProperties();

                foreach (string item in query)
                {
                  Type TypeOfProperty = props.FirstOrDefault(w => w.Name == item).PropertyType;

                    string newItem = item;
                    if (item == "Id") newItem = "_id";
                   q = LiteDB.Query.And( q, LiteDB.Query.EQ(newItem, Convert.ChangeType(query[item].Value,TypeOfProperty))   );
                }

                



                tmp = TableInstance.Find(q).ToList();


            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return JsonConvert.SerializeObject(tmp);
        }


    }
}

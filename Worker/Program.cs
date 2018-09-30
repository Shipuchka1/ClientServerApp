using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ClassLibrary;
using System.IO;
using System.Reflection;
using System.Net.Mail;
using System.Net;
using System.Collections.Specialized;
using Quartz;
using Quartz.Impl;

namespace Worker
{
    class Program
    {
        public static Dictionary<int,List<StudentFile>> tempStudentFiles = new Dictionary<int, List<StudentFile>>();
        
        static void Main(string[] args)
        {
            RESTApi.ServerUrl = "http://localhost:8082";
            string path = "/users?IsTeacher=true";
            List<User> uss = JsonConvert.DeserializeObject<List<User>>(RESTApi.Get(path));
            foreach (User item in uss)
            {
                List<StudentFile> files = new List<StudentFile>();
                tempStudentFiles.Add(item.Id, files);
            }

            
            Thread worker = new Thread(delegate ()
            {
                
                var factory = new ConnectionFactory() { HostName = "localhost" };
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare("QStudentFiles", false, false, false, null);


                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {

                        try
                        {
                            var body = ea.Body;
                            var message = Encoding.UTF8.GetString(body);
                            Console.WriteLine(" [x] Received {0}", message);
                            StudentFile tmp = JsonConvert.DeserializeObject<StudentFile>(message);

                            Console.WriteLine(tmp.FileName);
                            string RandomNameFile = "received/" + System.Guid.NewGuid().ToString() + Path.GetExtension(tmp.FileName);


                            File.WriteAllBytes(RandomNameFile, tmp.FileInBytes);
                            Assembly a = Assembly.LoadFile(Directory.GetCurrentDirectory() + "/" + RandomNameFile);
                            List<Arguments> inputArgs;
                            inputArgs = JsonConvert.DeserializeObject<List<Arguments>>(tmp.problem.InputArgs);
                            Console.WriteLine("mknk" + inputArgs.Count);
                            bool flag = true;
                            foreach (Arguments item in inputArgs)
                            {
                                Type t = a.GetType(item.className);
                                MethodInfo m = t.GetMethod(item.methodName);
                                dynamic c = Activator.CreateInstance(t);
                                object[] argArr = new object[item.args.Count];
                                Console.WriteLine("rttrr" + item.args.Count);
                                for (int i = 0; i < item.args.Count; i++)
                                {
                                    argArr[i] = Convert.ChangeType(item.args[i].value, Type.GetType(item.args[i].type));
                                }




                                object res = m.Invoke(c, argArr);
                                object res2 = Convert.ChangeType(item.result.value, Type.GetType(item.result.type));

                                if (res.GetType() == res2.GetType() && res.GetHashCode() == res2.GetHashCode())
                                {
                                    Console.WriteLine("ok");
                                }
                                else flag = false;
                                if (flag)
                                {

                                    tmp.state.Status = "Done";
                                    channel.QueueDeclare("NewState", false, false, false, null);

                                    string mess = JsonConvert.SerializeObject(tmp.state as State);
                                    var bodyState = Encoding.UTF8.GetBytes(mess);

                                    channel.BasicPublish("", "NewState", null, bodyState);
                                    Console.WriteLine(" [x] Sent {0}", mess);
                                }

                            }
                        

                        

                        MailMessage msg = new MailMessage();

                        msg.From = new MailAddress("robotworker6@gmail.com");
                        //msg.To.Add(tmp.user.Email);
                        msg.To.Add("natalyaplestsova@gmail.com");
                        msg.Subject = "Status your work " + DateTime.Now.ToString();
                        msg.Body = tmp.user.FullName + ", "+tmp.problem.Subject+" "+tmp.state.Status;
                        SmtpClient client = new SmtpClient();
                        client.UseDefaultCredentials = true;
                        client.Host = "smtp.gmail.com";
                        client.Port = 587;
                        client.EnableSsl = true;
                        client.DeliveryMethod = SmtpDeliveryMethod.Network;
                        client.Credentials = new NetworkCredential("robotworker6@gmail.com", "Qwerty`123");
                        client.Timeout = 20000;
                        try
                        {
                            client.Send(msg);
                            Console.WriteLine( "Mail has been successfully sent!");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine( "Fail Has error" + ex.Message);
                        }
                        finally
                        {
                            msg.Dispose();
                        }

                        tempStudentFiles[tmp.problem.TeacherId].Add(tmp);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    };
                    channel.BasicConsume("QStudentFiles", true, consumer);

                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();

                }

    }
                   );

            worker.Start();

            ISchedulerFactory schedFact = new StdSchedulerFactory();

            // get a scheduler
            IScheduler sched = schedFact.GetScheduler().Result;
            
            sched.Start();

          
            IJobDetail job = JobBuilder.Create<HelloJob>()
                .WithIdentity("myJob", "group1")
                .Build();

         
            ITrigger trigger = TriggerBuilder.Create()
              .WithIdentity("myTrigger", "group1")
              .StartNow()
              .WithSimpleSchedule(x => x
                  .WithIntervalInMinutes(1)
                  .RepeatForever())
              .Build();

            sched.ScheduleJob(job, trigger);

            
        }

        
    }
   public class HelloJob:IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            RESTApi.ServerUrl = "http://localhost:8082/";
            
            foreach (var item in Program.tempStudentFiles)
            {
                if (item.Value.Count > 0)
                {
                    string path = "users?Id=" + item.Key;
                    List<User> users = JsonConvert.DeserializeObject<List<User>>(RESTApi.Get(path));

                    MailMessage msg = new MailMessage();

                    msg.From = new MailAddress("robotworker6@gmail.com");


                    //msg.To.Add(users[0].Email);
                    msg.To.Add("natalyaplestsova@gmail.com");
                    msg.Subject = "Stats -  " + DateTime.Now.ToString();
                    foreach (StudentFile sf in item.Value)
                    {
                        msg.Body += sf.user.FullName + ", " + sf.problem.Subject + " " + sf.state.Status + "\n";
                    }

                    SmtpClient client = new SmtpClient();
                    client.UseDefaultCredentials = true;
                    client.Host = "smtp.gmail.com";
                    client.Port = 587;
                    client.EnableSsl = true;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.Credentials = new NetworkCredential("robotworker6@gmail.com", "Qwerty`123");
                    client.Timeout = 20000;
                    try
                    {
                        client.Send(msg);
                        Console.WriteLine("Mail with stats has been successfully sent!");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Fail Has error" + ex.Message);
                    }
                    finally
                    {
                        msg.Dispose();
                    }

                    item.Value.Clear();
                }
            }
        }
    }
}

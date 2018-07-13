using System;
using System.Threading;
using System.Threading.Tasks;

namespace testnetcore
{
    class Program
    {
        static WebSocketMoke _WS = new WebSocketMoke();
        static ManualResetEvent _stop = new ManualResetEvent(false);
        static AutoResetEvent _HandleChanges = new AutoResetEvent(false);
        static void Main(string[] args)
        {
            Console.WriteLine("started");
            var t2 = Task.Run(() => ProgramHandleEventThread());
            _WS.OnMessage+=OnMessage;
            _WS.Start();

            Console.ReadLine();
            _stop.Set();
             Console.WriteLine("stop was send");
             _WS.Stop();
            t2.Wait();
            
            Console.WriteLine("finish prog");
        }

        private static void OnMessage(int number)
        {
            Console.WriteLine(string.Format("start HandleStateChange->{0}",number));
            _HandleChanges.Set();
            Console.WriteLine(string.Format("finish HandleStateChange->{0}",number));  
        }

        
        private static void ProgramHandleEventThread()
        {
            while(true)
            {
                int res = WaitHandle.WaitAny(new WaitHandle[]{_HandleChanges,_stop});
                
                if(res==1)
                {
                    Console.WriteLine(string.Format("stop logic signal received")); 
                    break;
                }
                Console.WriteLine(string.Format("start logic")); 
                Thread.Sleep(5000);
                Console.WriteLine(string.Format("finish logic"));          
            }
            Console.WriteLine(string.Format("exit thread logic")); 
        }
    }
}

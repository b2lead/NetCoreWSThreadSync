using System;
using System.Threading;
using System.Threading.Tasks;

namespace testnetcore
{
    class Program
    {
        static ManualResetEvent _stop = new ManualResetEvent(false);
        static void Main(string[] args)
        {
            Console.WriteLine("started");

            var t1 = Task.Run(() => WS_InternalThread());
            var t2 = Task.Run(() => ProgramHandleEventThread());
            Console.ReadLine();
            _stop.Set();
             Console.WriteLine("stop was send");
            Task.WaitAll(t1,t2);
            
            Console.WriteLine("finish prog");
        }

       

        static int _eventCounetr=0;
        static object _locker=new object();

        private static void WS_InternalThread()
        {
            while(!_stop.WaitOne(1000))
            {
                _eventCounetr++;
                Console.WriteLine(string.Format("WS_StateChanges->{0}",_eventCounetr));
                
                Task.Run(()=>HandleStateChange(_eventCounetr));
                
            }
        }

        private static void HandleStateChange(int eventNumber)
        {
            lock(_locker)
            {
                ProgramStateChange(eventNumber);
            }
        }
        private static void ProgramStateChange(int eventNumber)
        {
            Console.WriteLine(string.Format("start HandleStateChange->{0}",eventNumber));
            _HandleChanges.Set();
            Console.WriteLine(string.Format("finish HandleStateChange->{0}",eventNumber));        
        }
        static AutoResetEvent _HandleChanges = new AutoResetEvent(false);
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
                Thread.Sleep(5000);
                Console.WriteLine(string.Format("finish logic"));          
            }
            Console.WriteLine(string.Format("exit thread logic")); 
        }
    }
}

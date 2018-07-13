using System;
using System.Threading;
using System.Threading.Tasks;

class WebSocketMoke
{
    private int _eventCounetr=0;
    private object _locker=new object();
    private Task _mainTask;
    private ManualResetEvent _stop = new ManualResetEvent(false);
    internal delegate void NewMessage(int number);
    public event NewMessage OnMessage;

    public void Start()
    {
        _mainTask = Task.Run(() => WS_InternalThread());
    }
    public void Stop()
    {
       _stop.Set();
       _mainTask.Wait();
    }

    private  void WS_InternalThread()
    {
        while(!_stop.WaitOne(1000))
        {
            _eventCounetr++;
            Console.WriteLine(string.Format("WebSocketMoke Event #{0}",_eventCounetr));
            
            Task.Run(()=>RaiseNewIncomeEvent(_eventCounetr));
            
        }
    }

    private void RaiseNewIncomeEvent(int eventNumber)
    {
        if(OnMessage==null)
            return;
            
        lock(_locker)
        {
            OnMessage(eventNumber);
        }
    }
}
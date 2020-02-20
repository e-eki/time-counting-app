using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpTime
{
    public class Counter
    {
        public TimeSpan SumTime { private set; get; }
        public TimeSpan SessionTime { private set; get; }
        public bool Working { get; private set; }

        private DateTime startTime;

        public Counter(TimeSpan sumTime)
        {
            SumTime = sumTime;
            SessionTime = TimeSpan.Zero;
            Working = false;
        }

        public void Tick()
        {
            SumTime += (DateTime.Now - startTime);
            SessionTime += DateTime.Now - startTime;
            startTime = DateTime.Now;
        }

        public void Start()
        {
            Working = true;
            startTime = DateTime.Now;
        }

        public void Stop()
        {
            Working = false;
        }

        public void Redact(TimeSpan newSumTime)
        {
            SumTime = newSumTime;
            SessionTime = TimeSpan.Zero;
        }
    }
}
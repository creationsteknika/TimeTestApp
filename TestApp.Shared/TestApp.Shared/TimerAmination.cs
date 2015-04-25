using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace TestApp.Shared
{
    class TimerAnimationState
    {
        public bool stop = false;
        public int second = 0;
        public Timer tmr;
    }

    class TimerAmination
    {
        public event EventHandler<int> UpdateNumber;
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public void Start(DateTime startDateTime, long millisecondesServerDifference)
        {
            TimerAnimationState s = new TimerAnimationState();

            // Create the delegate that invokes methods for the timer.
            TimerCallback timerDelegate = new TimerCallback(CheckStatus);


            // Create a timer that waits one second, then invokes every second.
            long delay = CalculateDelay(startDateTime, millisecondesServerDifference);
            Timer timer = new Timer(timerDelegate, s, delay, 1000);

        }
        // The following method is called by the timer's delegate.

        long CalculateDelay(DateTime startDateTime, long millisecondesServerDifference)
        {

			var test = Convert.ToInt64(startDateTime.Subtract (DateTime.Now).TotalMilliseconds);

            long start = Convert.ToInt64((startDateTime - UnixEpoch).TotalMilliseconds);
            long now = Convert.ToInt64((DateTime.UtcNow - UnixEpoch).TotalMilliseconds);

			long delay = test - millisecondesServerDifference;

            return delay;

        }

        void CheckStatus(Object state)
        {
            TimerAnimationState s = (TimerAnimationState)state;
            UpdateNumber(this, s.second);

            s.second++;

            if (s.stop)
            {
                s.tmr.Dispose();
                s.tmr = null;
            }
        }
    }
}

namespace HashPhotoSlideshow.Controller
{
    using System;
    using System.Collections.Generic;

    public class ConsoleProgressBar
    {
        private const int CircularBufferSize = 10;

        public int Min { get; }
        public int Max { get; }

        private List<ProgressInfo> ProgressInfos = new List<ProgressInfo>();

        private DateTime LastUpdated = DateTime.Now;

        private bool IsFirstUpdate = false;

        private int LastTimeLeft = 0;

        private int LastMessageLength = 0;

        public ConsoleProgressBar(int min, int max)
        {
            Min = min;
            Max = max;
            IsFirstUpdate = true;
        }

        public void Progress(int progress)
        {
            if (ProgressInfos.Count > CircularBufferSize)
            {
                ProgressInfos.RemoveAt(0);
            }

            ProgressInfos.Add(new ProgressInfo()
            {
                Progress = progress,
                Timestamp = DateTime.Now
            });

            double averagePerItem = 0;

            for (int i = 0; i < ProgressInfos.Count - 1; i++)
            {
                averagePerItem += (ProgressInfos[i + 1].Timestamp.Subtract(ProgressInfos[i].Timestamp).TotalMilliseconds) /
                (ProgressInfos[i + 1].Progress - ProgressInfos[i].Progress);
            }

            averagePerItem /= (ProgressInfos.Count - 1);

            int itemsLeft = (Max - progress);
            double timeLeft = itemsLeft * averagePerItem;
            int timeLeftSec = (int)(timeLeft / 1000);

            if (IsFirstUpdate)
            {
                IsFirstUpdate = false;
                LastTimeLeft = timeLeftSec;
                LastUpdated = DateTime.Now;
            }
            else
            {
                int secondsPassed = (int)DateTime.Now.Subtract(LastUpdated).TotalSeconds;
                if (secondsPassed >= 1)
                {
                    if (timeLeftSec * 2 < LastTimeLeft)
                    {
                        LastTimeLeft = timeLeftSec;
                    }
                    else
                    {
                        LastTimeLeft -= secondsPassed;
                    }
                    LastUpdated = DateTime.Now;
                }
            }

            int percentage = (progress - Min) == 0 ? 0 : (((progress - Min) * 100) / (Max - Min));
            percentage = percentage > 100 ? 100 : percentage;
            int interval = 5;
            int maxInterval = 100 / interval;
            int intervalAchieved = percentage / interval;

            string message = $"    [";
            message += new string('=', intervalAchieved);
            message += new string(' ', maxInterval - intervalAchieved);
            message += $"]    {percentage}% [{progress}/{Max}]    Time left: {LastTimeLeft} seconds.";

            int currentLineCursor = Console.CursorTop;

            if (message.Length < LastMessageLength)
            {
                Console.SetCursorPosition(0, currentLineCursor);
                Console.Write(new string(' ', LastMessageLength));
            }

            Console.SetCursorPosition(0, currentLineCursor);
            Console.Write(message);

            LastMessageLength = message.Length;

            if (progress == Max)
            {
                Console.SetCursorPosition(0, currentLineCursor);
                Console.Write(new string(' ', LastMessageLength));
            }
        }

        internal class ProgressInfo
        {
            public int Progress { get; set; }
            public DateTime Timestamp { get; set; }
        }
    }
}
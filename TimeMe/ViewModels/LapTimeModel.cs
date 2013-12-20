using System;
using System.IO;

namespace TimeMe.ViewModels
{
    public class LapTimeModel
    {
        public int LapNo { get; set; }
        public TimeSpan LapTime { get; set; }
        public TimeSpan TimeStamp { get; set; }
        public string LapTimeAsString
        {
            get
            {
                string lapNoFormatted;
                if (LapNo < 10)
                    lapNoFormatted = LapNo.ToString().PadLeft(3);
                else
                    lapNoFormatted = LapNo.ToString();

                return string.Format("{0}: {1:00}:{2:00}.{3:0} (total {4:00}:{5:00}.{6:0})",
                                     lapNoFormatted,
                                     (int)LapTime.TotalMinutes, LapTime.Seconds, LapTime.Milliseconds/100,
                                     (int)TimeStamp.TotalMinutes, TimeStamp.Seconds, TimeStamp.Milliseconds / 100);
            }
        }

        public void Save(BinaryWriter bw)
        {
            bw.Write(LapNo);
            bw.Write(LapTime.Hours);
            bw.Write(LapTime.Minutes);
            bw.Write(LapTime.Seconds);
            bw.Write(LapTime.Milliseconds);
            bw.Write(TimeStamp.Hours);
            bw.Write(TimeStamp.Minutes);
            bw.Write(TimeStamp.Seconds);
            bw.Write(TimeStamp.Milliseconds);
        }

        public static LapTimeModel Load(BinaryReader br)
        {
            LapTimeModel lap = new LapTimeModel();
            lap.LapNo = br.ReadInt32();
            int hour = br.ReadInt32();
            int minutes = br.ReadInt32();
            int seconds = br.ReadInt32();
            int miliseconds = br.ReadInt32();
            lap.LapTime = new TimeSpan(0, hour, minutes, seconds, miliseconds);
            hour = br.ReadInt32();
            minutes = br.ReadInt32();
            seconds = br.ReadInt32();
            miliseconds = br.ReadInt32();
            lap.TimeStamp = new TimeSpan(0, hour, minutes, seconds, miliseconds);

            return lap;
        }

    }
}

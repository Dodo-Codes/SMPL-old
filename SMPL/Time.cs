using SFML.System;
using System.Threading;
using System.Windows.Forms;

namespace SMPL
{
	public static class Time
	{
		public struct UnitDisplay
		{
			public bool areIncluded;
			public string display;
		}
		public struct Format
		{
			public string Separator;
			public UnitDisplay Hours;
			public UnitDisplay Minutes;
			public UnitDisplay Seconds;
			public UnitDisplay Milliseconds;

			public Format(UnitDisplay hours, UnitDisplay minutes, UnitDisplay seconds, UnitDisplay milliseconds,
				string separator = ":")
			{
				Hours = hours;
				Minutes = minutes;
				Seconds = seconds;
				Milliseconds = milliseconds;
				Separator = separator;
			}
		}
		public enum Convertion
		{
			MillisecondsToSeconds,
			SecondsToMilliseconds, SecondsToMinutes, SecondsToHours,
			MinutesToMilliseconds, MinutesToSeconds, MinutesToHours, MinutesToDays,
			HoursToSeconds, HoursToMinutes, HoursToDays, HoursToWeeks,
			DaysToMinutes, DaysToHours, DaysToWeeks,
			WeeksToHours, WeeksToDays
		}
		public enum Unit
		{
			Tick, Second
		}

		internal static Clock time, frameDeltaTime;
		internal static uint frameCount;
		public static double DeltaTime { get { return frameDeltaTime.ElapsedTime.AsSeconds(); } }
		private static uint frameRateLimit;
		public static uint FrameRateLimit
		{
			get { return frameRateLimit; }
			set
			{
				var n = (uint)Number.Limit(value, new Bounds(1, 60));
				frameRateLimit = n;
				Window.window.SetFramerateLimit(n);
			}
		}
		public static double Clock { get { return time.ElapsedTime.AsSeconds(); } }

		public static string ToText(double timeInSeconds, Format format)
		{
			timeInSeconds = Number.Sign(timeInSeconds, false);
			var secondsStr = timeInSeconds.ToString();
			var ms = 0;
			if (secondsStr.Contains('.'))
			{
				var spl = secondsStr.Split('.');
				ms = int.Parse(spl[1]) * 100;
				timeInSeconds = Number.Round(timeInSeconds, toward: Number.RoundToward.Down);
			}
			var sec = timeInSeconds % 60;
			var min = Number.Round(timeInSeconds / 60 % 60, toward: Number.RoundToward.Down);
			var hr = Number.Round(timeInSeconds / 3600, toward: Number.RoundToward.Down);
			var msShow = format.Milliseconds.areIncluded;
			var secShow = format.Seconds.areIncluded;
			var minShow = format.Minutes.areIncluded;
			var hrShow = format.Hours.areIncluded;

			var msStr = msShow ? $"{ms}" : "";
			var secStr = secShow ? $"{sec}" : "";
			var minStr = minShow ? $"{min}" : "";
			var hrStr = hrShow ? $"{hr}" : "";
			var msF = msShow ? $"{format.Milliseconds.display}" : "";
			var secF = secShow ? $"{format.Seconds.display}" : "";
			var minF = minShow ? $"{format.Minutes.display}" : "";
			var hrF = hrShow ? $"{format.Hours.display}" : "";
			var secMsSep = msShow && (secShow || minShow || hrShow) ? $"{format.Separator}" : "";
			var minSecSep = secShow && (minShow || hrShow) ? $"{format.Separator}" : "";
			var hrMinSep = minShow && hrShow ? $"{format.Separator}" : "";

			return $"{hrStr}{hrF}{hrMinSep}{minStr}{minF}{minSecSep}{secStr}{secF}{secMsSep}{msStr}{msF}";
		}
		public static double FromNumber(double number, Convertion convertType)
		{
			return convertType switch
			{
				Convertion.MillisecondsToSeconds => number / 1000,
				Convertion.SecondsToMilliseconds => number * 1000,
				Convertion.SecondsToMinutes => number / 60,
				Convertion.SecondsToHours => number / 3600,
				Convertion.MinutesToMilliseconds => number * 60000,
				Convertion.MinutesToSeconds => number * 60,
				Convertion.MinutesToHours => number / 60,
				Convertion.MinutesToDays => number / 1440,
				Convertion.HoursToSeconds => number * 3600,
				Convertion.HoursToMinutes => number * 60,
				Convertion.HoursToDays => number / 24,
				Convertion.HoursToWeeks => number / 168,
				Convertion.DaysToMinutes => number * 1440,
				Convertion.DaysToHours => number * 24,
				Convertion.DaysToWeeks => number / 7,
				Convertion.WeeksToHours => number * 168,
				Convertion.WeeksToDays => number * 7,
				_ => 0,
			};
		}
		public static double GetFrameRate(bool averaged = false)
		{
			return averaged ? frameCount / time.ElapsedTime.AsSeconds() : 1 / frameDeltaTime.ElapsedTime.AsSeconds();
		}
		internal static void Run()
		{
			while (Window.window.IsOpen)
			{
				Thread.Sleep(1);
				Application.DoEvents();
				Window.window.DispatchEvents();

				frameCount++;
				if (TimeEvents.instance != null) TimeEvents.instance.OnEachTick();

				Window.Draw();
				frameDeltaTime.Restart();

				File.UpdateMainThreadAssets();
			}
		}

		internal static void Initialize()
		{
			time = new();
			frameDeltaTime = new();
		}
	}
}

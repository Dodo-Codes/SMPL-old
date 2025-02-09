﻿using SFML.System;
using SMPL.Components;
using SMPL.Data;
using SMPL.Prefabs;
using System;
using System.Threading;
using System.Windows.Forms;

namespace SMPL.Gear
{
	public static class Time
	{
		public struct UnitDisplay
		{
			public bool AreSkipped { get; set; }
			public string Display { get; set; }

			public UnitDisplay(bool areSkipped = false, string display = "")
			{
				Display = display;
				AreSkipped = areSkipped;
			}
		}
		public struct Format
		{
			public string Separator { get; set; }
			public UnitDisplay Hours { get; set; }
			public UnitDisplay Minutes { get; set; }
			public UnitDisplay Seconds { get; set; }
			public UnitDisplay Milliseconds { get; set; }

			public Format(UnitDisplay hours = new(), UnitDisplay minutes = new(), UnitDisplay seconds = new(),
				UnitDisplay milliseconds = new(), string separator = ":")
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
			MillisecondsToSeconds, MillisecondsToMinutes,
			SecondsToMilliseconds, SecondsToMinutes, SecondsToHours,
			MinutesToMilliseconds, MinutesToSeconds, MinutesToHours, MinutesToDays,
			HoursToSeconds, HoursToMinutes, HoursToDays, HoursToWeeks,
			DaysToMinutes, DaysToHours, DaysToWeeks,
			WeeksToHours, WeeksToDays
		}
		public enum Unit
		{
			Frame, Second
		}

		internal static Clock time;
		public static double GameClock { get { return time.ElapsedTime.AsSeconds(); } }
		public static double Clock { get { return DateTime.Now.TimeOfDay.TotalSeconds; } }
		public static string Zone { get { return Hardware.GetData(Hardware.DataType.TimeZone, "Caption") as string; } }

		public static string ToText(double timeInSeconds, Format format = new())
		{
			timeInSeconds = Number.Sign(timeInSeconds, false);
			var secondsStr = $"{timeInSeconds:F0}";
			var ms = 0;
			if (secondsStr.Contains('.'))
			{
				var spl = secondsStr.Split('.');
				ms = int.Parse(spl[1]) * 100;
				timeInSeconds = Number.Round(timeInSeconds, toward: Number.RoundToward.Down);
			}
			var sec = timeInSeconds % 60;
			var min = FromNumber(timeInSeconds, Convertion.SecondsToMinutes) % 60;
			var hr = FromNumber(timeInSeconds, Convertion.SecondsToHours);
			var msShow = !format.Milliseconds.AreSkipped;
			var secShow = !format.Seconds.AreSkipped;
			var minShow = !format.Minutes.AreSkipped;
			var hrShow = !format.Hours.AreSkipped;

			var sep = format.Separator == null || format.Separator == "" ? ":" : format.Separator;
			var msStr = msShow ? $"{ms:D2}" : "";
			var secStr = secShow ? $"{(int)sec:D2}" : "";
			var minStr = minShow ? $"{(int)min:D2}" : "";
			var hrStr = hrShow ? $"{(int)hr:D2}" : "";
			var msF = msShow ? $"{format.Milliseconds.Display}" : "";
			var secF = secShow ? $"{format.Seconds.Display}" : "";
			var minF = minShow ? $"{format.Minutes.Display}" : "";
			var hrF = hrShow ? $"{format.Hours.Display}" : "";
			var secMsSep = msShow && (secShow || minShow || hrShow) ? $"{sep}" : "";
			var minSecSep = secShow && (minShow || hrShow) ? $"{sep}" : "";
			var hrMinSep = minShow && hrShow ? $"{sep}" : "";

			return $"{hrStr}{hrF}{hrMinSep}{minStr}{minF}{minSecSep}{secStr}{secF}{secMsSep}{msStr}{msF}";
		}
		public static double FromNumber(double number, Convertion convertType)
		{
			return convertType switch
			{
				Convertion.MillisecondsToSeconds => number / 1000,
				Convertion.MillisecondsToMinutes => number / 1000 / 60,
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

		internal static void Initialize() => time = new();
	}
}

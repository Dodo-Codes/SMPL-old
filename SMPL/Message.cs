using System.Windows.Forms;

namespace SMPL
{
	public static class Message
	{
		public enum Icon
		{
			None, Info, Error, Warning
		}
		public enum Buttons
		{
			OK = 0,
			OKCancel = 1,
			AbortRetryIgnore = 2,
			YesNoCancel = 3,
			YesNo = 4,
			RetryCancel = 5
		}
		public enum Result
		{
			None = 0,
			OK = 1,
			Cancel = 2,
			Abort = 3,
			Retry = 4,
			Ignore = 5,
			Yes = 6,
			No = 7
		}

		public static Result Show(object message, string title = "", Icon icon = Icon.None, Buttons buttons = Buttons.OK)
		{
			var msgIcon = MessageBoxIcon.None;
			var btn = (MessageBoxButtons)buttons;
			switch (icon)
			{
				case Icon.Info: msgIcon = MessageBoxIcon.Information; break;
				case Icon.Error: msgIcon = MessageBoxIcon.Error; break;
				case Icon.Warning: msgIcon = MessageBoxIcon.Warning; break;
				case Icon.None: msgIcon = MessageBoxIcon.None; break;
			}
			return (Result)MessageBox.Show($"{message}", title, btn, msgIcon);
		}
	}
}

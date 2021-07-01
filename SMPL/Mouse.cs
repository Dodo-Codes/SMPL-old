namespace SMPL
{
	public static class Mouse
	{
		public enum Button
		{
			Unknown = -1, Left = 0, Right = 1, Middle = 2, ExtraButton1 = 3, ExtraButton2 = 4
		}
		public enum Wheel
		{
			Vertical, Horizontal
		}

		public static Point CursorPositionWindow
		{
			get
			{
				var mousePos = SFML.Window.Mouse.GetPosition(Window.window);
				var pos = Window.window.MapPixelToCoords(mousePos);
				return new Point((int)pos.X, (int)pos.Y);
			}
		}
		public static Point CursorPositionScreen
		{
			get { var pos = System.Windows.Forms.Cursor.Position; return new Point(pos.X, pos.Y); }
			set { System.Windows.Forms.Cursor.Position = new System.Drawing.Point((int)value.X, (int)value.Y); }
		}
	}
}

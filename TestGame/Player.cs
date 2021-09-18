using SMPL;
using SMPL.Data;
using SMPL.Gear;

namespace TestGame
{
	public class Player : Events
	{
		private double camDist = 0;
		private double camLen = 200;
		private int rayAmount = 300;
		private double rayLength = 500;
		private double fov = 90;
		private Component2D Component2D { get; set; }
		private ComponentHitbox Map { get; set; }
		private ComponentCamera Cam { get; set; }

		public Player()
		{
			Subscribe(this, 0);

			Component2D = new() { Size = new Size(200, 200) };
			Map = new();
			Cam = new(new Point(), new Size(rayLength + 200, rayLength + 200));
			Cam.Component2D.Position = new Point(650, -250);
			Cam.Component2D.Size = new Size(500, 500);
			Cam.BackgroundColor = new Color(0, 0, 0, 100);

			for (int i = 0; i < 20; i++)
			{
				var c = 3 * camLen;
				var p = new Point(Probability.Randomize(new Number.Range(-c, c)), Probability.Randomize(new Number.Range(-c, c)));
				var p2 = Point.MoveAtAngle(p, Probability.Randomize(new Number.Range(0, c)),
					Probability.Randomize(new Number.Range(10, 500)), Time.Unit.Tick);
				var col = new Color(
					Probability.Randomize(new Number.Range(0, 255)),
					Probability.Randomize(new Number.Range(0, 255)),
					Probability.Randomize(new Number.Range(0, 255)));
				p.Color = col;
				p2.Color = col;
				Map.SetLine($"wall-{i}", new Line(p, p2));
			}
			Component2D.ComponentHitbox.SetLine("camera", new Line(new Point(camDist, -camLen / 2), new Point(camDist, camLen / 2)));
			for (int i = 0; i < rayAmount; i++)
			{
				var y = i * (camLen / rayAmount) - camLen / 2;
				var startPos = new Point(camDist, y);
				var endPos = new Point(camDist + rayLength, y);
				//var endPos = Point.MoveAtAngle(startPos, i * (fov / rayAmount) - fov / 2, rayLength, Time.Unit.Tick);
				Component2D.ComponentHitbox.SetLine($"ray-{i}", new Line(startPos, endPos));
			}
		}

		public override void OnKeyHold(Keyboard.Key key)
		{
			var sp = 0;
			if (key == Keyboard.Key.W) sp = 200;
			if (key == Keyboard.Key.S) sp = -200;
			if (key == Keyboard.Key.A) Component2D.Angle = Number.Move(Component2D.Angle, -100);
			if (key == Keyboard.Key.D) Component2D.Angle = Number.Move(Component2D.Angle, 100);
			Component2D.Position = Point.MoveAtAngle(Component2D.Position, Component2D.Angle, sp);
		}

		public override void OnDraw(ComponentCamera camera)
      {
			Cam.Position = Component2D.Position;
			Cam.Angle = Component2D.Angle + 90;

			var rays = Component2D.ComponentHitbox.Lines;
			if (camera == ComponentCamera.WorldCamera)
			{
				var scrSz = ComponentCamera.WorldCamera.Size;
				var topL = new Point(-scrSz.W / 2, -scrSz.H / 2);
				var topR = new Point(scrSz.W / 2, -scrSz.H / 2);
				var botR = new Point(scrSz.W / 2, 0);
				var botL = new Point(-scrSz.W / 2, 0);
				topL.Color = Color.BlueLight;
				topR.Color = Color.BlueLight;
				botR.Color = Color.CyanLight;
				botL.Color = Color.CyanLight;
				var sky = new Shape(topL, topR, botR, botL);
				sky.Draw(camera);

				var topL1 = new Point(-scrSz.W / 2, 0);
				var topR1 = new Point(scrSz.W / 2, 0);
				var botR1 = new Point(scrSz.W / 2, scrSz.H / 2);
				var botL1 = new Point(-scrSz.W / 2, scrSz.H / 2);
				topL1.Color = new Color(0, 50, 0);
				topR1.Color = new Color(0, 50, 0);
				botR1.Color = new Color(0, 150, 0);
				botL1.Color = new Color(0, 150, 0);
				var ground = new Shape(topL1, topR1, botR1, botL1);
				ground.Draw(camera);

				for (int i = 0; i < rays.Length; i++)
				{
					var mapLines = Map.Lines;
					for (int j = 0; j < mapLines.Length; j++)
					{
						var crossP = rays[i].CrossPoint(mapLines[j]);
						if (crossP.IsInvalid) continue;
						rays[i].EndPosition = crossP;

						var ang = Number.AngleBetweenPoints(rays[i].StartPosition, rays[i].EndPosition) - Component2D.Angle;
						var dist = (Point.Distance(rays[i].StartPosition, crossP));
						var pixelColumnHeight = rayLength - dist * Number.Cos(ang * Number.PI / 180);
						var pixelColumnWidth = ComponentCamera.WorldCamera.Size.W / rayAmount;
						var x = ((i - 1) * pixelColumnWidth) - ComponentCamera.WorldCamera.Size.W / 2;

						var topLeft = new Point(x, -pixelColumnHeight / 2);
						var topRight = new Point(x + pixelColumnWidth, -pixelColumnHeight / 2);
						var bottomLeft = new Point(x, pixelColumnHeight / 2);
						var bottomRight = new Point(x + pixelColumnWidth, pixelColumnHeight / 2);

						var c = mapLines[j].StartPosition.Color;
						c = Color.Lighten(c, -dist / 6, Time.Unit.Tick);
						topLeft.Color = c;
						topRight.Color = c;
						bottomLeft.Color = c;
						bottomRight.Color = c;

						var shape = new Shape(topLeft, topRight, bottomRight, bottomLeft);
						shape.Draw(camera);
					}
				}
			}
			else
			{
				Map.Draw(camera);
				rays[rays.Length / 2].Draw(camera);
			}
		}
	}
}

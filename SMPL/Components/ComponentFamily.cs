using SFML.System;
using System.Collections.Generic;

namespace SMPL
{
	public class ComponentFamily
	{
		private readonly ComponentVisual owner;
		private ComponentVisual parent;
		public ComponentVisual Parent
		{
			get { return parent; }
			set
			{
				if (parent == value) return;

				var os = owner is ComponentSprite ? owner as ComponentSprite : null;
				var ot = owner is ComponentText ? owner as ComponentText : null;
				var ps = parent is ComponentSprite ? parent as ComponentSprite : null;
				var pt = parent is ComponentText ? parent as ComponentText : null;
				var vs = value is ComponentSprite ? value as ComponentSprite : null;
				var vt = value is ComponentText ? value as ComponentText : null; 

				var pos = Point.From(owner.transform.Position);
				var scale = os != null ? os.transform.sprite.Scale : ot.transform.text.Scale;
				var angle = owner.transform.Angle;
				var parentScale = parent == null ? Window.world.Scale :
					ps != null ? ps.transform.sprite.Scale : pt.transform.text.Scale;
				var futureParentScale = value == null ? Window.world.Scale :
					vs != null ? vs.transform.sprite.Scale : vt.transform.text.Scale;
				var prevPar = parent;

				parent = value;

				if (value != null) // parent
				{
					var parAng = parent.transform.Angle;
					var newPos = value.transform.sprite.InverseTransform.TransformPoint(pos);

					value.Family.children.Add(owner);
					owner.transform.Position = Point.To(newPos);
					if (os != null)
					{
						var tsz = new Vector2f(os.transform.sprite.TextureRect.Width, os.transform.sprite.TextureRect.Height);
						var ssc = new Vector2f(scale.X / futureParentScale.X, scale.Y / futureParentScale.Y);
						var sz = new Size(tsz.X * ssc.X, tsz.Y * ssc.Y);
						os.transform.Size = sz;
						Console.Log(os.transform.Size);
					}
					else
					{
						var sc = new Vector2f(scale.X / futureParentScale.X, scale.Y / futureParentScale.Y);
						ot.transform.Size *= Size.To(sc);
						ot.transform.text.Scale = sc;
					}
					owner.transform.Angle = -(parAng - angle);
				}
				else // unparent
				{
					var newPos = prevPar == null ? pos : prevPar.transform.sprite.Transform.TransformPoint(pos);
					var parAng = prevPar.transform.sprite.Rotation;

					prevPar.Family.children.Remove(owner);
					owner.transform.Position = Point.To(newPos);
					if (os != null)
					{
						var tsz = new Vector2f(os.transform.sprite.TextureRect.Width, os.transform.sprite.TextureRect.Height);
						var ssc = new Vector2f(scale.X * parentScale.X, scale.Y * parentScale.Y);
						var sz = new Size(tsz.X * ssc.X, tsz.Y * ssc.Y);
						os.transform.Size = sz;
					}
					else
					{
						var sc = prevPar == null ? scale : new Vector2f(scale.X * parentScale.X, scale.Y * parentScale.Y);
						ot.transform.Size /= Size.To(scale);
						ot.transform.text.Scale = sc;
					}
					owner.transform.Angle = parAng + angle;
				}
			}
		}
		internal List<ComponentVisual> children = new();

		public Point Position { get; set; }
		public double Angle { get; set; }
		public Size Size { get; set; }

		public ComponentFamily(ComponentVisual owner) => this.owner = owner;
	}
}

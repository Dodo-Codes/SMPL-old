using SFML.Graphics;
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

				var pos = Point.From(owner.transform.Position);
				var scale = owner.transform.sprite.Scale;
				var angle = owner.transform.Angle;
				var parentScale = parent == null ? Window.world.Scale :
					parent is ComponentSprite ? parent.transform.sprite.Scale : parent.transform.text.Scale;
				var futureParentScale = value == null ? Window.world.Scale :
					value is ComponentSprite ? value.transform.sprite.Scale : value.transform.text.Scale;
				var prevPar = parent;

				parent = value;

				if (value != null) // parent
				{
					var parAng = parent.transform.Angle;
					var newPos = value.transform.sprite.InverseTransform.TransformPoint(pos);
					var ssc = new Vector2f(scale.X / futureParentScale.X, scale.Y / futureParentScale.Y);

					value.Family.children.Add(owner);
					owner.transform.Position = Point.To(newPos);
					if (owner is ComponentSprite)
					{
						var tsz = new Vector2f(owner.transform.sprite.TextureRect.Width, owner.transform.sprite.TextureRect.Height);
						var sz = new Size(tsz.X * ssc.X, tsz.Y * ssc.Y);
						owner.transform.Size = sz;
						Console.Log(owner.transform.Size);
					}
					owner.transform.Angle = -(parAng - angle);
				}
				else // unparent
				{
					var newPos = prevPar == null ? pos : prevPar.transform.sprite.Transform.TransformPoint(pos);
					var parAng = prevPar.transform.sprite.Rotation;
					var ssc = new Vector2f(scale.X * parentScale.X, scale.Y * parentScale.Y);

					prevPar.Family.children.Remove(owner);
					owner.transform.Position = Point.To(newPos);
					if (owner is ComponentSprite)
					{
						var tsz = new Vector2f(owner.transform.sprite.TextureRect.Width, owner.transform.sprite.TextureRect.Height);
						var sz = new Size(tsz.X * ssc.X, tsz.Y * ssc.Y);
						owner.transform.Size = sz;
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

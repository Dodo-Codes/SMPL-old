using SFML.System;
using System.Collections.Generic;

namespace SMPL
{
	public class ComponentFamily
	{
		private ComponentVisual parent, owner;
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

				parent = value;

				if (value != null)
				{
					var parAng = parent.transform.Angle;
					var newPos = value.transform.sprite.InverseTransform.TransformPoint(pos);
					var scaleW = scale.X / futureParentScale.X;
					var scaleH = scale.Y / futureParentScale.Y;

					value.Family.children.Add(owner);
					//Area.Position.Set(newPos.X, newPos.Y, true);
					//Area.Scale.Set(scaleW, scaleH, true);
					//Area.Angle.Set(-(parAng - angle), true);
				}
				else
				{

				}
			}
		}
		internal List<ComponentVisual> children;

		public Point Position { get; set; }
		public double Angle { get; set; }
		public Size Size { get; set; }

		public ComponentFamily(ComponentVisual owner) => this.owner = owner;
		private void UpdateChildrenAngles()
		{
			for (int i = 0; i < children.Count; i++)
			{
				var ownerAng = GetTr(owner).Item2;
				var childAng = GetTr(children[i]).Item2;
				var globalAngle = ownerAng + childAng;
				children[i].transform.Angle = Number.Limit(globalAngle, new Bounds(0, 359), Number.Limitation.Overflow);
			}
		}

		private (Vector2f, double, Vector2f) GetTr(ComponentVisual vis)
		{
			var s = vis is ComponentSprite ? vis as ComponentSprite : null;
			var t = vis is ComponentText ? vis as ComponentText : null;
			var pos = s != null ? s.transform.sprite.Position : t.transform.text.Position;
			var ang = s != null ? s.transform.sprite.Rotation : t.transform.text.Rotation;
			var sc = s != null ? s.transform.sprite.Scale : t.transform.text.Scale;
			return (pos, ang, sc);
		}
	}
}

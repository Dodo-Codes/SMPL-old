using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using SMPL.Data;
using SMPL.Gear;

namespace SMPL.Components
{
	public class Family : Thing
	{
		private uint visualParentUID;
		internal uint visualOwnerUID;
		internal List<uint> visualChildrenUIDs = new();

		public uint VisualParentUID
		{
			get { return ErrorIfDestroyed() ? default : visualParentUID; }
			set
			{
				if (ErrorIfDestroyed()) return;

				var owner = (Visual)Pick(visualOwnerUID);
				var ownerArea = (Area)Pick(owner.AreaUID);
				var pos = Point.From(ownerArea.LocalPosition);
				var angle = ownerArea.LocalAngle;
				var prevPar = (Visual)Pick(visualParentUID);
				var prevParArea = (Area)Pick(prevPar.AreaUID);
				var prevParFamily = (Family)Pick(prevPar.FamilyUID);

				visualParentUID = value;

				var parent = (Visual)Pick(visualParentUID);
				var parentArea = (Area)Pick(parent.AreaUID);
				var parentFamily = (Family)Pick(parent.FamilyUID);
				if (value != default) // parent
				{
					var parAng = parentArea.LocalAngle;
					var newPos = parentArea.sprite.InverseTransform.TransformPoint(pos);

					parentFamily.visualChildrenUIDs.Add(visualOwnerUID);
					ownerArea.LocalPosition = Point.To(newPos);
					ownerArea.LocalSize -= parentArea.Size;
					ownerArea.LocalAngle = -(parAng - angle);
				}
				else // unparent
				{
					var newPos = prevPar == null ? pos : prevParArea.sprite.Transform.TransformPoint(pos);
					var parAng = prevParArea.sprite.Rotation;

					prevParFamily.visualChildrenUIDs.Remove(visualOwnerUID);
					ownerArea.LocalPosition = Point.To(newPos);
					ownerArea.LocalSize += prevParArea.Size;
					ownerArea.LocalAngle = parAng + angle;
				}
			}
		}
		public List<uint> ChildrenUIDs => ErrorIfDestroyed() ? new() : new(visualChildrenUIDs);

		public void ParentChildren(params uint[] visualChildrenUIDs)
		{
			if (ErrorIfDestroyed()) return;
			if (visualChildrenUIDs == null) { Debug.LogError(1, "ComponentVisual children cannot be 'null'."); return; }
			for (int i = 0; i < visualChildrenUIDs.Length; i++)
			{
				if (this.visualChildrenUIDs.Contains(visualChildrenUIDs[i])) continue;

				var child = (Visual)Pick(visualChildrenUIDs[i]);
				var childFamily = (Family)Pick(child.FamilyUID);
				if (child.FamilyUID == default)
				{
					Debug.LogError(1, $"Cannot parent this child instance because it has no Family.\n" +
						$"Both (parent & child) Visual instances need a Family in order to bond.");
					continue;
				}
				childFamily.visualParentUID = visualOwnerUID;
			}
		}
		public void UnparentChildren(params uint[] visualChildrenUIDs)
		{
			if (ErrorIfDestroyed()) return;
			if (visualChildrenUIDs == null) { Debug.LogError(1, "ComponentVisual children cannot be 'null'."); return; }
			for (int i = 0; i < visualChildrenUIDs.Length; i++)
			{
				if (this.visualChildrenUIDs.Contains(visualChildrenUIDs[i]) == false) continue;
				var childFamily = (Family)Pick(visualChildrenUIDs[i]);
				childFamily.visualParentUID = default;
			}
		}
		public void UnparentAllChildren()
		{
			if (ErrorIfDestroyed()) return;
			for (int i = 0; i < visualChildrenUIDs.Count; i++)
			{
				var childFamily = (Family)Pick(visualChildrenUIDs[i]);
				childFamily.visualParentUID = default;
			}
		}
		public bool HasChildren(params uint[] visualChildrenUIDs)
		{
			if (visualChildrenUIDs == null)
			{
				Debug.LogError(1, "The collection of ComponentHitbox instances cannot be 'null'.");
				return false;
			}
			for (int i = 0; i < visualChildrenUIDs.Length; i++)
				if (this.visualChildrenUIDs.Contains(visualChildrenUIDs[i]) == false)
					return false;
			return true;
		}

		public override void Destroy()
		{
			if (ErrorIfDestroyed()) return;
			UnparentAllChildren();
			visualParentUID = default;
			visualOwnerUID = default;
			base.Destroy();
		}
	}
}

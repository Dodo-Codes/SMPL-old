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
		private string visualParentUID;

		//============

		internal string visualOwnerUID;
		internal List<string> visualChildrenUIDs = new();

		//============

		public string VisualParentUniqueID
		{
			get { return ErrorIfDestroyed() ? default : visualParentUID; }
			set
			{
				if (ErrorIfDestroyed()) return;

				var owner = (Visual)PickByUniqueID(visualOwnerUID);
				var ownerArea = (Area)PickByUniqueID(owner.AreaUniqueID);
				var pos = Point.From(ownerArea.LocalPosition);
				var angle = ownerArea.LocalAngle;
				var prevPar = (Visual)PickByUniqueID(visualParentUID);
				var prevParArea = (Area)PickByUniqueID(prevPar.AreaUniqueID);
				var prevParFamily = (Family)PickByUniqueID(prevPar.FamilyUniqueID);

				visualParentUID = value;

				var parent = (Visual)PickByUniqueID(visualParentUID);
				var parentArea = (Area)PickByUniqueID(parent.AreaUniqueID);
				var parentFamily = (Family)PickByUniqueID(parent.FamilyUniqueID);
				if (value != null) // parent
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
		public string[] ChildrenUniqueIDs => ErrorIfDestroyed() ? default : visualChildrenUIDs.ToArray();

		public void ParentChildren(params string[] visualChildrenUIDs)
		{
			if (ErrorIfDestroyed()) return;
			if (visualChildrenUIDs == null) { Debug.LogError(1, "ComponentVisual children cannot be 'null'."); return; }
			for (int i = 0; i < visualChildrenUIDs.Length; i++)
			{
				if (this.visualChildrenUIDs.Contains(visualChildrenUIDs[i])) continue;

				var child = (Visual)PickByUniqueID(visualChildrenUIDs[i]);
				var childFamily = (Family)PickByUniqueID(child.FamilyUniqueID);
				if (child.FamilyUniqueID == null)
				{
					Debug.LogError(1, $"Cannot parent this child instance because it has no Family.\n" +
						$"Both (parent & child) Visual instances need a Family in order to bond.");
					continue;
				}
				childFamily.visualParentUID = visualOwnerUID;
			}
		}
		public void UnparentChildren(params string[] visualChildrenUIDs)
		{
			if (ErrorIfDestroyed()) return;
			if (visualChildrenUIDs == null) { Debug.LogError(1, "ComponentVisual children cannot be 'null'."); return; }
			for (int i = 0; i < visualChildrenUIDs.Length; i++)
			{
				if (this.visualChildrenUIDs.Contains(visualChildrenUIDs[i]) == false) continue;
				var childFamily = (Family)PickByUniqueID(visualChildrenUIDs[i]);
				childFamily.visualParentUID = null;
			}
		}
		public void UnparentAllChildren()
		{
			if (ErrorIfDestroyed()) return;
			for (int i = 0; i < visualChildrenUIDs.Count; i++)
			{
				var childFamily = (Family)PickByUniqueID(visualChildrenUIDs[i]);
				childFamily.visualParentUID = null;
			}
		}
		public bool HasChildren(params string[] visualChildrenUIDs)
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

		public Family(string uniqueID) : base(uniqueID)
		{
			if (cannotCreate) { ErrorAlreadyHasUID(uniqueID); Destroy(); }
		}
		public override void Destroy()
		{
			if (ErrorIfDestroyed()) return;
			UnparentAllChildren();
			visualParentUID = null;
			visualOwnerUID = null;
			base.Destroy();
		}
	}
}

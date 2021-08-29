using SMPL.Gear;
using System;
using System.Collections.Generic;
using System.IO;

namespace SMPL.Components
{
	public class Access
	{
		internal List<string> accessPaths = new();
		public enum Extent { Varying, Allowed, Denied, Removed }

		private static event Events.ParamsTwo<Access, Identity<Access>> OnIdentityChange;
		private static event Events.ParamsOne<Access> OnCreate;
		private static event Events.ParamsTwo<Access, Extent> OnAllAccessChange;
		private static event Events.ParamsTwo<Access, string> OnGrantAccess, OnDenyAccess;

		public static class CallWhenAccess
		{
			public static void Create(Action<Access> method, uint order = uint.MaxValue) =>
				OnCreate = Events.Add(OnCreate, method, order);
			public static void IdentityChange(Action<Access, Identity<Components.Access>> method,
				uint order = uint.MaxValue) => OnIdentityChange = Events.Add(OnIdentityChange, method, order);
			public static void AllAccessChange(Action<Access, Extent> method, uint order = uint.MaxValue) =>
				OnAllAccessChange = Events.Add(OnAllAccessChange, method, order);
			public static void GrantChange(Action<Access, string> method, uint order = uint.MaxValue) =>
				OnGrantAccess = Events.Add(OnGrantAccess, method, order);
			public static void DenyChange(Action<Access, string> method, uint order = uint.MaxValue) =>
				OnDenyAccess = Events.Add(OnDenyAccess, method, order);
		}

		private Identity<Access> identity;
		public Identity<Access> AccessIdentity
		{
			get { return identity; }
			set
			{
				if (identity == value || (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false)) return;
				var prev = identity;
				identity = value;
				if (Debug.CalledBySMPL == false) OnIdentityChange?.Invoke(this, prev);
			}
		}

		private Extent access;
		public Extent AllAccess
		{
			get { return access; }
			set
			{
				if (access == value || (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false)) return;
				var prev = access;
				access = value;
				if (Debug.CalledBySMPL == false && value == Extent.Removed)
				{
					if (this is Sprite) (this as Sprite).IsDestroyed = true;
					else if (this is TextBox) (this as TextBox).IsDestroyed = true;
					else if (this is Audio)
					{

					}
					else if (this is Area)
					{

					}
				}
				if (Debug.CalledBySMPL == false) OnAllAccessChange?.Invoke(this, prev);
			}
		}
		public void GrantAccessToFile(string fullFilePath)
		{
			if (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false) return;
			if (fullFilePath == null)
			{
				Debug.LogError(1, "The file path cannot be 'null'.");
				return;
			}
			if (accessPaths.Contains(fullFilePath))
			{
				Debug.LogError(1, $"The file '{fullFilePath}' already has access.");
				return;
			}
			if (System.IO.File.Exists($"{Path.GetDirectoryName(fullFilePath)}\\{Debug.engineKey}.txt"))
				fullFilePath = "SMPL";
			accessPaths.Add(fullFilePath);
			if (Debug.CalledBySMPL == false) OnGrantAccess?.Invoke(this, fullFilePath);
		}
		public void DenyAccessToFile(string fullFilePath)
		{
			if (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false) return;
			if (accessPaths.Contains(fullFilePath) == false)
			{
				Debug.LogError(1, $"The file '{fullFilePath}' access is already denied.");
				return;
			}
			accessPaths.Remove(fullFilePath);
			if (Debug.CalledBySMPL == false) OnDenyAccess?.Invoke(this, fullFilePath);
		}
		public bool IsCurrentlyAccessible(bool displayError = true)
		{
			if (AllAccess == Extent.Allowed) return true;
			else if (AllAccess == Extent.Denied)
			{
				Debug.LogError(2, $"All access to this component is denied.\n" +
					$"No interaction is allowed but information can still be retrieved from it.");
				return false;
			}
			else if (AllAccess == Extent.Removed)
			{
				Debug.LogError(2, $"All access to this component is removed (as well as the component itself).\n" +
					$"Make sure to have no references (fields & properties) towards destroyed\n" +
					$"components (or set them to null when destroyed). This way the Garbage Collector\n" +
					$"will be able to dispose of them completely.");
				return false;
			}

			var filePath = Debug.CurrentFilePath(2);
			if (accessPaths.Contains(filePath)) return true;
			if (displayError == false) return false;
			var filesWithAccess = "";
			for (int i = 0; i < accessPaths.Count; i++)
			{
				filesWithAccess += $"- {accessPaths[i]}";
				if (i < accessPaths.Count - 1) filesWithAccess += "\n";
			}
			Debug.LogError(2, $"Access was denied for '{filePath}'.\n" +
				$"'{Debug.CurrentMethodName(1)}'\n" +
				$"can be accessed from the following files:\n{filesWithAccess}");
			return false;
		}
		public bool FileHasAccess(string fullFilePath)
		{
			return AllAccess switch
			{
				Extent.Allowed => true,
				Extent.Denied or Extent.Removed => false,
				_ => accessPaths.Contains(fullFilePath),
			};
		}

		public Access()
		{
			accessPaths.Add(Debug.CurrentFilePath(1));
			OnCreate?.Invoke(this);
		}
	}
}

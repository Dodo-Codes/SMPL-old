using System;
using System.Collections.Generic;

namespace SMPL
{
	public class ComponentAccess
	{
		internal List<string> accessPaths = new();
		public enum Access { Varying, Allowed, Denied }

		private static event Events.ParamsTwo<ComponentAccess, ComponentIdentity<ComponentAccess>> OnIdentityChange;
		private static event Events.ParamsTwo<ComponentAccess, Access> OnAllAccessChange;
		private static event Events.ParamsTwo<ComponentAccess, string> OnGrantAccess, OnDenyAccess;

		public static void CallOnIdentityChange(Action<ComponentAccess, ComponentIdentity<ComponentAccess>> method,
			uint order = uint.MaxValue) => OnIdentityChange = Events.Add(OnIdentityChange, method, order);
		public static void CallOnAllAccessChange(Action<ComponentAccess, Access> method, uint order = uint.MaxValue) =>
			OnAllAccessChange = Events.Add(OnAllAccessChange, method, order);
		public static void CallOnGrantAccess(Action<ComponentAccess, string> method, uint order = uint.MaxValue) =>
			OnGrantAccess = Events.Add(OnGrantAccess, method, order);
		public static void CallOnDenyAccess(Action<ComponentAccess, string> method, uint order = uint.MaxValue) =>
			OnDenyAccess = Events.Add(OnDenyAccess, method, order);

		private ComponentIdentity<ComponentAccess> identity;
		public ComponentIdentity<ComponentAccess> AccessIdentity
		{
			get { return identity; }
			set
			{
				if (identity == value || (Debug.CurrentMethodIsCalledByUser && IsCurrentlyAccessible() == false)) return;
				var prev = identity;
				identity = value;
				OnIdentityChange?.Invoke(this, prev);
			}
		}

		private Access access;
		public Access AllAccess
		{
			get { return access; }
			set
			{
				if (access == value || (Debug.CurrentMethodIsCalledByUser && IsCurrentlyAccessible() == false)) return;
				var prev = access;
				access = value;
				OnAllAccessChange?.Invoke(this, prev);
			}
		}
		public void GrantAccessToFile(string fullFilePath)
		{
			if (Debug.CurrentMethodIsCalledByUser && IsCurrentlyAccessible() == false) return;
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
			accessPaths.Add(fullFilePath);
		}
		public void DenyAccessToFile(string fullFilePath)
		{
			if (Debug.CurrentMethodIsCalledByUser && IsCurrentlyAccessible() == false) return;
			if (accessPaths.Contains(fullFilePath) == false)
			{
				Debug.LogError(1, $"The file '{fullFilePath}' access is already denied.");
				return;
			}
			accessPaths.Remove(fullFilePath);
		}
		public bool IsCurrentlyAccessible(bool displayError = true)
		{
			if (AllAccess == Access.Allowed) return true;
			else if (AllAccess == Access.Denied) return false;

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
				$"'{Debug.CurrentMethodName(1)}'\ncan be accessed from the following files:\n" +
				filesWithAccess);
			return false;
		}
		public bool FileHasAccess(string fullFilePath)
		{
			if (AllAccess == Access.Allowed) return true;
			else if (AllAccess == Access.Denied) return false;
			return accessPaths.Contains(fullFilePath);
		}

		public ComponentAccess() => accessPaths.Add(Debug.CurrentFilePath(2));
	}
}

using System;
using System.Collections.Generic;

namespace SMPL
{
	public class ComponentAccess
	{
		internal List<string> accessPaths = new();
		public enum Access { Varying, Allowed, Denied, Removed }

		private static event Events.ParamsTwo<ComponentAccess, ComponentIdentity<ComponentAccess>> OnIdentityChange;
		private static event Events.ParamsTwo<ComponentAccess, Access> OnAllAccessChange;
		private static event Events.ParamsTwo<ComponentAccess, string> OnGrantAccess, OnDenyAccess;

		public static class CallWhenAccess
		{
			public static void IdentityChange(Action<ComponentAccess, ComponentIdentity<ComponentAccess>> method,
				uint order = uint.MaxValue) => OnIdentityChange = Events.Add(OnIdentityChange, method, order);
			public static void AllAccessChange(Action<ComponentAccess, Access> method, uint order = uint.MaxValue) =>
				OnAllAccessChange = Events.Add(OnAllAccessChange, method, order);
			public static void GrantChange(Action<ComponentAccess, string> method, uint order = uint.MaxValue) =>
				OnGrantAccess = Events.Add(OnGrantAccess, method, order);
			public static void DenyChange(Action<ComponentAccess, string> method, uint order = uint.MaxValue) =>
				OnDenyAccess = Events.Add(OnDenyAccess, method, order);
		}

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
				if (value == Access.Removed)
				{
					if (this is ComponentSprite)
					{
						var sprite = this as ComponentSprite;
						if (sprite.image != null) sprite.image.Dispose();
						if (sprite.rawTexture != null) sprite.rawTexture.Dispose();
						if (sprite.rawTextureShader != null) sprite.rawTextureShader.Dispose();
						if (sprite.Effects.shader != null) sprite.Effects.shader.Dispose();
						if (sprite.transform.sprite != null) sprite.transform.sprite.Dispose();
						if (sprite.Identity != null)
						{
							ComponentIdentity<ComponentSprite>.uniqueIDs.Remove(sprite.identity.UniqueID);
							if (ComponentIdentity<ComponentSprite>.objTags.ContainsKey(sprite))
							{
								sprite.Identity.RemoveAllTags();
								ComponentIdentity<ComponentSprite>.objTags.Remove(sprite);
							}
						}
						ComponentSprite.sprites.Remove(sprite);
					}
					else if (this is ComponentText)
					{

					}
					else if (this is Component2D)
					{

					}
					else if (this is ComponentAudio)
					{

					}
				}
				if (Debug.CurrentMethodIsCalledByUser == false) return;
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
			if (Debug.CurrentMethodIsCalledByUser == false) return;
			OnGrantAccess?.Invoke(this, fullFilePath);
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
			if (Debug.CurrentMethodIsCalledByUser == false) return;
			OnDenyAccess?.Invoke(this, fullFilePath);
		}
		public bool IsCurrentlyAccessible(bool displayError = true)
		{
			if (AllAccess == Access.Allowed) return true;
			else if (AllAccess == Access.Denied)
			{
				Debug.LogError(2, $"All access to this component is denied.\n" +
					$"No interaction is allowed but information can still be retrieved from it.");
				return false;
			}
			else if (AllAccess == Access.Removed)
			{
				Debug.LogError(2, $"All access to this component is removed.\n" +
					$"The component is destroyed due to this.");
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
				Access.Allowed => true,
				Access.Denied or Access.Removed => false,
				_ => accessPaths.Contains(fullFilePath),
			};
		}

		public ComponentAccess() => accessPaths.Add(Debug.CurrentFilePath(2));
	}
}

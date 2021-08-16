using System;
using System.Collections.Generic;

namespace SMPL
{
	public abstract class Events
	{
		public delegate void ParamsZero();
		public delegate void ParamsOne<T>(T param1);
		public delegate void ParamsTwo<T1, T2>(T1 param1, T2 param2);
		public delegate void ParamsThree<T1, T2, T3>(T1 param1, T2 param2, T3 param3);
		public delegate void ParamsFour<T1, T2, T3, T4>(T1 param1, T2 param2, T3 param3, T4 param4);

		internal static ParamsZero Add(ParamsZero pz, Action method, uint order)
		{
			if (pz == null) { pz = new ParamsZero(method); return pz; }
			var l = pz.GetInvocationList();
			for (uint i = 0; i < l.Length; i++)
			{
				var a = l[i] as ParamsZero;
				pz -= a; if (i == order) pz += new ParamsZero(method); pz += a;
			}
			if (order >= l.Length) pz += new ParamsZero(method);
			return pz;
		}
		internal static ParamsOne<T1> Add<T1>(ParamsOne<T1> pz, Action<T1> method, uint order)
		{
			if (pz == null) { pz = new ParamsOne<T1>(method); return pz; }
			var l = pz.GetInvocationList();
			for (uint i = 0; i < l.Length; i++)
			{
				var a = l[i] as ParamsOne<T1>;
				pz -= a; if (i == order) pz += new ParamsOne<T1>(method); pz += a;
			}
			if (order >= l.Length) pz += new ParamsOne<T1>(method);
			return pz;
		}
		internal static ParamsTwo<T1, T2> Add<T1, T2>(ParamsTwo<T1, T2> pz, Action<T1, T2> method, uint order)
		{
			if (pz == null) { pz = new ParamsTwo<T1, T2>(method); return pz; }
			var l = pz.GetInvocationList();
			for (uint i = 0; i < l.Length; i++)
			{
				var a = l[i] as ParamsTwo<T1, T2>;
				pz -= a; if (i == order) pz += new ParamsTwo<T1, T2>(method); pz += a;
			}
			if (order >= l.Length) pz += new ParamsTwo<T1, T2>(method);
			return pz;
		}
		internal static ParamsThree<T1, T2, T3> Add<T1, T2, T3>(
			ParamsThree<T1, T2, T3> pz, Action<T1, T2, T3> method, uint order)
		{
			if (pz == null) { pz = new ParamsThree<T1, T2, T3>(method); return pz; }
			var l = pz.GetInvocationList();
			for (uint i = 0; i < l.Length; i++)
			{
				var a = l[i] as ParamsThree<T1, T2, T3>;
				pz -= a; if (i == order) pz += new ParamsThree<T1, T2, T3>(method); pz += a;
			}
			if (order >= l.Length) pz += new ParamsThree<T1, T2, T3>(method);
			return pz;
		}
		internal static ParamsFour<T1, T2, T3, T4> Add<T1, T2, T3, T4>(
			ParamsFour<T1, T2, T3, T4> pz, Action<T1, T2, T3, T4> method, uint order)
		{
			if (pz == null) { pz = new ParamsFour<T1, T2, T3, T4>(method); return pz; }
			var l = pz.GetInvocationList();
			for (uint i = 0; i < l.Length; i++)
			{
				var a = l[i] as ParamsFour<T1, T2, T3, T4>;
				pz -= a; if (i == order) pz += new ParamsFour<T1, T2, T3, T4>(method); pz += a;
			}
			if (order >= l.Length) pz += new ParamsFour<T1, T2, T3, T4>(method);
			return pz;
		}

		internal static SortedDictionary<int, List<Events>> instances = new();
		internal static Dictionary<Events, int> instancesOrder = new();

		internal static List<T> L<T>(List<T> list) => new List<T>(list);
		internal static SortedDictionary<T, T1> D<T, T1>(SortedDictionary<T, T1> dict) => new SortedDictionary<T, T1>(dict);
		//=================================================================

		public virtual void OnTextCreateSetup(ComponentText instance) { }
		public virtual void OnTextCreate(ComponentText instance) { }
		public virtual void OnTextMoveSetup(ComponentText instance, Point delta) { }
		public virtual void OnTextMove(ComponentText instance, Point delta) { }
		public virtual void OnTextMoveStartSetup(ComponentText instance, Point delta) { }
		public virtual void OnTextMoveStart(ComponentText instance, Point delta) { }
		public virtual void OnTextMoveEndSetup(ComponentText instance) { }
		public virtual void OnTextMoveEnd(ComponentText instance) { }
		public virtual void OnTextRotateSetup(ComponentText instance, double delta) { }
		public virtual void OnTextRotate(ComponentText instance, double delta) { }
		public virtual void OnTextRotateStartSetup(ComponentText instance, double delta) { }
		public virtual void OnTextRotateStart(ComponentText instance, double delta) { }
		public virtual void OnTextRotateEndSetup(ComponentText instance) { }
		public virtual void OnTextRotateEnd(ComponentText instance) { }
		public virtual void OnTextRescaleSetup(ComponentText instance, Size delta) { }
		public virtual void OnTextRescale(ComponentText instance, Size delta) { }
		public virtual void OnTextRescaleStartSetup(ComponentText instance, Size delta) { }
		public virtual void OnTextRescaleStart(ComponentText instance, Size delta) { }
		public virtual void OnTextRescaleEndSetup(ComponentText instance) { }
		public virtual void OnTextRescaleEnd(ComponentText instance) { }
		public virtual void OnTextOriginateSetup(ComponentText instance, Point delta) { }
		public virtual void OnTextOriginate(ComponentText instance, Point delta) { }
		public virtual void OnTextOriginateStartSetup(ComponentText instance, Point delta) { }
		public virtual void OnTextOriginateStart(ComponentText instance, Point delta) { }
		public virtual void OnTextOriginateEndSetup(ComponentText instance) { }
		public virtual void OnTextOriginateEnd(ComponentText instance) { }
		public virtual void OnTextVisibilityChangeSetup(ComponentText instance) { }
		public virtual void OnTextVisibilityChange(ComponentText instance) { }
		public virtual void OnTextCharacterResizeSetup(ComponentText instance, int delta) { }
		public virtual void OnTextCharacterResize(ComponentText instance, int delta) { }
		public virtual void OnTextChangeSetup(ComponentText instance, string prevoious) { }
		public virtual void OnTextChange(ComponentText instance, string previous) { }
		public virtual void OnTextTintRecolorSetup(ComponentText instance, Color previous) { }
		public virtual void OnTextTintRecolor(ComponentText instance, Color previous) { }
		public virtual void OnTextBackgroundRecolorSetup(ComponentText instance, Color previous) { }
		public virtual void OnTextBackgroundRecolor(ComponentText instance, Color previous) { }
		public virtual void OnTextOutlineRecolorSetup(ComponentText instance, Color previous) { }
		public virtual void OnTextOutlineRecolor(ComponentText instance, Color previous) { }
		public virtual void OnTextOutlineResizeSetup(ComponentText instance, double delta) { }
		public virtual void OnTextOutlineResize(ComponentText instance, double delta) { }
		public virtual void OnTextSpacingResizeSetup(ComponentText instance, Size delta) { }
		public virtual void OnTextSpacingResize(ComponentText instance, Size delta) { }
		public virtual void OnTextChangeStartSetup(ComponentText instance, string prevoious) { }
		public virtual void OnTextChangeStart(ComponentText instance, string previous) { }
		public virtual void OnTextTintRecolorStartSetup(ComponentText instance, Color previous) { }
		public virtual void OnTextTintRecolorStart(ComponentText instance, Color previous) { }
		public virtual void OnTextBackgroundRecolorStartSetup(ComponentText instance, Color previous) { }
		public virtual void OnTextBackgroundRecolorStart(ComponentText instance, Color previous) { }
		public virtual void OnTextOutlineRecolorStartSetup(ComponentText instance, Color previous) { }
		public virtual void OnTextOutlineRecolorStart(ComponentText instance, Color previous) { }
		public virtual void OnTextOutlineResizeStartSetup(ComponentText instance, double delta) { }
		public virtual void OnTextOutlineResizeStart(ComponentText instance, double delta) { }
		public virtual void OnTextSpacingResizeStartSetup(ComponentText instance, Size delta) { }
		public virtual void OnTextSpacingResizeStart(ComponentText instance, Size delta) { }
		public virtual void OnTextChangeEndSetup(ComponentText instance) { }
		public virtual void OnTextChangeEnd(ComponentText instance) { }
		public virtual void OnTextTintRecolorEndSetup(ComponentText instance) { }
		public virtual void OnTextTintRecolorEnd(ComponentText instance) { }
		public virtual void OnTextBackgroundRecolorEndSetup(ComponentText instance) { }
		public virtual void OnTextBackgroundRecolorEnd(ComponentText instance) { }
		public virtual void OnTextOutlineRecolorEndSetup(ComponentText instance) { }
		public virtual void OnTextOutlineRecolorEnd(ComponentText instance) { }
		public virtual void OnTextOutlineResizeEndSetup(ComponentText instance) { }
		public virtual void OnTextOutlineResizeEnd(ComponentText instance) { }
		public virtual void OnTextSpacingResizeEndSetup(ComponentText instance) { }
		public virtual void OnTextSpacingResizeEnd(ComponentText instance) { }

		public virtual void OnSpriteCreateSetup(ComponentSprite instance) { }
		public virtual void OnSpriteCreate(ComponentSprite instance) { }
		public virtual void OnSpriteRepeatingChangeSetup(ComponentSprite instance) { }
		public virtual void OnSpriteRepeatingChange(ComponentSprite instance) { }
		public virtual void OnSpriteSmoothingChangeSetup(ComponentSprite instance) { }
		public virtual void OnSpriteSmoothingChange(ComponentSprite instance) { }
		public virtual void OnSpriteVisibilityChangeSetup(ComponentSprite instance) { }
		public virtual void OnSpriteVisibilityChange(ComponentSprite instance) { }
		public virtual void OnSpriteGridResizeSetup(ComponentSprite instance, Size delta) { }
		public virtual void OnSpriteGridResize(ComponentSprite instance, Size delta) { }
		public virtual void OnSpriteOffsetSetup(ComponentSprite instance, Point delta) { }
		public virtual void OnSpriteOffset(ComponentSprite instance, Point delta) { }
		public virtual void OnSpriteResizeSetup(ComponentSprite instance, Size delta) { }
		public virtual void OnSpriteResize(ComponentSprite instance, Size delta) { }
		public virtual void OnSpriteOffsetStartSetup(ComponentSprite instance, Point delta) { }
		public virtual void OnSpriteOffsetStart(ComponentSprite instance, Point delta) { }
		public virtual void OnSpriteResizeStartSetup(ComponentSprite instance, Size delta) { }
		public virtual void OnSpriteResizeStart(ComponentSprite instance, Size delta) { }
		public virtual void OnSpriteOffsetEndSetup(ComponentSprite instance) { }
		public virtual void OnSpriteOffsetEnd(ComponentSprite instance) { }
		public virtual void OnSpriteResizeEndSetup(ComponentSprite instance) { }
		public virtual void OnSpriteResizeEnd(ComponentSprite instance) { }
		public virtual void OnSpriteTintRecolorSetup(ComponentSprite instance, Color previous) { }
		public virtual void OnSpriteTintRecolor(ComponentSprite instance, Color previous) { }
		public virtual void OnSpriteTintRecolorStartSetup(ComponentSprite instance, Color previous) { }
		public virtual void OnSpriteTintRecolorStart(ComponentSprite instance, Color previous) { }
		public virtual void OnSpriteTintRecolorEndSetup(ComponentSprite instance) { }
		public virtual void OnSpriteTintRecolorEnd(ComponentSprite instance) { }
		public virtual void OnSpriteOutlineResizeSetup(ComponentSprite instance, double delta) { }
		public virtual void OnSpriteOutlineResize(ComponentSprite instance, double delta) { }
		public virtual void OnSpriteOutlineResizeStartSetup(ComponentSprite instance, double delta) { }
		public virtual void OnSpriteOutlineResizeStart(ComponentSprite instance, double delta) { }
		public virtual void OnSpriteOutlineResizeEndSetup(ComponentSprite instance) { }
		public virtual void OnSpriteOutlineResizeEnd(ComponentSprite instance) { }
		public virtual void OnSpriteOutlineRecolorSetup(ComponentSprite instance, Color previous) { }
		public virtual void OnSpriteOutlineRecolor(ComponentSprite instance, Color previous) { }
		public virtual void OnSpriteOutlineRecolorStartSetup(ComponentSprite instance, Color previous) { }
		public virtual void OnSpriteOutlineRecolorStart(ComponentSprite instance, Color previous) { }
		public virtual void OnSpriteOutlineRecolorEndSetup(ComponentSprite instance) { }
		public virtual void OnSpriteOutlineRecolorEnd(ComponentSprite instance) { }
		public virtual void OnSpriteBackgroundRecolorSetup(ComponentSprite instance, Color previous) { }
		public virtual void OnSpriteBackgroundRecolor(ComponentSprite instance, Color previous) { }
		public virtual void OnSpriteBackgroundRecolorStartSetup(ComponentSprite instance, Color previous) { }
		public virtual void OnSpriteBackgroundRecolorStart(ComponentSprite instance, Color previous) { }
		public virtual void OnSpriteBackgroundRecolorEndSetup(ComponentSprite instance) { }
		public virtual void OnSpriteBackgroundRecolorEnd(ComponentSprite instance) { }
	}
}

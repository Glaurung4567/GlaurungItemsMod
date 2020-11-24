using System.Collections.Generic;
using UnityEngine;

namespace ItemAPI
{
	internal class EasyGoopDefinitions
	{
		public static void DefineDefaultGoops()
		{
			AssetBundle assetBundle = ResourceManager.LoadAssetBundle("shared_auto_001");
			EasyGoopDefinitions.goopDefs = new List<GoopDefinition>();
			foreach (string text in EasyGoopDefinitions.goops)
			{
				GoopDefinition goopDefinition;
				try
				{
					GameObject gameObject = assetBundle.LoadAsset(text) as GameObject;
					goopDefinition = gameObject.GetComponent<GoopDefinition>();
				}
				catch
				{
					goopDefinition = (assetBundle.LoadAsset(text) as GoopDefinition);
				}
				goopDefinition.name = text.Replace("assets/data/goops/", "").Replace(".asset", "");
				EasyGoopDefinitions.goopDefs.Add(goopDefinition);
			}
			List<GoopDefinition> list = EasyGoopDefinitions.goopDefs;
			EasyGoopDefinitions.FireDef = EasyGoopDefinitions.goopDefs[0];
			EasyGoopDefinitions.OilDef = EasyGoopDefinitions.goopDefs[1];
			EasyGoopDefinitions.PoisonDef = EasyGoopDefinitions.goopDefs[2];
			EasyGoopDefinitions.BlobulonGoopDef = EasyGoopDefinitions.goopDefs[3];
			EasyGoopDefinitions.WebGoop = EasyGoopDefinitions.goopDefs[4];
			EasyGoopDefinitions.WaterGoop = EasyGoopDefinitions.goopDefs[5];
		}

		// Token: 0x0600026C RID: 620 RVA: 0x00015F84 File Offset: 0x00014184
		// Note: this type is marked as 'beforefieldinit'.
		static EasyGoopDefinitions()
		{
			PickupObject byId = PickupObjectDatabase.GetById(310);
			GoopDefinition charmGoopDef;
			if (byId == null)
			{
				charmGoopDef = null;
			}
			else
			{
				WingsItem component = byId.GetComponent<WingsItem>();
				charmGoopDef = ((component != null) ? component.RollGoop : null);
			}
			EasyGoopDefinitions.CharmGoopDef = charmGoopDef;
			EasyGoopDefinitions.GreenFireDef = (PickupObjectDatabase.GetById(698) as Gun).DefaultModule.projectiles[0].GetComponent<GoopModifier>().goopDefinition;
			EasyGoopDefinitions.CheeseDef = (PickupObjectDatabase.GetById(808) as Gun).DefaultModule.projectiles[0].GetComponent<GoopModifier>().goopDefinition;
			EasyGoopDefinitions.TripleCrossbow = (ETGMod.Databases.Items["triple_crossbow"] as Gun);
			EasyGoopDefinitions.TripleCrossbowEffect = EasyGoopDefinitions.TripleCrossbow.DefaultModule.projectiles[0].speedEffect;
		}

		// Token: 0x040000CB RID: 203
		private static string[] goops = new string[]
		{
			"assets/data/goops/napalmgoopthatworks.asset",
			"assets/data/goops/oil goop.asset",
			"assets/data/goops/poison goop.asset",
			"assets/data/goops/blobulongoop.asset",
			"assets/data/goops/phasewebgoop.asset",
			"assets/data/goops/water goop.asset"
		};

		// Token: 0x040000CC RID: 204
		private static List<GoopDefinition> goopDefs;

		// Token: 0x040000CD RID: 205
		public static GoopDefinition FireDef;

		// Token: 0x040000CE RID: 206
		public static GoopDefinition OilDef;

		// Token: 0x040000CF RID: 207
		public static GoopDefinition PoisonDef;

		// Token: 0x040000D0 RID: 208
		public static GoopDefinition BlobulonGoopDef;

		// Token: 0x040000D1 RID: 209
		public static GoopDefinition WebGoop;

		// Token: 0x040000D2 RID: 210
		public static GoopDefinition WaterGoop;

		// Token: 0x040000D4 RID: 212
		public static GoopDefinition CharmGoopDef;

		// Token: 0x040000D5 RID: 213
		public static GoopDefinition GreenFireDef;

		// Token: 0x040000D6 RID: 214
		public static GoopDefinition CheeseDef;

		// Token: 0x040000D7 RID: 215
		private static Gun TripleCrossbow;

		// Token: 0x040000D8 RID: 216
		private static GameActorSpeedEffect TripleCrossbowEffect;
	}

	public class ExtendedColours
	{
		// Token: 0x040000DD RID: 221
		//public static Color freezeBlue = EasyStatusEffectAccess.freezeModifierEffect.TintColor;

		// Token: 0x040000DE RID: 222
		//public static Color poisonGreen = EasyStatusEffectAccess.irradiatedLeadEffect.TintColor;

		// Token: 0x040000DF RID: 223
		public static Color pink = new Color(0.9490196f, 0.454901963f, 0.882352948f);

		// Token: 0x040000E0 RID: 224
		public static Color lime = new Color(0.435294122f, 0.9882353f, 0.0117647061f);

		// Token: 0x040000E1 RID: 225
		public static Color brown = new Color(0.478431374f, 0.2784314f, 0.0627451f);

		// Token: 0x040000E2 RID: 226
		public static Color orange = new Color(0.9411765f, 0.627451f, 0.08627451f);

		// Token: 0x040000E3 RID: 227
		public static Color purple = new Color(0.670588255f, 0.08627451f, 0.9411765f);

		// Token: 0x040000E4 RID: 228
		public static Color skyblue = new Color(0.509803951f, 0.9019608f, 8.843137f);

		// Token: 0x040000E5 RID: 229
		public static Color honeyYellow = new Color(1f, 0.7058824f, 0.07058824f);
	}
}

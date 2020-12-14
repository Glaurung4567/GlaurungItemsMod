using Brave.BulletScript;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ItemAPI
{
    public static class Toolbox
    {
		// Token: 0x060003BC RID: 956 RVA: 0x00029880 File Offset: 0x00027A80
		public static void Init()
		{
			AssetBundle assetBundle = ResourceManager.LoadAssetBundle("shared_auto_001");
			AssetBundle assetBundle2 = ResourceManager.LoadAssetBundle("shared_auto_002");
			shared_auto_001 = assetBundle;
			shared_auto_002 = assetBundle2;
			string text = "assets/data/goops/water goop.asset";
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
			Toolbox.DefaultWaterGoop = goopDefinition;
			text = "assets/data/goops/poison goop.asset";
			GoopDefinition goopDefinition2;
			try
			{
				GameObject gameObject2 = assetBundle.LoadAsset(text) as GameObject;
				goopDefinition2 = gameObject2.GetComponent<GoopDefinition>();
			}
			catch
			{
				goopDefinition2 = (assetBundle.LoadAsset(text) as GoopDefinition);
			}
			goopDefinition2.name = text.Replace("assets/data/goops/", "").Replace(".asset", "");
			Toolbox.DefaultPoisonGoop = goopDefinition2;
			text = "assets/data/goops/napalmgoopquickignite.asset";
			GoopDefinition goopDefinition3;
			try
			{
				GameObject gameObject3 = assetBundle.LoadAsset(text) as GameObject;
				goopDefinition3 = gameObject3.GetComponent<GoopDefinition>();
			}
			catch
			{
				goopDefinition3 = (assetBundle.LoadAsset(text) as GoopDefinition);
			}
			goopDefinition3.name = text.Replace("assets/data/goops/", "").Replace(".asset", "");
			Toolbox.DefaultFireGoop = goopDefinition3;
			PickupObject byId = PickupObjectDatabase.GetById(310);
			bool flag = byId == null;
			GoopDefinition defaultCharmGoop;
			if (flag)
			{
				defaultCharmGoop = null;
			}
			else
			{
				WingsItem component = byId.GetComponent<WingsItem>();
				defaultCharmGoop = ((component != null) ? component.RollGoop : null);
			}
			Toolbox.DefaultCharmGoop = defaultCharmGoop;
			Toolbox.DefaultCheeseGoop = (PickupObjectDatabase.GetById(626) as Gun).DefaultModule.projectiles[0].cheeseEffect.CheeseGoop;
			Toolbox.DefaultBlobulonGoop = EnemyDatabase.GetOrLoadByGuid("0239c0680f9f467dbe5c4aab7dd1eca6").GetComponent<GoopDoer>().goopDefinition;
			Toolbox.DefaultPoopulonGoop = EnemyDatabase.GetOrLoadByGuid("116d09c26e624bca8cca09fc69c714b3").GetComponent<GoopDoer>().goopDefinition;
			GameObject gameObject4 = new GameObject("SprenSpunVFX");
			gameObject4.SetActive(false);
			ItemAPI.FakePrefab.MarkAsFakePrefab(gameObject4);
			Object.DontDestroyOnLoad(gameObject4);
			tk2dSpriteAnimator tk2dSpriteAnimator = gameObject4.AddComponent<tk2dSpriteAnimator>();
			bool flag2 = tk2dSpriteAnimator.Library == null;
			if (flag2)
			{
				tk2dSpriteAnimator.Library = tk2dSpriteAnimator.gameObject.AddComponent<tk2dSpriteAnimation>();
				tk2dSpriteAnimator.Library.clips = new tk2dSpriteAnimationClip[0];
				tk2dSpriteAnimator.Library.enabled = true;
			}
			tk2dSpriteAnimator.gameObject.AddComponent<tk2dSprite>();
			SprenOrbitalItem sprenOrbitalItem = PickupObjectDatabase.GetById(578) as SprenOrbitalItem;
			tk2dSpriteAnimationClip[] array = new tk2dSpriteAnimationClip[0];
			foreach (tk2dSpriteAnimationClip tk2dSpriteAnimationClip in sprenOrbitalItem.OrbitalFollowerPrefab.GetComponentInChildren<tk2dSpriteAnimator>().Library.clips)
			{
				tk2dSpriteAnimationClip tk2dSpriteAnimationClip2 = new tk2dSpriteAnimationClip();
				tk2dSpriteAnimationClip2.name = tk2dSpriteAnimationClip.name;
				tk2dSpriteAnimationClip2.fps = tk2dSpriteAnimationClip.fps;
				tk2dSpriteAnimationClip2.loopStart = tk2dSpriteAnimationClip.loopStart;
				tk2dSpriteAnimationClip2.wrapMode = tk2dSpriteAnimationClip.wrapMode;
				tk2dSpriteAnimationClip2.minFidgetDuration = tk2dSpriteAnimationClip.minFidgetDuration;
				tk2dSpriteAnimationClip2.maxFidgetDuration = tk2dSpriteAnimationClip.maxFidgetDuration;
				tk2dSpriteAnimationFrame[] array2 = new tk2dSpriteAnimationFrame[0];
				foreach (tk2dSpriteAnimationFrame tk2dSpriteAnimationFrame in tk2dSpriteAnimationClip.frames)
				{
					array2 = array2.Concat(new tk2dSpriteAnimationFrame[]
					{
						new tk2dSpriteAnimationFrame
						{
							spriteCollection = tk2dSpriteAnimationFrame.spriteCollection,
							spriteId = tk2dSpriteAnimationFrame.spriteId,
							invulnerableFrame = tk2dSpriteAnimationFrame.invulnerableFrame,
							groundedFrame = tk2dSpriteAnimationFrame.groundedFrame,
							requiresOffscreenUpdate = tk2dSpriteAnimationFrame.requiresOffscreenUpdate,
							eventVfx = tk2dSpriteAnimationFrame.eventVfx,
							eventStopVfx = tk2dSpriteAnimationFrame.eventStopVfx,
							eventLerpEmissive = tk2dSpriteAnimationFrame.eventLerpEmissive,
							eventLerpEmissiveTime = tk2dSpriteAnimationFrame.eventLerpEmissiveTime,
							eventLerpEmissivePower = tk2dSpriteAnimationFrame.eventLerpEmissivePower,
							forceMaterialUpdate = tk2dSpriteAnimationFrame.forceMaterialUpdate,
							finishedSpawning = tk2dSpriteAnimationFrame.finishedSpawning,
							triggerEvent = tk2dSpriteAnimationFrame.triggerEvent,
							eventAudio = "",
							eventInfo = tk2dSpriteAnimationFrame.eventInfo,
							eventInt = tk2dSpriteAnimationFrame.eventInt,
							eventFloat = tk2dSpriteAnimationFrame.eventFloat,
							eventOutline = tk2dSpriteAnimationFrame.eventOutline
						}
					}).ToArray<tk2dSpriteAnimationFrame>();
				}
				tk2dSpriteAnimationClip2.frames = array2;
				array = array.Concat(new tk2dSpriteAnimationClip[]
				{
					tk2dSpriteAnimationClip2
				}).ToArray<tk2dSpriteAnimationClip>();
			}
			tk2dSpriteAnimator.Library.clips = array;
			Toolbox.SprenSpunBehaviour sprenSpunBehaviour = gameObject4.AddComponent<Toolbox.SprenSpunBehaviour>();
			sprenSpunBehaviour.GunChangeMoreAnimation = sprenOrbitalItem.GunChangeMoreAnimation;
			sprenSpunBehaviour.BackchangeAnimation = sprenOrbitalItem.BackchangeAnimation;
			Toolbox.SprenSpunPrefab = gameObject4;
		}

		public static void SetProjectileSpriteRight2(this Projectile proj, string name, int pixelWidth, int pixelHeight, int? overrideColliderPixelWidth = null, int? overrideColliderPixelHeight = null)
		{
			try
			{
				bool flag = overrideColliderPixelWidth == null;
				bool flag2 = flag;
				if (flag2)
				{
					overrideColliderPixelWidth = new int?(pixelWidth);
				}
				bool flag3 = overrideColliderPixelHeight == null;
				bool flag4 = flag3;
				if (flag4)
				{
					overrideColliderPixelHeight = new int?(pixelHeight);
				}
				float num = (float)pixelWidth / 16f;
				float num2 = (float)pixelHeight / 16f;
				float x = (float)overrideColliderPixelWidth.Value / 16f;
				float y = (float)overrideColliderPixelHeight.Value / 16f;
				ETGMod.GetAnySprite(proj).spriteId = ETGMod.Databases.Items.ProjectileCollection.inst.GetSpriteIdByName(name);
				tk2dSpriteDefinition tk2dSpriteDefinition = ETGMod.Databases.Items.ProjectileCollection.inst.spriteDefinitions[ETGMod.GetAnySprite((PickupObjectDatabase.GetById(12) as Gun).DefaultModule.projectiles[0]).spriteId].CopyDefinitionFrom();
				tk2dSpriteDefinition.boundsDataCenter = new Vector3(num / 2f, num2 / 2f, 0f);
				tk2dSpriteDefinition.boundsDataExtents = new Vector3(num, num2, 0f);
				tk2dSpriteDefinition.untrimmedBoundsDataCenter = new Vector3(num / 2f, num2 / 2f, 0f);
				tk2dSpriteDefinition.untrimmedBoundsDataExtents = new Vector3(num, num2, 0f);
				tk2dSpriteDefinition.position0 = new Vector3(0f, 0f, 0f);
				tk2dSpriteDefinition.position1 = new Vector3(0f + num, 0f, 0f);
				tk2dSpriteDefinition.position2 = new Vector3(0f, 0f + num2, 0f);
				tk2dSpriteDefinition.position3 = new Vector3(0f + num, 0f + num2, 0f);
				tk2dSpriteDefinition.colliderVertices[1].x = x;
				tk2dSpriteDefinition.colliderVertices[1].y = y;
				tk2dSpriteDefinition.name = name;
				ETGMod.Databases.Items.ProjectileCollection.inst.spriteDefinitions[ETGMod.GetAnySprite(proj).spriteId] = tk2dSpriteDefinition;
				proj.baseData.force = 0f;
			}
			catch (Exception ex)
			{
				ETGModConsole.Log("Ooops! Seems like something got very, Very, VERY wrong. Here's the exception:", false);
				ETGModConsole.Log(ex.ToString(), false);
			}
		}

		public static void SetProjectileSpriteRight(this Projectile proj, string name, int pixelWidth, int pixelHeight, bool lightened = true, tk2dBaseSprite.Anchor anchor = 0, int? overrideColliderPixelWidth = null, int? overrideColliderPixelHeight = null, int? overrideColliderOffsetX = null, int? overrideColliderOffsetY = null, Projectile overrideProjectileToCopyFrom = null)
		{
			try
			{
				ETGMod.GetAnySprite(proj).spriteId = ETGMod.Databases.Items.ProjectileCollection.inst.GetSpriteIdByName(name);
				tk2dSpriteDefinition tk2dSpriteDefinition = Toolbox.SetupDefinitionForProjectileSprite(name, ETGMod.GetAnySprite(proj).spriteId, pixelWidth, pixelHeight, lightened, overrideColliderPixelWidth, overrideColliderPixelHeight, overrideColliderOffsetX, overrideColliderOffsetY, overrideProjectileToCopyFrom);
				tk2dSpriteDefinition.ConstructOffsetsFromAnchor(anchor, tk2dSpriteDefinition.position3);
			}
			catch (Exception ex)
			{
				ETGModConsole.Log("Ooops! Seems like something got very, Very, VERY wrong. Here's the exception:", false);
				ETGModConsole.Log(ex.ToString(), false);
			}
		}

		public static void ConstructOffsetsFromAnchor(this tk2dSpriteDefinition def, tk2dBaseSprite.Anchor anchor, Vector2 scale)
		{
			float num = 0f;
			bool flag = anchor == (tk2dBaseSprite.Anchor)1 || anchor == (tk2dBaseSprite.Anchor)4 || anchor == (tk2dBaseSprite.Anchor)7;
			if (flag)
			{
				num = -(scale.x / 2f);
			}
			else
			{
				bool flag2 = anchor == (tk2dBaseSprite.Anchor)2 || anchor == (tk2dBaseSprite.Anchor)5 || anchor == (tk2dBaseSprite.Anchor)8;
				if (flag2)
				{
					num = -scale.x;
				}
			}
			float num2 = 0f;
			bool flag3 = anchor == (tk2dBaseSprite.Anchor)3 || anchor == (tk2dBaseSprite.Anchor)4 || anchor == (tk2dBaseSprite.Anchor)3;
			if (flag3)
			{
				num2 = -(scale.y / 2f);
			}
			else
			{
				bool flag4 = anchor == (tk2dBaseSprite.Anchor)6 || anchor == (tk2dBaseSprite.Anchor)7 || anchor == (tk2dBaseSprite.Anchor)8;
				if (flag4)
				{
					num2 = -scale.y;
				}
			}
			def.MakeOffset(new Vector2(num, num2));
		}

		public static AssetBundle specialeverything;
		public static AssetBundle shared_auto_002;
		public static AssetBundle shared_auto_001;

		public static void SafeMove(string oldPath, string newPath, bool allowOverwritting = false)
		{
			if (File.Exists(oldPath) && (allowOverwritting || !File.Exists(newPath)))
			{
				string contents = SaveManager.ReadAllText(oldPath);
				try
				{
					SaveManager.WriteAllText(newPath, contents);
				}
				catch (Exception ex)
				{
					Debug.LogErrorFormat("Failed to move {0} to {1}: {2}", new object[]
					{
					oldPath,
					newPath,
					ex
					});
					return;
				}
				try
				{
					File.Delete(oldPath);
				}
				catch (Exception ex2)
				{
					Debug.LogErrorFormat("Failed to delete old file {0}: {1}", new object[]
					{
					oldPath,
					newPath,
					ex2
					});
					return;
				}
				if (File.Exists(oldPath + ".bak"))
				{
					File.Delete(oldPath + ".bak");
				}
			}
		}

		public static string PathCombine(string a, string b, string c)
		{
			return Path.Combine(Path.Combine(a, b), c);
		}

		public static void SetupUnlockOnCustomFlag(this PickupObject self, CustomDungeonFlags flag, bool requiredFlagValue)
		{
			EncounterTrackable encounterTrackable = self.encounterTrackable;
			if (encounterTrackable.prerequisites == null)
			{
				encounterTrackable.prerequisites = new DungeonPrerequisite[1];
				encounterTrackable.prerequisites[0] = new AdvancedDungeonPrerequisite
				{
					advancedPrerequisiteType = AdvancedDungeonPrerequisite.AdvancedPrerequisiteType.CUSTOM_FLAG,
					requireCustomFlag = requiredFlagValue,
					customFlagToCheck = flag
				};
			}
			else
			{
				encounterTrackable.prerequisites = encounterTrackable.prerequisites.Concat(new DungeonPrerequisite[] { new AdvancedDungeonPrerequisite
				{
					advancedPrerequisiteType = AdvancedDungeonPrerequisite.AdvancedPrerequisiteType.CUSTOM_FLAG,
					requireCustomFlag = requiredFlagValue,
					customFlagToCheck = flag
				}}).ToArray();
			}
			EncounterDatabaseEntry databaseEntry = EncounterDatabase.GetEntry(encounterTrackable.EncounterGuid);
			if (!string.IsNullOrEmpty(databaseEntry.ProxyEncounterGuid))
			{
				databaseEntry.ProxyEncounterGuid = "";
			}
			if (databaseEntry.prerequisites == null)
			{
				databaseEntry.prerequisites = new DungeonPrerequisite[1];
				databaseEntry.prerequisites[0] = new AdvancedDungeonPrerequisite
				{
					advancedPrerequisiteType = AdvancedDungeonPrerequisite.AdvancedPrerequisiteType.CUSTOM_FLAG,
					requireCustomFlag = requiredFlagValue,
					customFlagToCheck = flag
				};
			}
			else
			{
				databaseEntry.prerequisites = databaseEntry.prerequisites.Concat(new DungeonPrerequisite[] { new AdvancedDungeonPrerequisite
				{
					advancedPrerequisiteType = AdvancedDungeonPrerequisite.AdvancedPrerequisiteType.CUSTOM_FLAG,
					requireCustomFlag = requiredFlagValue,
					customFlagToCheck = flag
				}}).ToArray();
			}
		}

		public static void MakeOffset(this tk2dSpriteDefinition def, Vector2 offset)
		{
			float x = offset.x;
			float y = offset.y;
			def.position0 += new Vector3(x, y, 0f);
			def.position1 += new Vector3(x, y, 0f);
			def.position2 += new Vector3(x, y, 0f);
			def.position3 += new Vector3(x, y, 0f);
			def.boundsDataCenter += new Vector3(x, y, 0f);
			def.boundsDataExtents += new Vector3(x, y, 0f);
			def.untrimmedBoundsDataCenter += new Vector3(x, y, 0f);
			def.untrimmedBoundsDataExtents += new Vector3(x, y, 0f);
		}

		public static tk2dSpriteDefinition SetupDefinitionForProjectileSprite(string name, int id, int pixelWidth, int pixelHeight, bool lightened = true, int? overrideColliderPixelWidth = null, int? overrideColliderPixelHeight = null, int? overrideColliderOffsetX = null, int? overrideColliderOffsetY = null, Projectile overrideProjectileToCopyFrom = null)
		{
			bool flag = overrideColliderPixelWidth == null;
			if (flag)
			{
				overrideColliderPixelWidth = new int?(pixelWidth);
			}
			bool flag2 = overrideColliderPixelHeight == null;
			if (flag2)
			{
				overrideColliderPixelHeight = new int?(pixelHeight);
			}
			bool flag3 = overrideColliderOffsetX == null;
			if (flag3)
			{
				overrideColliderOffsetX = new int?(0);
			}
			bool flag4 = overrideColliderOffsetY == null;
			if (flag4)
			{
				overrideColliderOffsetY = new int?(0);
			}
			float num = 14f;
			float num2 = 16f;
			float num3 = (float)pixelWidth / num;
			float num4 = (float)pixelHeight / num;
			float x = (float)overrideColliderPixelWidth.Value / num2;
			float y = (float)overrideColliderPixelHeight.Value / num2;
			float x2 = (float)overrideColliderOffsetX.Value / num2;
			float y2 = (float)overrideColliderOffsetY.Value / num2;
			tk2dSpriteDefinition tk2dSpriteDefinition = ETGMod.Databases.Items.ProjectileCollection.inst.spriteDefinitions[ETGMod.GetAnySprite(overrideProjectileToCopyFrom ?? (PickupObjectDatabase.GetById(lightened ? 47 : 12) as Gun).DefaultModule.projectiles[0]).spriteId].CopyDefinitionFrom();
			tk2dSpriteDefinition.boundsDataCenter = new Vector3(num3 / 2f, num4 / 2f, 0f);
			tk2dSpriteDefinition.boundsDataExtents = new Vector3(num3, num4, 0f);
			tk2dSpriteDefinition.untrimmedBoundsDataCenter = new Vector3(num3 / 2f, num4 / 2f, 0f);
			tk2dSpriteDefinition.untrimmedBoundsDataExtents = new Vector3(num3, num4, 0f);
			tk2dSpriteDefinition.texelSize = new Vector2(0.0625f, 0.0625f);
			tk2dSpriteDefinition.position0 = new Vector3(0f, 0f, 0f);
			tk2dSpriteDefinition.position1 = new Vector3(0f + num3, 0f, 0f);
			tk2dSpriteDefinition.position2 = new Vector3(0f, 0f + num4, 0f);
			tk2dSpriteDefinition.position3 = new Vector3(0f + num3, 0f + num4, 0f);
			tk2dSpriteDefinition.colliderVertices[0].x = x2;
			tk2dSpriteDefinition.colliderVertices[0].y = y2;
			tk2dSpriteDefinition.colliderVertices[1].x = x;
			tk2dSpriteDefinition.colliderVertices[1].y = y;
			tk2dSpriteDefinition.name = name;
			ETGMod.Databases.Items.ProjectileCollection.inst.spriteDefinitions[id] = tk2dSpriteDefinition;
			return tk2dSpriteDefinition;
		}

		public static tk2dSpriteDefinition CopyDefinitionFrom(this tk2dSpriteDefinition other)
		{
			tk2dSpriteDefinition tk2dSpriteDefinition = new tk2dSpriteDefinition();
			Vector3 vector = default(Vector3);
			vector.x = other.boundsDataCenter.x;
			vector.y = other.boundsDataCenter.y;
			vector.z = other.boundsDataCenter.z;
			tk2dSpriteDefinition.boundsDataCenter = vector;
			vector = default(Vector3);
			vector.x = other.boundsDataExtents.x;
			vector.y = other.boundsDataExtents.y;
			vector.z = other.boundsDataExtents.z;
			tk2dSpriteDefinition.boundsDataExtents = vector;
			tk2dSpriteDefinition.colliderConvex = other.colliderConvex;
			tk2dSpriteDefinition.colliderSmoothSphereCollisions = other.colliderSmoothSphereCollisions;
			tk2dSpriteDefinition.colliderType = other.colliderType;
			tk2dSpriteDefinition.colliderVertices = other.colliderVertices;
			tk2dSpriteDefinition.collisionLayer = other.collisionLayer;
			tk2dSpriteDefinition.complexGeometry = other.complexGeometry;
			tk2dSpriteDefinition.extractRegion = other.extractRegion;
			tk2dSpriteDefinition.flipped = other.flipped;
			tk2dSpriteDefinition.indices = other.indices;
			tk2dSpriteDefinition.material = new Material(other.material);
			tk2dSpriteDefinition.materialId = other.materialId;
			tk2dSpriteDefinition.materialInst = new Material(other.materialInst);
			tk2dSpriteDefinition.metadata = other.metadata;
			tk2dSpriteDefinition.name = other.name;
			tk2dSpriteDefinition.normals = other.normals;
			tk2dSpriteDefinition.physicsEngine = other.physicsEngine;
			vector = default(Vector3);
			vector.x = other.position0.x;
			vector.y = other.position0.y;
			vector.z = other.position0.z;
			tk2dSpriteDefinition.position0 = vector;
			vector = default(Vector3);
			vector.x = other.position1.x;
			vector.y = other.position1.y;
			vector.z = other.position1.z;
			tk2dSpriteDefinition.position1 = vector;
			vector = default(Vector3);
			vector.x = other.position2.x;
			vector.y = other.position2.y;
			vector.z = other.position2.z;
			tk2dSpriteDefinition.position2 = vector;
			vector = default(Vector3);
			vector.x = other.position3.x;
			vector.y = other.position3.y;
			vector.z = other.position3.z;
			tk2dSpriteDefinition.position3 = vector;
			tk2dSpriteDefinition.regionH = other.regionH;
			tk2dSpriteDefinition.regionW = other.regionW;
			tk2dSpriteDefinition.regionX = other.regionX;
			tk2dSpriteDefinition.regionY = other.regionY;
			tk2dSpriteDefinition.tangents = other.tangents;
			Vector2 vector2 = default(Vector2);
			vector2.x = other.texelSize.x;
			vector2.y = other.texelSize.y;
			tk2dSpriteDefinition.texelSize = vector2;
			vector = default(Vector3);
			vector.x = other.untrimmedBoundsDataCenter.x;
			vector.y = other.untrimmedBoundsDataCenter.y;
			vector.z = other.untrimmedBoundsDataCenter.z;
			tk2dSpriteDefinition.untrimmedBoundsDataCenter = vector;
			vector = default(Vector3);
			vector.x = other.untrimmedBoundsDataExtents.x;
			vector.y = other.untrimmedBoundsDataExtents.y;
			vector.z = other.untrimmedBoundsDataExtents.z;
			tk2dSpriteDefinition.untrimmedBoundsDataExtents = vector;
			tk2dSpriteDefinition tk2dSpriteDefinition2 = tk2dSpriteDefinition;
			List<Vector2> list = new List<Vector2>();
			foreach (Vector2 vector3 in other.uvs)
			{
				List<Vector2> list2 = list;
				vector2 = default(Vector2);
				vector2.x = vector3.x;
				vector2.y = vector3.y;
				list2.Add(vector2);
			}
			tk2dSpriteDefinition2.uvs = list.ToArray();
			List<Vector3> list3 = new List<Vector3>();
			foreach (Vector3 vector4 in other.colliderVertices)
			{
				List<Vector3> list4 = list3;
				vector = default(Vector3);
				vector.x = vector4.x;
				vector.y = vector4.y;
				vector.z = vector4.z;
				list4.Add(vector);
			}
			tk2dSpriteDefinition2.colliderVertices = list3.ToArray();
			return tk2dSpriteDefinition2;
		}



		// SpecialItemPack.Toolbox
		// Token: 0x060003FC RID: 1020 RVA: 0x0002E505 File Offset: 0x0002C705
		// doesn't seem to work
		public static void PlaceItemInAmmonomiconAfterItemById(this PickupObject item, int id)
		{
			item.ForcedPositionInAmmonomicon = PickupObjectDatabase.GetById(id).ForcedPositionInAmmonomicon;
		}

		// Token: 0x060003EC RID: 1004 RVA: 0x0002D770 File Offset: 0x0002B970
		public static Gun GetGunById(this PickupObjectDatabase database, int id)
		{
			return PickupObjectDatabase.GetById(id) as Gun;
		}

		public static Gun GetGunById(int id)
		{
			return Toolbox.GetGunById((PickupObjectDatabase)null, id);
		}

		public static void SetupUnlockOnEncounter(this PickupObject self, string guid, DungeonPrerequisite.PrerequisiteOperation operation, int comparisonValue)
		{
			EncounterTrackable encounterTrackable = self.encounterTrackable;
			bool flag = encounterTrackable.prerequisites == null;
			if (flag)
			{
				encounterTrackable.prerequisites = new DungeonPrerequisite[1];
				encounterTrackable.prerequisites[0] = new DungeonPrerequisite
				{
					prerequisiteType = 0,
					prerequisiteOperation = operation,
					encounteredObjectGuid = guid,
					requiredNumberOfEncounters = comparisonValue
				};
			}
			else
			{
				encounterTrackable.prerequisites = encounterTrackable.prerequisites.Concat(new DungeonPrerequisite[]
				{
					new DungeonPrerequisite
					{
						prerequisiteType = 0,
						prerequisiteOperation = operation,
						encounteredObjectGuid = guid,
						requiredNumberOfEncounters = comparisonValue
					}
				}).ToArray<DungeonPrerequisite>();
			}
			EncounterDatabaseEntry entry = EncounterDatabase.GetEntry(encounterTrackable.EncounterGuid);
			bool flag2 = !string.IsNullOrEmpty(entry.ProxyEncounterGuid);
			if (flag2)
			{
				entry.ProxyEncounterGuid = "";
			}
			bool flag3 = entry.prerequisites == null;
			if (flag3)
			{
				entry.prerequisites = new DungeonPrerequisite[1];
				entry.prerequisites[0] = new DungeonPrerequisite
				{
					prerequisiteType = 0,
					prerequisiteOperation = operation,
					encounteredObjectGuid = guid,
					requiredNumberOfEncounters = comparisonValue
				};
			}
			else
			{
				entry.prerequisites = entry.prerequisites.Concat(new DungeonPrerequisite[]
				{
					new DungeonPrerequisite
					{
						prerequisiteType = 0,
						prerequisiteOperation = operation,
						encounteredObjectGuid = guid,
						requiredNumberOfEncounters = comparisonValue
					}
				}).ToArray<DungeonPrerequisite>();
			}
		}

		// Token: 0x060003F7 RID: 1015 RVA: 0x0002DEDC File Offset: 0x0002C0DC
		public static void SetupUnlockOnFlag(this PickupObject self, GungeonFlags flag, bool requiredFlagValue)
		{
			EncounterTrackable encounterTrackable = self.encounterTrackable;
			bool flag2 = encounterTrackable.prerequisites == null;
			if (flag2)
			{
				encounterTrackable.prerequisites = new DungeonPrerequisite[1];
				encounterTrackable.prerequisites[0] = new DungeonPrerequisite
				{
					prerequisiteType = (DungeonPrerequisite.PrerequisiteType)4,
					requireFlag = requiredFlagValue,
					saveFlagToCheck = flag
				};
			}
			else
			{
				encounterTrackable.prerequisites = encounterTrackable.prerequisites.Concat(new DungeonPrerequisite[]
				{
					new DungeonPrerequisite
					{
						prerequisiteType = (DungeonPrerequisite.PrerequisiteType)4,
						requireFlag = requiredFlagValue,
						saveFlagToCheck = flag
					}
				}).ToArray<DungeonPrerequisite>();
			}
			EncounterDatabaseEntry entry = EncounterDatabase.GetEntry(encounterTrackable.EncounterGuid);
			bool flag3 = !string.IsNullOrEmpty(entry.ProxyEncounterGuid);
			if (flag3)
			{
				entry.ProxyEncounterGuid = "";
			}
			bool flag4 = entry.prerequisites == null;
			if (flag4)
			{
				entry.prerequisites = new DungeonPrerequisite[1];
				entry.prerequisites[0] = new DungeonPrerequisite
				{
					prerequisiteType = (DungeonPrerequisite.PrerequisiteType)4,
					requireFlag = requiredFlagValue,
					saveFlagToCheck = flag
				};
			}
			else
			{
				entry.prerequisites = entry.prerequisites.Concat(new DungeonPrerequisite[]
				{
					new DungeonPrerequisite
					{
						prerequisiteType = (DungeonPrerequisite.PrerequisiteType)4,
						requireFlag = requiredFlagValue,
						saveFlagToCheck = flag
					}
				}).ToArray<DungeonPrerequisite>();
			}
		}

		public static bool BasicRandom(PlayerController player, float value, float divider)
		{
			return UnityEngine.Random.value > value - player.stats.GetStatValue(PlayerStats.StatType.Coolness) / divider;
		}
		public static void SetupUnlockOnStat(this PickupObject self, TrackedStats stat, DungeonPrerequisite.PrerequisiteOperation operation, int comparisonValue)
		{
			EncounterTrackable encounterTrackable = self.encounterTrackable;
			bool flag = encounterTrackable.prerequisites == null;
			if (flag)
			{
				encounterTrackable.prerequisites = new DungeonPrerequisite[1];
				encounterTrackable.prerequisites[0] = new DungeonPrerequisite
				{
					prerequisiteType = (DungeonPrerequisite.PrerequisiteType)1,
					prerequisiteOperation = operation,
					statToCheck = stat,
					comparisonValue = (float)comparisonValue
				};
			}
			else
			{
				encounterTrackable.prerequisites = encounterTrackable.prerequisites.Concat(new DungeonPrerequisite[]
				{
					new DungeonPrerequisite
					{
						prerequisiteType = (DungeonPrerequisite.PrerequisiteType)1,
						prerequisiteOperation = operation,
						statToCheck = stat,
						comparisonValue = (float)comparisonValue
					}
				}).ToArray<DungeonPrerequisite>();
			}
			EncounterDatabaseEntry entry = EncounterDatabase.GetEntry(encounterTrackable.EncounterGuid);
			bool flag2 = !string.IsNullOrEmpty(entry.ProxyEncounterGuid);
			if (flag2)
			{
				entry.ProxyEncounterGuid = "";
			}
			bool flag3 = entry.prerequisites == null;
			if (flag3)
			{
				entry.prerequisites = new DungeonPrerequisite[1];
				entry.prerequisites[0] = new DungeonPrerequisite
				{
					prerequisiteType = (DungeonPrerequisite.PrerequisiteType)1,
					prerequisiteOperation = operation,
					statToCheck = stat,
					comparisonValue = (float)comparisonValue
				};
			}
			else
			{
				entry.prerequisites = entry.prerequisites.Concat(new DungeonPrerequisite[]
				{
					new DungeonPrerequisite
					{
						prerequisiteType = (DungeonPrerequisite.PrerequisiteType)1,
						prerequisiteOperation = operation,
						statToCheck = stat,
						comparisonValue = (float)comparisonValue
					}
				}).ToArray<DungeonPrerequisite>();
			}
		}

		// by spapi
		public static void RemovePickupFromLootTables(this PickupObject po)
		{
			WeightedGameObject go1 = GameManager.Instance.RewardManager.GunsLootTable.defaultItemDrops.FindWeightedGameObjectInCollection(po);
			if (go1 != null)
			{
				GameManager.Instance.RewardManager.GunsLootTable.defaultItemDrops.elements.Remove(go1);
			}
			WeightedGameObject go2 = GameManager.Instance.RewardManager.ItemsLootTable.defaultItemDrops.FindWeightedGameObjectInCollection(po);
			if (go2 != null)
			{
				GameManager.Instance.RewardManager.ItemsLootTable.defaultItemDrops.elements.Remove(go2);
			}
		}

		public static WeightedGameObject FindWeightedGameObjectInCollection(this WeightedGameObjectCollection collection, PickupObject po)
		{
			WeightedGameObject go = collection.FindWeightedGameObjectInCollection(po.PickupObjectId);
			if (go == null)
			{
				go = collection.FindWeightedGameObjectInCollection(po.gameObject);
			}
			return go;
		}

		public static WeightedGameObject FindWeightedGameObjectInCollection(this WeightedGameObjectCollection collection, int id)
		{
			foreach (WeightedGameObject go in collection.elements)
			{
				if (go.pickupId == id)
				{
					return go;
				}
			}
			return null;
		}

		public static WeightedGameObject FindWeightedGameObjectInCollection(this WeightedGameObjectCollection collection, GameObject obj)
		{
			foreach (WeightedGameObject go in collection.elements)
			{
				if (go.gameObject == obj)
				{
					return go;
				}
			}
			return null;
		}

		public static void CopyAIBulletBank(UnityEngine.GameObject obj, AIBulletBank bank)
		{
			AIBulletBank newBank = obj.GetOrAddComponent<AIBulletBank>();
			newBank.Bullets = bank.Bullets;
			newBank.FixedPlayerPosition = bank.FixedPlayerPosition;
			newBank.OnProjectileCreated = bank.OnProjectileCreated;
			newBank.OverrideGun = bank.OverrideGun;
			newBank.rampTime = bank.rampTime;
			newBank.OnProjectileCreatedWithSource = bank.OnProjectileCreatedWithSource;
			newBank.rampBullets = bank.rampBullets;
			newBank.transforms = bank.transforms;
			newBank.useDefaultBulletIfMissing = bank.useDefaultBulletIfMissing;
			newBank.rampStartHeight = bank.rampStartHeight;
			newBank.SpecificRigidbodyException = bank.SpecificRigidbodyException;
			newBank.PlayShells = bank.PlayShells;
			newBank.PlayAudio = bank.PlayAudio;
			newBank.PlayVfx = bank.PlayVfx;
			newBank.CollidesWithEnemies = bank.CollidesWithEnemies;
			newBank.FixedPlayerRigidbodyLastPosition = bank.FixedPlayerRigidbodyLastPosition;
			newBank.ActorName = bank.ActorName;
			newBank.TimeScale = bank.TimeScale;
			newBank.SuppressPlayerVelocityAveraging = bank.SuppressPlayerVelocityAveraging;
			newBank.FixedPlayerRigidbody = bank.FixedPlayerRigidbody;
			/*newBank.spriteAnimator = bank.spriteAnimator;
            newBank.sprite = bank.sprite;
            newBank.specRigidbody = bank.specRigidbody;
            newBank.encounterTrackable = bank.encounterTrackable;
            newBank.majorBreakable = bank.majorBreakable;
            newBank.gameActor = bank.gameActor;
            newBank.aiAnimator = bank.aiAnimator;
            newBank.healthHaver = bank.healthHaver;
            newBank.aiActor = bank.aiActor;
            newBank.renderer = bank.renderer;*/
		}

		public static ExplosionData CopyExplosionData(this ExplosionData other)
		{
			ExplosionData explosionData = new ExplosionData();
			explosionData.useDefaultExplosion = other.useDefaultExplosion;
			explosionData.doDamage = other.doDamage;
			explosionData.forceUseThisRadius = other.forceUseThisRadius;
			explosionData.damageRadius = other.damageRadius;
			explosionData.damageToPlayer = other.damageToPlayer;
			explosionData.damage = other.damage;
			explosionData.breakSecretWalls = other.breakSecretWalls;
			explosionData.secretWallsRadius = other.secretWallsRadius;
			explosionData.forcePreventSecretWallDamage = other.forcePreventSecretWallDamage;
			explosionData.doDestroyProjectiles = other.doDestroyProjectiles;
			explosionData.doForce = other.doForce;
			explosionData.pushRadius = other.pushRadius;
			explosionData.force = other.force;
			explosionData.debrisForce = other.debrisForce;
			explosionData.preventPlayerForce = other.preventPlayerForce;
			explosionData.explosionDelay = other.explosionDelay;
			explosionData.usesComprehensiveDelay = other.usesComprehensiveDelay;
			explosionData.comprehensiveDelay = other.comprehensiveDelay;
			explosionData.effect = other.effect;
			explosionData.doScreenShake = other.doScreenShake;
			ScreenShakeSettings screenShakeSettings = new ScreenShakeSettings();
			screenShakeSettings.magnitude = other.ss.magnitude;
			screenShakeSettings.speed = other.ss.speed;
			screenShakeSettings.time = other.ss.time;
			screenShakeSettings.falloff = other.ss.falloff;
			Vector2 direction = default(Vector2);
			direction.x = other.ss.direction.x;
			direction.y = other.ss.direction.y;
			screenShakeSettings.direction = direction;
			screenShakeSettings.vibrationType = other.ss.vibrationType;
			screenShakeSettings.simpleVibrationTime = other.ss.simpleVibrationTime;
			screenShakeSettings.simpleVibrationStrength = other.ss.simpleVibrationStrength;
			explosionData.ss = screenShakeSettings;
			explosionData.doStickyFriction = other.doStickyFriction;
			explosionData.doExplosionRing = other.doExplosionRing;
			explosionData.isFreezeExplosion = other.isFreezeExplosion;
			explosionData.freezeRadius = other.freezeRadius;
			explosionData.freezeEffect = other.freezeEffect;
			explosionData.playDefaultSFX = other.playDefaultSFX;
			explosionData.IsChandelierExplosion = other.IsChandelierExplosion;
			explosionData.rotateEffectToNormal = other.rotateEffectToNormal;
			explosionData.ignoreList = other.ignoreList;
			explosionData.overrideRangeIndicatorEffect = other.overrideRangeIndicatorEffect;
			return explosionData;
		}

		// Token: 0x0400016F RID: 367
		public static Dictionary<Type, long> EnumsGiven = new Dictionary<Type, long>();

		// Token: 0x04000170 RID: 368
		public static GoopDefinition DefaultWaterGoop;

		// Token: 0x04000171 RID: 369
		public static GoopDefinition DefaultFireGoop;

		// Token: 0x04000172 RID: 370
		public static GoopDefinition DefaultPoisonGoop;

		// Token: 0x04000173 RID: 371
		public static GoopDefinition DefaultCharmGoop;

		// Token: 0x04000174 RID: 372
		public static GoopDefinition DefaultBlobulonGoop;

		// Token: 0x04000175 RID: 373
		public static GoopDefinition DefaultPoopulonGoop;

		// Token: 0x04000176 RID: 374
		public static GoopDefinition DefaultCheeseGoop;

		public class CustomBulletScriptSelector : BulletScriptSelector
		{
			public Type bulletType;

			public CustomBulletScriptSelector(Type _bulletType)
			{
				bulletType = _bulletType;
				this.scriptTypeName = bulletType.AssemblyQualifiedName;
			}

			public new Bullet CreateInstance()
			{
				if (bulletType == null)
				{
					ETGModConsole.Log("Unknown type! " + this.scriptTypeName);
					return null;
				}
				return (Bullet)Activator.CreateInstance(bulletType);
			}

			public new bool IsNull
			{
				get
				{
					return string.IsNullOrEmpty(this.scriptTypeName) || this.scriptTypeName == "null";
				}
			}

			public new BulletScriptSelector Clone()
			{
				return new CustomBulletScriptSelector(bulletType);
			}
		}

		// Token: 0x04000177 RID: 375
		public static GameObject SprenSpunPrefab;

		// Token: 0x020000FC RID: 252
		public class SprenSpunBehaviour : BraveBehaviour
		{
			// Token: 0x06000636 RID: 1590 RVA: 0x0003F970 File Offset: 0x0003DB70
			private void Update()
			{
				string text = this.GunChangeMoreAnimation;
				bool flag = this.isBackwards;
				if (flag)
				{
					text = this.BackchangeAnimation;
				}
				bool flag2 = base.spriteAnimator != null && !base.spriteAnimator.IsPlaying(text);
				if (flag2)
				{
					base.spriteAnimator.Play(text);
				}
			}

			// Token: 0x06000637 RID: 1591 RVA: 0x0003F9CB File Offset: 0x0003DBCB
			public void ChangeDirection(Toolbox.SprenSpunBehaviour.SprenSpunRotateType direction)
			{
				this.isBackwards = (direction == Toolbox.SprenSpunBehaviour.SprenSpunRotateType.BACKWARDS);
			}

			// Token: 0x0400047A RID: 1146
			public string GunChangeMoreAnimation;

			// Token: 0x0400047B RID: 1147
			public string BackchangeAnimation;

			// Token: 0x0400047C RID: 1148
			public bool isBackwards = false;

			// Token: 0x02000123 RID: 291
			public enum SprenSpunRotateType
			{
				// Token: 0x04000556 RID: 1366
				NORMAL,
				// Token: 0x04000557 RID: 1367
				BACKWARDS
			}
		}
	}
}

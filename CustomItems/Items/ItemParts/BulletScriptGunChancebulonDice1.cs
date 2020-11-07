using Brave.BulletScript;
using System.Collections;
using UnityEngine;

namespace GlaurungItems.Items
{
	public class BulletScriptGunChancebulonDice1 : Script
	{
		//public float aimDirection { get; private set; }
		private float aimDirection;
		public float AimDirectionGet()
		{
			return this.aimDirection;
		}
		public void AimDirectionSet(float value)
		{
			this.aimDirection = value;
		}

		// Token: 0x060004AD RID: 1197 RVA: 0x0001558C File Offset: 0x0001378C
		protected override IEnumerator Top()
		{
			this.EndOnBlank = true;
			this.FireSquare();
			AimDirectionSet(BulletScriptGun.playerGunCurrentAngle);// this.AimDirection;
			yield return this.Wait(15);
			float distanceToTarget = (this.BulletManager.PlayerPosition() - this.Position).magnitude;
			if (distanceToTarget > 4.5f)
			{
				AimDirectionSet(this.GetAimDirection(1f, 10f));
			}
			yield break;
		}

		// Token: 0x060004AE RID: 1198 RVA: 0x000155A8 File Offset: 0x000137A8
		private void FireSquare()
		{
			Vector2 vector = new Vector2(2.2f, 0f).Rotate(45f);
			Vector2 vector2 = new Vector2(2.2f, 0f).Rotate(135f);
			Vector2 vector3 = new Vector2(2.2f, 0f).Rotate(225f);
			Vector2 vector4 = new Vector2(2.2f, 0f).Rotate(-45f);
			this.FireExpandingLine(vector, vector2, 5);
			this.FireExpandingLine(vector2, vector3, 5);
			this.FireExpandingLine(vector3, vector4, 5);
			this.FireExpandingLine(vector4, vector, 5);
			base.Fire(new BulletScriptGunChancebulonDice1.ExpandingBullet(this, new Vector2(0f, 0f), new int?(0)));
			base.Fire(new BulletScriptGunChancebulonDice1.ExpandingBullet(this, new Vector2(0f, 0f), new int?(1)));
			base.Fire(new BulletScriptGunChancebulonDice1.ExpandingBullet(this, new Vector2(0f, 0f), new int?(2)));
			base.Fire(new BulletScriptGunChancebulonDice1.ExpandingBullet(this, new Vector2(0f, 0f), new int?(3)));
			base.Fire(new BulletScriptGunChancebulonDice1.ExpandingBullet(this, new Vector2(0f, 0f), new int?(4)));
			base.Fire(new BulletScriptGunChancebulonDice1.ExpandingBullet(this, new Vector2(0f, 0f), new int?(5)));
		}

		private void FireExpandingLine(Vector2 start, Vector2 end, int numBullets)
		{
			for (int i = 0; i < numBullets; i++)
			{
				base.Fire(new BulletScriptGunChancebulonDice1.ExpandingBullet(this, Vector2.Lerp(start, end, (float)i / ((float)numBullets - 1f)), null));
			}
		}

		public const float Radius = 2f;

		public const int GrowTime = 15;

		public const float RotationSpeed = 180f;

		public const float BulletSpeed = 10f;

		public class ExpandingBullet : Bullet
		{
			public ExpandingBullet(BulletScriptGunChancebulonDice1 parent, Vector2 offset, int? numeralIndex = null) : base(null, false, false, false)
			{
				this.m_parent = parent;
				this.m_offset = offset;
				this.m_numeralIndex = numeralIndex;
			}

			protected override IEnumerator Top()
			{
				this.ManualControl = true;
				Vector2 centerPosition = this.Position;
				for (int i = 0; i < 15; i++)
				{
					this.UpdateVelocity();
					centerPosition += this.Velocity / 60f;
					Vector2 actualOffset = Vector2.Lerp(Vector2.zero, this.m_offset, (float)i / 14f);
					actualOffset = actualOffset.Rotate(3f * (float)i);
					this.Position = centerPosition + actualOffset;
					yield return this.Wait(1);
				}
				this.Direction = this.m_parent.AimDirectionGet();
				this.Speed = 10f;
				for (int j = 0; j < 300; j++)
				{
					this.UpdateVelocity();
					centerPosition += this.Velocity / 60f;
					if (this.m_numeralIndex != null && j % 13 == 0 && j != 0)
					{
						this.m_currentNumeral = (this.m_currentNumeral + 1) % 6;
						switch (this.m_currentNumeral)
						{
							case 0:
								{
									int? numeralIndex = this.m_numeralIndex;
									if (numeralIndex != null && numeralIndex.GetValueOrDefault() < 3)
									{
										this.m_offset = new Vector2(-0.7f, 0.7f);
									}
									else
									{
										this.m_offset = new Vector2(0.7f, -0.7f);
									}
									break;
								}
							case 1:
								{
									int? numeralIndex2 = this.m_numeralIndex;
									if (numeralIndex2 != null && numeralIndex2.GetValueOrDefault() < 2)
									{
										this.m_offset = new Vector2(-0.7f, 0.7f);
									}
									else
									{
										int? numeralIndex3 = this.m_numeralIndex;
										if (numeralIndex3 != null && numeralIndex3.GetValueOrDefault() < 4)
										{
											this.m_offset = new Vector2(0f, 0f);
										}
										else
										{
											this.m_offset = new Vector2(0.7f, -0.7f);
										}
									}
									break;
								}
							case 2:
								{
									int? numeralIndex4 = this.m_numeralIndex;
									if (numeralIndex4 != null && numeralIndex4.GetValueOrDefault() < 1)
									{
										this.m_offset = new Vector2(-0.6f, -0.6f);
									}
									else
									{
										int? numeralIndex5 = this.m_numeralIndex;
										if (numeralIndex5 != null && numeralIndex5.GetValueOrDefault() < 2)
										{
											this.m_offset = new Vector2(-0.6f, 0.6f);
										}
										else
										{
											int? numeralIndex6 = this.m_numeralIndex;
											if (numeralIndex6 != null && numeralIndex6.GetValueOrDefault() < 3)
											{
												this.m_offset = new Vector2(0f, 0f);
											}
											else
											{
												int? numeralIndex7 = this.m_numeralIndex;
												if (numeralIndex7 != null && numeralIndex7.GetValueOrDefault() < 4)
												{
													this.m_offset = new Vector2(0.6f, -0.6f);
												}
												else
												{
													this.m_offset = new Vector2(0.6f, 0.6f);
												}
											}
										}
									}
									break;
								}
							case 3:
								{
									int? numeralIndex8 = this.m_numeralIndex;
									if (numeralIndex8 != null && numeralIndex8.GetValueOrDefault() < 2)
									{
										this.m_offset = new Vector2(-0.6f, -0.6f);
									}
									else
									{
										int? numeralIndex9 = this.m_numeralIndex;
										if (numeralIndex9 != null && numeralIndex9.GetValueOrDefault() < 3)
										{
											this.m_offset = new Vector2(-0.6f, 0.6f);
										}
										else
										{
											int? numeralIndex10 = this.m_numeralIndex;
											if (numeralIndex10 != null && numeralIndex10.GetValueOrDefault() < 4)
											{
												this.m_offset = new Vector2(0.6f, -0.6f);
											}
											else
											{
												this.m_offset = new Vector2(0.6f, 0.6f);
											}
										}
									}
									break;
								}
							case 4:
								{
									int? numeralIndex11 = this.m_numeralIndex;
									if (numeralIndex11 != null && numeralIndex11.GetValueOrDefault() < 1)
									{
										this.m_offset = new Vector2(-0.6f, -0.6f);
									}
									else
									{
										int? numeralIndex12 = this.m_numeralIndex;
										if (numeralIndex12 != null && numeralIndex12.GetValueOrDefault() < 2)
										{
											this.m_offset = new Vector2(-0.6f, 0f);
										}
										else
										{
											int? numeralIndex13 = this.m_numeralIndex;
											if (numeralIndex13 != null && numeralIndex13.GetValueOrDefault() < 3)
											{
												this.m_offset = new Vector2(-0.6f, 0.6f);
											}
											else
											{
												int? numeralIndex14 = this.m_numeralIndex;
												if (numeralIndex14 != null && numeralIndex14.GetValueOrDefault() < 4)
												{
													this.m_offset = new Vector2(0.6f, -0.6f);
												}
												else
												{
													int? numeralIndex15 = this.m_numeralIndex;
													if (numeralIndex15 != null && numeralIndex15.GetValueOrDefault() < 5)
													{
														this.m_offset = new Vector2(0.6f, 0f);
													}
													else
													{
														this.m_offset = new Vector2(0.6f, 0.6f);
													}
												}
											}
										}
									}
									break;
								}
							case 5:
								this.m_offset = new Vector2(0f, 0f);
								break;
						}
					}
					this.Position = centerPosition + this.m_offset.Rotate(3f * (float)(15 + j));
					yield return this.Wait(1);
				}
				this.Vanish(false);
				yield break;
			}

			private const int SingleFaceShowTime = 13;

			private BulletScriptGunChancebulonDice1 m_parent;

			private Vector2 m_offset;

			private int? m_numeralIndex;

			private int m_currentNumeral;
		}
	}
}

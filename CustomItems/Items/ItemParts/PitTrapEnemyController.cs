using System;
using Dungeonator;
using UnityEngine;

namespace GlaurungItems.Items
{
	public class PitTrapEnemyController : BasicTrapController
	{
		protected override void OnDestroy()
		{
			base.OnDestroy();
		}

		public override GameObject InstantiateObject(RoomHandler targetRoom, IntVector2 loc, bool deferConfiguration = false)
		{
			IntVector2 intVector = loc + targetRoom.area.basePosition;
			for (int i = intVector.x; i < intVector.x + this.placeableWidth; i++)
			{
				for (int j = intVector.y; j < intVector.y + this.placeableHeight; j++)
				{
					CellData cellData = GameManager.Instance.Dungeon.data.cellData[i][j];
					cellData.type = CellType.PIT;
					cellData.fallingPrevented = true;
				}
			}
			return base.InstantiateObject(targetRoom, loc, deferConfiguration);
		}

		protected override void BeginState(BasicTrapController.State newState)
		{
			if (newState == BasicTrapController.State.Active)
			{
				for (int i = this.m_cachedPosition.x; i < this.m_cachedPosition.x + this.placeableWidth; i++)
				{
					for (int j = this.m_cachedPosition.y; j < this.m_cachedPosition.y + this.placeableHeight; j++)
					{
						GameManager.Instance.Dungeon.data.cellData[i][j].fallingPrevented = false;
					}
				}
				if (base.specRigidbody)
				{
					base.specRigidbody.enabled = false;
				}
			}
			else if (newState == BasicTrapController.State.Resetting)
			{
				for (int k = this.m_cachedPosition.x; k < this.m_cachedPosition.x + this.placeableWidth; k++)
				{
					for (int l = this.m_cachedPosition.y; l < this.m_cachedPosition.y + this.placeableHeight; l++)
					{
						GameManager.Instance.Dungeon.data.cellData[k][l].fallingPrevented = true;
					}
				}
				if (base.specRigidbody)
				{
					base.specRigidbody.enabled = true;
				}
			}
			base.BeginState(newState);
		}
	}
}

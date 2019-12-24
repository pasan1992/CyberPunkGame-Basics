using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{

	[ActionCategory("GameAssets/Scripts/Character/Agents/FSM")]
	public class AgentFSMReturnToIdle : HumanoidAgentEvents
	{

		// Code that runs on entering the state.
		public override void OnEnter()
		{
			if(m_movingAgent.hosterWeapon())
			{
				Finish();
			}
			
		}

		public override void OnUpdate()
		{
			if(m_movingAgent.hosterWeapon())
			{
				Finish();
			}
		}

	}

}

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{

	[ActionCategory("Agent")]
	public class AgentFSMIterationState : AgentFSMSate
	{

		// Code that runs on entering the state.
		IteractionStage m_iteractionStage;

		public override void OnEnter()
		{
		}

		public override void OnPreprocess()
		{
			base.OnPreprocess();
			WaypointRutine rutine = gameObject.GetComponent<WaypointRutine>();
			m_iteractionStage = new IteractionStage(m_movingAgent,m_navmeshAgent,rutine.m_wayPoints.ToArray());
			Fsm.HandleFixedUpdate = true;
		}

		public override void OnFixedUpdate()
		{
			if(m_movingAgent.IsFunctional() && !m_movingAgent.isDisabled() && m_iteractionStage != null )
			{
				m_iteractionStage.updateStage();
			}
		}

		public override void OnExit()
		{	
			m_iteractionStage.endStage();
		}

	}

}

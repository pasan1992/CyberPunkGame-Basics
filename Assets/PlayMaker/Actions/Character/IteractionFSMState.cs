using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{

	[ActionCategory(ActionCategory.Character)]
	public class IteractionFSMState : AgentFSMSate
	{

		// Code that runs on entering the state.
		IteractionStage m_iteractionStage;

		public override void OnEnter()
		{
		}

		public override void OnPreprocess()
		{
			base.OnPreprocess();
			if(gameObject == null)
			{
				return;
			}
			WaypointRutine rutine = gameObject.GetComponent<WaypointRutine>();
			m_iteractionStage = new IteractionStage(m_movingAgent,m_navmeshAgent,rutine.m_wayPoints.ToArray());
			Fsm.HandleFixedUpdate = true;
			Debug.Log("iteraction here initalize");
		}

		public override void OnFixedUpdate()
		{
			Debug.Log("iteraction here update");
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

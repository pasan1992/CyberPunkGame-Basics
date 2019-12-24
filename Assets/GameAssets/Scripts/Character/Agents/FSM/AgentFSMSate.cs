using UnityEngine;
using UnityEngine.AI;

namespace HutongGames.PlayMaker.Actions
{

	[ActionCategory("Agent")]
	public class AgentFSMSate : FsmStateAction
	{		
		public FsmOwnerDefault agentGameObject;
		protected GameObject gameObject;
		//public GameEnums.CharacterStage AgentStateType;
		//private ICharacterBehaviorState m_behaviorStage;
		protected NavMeshAgent m_navmeshAgent;
		protected ICyberAgent m_movingAgent;
		//public WaypointRutine m_rutine;
		//public FsmGameObject target;
		//private GameObject m_currentTarget;

		public override void OnEnter()
		{
			// switch(AgentStateType)
			// {
			// 	case GameEnums.CharacterStage.Combat:
			// 		((CombatStage)m_behaviorStage).setTargets(target.Value.GetComponent<HumanoidMovingAgent>());
			// 	break;
			// 	case GameEnums.CharacterStage.Iteraction:
			// 	break;
			// }

			// m_behaviorStage.initalizeStage();
		}

		public override void OnPreprocess()
		{
			Fsm.HandleFixedUpdate = true;
			gameObject = Fsm.GetOwnerDefaultTarget(agentGameObject);
			if(gameObject == null)
			{
				return;
			}
			//Fsm.HandleLateUpdate = false;
			initalizeAgentData();
		}

		private void initalizeAgentData()
		{
			if(m_movingAgent == null)
			{
				// Get Required Components
				
					m_movingAgent = gameObject.GetComponent<HumanoidMovingAgent>();
					m_navmeshAgent = gameObject.GetComponent<NavMeshAgent>();
				// m_rutine = go.GetComponent<WaypointRutine>();
				//initalizeStageBehavior();
			}
		}

		protected virtual void initalizeStageBehavior()
		{
			// switch(AgentStateType)
			// {
			// 	case GameEnums.CharacterStage.Combat:
			// 		m_behaviorStage = new CombatStage(m_movingAgent, null,m_navmeshAgent);
			// 	break;
			// 	case GameEnums.CharacterStage.Iteraction:
			// 		m_behaviorStage = new IteractionStage(m_movingAgent,m_navmeshAgent,m_rutine.m_wayPoints.ToArray());
			// 		Debug.Log("Enter2");
			// 	break;
			// }
		}

		// Code that runs every frame.

		// private void updateTarget()
		// {
		// 	if(m_currentTarget == null || m_currentTarget != target.Value)
		// 	{
		// 		((CombatStage)m_behaviorStage).setTargets(target.Value.GetComponent<HumanoidMovingAgent>());
		// 	}
		// }

		// Code that runs when exiting the state.
		public override void OnExit()
		{
			// processOnExit();
		}

		private void processOnExit()
		{
			// m_behaviorStage.endStage();

			// switch(AgentStateType)
			// {
			// 	case GameEnums.CharacterStage.Combat:
			// 		if(m_movingAgent.isHidden())
			// 		{
			// 			m_movingAgent.toggleHide();
			// 		}

			// 		if(m_movingAgent.isAimed())
			// 		{
			// 			m_movingAgent.stopAiming();
			// 		}

			// 		((HumanoidMovingAgent)m_movingAgent).hosterWeapon();

			// 	break;
			// 	case GameEnums.CharacterStage.Iteraction:

			// 	break;
			// }			
		}
		public override void OnFixedUpdate()
		{
			Debug.Log("Fixed Update");
		}

		// Perform custom error checking here.
		public override string ErrorCheck()
		{
			return null;
		}


	}

}

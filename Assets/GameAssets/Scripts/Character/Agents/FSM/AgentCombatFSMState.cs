using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{

	[ActionCategory("Agent")]
	public class AgentCombatFSMState : AgentFSMSate
	{
        public FsmGameObject m_target;
        public FsmGameObject Target { get => m_target; set => m_target = value; }
		private BasicCombatStage m_behaviorStage; 
		private GameObject m_currentTarget;

        // Code that runs on entering the state.
		public override void OnPreprocess()
		{
			base.OnPreprocess();
			m_behaviorStage = new CombatStage(m_movingAgent, null,m_navmeshAgent);
		}
		
		public override void OnEnter()
		{
			updateTarget();
			m_behaviorStage.initalizeStage();
		}

		protected override void initalizeStageBehavior()
		{
			m_behaviorStage = new CombatStage(m_movingAgent, null,m_navmeshAgent);
		}

		// Code that runs every frame.
		public override void OnUpdate()
		{
		}

		public override void OnFixedUpdate()
		{	
			m_behaviorStage.updateStage();
			updateTarget();
		}

		// Code that runs when exiting the state.
		public override void OnExit()
		{	
			m_behaviorStage.endStage();

			if(m_movingAgent.isHidden())
			{
				m_movingAgent.toggleHide();
			}

			if(m_movingAgent.isAimed())
			{
				m_movingAgent.stopAiming();
			}
		}

		private void updateTarget()
		{
			if(m_currentTarget == null || (m_currentTarget !=null && m_currentTarget != m_target.Value))
			{
				m_currentTarget = m_target.Value;
				m_behaviorStage.setTargets(m_target.Value.GetComponent<ICyberAgent>());
			}
		}

	}

}

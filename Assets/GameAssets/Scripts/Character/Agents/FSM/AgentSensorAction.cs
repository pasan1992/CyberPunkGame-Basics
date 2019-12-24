using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{

	[ActionCategory("Agent")]
	public class AgentSensorAction : FsmStateAction
	{
		public FsmOwnerDefault agentGameObject;
		private ICyberAgent m_movingAgent;
		private HumanoidAgentBasicVisualSensor m_sensor;
		public FsmEvent allClearEvent;
		public FsmEvent onEnemyDetectionEvent;
		public FsmGameObject target;

		public override void OnEnter()
		{
			
		}

		public override void OnFixedUpdate()
		{
			m_sensor.UpdateSensor();
		}

		public override void OnPreprocess()
		{
			Fsm.HandleFixedUpdate = true;
			Fsm.HandleLateUpdate = false;
			initalizeAgentData();
		}

		public void initalizeAgentData()
		{
			GameObject go = Fsm.GetOwnerDefaultTarget(agentGameObject);
			m_movingAgent = go.GetComponent<HumanoidMovingAgent>();
			m_sensor =  new HumanoidAgentBasicVisualSensor(m_movingAgent);
			m_sensor.setOnEnemyDetectionEvent(onEnemyDetection);
        	m_sensor.setOnAllClear(allClearCallback);
		}

		public void allClearCallback()
		{
			Fsm.Event(allClearEvent);
		}

		public void onEnemyDetection(ICyberAgent opponent)
    	{
			target.Value = opponent.getGameObject();
			Fsm.Event(onEnemyDetectionEvent);
		}
	}

}

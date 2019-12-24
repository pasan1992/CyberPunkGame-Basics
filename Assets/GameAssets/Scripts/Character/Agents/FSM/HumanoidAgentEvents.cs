using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{

	[ActionCategory("Agent")]
	public class HumanoidAgentEvents : FsmStateAction
	{
		public FsmOwnerDefault agentGameObject;
		protected HumanoidMovingAgent m_movingAgent;

		public FsmEvent OnEquip;
		public FsmEvent OnUnequip;
		public FsmEvent OnReloadEnd;
		public FsmEvent OnDamaged;
		public FsmEvent OnDestoryCallback;
		public FsmEvent OnInteractionDone;
		public FsmEvent OnThrowItem;


		public override void OnEnter()
		{
		}

		public override void OnPreprocess()
		{
			Fsm.HandleFixedUpdate = true;
			GameObject go = Fsm.GetOwnerDefaultTarget(agentGameObject);
			m_movingAgent = go.GetComponent<HumanoidMovingAgent>();
			m_movingAgent.setBasicCallbacks(onEquip,onUnequip,onReloadEnd,onDamaged,onDestoryCallback,onInteractionDone,onThrowItem);
		}

		public void onEquip()
		{
			Fsm.Event(OnEquip);
		}

		public void onUnequip()
		{
			Fsm.Event(OnUnequip);
		}

		public void onReloadEnd()
		{
			Fsm.Event(OnReloadEnd);
		}

		public void onDamaged()
		{
			Fsm.Event(OnDamaged);
		}

		public void onDestoryCallback()
		{	
			Debug.Log("Event Fired");
			Fsm.Event(OnDestoryCallback);
		}

		public void onInteractionDone()
		{
			Fsm.Event(OnInteractionDone);
		}

		public void onThrowItem()
		{
			Fsm.Event(OnThrowItem);
		}
	}

}

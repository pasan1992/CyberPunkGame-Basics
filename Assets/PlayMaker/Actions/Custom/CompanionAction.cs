using UnityEngine;
using UnityEngine.AI;

namespace HutongGames.PlayMaker.Actions
{

	[ActionCategory("Custom")]
	public class CompanionAction : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(CompanionController))]
		private CompanionController companionAgent;
		public FsmVector3 moveVector;

		public FsmOwnerDefault gameObject;
		// Code that runs on entering the state.
		public override void OnEnter()
		{
			GameObject go = Fsm.GetOwnerDefaultTarget(gameObject);
			if(companionAgent == null)
			{
				companionAgent = go.GetComponent<CompanionController>();
			}
			companionAgent.MoveToPosition(moveVector.Value);
			Finish();
		}
		// Perform custom error checking here.
		public override string ErrorCheck()
		{
			// Return an error string or null if no error.
			
			return null;
		}


	}

}

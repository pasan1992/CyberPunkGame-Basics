using UnityEngine;
using System.Collections;

namespace RootMotion.FinalIK {

	public partial class IKSolverVR: IKSolver {

		// TODO Rename file to IKSolverVRFootstep

		[System.Serializable]
		public class Footstep {

			public float stepSpeed = 3f;
			public Vector3 characterSpaceOffset;

			public Vector3 position { get; private set; }
			public Quaternion rotation { get; private set; }
			public Quaternion stepToRootRot { get; private set; }
			public bool isStepping { get { return stepProgress < 1f; }}

			public float stepProgress { get; private set; }
			public Vector3 stepFrom { get; private set; }
			public Vector3 stepTo;
			private Quaternion stepFromRot = Quaternion.identity;
			private Quaternion stepToRot = Quaternion.identity;
			private Quaternion footRelativeToRoot = Quaternion.identity;

			public Footstep (Quaternion rootRotation, Vector3 footPosition, Quaternion footRotation, Vector3 characterSpaceOffset) {
				this.characterSpaceOffset = characterSpaceOffset;
				Reset(rootRotation, footPosition, footRotation);
			}

			public void Reset(Quaternion rootRotation, Vector3 footPosition, Quaternion footRotation) {
				position = footPosition;
				rotation = footRotation;
				stepFrom = position;
				stepTo = position;
				stepFromRot = rotation;
				stepToRot = rotation;
				stepToRootRot = rootRotation;
				stepProgress = 1f;
				footRelativeToRoot = Quaternion.Inverse(rootRotation) * rotation;
			}

			public void StepTo(Vector3 p, Quaternion rootRotation) {
				stepFrom = position;
				stepTo = p;
				stepFromRot = rotation;
				stepToRootRot = rootRotation;
				stepToRot = rootRotation * footRelativeToRoot;
				stepProgress = 0f;
			}

			public void Update(InterpolationMode interpolation) {
				if (!isStepping) return;

				stepProgress = Mathf.MoveTowards(stepProgress, 1f, Time.deltaTime * stepSpeed);
				float stepProgressSmooth = RootMotion.Interp.Float(stepProgress, interpolation);

				position = Vector3.Lerp(stepFrom, stepTo, stepProgressSmooth);
				rotation = Quaternion.Lerp(stepFromRot, stepToRot, stepProgressSmooth);
			}
		}
	}
}

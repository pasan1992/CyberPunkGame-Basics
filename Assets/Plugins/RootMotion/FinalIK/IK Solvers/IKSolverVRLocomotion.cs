using UnityEngine;
using System.Collections;

namespace RootMotion.FinalIK {

	/// <summary>
	/// Hybrid %IK solver designed for mapping a character to a VR headset and 2 hand controllers 
	/// </summary>
	public partial class IKSolverVR: IKSolver {

		[System.Serializable]
		public class Locomotion {

			[Tooltip("Used for blending in/out of procedural locomotion.")]
			/// <summary>
			/// Used for blending in/out of procedural locomotion.
			/// </summary>
			[Range(0f, 1f)] public float weight = 1f;

			[Tooltip("Tries to maintain this distance between the legs.")]
			/// <summary>
			/// Tries to maintain this distance between the legs.
			/// </summary>
			public float footDistance = 0.3f;

			[Tooltip("Makes a step only if step target position is at least this far from the current footstep or the foot does not reach the current footstep anymore or footstep angle is past the 'Angle Threshold'.")]
			/// <summary>
			/// Makes a step only if step target position is at least this far from the current footstep or the foot does not reach the current footstep anymore or footstep angle is past the 'Angle Threshold'.
			/// </summary>
			public float stepThreshold = 0.4f;

			[Tooltip("Makes a step only if step target position is at least 'Step Threshold' far from the current footstep or the foot does not reach the current footstep anymore or footstep angle is past this value.")]
			/// <summary>
			/// Makes a step only if step target position is at least 'Step Threshold' far from the current footstep or the foot does not reach the current footstep anymore or footstep angle is past this value.
			/// </summary>
			public float angleThreshold = 60f;

			[Tooltip("Multiplies angle of the center of mass - center of pressure vector. Larger value makes the character step sooner if losing balance.")]
			/// <summary>
			/// Multiplies angle of the center of mass - center of pressure vector. Larger value makes the character step sooner if losing balance.
			/// </summary>
			public float comAngleMlp = 1f;

			[Tooltip("Maximum magnitude of head/hand target velocity used in prediction.")]
			/// <summary>
			/// Maximum magnitude of head/hand target velocity used in prediction.
			/// </summary>
			public float maxVelocity = 0.4f;

			[Tooltip("The amount of head/hand target velocity prediction.")]
			/// <summary>
			/// The amount of head/hand target velocity prediction.
			/// </summary>
			public float velocityFactor = 0.4f;

			[Tooltip("How much can a leg be extended before it is forced to step to another position? 1 means fully stretched.")]
			/// <summary>
			/// How much can a leg be extended before it is forced to step to another position? 1 means fully stretched.
			/// </summary>
			[Range(0.9f, 1f)]
			public float maxLegStretch = 1f;

			[Tooltip("The speed of lerping the root of the character towards the horizontal mid-point of the footsteps.")]
			/// <summary>
			/// The speed of lerping the root of the character towards the horizontal mid-point of the footsteps.
			/// </summary>
			public float rootSpeed = 20f;

			[Tooltip("The speed of steps.")]
			/// <summary>
			/// The speed of steps
			/// </summary>
			public float stepSpeed = 3f;

			[Tooltip("The height of the foot by normalized step progress (0 - 1).")]
			/// <summary>
			/// The height of the foot by normalized step progress (0 - 1).
			/// </summary>
			public AnimationCurve stepHeight;

			[Tooltip("The height offset of the heel by normalized step progress (0 - 1).")]
			/// <summary>
			/// The height offset of the heel by normalized step progress (0 - 1).
			/// </summary>
			public AnimationCurve heelHeight;

			[Tooltip("Interpolation mode of the step.")]
			/// <summary>
			/// Interpolation mode of the step.
			/// </summary>
			public InterpolationMode stepInterpolation = InterpolationMode.InOutSine;

			[Tooltip("Offset for the approximated center of mass.")]
			/// <summary>
			/// Offset for the approximated center of mass.
			/// </summary>
			public Vector3 offset;

			[HideInInspector] public bool blockingEnabled;
			[HideInInspector] public LayerMask blockingLayers;
			[HideInInspector] public float raycastRadius = 0.2f;
			[HideInInspector] public float raycastHeight = 0.2f;

			/// <summary>
			/// Gets the approximated center of mass.
			/// </summary>
			public Vector3 centerOfMass { get; private set; }

			private Footstep[] footsteps = new Footstep[0];
			private Vector3 lastComPosition;
			private Vector3 comVelocity;
			private int leftFootIndex;
			private int rightFootIndex;

			public void Initiate(Vector3[] positions, Quaternion[] rotations, bool hasToes) {
				leftFootIndex = hasToes? 17: 16;
				rightFootIndex = hasToes? 21: 20;

				footsteps = new Footstep[2] { 
					new Footstep(rotations[0], positions[leftFootIndex], rotations[leftFootIndex], footDistance * Vector3.left), 
					new Footstep(rotations[0], positions[rightFootIndex], rotations[rightFootIndex], footDistance * Vector3.right) 
					};
			}

			public void Reset(Vector3[] positions, Quaternion[] rotations) {
				lastComPosition = Vector3.Lerp(positions[1], positions[5], 0.25f) + rotations[0] * offset;
				comVelocity = Vector3.zero;

				footsteps[0].Reset(rotations[0], positions[leftFootIndex], rotations[leftFootIndex]);
				footsteps[1].Reset(rotations[0], positions[rightFootIndex], rotations[rightFootIndex]);
			}
					
			public void Solve(VirtualBone rootBone, Spine spine, Leg leftLeg, Leg rightLeg, Arm leftArm, Arm rightArm, out Vector3 leftFootPosition, out Vector3 rightFootPosition, out Quaternion leftFootRotation, out Quaternion rightFootRotation, out float leftFootOffset, out float rightFootOffset, out float leftHeelOffset, out float rightHeelOffset) {
				if (weight <= 0f) {
					leftFootPosition = Vector3.zero;
					rightFootPosition = Vector3.zero;
					leftFootRotation = Quaternion.identity;
					rightFootRotation = Quaternion.identity;
					leftFootOffset = 0f;
					rightFootOffset = 0f;
					leftHeelOffset = 0f;
					rightHeelOffset = 0f;
					return;
				}

				Vector3 leftThighPosition = spine.pelvis.solverPosition + spine.pelvis.solverRotation * leftLeg.thighRelativeToPelvis;
				Vector3 rightThighPosition = spine.pelvis.solverPosition + spine.pelvis.solverRotation * rightLeg.thighRelativeToPelvis;

				footsteps[0].characterSpaceOffset = footDistance * Vector3.left;
				footsteps[1].characterSpaceOffset = footDistance * Vector3.right;

				Vector3 forward = spine.faceDirection;
				forward.y = 0f;
				Quaternion forwardRotation = Quaternion.LookRotation(forward);

				//centerOfMass = Vector3.Lerp(spine.pelvis.solverPosition, spine.head.solverPosition, 0.25f) + rootBone.solverRotation * offset;

				float pelvisMass = 1f;
				float headMass = 1f;
				float armMass = 0.2f;
				float totalMass = pelvisMass + headMass + 2f * armMass;

				centerOfMass = Vector3.zero;
				centerOfMass += spine.pelvis.solverPosition * pelvisMass;
				centerOfMass += spine.head.solverPosition * headMass;
				centerOfMass += leftArm.position * armMass;
				centerOfMass += rightArm.position * armMass;
				centerOfMass /= totalMass;

				centerOfMass += rootBone.solverRotation * offset;

				comVelocity = (centerOfMass - lastComPosition) / Time.deltaTime;
				lastComPosition = centerOfMass;
				comVelocity = Vector3.ClampMagnitude (comVelocity, maxVelocity) * velocityFactor;
				Vector3 centerOfMassV = centerOfMass + comVelocity;

				Vector3 pelvisPositionGroundLevel = new Vector3(spine.pelvis.solverPosition.x, rootBone.solverPosition.y, spine.pelvis.solverPosition.z);
				Vector3 centerOfMassVGroundLevel = new Vector3(centerOfMassV.x, rootBone.solverPosition.y, centerOfMassV.z);
				Vector3 centerOfPressure = Vector3.Lerp(footsteps[0].position, footsteps[1].position, 0.5f);

				Vector3 comDir = centerOfMassV - centerOfPressure;
				float comAngle = Vector3.Angle(comDir, rootBone.solverRotation * Vector3.up) * comAngleMlp;

				// Update stepTo while stepping
				for (int i = 0; i < footsteps.Length; i++) {
					if (footsteps[i].isStepping) {
						Vector3 stepTo = centerOfMassVGroundLevel +  rootBone.solverRotation * footsteps[i].characterSpaceOffset;
						if (!StepBlocked(footsteps[i].stepFrom, stepTo, rootBone.solverPosition)) {
							footsteps[i].stepTo = Vector3.Lerp(footsteps[i].stepTo, stepTo, Time.deltaTime * 8f);
						}
					}
				}

				// Triggering new footsteps
				if (CanStep()) {
					int stepLegIndex = -1;
					float bestValue = -Mathf.Infinity;

					for (int i = 0; i < footsteps.Length; i++) {
						if (!footsteps[i].isStepping) {
							Vector3 stepTo = centerOfMassVGroundLevel +  rootBone.solverRotation * footsteps[i].characterSpaceOffset;

							float legLength = i == 0? leftLeg.mag: rightLeg.mag;
							Vector3 thighPos = i == 0? leftThighPosition: rightThighPosition;

							float thighDistance = Vector3.Distance(footsteps[i].position, thighPos);

							bool lengthStep = false;
							if (thighDistance >= legLength * maxLegStretch) {// * 0.95f) {
								stepTo = pelvisPositionGroundLevel + rootBone.solverRotation * footsteps[i].characterSpaceOffset;
								lengthStep = true;
							}

							bool collision = false;
							for (int n = 0; n < footsteps.Length; n++) {
								if (n != i && !lengthStep) {
									if (Vector3.Distance(footsteps[i].position, footsteps[n].position) < 0.25f && (footsteps[i].position - stepTo).sqrMagnitude < (footsteps[n].position - stepTo).sqrMagnitude) {
									} else collision = GetLineSphereCollision(footsteps[i].position, stepTo, footsteps[n].position, 0.25f);
									if (collision) break;
								}
							}

							float angle = Quaternion.Angle(forwardRotation, footsteps[i].stepToRootRot);

							if (!collision || angle > angleThreshold) {
								float stepDistance = Vector3.Distance(footsteps[i].position, stepTo);
								float sT = Mathf.Lerp(stepThreshold, stepThreshold * 0.1f, comAngle * 0.015f);
								if (lengthStep) sT *= 0.5f;
								if (i == 0) sT *= 0.9f;

								if (!StepBlocked(footsteps[i].position, stepTo, rootBone.solverPosition)) {
									if (stepDistance > sT || angle > angleThreshold) {
										float value = 0f;

										value -= stepDistance;

										if (value > bestValue) {
											stepLegIndex = i;
											bestValue = value;
										}
									}
								}
							}
						}

					}

					if (stepLegIndex != -1) {
						Vector3 stepTo = centerOfMassVGroundLevel + rootBone.solverRotation * footsteps[stepLegIndex].characterSpaceOffset;
						footsteps[stepLegIndex].stepSpeed = UnityEngine.Random.Range(stepSpeed, stepSpeed * 1.5f);
						footsteps[stepLegIndex].StepTo(stepTo, forwardRotation);
					}
				}

				foreach (Footstep f in footsteps) f.Update(stepInterpolation);

				leftFootPosition = footsteps[0].position;
				rightFootPosition = footsteps[1].position;
				leftFootPosition.y = leftLeg.lastBone.readPosition.y;
				rightFootPosition.y = rightLeg.lastBone.readPosition.y;

				leftFootOffset = stepHeight.Evaluate(footsteps[0].stepProgress);
				rightFootOffset = stepHeight.Evaluate(footsteps[1].stepProgress);

				leftHeelOffset = heelHeight.Evaluate(footsteps[0].stepProgress);
				rightHeelOffset = heelHeight.Evaluate(footsteps[1].stepProgress);

				leftFootRotation = footsteps[0].rotation;
				rightFootRotation = footsteps[1].rotation;
			}

			private bool StepBlocked(Vector3 fromPosition, Vector3 toPosition, Vector3 rootPosition) {
				if (blockingLayers == -1 || !blockingEnabled) return false;

				Vector3 origin = fromPosition;
				origin.y = rootPosition.y + raycastHeight + raycastRadius;

				Vector3 direction = toPosition - origin;
				direction.y = 0f;

				RaycastHit hit;

				if (raycastRadius <= 0f) {
					return Physics.Raycast(origin, direction, out hit, direction.magnitude, blockingLayers);
				} else {
					return Physics.SphereCast(origin, raycastRadius, direction, out hit, direction.magnitude, blockingLayers);
				}
			}

			private bool CanStep() {
				foreach (Footstep f in footsteps) if (f.isStepping && f.stepProgress < 0.8f) return false;

				return true;
			}

			private static bool GetLineSphereCollision(Vector3 lineStart, Vector3 lineEnd, Vector3 sphereCenter, float sphereRadius) {
				Vector3 line = lineEnd - lineStart;
				Vector3 toSphere = sphereCenter - lineStart;
				float distToSphereCenter = toSphere.magnitude;
				float d = distToSphereCenter - sphereRadius;

				if (d > line.magnitude) return false;

				Quaternion q = Quaternion.LookRotation(line, toSphere);

				Vector3 toSphereRotated = Quaternion.Inverse(q) * toSphere;

				if (toSphereRotated.z < 0f) {
					return d < 0f; 
				} 

				return toSphereRotated.y - sphereRadius < 0f;
			}
		}
	}
}
using UnityEngine;
using System.Collections;
using System;
using RootMotion;

namespace RootMotion.FinalIK {
	
	/// <summary>
	/// Hybrid %IK solver designed for mapping a character to a VR headset and 2 hand controllers 
	/// </summary>
	public partial class IKSolverVR: IKSolver {
		
		/// <summary>
		/// Spine solver for IKSolverVR.
		/// </summary>
		[System.Serializable]
		public class Spine: BodyPart {

			[Tooltip("The head target.")]
			/// <summary>
			/// The head target.
			/// </summary>
			public Transform headTarget;

			[Tooltip("The pelvis target, useful with seated rigs.")]
			/// <summary>
			/// The pelvis target, useful with seated rigs.
			/// </summary>
			public Transform pelvisTarget;

			[Tooltip("Positional weight of the head target.")]
			/// <summary>
			/// Positional weight of the head target.
			/// </summary>
			[Range(0f, 1f)] public float positionWeight = 1f;

			[Tooltip("Rotational weight of the head target.")]
			/// <summary>
			/// Rotational weight of the head target.
			/// </summary>
			[Range(0f, 1f)] public float rotationWeight = 1f;

			[Tooltip("Positional weight of the pelvis target.")]
			/// <summary>
			/// Positional weight of the pelvis target.
			/// </summary>
			[Range(0f, 1f)] public float pelvisPositionWeight;

			[Tooltip("Determines how much the body will follow the position of the head.")]
			/// <summary>
			/// Determines how much the body will follow the position of the head.
			/// </summary>
			[Range(0f, 1f)] public float bodyPosStiffness = 0.55f;

			[Tooltip("Determines how much the body will follow the rotation of the head.")]
			/// <summary>
			/// Determines how much the body will follow the rotation of the head.
			/// </summary>
			[Range(0f, 1f)] public float bodyRotStiffness = 0.1f;

			[Tooltip("Determines how much the chest will rotate to the rotation of the head.")]
			/// <summary>
			/// Determines how much the chest will rotate to the rotation of the head.
			/// </summary>
			[Range(0f, 1f)] public float chestRotationWeight = 0.2f;

			[Tooltip("Clamps chest rotation.")]
			/// <summary>
			/// Clamps chest rotation.
			/// </summary>
			[Range(0f, 1f)] public float chestClampWeight = 0.5f;

			[Tooltip("Clamps head rotation.")]
			/// <summary>
			/// Clamps head rotation.
			/// </summary>
			[Range(0f, 1f)] public float headClampWeight = 0.6f;

			[Tooltip("How much will the pelvis maintain it's animated position?")]
			/// <summary>
			/// How much will the pelvis maintain it's animated position?
			/// </summary>
			[Range(0f, 1f)] public float maintainPelvisPosition = 0.2f;

			[Tooltip("Will automatically rotate the root of the character if the head target has turned past this angle.")]
			/// <summary>
			/// Will automatically rotate the root of the character if the head target has turned past this angle.
			/// </summary>
			[Range(0f, 180f)] public float maxRootAngle = 25f;

			/// <summary>
			/// Target position of the head. Will be overwritten if target is assigned.
			/// </summary>
			public Vector3 IKPositionHead { get; private set; }

			/// <summary>
			/// Target rotation of the head. Will be overwritten if target is assigned.
			/// </summary>
			/// <value>The IK rotation head.</value>
			public Quaternion IKRotationHead { get; private set; }

			/// <summary>
			/// Target position of the pelvis. Will be overwritten if target is assigned.
			/// </summary>
			public Vector3 IKPositionPelvis { get; private set; }

			/// <summary>
			/// Position offset of the pelvis. Will be applied on top of pelvis target position and reset to Vector3.zero after each update.
			/// </summary>
			[HideInInspector] public Vector3 pelvisPositionOffset;

			/// <summary>
			/// Position offset of the chest. Will be reset to Vector3.zero after each update.
			/// </summary>
			[HideInInspector] public Vector3 chestPositionOffset;

			/// <summary>
			/// Position offset of the head. Will be applied on top of head target position and reset to Vector3.zero after each update.
			/// </summary>
			[HideInInspector] public Vector3 headPositionOffset;

			/// <summary>
			/// Rotation offset of the pelvis. Will be reset to Quaternion.identity after each update.
			/// </summary>
			[HideInInspector] public Quaternion pelvisRotationOffset = Quaternion.identity;

			/// <summary>
			/// Rotation offset of the chest. Will be reset to Quaternion.identity after each update.
			/// </summary>
			[HideInInspector] public Quaternion chestRotationOffset = Quaternion.identity;

			/// <summary>
			/// Rotation offset of the head. Will be applied on top of head target rotation and reset to Quaternion.identity after each update.
			/// </summary>
			[HideInInspector] public Quaternion headRotationOffset = Quaternion.identity;

			public VirtualBone pelvis { get { return bones[pelvisIndex]; }}
			public VirtualBone firstSpineBone { get { return bones[spineIndex]; }}
			public VirtualBone chest { get { return bones[chestIndex]; }}
			private VirtualBone neck { get { return bones[neckIndex]; }}
			public VirtualBone head { get { return bones[headIndex]; }}

			[HideInInspector] public Vector3 faceDirection;
			public Quaternion anchorRotation { get; private set; }

			private Vector3 headPosition;
			private Quaternion headRotation = Quaternion.identity;
			private Quaternion anchorRelativeToHead = Quaternion.identity;
			private Quaternion pelvisRelativeRotation = Quaternion.identity;
			private Quaternion chestRelativeRotation = Quaternion.identity;
			private Vector3 headDeltaPosition;
			private Quaternion pelvisDeltaRotation = Quaternion.identity;
			private Quaternion chestTargetRotation = Quaternion.identity;
			private const int pelvisIndex = 0, spineIndex = 1, chestIndex = 2, neckIndex = 3;
			private int headIndex;
			private float length;
			private bool hasNeck;
			private float headHeight;

			protected override void OnRead(Vector3[] positions, Quaternion[] rotations, bool hasNeck, bool hasShoulders, bool hasToes, int rootIndex, int index) {
				Vector3 pelvisPos = positions[index];
				Quaternion pelvisRot = rotations[index];
				Vector3 spinePos = positions[index + 1];
				Quaternion spineRot = rotations[index + 1];
				Vector3 chestPos = positions[index + 2];
				Quaternion chestRot = rotations[index + 2];
				Vector3 neckPos = positions[index + 3];
				Quaternion neckRot = rotations[index + 3];
				Vector3 headPos = positions[index + 4];
				Quaternion headRot = rotations[index + 4];

				if (!initiated) {
					this.hasNeck = hasNeck;
					headHeight = headPos.y - positions[0].y;

					bones = new VirtualBone[hasNeck? 5: 4];
					headIndex = hasNeck? 4: 3;

					bones[0] = new VirtualBone(pelvisPos, pelvisRot);
					bones[1] = new VirtualBone(spinePos, spineRot);
					bones[2] = new VirtualBone(chestPos, chestRot);
					if (hasNeck) bones[3] = new VirtualBone(neckPos, neckRot);
					bones[headIndex] = new VirtualBone(headPos, headRot);

					pelvisRotationOffset = Quaternion.identity;
					chestRotationOffset = Quaternion.identity;
					headRotationOffset = Quaternion.identity;

					anchorRelativeToHead = Quaternion.Inverse(headRot) * rotations[0];
					
					// Forward and up axes
					pelvisRelativeRotation = Quaternion.Inverse(headRot) * pelvisRot;
					chestRelativeRotation = Quaternion.Inverse(headRot) * chestRot;

					faceDirection = rotations[0] * Vector3.forward;

					IKPositionHead = headPos;
					IKRotationHead = headRot;
					IKPositionPelvis = pelvisPos;
				}

				bones[0].Read(pelvisPos, pelvisRot);
				bones[1].Read(spinePos, spineRot);
				bones[2].Read(chestPos, chestRot);
				if (hasNeck) bones[3].Read(neckPos, neckRot);
				bones[headIndex].Read(headPos, headRot);
			}

			public override void PreSolve() {
				if (headTarget != null) {
					IKPositionHead = headTarget.position;
					IKRotationHead = headTarget.rotation;
				}

				if (pelvisTarget != null) {
					IKPositionPelvis = pelvisTarget.position;
				}

				headPosition = V3Tools.Lerp(head.solverPosition, IKPositionHead, positionWeight);
				headRotation = QuaTools.Lerp(head.solverRotation, IKRotationHead, rotationWeight);
			}

			public override void ApplyOffsets() {
				headPosition += headPositionOffset;
				headPosition.y = Math.Max(rootPosition.y + 0.8f, headPosition.y);

				headRotation = headRotationOffset * headRotation;

				headDeltaPosition = headPosition - head.solverPosition;
				pelvisDeltaRotation = QuaTools.FromToRotation(pelvis.solverRotation, headRotation * pelvisRelativeRotation);

				anchorRotation = headRotation * anchorRelativeToHead;
			}

			private void CalculateChestTargetRotation(Arm[] arms) {
				chestTargetRotation = headRotation * chestRelativeRotation;

				// Use hands to adjust c
				AdjustChestByHands(ref chestTargetRotation, arms);
				AdjustChestByOffset(ref chestTargetRotation);

				faceDirection = Vector3.Cross(anchorRotation * Vector3.right, Vector3.up) + anchorRotation * Vector3.forward;
			}

			public void Solve(VirtualBone rootBone, Leg[] legs, Arm[] arms) {
				CalculateChestTargetRotation(arms);

				// Root rotation
				if (maxRootAngle < 180f) {
					Vector3 faceDirLocal = Quaternion.Inverse(rootBone.solverRotation) * faceDirection;
					float angle = Mathf.Atan2(faceDirLocal.x, faceDirLocal.z) * Mathf.Rad2Deg;

					float rotation = 0f;
					float maxAngle = 25f;

					if (angle > maxAngle) {
						rotation = angle - maxAngle;
					}
					if (angle < -maxAngle) {
						rotation = angle + maxAngle;
					}

					rootBone.solverRotation = Quaternion.AngleAxis(rotation, Vector3.up) * rootBone.solverRotation;		
				}

				Vector3 animatedPelvisPos = pelvis.solverPosition;

				// Translate pelvis to make the head's position & rotation match with the head target
				TranslatePelvis(legs, headDeltaPosition, pelvisDeltaRotation, 1f);

				// Solve a FABRIK pass to squash/stretch the spine
				VirtualBone.SolveFABRIK(bones, Vector3.Lerp(pelvis.solverPosition, animatedPelvisPos, maintainPelvisPosition) + pelvisPositionOffset - chestPositionOffset, headPosition - chestPositionOffset, 1f, 1f, 1, mag);

				// Bend the spine to look towards chest target rotation
				Bend(bones, pelvisIndex, chestIndex, chestTargetRotation, chestClampWeight, false, chestRotationWeight);

				InverseTranslateToHead(legs, false, false, Vector3.zero, 1f);

				VirtualBone.SolveFABRIK(bones, Vector3.Lerp(pelvis.solverPosition, animatedPelvisPos, maintainPelvisPosition) + pelvisPositionOffset - chestPositionOffset, headPosition - chestPositionOffset, 1f, 1f, 1, mag);

				Bend(bones, neckIndex, headIndex, headRotation, headClampWeight, true, 1f);

				SolvePelvis ();
			}

			private void SolvePelvis() {
				// Pelvis target
				if (pelvisPositionWeight > 0f) {
					Quaternion headSolverRotation = head.solverRotation;
					
					Vector3 delta = ((IKPositionPelvis + pelvisPositionOffset) - pelvis.solverPosition) * pelvisPositionWeight;
					foreach (VirtualBone bone in bones) bone.solverPosition += delta;
					
					Vector3 bendNormal = anchorRotation * Vector3.right;
					
					if (hasNeck) {
						VirtualBone.SolveTrigonometric(bones, 0, 1, bones.Length - 1, headPosition, bendNormal, pelvisPositionWeight * 0.6f);
						VirtualBone.SolveTrigonometric(bones, 1, 2, bones.Length - 1, headPosition, bendNormal, pelvisPositionWeight * 0.6f);
						VirtualBone.SolveTrigonometric(bones, 2, 3, bones.Length - 1, headPosition, bendNormal, pelvisPositionWeight * 1f);
					} else {
						VirtualBone.SolveTrigonometric(bones, 0, 1, bones.Length - 1, headPosition, bendNormal, pelvisPositionWeight * 0.75f);
						VirtualBone.SolveTrigonometric(bones, 1, 2, bones.Length - 1, headPosition, bendNormal, pelvisPositionWeight * 1f);
					}
					
					head.solverRotation = headSolverRotation;
				}
			}

			public override void Write(ref Vector3[] solvedPositions, ref Quaternion[] solvedRotations) {
				// Pelvis
				solvedPositions[index] = bones[0].solverPosition;
				solvedRotations[index] = bones[0].solverRotation;

				// Spine
				solvedRotations[index + 1] = bones[1].solverRotation;

				// Chest
				solvedRotations[index + 2] = bones[2].solverRotation;

				// Neck
				if (hasNeck) solvedRotations[index + 3] = bones[3].solverRotation;

				// Head
				solvedRotations[index + 4] = bones[headIndex].solverRotation;
			}

			public override void ResetOffsets() {
				// Reset offsets to zero
				pelvisPositionOffset = Vector3.zero;
				chestPositionOffset = Vector3.zero;
				headPositionOffset = Vector3.zero;
				pelvisRotationOffset = Quaternion.identity;
				chestRotationOffset = Quaternion.identity;
				headRotationOffset = Quaternion.identity;
			}

			private void AdjustChestByOffset(ref Quaternion chestTargetRotation) {
				chestTargetRotation = chestRotationOffset * chestTargetRotation;
			}

			private void AdjustChestByHands(ref Quaternion chestTargetRotation, Arm[] arms) {
				Quaternion h = Quaternion.Inverse(anchorRotation);

				Vector3 pLeft = h * (arms[0].position - headPosition);
				Vector3 pRight = h * (arms[1].position - headPosition);

				Vector3 c = Vector3.forward;
				c.x += pLeft.x * Mathf.Abs(pLeft.x);
				c.x += pLeft.z * Mathf.Abs(pLeft.z);
				c.x += pRight.x * Mathf.Abs(pRight.x);
				c.x -= pRight.z * Mathf.Abs(pRight.z);
				c.x *= 5f;

				Quaternion q = Quaternion.FromToRotation(Vector3.forward, c);
				chestTargetRotation = q * chestTargetRotation;

				Vector3 t = Vector3.up;
				t.x += pLeft.y;
				t.x -= pRight.y;
				t.x *= 0.5f;

				q = Quaternion.FromToRotation(Vector3.up, anchorRotation * t);
				chestTargetRotation = q * chestTargetRotation;
			}

			// Move the pelvis so that the head would remain fixed to the anchor
			public void InverseTranslateToHead(Leg[] legs, bool limited, bool useCurrentLegMag, Vector3 offset, float w) {
				Vector3 p = pelvis.solverPosition + (headPosition + offset - head.solverPosition) * w * (1f - pelvisPositionWeight);
				MovePosition( limited? LimitPelvisPosition(legs, p, useCurrentLegMag): p);
			}

			// Move and rotate the pelvis
			private void TranslatePelvis(Leg[] legs, Vector3 deltaPosition, Quaternion deltaRotation, float w) {
				// Rotation
				Vector3 p = head.solverPosition;

				deltaRotation = QuaTools.ClampRotation(deltaRotation, chestClampWeight, 2);
				Quaternion f = w >= 1f? pelvisRotationOffset: Quaternion.Slerp(Quaternion.identity, pelvisRotationOffset, w);

				VirtualBone.RotateAroundPoint(bones, 0, pelvis.solverPosition, f * Quaternion.Slerp(Quaternion.identity, deltaRotation, w * bodyRotStiffness));

				deltaPosition -= head.solverPosition - p;

				// Position
				// Move the body back when head is moving down
				Vector3 m = anchorRotation * Vector3.forward;
				m.y = 0f;
				float backOffset = deltaPosition.y * 0.35f * headHeight;
				deltaPosition += m * backOffset;

				/*
				if (backOffset < 0f) {
					foreach (Leg leg in legs) leg.heelPositionOffset += Vector3.up * backOffset * backOffset; // TODO Ignoring root rotation
				}
				*/

				MovePosition (LimitPelvisPosition(legs, pelvis.solverPosition + deltaPosition * w * bodyPosStiffness, false));
			}

			// Limit the position of the pelvis so that the feet/toes would remain fixed
			private Vector3 LimitPelvisPosition(Leg[] legs, Vector3 pelvisPosition, bool useCurrentLegMag, int it = 2) {
				// Cache leg current mag
				if (useCurrentLegMag) {
					foreach (Leg leg in legs) {
						leg.currentMag = Vector3.Distance(leg.thigh.solverPosition, leg.lastBone.solverPosition);
					}
				}

				// Solve a 3-point constraint
				for (int i = 0; i < it; i++) {
					foreach (Leg leg in legs) {
						Vector3 delta = pelvisPosition - pelvis.solverPosition;
						Vector3 wantedThighPos = leg.thigh.solverPosition + delta;
						Vector3 toWantedThighPos = wantedThighPos - leg.position;
						float maxMag = useCurrentLegMag? leg.currentMag: leg.mag;
						Vector3 limitedThighPos = leg.position + Vector3.ClampMagnitude(toWantedThighPos, maxMag);
						pelvisPosition += limitedThighPos - wantedThighPos;

						// TODO rotate pelvis to accommodate, rotate the spine back then
					}
				}
				
				return pelvisPosition;
			}

			// Bending the spine to the head effector
			private void Bend(VirtualBone[] bones, int firstIndex, int lastIndex, Quaternion targetRotation, float clampWeight, bool uniformWeight, float w) {
				if (w <= 0f) return;
				if (bones.Length == 0) return;
				int bonesCount = (lastIndex + 1) - firstIndex;
				if (bonesCount < 1) return;

				Quaternion r = QuaTools.FromToRotation(bones[lastIndex].solverRotation, targetRotation);
				r = QuaTools.ClampRotation(r, clampWeight, 2);

				float step = uniformWeight? 1f / bonesCount: 0f;
				
				for (int i = firstIndex; i < lastIndex + 1; i++) {
					if (!uniformWeight) step = Mathf.Clamp(((i - firstIndex) + 1) / bonesCount, 0, 1f);
					VirtualBone.RotateAroundPoint(bones, i, bones[i].solverPosition, Quaternion.Slerp(Quaternion.identity, r, step * w));
				}
			}
		}
	}
}
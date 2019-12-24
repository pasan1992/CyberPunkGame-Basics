using UnityEngine;
using System.Collections;

namespace RootMotion.FinalIK {

	/// <summary>
	/// Relaxes the twist rotation of this Transform relative to a parent and a child Transform, using their initial rotations as the most relaxed pose.
	/// </summary>
	public class TwistRelaxer : MonoBehaviour {
		
		[Tooltip("The weight of relaxing the twist of this Transform")] 
		[Range(0f, 1f)] public float weight = 1f;

		[Tooltip("If 0.5, this Transform will be twisted half way from parent to child. If 1, the twist angle will be locked to the child and will rotate with along with it.")]
		[Range(0f, 1f)] public float parentChildCrossfade = 0.5f;

		[Tooltip("The parent Transform, does not need to be the actual transform.parent.")]
		public Transform parent;

		[Tooltip("The child Transform, does not need to be the direct child, you can skip bones in the hierarchy.")]
		public Transform child;

		[Tooltip("The local axis of this Transform that it will be twisted around (the axis pointing towards the parent).")]
		public Vector3 twistAxis = Vector3.right;

		[Tooltip("Another axis, orthogonal to twistAxis.")]
		public Vector3 axis = Vector3.forward;

		/// <summary>
		/// Rotate this Transform to relax it's twist angle relative to the "parent" and "child" Transforms.
		/// </summary>
		public void Relax() {
			if (weight <= 0f) return; // Nothing to do here
			
			// Find the world space relaxed axes of the parent and child
			Vector3 relaxedAxisParent = parent.rotation * axisRelativeToParentDefault;
			Vector3 relaxedAxisChild = child.rotation * axisRelativeToChildDefault;
			
			// Cross-fade between the parent and child
			Vector3 relaxedAxis = Vector3.Slerp(relaxedAxisParent, relaxedAxisChild, parentChildCrossfade);
			
			// Convert relaxedAxis to (axis, twistAxis) space so we could calculate the twist angle
			Quaternion r = Quaternion.LookRotation(transform.rotation * axis, transform.rotation * twistAxis);
			relaxedAxis = Quaternion.Inverse(r) * relaxedAxis;
			
			// Calculate the angle by which we need to rotate this Transform around the twist axis.
			float angle = Mathf.Atan2(relaxedAxis.x, relaxedAxis.z) * Mathf.Rad2Deg;
			
			// Store the rotation of the child so it would not change with twisting this Transform
			Quaternion childRotation = child.rotation;
			
			// Twist the bone
			transform.rotation = Quaternion.AngleAxis(angle * weight, transform.rotation * twistAxis) * transform.rotation;
			
			// Revert the rotation of the child
			child.rotation = childRotation;
		}

		private Vector3 axisRelativeToParentDefault, axisRelativeToChildDefault;
		
		void Start() {
			// Axis in world space
			Vector3 axisWorld = transform.rotation * axis;

			// Store the axis in worldspace relative to the rotations of the parent and child
			axisRelativeToParentDefault = Quaternion.Inverse(parent.rotation) * axisWorld;
			axisRelativeToChildDefault = Quaternion.Inverse(child.rotation) * axisWorld;
		}

		void LateUpdate() {
			Relax();
		}
	}
}

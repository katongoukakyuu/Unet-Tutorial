using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[NetworkSettings(channel = 0, sendInterval = 0.033f)]
public class Player_SyncRotation : NetworkBehaviour {
	
	[SyncVar] private Quaternion syncPlayerRotation;
	[SyncVar] private Quaternion syncCamRotation;
	[SerializeField] private Transform playerTransform;
	[SerializeField] private Transform camTransform;
	[SerializeField] private float lerpRate = 15f;

	private Quaternion lastPlayerRot;
	private Quaternion lastCamRot;
	private float threshold = 5f;

	void Update() {
		LerpRotations();
	}

	// Update is called once per frame
	void FixedUpdate () {
		TransformRotations();
	}

	void LerpRotations() {
		if(!isLocalPlayer) {
			playerTransform.rotation = Quaternion.Lerp (playerTransform.rotation, syncPlayerRotation, Time.deltaTime * lerpRate);
			camTransform.rotation = Quaternion.Lerp (camTransform.rotation, syncCamRotation, Time.deltaTime * lerpRate);
		}
	}

	[Command]
	void CmdProvideRotationsToServer(Quaternion playerRot, Quaternion camRot) {
		syncPlayerRotation = playerRot;
		syncCamRotation = camRot;
	}

	[ClientCallback]
	void TransformRotations() {
		if(isLocalPlayer && 
		   (Quaternion.Angle (playerTransform.rotation, lastPlayerRot) > threshold || Quaternion.Angle (camTransform.rotation, lastCamRot) > threshold)) {
			CmdProvideRotationsToServer(playerTransform.rotation, camTransform.rotation);
			lastPlayerRot = playerTransform.rotation;
			lastCamRot = camTransform.rotation;
		}
	}
}

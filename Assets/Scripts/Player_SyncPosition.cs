using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

[NetworkSettings(channel = 0, sendInterval = 0.033f)]
public class Player_SyncPosition : NetworkBehaviour {

	[SyncVar] private Vector3 syncPos;
	[SerializeField] Transform myTransform;
	[SerializeField] float lerpRate = 15f;
	
	private Vector3 lastPos;
	private float threshold = 0.5f;

	private NetworkClient nClient;
	private float latency;

	private Text latencyText;

	void Start() {
		nClient = GameObject.Find ("Network Manager").GetComponent<NetworkManager>().client;
		latencyText = GameObject.Find ("Latency Text").GetComponent<Text>();
	}

	void Update() {
		LerpPosition();
		ShowLatency();
	}

	// Update is called once per frame
	void FixedUpdate () {
		TransmitPosition();
	}

	void LerpPosition() {
		if(!isLocalPlayer) {
			myTransform.position = Vector3.Lerp (myTransform.position, syncPos, Time.deltaTime * lerpRate);
		}
	}

	[Command]
	void CmdProvidePositionToServer(Vector3 pos) {
		syncPos = pos;
	}

	[ClientCallback]
	void TransmitPosition() {
		if(isLocalPlayer && Vector3.Distance(myTransform.position, lastPos) > threshold) {
			CmdProvidePositionToServer(myTransform.position);
			lastPos = myTransform.position;
		}
	}

	void ShowLatency() {
		if(isLocalPlayer) {
			latency = nClient.GetRTT();
			latencyText.text = latency.ToString();
		}
	}
}

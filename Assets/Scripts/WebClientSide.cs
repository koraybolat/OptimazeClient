using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.WebSockets;
using System.Threading.Tasks;

public class WebClientSide : MonoBehaviour
{

	Uri u = new Uri("ws://localhost:1234/");
	ClientWebSocket cws = null;
	int replacerInt;
    
    void Start(){}

	public async void Connect() {
		cws = new ClientWebSocket();
		try {
			await cws.ConnectAsync(u, CancellationToken.None);
			if (cws.State == WebSocketState.Open) Debug.Log("Web Server Connected");

			sendData(StaticValueScript.dimensionSize);
			sendData(StaticValueScript.problemNumber);

			int expandedDim = Convert.ToInt32(Math.Pow(2,StaticValueScript.dimensionSize));

			await organizeInitialCubes(expandedDim);
			await organizeAnswerCubes(expandedDim);
		}
		catch (Exception e) { Debug.Log("Error: " + e.Message); }
	}

	async void sendData(int x) {
		ArraySegment<byte> b = new ArraySegment<byte>(Encoding.UTF8.GetBytes(""+x));
		await cws.SendAsync(b, WebSocketMessageType.Text, true, CancellationToken.None);
	}
		
	private async Task<WebSocketReceiveResult> getIntegerFromServer() {
		ArraySegment<byte> buf = new ArraySegment<byte>(new byte[1024]);
		WebSocketReceiveResult r = await cws.ReceiveAsync(buf, CancellationToken.None);
		var x = Encoding.UTF8.GetString(buf.Array, 0, r.Count);
		var y = Int32.Parse(x);
		replacerInt = y;
		return r;
	}

	async Task organizeInitialCubes(int dimensionSize) {
		for(int i=0;i<dimensionSize;i++){
			await getIntegerFromServer();
			DenemeGameManagerScript gameManager = GetComponent<DenemeGameManagerScript>();
			gameManager.initialCubes[i] = replacerInt;
		}
	}

	async Task organizeAnswerCubes(int dimensionSize) {
		for(int i=0;i<dimensionSize;i++){
			for(int j=0;j<dimensionSize;j++){
				await getIntegerFromServer();
				DenemeGameManagerScript gameManager = GetComponent<DenemeGameManagerScript>();
				gameManager.answerCubes[i,j] = replacerInt;
			}
		}
	}

    // Update is called once per frame
    void Update() {}
}

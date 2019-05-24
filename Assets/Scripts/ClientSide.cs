using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System;
using System.IO;
using System.Text;
using static System.Math;

public class ClientSide : MonoBehaviour {

	internal Boolean socketReady = false;
	TcpClient mySocket;
	NetworkStream theStream;
	StreamWriter theWriter;
	StreamReader theReader;
	String Host = "localhost";
	Int32 Port = 65111;

	private int dim;
	private int prob;
	private int[,] pendingCubesMatrix;

	// Use this for initialization
	void Start () {
		dim = StaticValueScript.dimensionSize;
		prob = StaticValueScript.problemNumber;
		setupSocket ();
		Debug.Log ("socket is set up");
		pendingCubesMatrix = new int[dim,dim];
	}

	// Update is called once per frame
	void Update () {		
	}

	public void setupSocket(){
		try{
			mySocket = new TcpClient(Host,Port);
			theStream = mySocket.GetStream();
			theWriter = new StreamWriter(theStream);
			socketReady = true;
			theReader = new StreamReader(theStream);

			Byte[] sendBytes = System.Text.Encoding.ASCII.GetBytes(""+dim);
			theStream.Write(sendBytes, 0, sendBytes.Length);
			Debug.Log("socket is sent: " + dim);

			Byte[] probBytes = System.Text.Encoding.ASCII.GetBytes(""+prob);
			theStream.Write(probBytes, 0, probBytes.Length);
			Debug.Log("problem number is: " + prob);

			var expectedDim = Math.Pow(2,dim);

			for(int i=0;i<expectedDim;i++){
				var x = readPythonInt();
				DenemeGameManagerScript gameManager = GetComponent<DenemeGameManagerScript>();
				gameManager.initialCubes[i] = x;
			}
				

			for(int i=0;i<expectedDim;i++){
				for(int j=0;j<expectedDim;j++){
					var x = readPythonInt();
					DenemeGameManagerScript gameManager = GetComponent<DenemeGameManagerScript>();
					gameManager.answerCubes[i,j] = x;
				}
			}

			mySocket.Close();
		}
		catch(Exception e){
			Debug.Log ("Socket error: " + e);
			//mySocket.Close();
		}
	}
		

	public byte[] Converse(byte[] bytes){
		byte[] temp = new byte[bytes.Length];
		for (int i = 0; i < bytes.Length; i++) {
			temp [i] = bytes [bytes.Length - i - 1];
		}
		return temp;
	}

	public int readPythonInt(){
		
		byte[] incomingBytesSize = new byte[1];
		var tempSize = theStream.Read(incomingBytesSize,0,1);

		var stroTempo = System.Text.Encoding.UTF8.GetString(incomingBytesSize);
		var it = Int32.Parse(stroTempo);

		byte[] incomingBytes = new byte[it];
		var temp = theStream.Read(incomingBytes,0,it);

		var textoTempo = System.Text.Encoding.UTF8.GetString(incomingBytes);
		var temInt = Int32.Parse(textoTempo);

		return temInt;
	}
}

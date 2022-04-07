using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEditor;
using UnityEngine;
using System.IO;


public class ThetaNftDeployer : EditorWindow
{
    /* string objectBaseName = "";
    int objectID = 1; */
    string tokenName;
    string tokenSymbol;
    GameObject objectToDeploy;
    [MenuItem("Theta Tools/NFT Deployer")]
    public static void ShowWindow()
    {
        GetWindow(typeof(ThetaNftDeployer), true, "Theta NFT Deployer");      //GetWindow is a method inherited from the EditorWindow class
    }

    public class SmartContract
    {
        public string bytecodeSource;
        public string opcodes;
        public string sourceMap;
    }

    private void OnGUI()
    {
        /* GUILayout.Label("Theta NFT Deployer", EditorStyles.boldLabel); */
        /*  objectBaseName = EditorGUILayout.TextField("Base Name", objectBaseName);
        objectID = EditorGUILayout.IntField("Object ID", objectID); */
        tokenName = EditorGUILayout.TextField("NFT Name", tokenName);
        tokenSymbol = EditorGUILayout.TextField("NFT Symbol", tokenSymbol);
        objectToDeploy = EditorGUILayout.ObjectField("Prefab to deploy", objectToDeploy, typeof(GameObject), false) as GameObject;

        if (GUILayout.Button("Deploy as NFT")){
            DeployObject();
        }
        
        if (GUILayout.Button("ByteCodeReader")){
            ByteCodeReader();
        }
    }

    private void DeployObject()
    {
       /*  if(objectToDeploy == null)
        {
            Debug.LogError("Error: Please choose wich object you want to deploy as NFT.");
            return;
        } */
        var url = "https://eth-rpc-api-testnet.thetatoken.org/rpc";
        var privateKey = "5c5fe39a97019300e92794f387e14f6b6f0302857803ddc349e28111c0693dec";
        var account = "0xb82F1f95C89cb666f53e6461171311d6aF9F63Ae";
        //initialising the transaction request sender
        var transactionRequest = new TransactionSignedUnityRequest(url, privateKey, 444444444500);
        transactionRequest.UseLegacyAsDefault = true;


        var deployContract = new EIP20Deployment()
        {
            InitialAmount = 10000,
            FromAddress = account,
            TokenName = "TST",
            TokenSymbol = "TST"
        };

        //deploy the contract and True indicates we want to estimate the gas
        yield return transactionRequest.SignAndSendDeploymentContractTransaction<EIP20DeploymentBase>(deployContract);

        if (transactionRequest.Exception != null)
        {
            Debug.Log(transactionRequest.Exception.Message);
            yield break;
        }

        var transactionHash = transactionRequest.Result;

        Debug.Log("Deployment transaction hash:" + transactionHash);

        //create a poll to get the receipt when mined
        var transactionReceiptPolling = new TransactionReceiptPollingRequest(url);
        //checking every 2 seconds for the receipt
        yield return transactionReceiptPolling.PollForReceipt(transactionHash, 2);
        var deploymentReceipt = transactionReceiptPolling.Result;

        Debug.Log("Deployment contract address:" + deploymentReceipt.ContractAddress);

    /*  if (objectBaseName == string.Empty) {
            Debug.LogError("Error: Please enter a base name for the object.");
            return;
        } */
/* 
        Vector3 spawnPos = new Vector3(spawnCircle.x, 0f, spawnCircle.y);

        GameObject newObject = Instantiate(objectToDeploy, spawnPos, Quaternion.identity); */
    }
}




    public partial class NFTDeployment : NFTDeploymentBase
    {
        public NFTDeployment() : base(BYTECODE) { }

        public NFTDeployment(string byteCode) : base(byteCode) { }
    }

    public class NFTDeploymentBase : ContractDeploymentMessage
    {
        StreamReader bytecodeFromJson = File.OpenText("./bytecode.json");
        //Console.WriteLine("bytecodeFromJson total {0}", bytecodeFromJson);
        public static string BYTECODE = "000";//bytecodeFromJson.object;

        public NFTDeploymentBase() : base(BYTECODE) { }

        public NFTDeploymentBase(string byteCode) : base(byteCode) { }

        /* [Parameter("uint256", "_initialAmount", 1)]
        public BigInteger InitialAmount { get; set; } */
        [Parameter("string", "name", 1)]
        public string TokenName { get; set; }
        [Parameter("string", "uri", 2)]
        public string TokenUri { get; set; }
        [Parameter("string", "symbol", 3)]
        public string TokenSymbol { get; set; }
    } 
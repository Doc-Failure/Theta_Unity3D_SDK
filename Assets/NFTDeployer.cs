using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.ABI.Model;
using Nethereum.Contracts;
using Nethereum.Contracts.CQS;
using Nethereum.Contracts.Extensions;
using Nethereum.JsonRpc.UnityClient;
using UnityEngine;
using System.IO;

public class NFTDeployer : MonoBehaviour {

    //Deployment contract object definition

     public partial class NFTDeployment : NFTDeploymentBase
    {
        public NFTDeployment() : base(BYTECODE) { }

        public NFTDeployment(string byteCode) : base(byteCode) { }
    }

    public class NFTDeploymentBase : ContractDeploymentMessage
    {
       /*  StreamReader bytecodeFromJson = File.OpenText("./bytecode.json"); */
        //Console.WriteLine("bytecodeFromJson total {0}", bytecodeFromJson);
        public static string BYTECODE = "000";//bytecodeFromJson.object;

        public NFTDeploymentBase() : base(BYTECODE) { }

        public NFTDeploymentBase(string byteCode) : base(byteCode) { }

       /*  [Parameter("uint256", "_initialAmount", 1)]
        public BigInteger InitialAmount { get; set; } */
        [Parameter("string", "name", 1)]
        public string TokenName { get; set; }
        [Parameter("string", "uri", 2)]
        public string TokenUri { get; set; }
        [Parameter("string", "symbol", 3)]
        public string TokenSymbol { get; set; }

    }
/*
    [Function("transfer", "bool")]
    public class TransferFunctionBase : FunctionMessage
    {
        [Parameter("address", "_to", 1)]
        public string To { get; set; }
        [Parameter("uint256", "_value", 2)]
        public BigInteger Value { get; set; }
    }

    public partial class TransferFunction : TransferFunctionBase
    {

    }

    [Function("balanceOf", "uint256")]
    public class BalanceOfFunction : FunctionMessage
    {
        [Parameter("address", "_owner", 1)]
        public string Owner { get; set; }
    }

    [FunctionOutput]
    public class BalanceOfFunctionOutput : IFunctionOutputDTO
    {
        [Parameter("uint256", 1)]
        public int Balance { get; set; }
    }

    [Event("Transfer")]
    public class TransferEventDTOBase : IEventDTO
    {

        [Parameter("address", "_from", 1, true)]
        public virtual string From { get; set; }
        [Parameter("address", "_to", 2, true)]
        public virtual string To { get; set; }
        [Parameter("uint256", "_value", 3, false)]
        public virtual BigInteger Value { get; set; }
    }

    public partial class TransferEventDTO : TransferEventDTOBase
    {
        public static EventABI GetEventABI()
        {
            return EventExtensions.GetEventABI<TransferEventDTO>();
        }
    } */

    //Sample of new features / requests
    public IEnumerator DeployAndTransferToken()
    {
        var url = "https://eth-rpc-api-testnet.thetatoken.org/rpc";
        var privateKey = "YourPrivateKey";
        var account = "0xYourAccountAddress";
        //initialising the transaction request sender
         var transactionRequest = new TransactionSignedUnityRequest(url, privateKey, 444444444500);
        transactionRequest.UseLegacyAsDefault = true;


        var deployContract = new NFTDeployment()
        {
            FromAddress = account,
            TokenName = "TST",
            TokenUri = "TST",
            TokenSymbol = "TST"
        };

        //deploy the contract and True indicates we want to estimate the gas
        yield return transactionRequest.SignAndSendDeploymentContractTransaction<NFTDeployment>(deployContract);

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
    }
/*
        //Query request using our acccount and the contracts address (no parameters needed and default values)
        var queryRequest = new QueryUnityRequest<BalanceOfFunction, BalanceOfFunctionOutput>(url, account);
        yield return queryRequest.Query(new BalanceOfFunction(){Owner = account}, deploymentReceipt.ContractAddress);

        //Getting the dto response already decoded
        var dtoResult = queryRequest.Result;
        Debug.Log(dtoResult.Balance);


        var transactionTransferRequest = new TransactionSignedUnityRequest(url, privateKey, 444444444500);
        transactionTransferRequest.UseLegacyAsDefault = true;

        var newAddress = "0xde0B295669a9FD93d5F28D9Ec85E40f4cb697BAe";

        var transactionMessage = new TransferFunction
        {
            FromAddress = account,
            To = newAddress,
            Value = 1000,

        };

        yield return transactionTransferRequest.SignAndSendTransaction(transactionMessage, deploymentReceipt.ContractAddress);

        var transactionTransferHash = transactionTransferRequest.Result;

        Debug.Log("Transfer txn hash:" + transactionHash);

        transactionReceiptPolling = new TransactionReceiptPollingRequest(url);
        yield return transactionReceiptPolling.PollForReceipt(transactionTransferHash, 2);
        var transferReceipt = transactionReceiptPolling.Result;

        var transferEvent = transferReceipt.DecodeAllEvents<TransferEventDTO>();
        Debug.Log("Transferd amount from event: " + transferEvent[0].Event.Value);

        var getLogsRequest = new EthGetLogsUnityRequest(url);

        var eventTransfer = TransferEventDTO.GetEventABI();
        yield return getLogsRequest.SendRequest(eventTransfer.CreateFilterInput(deploymentReceipt.ContractAddress, account));

        var eventDecoded = getLogsRequest.Result.DecodeAllEvents<TransferEventDTO>();

        Debug.Log("Transferd amount from get logs event: " + eventDecoded[0].Event.Value); */
    

  // Use this for initialization
    void Start () {

        Debug.Log("Starting NFTDeployer");
        StartCoroutine(DeployAndTransferToken());
    }


    // Update is called once per frame
    void Update () {
		
	}
}

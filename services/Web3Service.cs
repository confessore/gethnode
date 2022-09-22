using example.web.options;
using example.web.services.interfaces;
using Nethereum.Geth;
using Nethereum.JsonRpc.WebSocketClient;
using Nethereum.JsonRpc.WebSocketStreamingClient;
using Nethereum.RPC.Reactive.Eth.Subscriptions;
using Nethereum.Web3;
using Serilog;
using System;
using System.Threading.Tasks;

namespace example.web.services
{
    sealed class Web3Service : IWeb3Service
    {
        readonly IAdminApiService _adminApiService;

        public Web3Service(InfuraOptions infuraOptions)
        {
#if LOCALDEBUG
            var geth = "ws://localhost:8546";
#else
            var geth = "ws://geth:8546";
#endif
#if DEBUG || LOCALDEBUG || STAGING
            var infura = $"wss://ropsten.infura.io/ws/v3/{infuraOptions.ProjectId?.Trim()}";
#else
            var infura = $"wss://mainnet.infura.io/ws/v3/{infuraOptions.ProjectId?.Trim()}";
#endif
            using var geth_wsc = new WebSocketClient(geth);
            using var infura_wsc = new WebSocketClient(infura);
            Geth = new Web3(geth_wsc);
            Infura = new Web3(infura_wsc);
            using var geth_swsc = new StreamingWebSocketClient(geth);
            using var infura_swsc = new StreamingWebSocketClient(geth);
            var geth_subcription = new EthNewBlockHeadersObservableSubscription(geth_swsc);
            var infura_subcription = new EthNewBlockHeadersObservableSubscription(infura_swsc);
            var web3geth = new Web3Geth(geth_wsc);
            _adminApiService = web3geth.Admin; 
        }

        public Web3 Geth { get; }
        public Web3 Infura { get; }

        public async Task<T> InvokeAsync<T>(Func<Web3, Task<T>> function)
        {
            try
            {
                var peers = await _adminApiService.Peers.SendRequestAsync();
                if (peers.Count > 0)
                    return await function.Invoke(Geth);
                throw new Exception("the local geth node does not have any peers");
            }
            catch (Exception e)
            {
                Log.Warning("falling back to infura");
                Log.Warning(e.Message);
                try
                {
                    return await function.Invoke(Infura);
                }
                catch (Exception ex)
                {
                    Log.Fatal("could not interface with blockchain");
                    Log.Fatal(ex.Message);
                    return default!;
                }
            }
        }
    }
}
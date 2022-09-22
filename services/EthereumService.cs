using example.web.services.interfaces;
using Nethereum.Web3;
using System.Threading.Tasks;

namespace example.web.services
{
    sealed class EthereumService : IEthereumService
    {
        readonly IWeb3Service _web3Service;

        public EthereumService(
            IWeb3Service web3Service)
        {
            _web3Service = web3Service;
        }

        public async Task<decimal> GetBalanceInWei(string address)
        {
            var balance = await _web3Service.InvokeAsync(async (web3) =>
                await web3.Eth.GetBalance.SendRequestAsync(address));
            if (balance != null)
                return Web3.Convert.FromWei(balance.Value);
            return decimal.MinusOne;
        }
    }
}

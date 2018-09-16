using NBitcoin;

namespace SafeBlock.io.Models.API
{
    public class NewAddressModel
    {
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
    }
}
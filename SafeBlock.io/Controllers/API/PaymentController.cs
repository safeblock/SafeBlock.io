using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NBitcoin;
using Newtonsoft.Json;
using SafeBlock.io.Models.API;
using SafeBlock.Io.Models;
using SafeBlock.Io.Services;

namespace SafeBlock.io.Controllers.API
{
    /// <summary>
    /// The Payment Controller for handling payments with SafeBlock.io
    /// </summary>
    [Produces("application/json")]
    [Route("api/payment")]
    public class PaymentController : Controller
    {
        /// <summary>
        /// Generate a Mnemonic of 21 chars
        /// </summary>
        /// <remarks>This API methods does not require initialization (/api/initialize)</remarks>
        /// <returns>A mnemonic in english languages and 21 words.</returns>
        [HttpGet]
        [Route("new-mnemonic")]
        public string NewMnemonic()
        {
            return new Mnemonic(Wordlist.English, WordCount.TwentyOne).ToString();
        }

        /// <summary>
        /// Generate a specified currency address pair of Public and Private Keys
        /// </summary>
        /// /// <remarks>This API methods does not require initialization (/api/initialize)</remarks>
        /// <param name="currency">Specify the currency to use (default: bitcoin)</param>
        /// <returns>A pair of Private and Public Keys</returns>
        [HttpGet]
        [Route("new-address")]
        [Route("new-address/{currency}")]
        public object NewAddress(string currency)
        {
            switch(currency)
            {
                default:
                    var privateKey = new Key();
                    var publicKey = privateKey.PubKey.GetAddress(Network.Main);
                    return new NewAddressModel()
                    {
                        PublicKey = publicKey.ToString(),
                        PrivateKey = privateKey.GetWif(Network.Main).ToString()
                    };
            }
        }

        [HttpGet]
        [Route("received-by-address/{address}")]
        [Route("received-by-address/{address}/{currency?}")]
        public string ReceivedByAddress(string address, string currency)
        {
            switch(currency)
            {
                default:
                    //TODO : to change without using sub api
                    using (var balanceLookup = new WebClient())
                    {
                        dynamic lookup = JsonConvert.DeserializeObject(balanceLookup.DownloadString($"https://blockchain.info/fr/balance?active={address}"));
                        return lookup[address]["total_received"].ToString();
                    }
            }
        }
    }
}

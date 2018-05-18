using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NBitcoin;
using SafeBlock.io.Models.API;
using SafeBlock.Io.Services;

namespace SafeBlock.io.Controllers.API
{
    [Produces("application/json")]
    [Route("api/escrow")]
    public class EscrowController : Controller
    {
        /// <summary>
        /// Initialize the using of the API and get an entrypoint and wallet to deposit
        /// </summary>
        /// <returns>An entrypoint and a public address for deposit</returns>
        [HttpGet]
        [Route("initialize")]
        [Route("initialize/{releaseAddress}")]
        public object Initialize(string releaseAddress)
        {
            var privateKey = new Key();
            var publicKey = privateKey.PubKey.GetAddress(Network.Main);
            //TODO : store escrow in database private key, public key and entrypoint

            return new InitializeModel()
            {
                EntryPoint = SecurityUsing.CreateCryptographicallySecureGuid(),
                PublicKey = publicKey.ToString(),
                ReleaseAddress = null
            };
        }

        /// <summary>
        /// Release the specified escrow
        /// </summary>
        /// <param name="releaseAddress">Specify a release address, if not specified the default is used</param>
        /// <returns>The result</returns>
        [HttpGet]
        [Route("release/{entrypoint}")]
        [Route("release/{entrypoint}/{releaseAddress}")]
        public string Release(string entrypoint, string releaseAddress)
        {
            //TODO : create transaction for released and specify comission
            return $"The escrow {entrypoint} is released at {releaseAddress}.";
        }

        /// <summary>
        /// Remove the escrow transaction from database
        /// </summary>
        /// <param name="entrypoint">Specify the entrypoint to remove</param>
        /// <returns>The result</returns>
        [HttpDelete]
        [Route("destroy/{entrypoint}")]
        public string Destroy(string entrypoint)
        {
            //TODO : remove the entrypoint from the database
            return $"The escrow {entrypoint} is fully destroyed.";
        }
    }
}

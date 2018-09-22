# Official SafeBlock.io Repository
This is the official repository of safeblock.io a multicurrencies wallet with lots off features.

![](https://img.shields.io/badge/netcore-v2.1-brightgreen.svg?longCache=true) ![](https://img.shields.io/badge/bootstrap-4.1.1-blue.svg?longCache=true) ![](https://img.shields.io/badge/nbitcoin-4.1.1.7-lightgray.svg?longCache=true) ![](https://img.shields.io/badge/BCrypt.Net--Next-2.1.3-lightgray.svg?longCache=true)

![](https://pix.watch/_NDYc1/L2lmib.png)

## Proof of Concept for SafeBlock.io Platform
SafeBlock.io is a cryptocurrency platform with multiple services, the base service is a multicurrencies wallet, user had the possibility to transparently store his mnemonic seed on the platform (like blockchain.com) or only in the session, the kind of wallet is choosen at the wallet creation.

The others services are a integrated mixer, an API for handling payment, escrow transactions etc, a full node hosting service (for stake Stratis by example), an exchange solution (like shapeshift.io) and a Vault.

We want to propose a large panel of security solutions, like PGP-2FA, SMS-2FA, OTP-2FA, delayed vault opening, and more.

## Microsoft .Net Core Using
We opted for [Microsoft .Net Core](https://www.microsoft.com/net/learn/dotnet/what-is-dotnet) that provide all the options ans solutions that we need for building this platform, for the cryptocurrency manipulation we use [NBitcoin](https://github.com/MetacoSA/NBitcoin) C# Library.

## Consul as Health Check API
SafeBLock.io use `consul` for populate the Service Status page and also for creating watchdog.

## Redis as Distributed Cache
SafeBlock.io use `Microsoft.Extensions.Caching.Redis` for adding a .Net Core abstraction layer to use `redis` as a session storage.

## HashiCorp Vault for Wallets Hosting
SafeBlock.io use `vault` and `VaultSharp` library to store all his wallets.


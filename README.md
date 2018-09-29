# Official SafeBlock.io Repository
This is the official repository of safeblock.io a multicurrencies wallet with lots off features.

![](https://img.shields.io/badge/netcore-v2.1-brightgreen.svg?longCache=true) ![](https://img.shields.io/badge/bootstrap-4.1.1-blue.svg?longCache=true) ![](https://img.shields.io/badge/nbitcoin-4.1.1.7-lightgray.svg?longCache=true) ![](https://img.shields.io/badge/BCrypt.Net--Next-2.1.3-lightgray.svg?longCache=true)

![](https://pix.watch/_NDYc1/L2lmib.png)

# What does SafeBlock.io Offer ?

It was in 2017 that was born the idea of SafeBlock.io, our philosophy is quite simple, build a platform supplying many services and easy to use for novices we want both create innovative services that the blockchain community needs while making possible the cryptocurrencies usage in the real economy for people.

It’s fair to note that currently the cryptocurrencies are mainly (and we hope for the shortest time possible) used by rich people to become richer and not really by the greatest number, but we think this trend will be reversed in few years, and we do our best to work on it.

## Overview of our Services

SafeBlock.io embed of course a secure wallet for $BTC, $ETH, $STRAT, $ZEC, $XMR, $LTC and $DASH (and more in the future), a mixing service, a low fees exchange, a simple way to buy coins and more explained below :

*    Vault: Store your funds in cryptos or in fiat.
*   Escrow Platform: Make secure transactions on the fly by using our API.
*    Full Node Hosting: Deploy a Full Node in few seconds and to secure your funds.
*    Cryptos Debit Card: Use your SafeBlock.io account for your everyday life.
*    API: Safely use our services everywhere.
*    And some surprises !

## Subscribe to our Newsletter

If you are interested in the project, and you want to get informed about the launching of SafeBlock.io, please, do not hesitate to click below and put your email address:

> [Subscribe to our Newsletter by Clicking Here](https://safeblock.io/)

## We Need your Help

We are currently hard working on SafeBlock to make the best product that fits our vision. We need talented people to reach the ship and navigate through the restless ocean of cryptocurrencies. If you want to contribute to make this ocean a better place to be, we need you so please consider joining us !
How to Join Us

*    Contact us by email at contact@safeblock.io
*    Join our Discord : https://discord.gg/HdsVDn2
*   Our twitter https://twitter.com/safeblock_io

## Share and Enjoy

# Technicals Details

## Microsoft .Net Core Using
We opted for [Microsoft .Net Core](https://www.microsoft.com/net/learn/dotnet/what-is-dotnet) that provide all the options ans solutions that we need for building this platform, for the cryptocurrency manipulation we use [NBitcoin](https://github.com/MetacoSA/NBitcoin) C# Library.

## Consul as Health Check API
SafeBLock.io use `consul` for populate the Service Status page and also for creating watchdog.

## Redis as Distributed Cache
SafeBlock.io use `Microsoft.Extensions.Caching.Redis` for adding a .Net Core abstraction layer to use `redis` as a session storage.

## HashiCorp Vault for Wallets Hosting
SafeBlock.io use `vault` and `VaultSharp` library to store all his wallets.


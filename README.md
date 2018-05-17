# SafeBlock Repository
This is the official repository of safeblock.io a multicurrencies wallet with lots off features.

![](https://img.shields.io/badge/safeblock-v2.0.0-brightgreen.svg?longCache=true) ![](https://img.shields.io/badge/bootstrap-4.1.1-blue.svg?longCache=true) ![](https://img.shields.io/badge/nbitcoin-4.1.1.7-lightgray.svg?longCache=true) ![](https://img.shields.io/badge/BCrypt.Net--Next-2.1.3-lightgray.svg?longCache=true)

## Installation on Debian Stretch
This is the manifest for the installation in a production environment.
> This manifest is for a debian 9 installation

### Dotnet Framework installation
![](https://img.shields.io/badge/netcore-2.0-blue.svg?longCache=true) SafeBlock is coded in ASP.Net Core.

```console
$ wget -qO- https://packages.microsoft.com/keys/microsoft.asc | gpg --dearmor > microsoft.asc.gpg
$ sudo mv microsoft.asc.gpg /etc/apt/trusted.gpg.d/
$ wget -q https://packages.microsoft.com/config/debian/9/prod.list
$ sudo mv prod.list /etc/apt/sources.list.d/microsoft-prod.list
```

### PostgresSQL Installation
![](https://img.shields.io/badge/postgresql-10.4-lightgray.svg?longCache=true) Postgres is one of the best SGBD, he is famous for storing huge datas.

```console
$ sudo apt-get update
$ sudo apt-get install postgresql postgresql-client
#testing installation
$ su - postgres
$ psql
```
#### Creating Users/Roles and Importing Database
lorem ipsum

### Vault Installation
![](https://img.shields.io/badge/vault-0.10-lightgray.svg?longCache=true) Vault protect sensitive data in a secure vault, with sealing capabilities.

Download the binary from here https://www.vaultproject.io/downloads.html and execute this commands :
```console
$ wget https://releases.hashicorp.com/vault/0.10.1/vault_0.10.1_linux_amd64.zip
$ unzip vault_0.10.1_linux_amd64.zip
```

### Redis Installation
![](https://img.shields.io/badge/redis-3.2.11-lightgray.svg?longCache=true)  Redis is used for session distributed cache for storing sessions datas in a secured environnement.

The previous version of Redis is used for his security improvements like redis.io said : 
>Redis 3.2 is the previous stable release. Does not include all the improvements in Redis 4.0 but is a very battle tested release, probably a good pick for critical applications while 4.0 matures more in the next months. ""

```console
$ wget http://download.redis.io/releases/redis-3.2.11.tar.gz
$ tar xvzf redis-3.2.11.tar.gz
$ cd redis-3.2.11
$ make
```

### Security Requirements
It's important that nobody can to connect to the server without an RSA key.

### Required Libraries
You must install this libraries before installing SafeBlock.io : `apt-transport-https`, ``

### Tmux Using
For launching programs and other stuffs we use tmux, for example launching vault or redis

### Health & Stats
lorem ipsum
(consul / sentry)

### Check List after Installation
Some operations are needed for finishing installation.

# SafeBlock.io API Documentation

SafeBlock.io provides a simple and convenient API for handling payments and escrow transactions, mixing and more, usable without registration or API key.

The SafeBlock.io platform and APIs can be used through the Tor network. [See More](#use-api-under-tor-network)

## API Guide Reference
This API is built with [Microsoft .NET Core](https://docs.microsoft.com/fr-fr/aspnet/core/?view=aspnetcore-2.1) as a RestFul Web API.

### Payment Entrypoint
The entrypoint for payment handling is `/api/payment` and must be called by the **POST** method.

#### Generate a Mnemonic
```bash
curl -X POST 'https://safeblock.io/api/payment/new-mnemonic'
```

```json
{
    "mnemonic": "among coffee return able govern scrub cave trim allow burger gather raccoon regret summer pear crucial film expire flock rack punch"
}
```

## Payment Request

## Use API under Tor Network

## Commands

* `mkdocs new [dir-name]` - Create a new project.
* `mkdocs serve` - Start the live-reloading docs server.
* `mkdocs build` - Build the documentation site.
* `mkdocs help` - Print this help message.

## Project layout

    mkdocs.yml    # The configuration file.
    docs/
        index.md  # The documentation homepage.
        ...       # Other markdown pages, images and other files.


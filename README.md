# expenses-api

This is a simple and lazy cloud web API with the goal to keep track of incomes and expenses. Since I'm a poor man and I'm literally doing this project with the purpose of saving some money, all the requirements, configurations and tools used are free!

## Configuration

This project has been created with the will of publishing it in the Azure cloud platform with a Cosmos DB backend so it could be accessible from anywhere. The service can still be run locally but the Cosmos DB configuration is mandatory in order to use this web API. Below is presented the list of operations to perform:

### Azure web portal

- Create a Free tier Azure account
- Create a Free tier Azure Cosmos DB account

  - Core (SQL) API
  - Basics
    - Subscription and Resource Group: select an existing one or create new ones
    - Account name: whatever you want
    - Location: what you prefer. I'm from the pizza mandolino country, so I have chosen West Europe.
    - Capacity mode: `Provisioned throughput`
    - Apply Free Tier Discount: `Apply` (Recommended to have access to furhter 400 RU/s per month - on top of the 1000 RU/s of the Free Tier - for 12 months)
    - Limit total account throughput: check the `Limit the total amount of throughput that can be provisioned on this account` (Recommended if you want to avoid unexpected charges)
  - For all the other settings (section Global Distribution, Networking, Backup Policy, Encryption and Tags) I left the default values

  Once created access your Cosmos DB account and go to the `Settings`&rarr;`Keys`&rarr;`Read-write Keys` section and save somewhere the URI and primary key values (this surprise tools will help us later).

  If you want to be sure to keep your account free you can go to the `Cost Management` section and set the total throughput limit to 1000 RU/s or 1400 if you applied the Free Tier Discount (Recommended if you want to avoid unexpected charges)

- Create new Container (one for Expenses and one for Incomes, the Database can be the same)
  - Create new Database
    - Choose a database ID (remember it because this surprise tool will help us later)
    - Database throughput: `Manual`
    - Database Max RU/s: 1000 RU/s
  - Choose a container ID (remember it because this surprise tool will help us later)
  - Indexing: `Automatic`
  - Partition key:
    - Partition key value: `/username, /guid`
    - Partitioning: `Hierarchical`

### Project configuration

#### Run project in local

If you want to run the project locally you just need to go to the `appsettings.json` file and substitute the following placeholders:

```(text)
  "CosmosDB": {
    "DatabaseName": "<DATABASE_NAME>",
    "ExpensesContainerName": "<CONTAINER_NAME>",
    "IncomesContainerName": "<CONTAINER_NAME>",
    "AccountEndpoint": "<ACCOUNT_ENDPOINT>",
    "Key": "<KEY>"
  }
```

##### CosmosDB

Remember all the surprise tools we encountered before? Well it's time to substitute those values here in order to integrate the web API with the Cosmos DB container we have created. The `DatabaseName` and `ContainerName` settings must contain the database ID and container ID created on the Cosmos DB account, while the `AccountEndpoint` and `Key` settings corresponds to the URI and the read-write primary key retrieved.

#### Publish project on Azure

If you want to publish the project on Azure, after configuring all the staff required in order to publish the web API in your Azure account, before pressing the `Publish` button remember to go in the `Hosting profile section actions`&rarr;`Manage Azure App Service settings` (you can access it by pressing the `...` button in the `Hosting` section) and manually add all the authentication and Cosmos DB settings. Remember that the keys must be set as follows:

- CosmosDB\_\_DatabaseName
- CosmosDB\_\_ExpensesContainerName
- CosmosDB\_\_IncomesContainerName
- CosmosDB\_\_AccountEndpoint
- CosmosDB\_\_Key

#### Enable logging in Azure

Go to the web app you created, and under the section `Monitoring`&rarr;`App service logs` enable `Application logging (Filesystem)` with the verbosity level you prefer and set `Web server logging` to `Filesystem`.
Once saved the configuration go to `Monitoring`&rarr;`Log stream` and you you start seeing the console logs stream of the application

## Usage

To use the web API you can either use a custom HTTP client or something like Postman. You can also access the Swagger UI page at `<ENDPOINT>/swagger/index.html` where `<ENDPOINT>` will be for example `http://localhost:5000` or `https://localhost:5001` if you run the application locally or the `<ACCOUNT_ENDPOINT>` value if the project has been published on Azure, in order to consult the documentation.

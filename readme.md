# ID.me .NET Sample App

Sample app for an ID.me OIDC integration 
## Table of Contents

- [ID.me .NET Sample App](#idme-net-sample-app)
  - [Table of Contents](#table-of-contents)
  - [Installation](#installation)
  - [Usage](#usage)
  - [Contributing](#contributing)

## Installation

1. Clone the repository.
2. Rename appsettings copy.json to appsettings.json
3. Input Client ID/Client Secret from ID.me Developer Portal into appsettings.json
4. Ensure "http://localhost:5030/authorization-code/callback" is in your Consumer's list of Redirect URI
5. Restore NuGet packages.
    `dotnet restore`
6. Build + run the solution.
    `dotnet run`

## Usage

1. Configure the necessary settings in the appsettings.json file.
2. Run the application.
3. Access the application in your web browser at http://localhost:5030

## Contributing

Contributions are welcome! 

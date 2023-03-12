# Onspring Azure Active Directory Syncer

A .NET console application that can be run on a schedule or as a scheduled task that will synchronize users and groups between Azure Active Directory and Onspring making Azure Active Directory the system of record.

_**Note:**_ This is an unofficial Onspring integration. It was not built in consultation with Onspring Technologies LLC.

## Table of Contents

- [Overview](#overview)
  - [Features](#features)
- [Requirements](#requirements)
- [Installation](#installation)
- [Onspring Setup](#onspring-setup)
- [Azure Active Directory Setup](#azure-active-directory-setup)
- [Configuration](#configuration)
  - [Default Mappings](#default-mappings)
  - [Activating and Deactivating Users](#activating-and-deactivating-users)
  - [Custom Mappings](#custom-mappings)
  - [Validating Mappings](#validating-mappings)
- [Usage](#usage)
- [Limitations](#limitations)
- [License](#license)
- [Inspiration](#inspiration)

## Overview

### Features

✅ Allow Azure Active Directory to serve as the system of record for managing users and groups.

✅ Synchronize all users and groups in Azure Active Directory with Onspring.

✅ Activate and deactivate Onspring users based on group membership in specific Azure Active Directory groups.

✅ Map Azure Active Directory user properties to Onspring user fields.

✅ Map Azure Active Directory group properties to Onspring group fields.

## Requirements

- You must have an Onspring instance.
- You must have an Onspring API Key that has the following permissions to the following apps:
  - `Users`
    - Create
    - Read
    - Update
  - `Groups`
    - Create
    - Read
    - Update
- You must have an Azure Active Directory tenant.
- You must have an Azure Active Directory application registered in your Azure Active Directory tenant.
  - The application must have the following permissions:
    - `Group.Read.All`
    - `User.Read.All`

## Installation

The app is published as a release on GitHub. You can download the latest release from the [releases](https://github.com/StevanFreeborn/OnspringAzureADSyncer/releases) page. It is published as a single executable file for the following operating systems:

- win-x64
- linux-x64
- ox-x64 (Minimum OS version is macOS 10.12 Sierra)
  - Note after downloading the executable you may need to run `chmod +x` to give the executable execute permissions on your machine.
  - Note after downloading the executable you may need to provide permission for the application to run via the your systems settings.

You are also welcome to clone this repository and run the app using the [.NET 7](https://dotnet.microsoft.com/en-us/download) tooling and runtime. As well as modify the app further for your specific needs.

## Onspring Setup

## Azure Active Directory Setup

## Configuration

The app is configured via a JSON file. You will need to pass the path to the configuration file using the configuration option `--config` or `-c`. If the config file is not specified the app will not run. See below for an example:

```shell
./OnspringAzureADSyncer -c /path/to/config.json
```

The configuration file needs to have the following format and properties set:

```json
{
  "Settings": {
    "Onspring": {
      "BaseUrl": "https://api.onspring.com",
      "ApiKey": "000000ffffff000000ffffff/00000000-ffff-0000-ffff-000000000000",
      "UsersAppId": 1,
      "GroupsAppId": 1
    },
    "Azure": {
      "TenantId": "00000000-0000-0000-0000-000000000000",
      "ClientId": "0a000aa0-1b11-2222-3c33-444444d44d44",
      "ClientSecret": "00000~00000--000000000-00000000000000000"
    }
  }
}
```

### Default Mappings

The app will map the following properties from Azure Active Directory to Onspring by default:

#### Groups

| Azure Active Directory Property | Onspring Group Field |
| ------------------------------- | -------------------- |
| `id`                            | `Name`               |
| `description`                   | `Description`        |

_**Note:**_ The `id` property is used as the `Name` field in Onspring because the `Name` field is required and the `id` property is guaranteed to be unique.

_**Note:**_ The `id` property to `Name` field mapping cannot be changed or overwritten.

#### Users

| Azure Active Directory Property | Onspring User Field |
| ------------------------------- | ------------------- |
| `userPrincipalName`             | `Username`          |
| `givenName`                     | `First Name`        |
| `surname`                       | `Last Name`         |
| `mail`                          | `Email Address`     |
| `memberOf`                      | `Groups`            |

_**Note:**_ The `memberOf` property to `Groups` field mapping cannot be changed or overwritten.

_**Note:**_ The Onspring Users `Status` field is not mapped and cannot be mapped to an Azure property. However it will be set by the app for each user. The app determines what value (Active or Inactive) this field is set to based upon whether the user is a member of a specified [OnspringActiveGroup](#activating-and-deactivating-users) and is `Enabled` in Azure Active Directory.

### Activating and Deactivating Users

The app can be configured to enable activating and deactivating users in Onspring based upon membership to specific Azure Active Directory groups by adding a property to the `Azure` object called `OnspringActiveGroups` that contains an array of object ids for each group that contains users who should be active in Onspring. See below for an example:

```json
{
  ...,
  "Azure": {
    "TenantId": "00000000-0000-0000-0000-000000000000",
    "ClientId": "0a000aa0-1b11-2222-3c33-444444d44d44",
    "ClientSecret": "00000~00000--000000000-00000000000000000",
    "OnspringActiveGroups": [
      "00000000-0000-0000-0000-000000000000",
      "00000000-0000-0000-0000-000000000000"
    ]
  },
  ...
}
```

When this property is set the app will set Onspring users to active if they are a member of one of these groups and their account is `Enabled` in Azure Active Directory.

### Custom Mappings

The app can be configured to map custom properties from Azure Active Directory to Onspring by adding the `GroupsFieldMappings` and `UsersFieldMappings` properties to the `Settings` object. The properties in these objects should be the id of the field in Onspring and the name of the property for the Azure resource whose value you want to map to that field in Onspring. See below for an example:

```json
{
  ...,
  "GroupsFieldMappings": {
    "4933": "displayName"
  },
  "UsersFieldMappings": {
    "9": "streetAddress",
    "2": "city",
    "4934": "state",
    "11": "postalCode"
  },
  ...
}
```

This does support mapping Azure properties to Onspring fields that are and are not included in the default mappings. In addition you can map a single Azure property to multiple Onspring fields if you need to. As noted in the [Default Mappings](#default-mappings) section there are some fields that cannot be mapped or whose mappings can not be overwritten.

A list of properties that can be mapped to Onspring fields can be found in the [Microsoft Graph API documentation](https://learn.microsoft.com/graph):

- [Group](https://docs.microsoft.com/graph/api/resources/group?view=graph-rest-1.0#properties)
- [User](https://docs.microsoft.com/graph/api/resources/user?view=graph-rest-1.0#properties)

_**Note:**_ Only properties that are of primitive types can be mapped to Onspring fields. Complex types are not supported.

### Validating Mappings

Prior to the app actually attempting to sync groups or users it will validate the mappings that have been passed to it in the configuration file. Specifically it will check to make sure that all the required fields have a property mapped to them. It will also validate that the type of field a property is mapped to is compatible with the type of the property.

#### Required Fields

##### Groups - Required Fields

| Onspring Group Field |
| -------------------- |
| `Name`               |

##### Users - Required Fields

| Onspring User Field |
| ------------------- |
| `Username`          |
| `First Name`        |
| `Last Name`         |
| `Email Address`     |
| `Groups`            |

#### Azure Property to Onspring Field Type Compatibility

| Azure Property Type | Onspring Field Type |
| ------------------- | ------------------- |
| `String`            | `Text`              |
| `String`            | `List`              |
| `String Collection` | `Text`              |
| `String Collection` | `Multi-Select List` |
| `Boolean`           | `Text`              |
| `Boolean`           | `List`              |
| `DateTimeOffset`    | `Text`              |
| `DateTimeOffset`    | `Date/Time`         |

## Usage

## Limitations

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE.txt) file for details

## Inspiration

This project was built as a submission for the [Microsoft Graph Hackathon 2023](https://devblogs.microsoft.com/dotnet/hack-together-microsoft-graph-dotnet/). The project was built using the [Microsoft Graph .NET SDK](https://github.com/microsoftgraph/msgraph-sdk-dotnet) and the [Onspring API .NET SDK](https://github.com/onspring-technologies/onspring-api-sdk). It illustrates a solution to a real world challenge that Onspring customers face when trying to find a way to leverage their existing identity management solution in Azure AD to also manage groups and users in [Onspring](https://onspring.com/).

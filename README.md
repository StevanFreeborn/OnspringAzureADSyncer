# Onspring Azure Active Directory Syncer

[![build_test](https://github.com/StevanFreeborn/OnspringAzureADSyncer/actions/workflows/build_test.yml/badge.svg)](https://github.com/StevanFreeborn/OnspringAzureADSyncer/actions/workflows/build_test.yml)
[![codecov](https://codecov.io/github/StevanFreeborn/OnspringAzureADSyncer/branch/main/graph/badge.svg?token=5rzMLI4SJz)](https://codecov.io/github/StevanFreeborn/OnspringAzureADSyncer)
[![publish_and_release](https://github.com/StevanFreeborn/OnspringAzureADSyncer/actions/workflows/publish_release.yml/badge.svg)](https://github.com/StevanFreeborn/OnspringAzureADSyncer/actions/workflows/publish_release.yml)
![GitHub](https://img.shields.io/github/license/StevanFreeborn/OnspringAzureADSyncer)
![GitHub release (latest SemVer)](https://img.shields.io/github/v/release/StevanFreeborn/OnspringAzureADSyncer)
[![semantic-release: angular](https://img.shields.io/badge/semantic--release-angular-e10079?logo=semantic-release)](https://github.com/semantic-release/semantic-release)
![GitHub Downloads (all assets, all releases)](https://img.shields.io/github/downloads/StevanFreeborn/OnspringAzureADSyncer/total)

A .NET console application that can be run on a schedule or as a scheduled task that will synchronize users and groups between Azure Active Directory and [Onspring](https://onspring.com/) making Azure Active Directory the system of record.

_**Note:**_ This is an unofficial Onspring integration. It was not built in consultation with Onspring Technologies LLC.

## Table of Contents

- [Overview](#overview)
  - [Features](#features)
- [Requirements](#requirements)
- [Installation](#installation)
- [Onspring Setup](#onspring-setup)
  - [BaseUrl](#base-url)
  - [ApiKey](#apikey)
    - [Permission Considerations](#permission-considerations)
    - [API Usage Considerations](#api-usage-considerations)
- [Azure Active Directory Setup](#azure-active-directory-setup)
- [Configuration](#configuration)
  - [Default Mappings](#default-mappings)
  - [Activating and Deactivating Users](#activating-and-deactivating-users)
  - [Custom Mappings](#custom-mappings)
  - [Validating Mappings](#validating-mappings)
  - [Group Filters](#group-filters)
- [Options](#options)
- [Output](#output)
  - [Log](#log)
- [Limitations](#limitations)
- [License](#license)
- [Inspiration](#inspiration)

## Overview

Many Onspring customers who utilize the Onspring platform choose to leverage Onspring's existing SSO integration for authentication. This integration allows customers to setup a single sign-on experience for their users using their existing identity management provider. As part of this existing integration Onspring supports accepting claims for the user's username, email address, first name, last name, and optionally groups.

It also allows for just-in-time provisioning of users and groups if they are not already present in Onspring. This integration also updates the user's username, email address, first name, last name, and groups in Onspring upon each login if they change in the identity management provider between logins. Many customers utilize this integration with Azure Active Directory (AAD) as their identity management provider

However the existing SSO integration doesn't allow customers to centrally manage their users and groups in Onspring through Azure Active Directory and keep the two systems in sync using Azure Active Directory as the system of record. Nor allow them to leverage any existing user and group data or access models built in Azure Active Directory to also manage access in Onspring.

The OnspringAzureADSyncer app is meant to help fill this gap and provide Onspring customers with a way to sync Azure AD groups and users to Onspring.

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

You are also welcome to clone this repository and run the app using the [.NET 9](https://dotnet.microsoft.com/en-us/download) tooling and runtime. As well as modify the app further for your specific needs.

## Onspring Setup

In order to configure the app you will need to perform some setup in Onspring to acquire the following required configuration values:

- `BaseUrl`
- `ApiKey`
- `UsersAppId`
- `GroupsAppId`

### Base Url

The `BaseUrl` is the base url for the Onspring API. Currently this will always be `https://api.onspring.com`. It does not matter if you are using the app with a `Development`, `Test`, or `Production` instance.

### ApiKey

This app makes use of version 2 of the Onspring API. Therefore you will need an API Key to be able to utilize the app. API keys may be obtained by an Onspring user with permissions to at least read API Keys for your instance, using the following instructions:

- Within Onspring, navigate to /Admin/Security/ApiKey.
- On the list page, add a new API Key (requires Create permissions) or click an existing API Key to view its details.
- On the details page for an API Key, click on the Developer Information tab.
- Copy the X-ApiKey Header value from this section.

_**Important:**_

- An API Key must have a status of Enabled in order to be used.
- Each API Key must have an assigned Role. This role controls the permissions for requests that are made by this tool to retrieve files from fields on records in an app. If the API Key does not have sufficient permissions the attachment will not be downloaded.

#### Permission Considerations

You can think of any API Key as another user in your Onspring instance and therefore it is subject to all the same permission considerations as any other user when it comes to it's ability to access a file. The API Key you use with this tool need to have all the correct permissions within your instance to access the record where a file is held and the field where the file is held. Things to think about in this context are `role permissions`, `content security`, and `field security`.

#### API Usage Considerations

This app uses version 2 of the Onspring API to sync groups and users. Currently this version of the Onspring API does not provide any endpoints to perform bulk operations.

This app will make a number of api requests to Onspring in order to...

- collect all the fields in each app
- look up if list values need to be created
- create list values
- look up whether a group or user exists
- create or update groups or users

The total number of requests will vary depending on the number of users and groups being synced as well as the mappings you've configured.

This all being shared because it is important you take into consideration the number of Groups and Users you are planning to sync with Onspring. If the quantity is quite considerable I'd encourage you to consult with your Onspring representative to understand what if any limits there are to your usage of the Onspring API.

### App Ids

The app needs to know the `App Id` for the `Users` and `Groups` apps in your Onspring instance. You can find the `App Id` for each app by navigating to the app's admin panel in Onspring and looking at the url. The `App Id` is the number at the end of the url.

For example, if you are looking at the `Users` app in Onspring and the url is `https://myinstance.onspring.com/Admin/App/1` then the `App Id` is `1`.

You can also find the `AppId` for each app by using the `API Key` you created to call the `/Apps` endpoint of the Onspring API. This [swagger page](https://api.onspring.com/swagger/index.html) can be useful for this type of exploratory call.

## Azure Active Directory Setup

In order to provide the app with the proper permissions to sync your Azure Active Directory groups and users you will need to setup a new application registration within Azure Active Directory. The following steps can be followed to register the app and get the necessary `Tenant Id`, `Client Id`, and `Client Secret`.

1. Go to Azure Active Directory and select `App Registrations` from the left hand menu.
2. Click `New Registration`.
3. Enter a name for the app and select `Accounts in any organizational directory (Any Azure AD directory - Multitenant) and personal Microsoft accounts (e.g. Skype, Xbox)` for the supported account types.
4. Leave the `Redirect URI` blank.
5. Click `Register`.
6. Go to the `API Permissions` section and click `Add a permission`.
7. Select `Microsoft Graph` and then `Application permissions`.
8. Select the following permissions:
   - `Group.Read.All`
   - `User.Read.All`
9. Click `Add permissions`.
10. Click `Grant admin consent for <your tenant name>`.
11. Go to the `Certificates & secrets` section and click `New client secret`.
12. Enter a description for the secret and select an expiry time. Click `Add`.
13. Copy the `Tenant Id`, `Client Id`, and `Client Secret` values from the `Overview` section of the app registration.

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

_**Note:**_ You can see an example of a complete configuration file in the [exampleconfig.json](src/exampleconfig.json) file in this repository.

### Default Mappings

The app will map the following properties from Azure Active Directory to Onspring by default:

#### Groups

| Azure Active Directory Property | Onspring Group Field |
| ------------------------------- | -------------------- |
| `id`                            | `Name`               |
| `description`                   | `Description`        |

_**Note:**_ The `id` property is used as the `Name` field in Onspring because the `Name` field is required and the `id` property is guaranteed to be unique. This however can be changed by adding a custom mapping for the `Name` field in the configuration file. You will want to ensure that whatever property you map to the `Name` field is unique for each group otherwise you will get an error when trying to create the group in Onspring. It would also be worthy to note that if you are using this app in conjunction with the Onspring SSO integration you will want to ensure that the property you have mapped to the `Name` field is the same as the property for the `Group` claim that is being passed to Onspring.

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

_**Note:**_ The app will only manage the `Status` field for users in Onspring that match with a user in Azure Active Directory. If a user is in Onspring but not in Azure Active Directory the app will not change the `Status` field for that user.

_**Note:**_ If you've configured group filters the app will only consider groups that match the filters when determining if a user should be active or inactive. So you'll want to ensure that the groups you've specified in the `OnspringActiveGroups` array will satisfy the group filters you've set.

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

_**Note:**_ Mapped property names are case insensitive.

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

### Group Filters

The app can be configured to filter the groups that are synced from Azure Active Directory to Onspring by adding the `GroupFilters` property to the `Azure` section of the configuration file. The `GroupFilters` property should be an array of objects that contain a `Property` and `Pattern` property. The `Property` value should be the name of the property on the Azure Active Directory group that you want to filter on and the `Pattern` value should be a regular expression that the value of the property should match. See below for an example:

```json
{
  ...,
  "Azure": {
    "TenantId": "00000000-0000-0000-0000-000000000000",
    "ClientId": "0a000aa0-1b11-2222-3c33-444444d44d44",
    "ClientSecret": "00000~00000--000000000-00000000000000000",
    "GroupFilters": [
      {
        "Property": "displayName",
        "Pattern": ".*-Onspring$"
      }
    ]
  },
  ...
}
```

_**Note:**_ These filters are ANDed together. So a group must match all the filters to be synced to Onspring.

_**Note:**_ The `Property` value is case insensitive, but it should be a property of type `String` on the Azure Active Directory group.

_**Note:**_ The `Pattern` value is a regular expression that will be used to match the value of the property on the Azure Active Directory group. The regular expression should be a valid .NET regular expression pattern.

_**Note:**_ The app will only sync groups from Azure Active Directory to Onspring that match the filters you've set. If you have not set any filters the app will sync all groups from Azure Active Directory to Onspring.

_**Note:**_ Prior to running the syncer will validate the group filters to ensure the property is a valid property on the Azure Active Directory group with the type string and that the pattern is a valid regular expression. Note the regular expression uses the .NET regular expression engine. See the [Microsoft .NET Regular Expression Language](https://learn.microsoft.com/en-us/dotnet/standard/base-types/regular-expression-language-quick-reference) for more information.

## Options

This app currently has a number of options that can be passed as command-line arguments to alter the behavior of the app. Theres are detailed below and can also be viewed by passing the `-h` or `--help` option to the app.

- **Log Level:** `--log` or `-l`
  - Allows you to specify what the minimum level of event that will be written to the log file while the app is running.
  - By default this will be set to the Verbose level.
  - The valid levels are: Debug | Error | Fatal | Information | Verbose | Warning
  - **Example usage:** `OnspringAzureADSyncer.exe -l warning`

## Output

Each time the app runs it will generate a new folder that will be named based upon the time at which the app began running and the word `output` in the following format: `YYYY_MM_DD_HHMMSS_output`. All files generated by the app during the run will be saved into this folder.

Example Output Folder Name:

```text
2023_03_12_205901_output
```

### Log

In addition to the information the app will log out to the console as it is running a log file will also be written to the output folder that contains information about the completed run. This log can be used to review the work done and troubleshoot any issues the app may have encountered during the run. Please note that each log event is written in [Compact Log Event Format](http://clef-json.org/). You are welcome to parse the log file in the way that best suits your needs.

Various tools are available for working with the CLEF format.

- [Analogy.LogViewer.Serilog](https://github.com/Analogy-LogViewer/Analogy.LogViewer.Serilog) - CLEF parser for Analogy Log Viewer
- [clef-tool](https://github.com/datalust/clef-tool) - a CLI application for processing CLEF files
- [Compact Log Format Viewer](https://github.com/warrenbuckley/Compact-Log-Format-Viewer) - a cross-platform viewer for CLEF files
- [Seq](https://datalust.co/seq) - import, search, analyze, and export application logs in the CLEF format
- [seqcli](https://github.com/datalust/seqcli) - pretty-print CLEF files at the command-line

Example log message:

```json
{"@t":"2023-03-13T01:56:45.2880591Z","@mt":"Starting syncer"}
{"@t":"2023-03-13T01:56:46.6855315Z","@mt":"Connected successfully to Onspring and Azure AD"}
{"@t":"2023-03-13T01:56:46.6860213Z","@mt":"Retrieving fields for Onspring Groups app"}
{"@t":"2023-03-13T01:56:47.0317460Z","@mt":"Retrieving fields for Onspring Users app"}
{"@t":"2023-03-13T01:56:47.2437652Z","@mt":"Setting default Groups field mappings"}
```

_**Note:**_ Only properties that are mapped to Onspring fields will be logged for Azure Active Directory groups and users.

## Limitations

- The app will not support deleting groups or users in Onspring. It will only support creating and updating groups and users.
- The application will only support syncing groups and users in Azure Active Directory that are in the same tenant as specified in the configuration file.
- The application will only support syncing groups and users in Onspring that are in the same Onspring instance as that identified by the configured `ApiKey`.
- The application will only support syncing groups and users between one Onspring instance and one Azure Active Directory tenant.

_**Note:**_ You can run the app multiple times with different configuration values to allow for syncing groups and users for multiple tenants into multiple instances.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE.txt) file for details

## Inspiration

[![Hack Together: Microsoft Graph and .NET](https://img.shields.io/badge/Microsoft%20-Hack--Together-orange?style=for-the-badge&logo=microsoft)](https://github.com/microsoft/hack-together)

This project was built as a submission for the [Microsoft Graph Hackathon 2023](https://devblogs.microsoft.com/dotnet/hack-together-microsoft-graph-dotnet/). The project was built using the [Microsoft Graph .NET SDK](https://github.com/microsoftgraph/msgraph-sdk-dotnet) and the [Onspring API .NET SDK](https://github.com/onspring-technologies/onspring-api-sdk). It illustrates a solution to a real world challenge that Onspring customers face when trying to find a way to leverage their existing identity management solution in Azure AD to also manage groups and users in [Onspring](https://onspring.com/).

# Tasks

## Sprint 1: Setup Project

- [ ] register an app in Azure Active Directory
- [ ] create a new .NET console app
- [ ] create a new xUnit test project
- [ ] add the Microsoft Graph SDK
- [ ] add the Onspring API SDK
  - need to use my forked version of the SDK until the PR is merged
  - [https://github.com/StevanFreeborn/onspring-api-sdk](https://github.com/StevanFreeborn/onspring-api-sdk)

## Sprint 2: Add Ability to Configure the App

- [ ] add ability to provide configuration via config file or environment variables
- [ ] add ability to specify config file location via command line argument

## Sprint 3: Add Ability to Authenticate with Azure Active Directory

- [ ] validate that the app can authenticate with Azure Active Directory
- [ ] validate the app can get data for users and groups in Azure Active Directory

## Sprint 4: Add Ability to Authenticate with Onspring

- [ ] validate that the app can authenticate with the Onspring API
- [ ] validate that the app can get data from the Onspring API

## Sprint 5: Sync Groups to Onspring with Default Properties

- [ ] add ability to sync groups from Azure Active Directory to Onspring

## Sprint 6: Sync Users to Onspring with Default Properties

- [ ] add ability to sync users from Azure Active Directory to Onspring
- [ ] add ability to sync users to groups in Onspring according to their group membership in Azure Active Directory

## Sprint 7: Sync Users to Onspring with Custom Properties

- [ ] add ability to specify custom properties to sync from Azure Active Directory to Onspring

## Sprint 8: Sync Groups to Onspring with Custom Properties

- [ ] add ability to specify custom properties to sync from Azure Active Directory to Onspring

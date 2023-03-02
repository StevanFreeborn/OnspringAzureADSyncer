# Tasks

## Sprint 1: Setup Project

- [x] register an app in Azure Active Directory
  - [x] go to azure active directory
  - [x] Add new app registration
  - [x] give it a name
  - [x] for supported account types, select "Accounts in any organizational directory (Any Azure AD directory - Multitenant) and personal Microsoft accounts (e.g. Skype, Xbox)"
  - [x] leave redirect URI blank
  - [x] click register
  - [x] go to "API Permissions" and remove the existing Delegated permission
  - [x] click "Add a permission"
  - [x] click "Microsoft Graph"
  - [x] click "Application permissions"
  - [x] Search for "Group"
  - [x] select "Group.Read.All"
  - [x] Search for "User"
  - [x] select "User.Read.All"
  - [x] click "Add permissions"
  - [x] click "Grant admin consent for [tenant name]"
  - [x] click "Certificates & secrets"
  - [x] click "New client secret"
  - [x] give it a description
  - [x] click "Add"
  - [x] copy the value of the secret
  - [x] click "Overview"
  - [x] copy the value of the Application (client) ID
- [x] Create Api Key in Onspring
  - [x] go to [https://[your onspring instance].onspring.com/admin/security/role](https://[your onspring instance].onspring.com/admin/security/role)
  - [x] click "Create Role"
  - [x] give it a name
  - [x] set status to "Enabled"
  - [x] click "App Permissions"
  - [x] search for "Users"
  - [x] give the role the following permissions:
    - [x] "Create" Content Records
    - [x] "Read" Content Records
    - [x] "Update" Content Records
  - [x] search for "Groups"
  - [x] give the role the following permissions:
    - [x] "Create" Content Records
    - [x] "Read" Content Records
    - [x] "Update" Content Records
  - [x] click "Save Record"
  - [x] go to [https://[your onspring instance].onspring.com/admin/security/apikey](https://[your onspring instance].onspring.com/admin/security/apikey)
  - [x] click "Create API Key"
  - [x] give it a Name
  - [x] set status to "Enabled"
  - [x] assign it the role you created in the previous step
  - [x] click "Save Changes"
  - [x] click on "Developer Information" tab
  - [x] copy the value of the API Key
- [x] create a new .NET console app
- [x] create a new xUnit test project
- [ ] add the Microsoft Graph SDK
- [x] add the Onspring API SDK
  - need to use my forked version of the SDK until the PR is merged
  - [https://github.com/StevanFreeborn/onspring-api-sdk](https://github.com/StevanFreeborn/onspring-api-sdk)

## Sprint 2: Add Ability to Configure the App

- [x] add ability to provide configuration via config file or environment variables
- [x] add ability to specify config file location via command line argument

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

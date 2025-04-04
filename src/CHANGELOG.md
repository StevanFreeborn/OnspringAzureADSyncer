# Change Log

All notable changes to this project will be documented in this file. See [versionize](https://github.com/versionize/versionize) for commit guidelines.

<a name="2.2.1"></a>
## [2.2.1](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/releases/tag/v2.2.1) (2025-04-01)

### Bug Fixes

* properly compare existing and new group lists ([4c4a3d7](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/4c4a3d77aa98afabdf35b472903efc19e68c92c0))

<a name="2.2.0"></a>
## [2.2.0](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/releases/tag/v2.2.0) (2025-03-20)

### Features

* avoid processing users twice ([ce3402b](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/ce3402bfc37fc85cb47f33299e58ef825aff636f))
* reduce number of requests when syncing users ([583f6a0](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/583f6a05f18fae5cddf1f892038a66807c837356))

<a name="2.1.1"></a>
## [2.1.1](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/releases/tag/v2.1.1) (2025-03-16)

### Bug Fixes

* enforce member properties are sent to MS Graph API with camelcase ([9a34aaa](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/9a34aaa6fb641247592aa763b4e0148432c7f7eb))

<a name="2.1.0"></a>
## [2.1.0](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/releases/tag/v2.1.0) (2025-03-14)

### Features

* add version to starting message ([7886e45](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/7886e45891d660fcc88ceb92b11d35c82a973792))

### Bug Fixes

* add missing whitespace ([ca8d6b3](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/ca8d6b3c3c61dd840d3f243bf0c12b4bc8515da2))

<a name="2.0.0"></a>
## [2.0.0](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/releases/tag/v2.0.0) (2025-03-13)

### Features

* accept group filter as odata $filter query string ([e901cd2](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/e901cd2e4cdd9f3c100b5c96af78caf3d34ba088))
* use group filter to retrieve groups and only retrieve members of groups included by that filter ([06d2724](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/06d2724f76a82ea5fb5323d4f77225638816387c))

### Bug Fixes

* remove group filter ([b62a9d7](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/b62a9d76a5abc5d1261b5470eaddca651ae33575))

### Breaking Changes

* accept group filter as odata $filter query string ([e901cd2](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/e901cd2e4cdd9f3c100b5c96af78caf3d34ba088))

<a name="1.4.3"></a>
## [1.4.3](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/releases/tag/v1.4.3) (2025-02-26)

### Bug Fixes

* include null properties in log statements and correct file name ([8cb8d73](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/8cb8d734adf04f6392deb689efbe89cd1749b7f7))

<a name="1.4.2"></a>
## [1.4.2](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/releases/tag/v1.4.2) (2025-02-26)

### Bug Fixes

* include group properties that are being filtered on in logs ([15c7308](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/15c7308d90697f8e6d6b1021c39bd4c08001210d))

<a name="1.4.1"></a>
## [1.4.1](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/releases/tag/v1.4.1) (2025-02-26)

### Bug Fixes

* correct destructing policies ([29c42c0](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/29c42c0dc381fbe9ea6d7870f642bb12ac8abe9f))

<a name="1.4.0"></a>
## [1.4.0](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/releases/tag/v1.4.0) (2025-02-24)

### Features

* add ability to define group filters ([9930e3a](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/9930e3a77a1afe4bc229c126babfba87ad775618))

### Bug Fixes

* ensure we fetch the data for group properties that are in the group filters array so we can property filter based on those values even if they are not specified in the mappings ([69ebcba](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/69ebcba8c8fe2074774e6d3932b39ec7e73f83e6))

<a name="1.3.0"></a>
## [1.3.0](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/releases/tag/v1.3.0) (2025-02-23)

### Features

* allow default group name mapping to be overwritten ([8084119](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/8084119eb669658298eb1e79c06040fa7d723955))

<a name="1.2.0"></a>
## [1.2.0](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/releases/tag/v1.2.0) (2025-02-23)

### Features

* make azure properties case insensitive ([025af95](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/025af95859eafb38dc3405ab9603b66254364031))

<a name="1.1.0"></a>
## [1.1.0](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/releases/tag/v1.1.0) (2025-02-21)

### Features

* upgrade to .NET 9 ([f717ab5](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/f717ab5ef346529185f6f68a72f130e9cb39964a))

### Bug Fixes

* use proper actionv version ([a4e8cd3](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/a4e8cd3b8a0d8da435b00057c165454d700e24cd))

<a name="1.0.0"></a>
## [1.0.0](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/releases/tag/v1.0.0) (2023-3-16)

### Features

* able to create new user with proper default mappings and status values ([643ba07](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/643ba07aacd1b85211a727b0e5a75610df76bd30))
* add ability to specify config file ([fc18714](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/fc187148be1912ddedee6827a275ed264bdb43be))
* add destructuring serilog policy for azure groups ([9057a88](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/9057a88e7b40af76cbf3d47a9e1f1f40304268c2))
* add initial settings models for binding user supplied configuration to ([51d3c62](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/51d3c62802c236b4172cbffd4ef626ff050e2145))
* add support for custom field mappings fields ([7acb8ad](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/7acb8ad4cffa88ee944127ead7906a27c2267d6e))
* add validation for custom field mappings ([457df00](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/457df003d5c69ec723029631892d40e8fcd8996c))
* added ability to add missing list values. fix: clean up log statements ([d746d38](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/d746d38ec72ccc05bc78df5a0f0b75bb51fcb1a3))
* added ability to create a new group in onspring ([947d55d](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/947d55d851447cd4ece79b7933e780edc4f1d1d5))
* added connecting to onspring and graph ([34c76ac](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/34c76ac1a4501b4733f985a423c3319112d50f1c))
* added default mappings for required users fields ([57c767f](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/57c767f2c0880d32fea6850b9fd8e9b97cc7b293))
* added sync groups ([a81143f](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/a81143f2bc29f0928986831443440bdd958251cc))
* added validating property type has been mapped to support onspring field ([ff583c5](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/ff583c53ec29e1fdaccc7935967126c0c75d073e))
* pulling groups from azure ad and looking up onspring groups ([3c57162](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/3c5716283c59940ac1bbb2c3e622af7faaa8a3d8))
* relate users to proper groups on creation ([de2b202](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/de2b2021da97ca5d9d5b4c6b2f5bb86862b3c788))
* stub out logic for syncing users ([54607ea](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/54607eabedc0d3fc22b8663b2710eb033d6cb3ef))
* updating users correctly ([93740ce](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/93740ce105a069f6e43a454fa82ecda5f88a7c21))

### Bug Fixes

* add named variable for page size for readability ([ae4d9e8](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/ae4d9e8383288277b8fad3c8df6ac7b5bb409bc6))
* add retry helper to onspring service ([cc3ab69](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/cc3ab69245e2a545cc69a5ea343c76c2d9494471))
* added case for int property types ([25eec82](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/25eec82a43ed9297c2b51050f93657594e60a742))
* added comparison logic for record values being null or a list ([b8ee305](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/b8ee305681bf9dd19c472bba86a8a9de7b5cdf4b))
* adding more tests ([801f90d](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/801f90d0ae82a2091620791f2fd70d56438a6162))
* allow for mocking clients in tests ([24a5206](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/24a52068ce5b1c388a730454fafd471bd68e5e65))
* convert to program.main style ([2f793c4](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/2f793c4f9fa9e9fce3f54febbee77034d0ae2987))
* correct log statements ([914ab70](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/914ab70f38cd1d2d21d2bdb88237346946a25561))
* don't skip accountEnabled mapping when building update record ([18f3e61](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/18f3e61cc2192ac10d7502189b21da382e6e5e10))
* existing unit tests that were broken in development ([b7e3c04](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/b7e3c047792690730cdcd139eb08ab00c384d2d9))
* get default mapped group field ids dynamically ([f8366ec](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/f8366ec14cb53ad8cfc0d6bb8e62d4f8410e6da7))
* issue with serilog not logging to files ([000a2b1](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/000a2b10290966f7f9679446b0d457e6a790043c))
* issue with serilog not logging to files ([c1cabb0](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/c1cabb0172282e2aaab38ef4f6c1a3299935cdc6))
* log to proper location ([53f5f16](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/53f5f16cbd1f2d440a321e9184362f4af5755449))
* not testing startup or program main method. ([f978fe8](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/f978fe85f1d03cf0c74bb8fb6f009fbec95d6de5))
* only update users status and groups when necessary ([76c6701](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/76c6701ab92c6eb286cbe09441f8df430839abfe))
* refactor to allow unit testing program and syncer class ([f9065bc](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/f9065bc12675840ca0140d63d7d84854a58bc0b0))
* refactor to sync list values when syncing each page of users or groups ([3f28c26](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/3f28c26b3a9c7e4ad67fea6b8f6577402cc4de23))
* removed unused extensions. fix: move command construction to program main ([73d79b5](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/73d79b56a227a4e0e077bece2e6ba7a0c0c2da20))
* rename build command method ([844f947](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/844f9478db98db392bdf81c8effc6d09be6042fc))
* update error message ([f98a78b](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/f98a78ba80b629e43bde3c9d3f1f7b9e8a32a3f3))
* update get groups and get group to only request mapped data ([205b314](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/205b3144caf46ccb44d49c940f451a754dd28e40))
* wrap graph service client ([8acee46](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/8acee46242d07793d1ead1f34a1f8e9e22ffda20))
* wrap startup in try catch block ([375317f](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/375317f03c801b05caa22af80c8cc3743b57af58))
* wrap startup in try catch incase exception occurrs when creating either client ([06872d4](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/06872d4986beb952aaecee1cee6071423aec1952))
* **OnspringAzureADSyncer:** refactor services configuration ([25e339a](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/25e339a6fbd5f41164b5161f9a2c5d9d82d46c96))
* **OnspringAzureADSyncer:** update access modifiers ([77f0028](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/77f002854d44063c0b3249e794e6e4e1de337d57))

### Other

* Create .github/workflows/main.yml ([60949fd](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/60949fdcae3c134658718a232d875a42f73131b9))
* Create .github/workflows/main.yml ([397a6df](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/397a6df166206cb541e042949f2b0a1177740ac3))
* initial commit ([f067c81](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/f067c81bea54b0c4f3401c7cd137216ad319bf9f))
* Merge branch 'fix/add-initial-tests' of https://github.com/StevanFreeborn/OnspringAzureADSyncer into fix/add-initial-tests ([f626083](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/f626083b1b4e182588f85a03b2f4a67978d4141b))
* Merge pull request #1 from StevanFreeborn/feat/add-config-and-auth ([23d0045](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/23d0045de0ae34830bd5105ebf05beb23a664c56))
* Merge pull request #10 from StevanFreeborn/tests/add-unit-tests ([c72faa6](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/c72faa60b36e01970296b548fccff12a293f72f9))
* Merge pull request #2 from StevanFreeborn/feat/sync-groups ([2b72a68](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/2b72a68e464129df6791916a85a4deccd4cb0913))
* Merge pull request #3 from StevanFreeborn/fix/add-initial-tests ([9b5c0e2](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/9b5c0e264aa3020a848b85ad64bb95ff21cae724))
* Merge pull request #4 from StevanFreeborn/fix/remove-serilog-console ([161705e](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/161705eca995764f40f546eae08825252d3918ca))
* Merge pull request #5 from StevanFreeborn/feat/sync-users ([bc1e14f](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/bc1e14fab67c022671361eef066ff3c0030c1c4e))
* Merge pull request #6 from StevanFreeborn/fix/refactor ([afbb7f0](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/afbb7f0f3ae3ce16ae13c11d680bcc0120f5ae13))
* Merge pull request #7 from StevanFreeborn/feat/add-validation-for-mapped-field-and-property-type ([1e7f7cd](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/1e7f7cddf92c3b1b31ab3a1193e46c6d987ea84c))
* Merge pull request #8 from StevanFreeborn/docs/update-README.md ([6e25d47](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/6e25d478fc6965ef6dc02462d9a0c8aea95680cf))
* Merge pull request #9 from StevanFreeborn/development ([b21b735](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/b21b7355ad2b3446c38c56e157cde00a0f416117))
* Update build_test.yml ([e47d5b9](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/e47d5b9c50af83c4383c4b69ac08e4ad83b28699))
* Update README.md ([538d061](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/538d061a4fc857e7cb8a8f21670e9fe25cc198ea))
* add example videos for readme ([b9367f1](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/b9367f157a457b091fc74e4596f2d38883c39203))
* add live-server command to test coverage script ([bde94ce](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/bde94ce224533c518262d43ccdf9129f5563a1e1))
* add microsoft.exensions.dependencyinjection as dependency ([341d4e8](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/341d4e857bee7876685d3c52f654d53946fb6b6d))
* add microsoft.extensions.configuration as dependency ([be47f85](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/be47f8572fae2870ddd2f66b3d3ce7429e1de873))
* add microsoft.extensions.configuration.json as dependency ([f0da65d](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/f0da65d8501a65e767519e556c71b3c4b8aab3d5))
* add microsoft.extensions.hosting as dependency ([29487ff](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/29487ffbf66d9580c269a35d4f3aad1eebae7cf5))
* add moq and fluent assertions as dependencies to test project ([9f9cce1](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/9f9cce139027fd2a80e5c39e62023370b99b8d6c))
* add more OnspringSettings tests ([420bb0b](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/420bb0b97b1630c04ee9773caa878eb43aa45201))
* add more unit tests ([ac4c31b](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/ac4c31b60a10088de5040d2ae7e715a86e151daf))
* add more unit tests ([0494d5c](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/0494d5caa4f865a0c51fc439862ca73ca68b374d))
* add more unit tests ([bf5bd4e](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/bf5bd4ee2acf3d101ecd302eada11acdcb991289))
* add notes for getting started session ([35bfe63](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/35bfe634370a58818df683320ef61d376ed0376e))
* add notes from hackathon kickoff ([8d47dd8](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/8d47dd80e3526a6464907860ed0671deba86fe21))
* add project references and add projects to solution ([d495eba](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/d495eba727fdc19802abee6ad40f65217644db59))
* add projects to solution ([9520c44](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/9520c446d5ccc7310cd576013d2350ba6316d317))
* add property to .csproj to copy development config to output when debugging ([c232f06](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/c232f062930ba803be6da9a1af862d78a566ad11))
* add publish script ([de508b5](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/de508b5c35686f431f1801f4e1253e2acc58bdc8))
* add serilog dependencies ([f0bbf50](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/f0bbf501a08a3e79dce68c23d02d7abd6dfd8649))
* add system.commandline as dependency ([8b5adac](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/8b5adaccfe26a3a8bed61bdb466a17df930da325))
* add testconfig to source control ([85fc2e8](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/85fc2e8e084cc0d19b04e461fbde34ed5d1259fe))
* add to do notes ([0604a56](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/0604a56f9262c064dec863dd28b71d4d0b7514e9))
* added 100% test coverage for OnspringService ([5f9633c](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/5f9633cbb1616e9fc6af8ccbfeee40c33b5f71c9))
* added graph sdk and azure identity as dependencies ([7ddce55](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/7ddce5575c8e73deeaa4a87f93433439157da96d))
* added link to sdk dotnet documentation ([a682734](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/a6827342a8d1b59a1316b6fb194c3075ff3f67c6))
* added more tests for processor ([4b441a7](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/4b441a7e0cdd211b832f5a389f9aee6f94b19673))
* added more unit tests ([5cb4493](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/5cb4493d1a990ce3000ee37f038ce032741779a8))
* added notes for ask_the_experts ([188c302](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/188c302a627da441e0f73ad33ec01e195a6d131b))
* added reportgenerator as tool for looking at code coverage results locally ([57ba546](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/57ba5463c78ea846f013cae04e0da4e2ce8d2fe2))
* added script for quickly create local code coverage report ([1ddd4e3](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/1ddd4e36cc00cc157d8705c437208433956c9b16))
* added string extensions unit tests ([6608f7c](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/6608f7c80a7ad243bf5b1ac14fddcfe543e63c91))
* added system.commandline.hosting as dependency ([7bde4b5](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/7bde4b54b42f459ad7988d5388d2f594a0ca968b))
* added tests for GetGroupFields method ([88a26b8](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/88a26b86ef2b98fa601a660107d3fddbeb01fb54))
* added tests for GraphService ([005357b](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/005357b5c6cd8c8f29efb5a6f9413fbc76bf7ae8))
* added vs code debug assets ([e6daa40](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/e6daa407c13eb4a8277c8099ed84bc8b986c0191))
* begin drafting readme.md ([3a3371e](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/3a3371e4dc563d0763c0f68a8eb3ddad89abf38f))
* begin working on final OnspringService unit tests ([47954f1](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/47954f1f829d528b4c1a8ba25f941970fb1468bb))
* begin writing tests for OnspringService ([aa1f40e](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/aa1f40e2c7aa06da2fb2de9f8f8408e51a7e2a0e))
* cleanup ([db356fc](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/db356fcb53b3c3bae265fe5d4b7b7cd40789945f))
* complete unit tests for GraphService and Processor ([5e97982](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/5e97982bbb92005b1e37a792e887999090bcb3e3))
* continue drafting readme.md ([cc1eac6](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/cc1eac6aed677a752896e4ec7242f98c30f9ab01))
* correct name of mocked credential token. ([a212b8d](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/a212b8d46ff3783e5320e8508de29fea3bbe3a61))
* coverage over all except processor and services ([6e790e0](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/6e790e010cf4fad92063ef9d051cfd3376c90f20))
* finished README.md ([ca339c7](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/ca339c77ad21b1fc3ae9108d372c9b0fb02540fd))
* fix bare links in tasks.md ([297f950](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/297f9500a366eb76d85ec1815df24614ddd5ba9a))
* fix mocking in syncer tests ([a7b7726](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/a7b7726c53c4c4e7c6600206dd584f55ef155ae7))
* initial project setups ([e2208f0](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/e2208f0a6f2e11bb210228a7c5fe12830bda38c2))
* more unit tests ([1dff415](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/1dff4155c3fbb26f446f7ac815bd4b51ea8613f9))
* more unit tests ([7b9b452](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/7b9b4520bb6bd4eb615c3159832cf287e6a93249))
* more unit tests ([c6841a9](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/c6841a941a7811f2c70ef4baf1b24a6596299958))
* more unit tests ([9218806](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/9218806a86d01f50fc934658e57e490fd6aba426))
* more unit tests for Processor.cs ([75a37e3](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/75a37e3cc7bde6693d6479e6566cbc10a273844f))
* organize cs.proj groups ([35aba04](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/35aba04626f16cc0ae022d9fbdefe9c9a8c1cf4a))
* reached test coverage goal ([c0cd4bf](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/c0cd4bfe218f52b5ce18eb39346e7cbbb24877ec))
* remove unused usings ([7755993](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/77559936dd7a568f94a83e9a128d1b72317e5c32))
* reset up tests project. chore: update editor config ([1610676](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/16106763d57f0116af5cb32fe5162e51b46c70ed))
* results of running dotnet format ([d7633b6](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/d7633b67c2105ee5df26341d45ababa2e3db43dc))
* run dotnet format ([da2988f](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/da2988fe4fd6d384c5b60d6c6b44acaa8c192c57))
* setup git module using forked version of Onspring API SDK ([9caf00b](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/9caf00b23d0536722c5c7f468192a2c9989fb64c))
* small changes ([b2fc1df](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/b2fc1df6a6ce89859c860940c46ed7854b55d8e1))
* small formatting change ([1d905cb](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/1d905cb99553231a7b094bb392a479b18d3da1e0))
* stub out tests for SyncGroups method ([34535bb](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/34535bb034b14f173b0ed61c955d391e9f04f81d))
* troubleshooting ([4d761db](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/4d761dbbabc844b061cf65c7415e403a21e4c482))
* update editorconfig. ([783c623](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/783c6237e7d91a017dd2e4a28b6786102c4fb220))
* update git ([3277a8e](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/3277a8e115357bc8d81f98cb185e3ea9d9a8d57c))
* update gitignore to exclude test config file for app ([e3b7181](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/e3b718155762962e87bd962b01bd285114dd93db))
* update method name ([39178df](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/39178df2db5afd0870a11633c43117cf4174ea12))
* update punchlist ([af8b99f](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/af8b99f02517407748dad4c04d683a499e7455ea))
* update punchlist ([98569ec](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/98569ecc58baebec9907d7bf628e6580cb647c97))
* update punchlist ([2d912d9](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/2d912d9bfac63734c6acae4a6c5618fb8f026e55))
* update readme ([e4713ce](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/e4713cef9e541f15da202cfb6bb257d82ec1196a))
* update readme.md ([e865d0d](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/e865d0d8fc0f2b507567718ee74b77f7e83e78e3))
* update task list and add individual steps for setting things up in Azure and Onspring ([4173fd9](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/4173fd908fc2e03bf08f98035d90e50e0eafa6b6))
* update tasks.md ([88663c2](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/88663c241cc4633c3d9698bfeffec1a03e80d6bb))
* updated example config with custom field mappings ([8843597](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/8843597ca3511e3f4c06ea8e6d1c25d14651ef9e))
* updated example config with required onspring user property mappings ([219fdba](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/219fdba82366d4cde20acd36b47438054ca4f2bf))
* updated planning doc and added initial task plan ([0593aed](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/0593aed122aeb086700b5c63cae93f836dbfe1cb))
* updated punchlist ([beeec2f](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/beeec2f8298a3278dd0caf754a88a19e4aabb9a5))
* updated punchlist ([e0f9570](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/e0f9570e8f00355dd39f649468c52e3de04d1ea2))
* updated punchlist ([cb1baa8](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/cb1baa845c5c474155c2a00e8e05d5d5079f16c3))
* updated punchlist ([7d7bb7d](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/7d7bb7de46c4bd64171f4ba24400f7f8936456d7))
* **OnspringAzureAdSyncer:** add system.commandline.hosting as dependency ([6864fcb](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/6864fcb0a5acb0f66973d523784538ad9e204c24))
* **OnspringAzureADSyncer:** make internals visible to test project ([641049a](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/641049ac5344cf7b41a3f77d4f33962089338cf1))
* **tests:** add initial unit tests for syncer ([1acae14](https://www.github.com/StevanFreeborn/OnspringAzureADSyncer/commit/1acae14531844b5a6bda90bf8df35d8df1b594ac))


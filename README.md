eCrypt
=============
eCrypt is command line tool for creation of self-extractable encrypted package as executable file.
Also it contains application with user interface that allows to generate public and private keys.

Artifacts contains few files:

1. `eVision.eCrypt.exe` - command line tool
2. `eVision.KeyGenerator.exe` - public/private key generation tool
3. `eVision.eCrypt.<some version>.nupkg` - nuget package

Usage
-------

1. Compile the solution using build.ps1 (folder "build_output\artifacts") or download "eVision.eCrypt" nuget package from nuget.org
2. Run console app `eVision.eCrypt.exe` with parameters or use user interface to generate public and private keys `eVision.KeyGenerator.exe`


Documentation
-------

Documentation is available within tool

`eVision.eCrypt.exe --help`

Public and private key can be generated using UI tool. Pulic key is used for encryption and private key is used for decryption.

Example:
You can create self-extractable encrypted package ("MySelfExtractablePackage.exe") using public key ("publick-key.asc") and package ("package-1.0.0.zip").

`...eCrypt.exe --output-path=C:\Destination\MySelfExtractablePackage.exe --key-path=C:\Keys\publick-key.asc --target=C:\SomePackage\package-1.0.0.zip`


Self extractor has following interface:

![alt text](https://raw.githubusercontent.com/eVision-oss/e-crypt/master/docs/img/self_extracted_package.png "Self extracted package")

Key generator user interface:

![alt text](https://raw.githubusercontent.com/eVision-oss/e-crypt/master/docs/img/key_generator.png "Key generator")

Maintainers
-------

(@StefanWink, @antondimitrov, @WingTai, @Eugene1982, @iarovyi, @alex-shmyga).
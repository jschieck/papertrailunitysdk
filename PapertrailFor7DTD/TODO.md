1. Download the [latest release](https://github.com/jschieck/papertrailunitysdk/raw/develop/release/papertrail-sdk.unitypackage) of the SDK.
1. Import the Papertrail SDK into the project by navigating to Assets/Import Package/Custom Package… and select the downloaded .unitypackage file.

![Import Package](https://github.com/jschieck/papertrailunitysdk/blob/docs/assets/img/import_package.png?raw=true)

3. Open the Project View and navigate to the Papertrail/Resources folder and locate the PapertrailSettings asset file.

![Settings Asset](https://github.com/jschieck/papertrailunitysdk/blob/docs/assets/img/settings.png?raw=true)

4. Log into the Papertrail dashboard by pressing the Open Dashboard button or visiting <https://papertrailapp.com/dashboard>.
5. Locate the Add Systems button in the upper right.

![Add System](https://github.com/jschieck/papertrailunitysdk/blob/docs/assets/img/addsystem.png?raw=true)

6. After entering the Add Systems section, locate the hostname and port at the top of the page, directly under Setup Logging.

![Hostname](https://github.com/jschieck/papertrailunitysdk/blob/docs/assets/img/hostname.png?raw=true)

7. Copy the information into the PapertrailSettings configuration.
8. Papertrail logging is now set up. Any logs made using Unity’s Debug logging system will be automatically forwarded to Papertrail.
9. Return to the Papertrail Dashboard and locate the system by searching for the name entered in the PapertrailSettings asset.

![Search](https://github.com/jschieck/papertrailunitysdk/blob/docs/assets/img/search.png?raw=true)

10. The logs will now appear as they happen in the Events view of the system.

![Logs](https://github.com/jschieck/papertrailunitysdk/blob/docs/assets/img/logging.gif?raw=true)
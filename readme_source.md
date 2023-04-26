## Use Cases Supported and Limitations  

The Fortigate Orchestrator Extension supports the following use cases:
1. Inventory of local user and factory cerificates
2. Ability to add new local certificates
3. Ability to renew **unbound** local user certificates
4. Ability to delete **unbound** local user certificates

The Fortigate Orchestrator Extension DOES NOT support the following use cases:
1. The renewal or removal of certificates enrolled through the internal Fortigate CA.
2. The renewal or removal of factory certificates
3. The renewal or removal of ANY certificate bound to a Fortigate object

## Fortigate Version Supported  

The Fortigate Orchestrator Extension was tested using Fortigate, version 7.2.4  

## Fortigate Orchestrator Extension Versioning  

The version number of a the Fortigate Orchestrator Extension can be verified by right clicking on the Fortigate.dll file in the Extensions/Fortigate installation folder, selecting Properties, and then clicking on the Details tab.  

## Fortigate Orchestrator Extension Installation  

1. Create the Fortigate certificate store type.  This can be done either: a) using the Keyfactor Command UI to manually set up the certificate store type (please refer to the Keyfactor Command Reference Guide for more information on how to do this), or b) by using the Keyfactor Command API to automate the creation of the store type.  Please see the provided CURL script [here](Certificate%20Store%20Type%20CURL%20Script/Fortigate.curl).  A detailed description of how the Fortigate certificate store type should be configured can be found in the Fortigate Certificate Store Type section below.
2. Stop the Keyfactor Orchestrator Service on the server hosting the Keyfactor Orchestrator.
3. In the Keyfactor Orchestrator installation folder (by convention usually C:\Program Files\Keyfactor\Keyfactor Orchestrator), find the "extensions" folder. Underneath that, create a new folder.  The name doesn't matter, but something descriptive like "Fortigate" would probably be best.
4. Download the latest version of the Fortigate Orchestrator from [GitHub](https://github.com/Keyfactor/fortigate-orchestrator).
5. Copy the contents of the download installation zip file to the folder created in step 1.
6. Start the Keyfactor Orchestrator Service on the server hosting the Keyfactor Orchestrator.  

## Fortigate Setup  

The Fortigate Orchestrator Extension requires an API token be created in the Fortigate environment being managed.  Please review the following [instructions](https://docs.fortinet.com/document/forticonverter/7.0.1/online-help/866905/connect-fortigate-device-via-api-token) for creating an API token to be used in this integration.  

## Certificate Store Type Settings  

Below are the values you need to enter if you choose to manually create the Fortigate certificate store type in the Keyfactor Command UI (related to Step 1 of Fortigate Orchestrator Extension Installation above).  

*Basic Tab:*
- **Name** – Required. The display name you wish to use for the new certificate store type.  Suggested value - Fortigate
- **ShortName** - Required. Suggested value - Fortigate.  If you choose to use a different value, you will need to modify the manifest.json file accordingly.
- **Custom Capability** - Unchecked
- **Supported Job Types** - Inventory, Add, and Remove should all be checked.
- **Needs Server** - Unchecked
- **Blueprint Allowed** - Checked if you wish to make use of blueprinting.  Please refer to the Keyfactor Command Reference Guide for more details on this feature.
- **Uses PoserShell** - Unchecked
- **Requires Store Password** - Checked.
- **Supports Entry Password** - Unchecked.  

*Advanced Tab:*  
- **Store Path Type** - Freeform
- **Supports Custom Alias** - Required
- **Private Key Handling** - Required
- **PFX Password Style** - Default  

*Custom Fields Tab:*
None

*Entry Parameters:*
None
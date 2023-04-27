# Fortigate

This integration is used to inventory and manage certificates in Fortigate.

#### Integration status: Production - Ready for use in production environments.

## About the Keyfactor Universal Orchestrator Extension

This repository contains a Universal Orchestrator Extension which is a plugin to the Keyfactor Universal Orchestrator. Within the Keyfactor Platform, Orchestrators are used to manage “certificate stores” &mdash; collections of certificates and roots of trust that are found within and used by various applications.

The Universal Orchestrator is part of the Keyfactor software distribution and is available via the Keyfactor customer portal. For general instructions on installing Extensions, see the “Keyfactor Command Orchestrator Installation and Configuration Guide” section of the Keyfactor documentation. For configuration details of this specific Extension see below in this readme.

The Universal Orchestrator is the successor to the Windows Orchestrator. This Orchestrator Extension plugin only works with the Universal Orchestrator and does not work with the Windows Orchestrator.



## Support for Fortigate

Fortigate is open source and community supported, meaning that there is **no SLA** applicable for these tools.

###### To report a problem or suggest a new feature, use the **[Issues](../../issues)** tab. If you want to contribute actual bug fixes or proposed enhancements, use the **[Pull requests](../../pulls)** tab.
___



---




## Keyfactor Version Supported

The minimum version of the Keyfactor Universal Orchestrator Framework needed to run this version of the extension is 10.1

## Platform Specific Notes

The Keyfactor Universal Orchestrator may be installed on either Windows or Linux based platforms. The certificate operations supported by a capability may vary based what platform the capability is installed on. The table below indicates what capabilities are supported based on which platform the encompassing Universal Orchestrator is running.
| Operation | Win | Linux |
|-----|-----|------|
|Supports Management Add|&check; |&check; |
|Supports Management Remove|&check; |&check; |
|Supports Create Store|  |  |
|Supports Discovery|  |  |
|Supports Renrollment|  |  |
|Supports Inventory|&check; |&check; |


## PAM Integration

This orchestrator extension has the ability to connect to a variety of supported PAM providers to allow for the retrieval of various client hosted secrets right from the orchestrator server itself.  This eliminates the need to set up the PAM integration on Keyfactor Command which may be in an environment that the client does not want to have access to their PAM provider.

The secrets that this orchestrator extension supports for use with a PAM Provider are:

|Name|Description|
|----|-----------|
|StorePassword|The Fortigate API Access Token used to execute Fortigate API requests|
  

It is not necessary to use a PAM Provider for all of the secrets available above. If a PAM Provider should not be used, simply enter in the actual value to be used, as normal.

If a PAM Provider will be used for one of the fields above, start by referencing the [Keyfactor Integration Catalog](https://keyfactor.github.io/integrations-catalog/content/pam). The GitHub repo for the PAM Provider to be used contains important information such as the format of the `json` needed. What follows is an example but does not reflect the `json` values for all PAM Providers as they have different "instance" and "initialization" parameter names and values.

### Example PAM Provider Setup

To use a PAM Provider to resolve a field, in this example the __Server Password__ will be resolved by the `Hashicorp-Vault` provider, first install the PAM Provider extension from the [Keyfactor Integration Catalog](https://keyfactor.github.io/integrations-catalog/content/pam) on the Universal Orchestrator.

Next, complete configuration of the PAM Provider on the UO by editing the `manifest.json` of the __PAM Provider__ (e.g. located at extensions/Hashicorp-Vault/manifest.json). The "initialization" parameters need to be entered here:

~~~ json
  "Keyfactor:PAMProviders:Hashicorp-Vault:InitializationInfo": {
    "Host": "http://127.0.0.1:8200",
    "Path": "v1/secret/data",
    "Token": "xxxxxx"
  }
~~~

After these values are entered, the Orchestrator needs to be restarted to pick up the configuration. Now the PAM Provider can be used on other Orchestrator Extensions.

### Use the PAM Provider
With the PAM Provider configured as an extenion on the UO, a `json` object can be passed instead of an actual value to resolve the field with a PAM Provider. Consult the [Keyfactor Integration Catalog](https://keyfactor.github.io/integrations-catalog/content/pam) for the specific format of the `json` object.

To have the __Server Password__ field resolved by the `Hashicorp-Vault` provider, the corresponding `json` object from the `Hashicorp-Vault` extension needs to be copied and filed in with the correct information:

~~~ json
{"Secret":"my-kv-secret","Key":"myServerPassword"}
~~~

This text would be entered in as the value for the __Server Password__, instead of entering in the actual password. The Orchestrator will attempt to use the PAM Provider to retrieve the __Server Password__. If PAM should not be used, just directly enter in the value for the field.




---


## Use Cases Supported and Limitations  

The Fortigate Orchestrator Extension supports the following use cases:
1. Inventory of local user and factory cerificates
2. Ability to add new local certificates
3. Ability to renew **unbound** local user certificates
4. Ability to delete **unbound** local user certificates

The Fortigate Orchestrator Extension DOES NOT support the following use cases:
1. The renewal or removal of certificates enrolled through the internal Fortigate CA
2. The renewal or removal of factory certificates
3. The renewal or removal of ANY certificate bound to a Fortigate object
4. Certificate enrollment using the internal Fortigate CA (Keyfactor's "reenrollment" or "on device key generation use case)

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

## Certificate Store Setup  

Please refer to the Keyfactor Command Reference Guide for information on creating certificate stores in Keyfactor Command.  However, there are a few fields that are important to highlight here:
- Category - Select "Fortigate" or whatever ShortName you chose for the store type.  
- Client Machine - The IP address or DNS for your Fortigate server.  
- Store Path - This is not used in this integration, but is a required field in the UI.  Just enter any value here.  
- Password - Click the button here and enter the Fortigate API Token you previously set up (See Fortigate Setup earlier in this README).


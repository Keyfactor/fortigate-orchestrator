<h1 align="center" style="border-bottom: none">
    Fortigate Universal Orchestrator Extension
</h1>

<p align="center">
  <!-- Badges -->
<img src="https://img.shields.io/badge/integration_status-production-3D1973?style=flat-square" alt="Integration Status: production" />
<a href="https://github.com/Keyfactor/fortigate-orchestrator/releases"><img src="https://img.shields.io/github/v/release/Keyfactor/fortigate-orchestrator?style=flat-square" alt="Release" /></a>
<img src="https://img.shields.io/github/issues/Keyfactor/fortigate-orchestrator?style=flat-square" alt="Issues" />
<img src="https://img.shields.io/github/downloads/Keyfactor/fortigate-orchestrator/total?style=flat-square&label=downloads&color=28B905" alt="GitHub Downloads (all assets, all releases)" />
</p>

<p align="center">
  <!-- TOC -->
  <a href="#support">
    <b>Support</b>
  </a>
  ·
  <a href="#installation">
    <b>Installation</b>
  </a>
  ·
  <a href="#license">
    <b>License</b>
  </a>
  ·
  <a href="https://github.com/orgs/Keyfactor/repositories?q=orchestrator">
    <b>Related Integrations</b>
  </a>
</p>


## Overview

The Fortigate Universal Orchestrator extension enables remote management of cryptographic certificates on Fortigate devices via Keyfactor Command. Fortigate devices use certificates to secure communications, authenticate users, and enforce security policies across networks.

In the context of this extension, defined Certificate Stores represent specific locations on the Fortigate device where certificates are stored and managed. These locations can include local user certificates, factory certificates, and other defined repositories. The Universal Orchestrator extension facilitates inventory, addition, renewal, and removal of certificates within these stores, streamlining the certificate management process for IT administrators.

## Compatibility

This integration is compatible with Keyfactor Universal Orchestrator version 10.1 and later.

## Support
The Fortigate Universal Orchestrator extension is supported by Keyfactor for Keyfactor customers. If you have a support issue, please open a support ticket with your Keyfactor representative. If you have a support issue, please open a support ticket via the Keyfactor Support Portal at https://support.keyfactor.com. 
 
> To report a problem or suggest a new feature, use the **[Issues](../../issues)** tab. If you want to contribute actual bug fixes or proposed enhancements, use the **[Pull requests](../../pulls)** tab.

## Installation
Before installing the Fortigate Universal Orchestrator extension, it's recommended to install [kfutil](https://github.com/Keyfactor/kfutil). Kfutil is a command-line tool that simplifies the process of creating store types, installing extensions, and instantiating certificate stores in Keyfactor Command.


1. Follow the [requirements section](docs/fortigate.md#requirements) to configure a Service Account and grant necessary API permissions.

    <details><summary>Requirements</summary>

    ### Fortigate Setup

    The Fortigate Orchestrator Extension requires an API token be created in the Fortigate environment being managed.  Please review the following [instructions](https://docs.fortinet.com/document/forticonverter/7.0.1/online-help/866905/connect-fortigate-device-via-api-token) for creating an API token to be used in this integration.

    ### Fortigate Version Supported

    The Fortigate Orchestrator Extension was tested using Fortigate, version 7.2.4



    </details>

2. Create Certificate Store Types for the Fortigate Orchestrator extension. 

    * **Using kfutil**:

        ```shell
        # Fortigate
        kfutil store-types create Fortigate
        ```

    * **Manually**:
        * [Fortigate](docs/fortigate.md#certificate-store-type-configuration)

3. Install the Fortigate Universal Orchestrator extension.
    
    * **Using kfutil**: On the server that that hosts the Universal Orchestrator, run the following command:

        ```shell
        # Windows Server
        kfutil orchestrator extension -e fortigate-orchestrator@latest --out "C:\Program Files\Keyfactor\Keyfactor Orchestrator\extensions"

        # Linux
        kfutil orchestrator extension -e fortigate-orchestrator@latest --out "/opt/keyfactor/orchestrator/extensions"
        ```

    * **Manually**: Follow the [official Command documentation](https://software.keyfactor.com/Core-OnPrem/Current/Content/InstallingAgents/NetCoreOrchestrator/CustomExtensions.htm?Highlight=extensions) to install the latest [Fortigate Universal Orchestrator extension](https://github.com/Keyfactor/fortigate-orchestrator/releases/latest).

4. Create new certificate stores in Keyfactor Command for the Sample Universal Orchestrator extension.

    * [Fortigate](docs/fortigate.md#certificate-store-configuration)



## License

Apache License 2.0, see [LICENSE](LICENSE).

## Related Integrations

See all [Keyfactor Universal Orchestrator extensions](https://github.com/orgs/Keyfactor/repositories?q=orchestrator).
## Overview

The Fortigate Certificate Store Type in the Keyfactor Command platform facilitates the management of certificates specifically for Fortigate devices. This store type is designed to represent various locations within Fortigate's environment where certificates are stored and managed. It includes repositories for local user certificates and factory certificates, among others. By leveraging this Certificate Store Type, administrators can perform critical functions like inventory, addition, and removal of certificates efficiently.

There are a few important caveats to keep in mind when utilizing the Fortigate Certificate Store Type. Notably, it does not support the renewal or removal of certificates enrolled through Fortigate's internal CA, nor does it support factory certificates or any certificates bound to a Fortigate object. Additionally, this store type does not utilize an SDK.

One significant limitation is the inability to perform certificate enrollments using Fortigate's internal CA, which means Keyfactor's 'reenrollment' or 'on-device key generation' functionalities are not applicable. Users should note these constraints to avoid confusion and ensure smooth operation within the supported capabilities.

## Requirements

### Fortigate Setup

The Fortigate Orchestrator Extension requires an API token be created in the Fortigate environment being managed.  Please review the following [instructions](https://docs.fortinet.com/document/forticonverter/7.0.1/online-help/866905/connect-fortigate-device-via-api-token) for creating an API token to be used in this integration.

### Fortigate Version Supported

The Fortigate Orchestrator Extension was tested using Fortigate, version 7.2.4


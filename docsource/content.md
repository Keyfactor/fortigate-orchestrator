## Overview

The Fortigate Orchestrator Extension supports the following use cases:
1. Inventory of local user and factory cerificates
2. Ability to add new local certificates
3. Ability to renew **unbound** local user certificates
4. Ability to delete **unbound** local user certificates

The Fortigate Orchestrator Extension DOES NOT support the following use cases:
1. The renewal or removal of certificates enrolled through the internal Fortigate CA
2. The renewal or removal of factory certificates
3. The renewal or removal of ANY certificate bound to a Fortigate object
4. Certificate enrollment using the internal Fortigate CA (Keyfactor's "reenrollment" or "on device key generation" use case)


## Requirements

The Fortigate Orchestrator Extension requires an API token be created in the Fortigate environment being managed.  Please review the following [instructions](https://docs.fortinet.com/document/forticonverter/7.0.1/online-help/866905/connect-fortigate-device-via-api-token) for creating an API token to be used in this integration. 


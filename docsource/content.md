## Overview

The Fortigate Orchestrator Extension supports the following use cases:
1. Inventory of local user and factory cerificates
2. Ability to add new local certificates
3. Ability to replace bound* and unbound local user certificates (usually after renewal in Keyfactor Command)
4. Ability to delete **unbound** local user certificates

The Fortigate Orchestrator Extension DOES NOT support the following use cases:
1. The renewal or removal of certificates enrolled through the internal Fortigate CA
2. The renewal or removal of factory certificates
3. The removal of ANY certificate bound to a Fortigate object
4. Certificate enrollment using the internal Fortigate CA (Keyfactor's "reenrollment" or "on device key generation" use case)

\* Because the Fortigate API does not allow for updating certificates in place, and to avoid temporary outages, when replacing local certificates that are bound, it is necessary to create a new name (alias) for the certificate.  The new name is created using the first 8 characters of the previous name (larger names truncated due to Fortigate name length constraints) allong with a suffix comprised of "--" and a 15 character hash of the current date/time.  The replaced certificate with the old name is then removed from the Fortigate instance.  For example, a bound certificate with the name "CertName" would be replaced and the name would then be "CertName--8DD76A97A98E4C1".  The existing bindings would remain in place with the new name.  At no point during the management job would any of the bound objects be left without a valid certificate binding.


## Requirements

The Fortigate Orchestrator Extension requires an API token be created in the Fortigate environment being managed.  Please review the following [instructions](https://docs.fortinet.com/document/forticonverter/7.0.1/online-help/866905/connect-fortigate-device-via-api-token) for creating an API token to be used in this integration. 


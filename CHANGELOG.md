v1.4.2
- Bug Fix: Fixed issue where inability to retreive a certificate was ending the entire job instead of producing a log entry but continuing on and ending with a warning status.

v1.4.1
- Administrative modification

v1.4.0
- Add ability to manage custom VDOMs.  PLEASE NOTE this release contains a breaking change.  Store Path MUST contain the value for the VDOM the certificate will be managing.  `root` must be entered to manage the default 'root' VDOM.

v1.3.0
- Add support for renewing certificate bound to the HTTPS server

v1.2.0
- Allow for the management (renew/replace) of bound certificates

v1.1.0
- Modified Fortigate authentication to use Authorization Bearer Token header rather than query string
- Logging improvements
- Upgraded README to use doctool

v1.0.0
- Initial Version

> [!NOTE]
> Welcome to the new home of **Eclipse AASX Package Explorer**!
> See [CONTRIBUTING](CONTRIBUTING.md) for details on how to migrate your development.

> [!IMPORTANT]
> Current development of AASX Package Explorer only supports AAS V3.  
> If you need to view AAS V2 files, see the branch [here](https://github.com/eclipse-aaspe/aaspe/tree/V2).

# Eclipse AASX Package Explorer™

![Build-test-inspect](
https://github.com/admin-shell-io/aasx-package-explorer/actions/workflows/build-test-inspect.yml/badge.svg
) ![Check-style](
https://github.com/admin-shell-io/aasx-package-explorer/actions/workflows/check-style.yml/badge.svg
) ![Check-commit-messages](
https://github.com/admin-shell-io/aasx-package-explorer/actions/workflows/check-commit-messages.yml/badge.svg
) ![Generate-docdev](
https://github.com/admin-shell-io/aasx-package-explorer/workflows/Generate-docdev/badge.svg
) [![Coverage Status](
https://coveralls.io/repos/github/admin-shell-io/aasx-package-explorer/badge.svg?branch=master
)](
https://coveralls.io/github/admin-shell-io/aasx-package-explorer?branch=master
)

[![TODOs](
https://admin-shell-io.github.io/aasx-package-explorer/todos/badges/TODOs.svg
)](
https://github.com/admin-shell-io/aasx-package-explorer/blob/gh-pages/todos/task-list/task-list-by-file.md
) [![BUGs](
https://admin-shell-io.github.io/aasx-package-explorer/todos/badges/BUGs.svg
)](
https://github.com/admin-shell-io/aasx-package-explorer/blob/gh-pages/todos/task-list/task-list-by-file.md
) [![HACKs](
https://admin-shell-io.github.io/aasx-package-explorer/todos/badges/HACKs.svg
)](
https://github.com/admin-shell-io/aasx-package-explorer/blob/gh-pages/todos/task-list/task-list-by-file.md
)

Eclipse AASX Package Explorer™ is a C# based viewer and editor for the 
Asset Administration Shell.

![screenshot](
https://github.com/admin-shell-io/aasx-package-explorer/raw/master/screenshot.png
)

To help you familiarize with the concept of Asset Administration Shell 
we provide the screencasts (both in English and German) at: 
https://admin-shell-io.com/screencasts/.

For further information about the Asset Administration Shell, see the 
publication [Details of the Asset Administration Shell](
https://www.plattform-i40.de/IP/Redaktion/EN/Standardartikel/specification-administrationshell.html
) by Plattform Industrie 4.0.

We provide a couple of sample admin shells (packaged as .aasx) for you to 
test and play with the software at:
http://www.admin-shell-io.com/samples/

## Installation

We provide the binaries for Windows 10 in [the releases](
https://github.com/admin-shell-io/aasx-package-explorer/releases). 

(Remark: In special cases you may like to use a current build.
Please click on a green check mark and select "Check-release" details.)

## Issues

If you want to request new features or report bugs, please 
[create an issue](
https://github.com/admin-shell-io/aasx-package-explorer/issues/new/choose). 

## Contributing

Please see [CONTRIBUTING](CONTRIBUTING.md) for instructions on joining the development and general contribution guidelines.

The documentation for developers is still available at [admin-shell-io](
https://admin-shell-io.github.io/aasx-package-explorer/devdoc/
) and will be migrated to eclipse-aaspe in the near future. 

## Other Open Source Implementations of AAS

At the time of this writing (2020-08-14), we are aware of the following related
implementations of asset administration shells (AAS):

* **BaSyx** (https://projects.eclipse.org/projects/technology.basyx) provides
  various modules to cover a broad scope of Industrie 4.0 (including AAS).
  Hence its substantially more complex architecture. 
  
* **PyI40AAS** (https://git.rwth-aachen.de/acplt/pyi40aas) is a Python 
  module for manipulating and validating AAS. 
  
* **SAP AAS Service** (https://github.com/SAP/i40-aas) provides a system based
  on Docker images implementing the RAMI 4.0 reference architecture (including
  AAS).

*	**NOVAAS** (https://gitlab.com/gidouninova/novaas) provides an implementation
  of the AAS concept by using JavaScript and Low-code development platform (LCDP)
  Node-Red.

* **Java Dataformat Library** (https://github.com/admin-shell-io/java-serializer)
  provides serializer and derserializer for various dataformats as well as the
  creation and validation of AAS, written in Java.

While these projects try to implement a wider scope of programatic features,
AASX Package Explorer, in contrast, is a tool with graphical user interface 
meant for experimenting and demonstrating the potential of asset administration
shells targeting tech-savvy and less technically-inclined users alike.

The AASX Package Explorer also includes an internal REST server and OPC UA
server for the loaded .AASX. Based on this a separate AASX Server is
available (https://github.com/admin-shell-io/aasx-server) which can host
several .AASX simultanously (see example https://example.admin-shell-io.com).


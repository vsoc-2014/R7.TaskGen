#!/bin/bash

# Usage:

# sudo ./uninstall.sh

PREFIX="/usr/local"

PROJECTNAME="R7.TaskGen"

PROJECTNAME_LC="${PROJECTNAME,,}"

# remove desktop file

rm -f "${PREFIX}/share/applications/${PROJECTNAME_LC}.desktop" 

# remove simlink to start script

rm -f "${PREFIX}/bin/${PROJECTNAME_LC}"

# remove application directory and all files

rm -f -r "${PREFIX}/lib/${PROJECTNAME_LC}"

echo "Uninstall of ${PROJECTNAME} complete."           
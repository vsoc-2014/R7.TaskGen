#!/bin/bash

# Usage:

# sudo ./install.sh

PREFIX="/usr/local"

PROJECTNAME="R7.TaskGen"

PROJECTNAME_LC="${PROJECTNAME,,}"

# cd to current dir

cd $(dirname ${0})

# make some dirs, if they does't where

mkdir -p "${PREFIX}/lib/${PROJECTNAME_LC}"

mkdir -p "${PREFIX}/share/applications"

# install project to application directory

cp -r -f "." "${PREFIX}/lib/${PROJECTNAME_LC}"

# make symlink to start script in prefix/bin

ln -s "${PREFIX}/lib/${PROJECTNAME_LC}/run.sh" "${PREFIX}/bin/${PROJECTNAME_LC}"

# install desktop file

cp -r -f "${PROJECTNAME_LC}.desktop" "${PREFIX}/share/applications"

echo "Installation of ${PROJECTNAME} complete."

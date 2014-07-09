#!/bin/bash

# Usage:

# sudo ./install.sh

PREFIX="/usr/local"

PROJECTNAME="R7.TaskGen"

PROJECTNAME_LC="${PROJECTNAME,,}"

# cd to current dir

cd $(dirname ${0})

# make some dirs, if they does't where

sudo mkdir -p "${PREFIX}/lib/${PROJECTNAME_LC}"

sudo mkdir -p "${PREFIX}/share/applications"

mkdir -p "${HOME}/.config/${PROJECTNAME_LC}"

cp -r -f "./App_Data/taskgen.sqlite" "${HOME}/.config/${PROJECTNAME_LC}"

# install project to application directory

sudo cp -r -f "." "${PREFIX}/lib/${PROJECTNAME_LC}"

cp -r -f "./${PROJECTNAME}.exe.config" "${HOME}/.config/${PROJECTNAME_LC}/user.config"

sudo cp -r -f "./${PROJECTNAME_LC}.svg" "/usr/share/pixmaps"

# make symlink to start script in prefix/bin

sudo ln -s "${PREFIX}/lib/${PROJECTNAME_LC}/run.sh" "${PREFIX}/bin/${PROJECTNAME_LC}"

# install desktop file

sudo cp -r -f "${PROJECTNAME_LC}.desktop" "${PREFIX}/share/applications"

echo "Installation of ${PROJECTNAME} complete."

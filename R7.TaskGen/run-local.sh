#!/bin/bash

# Usage:

# ./run-local.sh

PROJECTNAME="R7.TaskGen"

cd $(dirname $0)

exec mono "./${PROJECTNAME}.exe" "$@"

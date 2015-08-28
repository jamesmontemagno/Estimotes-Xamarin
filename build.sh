#!/bin/bash
###############################################################
# This is the Cake bootstrapper script that is responsible for 
# downloading Cake and all specified tools from NuGet.
###############################################################

# Define directories.
SCRIPT_DIR=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )
TOOLS_DIR=$SCRIPT_DIR/tools
NUGET_EXE=$TOOLS_DIR/nuget.exe
CAKE_EXE=$TOOLS_DIR/Cake/Cake.exe

# Define default arguments.
SCRIPT="build.cake"
TARGET="Default"
CONFIGURATION="Release"
VERBOSITY="verbose"
DRYRUN=false
SHOW_VERSION=false

# Make dir for tools if it doesn't exist
mkdir -p $TOOLS_DIR

# Parse arguments.
for i in "$@"; do
    case $1 in 
        -s=*|--SCRIPT=*) SCRIPT="${i#*=}" ;;
        -t=*|--TARGET=*) TARGET="${i#*=}" ;;
        -c=*|--CONFIGURATION=*) CONFIGURATION="${i#*=}" ;;
        -v=*|--VERBOSITY=*) VERBOSITY="${i#*=}" ;;
        --version) SHOW_VERSION=true ;;
        -d|--DRYRUN) DRYRUN=true ;;
    esac
    shift
done

# Download NuGet if it does not exist.
if [ ! -f $NUGET_EXE ]; then
    echo "Downloading NuGet..."
    curl -Lsfo $NUGET_EXE https://www.nuget.org/nuget.exe
    if [ $? -ne 0 ]; then
        echo "An error occured while downloading nuget.exe."
        exit 1
    fi
fi

# Restore tools from NuGet.
pushd $TOOLS_DIR >/dev/null
mono $NUGET_EXE install -ExcludeVersion
popd >/dev/null

# Make sure that Cake has been installed.
if [ ! -f $CAKE_EXE ]; then
    echo "Could not find Cake.exe."
    exit 1
fi

# Start Cake
if $SHOW_VERSION; then
    mono $CAKE_EXE -version
elif $DRYRUN; then
    mono $CAKE_EXE -verbosity=$VERBOSITY -configuration=$CONFIGURATION -target=$TARGET -dryrun
else
    mono $CAKE_EXE -verbosity=$VERBOSITY -configuration=$CONFIGURATION -target=$TARGET
fi
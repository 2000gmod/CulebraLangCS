#!/usr/bin/env python3

import os

def checked(cmd, skip=False) :
    a = os.system(cmd)
    if a != 0 and skip:
        print(f"Had error at command {cmd}")
        exit(a)

checked("dotnet tool uninstall -g CulebraLang", True)
checked("dotnet build")
checked("dotnet tool install --global --add-source ./nupkg CulebraLang")
#!/usr/bin/env python3

import os

def checked(cmd, skip=False) :
    a = os.system(cmd)
    print("-"*80)
    if a != 0:
        print(f"Had error at command {cmd}")
        if not skip:
            exit(a)

checked("dotnet tool uninstall -g CulebraLang", True)
checked("dotnet build")
checked("dotnet tool install --global --add-source ./nupkg CulebraLang")
// Checkings.cpp : main project file.

#include "stdafx.h"
#include <Windows.h>
#include <winsvc.h>

using namespace System;

int main(array<System::String ^> ^args)
{
	Console::WriteLine(L"Size: " + sizeof(BOOLEAN));
    return 0;
}

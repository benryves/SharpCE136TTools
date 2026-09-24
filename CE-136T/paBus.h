#pragma once

#include <Windows.h>

__declspec(dllexport) BOOL paBusOpen(LPCTSTR portName);
__declspec(dllexport) void paBusClose(void);

__declspec(dllexport) BOOL paBusIsOpen(void);

__declspec(dllexport) BOOL paBusReadByte(BYTE *value);
__declspec(dllexport) DWORD paBusReadBytes(BYTE *buffer, DWORD offset, DWORD length, DWORD timeout);

__declspec(dllexport) BOOL paBusWriteByte(BYTE value);
__declspec(dllexport) DWORD paBusWriteBytes(BYTE *buffer, DWORD offset, DWORD length, DWORD timeout);

__declspec(dllexport) BOOL paBusEnd(void);
__declspec(dllexport) BOOL paBusIsEnded(BOOL *ended);

#include "paBus.h"

HANDLE paBusPort = NULL;

LARGE_INTEGER qpFrequency = { 0, };
LARGE_INTEGER qpTimeout = { 0, };

BOOL paBusReading = FALSE;
BOOL paBusWriting = FALSE;

static void paBusResetTimeout(DWORD microseconds) {
	LARGE_INTEGER qpCurrent = { 0, };
	QueryPerformanceCounter(&qpCurrent);
	qpTimeout.QuadPart = qpCurrent.QuadPart + (microseconds * qpFrequency.QuadPart) / 1000000;
}

static BOOL paBusTimedOut(void) {
	LARGE_INTEGER qpCurrent = { 0, };
	QueryPerformanceCounter(&qpCurrent);
	return qpCurrent.QuadPart >= qpTimeout.QuadPart;
}

void paBusDelay(DWORD microseconds) {
	paBusResetTimeout(microseconds);
	while (!paBusTimedOut());
}

static void paBusSetSO(BOOL level) {
	if (level) {
		EscapeCommFunction(paBusPort, CLRRTS);
	} else {
		EscapeCommFunction(paBusPort, SETRTS);
	}
}

static BOOL paBusGetSI(void) {
	DWORD modemStat;
	GetCommModemStatus(paBusPort, &modemStat);
	return (modemStat & MS_DSR_ON) == 0;
}

BOOL paBusEnd(void) {

	// Check port is open
	if (!paBusPort) return FALSE;

	// Wait for the line to go back low naturally
	paBusResetTimeout(40000);
	while (paBusGetSI() && !paBusTimedOut());

	// Drive our end of the line low
	paBusSetSO(0);

	// Ensure the bus has gone back idle
	paBusResetTimeout(40000);
	while (paBusGetSI() && !paBusTimedOut());

	paBusReading = FALSE;
	paBusWriting = FALSE;

	return TRUE;

}

BOOL paBusReadByte(BYTE *value) {

	// Check port is open
	if (!paBusPort) return FALSE;

	// We're not writing any more
	paBusWriting = FALSE;

	*value = 0;

	if (!paBusReading) {

		// Wait for the line to go high
		paBusResetTimeout(40000);
		while (!paBusGetSI()) {
			if (paBusTimedOut()) goto readTimedOut;
		}

		// Respond by driving our line high
		paBusSetSO(1);

		// We're now reading
		paBusReading = TRUE;
	}

	for (int bit = 0; bit < 8; ++bit) {

		// Wait for the line to go low again
		paBusResetTimeout(40000);
		while (paBusGetSI()) {
			if (paBusTimedOut()) goto readTimedOut;
		}

		// Respond by driving our line low
		paBusSetSO(0);

		// Sampling delay
		paBusDelay(300);

		// Sample the bit
		*value <<= 1;
		if (paBusGetSI()) {
			*value |= 1;
		}

		// Acknowledge by driving our line high
		paBusSetSO(1);

		// Wait for the sender to go drive their line high again
		paBusResetTimeout(40000);
		while (!paBusGetSI()) {
			if (paBusTimedOut()) goto readTimedOut;
		}

	}

	return TRUE;

	// Jump here if a read times out
readTimedOut:
	paBusEnd();
	return FALSE;

}

DWORD paBusReadBytes(BYTE *buffer, DWORD length) {
	BYTE value;
	for (DWORD i = 0; i < length; ++i) {
		if (paBusReadByte(&value)) {
			*buffer++ = value;
		} else {
			return i;
		}
	}
	return length;
}

BOOL paBusWriteByte(BYTE value) {

	// Check port is open
	if (!paBusPort) return FALSE;

	// We're not reading any more
	paBusReading = FALSE;

	// Drive our line high
	paBusSetSO(1);

	// Wait for the receiver to respond and go high too
	paBusResetTimeout(40000);
	while (!paBusGetSI()) {
		if (paBusTimedOut()) goto writeTimedOut;
	}

	// We're now writing
	paBusWriting = TRUE;

	for (int bit = 0; bit < 8; ++bit) {

		// Drive our line low
		paBusSetSO(0);

		// Wait for the receiver to acknowledge
		paBusResetTimeout(20000);
		while (paBusGetSI()) {
			if (paBusTimedOut()) goto writeTimedOut;
		}

		// Within 100us, we need to set the data level
		paBusSetSO((value & 0x80) != 0);
		value <<= 1;

		// Wait for the receiver to clock in the bit
		paBusResetTimeout(20000);
		while (!paBusGetSI()) {
			if (paBusTimedOut()) goto writeTimedOut;
		}

		// Drive the line back high
		paBusSetSO(1);

		// Hold for at least 150us
		paBusDelay(150);

	}

	return TRUE;

	// Jump here if a write times out
writeTimedOut:
	paBusEnd();
	return FALSE;

}

DWORD paBusWriteBytes(BYTE *buffer, DWORD length) {
	for (DWORD i = 0; i < length; ++i) {
		if (!paBusWriteByte(*buffer++)) {
			return i;
		}
	}
	return length;
}

void paBusClose(void) {
	if (paBusPort) {
		CloseHandle(paBusPort);
		paBusPort = NULL;
	}
}

BOOL paBusOpen(LPCTSTR portName) {

	DCB dcb = { 0, };
	dcb.DCBlength = sizeof(DCB);

	// If nothing else, we need a high-resolution counter for timing purposes
	if (!QueryPerformanceFrequency(&qpFrequency)) goto openFailed;

	// Before we do anything else, ensure the bus is closed
	paBusClose();

	// Try to open the serial port
	paBusPort = CreateFile(portName, GENERIC_READ | GENERIC_WRITE, 0, NULL, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, NULL);
	if (paBusPort == INVALID_HANDLE_VALUE) goto openFailed;

	// Try to build the DCB from some suitably sensible defaults
	if (!BuildCommDCB(TEXT("baud=9600 parity=N data=8 stop=1"), &dcb)) goto openFailed;

	// Update the port state
	if (!SetCommState(paBusPort, &dcb)) goto openFailed;

	// Pull DTR high for +12V supply
	if (!EscapeCommFunction(paBusPort, SETDTR)) goto openFailed;

	// Drive our level low
	paBusSetSO(0);

	return TRUE;

openFailed:
	paBusClose();
	return FALSE;
}

BOOL paBusIsOpen(void) {
	return paBusPort != NULL;
}
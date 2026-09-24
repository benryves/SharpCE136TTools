#include "paBus.h"

HANDLE paBusPort = NULL;

LARGE_INTEGER qpFrequency = { 0, };
LARGE_INTEGER qpCurrent = { 0, };
LARGE_INTEGER qpTimeout = { 0, };

DWORD paBusReadDelay = 0;
DWORD paBusWriteDelay = 0;

static void paBusResetTimeout(DWORD microseconds) {
	QueryPerformanceCounter(&qpCurrent);
	qpTimeout.QuadPart = qpCurrent.QuadPart + (microseconds * qpFrequency.QuadPart) / 1000000;
}

static BOOL paBusTimedOut(void) {
	QueryPerformanceCounter(&qpCurrent);
	return qpCurrent.QuadPart >= qpTimeout.QuadPart;
}

static void paBusDelay(DWORD microseconds) {
	LARGE_INTEGER qpDelay = { 0, };
	if (!QueryPerformanceCounter(&qpDelay)) return;
	qpDelay.QuadPart += (microseconds * qpFrequency.QuadPart) / 1000000;
	do {
		if (!QueryPerformanceCounter(&qpCurrent)) return;
	} while (qpCurrent.QuadPart < qpDelay.QuadPart);
}

BOOL writtenSO;
static BOOL paBusSetSO(BOOL level) {
	if (EscapeCommFunction(paBusPort, level ? CLRRTS : SETRTS)) {
		writtenSO = level;
		return TRUE;
	} else {
		return FALSE;
	}
}

static BOOL paBusGetSO(void) {
	return writtenSO;
}

static BOOL paBusGetSI(void) {
	DWORD modemStat;
	GetCommModemStatus(paBusPort, &modemStat);
	return (modemStat & MS_DSR_ON) == 0;
}

BOOL paBusEnd(void) {
	// Set SO low
	paBusSetSO(0);
	// Wait for SI to follow suit
	paBusResetTimeout(40000);
	while (paBusGetSI()) {
		if (paBusTimedOut()) {
			return FALSE;
		}
	}
	return TRUE;
}

BOOL paBusReadByte(BYTE *value) {

	// Check port is open
	if (!paBusPort) return FALSE;

	BOOL entrySO = paBusGetSO();
	BOOL entrySI = paBusGetSI();

	*value = 0;

	// Is the line closed?
	if (!entrySO && !entrySI) {
		// Line closed.
		// Need to wait for SI to go high first
		paBusResetTimeout(40000);
		while (!paBusGetSI()) {
			if (paBusTimedOut()) goto readTimedOut;
		}
		// Acknowledge by setting SO high
		paBusSetSO(1);
		// Now wait for SI to go low...
	} else if (entrySO && entrySI) {
		// Both lines are high, so line already open.
		// Wait for SI to go low.
	} else if (!entrySO && entrySI) {
		// I'm inactive, but SI already gone high to start sending something!
		paBusSetSO(1);
		// Now wait for SI to go low...
	} else {
		// I'm active, but SI has already gone low
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
		paBusDelay(300 - (paBusReadDelay + paBusWriteDelay));

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
	paBusSetSO(entrySO);
	return FALSE;

}

DWORD paBusReadBytes(BYTE *buffer, DWORD offset, DWORD length, DWORD timeout) {
	LARGE_INTEGER qpUserTimeout;
	DWORD read = 0;

	// Check port is open
	if (!paBusPort) return read;

	// Initialise the user timeout
	if (!QueryPerformanceCounter(&qpUserTimeout)) return 0;
	qpUserTimeout.QuadPart += (timeout * qpFrequency.QuadPart) / 1000;

	buffer += offset;

	while (read < length) {
		if (paBusReadByte(buffer)) {
			// Update pointers/counters
			++buffer;
			++read;
			// Update the user timeout
			if (QueryPerformanceCounter(&qpUserTimeout)) {
				qpUserTimeout.QuadPart += (timeout * qpFrequency.QuadPart) / 1000;
			}
		} else {
			// Timed out?
			if (!QueryPerformanceCounter(&qpCurrent) || qpCurrent.QuadPart >= qpUserTimeout.QuadPart) break;
		}
	}
	return read;
}

BOOL paBusWriteByte(BYTE value) {

	// Check port is open
	if (!paBusPort) return FALSE;

	// Drive our line high
	paBusSetSO(1);
	paBusDelay(300);

	// Wait for the receiver to respond and go high too
	paBusResetTimeout(40000);
	while (!paBusGetSI()) {
		if (paBusTimedOut()) goto writeTimedOut;
	}

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

	// Extra delay at the end of each byte
	paBusDelay(150);
	return TRUE;

	// Jump here if a write times out
writeTimedOut:
	paBusSetSO(FALSE);
	return FALSE;

}

DWORD paBusWriteBytes(BYTE *buffer, DWORD offset, DWORD length, DWORD timeout) {
	LARGE_INTEGER qpUserTimeout;
	DWORD written = 0;

	// Check port is open
	if (!paBusPort) return written;

	// Initialise the user timeout
	if (!QueryPerformanceCounter(&qpUserTimeout)) return 0;
	qpUserTimeout.QuadPart += (timeout * qpFrequency.QuadPart) / 1000;

	buffer += offset;

	while (written < length) {
		if (paBusWriteByte(*buffer)) {
			// Update pointers/counters
			++buffer;
			++written;
			// Update the user timeout
			if (QueryPerformanceCounter(&qpUserTimeout)) {
				qpUserTimeout.QuadPart += (timeout * qpFrequency.QuadPart) / 1000;
			}
		} else {
			// Timed out?
			if (!QueryPerformanceCounter(&qpCurrent) || qpCurrent.QuadPart >= qpUserTimeout.QuadPart) break;
		}
	}
	return written;
}

void paBusClose(void) {
	if (paBusPort) {
		SetThreadPriority(GetCurrentThread(), THREAD_PRIORITY_NORMAL);
		SetPriorityClass(GetCurrentProcess(), NORMAL_PRIORITY_CLASS);
		CloseHandle(paBusPort);
		paBusPort = NULL;
	}
	paBusWriteDelay = 0;
	paBusReadDelay = 0;
}

BOOL paBusOpen(LPCTSTR portName) {

	LARGE_INTEGER qpBefore = { 0, };
	LARGE_INTEGER qpAfter = { 0, };

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
	QueryPerformanceCounter(&qpBefore);
	for (int i = 0; i < 10; ++i) paBusSetSO(0);
	QueryPerformanceCounter(&qpAfter);

	// Calculate the write delay
	paBusWriteDelay = (DWORD)((qpAfter.QuadPart - qpBefore.QuadPart) * 100000 / qpFrequency.QuadPart);

	// Calculate the read delay
	QueryPerformanceCounter(&qpBefore);
	for (int i = 0; i < 10; ++i) paBusGetSI();
	QueryPerformanceCounter(&qpAfter);
	paBusReadDelay = (DWORD)((qpAfter.QuadPart - qpBefore.QuadPart) * 100000 / qpFrequency.QuadPart);

	// Port successfully opened and both lines now idling high
	SetPriorityClass(GetCurrentProcess(), REALTIME_PRIORITY_CLASS);
	SetThreadPriority(GetCurrentThread(), THREAD_PRIORITY_TIME_CRITICAL);

	return TRUE;

openFailed:
	paBusClose();
	return FALSE;
}

BOOL paBusIsOpen(void) {
	return paBusPort != NULL;
}

BOOL paBusIsEnded(BOOL *ended) {

	// Check port is open
	if (!paBusPort) return FALSE;

	// Check whether the communication line is closed ("ended") - both lines should be low
	*ended = !paBusGetSO() && !paBusGetSI();

	return TRUE;
}
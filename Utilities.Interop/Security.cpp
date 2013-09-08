#include "Security.h"

using namespace Utilities::Interop;

void Security::ZeroMemorySecured(PVOID ptr, SIZE_T count)
{
	SecureZeroMemory(ptr, count);
}

#pragma once
#include <Windows.h>

namespace Utilities
{
	namespace Interop
	{
		public ref class Security abstract sealed
		{
		public:
			static void ZeroMemorySecured(PVOID ptr, SIZE_T count);
		};
	}
}

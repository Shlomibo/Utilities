using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Windows.Services.Interop
{
	/// <summary>
	/// Contains status information for a service.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal struct ServiceStatus
	{
		public const uint ERROR_SERVICE_SPECIFIC_ERROR = 1066;
		public const uint NO_ERROR = 0;

		/// <summary>The type of service. </summary>
		public ServiceType type;
		/// <summary>The current state of the service.</summary>
		public State state;
		/// <summary>
		/// The control codes the service accepts and processes in its handler function (see Handler and HandlerEx). 
		/// A user interface process can control a service by specifying a control command in 
		/// the ControlService or ControlServiceEx function. 
		/// By default, all services accept the Interrogate value. To accept the DeviceEvent value, 
		/// the service must register to receive device events by using the RegisterDeviceNotification function.
		/// </summary>
		public AcceptedControls acceptedControls;
		/// <summary>
		/// The error code the service uses to report an error that occurs when it is starting or stopping. 
		/// To return an error code specific to the service, 
		/// the service must set this value to ERROR_SERVICE_SPECIFIC_ERROR to indicate that 
		/// the specificExitCode member contains the error code. 
		/// The service should set this value to NO_ERROR when it is running and on normal termination.
		/// </summary>
		public uint win32ExitCode;
		/// <summary>
		/// A service-specific error code that the service returns when an error occurs while
		/// the service is starting or stopping. 
		/// This value is ignored unless the win32ExitCode member is set to ERROR_SERVICE_SPECIFIC_ERROR.
		/// </summary>
		public int specificExitCode;
		/// <summary>
		/// The check-point value the service increments periodically to report its progress during a lengthy start, 
		/// stop, pause, or continue operation. 
		/// For example, the service should increment this value as it completes each step of 
		/// its initialization when it is starting up. 
		/// The user interface program that invoked the operation on the service uses this value to track 
		/// the progress of the service during a lengthy operation. 
		/// This value is not valid and should be zero when the service does not have a start,
		/// stop, pause, or continue operation pending.
		/// </summary>
		public int checkPoint;
		/// <summary>
		/// The estimated time required for a pending start, stop, pause, or continue operation, in milliseconds. 
		/// Before the specified amount of time has elapsed, 
		/// the service should make its next call to the SetServiceStatus function with either 
		/// an incremented dwCheckPoint value or a change in dwCurrentState. 
		/// If the amount of time specified by dwWaitHint passes, 
		/// and dwCheckPoint has not been incremented or dwCurrentState has not changed, 
		/// the service control manager or service control program can assume that 
		/// an error has occurred and the service should be stopped. 
		/// However, if the service shares a process with other services, 
		/// the service control manager cannot terminate the service application because it would have 
		/// to terminate the other services sharing the process as well.
		/// </summary>
		public int waitHint;
	}

	/// <summary>
	/// Contains process status information for a service. 
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal struct ServiceStatusProcess
	{
		/// <summary>The type of service.</summary>
		public ServiceType type;
		/// <summary>The current state of the service. </summary>
		public State state;
		/// <summary>
		/// The control codes the service accepts and processes in its handler function (see Handler and HandlerEx). 
		/// A user interface process can control a service by specifying a control command in 
		/// the ControlService or ControlServiceEx function. 
		/// By default, all services accept the Interrogate value. To accept the DeviceEvent value, 
		/// the service must register to receive device events by using the RegisterDeviceNotification function.
		/// </summary>
		public AcceptedControls acceptedControls;
		/// <summary>
		/// The error code the service uses to report an error that occurs when it is starting or stopping. 
		/// To return an error code specific to the service, 
		/// the service must set this value to ERROR_SERVICE_SPECIFIC_ERROR to indicate that 
		/// the specificExitCode member contains the error code. 
		/// The service should set this value to NO_ERROR when it is running and on normal termination.
		/// </summary>
		public uint win32ExitCode;
		/// <summary>
		/// A service-specific error code that the service returns when an error occurs while
		/// the service is starting or stopping. 
		/// This value is ignored unless the win32ExitCode member is set to ERROR_SERVICE_SPECIFIC_ERROR.
		/// </summary>
		public int specificExitCode;
		/// <summary>
		/// The check-point value the service increments periodically to report its progress during a lengthy start, 
		/// stop, pause, or continue operation. 
		/// For example, the service should increment this value as it completes each step of 
		/// its initialization when it is starting up. 
		/// The user interface program that invoked the operation on the service uses this value to track 
		/// the progress of the service during a lengthy operation. 
		/// This value is not valid and should be zero when the service does not have a start,
		/// stop, pause, or continue operation pending.
		/// </summary>
		public int checkPoint;
		/// <summary>
		/// The estimated time required for a pending start, stop, pause, or continue operation, in milliseconds. 
		/// Before the specified amount of time has elapsed, 
		/// the service should make its next call to the SetServiceStatus function with either 
		/// an incremented dwCheckPoint value or a change in dwCurrentState. 
		/// If the amount of time specified by dwWaitHint passes, 
		/// and dwCheckPoint has not been incremented or dwCurrentState has not changed, 
		/// the service control manager or service control program can assume that 
		/// an error has occurred and the service should be stopped. 
		/// However, if the service shares a process with other services, 
		/// the service control manager cannot terminate the service application because it would have 
		/// to terminate the other services sharing the process as well.
		/// </summary>
		public int waitHint;
		/// <summary>The process identifier of the service.</summary>
		public uint processId;
		/// <summary>Specifies service process flags.</summary>
		public ProcessFlags flags;
	}

	/// <summary>
	/// Contains the name of a service in a service control manager database and information about that service. 
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal unsafe struct EnumServiceStatus
	{
		/// <summary>
		/// The name of a service in the service control manager database. 
		/// The maximum string length is 256 characters. 
		/// The service control manager database preserves the case of the characters, 
		/// but service name comparisons are always case insensitive. 
		/// A slash (/), backslash (\), comma, and space are invalid service name characters.
		/// </summary>
		public char* lpServiceName;
		/// <summary>
		/// A display name that can be used by service control programs, 
		/// such as Services in Control Panel, to identify the service. 
		/// This string has a maximum length of 256 characters. 
		/// The name is case-preserved in the service control manager. 
		/// Display name comparisons are always case-insensitive.
		/// </summary>
		public char* lpDisplayName;
		/// <summary>
		/// A ServiceStatus structure that contains status information for the lpServiceName service.
		/// </summary>
		public ServiceStatus status;
	}
	
	/// <summary>
	/// Contains the name of a service in a service control manager database and information about the service.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal unsafe struct EnumServiceStatusProcess
	{
		/// <summary>
		/// The name of a service in the service control manager database. 
		/// The maximum string length is 256 characters. 
		/// The service control manager database preserves the case of the characters, 
		/// but service name comparisons are always case insensitive. 
		/// A slash (/), backslash (\), comma, and space are invalid service name characters.
		/// </summary>
		public char* lpServiceName;
		/// <summary>
		/// A display name that can be used by service control programs, 
		/// such as Services in Control Panel, to identify the service. 
		/// This string has a maximum length of 256 characters. 
		/// The case is preserved in the service control manager. 
		/// Display name comparisons are always case-insensitive.
		/// </summary>
		public char* lpDisplayName;
		/// <summary>
		/// A ServiceStatusProcess structure that contains status information for the lpServiceName service.
		/// </summary>
		public ServiceStatusProcess processStatus;
	}

	/// <summary>
	/// Contains configuration information for an installed service.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal unsafe struct QueryServiceConfig
	{
		public const char SC_GROUP_IDENTIFIER = '+';

		/// <summary>The type of service.</summary>
		public ServiceType type;
		/// <summary>When to start the service. This member can be one of the following values.</summary>
		public StartType startType;
		/// <summary>The severity of the error, and action taken, if this service fails to start.</summary>
		public ErrorControl errorControl;
		/// <summary>
		/// The fully qualified path to the service binary file.
		/// 
		/// The path can also include arguments for an auto-start service. 
		/// These arguments are passed to the service entry point (typically the main function).
		/// </summary>
		public char* lpBinaryPathName;
		/// <summary>
		/// The name of the load ordering group to which this service belongs. 
		/// If the member is NULL or an empty string, the service does not belong to a load ordering group.
		/// 
		/// The startup program uses load ordering groups to load groups of services in a specified 
		/// order with respect to the other groups. 
		/// The list of load ordering groups is contained in the following registry value:
		/// 
		/// HKEY_LOCAL_MACHINE\System\CurrentControlSet\Control\ServiceGroupOrder
		/// </summary>
		public char* lpLoadOrderGroup;
		/// <summary>
		/// A unique tag value for this service in the group specified by the lpLoadOrderGroup parameter. 
		/// A value of zero indicates that the service has not been assigned a tag. 
		/// You can use a tag for ordering service startup within a load order group by 
		/// specifying a tag order vector in the registry located at:
		/// 
		/// HKEY_LOCAL_MACHINE\System\CurrentControlSet\Control\GroupOrderList
		/// 
		/// Tags are only evaluated for KernelDriver and FileSystemDriver type services that have RootStart or 
		/// SystemStart start types.
		/// </summary>
		public uint tagId;
		/// <summary>
		/// A pointer to an array of null-separated names of services or load ordering groups that 
		/// must start before this service. 
		/// The array is doubly null-terminated. 
		/// If the pointer is NULL or if it points to an empty string, the service has no dependencies. 
		/// If a group name is specified, it must be prefixed by the SC_GROUP_IDENTIFIER (defined in WinSvc.h) 
		/// character to differentiate it from a service name, 
		/// because services and service groups share the same name space. 
		/// Dependency on a service means that this service can only run if the service it depends on is running. 
		/// Dependency on a group means that this service can run if at least one member of the group is running 
		/// after an attempt to start all members of the group.
		/// </summary>
		public char* lpDependencies;
		/// <summary>
		/// If the service type is Win32OwnProcess or Win32SharedProcess,
		/// this member is the name of the account that the service process will be logged on as when it runs. 
		/// This name can be of the form Domain\UserName. 
		/// If the account belongs to the built-in domain, the name can be of the form .\UserName. 
		/// The name can also be "LocalSystem" if the process is running under the LocalSystem account.
		/// 
		/// If the service type is KernelDriver or FileSystemDriver, 
		/// this member is the driver object name (that is, \FileSystem\Rdr or \Driver\Xns) 
		/// which the input and output (I/O) system uses to load the device driver. 
		/// If this member is NULL, the driver is to be run with a default object name created by the I/O system, 
		/// based on the service name.
		/// </summary>
		public char* lpServiceStartName;
		/// <summary>
		/// The display name to be used by service control programs to identify the service. 
		/// This string has a maximum length of 256 characters. The name is case-preserved in the service control manager.
		/// Display name comparisons are always case-insensitive.
		/// 
		/// This parameter can specify a localized string using the following format:
		/// 
		/// @[Path\]DLLName,-StrID
		/// 
		/// The string with identifier StrID is loaded from DLLName; the Path is optional.
		/// </summary>
		public char* lpDisplayName;
	}
	/// <summary>
	/// Contains information about the lock status of a service control manager database. 
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal unsafe struct QueryServiceLockStatus
	{
		/// <summary>
		/// The lock status of the database. If this member is nonzero, the database is locked. 
		/// If it is zero, the database is unlocked.
		/// </summary>
		public bool isLocked;
		/// <summary>The name of the user who acquired the lock.</summary>
		public char* lpLockOwner;
		/// <summary>The time since the lock was first acquired, in seconds.</summary>
		public uint lockDuration;
	}

	/// <summary>
	/// Represents an action that the service control manager can perform.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal struct SCAction
	{
		/// <summary>The action to be performed.</summary>
		public ServiceControlActionType actionType;
		/// <summary>The time to wait before performing the specified action, in milliseconds.</summary>
		public uint delay;
	}

	/// <summary>
	/// Contains the delayed auto-start setting of an auto-start service.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal struct ServiceDelayedAutoStartInfo
	{
		/// <summary>
		/// If this member is TRUE, the service is started after other auto-start services are started plus a short delay. 
		/// Otherwise, the service is started during system boot.
		/// 
		/// This setting is ignored unless the service is an auto-start service.
		/// </summary>
		public bool isAutoStartDelayed;
	}

	/// <summary>
	/// Contains a service description.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal unsafe struct ServiceDescription
	{
		/// <summary>
		/// The description of the service. 
		/// If this member is NULL, the description remains unchanged. 
		/// If this value is an empty string (""), the current description is deleted.
		/// 
		/// The service description must not exceed the size of 255.
		/// 
		/// This member can specify a localized string using the following format:
		/// 
		/// @[path\]dllname,-strID
		/// 
		/// The string with identifier strID is loaded from dllname; the path is optional.
		/// </summary>
		public char* lpDescription;
	}

	/// <summary>
	/// Represents the action the service controller should take on each failure of a service. 
	/// A service is considered failed when it terminates without reporting a status of Stopped to the service controller.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal unsafe struct ServiceFailureActions
	{
		public const uint INFINITE = 0xFFFFFFFF;
		/// <summary>
		/// The time after which to reset the failure count to zero if there are no failures, in seconds. 
		/// Specify INFINITE to indicate that this value should never be reset.
		/// </summary>
		public uint resetPeriod;
		/// <summary>
		/// The message to be broadcast to server users before rebooting in 
		/// response to the Reboot service controller action.
		/// 
		/// If this value is NULL, the reboot message is unchanged. 
		/// If the value is an empty string (""), the reboot message is deleted and no message is broadcast.
		/// 
		/// This member can specify a localized string using the following format:
		/// 
		/// @[path\]dllname,-strID
		/// 
		/// The string with identifier strID is loaded from dllname; the path is optional.
		/// </summary>
		public char* lpRebootMessage;
		/// <summary>
		/// The command line of the process for the CreateProcess function to execute in response to the 
		/// RunCommand service controller action. 
		/// This process runs under the same account as the service.
		/// 
		/// If this value is NULL, the command is unchanged. 
		/// If the value is an empty string (""), the command is deleted and no program is run when the service fails.
		/// </summary>
		public char* lpCommand;
		/// <summary>
		/// The number of elements in the lpsaActions array.
		/// 
		/// If this value is 0, but lpsaActions is not NULL, the reset period and array of failure actions are deleted.
		/// </summary>
		public int actionsCount;
		/// <summary>
		/// A pointer to an array of SCAction structures.
		/// 
		/// If this value is NULL, the cActions and dwResetPeriod members are ignored.
		/// </summary>
		public SCAction* lpActions;
	}

	/// <summary>
	/// Contains the failure actions flag setting of a service. 
	/// This setting determines when failure actions are to be executed.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal struct ServiceFailureActionsFlag
	{
		/// <summary>
		/// If this member is TRUE and the service has configured failure actions, 
		/// the failure actions are queued if the service process terminates without reporting 
		/// a status of Stopped or if it enters the Stopped state but the Win32ExitCode member of
		/// the ServiceStatus structure is not ERROR_SUCCESS (0).
		/// 
		/// If this member is FALSE and the service has configured failure actions,
		/// the failure actions are queued only if the service terminates without reporting a status of Stopped.
		/// 
		/// This setting is ignored unless the service has configured failure actions.
		/// </summary>
		public bool failureActionsOnNonCrashFailures;
	}

	/// <summary>
	/// Represents service status notification information.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal unsafe struct ServiceNotify
	{
		public const uint SERVICE_NOTIFY_STATUS_CHANGE = 2;
		public const uint ERROR_SUCCESS = 0;
		public const uint ERROR_SERVICE_MARKED_FOR_DELETE = 1072;

		/// <summary>
		/// The structure version. This member must be SERVICE_NOTIFY_STATUS_CHANGE
		/// </summary>
		public uint version;
		/// <summary>
		/// A pointer to the callback function of the signature:
		/// 
		/// void Func(void* parameter)
		/// </summary>
		public IntPtr notifyCallback;
		/// <summary>Any user-defined data to be passed to the callback function.</summary>
		public IntPtr context;
		/// <summary>
		/// A value that indicates the notification status. 
		/// If this member is ERROR_SUCCESS, 
		/// the notification has succeeded and the ServiceStatus member contains valid information. 
		/// If this member is ERROR_SERVICE_MARKED_FOR_DELETE, 
		/// the service has been marked for deletion and the service 
		/// handle used by NotifyServiceStatusChange must be closed.
		/// </summary>
		public uint notificationStatus;
		/// <summary>
		/// A ServiceStatusProcess structure that contains the service status information. 
		/// This member is only valid if notificationStatus is ERROR_SUCCESS.
		/// </summary>
		public ServiceStatusProcess serviceStatus;
		/// <summary>
		/// If notificationStatus is ERROR_SUCCESS, 
		/// this member contains a bitmask of the notifications that triggered this call to the callback function.
		/// </summary>
		public Notification notificationTriggered;
		/// <summary>
		/// If notificationStatus is ERROR_SUCCESS and the notification is Created or Deleted, 
		/// this member is valid and it is a MULTI_SZ string that contains one or more service names.
		/// The names of the created services will have a '/' prefix so you can distinguish them from 
		/// the names of the deleted services.
		/// 
		/// If this member is valid, the notification callback function must free the string using the LocalFree function.
		/// </summary>
		public char* serviceName;
	}
	/// <summary>
	/// Contains preshutdown settings.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal struct ServicePreShutdownInfo
	{
		/// <summary>The time-out value, in milliseconds.</summary>
		public uint timeout;
	}

	/// <summary>
	/// Represents the required privileges for a service.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal unsafe struct ServiceRequiredPrivilegesInfo
	{
		/// <summary>
		/// Required to assign the primary token of a process.
		/// 
		/// User Right: Replace a process-level token.
		/// </summary>
		public const string SE_ASSIGNPRIMARYTOKEN_NAME = "SeAssignPrimaryTokenPrivilege";
		/// <summary>
		/// Required to generate audit-log entries. Give this privilege to secure servers.
		/// 
		/// User Right: Generate security audits.
		/// </summary>
		public const string SE_AUDIT_NAME = "SeAuditPrivilege";
		/// <summary>
		/// Required to perform backup operations. 
		/// This privilege causes the system to grant all read access control to any file, 
		/// regardless of the access control list (ACL) specified for the file. 
		/// Any access request other than read is still evaluated with the ACL. 
		/// This privilege is required by the RegSaveKey and RegSaveKeyExfunctions. 
		/// The following access rights are granted if this privilege is held:
		/// 
		/// READ_CONTROL
		/// ACCESS_SYSTEM_SECURITY
		/// FILE_GENERIC_READ
		/// FILE_TRAVERSE
		/// 
		/// User Right: Back up files and directories.
		/// </summary>
		public const string SE_BACKUP_NAME = "SeBackupPrivilege";
		/// <summary>
		/// Required to receive notifications of changes to files or directories. 
		/// This privilege also causes the system to skip all traversal access checks. 
		/// It is enabled by default for all users.
		/// 
		/// User Right: Bypass traverse checking.
		/// </summary>
		public const string SE_CHANGE_NOTIFY_NAME = "SeChangeNotifyPrivilege";
		/// <summary>
		/// Required to create named file mapping objects in the global namespace during Terminal Services sessions. 
		/// This privilege is enabled by default for administrators, services, and the local system account.
		/// 
		/// User Right: Create global objects.
		/// </summary>
		public const string SE_CREATE_GLOBAL_NAME = "SeCreateGlobalPrivilege";
		/// <summary>
		/// Required to create a paging file.
		/// 
		/// User Right: Create a pagefile.
		/// </summary>
		public const string SE_CREATE_PAGEFILE_NAME = "SeCreatePagefilePrivilege";
		/// <summary>
		/// Required to create a permanent object.
		/// 
		/// User Right: Create permanent shared objects.
		/// </summary>
		public const string SE_CREATE_PERMANENT_NAME = "SeCreatePermanentPrivilege";
		/// <summary>
		/// Required to create a symbolic link.
		/// 
		/// User Right: Create symbolic links.
		/// </summary>
		public const string SE_CREATE_SYMBOLIC_LINK_NAME = "SeCreateSymbolicLinkPrivilege";
		/// <summary>
		/// Required to create a primary token.
		/// 
		/// User Right: Create a token object.
		/// 
		/// You cannot add this privilege to a user account with the "Create a token object" policy. 
		/// Additionally, you cannot add this privilege to an owned process using Windows APIs.
		/// </summary>
		public const string SE_CREATE_TOKEN_NAME = "SeCreateTokenPrivilege";
		/// <summary>
		/// Required to debug and adjust the memory of a process owned by another account.
		/// 
		/// User Right: Debug programs.
		/// </summary>
		public const string SE_DEBUG_NAME = "SeDebugPrivilege";
		/// <summary>
		/// Required to mark user and computer accounts as trusted for delegation.
		/// 
		/// User Right: Enable computer and user accounts to be trusted for delegation.
		/// </summary>
		public const string SE_ENABLE_DELEGATION_NAME = "SeEnableDelegationPrivilege";
		/// <summary>
		/// Required to impersonate.
		/// 
		/// User Right: Impersonate a client after authentication.
		/// </summary>
		public const string SE_IMPERSONATE_NAME = "SeImpersonatePrivilege";
		/// <summary>
		/// Required to increase the base priority of a process.
		/// 
		/// User Right: Increase scheduling priority.
		/// </summary>
		public const string SE_INC_BASE_PRIORITY_NAME = "SeIncreaseBasePriorityPrivilege";
		/// <summary>
		/// Required to increase the quota assigned to a process.
		/// 
		/// User Right: Adjust memory quotas for a process.
		/// </summary>
		public const string SE_INCREASE_QUOTA_NAME = "SeIncreaseQuotaPrivilege";
		/// <summary>
		/// Required to allocate more memory for applications that run in the context of users.
		/// 
		/// User Right: Increase a process working set.
		/// </summary>
		public const string SE_INC_WORKING_SET_NAME = "SeIncreaseWorkingSetPrivilege";
		/// <summary>
		/// Required to load or unload a device driver.
		/// 
		/// User Right: Load and unload device drivers.
		/// </summary>
		public const string SE_LOAD_DRIVER_NAME = "SeLoadDriverPrivilege";
		/// <summary>
		/// Required to lock physical pages in memory.
		/// 
		/// User Right: Lock pages in memory.
		/// </summary>
		public const string SE_LOCK_MEMORY_NAME = "SeLockMemoryPrivilege";
		/// <summary>
		/// Required to create a computer account.
		/// 
		/// User Right: Add workstations to domain.
		/// </summary>
		public const string SE_MACHINE_ACCOUNT_NAME = "SeMachineAccountPrivilege";
		/// <summary>
		/// Required to enable volume management privileges.
		/// 
		/// User Right: Manage the files on a volume.
		/// </summary>
		public const string SE_MANAGE_VOLUME_NAME = "SeManageVolumePrivilege";
		/// <summary>
		/// Required to gather profiling information for a single process.
		/// 
		/// User Right: Profile single process.
		/// </summary>
		public const string SE_PROF_SINGLE_PROCESS_NAME = "SeProfileSingleProcessPrivilege";
		/// <summary>
		/// Required to modify the mandatory integrity level of an object.
		/// 
		/// User Right: Modify an object label.
		/// </summary>
		public const string SE_RELABEL_NAME = "SeRelabelPrivilege";
		/// <summary>
		/// Required to shut down a system using a network request.
		/// 
		/// User Right: Force shutdown from a remote system.
		/// </summary>
		public const string SE_REMOTE_SHUTDOWN_NAME = "SeRemoteShutdownPrivilege";
		/// <summary>
		/// Required to perform restore operations. This privilege causes the system to grant all write access control to any file, regardless of the ACL specified for the file. Any access request other than write is still evaluated with the ACL. Additionally, this privilege enables you to set any valid user or group SID as the owner of a file. This privilege is required by the RegLoadKey function. The following access rights are granted if this privilege is held:
		/// 
		/// WRITE_DAC
		/// WRITE_OWNER
		/// ACCESS_SYSTEM_SECURITY
		/// FILE_GENERIC_WRITE
		/// FILE_ADD_FILE
		/// FILE_ADD_SUBDIRECTORY
		/// DELETE
		/// 
		/// User Right: Restore files and directories.
		/// </summary>
		public const string SE_RESTORE_NAME = "SeRestorePrivilege";
		/// <summary>
		/// Required to perform a number of security-related functions, such as controlling and viewing audit messages. 
		/// This privilege identifies its holder as a security operator.
		/// 
		/// User Right: Manage auditing and security log.
		/// </summary>
		public const string SE_SECURITY_NAME = "SeSecurityPrivilege";
		/// <summary>
		/// Required to shut down a local system.
		/// 
		/// User Right: Shut down the system.
		/// </summary>
		public const string SE_SHUTDOWN_NAME = "SeShutdownPrivilege";
		/// <summary>
		/// Required for a domain controller to use the Lightweight Directory Access Protocol 
		/// directory synchronization services. 
		/// This privilege enables the holder to read all objects and properties in the directory, 
		/// regardless of the protection on the objects and properties. 
		/// By default, it is assigned to the Administrator and LocalSystem accounts on domain controllers.
		/// 
		/// User Right: Synchronize directory service data.
		/// </summary>
		public const string SE_SYNC_AGENT_NAME = "SeSyncAgentPrivilege";
		/// <summary>
		/// Required to modify the nonvolatile RAM of systems that use this type of memory to 
		/// store configuration information.
		/// 
		/// User Right: Modify firmware environment values.
		/// </summary>
		public const string SE_SYSTEM_ENVIRONMENT_NAME = "SeSystemEnvironmentPrivilege";
		/// <summary>
		/// Required to gather profiling information for the entire system.
		/// 
		/// User Right: Profile system performance.
		/// </summary>
		public const string SE_SYSTEM_PROFILE_NAME = "SeSystemProfilePrivilege";
		/// <summary>
		/// Required to modify the system time.
		/// 
		/// User Right: Change the system time.
		/// </summary>
		public const string SE_SYSTEMTIME_NAME = "SeSystemtimePrivilege";
		/// <summary>
		/// Required to take ownership of an object without being granted discretionary access. 
		/// This privilege allows the owner value to be set only to those values that 
		/// the holder may legitimately assign as the owner of an object.
		/// 
		/// User Right: Take ownership of files or other objects.
		/// </summary>
		public const string SE_TAKE_OWNERSHIP_NAME = "SeTakeOwnershipPrivilege";
		/// <summary>
		/// This privilege identifies its holder as part of the trusted computer base. 
		/// Some trusted protected subsystems are granted this privilege.
		/// 
		/// User Right: Act as part of the operating system.
		/// </summary>
		public const string SE_TCB_NAME = "SeTcbPrivilege";
		/// <summary>
		/// Required to adjust the time zone associated with the computer's internal clock.
		/// 
		/// User Right: Change the time zone.
		/// </summary>
		public const string SE_TIME_ZONE_NAME = "SeTimeZonePrivilege";
		/// <summary>
		/// Required to access Credential Manager as a trusted caller.
		/// 
		/// User Right: Access Credential Manager as a trusted caller.
		/// </summary>
		public const string SE_TRUSTED_CREDMAN_ACCESS_NAME = "SeTrustedCredManAccessPrivilege";
		/// <summary>
		/// Required to undock a laptop.
		/// 
		/// User Right: Remove computer from docking station.
		/// </summary>
		public const string SE_UNDOCK_NAME = "SeUndockPrivilege";
		/// <summary>
		/// Required to read unsolicited input from a terminal device.
		/// 
		/// User Right: Not applicable.
		/// </summary>
		public const string SE_UNSOLICITED_INPUT_NAME = "SeUnsolicitedInputPrivilege";


		/// <summary>
		/// A multi-string that specifies the privileges.
		/// </summary>
		public char* requiredPrivileges;
	}

	/// <summary>
	/// Represents a service security identifier (SID).
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal struct ServiceSidInfo
	{
		/// <summary>The service SID type.</summary>
		public SidType serviceSidType;
	}

	/// <summary>
	/// Specifies the ServiceMain function for a service that can run in the calling process. 
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal unsafe struct ServiceTableEntry
	{
		/// <summary>
		/// Enrty to signals the end of the table
		/// </summary>
		public static ServiceTableEntry EndOfTable
		{
			get { return new ServiceTableEntry(); }
		}

		/// <summary>
		/// The name of a service to be run in this service process.
		/// 
		/// If the service is installed with the SERVICE_WIN32_OWN_PROCESS service type, this member is ignored, 
		/// but cannot be NULL. This member can be an empty string ("").
		/// 
		/// If the service is installed with the SERVICE_WIN32_SHARE_PROCESS service type, 
		/// this member specifies the name of the service that uses the ServiceMain function pointed to by 
		/// the lpServiceProc member.
		/// </summary>
		public char* serviceName;
		/// <summary>
		/// A pointer to a ServiceMain function.
		/// </summary>
		public IntPtr serviceProc;
	}

	/// <summary>
	/// Represents a service trigger event. 
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal unsafe struct ServiceTrigger
	{
		/// <summary>
		/// The event is triggered when a request is made to open the named pipe specified by pDataItems. 
		/// The triggerType member must be NetworkEndPoint. 
		/// The action member must be Start.
		/// </summary>
		public static readonly Guid NAMED_PIPE_EVENT_GUID = new Guid("1F81D131-3FAC-4537-9E0C-7E7B0C2F4B55");
		/// <summary>
		/// The event is triggered when an endpoint resolution request arrives for
		/// the RPC interface GUID specified by dataItems. 
		/// The triggerType member must be NetworkEndPoint. 
		/// The dwAction member must be Start.
		/// </summary>
		public static readonly Guid RPC_INTERFACE_EVENT_GUID = new Guid("BC90D167-9470-4139-A9BA-BE0BBBF5B74D");
		/// <summary>
		/// The event is triggered when the computer joins a domain. 
		/// The triggerType member must be DomainJoin.
		/// </summary>
		public static readonly Guid DOMAIN_JOIN_GUID = new Guid("1ce20aba-9851-4421-9430-1ddeb766e809");
		/// <summary>
		/// The event is triggered when the computer leaves a domain. 
		/// The triggerType member must be Domain Join
		/// </summary>
		public static readonly Guid DOMAIN_LEAVE_GUID = new Guid("ddaf516e-58c2-4866-9574-c3b615d42ea1");
		/// <summary>
		/// The event is triggered when the specified firewall port is opened. 
		/// The triggerType member must be FirewallPortEvent.
		/// </summary>
		public static readonly Guid FIREWALL_PORT_OPEN_GUID = new Guid("b7569e07-8421-4ee0-ad10-86915afdad09");
		/// <summary>
		/// The event is triggered approximately 60 seconds after the specified firewall port is closed. 
		/// The triggerType member must be FirewallPortEvent.
		/// </summary>
		public static readonly Guid FIREWALL_PORT_CLOSE_GUID = new Guid("a144ed38-8e12-4de4-9d96-e64740b1a524");
		/// <summary>
		/// The event is triggered when the machine policy has changed. 
		/// The triggerType member must be GroupPolicy.
		/// </summary>
		public static readonly Guid MACHINE_POLICY_PRESENT_GUID = new Guid("659FCAE6-5BDB-4DA9-B1FF-CA2A178D46E0");
		/// <summary>
		/// The event is triggered when the first IP address on the TCP/IP networking stack becomes available. 
		/// The triggerType member must be IPAddressAvailability.
		/// </summary>
		public static readonly Guid NETWORK_MANAGER_FIRST_IP_ADDRESS_ARRIVAL_GUID =
			new Guid("4f27f2de-14e2-430b-a549-7cd48cbc8245");
		/// <summary>
		/// The event is triggered when the last IP address on the TCP/IP networking stack becomes unavailable. 
		/// The triggerType member must be IPAddressAvailability.
		/// </summary>
		public static readonly Guid NETWORK_MANAGER_LAST_IP_ADDRESS_REMOVAL_GUID =
			new Guid("cc4ba62a-162e-4648-847a-b6bdf993e335");
		/// <summary>
		/// The event is triggered when the user policy has changed. 
		/// The triggerType member must be GroupPolicy.
		/// </summary>
		public static readonly Guid USER_POLICY_PRESENT_GUID = new Guid("54FB46C8-F089-464C-B1FD-59D1B62C3B50");

		/// <summary>The trigger event type. </summary>
		public TriggerType triggerType;
		/// <summary>The action to take when the specified trigger event occurs.</summary>
		public TriggerAction action;
		/// <summary>
		/// Points to a GUID that identifies the trigger event subtype. 
		/// The value of this member depends on the value of the dwTriggerType member.
		/// 
		/// If triggerType is SERVICE_TRIGGER_TYPE_CUSTOM, 
		/// triggerSubType is the GUID that identifies the custom event provider.
		/// 
		/// If triggerType is SERVICE_TRIGGER_TYPE_DEVICE_INTERFACE_ARRIVAL, 
		/// triggerSubType is the GUID that identifies the device interface class.
		/// 
		/// If triggerType is SERVICE_TRIGGER_TYPE_NETWORK_ENDPOINT, triggerSubType is one of the following values:
		/// NAMED_PIPE_EVENT_GUID, RPC_INTERFACE_EVENT_GUID.
		/// 
		/// For other trigger event types, triggerSubType can be one of the following values:
		/// DOMAIN_JOIN_GUID, DOMAIN_LEAVE_GUID, FIREWALL_PORT_OPEN_GUID, FIREWALL_PORT_CLOSE_GUID, 
		/// MACHINE_POLICY_PRESENT_GUID, NETWORK_MANAGER_FIRST_IP_ADDRESS_ARRIVAL_GUID, 
		/// NETWORK_MANAGER_LAST_IP_ADDRESS_REMOVAL_GUID, USER_POLICY_PRESENT_GUID.
		/// </summary>
		public Guid* triggerSubType;
		/// <summary>
		/// The number of SERVICE_TRIGGER_SPECIFIC_DATA_ITEM structures in the array pointed to by pDataItems.
		/// 
		/// This member is valid only if the dataType member is 
		/// Custom, DeviceArrival, FirewallPortEvent, or NetworkEndPoint.
		/// </summary>
		public uint dataItemsCount;
		/// <summary>
		/// A pointer to an array of TriggerSpecificDataItem structures that contain trigger-specific data.
		/// </summary>
		public TriggerSpecificDataItem* dataItems;
	}

	/// <summary>
	/// Contains trigger-specific data for a service trigger event. 
	/// This structure is used by the SERVICE_TRIGGER structure for 
	/// Custom, DeviceArrival, FirewallPortEvent, or NetworkEndPoint trigger events.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal unsafe struct TriggerSpecificDataItem
	{
		/// <summary>The data type of the trigger-specific data pointed to by data.</summary>
		public DataItemType type;
		/// <summary>
		/// The size of the trigger-specific data pointed to data, in bytes. The maximum value is 1024.
		/// </summary>
		public uint bytesCount;
		/// <summary>
		/// A pointer to the trigger-specific data for the service trigger event. 
		/// The trigger-specific data depends on the trigger event type; see Remarks.
		/// 
		/// If the type member is Binary, the trigger-specific data is an array of bytes.
		/// 
		/// If the type member is String, the trigger-specific data is a null-terminated string or a multistring.
		/// </summary>
		public byte* data;
	}

	/// <summary>
	/// Contains trigger event information for a service. 
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal unsafe struct ServiceTriggerInfo
	{
		/// <summary>
		/// The number of triggers in the array of ServiceTrigger structures pointed to by the triggers member.
		/// </summary>
		public uint triggersCount;
		/// <summary>
		/// A pointer to an array of ServiceTrigger structures that specify the trigger events for the service. 
		/// If the triggersCount member is 0, this member is not used.
		/// </summary>
		public ServiceTrigger* triggers;
		/// <summary>
		/// This member is reserved and must be NULL.
		/// </summary>
		public IntPtr reserved;
	}

	/// <summary>
	/// 
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal unsafe struct PowerBroadcastSetting
	{
		public Guid powerSetting;
		public uint dataLen;
		public fixed byte data[1];
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct SessionNotification
	{
		public uint size;
		public uint sessionId;
	}

	/// <summary>
	/// Contains system time change settings.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal struct ServiceTimeChangeInfo
	{
		/// <summary>The new system time.</summary>
		public long newTime;
		/// <summary>The previous system time.</summary>
		public long oldTime;
	}

	/// <summary>
	/// Represents the preferred node on which to run a service.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal struct ServicePreferedNodeInfo
	{
		/// <summary>The node number of the preferred node.</summary>
		public ushort preferedNode;
		/// <summary>If this member is TRUE, the preferred node setting is deleted.</summary>
		public bool isDeleted;
	}

	/// <summary>
	/// Indicates a service protection type.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal struct ServiceLaunchProtectedInfo
	{
		/// <summary>The protection type of the service. </summary>
		public LaunchProtected launchProtected;
	}

	/// <summary>
	///  Contains service control parameters.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal unsafe struct ServiceControlStatusReasonParams
	{
		public const uint NO_ERROR = 0;
		public const uint ERROR_INVALID_SERVICE_CONTROL = 1052;
		public const uint ERROR_SERVICE_CANNOT_ACCEPT_CTRL = 1061;
		public const uint ERROR_SERVICE_NOT_ACTIVE = 1062;

		/// <summary>
		/// The reason for changing the service status to Stop. 
		/// If the current control code is not Stop, this member is ignored.
		/// 
		/// This member must be set to a combination of one general code, 
		/// one major reason code, and one minor reason code.
		/// </summary>
		public StopReasonFlag reason;
		/// <summary>
		/// An optional string that provides additional information about the service stop. 
		/// This string is stored in the event log along with the stop reason code. 
		/// This member must be NULL or a valid string that is less than 128 characters, 
		/// including the terminating null character.
		/// </summary>
		public char* comment;
		/// <summary>
		/// A pointer to a ServiceStatusProcess structure that receives the latest service status information.
		/// The information returned reflects the most recent status that the service reported to
		/// the service control manager.
		/// 
		/// The service control manager fills in the structure only when ControlService returns one of
		/// the following error codes: 
		/// NO_ERROR, ERROR_INVALID_SERVICE_CONTROL, ERROR_SERVICE_CANNOT_ACCEPT_CTRL, 
		/// or ERROR_SERVICE_NOT_ACTIVE. Otherwise, the structure is not filled in.
		/// </summary>
		public ServiceStatusProcess serviceStatus;
	}
}

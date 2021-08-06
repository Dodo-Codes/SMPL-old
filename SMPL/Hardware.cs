﻿using System.IO;
using System.Management;

namespace SMPL
{
	public static class Hardware
	{
		public struct DriveInfo
		{
			public bool IsReady { get; internal set; }
			public string Name { get; internal set; }
			public string Type { get; internal set; }
			public string Directory { get; internal set; }
			public double TotalGB { get; internal set; }
			public double AvailableGB { get; internal set; }
		}

		private enum DataType
		{
			Account, AccountSID, ACE, ActionCheck, AllocatedResource,
			ApplicationCommandLine, ApplicationService, AssociatedBattery, AssociatedProcessorMemory, BaseBoard, BaseService,
			Battery, Binary, BindImageAction, BIOS, BootConfiguration, Bus, CacheMemory, CDROMDrive, CheckCheck,
			CIMLogicalDeviceCIMDataFile, ClassicCOMApplicationClasses, ClassicCOMClass, ClassicCOMClassSetting,
			ClassicCOMClassSettings, ClassInfoAction, ClientApplicationSetting, CodecFile, COMApplication,
			COMApplicationClasses, COMApplicationSettings, COMClass, ComClassAutoEmulator, ComClassEmulator, CommandLineAccess,
			ComponentCategory, ComputerSystem, ComputerSystemProcessor, ComputerSystemProduct, COMSetting,
			Condition, CreateFolderAction, CurrentProbe, DCOMApplication,
			DCOMApplicationAccessAllowedSetting, DCOMApplicationLaunchAllowedSetting, DCOMApplicationSetting, DependentService,
			Desktop, DesktopMonitor, DeviceBus, DeviceMemoryAddress, DeviceSettings, Directory, DirectorySpecification, DiskDrive,
			DiskDriveToDiskPartition, DiskPartition, DisplayConfiguration, DisplayControllerConfiguration, DMAChannel, DriverVXD,
			DuplicateFileAction, Environment, EnvironmentSpecification, ExtensionInfoAction, Fan, FileSpecification, FloppyController,
			FloppyDrive, FontInfoAction, Group, GroupUser, HeatPipe, IDEController, IDEControllerDevice, ImplementedCategory,
			InfraredDevice, IniFileSpecification, InstalledSoftwareElement, IRQResource, Keyboard, LaunchCondition,
			LoadOrderGroup, LoadOrderGroupServiceDependencies, LoadOrderGroupServiceMembers, LogicalDisk,
			LogicalDiskRootDirectory, LogicalDiskToPartition, LogicalFileAccess, LogicalFileAuditing, LogicalFileGroup,
			LogicalFileOwner, LogicalFileSecuritySetting, LogicalMemoryConfiguration, LogicalProgramGroup,
			LogicalProgramGroupDirectory, LogicalProgramGroupItem, LogicalProgramGroupItemDataFile, LogicalShareAccess,
			LogicalShareAuditing, LogicalShareSecuritySetting, ManagedSystemElementResource, MemoryArray, MemoryArrayLocation,
			MemoryDevice, MemoryDeviceArray, MemoryDeviceLocation, MethodParameterClass, MIMEInfoAction, MotherboardDevice,
			MoveFileAction, MSIResource, NetworkAdapter, NetworkAdapterConfiguration, NetworkAdapterSetting, NetworkClient,
			NetworkConnection, NetworkLoginProfile, NetworkProtocol, NTEventlogFile, NTLogEvent, NTLogEventComputer, NTLogEventLog,
			NTLogEventUser, ODBCAttribute, ODBCDataSourceAttribute, ODBCDataSourceSpecification, ODBCDriverAttribute,
			ODBCDriverSoftwareElement, ODBCDriverSpecification, ODBCSourceAttribute, ODBCTranslatorSpecification, OnBoardDevice,
			OperatingSystem, OperatingSystemQFE, OSRecoveryConfiguration, PageFile, PageFileElementSetting, PageFileSetting,
			PageFileUsage, ParallelPort, Patch, PatchFile, PatchPackage, PCMCIAController, Perf, PerfRawData,
			PerfRawData_ASP_ActiveServerPages, PerfRawData_ASPNET_114322_ASPNETAppsv114322, PerfRawData_ASPNET_114322_ASPNETv114322,
			PerfRawData_ASPNET_ASPNET, PerfRawData_ASPNET_ASPNETApplications, PerfRawData_IAS_IASAccountingClients,
			PerfRawData_IAS_IASAccountingServer, PerfRawData_IAS_IASAuthenticationClients, PerfRawData_IAS_IASAuthenticationServer,
			PerfRawData_InetInfo_InternetInformationServicesGlobal, PerfRawData_MSDTC_DistributedTransactionCoordinator,
			PerfRawData_MSFTPSVC_FTPService, PerfRawData_MSSQLSERVER_SQLServerAccessMethods,
			PerfRawData_MSSQLSERVER_SQLServerBackupDevice, PerfRawData_MSSQLSERVER_SQLServerBufferManager,
			PerfRawData_MSSQLSERVER_SQLServerBufferPartition, PerfRawData_MSSQLSERVER_SQLServerCacheManager,
			PerfRawData_MSSQLSERVER_SQLServerDatabases, PerfRawData_MSSQLSERVER_SQLServerGeneralStatistics,
			PerfRawData_MSSQLSERVER_SQLServerLatches, PerfRawData_MSSQLSERVER_SQLServerLocks,
			PerfRawData_MSSQLSERVER_SQLServerMemoryManager, PerfRawData_MSSQLSERVER_SQLServerReplicationAgents,
			PerfRawData_MSSQLSERVER_SQLServerReplicationDist, PerfRawData_MSSQLSERVER_SQLServerReplicationLogreader,
			PerfRawData_MSSQLSERVER_SQLServerReplicationMerge, PerfRawData_MSSQLSERVER_SQLServerReplicationSnapshot,
			PerfRawData_MSSQLSERVER_SQLServerSQLStatistics, PerfRawData_MSSQLSERVER_SQLServerUserSettable,
			PerfRawData_NETFramework_NETCLRExceptions, PerfRawData_NETFramework_NETCLRInterop, PerfRawData_NETFramework_NETCLRJit,
			PerfRawData_NETFramework_NETCLRLoading, PerfRawData_NETFramework_NETCLRLocksAndThreads,
			PerfRawData_NETFramework_NETCLRMemory, PerfRawData_NETFramework_NETCLRRemoting, PerfRawData_NETFramework_NETCLRSecurity,
			PerfRawData_Outlook_Outlook, PerfRawData_PerfDisk_PhysicalDisk, PerfRawData_PerfNet_Browser,
			PerfRawData_PerfNet_Redirector, PerfRawData_PerfNet_Server, PerfRawData_PerfNet_ServerWorkQueues,
			PerfRawData_PerfOS_Cache, PerfRawData_PerfOS_Memory, PerfRawData_PerfOS_Objects, PerfRawData_PerfOS_PagingFile,
			PerfRawData_PerfOS_Processor, PerfRawData_PerfOS_System, PerfRawData_PerfProc_FullImage_Costly,
			PerfRawData_PerfProc_Image_Costly, PerfRawData_PerfProc_JobObject, PerfRawData_PerfProc_JobObjectDetails,
			PerfRawData_PerfProc_Process, PerfRawData_PerfProc_ProcessAddressSpace_Costly, PerfRawData_PerfProc_Thread,
			PerfRawData_PerfProc_ThreadDetails_Costly, PerfRawData_RemoteAccess_RASPort, PerfRawData_RemoteAccess_RASTotal,
			PerfRawData_RSVP_ACSPerRSVPService, PerfRawData_Spooler_PrintQueue, PerfRawData_TapiSrv_Telephony,
			PerfRawData_Tcpip_ICMP, PerfRawData_Tcpip_IP, PerfRawData_Tcpip_NBTConnection, PerfRawData_Tcpip_NetworkInterface,
			PerfRawData_Tcpip_TCP, PerfRawData_Tcpip_UDP, PerfRawData_W3SVC_WebService, PhysicalMemory, PhysicalMemoryArray,
			PhysicalMemoryLocation, PNPAllocatedResource, PnPDevice, PnPEntity, PointingDevice, PortableBattery, PortConnector,
			PortResource, POTSModem, POTSModemToSerialPort, PowerManagementEvent, Printer, PrinterConfiguration, PrinterController,
			PrinterDriverDll, PrinterSetting, PrinterShare, PrintJob, PrivilegesStatus, Process, Processor, ProcessStartup, Product,
			ProductCheck, ProductResource, ProductSoftwareFeatures, ProgIDSpecification, ProgramGroup, ProgramGroupContents,
			ProgramGroupOrItem, Property, ProtocolBinding, PublishComponentAction, QuickFixEngineering, Refrigeration, Registry,
			RegistryAction, RemoveFileAction, RemoveIniAction, ReserveCost, ScheduledJob, SCSIController, SCSIControllerDevice,
			SecurityDescriptor, SecuritySetting, SecuritySettingAccess, SecuritySettingAuditing, SecuritySettingGroup,
			SecuritySettingOfLogicalFile, SecuritySettingOfLogicalShare, SecuritySettingOfObject, SecuritySettingOwner,
			SelfRegModuleAction, SerialPort, SerialPortConfiguration, SerialPortSetting, Service, ServiceControl,
			ServiceSpecification, ServiceSpecificationService, SettingCheck, Share, ShareToDirectory, ShortcutAction, ShortcutFile,
			ShortcutSAP, SID, SMBIOSMemory, SoftwareElement, SoftwareElementAction, SoftwareElementCheck, SoftwareElementCondition,
			SoftwareElementResource, SoftwareFeature, SoftwareFeatureAction, SoftwareFeatureCheck, SoftwareFeatureParent,
			SoftwareFeatureSoftwareElements, SoundDevice, StartupCommand, SubDirectory, SystemAccount, SystemBIOS,
			SystemBootConfiguration, SystemDesktop, SystemDevices, SystemDriver, SystemDriverPNPEntity, SystemEnclosure,
			SystemLoadOrderGroups, SystemLogicalMemoryConfiguration, SystemMemoryResource, SystemNetworkConnections,
			SystemOperatingSystem, SystemPartitions, SystemProcesses, SystemProgramGroups, SystemResources, SystemServices,
			SystemSetting, SystemSlot, SystemSystemDriver, SystemTimeZone, SystemUsers, TapeDrive, TemperatureProbe, Thread,
			TimeZone, Trustee, TypeLibraryAction, UninterruptiblePowerSupply, USBController, USBControllerDevice, UserAccount,
			UserDesktop, VideoConfiguration, VideoController, VideoSettings, VoltageProbe, WMIElementSetting, WMISetting
		}
		private static object GetData(DataType dataType, string name)
		{
#pragma warning disable CA1416
			var searcher = new ManagementObjectSearcher("select * from Win32_" + $"{dataType}");
			var result = searcher.Get();
			foreach (var item in result)
			{
				//foreach (var PC in item.Properties)
				//{
				//	Console.Log($"{PC.Name} {PC.Value}");
				//}
				return item[name];
			}
			return default;
#pragma warning restore CA1416
		}

		public static DriveInfo GetDriveInfo(string drive = "C")
		{
			foreach (var d in System.IO.DriveInfo.GetDrives())
			{
				if (d.IsReady)
				{
					return new DriveInfo()
					{
						IsReady = true,
						AvailableGB =
						Number.FromDataSize(Number.FromDataSize(d.AvailableFreeSpace,
						Number.DataSizeConvertion.Byte_MB), Number.DataSizeConvertion.MB_GB),
						TotalGB = Number.FromDataSize(Number.FromDataSize(d.TotalSize,
						Number.DataSizeConvertion.Byte_MB), Number.DataSizeConvertion.MB_GB),
						Directory = $"{d.RootDirectory}",
						Name = d.Name,
						Type = $"{d.DriveType}"
					};
				}
				else return new DriveInfo()
					{
						IsReady = false,
						AvailableGB = 0,
						TotalGB = 0,
						Directory = null,
						Name = d.Name,
						Type = $"{d.DriveType}"
					};
			}
			return new DriveInfo();
		}
		public static string ProcessorName { get { return GetData(DataType.Processor, "Name") as string; } }
		public static string VideoCardName { get { return GetData(DataType.VideoController, "Name") as string; } }
	}
}

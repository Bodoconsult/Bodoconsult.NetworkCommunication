Release notes for Bodoconsult.App
==========================


# 1.0.0

- New package

# 1.0.1

-	Bugfixes 

# 1.0.2


# 1.0.3


# 1.0.4

-	Migrated CredentialManager implementation to Bodoconsult.App.Windows package

# 1.0.5 

-	Extracted abstractions to Bodoconsult.App.Abstractions

# 1.0.6

-	Using Bodoconsult.App.Abstractions 1.0.6

# 1.0.7

-	Using Bodoconsult.App.Abstractions 1.0.7

# 1.0.8

-	Minor enhancements for StringExtensions

-	New FileTimeExtenions and GuidExtensions classes

# 1.0.9

-   Uses Bodoconsult.App.Abstractions 1.0.9

-   MonitorLoggerFactory added for creating special purpose loggers (in addition to the default app logging). Intended to write logs for a device, a database connection or other topics requiring separate logging

-   Added IDataExportService<T>/DataExportServiceBase<T> for long running data exports to string or binary files

-   Added BufferPoolResetable<T> for reusing instances of classes implementing IResetable

-   Added MemoryStreamBufferPool for reusing instances of MemoryStream class



CREATE DATABASE [fi.retorch.com] ON  PRIMARY 
( NAME = N'fi.retorch.com', FILENAME = N'C:\Data\MSSQL13.SQLEXPRESS\MSSQL\DATA\fi.retorch.com.mdf' , SIZE = 2048KB , FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'fi.retorch.com_log', FILENAME = N'C:\Data\MSSQL13.SQLEXPRESS\MSSQL\DATA\fi.retorch.com_log.ldf' , SIZE = 1024KB , FILEGROWTH = 10%)
GO
ALTER DATABASE [fi.retorch.com] SET COMPATIBILITY_LEVEL = 100
GO
ALTER DATABASE [fi.retorch.com] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [fi.retorch.com] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [fi.retorch.com] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [fi.retorch.com] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [fi.retorch.com] SET ARITHABORT OFF 
GO
ALTER DATABASE [fi.retorch.com] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [fi.retorch.com] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [fi.retorch.com] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [fi.retorch.com] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [fi.retorch.com] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [fi.retorch.com] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [fi.retorch.com] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [fi.retorch.com] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [fi.retorch.com] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [fi.retorch.com] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [fi.retorch.com] SET  DISABLE_BROKER 
GO
ALTER DATABASE [fi.retorch.com] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [fi.retorch.com] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [fi.retorch.com] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [fi.retorch.com] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [fi.retorch.com] SET  READ_WRITE 
GO
ALTER DATABASE [fi.retorch.com] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [fi.retorch.com] SET  MULTI_USER 
GO
ALTER DATABASE [fi.retorch.com] SET PAGE_VERIFY CHECKSUM  
GO
USE [fi.retorch.com]
GO
IF NOT EXISTS (SELECT name FROM sys.filegroups WHERE is_default=1 AND name = N'PRIMARY') ALTER DATABASE [fi.retorch.com] MODIFY FILEGROUP [PRIMARY] DEFAULT
GO

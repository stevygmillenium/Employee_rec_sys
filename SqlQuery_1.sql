create database emp_rec_sys;
use emp_rec_sys;
CREATE TABLE [dbo].[Table] (
    [Id]       INT           IDENTITY (1, 1) NOT NULL,
    [name]     VARCHAR (MAX) NULL,
    [email]    VARCHAR (150) NULL,
    [phone]    VARCHAR (50)  NULL,
    [job_pos]  TEXT          NULL,
    [des_sal]  FLOAT (53)    NULL,
    [dateTime] DATETIME      NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);
CREATE TABLE [dbo].[sel_File] (
    [filename] VARCHAR (100)   NULL,
    [filetype] VARCHAR (MAX)   NULL,
    [data]     VARBINARY (MAX) NULL,
    [email]    VARCHAR (150)   NULL
);
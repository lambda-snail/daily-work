-- Script to create the entire database

create database dailywork;
use dailywork;

create table [User] (
    UserId int identity(127, 1) primary key,
    EntraId varchar(34) -- Guid
);

-- Allow manual insert of users with special ids
set identity_insert [User] ON;

create table [Todo] (
                        Id int identity(0,1) primary key,
    Title nvarchar(128) not null,
    Description nvarchar(4000) null,
    Owner int not null,
    Date date not null default GETDATE(),
    LastUpdated datetime not null default CURRENT_TIMESTAMP,
    constraint FK_Owner foreign key ([Owner]) references [User]([UserId]) on delete cascade
    );

create table TodoItem (
    Id int identity(0,1) primary key,
    IsDone bit not null,
    Text nvarchar(256) not null,
    ParentTodo int not null,
    constraint FK_ParentTodo foreign key ([ParentTodo]) references [Todo]([Id]) on delete cascade
);

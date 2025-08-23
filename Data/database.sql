-- Script to create the entire database

create database dailywork;
use dailywork;

create table [User] (
    Id uniqueidentifier primary key default NEWID(),
    ObjectId uniqueidentifier not null, -- Object Id in Entra
    DisplayName nvarchar(64) not null, -- display name
    Email nvarchar(256) not null, -- preferred_username claim

    Created datetime not null default CURRENT_TIMESTAMP
);

create unique index IND_ObjId on [User](ObjectId);
create unique index IND_Email on [User](Email);

create table [Todo] (
    Id int identity(0,1) primary key,
    Title nvarchar(128) not null,
    Description nvarchar(4000) null,
    State int not null default 0,
    Owner uniqueidentifier not null,
    Date date not null default GETDATE(),
    LastUpdated datetime not null default CURRENT_TIMESTAMP,
    constraint FK_Owner foreign key ([Owner]) references [User]([Id]) on delete cascade
);

create table [TodoItem] (
    Id int identity(0,1) primary key,
    IsDone bit not null,
    Text nvarchar(256) not null,
    ParentTodo int not null,
    constraint FK_ParentTodo foreign key ([ParentTodo]) references [Todo]([Id]) on delete cascade
);

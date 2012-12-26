/*
    Copyright (c) 2012, Jardalu LLC. (http://jardalu.com)
        
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
  
    For complete licensing, see license.txt or visit http://ratnazone.com/v0.2/license.txt

*/
-------------------------------------------------------------------------------
--
-- Tbl_Ratna_Site : Site Information
--
-------------------------------------------------------------------------------
CREATE TABLE Tbl_Ratna_Site
(
    Id              INT                 NOT NULL    IDENTITY ( 1, 1 ) PRIMARY KEY,
    Host            NVARCHAR(1024)      NOT NULL    UNIQUE,
    Title           NVARCHAR(256)       NOT NULL,
    IsActive        BIT                 DEFAULT(0),
    IsProvisioned   BIT                 DEFAULT(0)
)

GO

-------------------------------------------------------------------------------
--
-- Tbl_Ratna_User : User Information
--
-- The User Id (1) is reserved for Guest.
-- User Password : Stored as hash, there is no recovery mechanism
-------------------------------------------------------------------------------

CREATE TABLE Tbl_Ratna_User
(
    SiteId             INT             NOT NULL,
    Id                 BIGINT          NOT NULL    IDENTITY ( 1, 1 ) PRIMARY KEY,
    PrincipalId        BIGINT          NOT NULL,
    Alias              NVARCHAR(12)    NOT NULL,
    Email              NVARCHAR(64)    NOT NULL,
    Password           NVARCHAR(128),
    DisplayName        NVARCHAR(64),
    FirstName          NVARCHAR(32),
    LastName           NVARCHAR(32),
    Photo              NVARCHAR(256),
    Description        NVARCHAR(1024),
    ActivationCode     UNIQUEIDENTIFIER,
    IsActivated        BIT,
    IsDeleted          BIT,
    CreatedTime        DATETIME    NOT NULL DEFAULT(GETDATE()),
    UpdatedTime        DATETIME    NOT NULL DEFAULT(GETDATE())
)

ALTER TABLE Tbl_Ratna_User
    ADD CONSTRAINT UK_User_Alias UNIQUE (SiteId, Alias)

ALTER TABLE Tbl_Ratna_User
    ADD CONSTRAINT UK_User_Email UNIQUE (SiteId, Alias)

GO

-------------------------------------------------------------------------------
--
-- Tbl_Ratna_UserLogin : User Login Information
--
-------------------------------------------------------------------------------

CREATE TABLE Tbl_Ratna_UserLogin
(
    SiteId            INT                   NOT NULL,
    UserId            BIGINT                NOT NULL PRIMARY KEY,
    Cookie            UNIQUEIDENTIFIER      NOT NULL,
    CookieExpiry      DATETIME              NOT NULL DEFAULT(GETDATE()),
    LastLoginTime     DATETIME              NOT NULL DEFAULT(GETDATE())
)

GO


-------------------------------------------------------------------------------
--
-- Tbl_Ratna_Group : Group Information
--
-------------------------------------------------------------------------------

CREATE TABLE Tbl_Ratna_Group
(
    SiteId            INT               NOT NULL,
    Id                BIGINT            NOT NULL    IDENTITY(1,1) PRIMARY KEY,
    PrincipalId       BIGINT            NOT NULL,
    Name              NVARCHAR(24)      NOT NULL,
    Description       NVARCHAR(512)
)

ALTER TABLE Tbl_Ratna_Group
    ADD CONSTRAINT UK_Group_Name UNIQUE (SiteId, Name)

GO

-------------------------------------------------------------------------------
--
-- Tbl_Ratna_GroupMembers : GroupMembers Information
--
-- A group can contain a group as its member, however, a circular reference
-- should not be allowed.
-------------------------------------------------------------------------------

CREATE TABLE Tbl_Ratna_GroupMembers
(
    SiteId            INT               NOT NULL,
    GroupId           BIGINT            NOT NULL,
    PrincipalId       BIGINT            NOT NULL
)

ALTER TABLE Tbl_Ratna_GroupMembers
    ADD CONSTRAINT PK_GroupMembersKeyId PRIMARY KEY (SiteId, GroupId, PrincipalId)

GO

-------------------------------------------------------------------------------
--
-- Tbl_Ratna_Resource : Resource Id information
--
-- Principalid : The owner of the resource
-------------------------------------------------------------------------------

CREATE TABLE Tbl_Ratna_Resource
(
    SiteId               INT               NOT NULL,
    Id                   BIGINT            NOT NULL    IDENTITY(1,1) PRIMARY KEY,
    PrincipalId          BIGINT            NOT NULL
)

GO
-------------------------------------------------------------------------------
--
-- Tbl_Ratna_Principal : Principal such as user, group, etc
--
-------------------------------------------------------------------------------

CREATE TABLE Tbl_Ratna_Principal
(
    Id                BIGINT            NOT NULL    IDENTITY(1,1) PRIMARY KEY,
    SiteId            INT               NOT NULL,
    Name              NVARCHAR(64)      NOT NULL
)

ALTER TABLE Tbl_Ratna_Principal
    ADD CONSTRAINT UK_Principal_Name UNIQUE (SiteId, Name)

GO

-------------------------------------------------------------------------------
--
-- Tbl_Ratna_Acls : Acls for objects
--
-- ResourceId : such as article
-- Acls : Read (1) , Write (2), Delete (4)
--
-- ObjectId, ObjectType and ActorId makes an acl unique
-------------------------------------------------------------------------------

CREATE TABLE Tbl_Ratna_Acls
(
    SiteId            INT               NOT NULL,
    ResourceId        BIGINT            NOT NULL,
    PrincipalId       BIGINT            NOT NULL,
    Acls              INT               NOT NULL
)

ALTER TABLE Tbl_Ratna_Acls
    ADD CONSTRAINT PK_AclKeyId PRIMARY KEY (SiteId, [ResourceId], [PrincipalId])

GO


-------------------------------------------------------------------------------
--
-- Tbl_Ratna_Article : Articles
--
-------------------------------------------------------------------------------

CREATE TABLE Tbl_Ratna_Article
(
    SiteId                INT                   NOT NULL,
    Id                    BIGINT                NOT NULL    IDENTITY (1,1),
    ResourceId            BIGINT                NOT NULL,
    HandlerId             UNIQUEIDENTIFIER      NOT NULL,
    Title                 NVARCHAR(256)         NOT NULL,
    UrlKey                NVARCHAR(256)         NOT NULL,
    Version               INT                   NOT NULL    DEFAULT(1),
    Stage                 INT                   NOT NULL    DEFAULT(0),
    OwnerId               BIGINT                NOT NULL,
    RawData               NTEXT,
    CreatedDate           DATETIME              NOT NULL    DEFAULT(GETDATE()),
    LastModifiedDate      DATETIME              NOT NULL    DEFAULT(GETDATE()),
    PublishedDate         DATETIME              NOT NULL    DEFAULT(GETDATE())
)

ALTER TABLE Tbl_Ratna_Article
    ADD CONSTRAINT PK_ArticleId PRIMARY KEY (Id)

ALTER TABLE Tbl_Ratna_Article
    ADD CONSTRAINT FK_ArticleResourceId FOREIGN KEY (ResourceId) REFERENCES Tbl_Ratna_Resource(Id)

GO

-------------------------------------------------------------------------------
--
-- Tbl_Ratna_ArticleArchive : Articles Archive
--
-------------------------------------------------------------------------------

CREATE TABLE Tbl_Ratna_ArticleArchive
(
    SiteId                INT                   NOT NULL,
    Id                    BIGINT                NOT NULL,
    ResourceId            BIGINT                NOT NULL,
    HandlerId             UNIQUEIDENTIFIER      NOT NULL,
    Title                 NVARCHAR(256)         NOT NULL,
    UrlKey                NVARCHAR(256)         NOT NULL,
    Version               INT                   NOT NULL,
    OwnerId               BIGINT                NOT NULL,
    RawData               NTEXT,
    CreatedDate           DATETIME              NOT NULL    DEFAULT(GETDATE()),
    LastModifiedDate      DATETIME              NOT NULL    DEFAULT(GETDATE()),
    ArchivedDate          DATETIME              NOT NULL    DEFAULT(GETDATE())
)

ALTER TABLE Tbl_Ratna_ArticleArchive
    ADD CONSTRAINT FK_ArticleArchiveResourceId FOREIGN KEY (ResourceId) REFERENCES Tbl_Ratna_Resource(Id)

GO

-------------------------------------------------------------------------------
--
-- Tbl_Ratna_Media : Media such as photo, video, document etc
--
-------------------------------------------------------------------------------

CREATE TABLE Tbl_Ratna_Media
(
    SiteId                INT                   NOT NULL,
    Id                    BIGINT                NOT NULL    IDENTITY (1,1),
    ResourceId            BIGINT                NOT NULL,
    MediaType             INT                   NOT NULL,
    Name                  NVARCHAR(256)         NOT NULL,
    Url                   NVARCHAR(512)         NOT NULL    UNIQUE,
    OwnerId               BIGINT                NOT NULL,
    RawData               NTEXT,
    CreatedDate           DATETIME              NOT NULL    DEFAULT(GETDATE()),
    LastModifiedDate      DATETIME              NOT NULL    DEFAULT(GETDATE())
)

ALTER TABLE Tbl_Ratna_Media
    ADD CONSTRAINT PK_MediaId PRIMARY KEY (Id)

ALTER TABLE Tbl_Ratna_Media
    ADD CONSTRAINT FK_MediaResourceId FOREIGN KEY (ResourceId) REFERENCES Tbl_Ratna_Resource(Id)

GO


-------------------------------------------------------------------------------
--
-- Plugin(s)
--
-------------------------------------------------------------------------------

CREATE TABLE Tbl_Ratna_Plugins
(
    SiteId        INT                 NOT NULL,
    Id            UNIQUEIDENTIFIER    NOT NULL,
    Name          NVARCHAR(24)        NOT NULL,
    Type          NVARCHAR(256)       NOT NULL,
    Active        BIT                 DEFAULT(0),
    RawData       NTEXT
)

ALTER TABLE Tbl_Ratna_Plugins
    ADD CONSTRAINT PK_PluginsId PRIMARY KEY (SiteId, Id)

ALTER TABLE Tbl_Ratna_Plugins
    ADD CONSTRAINT UK_Plugins_Name UNIQUE (SiteId, Name)

GO

-------------------------------------------------------------------------------
--
-- Plugin Data : a generic way to store objects in Ratna database
--
-- for example, a template consists of multiple properties and only templateplugin
-- will understand a template. TemplatePlugin will add/delete/update the template
-- information using plugindata.
--
-------------------------------------------------------------------------------

CREATE TABLE Tbl_Ratna_PluginData
(
    SiteId              INT                  NOT NULL,
    PluginId            UNIQUEIDENTIFIER     NOT NULL,
    [Key]               NVARCHAR(128)        NOT NULL,
    Id                  NVARCHAR(128)        NOT NULL,
    UId                 UNIQUEIDENTIFIER     NOT NULL   DEFAULT(NEWID()),
    RawData             NTEXT,
    CreatedTime         DATETIME    NOT NULL DEFAULT(GETDATE()),
    UpdatedTime         DATETIME    NOT NULL DEFAULT(GETDATE())
)

ALTER TABLE Tbl_Ratna_PluginData
    ADD CONSTRAINT FK_PluginDataPluginId FOREIGN KEY (SiteId, PluginId) REFERENCES Tbl_Ratna_Plugins(SiteId, Id)

ALTER TABLE Tbl_Ratna_PluginData
    ADD CONSTRAINT PK_PluginDataKeyId PRIMARY KEY (SiteId, [Key], Id)

GO

---------------------------------------------------------------------
-- Tbl_Ratna_TagKeys
---------------------------------------------------------------------
CREATE TABLE Tbl_Ratna_TagKeys
(
    SiteId          INT                   NOT NULL,
    Id              BIGINT                NOT NULL        IDENTITY(1,1)    PRIMARY KEY,
    [Key]           UNIQUEIDENTIFIER      NOT NULL
)

GO

---------------------------------------------------------------------
-- Tbl_Ratna_Tags
--
-- KeyId and ResourceId and Tag makes an unique key
---------------------------------------------------------------------
CREATE TABLE Tbl_Ratna_Tags
(
    SiteId           INT               NOT NULL,
    KeyId            BIGINT            NOT NULL,
    ResourceId       BIGINT            NOT NULL,
    Weight           INT               NOT NULL,
    Tag              NVARCHAR(80)      NOT NULL
)

ALTER TABLE Tbl_Ratna_Tags
    ADD CONSTRAINT FK_TagsKey FOREIGN KEY (KeyId) REFERENCES Tbl_Ratna_TagKeys(Id)

ALTER TABLE Tbl_Ratna_Tags
    ADD CONSTRAINT UK_TagsUniqueness UNIQUE (KeyId, ResourceId, Tag)

GO

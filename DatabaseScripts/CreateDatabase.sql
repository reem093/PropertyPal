IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [Amenities] (
    [AmenityId] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    CONSTRAINT [PK__Amenitie__842AF50B54EE520E] PRIMARY KEY ([AmenityId])
);

CREATE TABLE [AspNetRoles] (
    [Id] nvarchar(450) NOT NULL,
    [Name] nvarchar(256) NULL,
    [NormalizedName] nvarchar(256) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
);

CREATE TABLE [AspNetUsers] (
    [Id] nvarchar(450) NOT NULL,
    [FirstName] nvarchar(max) NOT NULL,
    [LastName] nvarchar(max) NOT NULL,
    [IsAvailable] bit NOT NULL,
    [UserName] nvarchar(256) NULL,
    [NormalizedUserName] nvarchar(256) NULL,
    [Email] nvarchar(256) NULL,
    [NormalizedEmail] nvarchar(256) NULL,
    [EmailConfirmed] bit NOT NULL,
    [PasswordHash] nvarchar(max) NULL,
    [SecurityStamp] nvarchar(max) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [PhoneNumberConfirmed] bit NOT NULL,
    [TwoFactorEnabled] bit NOT NULL,
    [LockoutEnd] datetimeoffset NULL,
    [LockoutEnabled] bit NOT NULL,
    [AccessFailedCount] int NOT NULL,
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
);

CREATE TABLE [Skills] (
    [SkillId] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    CONSTRAINT [PK__Skills__DFA091877740A35A] PRIMARY KEY ([SkillId])
);

CREATE TABLE [AspNetRoleClaims] (
    [Id] int NOT NULL IDENTITY,
    [RoleId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AspNetUserClaims] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AspNetUserLogins] (
    [LoginProvider] nvarchar(450) NOT NULL,
    [ProviderKey] nvarchar(450) NOT NULL,
    [ProviderDisplayName] nvarchar(max) NULL,
    [UserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AspNetUserRoles] (
    [UserId] nvarchar(450) NOT NULL,
    [RoleId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AspNetUserTokens] (
    [UserId] nvarchar(450) NOT NULL,
    [LoginProvider] nvarchar(450) NOT NULL,
    [Name] nvarchar(450) NOT NULL,
    [Value] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Properties] (
    [PropertyId] int NOT NULL IDENTITY,
    [ManagerId] nvarchar(450) NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    [Location] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NULL,
    CONSTRAINT [PK__Properti__70C9A735FC8164CB] PRIMARY KEY ([PropertyId]),
    CONSTRAINT [FK_Properties_AspNetUsers_ManagerId] FOREIGN KEY ([ManagerId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [StaffSkills] (
    [StaffId] nvarchar(450) NOT NULL,
    [SkillId] int NOT NULL,
    CONSTRAINT [PK_StaffSkills] PRIMARY KEY NONCLUSTERED ([StaffId], [SkillId]),
    CONSTRAINT [FK_StaffSkills_AspNetUsers_StaffId] FOREIGN KEY ([StaffId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [Units] (
    [UnitId] int NOT NULL IDENTITY,
    [PropertyId] int NOT NULL,
    [UnitNumber] nvarchar(max) NOT NULL,
    [RentAmount] decimal(18,2) NOT NULL,
    [Type] int NOT NULL,
    [Status] int NOT NULL,
    [Size] decimal(18,2) NULL,
    CONSTRAINT [PK__Units__44F5ECB5C790A4F7] PRIMARY KEY ([UnitId]),
    CONSTRAINT [FK_Units_Properties] FOREIGN KEY ([PropertyId]) REFERENCES [Properties] ([PropertyId]) ON DELETE CASCADE
);

CREATE TABLE [Applications] (
    [ApplicationId] int NOT NULL IDENTITY,
    [TenantId] nvarchar(450) NOT NULL,
    [UnitId] int NOT NULL,
    [Status] int NOT NULL,
    [CreatedAt] datetime NOT NULL,
    CONSTRAINT [PK__Applicat__C93A4C99E9E00BEB] PRIMARY KEY ([ApplicationId]),
    CONSTRAINT [FK_Applications_AspNetUsers_TenantId] FOREIGN KEY ([TenantId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Applications_Units] FOREIGN KEY ([UnitId]) REFERENCES [Units] ([UnitId]) ON DELETE CASCADE
);

CREATE TABLE [Leases] (
    [LeaseId] int NOT NULL IDENTITY,
    [TenantId] nvarchar(450) NOT NULL,
    [UnitId] int NOT NULL,
    [StartDate] datetime NOT NULL,
    [EndDate] datetime NOT NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK__Leases__21FA58C10C15B77C] PRIMARY KEY ([LeaseId]),
    CONSTRAINT [FK_Leases_AspNetUsers_TenantId] FOREIGN KEY ([TenantId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Leases_Units] FOREIGN KEY ([UnitId]) REFERENCES [Units] ([UnitId]) ON DELETE CASCADE
);

CREATE TABLE [MaintenanceRequests] (
    [RequestId] int NOT NULL IDENTITY,
    [TenantId] nvarchar(450) NOT NULL,
    [UnitId] int NOT NULL,
    [Category] int NOT NULL,
    [Priority] int NOT NULL,
    [Status] int NOT NULL,
    [TicketNumber] nvarchar(50) NOT NULL,
    [Title] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NULL,
    [AssignedStaffId] nvarchar(450) NULL,
    [CreatedAt] datetime NOT NULL,
    [CompletedAt] datetime NULL,
    CONSTRAINT [PK__Maintena__33A8517AEF339621] PRIMARY KEY ([RequestId]),
    CONSTRAINT [FK_MaintenanceRequests_AspNetUsers_AssignedStaffId] FOREIGN KEY ([AssignedStaffId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_MaintenanceRequests_AspNetUsers_TenantId] FOREIGN KEY ([TenantId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_MaintenanceRequests_Units] FOREIGN KEY ([UnitId]) REFERENCES [Units] ([UnitId]) ON DELETE CASCADE
);

CREATE TABLE [UnitAmenities] (
    [UnitId] int NOT NULL,
    [AmenityId] int NOT NULL,
    CONSTRAINT [PK_UnitAmenities] PRIMARY KEY ([UnitId], [AmenityId]),
    CONSTRAINT [FK_UnitAmenities_Amenities] FOREIGN KEY ([AmenityId]) REFERENCES [Amenities] ([AmenityId]) ON DELETE CASCADE,
    CONSTRAINT [FK_UnitAmenities_Units] FOREIGN KEY ([UnitId]) REFERENCES [Units] ([UnitId]) ON DELETE CASCADE
);

CREATE TABLE [UnitImages] (
    [ImageId] int NOT NULL IDENTITY,
    [UnitId] int NOT NULL,
    [ImageUrl] nvarchar(max) NOT NULL,
    CONSTRAINT [PK__UnitImag__7516F70C68E38D06] PRIMARY KEY ([ImageId]),
    CONSTRAINT [FK_UnitImages_Units] FOREIGN KEY ([UnitId]) REFERENCES [Units] ([UnitId]) ON DELETE CASCADE
);

CREATE TABLE [Payments] (
    [PaymentId] int NOT NULL IDENTITY,
    [LeaseId] int NOT NULL,
    [Amount] decimal(18,2) NOT NULL,
    [Status] int NOT NULL,
    [DueDate] datetime NOT NULL,
    [TransactionReference] nvarchar(max) NOT NULL,
    [CreatedAt] datetime NOT NULL,
    [PaidDate] datetime NULL,
    CONSTRAINT [PK__Payments__9B556A384E409EE6] PRIMARY KEY ([PaymentId]),
    CONSTRAINT [FK_Payments_Leases] FOREIGN KEY ([LeaseId]) REFERENCES [Leases] ([LeaseId]) ON DELETE CASCADE
);

CREATE TABLE [Feedback] (
    [FeedbackId] int NOT NULL IDENTITY,
    [RequestId] int NOT NULL,
    [TenantId] nvarchar(450) NOT NULL,
    [Message] nvarchar(max) NOT NULL,
    [Rating] int NOT NULL,
    CONSTRAINT [PK__Feedback__6A4BEDD627FE7AC4] PRIMARY KEY ([FeedbackId]),
    CONSTRAINT [FK_Feedback_MaintenanceRequests] FOREIGN KEY ([RequestId]) REFERENCES [MaintenanceRequests] ([RequestId]) ON DELETE CASCADE
);

CREATE TABLE [MaintenanceAttachments] (
    [AttachmentId] int NOT NULL IDENTITY,
    [RequestId] int NOT NULL,
    [FileUrl] nvarchar(max) NOT NULL,
    CONSTRAINT [PK__Maintena__442C64BE1EA89ECB] PRIMARY KEY ([AttachmentId]),
    CONSTRAINT [FK_MaintenanceAttachments_MaintenanceRequests] FOREIGN KEY ([RequestId]) REFERENCES [MaintenanceRequests] ([RequestId]) ON DELETE CASCADE
);

CREATE TABLE [MaintenanceUpdates] (
    [UpdateId] int NOT NULL IDENTITY,
    [RequestId] int NOT NULL,
    [UpdatedBy] nvarchar(450) NOT NULL,
    [Notes] nvarchar(max) NULL,
    [CreatedAt] datetime NOT NULL,
    CONSTRAINT [PK__Maintena__7A0CF3C5C52ABA28] PRIMARY KEY ([UpdateId]),
    CONSTRAINT [FK_MaintenanceUpdates_AspNetUsers_UpdatedBy] FOREIGN KEY ([UpdatedBy]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_MaintenanceUpdates_MaintenanceRequests] FOREIGN KEY ([RequestId]) REFERENCES [MaintenanceRequests] ([RequestId]) ON DELETE CASCADE
);

CREATE TABLE [Notifications] (
    [NotificationId] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [RequestId] int NULL,
    [Message] nvarchar(max) NOT NULL,
    [CreatedAt] datetime NOT NULL,
    CONSTRAINT [PK__Notifica__20CF2E1266919A12] PRIMARY KEY ([NotificationId]),
    CONSTRAINT [FK_Notifications_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Notifications_MaintenanceRequests] FOREIGN KEY ([RequestId]) REFERENCES [MaintenanceRequests] ([RequestId]) ON DELETE CASCADE
);

CREATE INDEX [IX_Applications_TenantId] ON [Applications] ([TenantId]);

CREATE INDEX [IX_Applications_UnitId] ON [Applications] ([UnitId]);

CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);

CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;

CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);

CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);

CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);

CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);

CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;

CREATE INDEX [IX_Feedback_RequestId] ON [Feedback] ([RequestId]);

CREATE INDEX [IX_Feedback_TenantId] ON [Feedback] ([TenantId]);

CREATE INDEX [IX_Leases_TenantId] ON [Leases] ([TenantId]);

CREATE INDEX [IX_Leases_UnitId] ON [Leases] ([UnitId]);

CREATE INDEX [IX_MaintenanceAttachments_RequestId] ON [MaintenanceAttachments] ([RequestId]);

CREATE INDEX [IX_MaintenanceRequests_AssignedStaffId] ON [MaintenanceRequests] ([AssignedStaffId]);

CREATE INDEX [IX_MaintenanceRequests_Status_Priority] ON [MaintenanceRequests] ([Status], [Priority]);

CREATE INDEX [IX_MaintenanceRequests_TenantId] ON [MaintenanceRequests] ([TenantId]);

CREATE INDEX [IX_MaintenanceRequests_UnitId] ON [MaintenanceRequests] ([UnitId]);

CREATE UNIQUE INDEX [UQ__Maintena__CBED06DA532A45EB] ON [MaintenanceRequests] ([TicketNumber]);

CREATE INDEX [IX_MaintenanceUpdates_RequestId] ON [MaintenanceUpdates] ([RequestId]);

CREATE INDEX [IX_MaintenanceUpdates_UpdatedBy] ON [MaintenanceUpdates] ([UpdatedBy]);

CREATE INDEX [IX_Notifications_RequestId] ON [Notifications] ([RequestId]);

CREATE INDEX [IX_Notifications_UserId] ON [Notifications] ([UserId]);

CREATE INDEX [IX_Payments_LeaseId] ON [Payments] ([LeaseId]);

CREATE INDEX [IX_Properties_ManagerId] ON [Properties] ([ManagerId]);

CREATE INDEX [IX_StaffSkills_SkillId] ON [StaffSkills] ([SkillId]);

CREATE INDEX [IX_UnitAmenities_AmenityId] ON [UnitAmenities] ([AmenityId]);

CREATE INDEX [IX_UnitImages_UnitId] ON [UnitImages] ([UnitId]);

CREATE INDEX [IX_Units_PropertyId] ON [Units] ([PropertyId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260506134846_InitialCreate', N'9.0.13');

COMMIT;
GO


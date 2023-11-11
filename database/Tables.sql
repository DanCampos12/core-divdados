-- Postgres -> pgAdmin 4
create table "Users" ( 
	  "Id" uuid not null
	, "Name" varchar(50) not null
	, "Sex" char(1) not null check ("Sex" in ('F','M','O'))
    , "BirthDate" date not null
	, "Email" varchar(100) not null
	, "Password" varchar(500) not null
    , "Effected" boolean not null
    , "FlowComplete" boolean not null
);
alter table "Users" add constraint "PK_User_Id" primary key("Id");

create table "Preferences" ( 
	  "Id" uuid not null
    , "Dark" boolean not null
    , "DisplayValues" boolean not null
    , "UserId" uuid not null
);
alter table "Preferences" add constraint "PK_Preference_Id" primary key("Id");
alter table "Preferences" add constraint "FK_Preference_UserId" foreign key("UserId") references "Users"("Id");

create table "Categories" ( 
      "Id" uuid not null
    , "Name" varchar(50) not null
    , "Color" varchar(7) not null
    , "IsAutomaticInput" boolean not null
    , "MaxValueMonthly" numeric(20, 8) null
    , "UserId" uuid not null
);
alter table "Categories" add constraint "PK_Category_Id" primary key("Id");
alter table "Categories" add constraint "FK_Category_UserId" foreign key("UserId") references "Users"("Id");

create table "Events" ( 
	  "Id" uuid not null
    , "Value" numeric(20, 8) not null
    , "Type" char(1) not null check ("Type" in ('I', 'O'))
    , "Description" varchar(50) not null
    , "InitialDate" date not null
    , "FinalDate" date not null
    , "Period" char(3) not null check ("Period" in ('7D', '15D', '1M', '2M', '3M', '6M', '9M', '1Y'))
    , "UserId" uuid not null
    , "CategoryId" uuid not null
);
alter table "Events" add constraint "PK_Event_Id" primary key("Id");
alter table "Events" add constraint "FK_Event_UserId" foreign key("UserId") references "Users"("Id");
alter table "Events" add constraint "FK_Event_CategoryId" foreign key("CategoryId") references "Categories"("Id");

create table "Operations" ( 
	  "Id" uuid not null
    , "Value" numeric(20, 8) not null
    , "Type" char(1) not null check ("Type" in ('I', 'O'))
    , "Description" varchar(50) not null
    , "Date" date not null
    , "Effected" boolean not null
    , "UserId" uuid not null
    , "CategoryId" uuid not null
    , "EventId" uuid
);
alter table "Operations" add constraint "PK_Operation_Id" primary key("Id");
alter table "Operations" add constraint "FK_Operation_UserId" foreign key("UserId") references "Users"("Id");
alter table "Operations" add constraint "FK_Operation_CategoryId" foreign key("CategoryId") references "Categories"("Id");
alter table "Operations" add constraint "FK_Operation_EventId" foreign key("EventId") references "Events"("Id");

create table "Objectives" ( 
	  "Id" uuid not null
    , "Value" numeric(20, 8) not null
    , "Description" varchar(50) not null
    , "InitialDate" date not null
    , "FinalDate" date not null
    , "Status" varchar(20) not null
    , "Order" int not null
    , "UserId" uuid not null
);
alter table "Objectives" add constraint "FK_Objective_UserId" foreign key("UserId") references "Users"("Id");

create table "Notifications" ( 
	  "Id" uuid not null
    , "Type" varchar(30) not null
    , "Date" timestamp not null
    , "Title" varchar(50) not null
    , "Message" varchar(100) not null
    , "Read" boolean not null
    , "Removed" boolean not null
    , "UserId" uuid not null
    , "ExternalId" uuid
);
alter table "Notifications" add constraint "PK_Notification_Id" primary key("Id");
alter table "Notifications" add constraint "FK_Notification_UserId" foreign key("UserId") references "Users"("Id");
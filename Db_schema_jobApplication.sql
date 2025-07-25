-- CREATE TABLE QUERIES 

CREATE TABLE "Roles" (
   	"Id" SERIAL PRIMARY KEY,
   	"Name" VARCHAR(20) NOT NULL
);

CREATE TABLE "Users" (
	"Id" SERIAL PRIMARY KEY,
	"Email" VARCHAR(100) NOT NULL UNIQUE,
	"Password" VARCHAR(255) NOT NULL,
	"RoleId" INTEGER NOT NULL REFERENCES "Roles"("Id")
);

CREATE TABLE "Employers" (
	"Id" SERIAL PRIMARY KEY,
	"Name" VARCHAR(50) NOT NULL,
	"CompanyName" VARCHAR(100) NOT NULL,
	"UserId" INTEGER NOT NULL UNIQUE REFERENCES "Users"("Id")
);

CREATE TABLE "Candidates" (
	"Id" SERIAL PRIMARY KEY,
	"Name" VARCHAR(50) NOT NULL,
	"UserId" INTEGER NOT NULL UNIQUE REFERENCES "Users"("Id")
);

CREATE TABLE "Categories" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE "Jobs" (
	"Id" SERIAL PRIMARY KEY,
	"Title" VARCHAR(50) NOT NULL,
    "Description" TEXT NOT NULL,
    "Location" VARCHAR(100) NOT NULL,
    "ExperienceRequired" INTEGER DEFAULT 0,
    "EmployerId" INTEGER NOT NULL REFERENCES "Employers"("Id"),
    "OpenFrom" DATE NOT NULL,
    "CategoryId" INTEGER NOT NULL REFERENCES "Categories"("Id"),
    "Vacancy" INTEGER NOT NULL DEFAULT 1,
    "CreatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "IsActive" BOOLEAN NOT NULL DEFAULT TRUE,
    "IsDeleted" BOOLEAN NOT NULL DEFAULT FALSE
);

CREATE TABLE "Skills" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE "JobSkills" (
    "Id" SERIAL PRIMARY KEY,
    "JobId" INTEGER NOT NULL REFERENCES "Jobs"("Id") ON DELETE CASCADE,
    "SkillId" INTEGER NOT NULL REFERENCES "Skills"("Id") ON DELETE CASCADE,
    UNIQUE ("JobId", "SkillId")
);

CREATE TABLE "Status" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(50) NOT NULL
);

CREATE TABLE "Applications" (
    "Id" SERIAL PRIMARY KEY,
    "CandidateId" INTEGER NOT NULL REFERENCES "Candidates"("Id"),
    "JobId" INTEGER NOT NULL REFERENCES "Jobs"("Id"),
    "Experience" INTEGER NOT NULL DEFAULT 0,
    "NoteForEmployer" TEXT,
    "CoverLetter" TEXT NOT NULL,
    "AppliedDate" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "StatusId" INTEGER NOT NULL REFERENCES "Status"("Id")
);

CREATE TABLE "JobPreference" (
    "Id" SERIAL PRIMARY KEY,
    "CandidateId" INTEGER NOT NULL REFERENCES "Candidates"("Id"),
    "CategoryId" INTEGER NOT NULL REFERENCES "Categories"("Id"),
    "ExperienceRequired" INTEGER DEFAULT 0,
    "Location" VARCHAR(100) NOT NULL,
    "CreatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);


-- Insert initiall data

INSERT INTO "Roles" ("Name") VALUES
('Admin'),
('Employer'),
('Candidate');

INSERT INTO "Skills" ("Name") VALUES
('.NET'),
('Python'),
('Buisness'),
('Azure');

INSERT INTO "Categories" ("Name") VALUES
('Tech'),
('Buisness'),
('Research'),
('Chemical');

INSERT INTO "Status" ("Name") VALUES
('Rejected'),
('Hired'),
('Shortlisted'),
('Applied'),
('Withdrawn');
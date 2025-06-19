# JobApplicationPortal

JobApplicationPortal is a .NET Web API project designed to facilitate job applications, employer job postings, and user management for both candidates and employers. It provides a robust set of APIs for user authentication, job management, application tracking, and profile management, with role-based access control for Admins, Employers, and Candidates.

## Table of Contents

- Features
- API Endpoints
- Technologies
- Setup Instructions
- Database Setup
- Git Repository
- Contributing

## Features

- User registration and authentication for Employers and Candidates.
- Job creation, updating, and deletion by Employers.
- Job application management for Candidates, including applying, withdrawing, and viewing application history.
- Mail sent to candidate on updation of there application.
- Mail sent to employer when any job of there is applied.
- Admin functionalities to view all users, jobs, and applications.
- Search, pagination, and filtering for job listings by skills, location, experience, and category.
- Profile management for both Employers and Candidates.
- Job preference management for Candidates.
- Mail sent to all candidate on job creation with same preference.
- Role-based access control for secure API usage.
- Validation performed through Fluent validator


## API Endpoints

- Api rate is limited to 10 request/ 10 seconds with 2 can be in queue for each user.

| **Endpoint** | **Method** | **Access** | **Description** |
| --- | --- | --- | --- |
| `api/Auth/register/employer` | POST | All | Register a new employer. |
| `api/Auth/register/candidate` | POST | All | Register a new candidate. |
| `api/Auth/login` | POST | All | Authenticate a user and return a JWT token. |
| `api/get/employers` | GET | Admin | Retrieve a list of all employers. |
| `api/get/candidates` | GET | Admin | Retrieve a list of all candidates. |
| `api/create/job` | POST | Employer | Create a new job posting. |
| `api/get/created-jobs` | GET | Employer | Retrieve jobs created by the authenticated employer. |
| `api/get/job-by-id` | GET | Employer | Retrieve a specific job by ID. |
| `api/update/job` | PUT | Employer | Update a job before it is open for applications. |
| `api/delete/job` | PUT | Employer | Soft delete a job. |
| `api/get/jobs-by-employer` | GET | Admin | Retrieve all jobs posted by a specific employer. |
| `api/get/job` | GET | Admin, Candidate | Retrieve all jobs with search, pagination, and filtering (Skill, Location, etc.). |
| `api/job/application` | POST | Candidate | Apply for a job. |
| `api/get/applications-by-candidate` | GET | Candidate | View job application history for the authenticated candidate. |
| `api/get/applications` | GET | Admin | Retrieve all job applications. |
| `api/get/applications-by-job` | GET | Employer | View applications for a specific job. |
| `api/application/change-status` | PUT | Employer | Update the status of a job application. |
| `api/get/job/total-applications` | GET | Admin, Employer | Get the total number of applications for a specific job. |
| `api/get/statuses` | GET | Employer, Admin | Retrieve all possible application statuses. |
| `api/get/skills` | GET | All | Retrieve all available skills. |
| `api/application/withdraw-application` | PUT | Candidate | Withdraw a job application. |
| `api/get/profile` | GET | Candidate, Employer | Retrieve the authenticated user's profile. |
| `api/update/employer-profile` | PUT | Employer | Update the employer's profile. |
| `api/update/candidate-profile` | PUT | Candidate | Update the candidate's profile. |
| `api/get/categories` | GET | All | Retrieve all job categories. |
| `api/create/job-preference` | POST | Candidate | Create job preferences for a candidate. |
| `api/update/job-preference` | PUT | Candidate | Update job preferences for a candidate. |
| `api/delete/job-preference` | DELETE | Candidate | Delete job preferences for a candidate. |

## Technologies

- **Framework**: .NET Web API
- **Database**: PostgreSQL
- **ORM**: Entity Framework Core
- **Authentication**: JWT (JSON Web Tokens)
- **Packages**:
  - `Microsoft.AspNetCore.Authentication.JwtBearer`
  - `Microsoft.AspNetCore.JsonPatch`
  - `Microsoft.AspNetCore.Mvc.NewtonsoftJson`
  - `Microsoft.AspNetCore.OpenApi`
  - `Microsoft.EntityFrameworkCore.Design`
  - `Npgsql.EntityFrameworkCore.PostgreSQL`
  - `Npgsql.EntityFrameworkCore.PostgreSQL.Design`

## Setup Instructions

1. **Clone the Repository**:

   ```bash
   git clone https://github.com/naman-tatvasoft/JobApplicationPortal.git
   cd JobApplicationPortal
   ```

2. **Install Dependencies**: Ensure you have the .NET SDK installed. Then, restore the required NuGet packages:

   ```bash
   dotnet restore
   ```

3. **Configure the Database**: Update the connection string in `appsettings.json` to match your PostgreSQL database configuration:

   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Host=localhost;Database=Job_Application_Portal;Username=postgres;Password=Tatva@123"
   }
   ```

4. **Run the Application**:

   ```bash
   dotnet run
   ```

5. **Access the API**: The API will be available at `https://localhost:<port>` (port is typically 5001 or as configured).

## Database Setup

To scaffold the database context and generate models:

```bash
cd ..
cd JobApplicationPortal.DataModels/
dotnet ef dbcontext scaffold "Host=localhost;Database=Job_Application_Portal;Username=postgres;Password=Tatva@123" Npgsql.EntityFrameworkCore.PostgreSQL -o Models -f
```

Ensure PostgreSQL is running and the database `Job_Application_Portal` is created before running the command.

## Git Repository

The source code is available at: https://github.com/naman-tatvasoft/JobApplicationPortal/

## Contributing

1. Fork the repository.
2. Create a new branch (`git checkout -b feature-branch`).
3. Make your changes and commit (`git commit -m "Add feature"`).
4. Push to the branch (`git push origin feature-branch`).
5. Create a Pull Request.
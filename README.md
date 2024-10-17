# My Claims App

## Overview
**My Claims App** is a prototype web application designed to facilitate the management of lecturer claims within an academic institution. This prototype emphasizes core functionalities such as claims submission, review, and approval by different user roles: lecturers, coordinators, and managers. While focused on the front-end and essential back-end processes, the system lays the foundation for future automation of the entire claims approval and payment pipeline. 

This README outlines the system's current features, sprint-based development process, and next steps for further enhancements.

---

## Table of Contents
- [Overview](#overview)
- [Sprints](#sprints)
  - [Sprint Part 1](#sprint-part-1-project-planning-and-documentation)
  - [Sprint Part 2](#sprint-part-2-developing-the-web-application-prototype)
  - [Sprint Part 3](#sprint-part-3-planning-for-future-enhancements)
- [Cloning the Project and Restoring the Database](#cloning-the-project-and-restoring-the-database)
- [Assumptions for Usage](#assumptions-for-usage)
- [Application Features and Usage](#application-features-and-usage)
- [Future Requirements](#future-requirements)
- [Code Attribution](#code-attribution)

---

## Sprints

### Sprint Part 1: Project Planning and Documentation
**Deliverables:**
- UML Class Diagram outlining the database structure.
- Project plan, including timeline and milestones.
- Initial wireframes and GUI design for user interactions.

**Goal:**  
This sprint focuses on laying out a clear vision for the application, ensuring the technical structure aligns with the project’s goals. The UML class diagram provides insight into how the data is organized, and the GUI design sets the stage for user experience.

---

### Sprint Part 2: Developing the Web Application Prototype
**Deliverables:**
- Implementation of an MVC ASP.NET Core web application.
- Integration with SQL Server for data management.
- Role-based workflows for lecturers, coordinators, and managers.

**Features Implemented:**
- **Lecturer Claims Submission:** A streamlined form where lecturers can input hours worked, hourly rate, and additional notes.
- **Document Upload:** Lecturers can attach PDF documents (up to 15MB) as proof of work or contracts.
- **Approval Workflow:** Coordinators and managers review and approve claims, with real-time status updates.
- **Claim Status Tracking:** Lecturers can monitor their claims as they move through "Pending," "Approved," and "Rejected" stages.
- **Unit Testing:** Validation for claim submission, document uploads, and error handling.

---

### Sprint Part 3: Planning for Future Enhancements
**Planned Features:**
- **Automated Payment Processing:** Automatically calculate and initiate payments based on approved claims.
- **Claim Verification:** Systematically verify claims against contract terms and hours worked.
- **Reporting and HR Integration:** Enable HR to generate reports summarizing claims for further processing.
- **Payment Gateway Integration:** Seamlessly link claim approvals with payment platforms for fast disbursements.

---

## Cloning the Project and Restoring the Database

### Steps to Get Started:
1. Clone the repository:
    ```bash
    git clone https://github.com/st10251759/prog6212-poe-part-2.git
    ```
2. Open the project in Visual Studio and restore necessary NuGet packages.
3. Ensure SQL Server Management Studio (SSMS) is installed and running.
4. Restore the provided database:
   - Open SSMS and connect to your server.
   - Right-click on "Databases," select "Restore Database," and follow the prompts using the `.bak` file included in the repository.
5. Update the `appsettings.json` file to include your local database configuration.

---

## Assumptions for Usage
- **User Roles:** The system supports three user types—Lecturers, Coordinators, and Managers—each with distinct permissions and access to relevant parts of the system.
- **Claim Submission Rules:**
  - Hourly rates must range between R200 and R1000.
  - Lecturers can submit claims for up to 150 hours of work per month.
- **Time Restrictions:** Claims can only be made for the current or previous month. Future claims are disallowed.
- **Supporting Documents:** Every submission must include a PDF with proof of work, such as contracts or hourly rate agreements. These are reviewed before approval.

---

## Application Features and Usage

### Claim Submission
- **Form Submission:** Lecturers submit claims by specifying hours worked, hourly rate, and any additional notes.
- **File Upload:** PDF files (max 15MB) can be attached to the claim as proof of contract or hourly rate.

### Approval Workflow
- **Coordinator:** Reviews and approves/rejects claims. Approved claims are forwarded to the manager.
- **Manager:** Provides final approval. Fully approved claims are marked as "Approved."

### Claim Status Tracking
Lecturers can view their claim’s status in real-time as it moves through the approval stages ("Pending", "Approved", "Rejected").

### Document Management
Uploaded files are securely stored and linked to the claim. The system only accepts PDF files and enforces a file size limit of 15MB.

---

## Future Requirements
1. **Automated Payments:** Once claims are approved, the system should calculate and automate payment processing.
2. **Claim Verification:** Implement automated checks to ensure submitted claims comply with contract terms.
3. **Reporting:** HR should be able to generate detailed reports summarizing approved claims for payroll.
4. **Payment Gateway Integration:** Seamless integration with third-party payment providers to handle claim payouts.

---

## Code Attribution
- **HTML & CSS:**
  - **Author:** w3schools
  - **Date Accessed:** 12 October 2024
- **ASP.NET Core Application:**
  - **Author:** Fatima Shaik
  - **Link:** EmployeeLeaveManagement_G1
  - **Date Accessed:** 11 October 2024
- **Database and LINQ:**
  - **Author:** Microsoft Documentation
  - **Link:** Working with SQL
  - **Date Accessed:** 11 October 2024
- **File Handling & Identity Integration:**
  - **Author:** Fatima Shaik
  - **Link:** FileHandlingApp, Employee_Management_LINQ_FileHandling_G1
  - **Date Accessed:** 11 October 2024
- **Microsoft Identity:**
  - **Author:** Andy Malone MVP
  - **Link:** Microsoft Identity Tutorial
  - **Date Accessed:** 11 October 2024

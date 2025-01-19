# SocialSync

**SocialSync** is a powerful social media management tool designed to simplify and streamline the management of social media accounts. With features like post creation, scheduling, analytics, and platform integration, SocialSync empowers users to effectively manage their social presence across platforms like LinkedIn,Instagram, and others.

---

## Features

- **Create Posts**: Craft engaging posts for your social media accounts.
- **Schedule Posts**: Plan and schedule posts for future publication.
- **List Scheduled Posts**: View and manage all your scheduled posts.
- **My Pages**: Access administrative pages for connected social media platforms.
- **Connect Social Media Accounts**: Seamlessly integrate and manage multiple accounts, including LinkedIn,Instagram,etc.
- **View Latest Posts and Engagements**: Monitor your recent posts and their performance, including likes, comments, and shares.

---

## Tech Stack

- **Backend**: ASP.NET Core 8
- **Database**: SQL Server
- **Frontend**: Angular.js, Bootstrap and scss.
- **Social Media APIs**: Integration with LinkedIn platforms.

---

## Installation

Follow these steps to set up and run the project locally:

1. Clone the repository:
   ```bash
   git clone https://github.com/username/socialsync.git
   `````
2. SocialSync-Server:
   ### Prerequisites
   Before starting, ensure you have the following installed on your system:
   **.NET Core SDK** (version 8 or later)
   **SQL Server**
   - An IDE supporting .NET development (e.g., Visual Studio, JetBrains Rider, or Visual Studio Code)
   ### Steps
   
   **1. Navigate to the project directory:**
   
   After cloning the repository, move into the project folder using the following command:

   ```bash
   cd SocialSync-Server
   `````
   **2. Restore dependencies:**
  
   ```bash
   dotnet restore
   `````

   **3. Set up the database:**
       - Open the appsettings.json file located in the project directory and update the connection string to match your SQL Server instance.
       - Apply migrations to create the database schema:
   ```bash
   dotnet ef database update
   `````
   
   **4. Run the application:**
      - Start the application by executing the following command:
   ```bash
   dotnet run
   `````

3. SocialSync-Client:

   ## Prerequisites
   Ensure that you have the following software installed:
   **Node.js**: [Download and install Node.js](https://nodejs.org/)
   **npm**: npm is automatically installed with Node.js.
   ## Steps to Start the Application

   ### Steps

   **1. Navigate to the project directory:**
   
   ```bash
   cd SocialSync-Client
   `````
   **2. Install dependencies:**
   ```bash
   npm install
   `````
   **3. Run the application:**
   ```bash
   ng serve
   `````
   
  



 

  
 



   
   


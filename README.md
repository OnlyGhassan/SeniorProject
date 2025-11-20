
# **Intelligent Virtual Reality Job Interview Simulator**

The **Intelligent VR Job Interview Simulator** is a graduation project that blends **Artificial Intelligence (AI)** and **Virtual Reality (VR)** technologies to create realistic, interactive job interview simulations. Users upload their CV, choose a job position, and interact with AI interviewers who ask personalized questions and generate a performance report.

Even without a VR headset, you can still **run the system**, **test all backend functionalities**, and **open the game in desktop mode** to explore the environment.

---

# 🚨 **Important Notes Before You Start**

* The system downloads and runs the model **meta-llama/Llama-3.2-1B locally**, which requires:

  * **An NVIDIA GPU**
  * **CUDA support**
  * Sufficient GPU memory to run both the **local AI model** and the **3D game viewer**

* You will need the following installed on your machine:

  * **Python 3.10+**
  * **FastAPI** (installed via requirements.txt)
  * **.NET SDK (net9.0)**
  * **Microsoft SQL Server**
  * **SQL Server Management Studio (SSMS)**
  * **NVIDIA GPU drivers + CUDA Toolkit**

* No physical VR headset is required to test or run the project.

---

# 🎯 **Project Features**

* AI-powered interviewer behavior
* CV analysis (experience, skills, specialty extraction)
* Job-specific interview question generation
* Real-time voice-based interaction
* Detailed performance evaluation report
* 3D interactive interview environment

---

# 🚀 **How the Project Works**

1. Upload your CV
2. Choose the job position
3. AI analyzes the CV
4. Interview starts and questions are asked dynamically
5. User responds (voice or text)
6. System generates a performance report

---

# 🛠️ **Full Installation & Setup Guide**

Follow the steps carefully to run the entire system.

---

# **1️⃣ Clone the Repository**

Open a terminal and run:

```bash
git clone https://github.com/OnlyGhassan/SeniorProject.git
cd SeniorProject
```

This downloads all project folders, including:

* **FastApi/**
* **AIModels/**
* **SenioProject/**
* Additional backend files

---

# **2️⃣ Python Environment Setup (FastAPI Backend)**

### **Step 1 — Create a virtual environment**

```bash
python -m venv .env
```

### **Step 2 — Activate it**

* **Windows**

  ```bash
  .env\Scripts\activate
  ```
* **Mac/Linux**

  ```bash
  source .env/bin/activate
  ```

### **Step 3 — Install all Python dependencies**

```bash
cd FastApi
pip install -r requirements.txt
```

This installs:

* FastAPI
* CV parsing models
* Dependencies for running Llama-3.2-1B locally
* Additional utility packages

---

# **3️⃣ Run the AI Model Service**

### **Step 1 — Move into the AIModels directory**

```bash
cd AIModels
```

### **Step 2 — Run the FastAPI service**

```bash
fastapi run AIModels.py
```

What happens here:

* The local AI model (**meta-llama/Llama-3.2-1B**) begins downloading (first run only).
* GPU + CUDA are used to load and run the model.
* This service generates interview questions and analyzes CVs.

---

# **4️⃣ .NET API Setup (Main Backend)**

### **Step 1 — Go back to the main folder**

```bash
cd ..
cd ..
cd SenioProject
```

### **Step 2 — Restore .NET dependencies**

```bash
dotnet restore
```

Your `.csproj` file includes packages like:

* Entity Framework Core
* SQL Server Provider
* Swagger
* Newtonsoft.Json

### **Step 3 — Configure SQL Server Database**

#### 1. Open **SSMS**

#### 2. Create a **new database** (example: `InterviewDB`)

#### 3. Open **appsettings.json** and update this line:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=InterviewDB;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

Replace `YOUR_SERVER` with:

* `(localdb)\\MSSQLLocalDB`, or
* Your SQL instance name

### **Step 4 — Apply migrations**

```bash
dotnet ef database update
```

This creates all required tables automatically.

### **Step 5 — Run the .NET backend**

```bash
dotnet run
```

This starts:

* Database API
* CV endpoints
* Interview preparation endpoints
* Swagger UI

---

# **5️⃣ Download and Run the Game (Desktop Mode — No VR Required)**

### **Step 1 — Download the game files**

Game download (too large for GitHub):
🔗 [https://drive.google.com/drive/folders/1woVVXiJGPvg9HrXT4fY4-YuqfZG-bnVa?usp=sharing](https://drive.google.com/drive/folders/1woVVXiJGPvg9HrXT4fY4-YuqfZG-bnVa?usp=sharing)

### **Step 2 — Extract the .rar file**

You should get a folder named **full game 5**.

### **Step 3 — Run the game**

Go to:

```
VRInterviewGame.rar\full game 5\
```

Open:

```
AI VR Interview Simulation.exe
```

The game will launch in **desktop mode** if a VR headset is not detected.

---

# 📌 **Important Startup Order**

To run the full system correctly, start the services in this order:

### 1. **AI Model Service**

```
cd FastApi/AIModels
fastapi run AIModels.py
```

### 2. **.NET Core API**

```
cd SenioProject
dotnet run
```

### 3. **Open the Game (.exe)**

From the extracted folder.

---

# 📚 **Troubleshooting**

### **Model is slow or not loading**

* Check if CUDA is installed
* Ensure you have an NVIDIA GPU
* Try updating GPU drivers

### **Database errors**

* Recheck connection string
* Ensure SQL Server is running
* Run migrations again

### **Game won’t open**

* Extract the .rar fully
* Update GPU drivers
* Try running as administrator

---

# 🎉 **You’re Ready to Explore the Project!**

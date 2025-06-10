# 🏥 SmartClinic - Microservices-Based Appointment System

SmartClinic is a microservices-based system built with .NET 8 designed to manage user profiles, appointments, and notifications in a scalable and maintainable architecture. This project demonstrates clean separation of concerns using **CQRS**, **Event-Driven Communication**, and **Dockerized Services**.

---

## ⚙️ Technologies Used

- **.NET 8**
- **C#**
- **ASP.NET Core Web API**
- **Entity Framework Core**
- **MongoDB**
- **Redis**
- **RabbitMQ**
- **Docker & Docker Compose**
- **Microservices Architecture**
- **CQRS Pattern**
- **RESTful APIs**

---

## 📦 Microservices

| Service                | Description                                      |
|------------------------|--------------------------------------------------|
| `UserService`          | Handles user registration and authentication     |
| `AppointmentService`   | Manages appointment creation, rescheduling, etc. |
| `NotificationService`  | Listens to events and sends notifications        |
| `NotificationGateway`  | API Gateway to expose NotificationService        |

Each service runs in a separate Docker container and communicates via RabbitMQ (event-driven).

---

## 📁 Project Structure

```bash
SmartClinicSolution/
│
├── docker-compose.yml
├── Src/
│   ├── Services/
│   │   ├── AppointmentService/
│   │   │   ├── AppointmentService.API/
│   │   │   ├── AppointmentService.Application/
│   │   │   ├── AppointmentService.Infrastructure/
│   │   │   └── AppointmentService.Domain/
│   │   ├── UserService/
│   │   ├── NotificationService/
│   │   └── NotificationGateway/
│   └── Shared/
 Getting Started
1. Prerequisites
Docker Desktop

.NET 8 SDK

Git

2. Clone the Repository

git clone https://github.com/your-username/SmartClinicSolution.git
cd SmartClinicSolution

3. Build and Run the Project

docker-compose build --no-cache
docker-compose up

4. Access Services


UserService API       	http://localhost:5001
AppointmentService	  http://localhost:5002
NotificationGateway	  http://localhost:5003

Architecture Overview:

CQRS separates read and write models for maintainability and scalability.

RabbitMQ is used for asynchronous communication between services.

MongoDB is used for logging and operational data in NotificationService.

Redis is used for caching available appointment slots.

✨ Features Implemented
✅ User Registration and Login (with JWT)

✅ Create and Reschedule Appointments

✅ View Available Appointment Slots

✅ Event Publishing via RabbitMQ

✅ Notification Service Consumes Events

✅ Dockerized Services with Docker Compose

📌 Author
👤 Soroush Jalali
📧 Soroushjalali1998@gmail.com
💼 Backend Developer | .NET Microservices | CQRS


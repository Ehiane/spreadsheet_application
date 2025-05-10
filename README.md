# Spreadsheet Engine – CPTS 321 Project

This is a spreadsheet application built from scratch in C# using .NET WinForms. Developed as a semester project for Washington State University's **CPTS 321: Object-Oriented Software Principles**, the application emulates key spreadsheet behaviors such as cell editing, formula parsing, dependency tracking, and dynamic UI updates.
---
## Demo 📹
https://github.com/user-attachments/assets/e2469001-03f1-4ea7-a447-b430640c0ea8

---




## 🚀 Features

- **Cell Expression Parsing:**  
  Supports formulas (e.g. `A1+B2`) with binary operations using a custom-built expression tree and evaluation engine.

- **Cell Dependency Tracking:**  
  Automatically tracks cell references and updates dependent values on change.

- **Undo/Redo Functionality:**  
  Custom command pattern implementation for seamless undo/redo of text and formatting operations.

- **File I/O:**  
  Save and load spreadsheets via `.csv` with full cell data retention.

- **Cell Formatting:**  
  Users can change background color of individual cells to enhance visibility.

- **Performance-Optimized Event Handling:**  
  Efficient internal update propagation prevents circular references and ensures consistent state.

---

## 🧪 Testing

Extensive unit testing using `NUnit` framework covering:
- Formula evaluation
- Dependency propagation
- Undo/redo history
- File save/load integrity

---
## 🛠️ Tech Stack

- **Language:** C#  
- **Framework:** .NET WinForms  
- **Testing:** NUnit  
- **Architecture:** MVC-inspired with custom command pattern and event-driven updates

---

## 📚 Learning Outcomes

- Object-oriented design and principles (encapsulation, inheritance, polymorphism)  
- UI design using WinForms  
- Practical application of data structures (trees, stacks, dictionaries)  
- Test-driven development and custom undo/redo systems

---

## 📁 Folder Structure

/SpreadsheetApp
│
├── SpreadsheetEngine # Core logic for cell management, formula evaluation
├── UI # WinForms-based user interface
├── Commands # Undo/redo command classes
├── Tests # NUnit unit tests
└── README.md



---

## 🤝 Contributors

- **Ehiane Kelvin Oigiagbe** – [ehiane.info](http://www.ehiane.info)  
*(Solo project for course credit)*

---

## 📄 License

MIT License – feel free to fork, contribute, and build upon this project.

namenamename

USE employee_system;

CREATE TABLE IF NOT EXISTS departments (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE IF NOT EXISTS positions (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE IF NOT EXISTS employees (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    lastname VARCHAR(100) NOT NULL,
    idcard CHAR(11) NOT NULL UNIQUE,
    department_id INT NOT NULL,
    position_id INT NOT NULL,
    FOREIGN KEY (department_id) REFERENCES departments(id),
    FOREIGN KEY (position_id) REFERENCES positions(id)
);

INSERT IGNORE INTO departments (name) VALUES
    ('IT'),
    ('Human Resources'),
    ('Finance'),
    ('Sales'),
    ('Marketing');

INSERT IGNORE INTO positions (name) VALUES
    ('Developer'),
    ('Manager'),
    ('Accountant'),
    ('Sales Representative'),
    ('Designer'),
    ('Technician');

INSERT IGNORE INTO employees (name, lastname, idcard, department_id, position_id) VALUES
    ('Michael', 'De La Rosa', '00100000001', 1, 1),
    ('Carlos', 'Martinez', '00100000002', 1, 6),
    ('Maria', 'Rodriguez', '00100000003', 2, 2),
    ('Jose', 'Perez', '00100000004', 3, 3),
    ('Ana', 'Gomez', '00100000005', 4, 4),
    ('Pedro', 'Ramirez', '00100000006', 5, 5),
    ('Laura', 'Fernandez', '00100000007', 1, 1),
    ('Luis', 'Hernandez', '00100000008', 3, 3),
    ('Sofia', 'Castillo', '00100000009', 2, 2),
    ('Daniel', 'Torres', '00100000010', 4, 4);
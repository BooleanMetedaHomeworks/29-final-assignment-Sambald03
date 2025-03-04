CREATE TABLE Categories (
    [id] INT NOT NULL identity(1,1) PRIMARY KEY,
    [name] VARCHAR(100) NOT NULL
);

CREATE TABLE Dishes
(
    [id] INT NOT NULL identity(1,1) PRIMARY KEY,
    [name] VARCHAR(100) NOT NULL,
    [description] TEXT,
    [price] DECIMAL(6,2) NOT NULL,
    [category_id] INT,

    FOREIGN KEY (category_id) REFERENCES Categories(id) ON DELETE SET NULL
)

CREATE TABLE Menus (
    [id] INT NOT NULL identity(1,1) PRIMARY KEY,
    [name] VARCHAR(100) NOT NULL
);

CREATE TABLE Menus_Dishes (
    [menu_id] INT NOT NULL,
    [dish_id] INT NOT NULL,

    PRIMARY KEY (menu_id, dish_id),

    FOREIGN KEY (menu_id) REFERENCES Menus(id) ON DELETE CASCADE,
    FOREIGN KEY (dish_id) REFERENCES Dishes(id) ON DELETE CASCADE
);

INSERT INTO Categories ([name]) VALUES ('Primo Piatto');
INSERT INTO Categories ([name]) VALUES ('Secondo Piatto');
INSERT INTO Categories ([name]) VALUES ('Contorno');

INSERT INTO Dishes ([name], [description], [price], [category_id]) VALUES ('Spaghetti al Pomodoro', 'Spaghetti con salsa di pomodoro fresco', 8.50, 1);
INSERT INTO Dishes ([name], [description], [price], [category_id]) VALUES ('Lasagna', 'Lasagna con carne e besciamella', 12.00, 1);
INSERT INTO Dishes ([name], [description], [price], [category_id]) VALUES ('Pollo alla Griglia', 'Pollo grigliato con contorno di verdure', 14.50, 2);

INSERT INTO Menus ([name]) VALUES ('Menù di Primavera');
INSERT INTO Menus ([name]) VALUES ('Menù di Natale');

INSERT INTO Menus_Dishes ([menu_id], [dish_id]) VALUES (1, 1);
INSERT INTO Menus_Dishes ([menu_id], [dish_id]) VALUES (1, 2);
INSERT INTO Menus_Dishes ([menu_id], [dish_id]) VALUES (2, 2);
INSERT INTO Menus_Dishes ([menu_id], [dish_id]) VALUES (2, 3);

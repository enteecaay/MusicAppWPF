-- Create the database
CREATE DATABASE MusicPlayListDB;
GO

-- Use the database
USE MusicPlayListDB;
GO

-- Create the Users table
CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Password NVARCHAR(255) NOT NULL
);
GO

-- Create the Songs table
CREATE TABLE Songs (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Title NVARCHAR(100) NOT NULL,
    Artist NVARCHAR(100) NOT NULL,
    Album NVARCHAR(100),
    Url NVARCHAR(255) NOT NULL,
);

GO
-- Create the Playlists table
CREATE TABLE Playlists (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    UserId INT NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);
GO

-- Create the PlaylistSongs table (many-to-many relationship between Playlists and Songs)
CREATE TABLE PlaylistSongs (
    PlaylistId INT NOT NULL,
    SongId INT NOT NULL,
    PRIMARY KEY (PlaylistId, SongId),
    FOREIGN KEY (PlaylistId) REFERENCES Playlists(Id),
    FOREIGN KEY (SongId) REFERENCES Songs(Id)
);
GO

-- Create the FavoriteMusic table
CREATE TABLE FavoriteMusic (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL,
    SongId INT NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    FOREIGN KEY (SongId) REFERENCES Songs(Id)
);